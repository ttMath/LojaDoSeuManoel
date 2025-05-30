using LojaDoSeuManoel.Infrastructure.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LojaDoSeuManoel.Api.Controllers;

public record TestTokenRequest(string FraseSecreta);
public record TokenResponse(string AccessToken, DateTime AccessTokenExpiration, string RefreshToken, string Usuario, IEnumerable<string> Papeis);
public record RefreshTokenApiRequest(string RefreshToken);


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private const string TestUserName = "matheus.rodrigues@l2code.com.br";
    private const string RefreshTokenClaimType = "token_type";
    private const string RefreshTokenClaimValue = "refresh";

    public AuthController(
        ITokenService tokenService,
        IConfiguration configuration
    )
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpPost("gerar-token")]
    [AllowAnonymous]
    public IActionResult GerarTokenTeste([FromBody] TestTokenRequest request)
    {
        var fraseSecretaConfigurada = _configuration["AppSettings:FraseSecretaParaToken"];

        if (string.IsNullOrEmpty(fraseSecretaConfigurada))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro de configuração no servidor." });
        }
        if (request == null || request.FraseSecreta != fraseSecretaConfigurada)
        {
            return Unauthorized(new { message = "Frase secreta inválida." });
        }

        var testUserRoles = new List<string>();
        var (accessToken, refreshTokenString) = _tokenService.GerarTokens(TestUserName, testUserRoles);

        var accessTokenExpirationMinutes = Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "1");
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);

        return Ok(new TokenResponse(accessToken, accessTokenExpiresAt, refreshTokenString, TestUserName, testUserRoles));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public IActionResult Refresh([FromBody] RefreshTokenApiRequest refreshTokenRequest)
    {
        if (refreshTokenRequest == null || string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
        {
            return BadRequest(new { message = "Refresh token é obrigatório." });
        }

        var principal = _tokenService.ValidarTokenJwt(refreshTokenRequest.RefreshToken);

        if (principal?.Identity?.Name == null)
        {
            return BadRequest(new { message = "Refresh token inválido ou expirado." });
        }

        if (!principal.HasClaim(c => c.Type == RefreshTokenClaimType && c.Value == RefreshTokenClaimValue))
        {
            return BadRequest(new { message = "Tipo de token inválido para refresh." });
        }

        var userName = principal.Identity.Name;

        var roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        var (newAccessToken, newRefreshTokenString) = _tokenService.GerarTokens(userName, roles);

        var accessTokenExpirationMinutes = Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "1");
        var newAccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes);

        return Ok(new TokenResponse(newAccessToken, newAccessTokenExpiresAt, newRefreshTokenString, userName, roles));
    }
}