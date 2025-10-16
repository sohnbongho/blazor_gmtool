using Library.Repository.Redis;

public class RedisLockGuardAsync : IAsyncDisposable
{
    public bool Entered => _entered;
    private bool _isDisposed = false;
    private LockRedisSharedRepo _lockRedisRepo = new();

    private ulong _userSeq;
    private string []_tblNames ;
    private bool _entered;

    private RedisLockGuardAsync()
    {
        _tblNames = Array.Empty<string>();
    }

    public static async Task<RedisLockGuardAsync> Of(ulong userSeq, string tblName)
    {
        var guard = new RedisLockGuardAsync();
        await guard.EnterAsync(userSeq, tblName);
        return guard;
    }
    public static async Task<RedisLockGuardAsync> Of(ulong userSeq, string[] tblNames)
    {
        var guard = new RedisLockGuardAsync();
        await guard.EnterAsync(userSeq, tblNames);
        return guard;
    }

    private async Task<bool> EnterAsync(ulong userSeq, string tblName)
    {
        // 비동기 초기화 작업
        _entered = await _lockRedisRepo.EnterLockForDbTableAsync(userSeq, tblName);
        if (_entered)
        {
            _userSeq = userSeq;
            _tblNames = new string [] { tblName };
        }
        return _entered;
    }
    private async Task<bool> EnterAsync(ulong userSeq, string[] tblNames)
    {
        // 비동기 초기화 작업
        _entered = await _lockRedisRepo.EnterLockForDbTablesAsync(userSeq, tblNames);
        if (_entered)
        {
            _userSeq = userSeq;
            _tblNames = tblNames.ToArray();
        }
        return _entered;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isDisposed)
        {
            // 비동기 해제 작업            
            if (_tblNames.Any())
            {
                await _lockRedisRepo.LeaveLockForDbTablesAsync(_userSeq, _tblNames);
            }
            _isDisposed = true;
        }

    }
}
