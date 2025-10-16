using Library.Data.Enums;

namespace Library.Repository.Redis.Lock;

public class RedisTableLockGuardAsync : IAsyncDisposable
{
    public bool Entered => _entered;

    private bool _isDisposed = false;
    private LockRedisSharedRepo _lockRedisRepo = new LockRedisSharedRepo();
    
    private string _tableName = string.Empty;
    private bool _entered = false;

    private RedisTableLockGuardAsync()
    {
    }

    public static async Task<RedisTableLockGuardAsync> Of(string tableName)
    {
        var guard = new RedisTableLockGuardAsync();
        await guard.EnterAsync(tableName);
        return guard;
    }

    private async Task<bool> EnterAsync(string tableName)
    {
        // 비동기 초기화 작업
        _entered = await _lockRedisRepo.EnterLockForTableAsync(tableName);
        if (_entered)
        {
            _tableName = tableName;            
        }
        return _entered;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isDisposed)
        {
            // 비동기 해제 작업
            if (false == string.IsNullOrEmpty(_tableName))
            {
                await _lockRedisRepo.LeaveLockForTableAsync(_tableName);
            }
            _isDisposed = true;

        }
    }
}
