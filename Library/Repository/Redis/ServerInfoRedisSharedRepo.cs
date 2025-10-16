using Library.Connector;
using Library.Data.Enums;
using Library.DBTables.Redis;
using Library.Helper;

namespace Library.Repository.Redis;

public class ServerInfoRedisSharedRepo
{
    /// <summary>
    /// Server 유저수
    /// </summary>
    /// <param name="serverId"></param>
    /// <returns></returns>
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

    public void ClearServerUserCount(int serverId)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.ServerStatus;
        var db = redis.GetDatabase(dbId);

        var key = RedisKeyCollection.ServerUserCountKey(serverId);
        db.StringSet(key, 0); //set하는 함수
        db.KeyExpire(key, ConstInfo.DefaultRedisExpireDate); // HashSet의 만료 시간을 30일로
    }
    public void IncreaseServerUserCount(int serverId)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.ServerStatus;
        var db = redis.GetDatabase(dbId);

        var key = RedisKeyCollection.ServerUserCountKey(serverId);
        db.StringIncrement(key); //set하는 함수
        db.KeyExpire(key, ConstInfo.DefaultRedisExpireDate); // HashSet의 만료 시간을 30일로
    }

    public void DecreaseServerUserCount(int serverId)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.ServerStatus;
        var db = redis.GetDatabase(dbId);

        var key = RedisKeyCollection.ServerUserCountKey(serverId);
        db.StringDecrement(key); //set하는 함수
        db.KeyExpire(key, ConstInfo.DefaultRedisExpireDate); // HashSet의 만료 시간을 30일로
    }


}
