namespace AdminTool.Services.Account;

public interface IAccountService
{
    Task<bool> RegisterAsync(string username, string password, string userDesc);
    Task<bool> ValidateLoginAsync(string username, string password);
    Task<bool> HasUserAsync(string username);
}
