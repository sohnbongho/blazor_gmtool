namespace AdminTool.Services.Login;

public interface ILoginService
{
    Task<bool> Login(string username, string password);
}
