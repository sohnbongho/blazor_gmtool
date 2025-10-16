using System.Security.Claims;

namespace AdminTool.Services.Login;

public interface ITokenService
{
    string GenerateToken(string username, string role);
    ClaimsPrincipal ReadToken(string token);
}
