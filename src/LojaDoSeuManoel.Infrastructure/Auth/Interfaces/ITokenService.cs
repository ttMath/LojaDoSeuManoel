using System.Security.Claims;

namespace LojaDoSeuManoel.Infrastructure.Auth.Interfaces
{
    public interface ITokenService
    {
        (string AccessToken, string RefreshToken) GerarTokens(string userName, IEnumerable<string> roles);
        ClaimsPrincipal? ValidarTokenJwt(string token);
    }
}
