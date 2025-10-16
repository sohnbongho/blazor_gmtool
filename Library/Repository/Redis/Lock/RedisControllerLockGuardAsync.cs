using Library.DTO;

namespace Library.Repository.Redis.Lock;

public class RedisControllerLockGuardAsync : IAsyncDisposable
{
    public bool Entered => _entered;

    private bool _isDisposed = false;
    private LockRedisSharedRepo _lockRedisRepo = new();

    private LoginType _loginType;
    private string _accountId = string.Empty;
    private string _controllerName = string.Empty;
    private bool _entered = false;

    private RedisControllerLockGuardAsync()
    {
    }

    public static async Task<RedisControllerLockGuardAsync> Of(LoginType loginType, string accountId, string controllerName)
    {
        var guard = new RedisControllerLockGuardAsync();
        await guard.EnterAsync(loginType, accountId, controllerName);
        return guard;
    }

    private async Task<bool> EnterAsync(LoginType loginType, string accountId, string controllerName)
    {
        // 비동기 초기화 작업
        _entered = await _lockRedisRepo.EnterLockForControllerAsync(loginType, accountId, controllerName);
        if (_entered)
        {
            _loginType = loginType;
            _accountId = accountId;
            _controllerName = controllerName;
        }
        return _entered;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isDisposed)
        {
            // 비동기 해제 작업
            if (false == string.IsNullOrEmpty(_controllerName))
            {
                await _lockRedisRepo.LeaveLockForControllerAsync(_loginType, _accountId, _controllerName);
            }
            _isDisposed = true;

        }
    }
}
