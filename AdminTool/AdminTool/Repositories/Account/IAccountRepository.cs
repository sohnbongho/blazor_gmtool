using DocumentFormat.OpenXml.Spreadsheet;
using Library.messages;
using ThirdParty.Json.LitJson;

namespace AdminTool.Repositories.Account;

public interface IAccountRepository
{
    Task<bool> RegisterAsync(string username, string password, string user_desc);
    Task<bool> ValidateLoginAsync(string username, string password);
    Task<bool> HasUserAsync(string username);
}
