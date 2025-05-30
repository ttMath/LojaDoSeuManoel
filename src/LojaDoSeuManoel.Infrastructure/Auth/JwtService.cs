using LojaDoSeuManoel.Infrastructure.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LojaDoSeuManoel.Infrastructure.Auth;
public class JwtService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IConfiguration _config;
    private const string RefreshTokenClaimType = "token_type";
    private const string RefreshTokenClaimValue = "refresh";

    public JwtService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        _issuer = _config["JwtSettings:Issuer"]!;
        _audience = _config["JwtSettings:Audience"]!;
    }

    private string GerarTokenJwt(IEnumerable<Claim> claims, TimeSpan expiresIn)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(expiresIn),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public (string AccessToken, string RefreshToken) GerarTokens(string userName, IEnumerable<string> roles)
    {
        var userClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userName),
            new(JwtRegisteredClaimNames.Name, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in roles)
        {
            userClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessTokenExpiration = TimeSpan.FromMinutes(Convert.ToDouble(_config["JwtSettings:AccessTokenExpirationMinutes"] ?? "5"));
        var accessToken = GerarTokenJwt(userClaims, accessTokenExpiration);

        var refreshTokenClaims = new List<Claim>(userClaims)
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(RefreshTokenClaimType, RefreshTokenClaimValue)
        };

        var refreshTokenExpiration = TimeSpan.FromMinutes(Convert.ToDouble(_config["JwtSettings:RefreshTokenExpirationMinutes"] ?? "30"));
        var refreshToken = GerarTokenJwt(refreshTokenClaims, refreshTokenExpiration);

        return (accessToken, refreshToken);
    }

    public ClaimsPrincipal? ValidarTokenJwt(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}