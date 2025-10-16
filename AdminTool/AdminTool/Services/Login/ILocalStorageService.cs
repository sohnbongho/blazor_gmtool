using System.Threading.Tasks;
namespace AdminTool.Services.Login;
public interface ILocalStorageService
{
    Task SetItemAsync(string key, string value);
    Task<string> GetItemAsync(string key);
    Task RemoveItemAsync(string key);
}
