using Dapper;
using Library.Connector;
using Library.Data.Enums;
using Library.DBTables.MySql;
using Library.DBTables.Redis;
using Library.DTO;
using log4net;
using System.Reflection;

namespace AdminTool.Repositories.ConcurrentUser;

public interface IConcurrentUserRepository
{
    Task<List<TblLogConcurrentUser>> FetchConcurrentUserLogsAsync();
    Task<int> FetchServerUserCountAsync(int serverId);
    Task<List<TblServerList>> FetchServerInfosAsync();
}
public class ConcurrentUserRepository : IConcurrentUserRepository
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    public async Task<List<TblLogConcurrentUser>> FetchConcurrentUserLogsAsync()
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogConcurrentUser>();
            }
            var query = $"SELECT * FROM tbl_log_concurrent_user order by id desc limit 1000;";
            var reuslt = await db.QueryAsync<TblLogConcurrentUser>(query);
            return reuslt.ToList();
        }
        catch (Exception ex)
        {
            _logger.Error("fail to GetLogConcurrentUsersAsync", ex);
            return new List<TblLogConcurrentUser>();
        }
    }
    public async Task<int> FetchServerUserCountAsync(int serverId)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.ServerStatus;
        var db = redis.GetDatabase(dbId);

        var key = RedisKeyCollection.ServerUserCountKey(serverId);
        var value = await db.StringGetAsync(key); //set하는 함수
        if (false == value.HasValue)
        {
            return 0;
        }
        return (int)value;
    }
    public async Task<List<TblServerList>> FetchServerInfosAsync()
    {
        await using (var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.System))
        {
            if(db == null)
                return new List<TblServerList> { };

            var query = $"select * from tbl_server_list";
            var result = await db.QueryAsync<TblServerList>(query);

            return result.ToList();
        }
    }
}
