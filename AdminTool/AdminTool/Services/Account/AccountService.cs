using AdminTool.Repositories.Account;
using System.Collections.Concurrent;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AdminTool.Services.Account;
public class AccountService : IAccountService
{
    private readonly ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();

    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> RegisterAsync(string username, string password, string userDesc)
    {
        // 사용자명이 이미 존재하는지 확인합니다.
        var affected = await _repo.RegisterAsync(username, password, userDesc);
        return affected;
    }
    public async Task<bool> ValidateLoginAsync(string username, string password)
    {
        var affected = await _repo.ValidateLoginAsync(username, password);
        return affected;
    }
    public async Task<bool> HasUserAsync(string username)
    {
        var affected = await _repo.HasUserAsync(username);
        return affected;
    }
}

