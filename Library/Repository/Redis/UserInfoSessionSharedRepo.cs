using Library.Connector;
using Library.Data.Enums;
using Library.DBTables;
using Library.DBTables.Redis;
using Library.Helper;
using StackExchange.Redis;

namespace Library.Repository.Redis;

public class UserInfoSessionSharedRepo
{
    public static UserInfoSessionSharedRepo Of()
    {
        return new UserInfoSessionSharedRepo();
    }
    private UserInfoSessionSharedRepo()
    {

    }

    /// <summary>
    /// 유저 정보 가져오기
    /// </summary>
    /// <param name="sessionGuid"></param>
    /// <returns></returns>
    public async Task<RedisCommonQuery.UserSessionInfo> FetchUserSessionAsync(string sessionGuid)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Session;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.UserSessionKey(sessionGuid);

        HashEntry[] vals = await db.HashGetAllAsync(key);
        var classValue = RedisConvertHelper.HashEntriesToClass<RedisCommonQuery.UserSessionInfo>(vals);
        return classValue;
    }

    public RedisCommonQuery.UserSessionInfo FetchUserSession(string sessionGuid)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Session;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.UserSessionKey(sessionGuid);

        HashEntry[] vals = db.HashGetAll(key);
        var classValue = RedisConvertHelper.HashEntriesToClass<RedisCommonQuery.UserSessionInfo>(vals);
        return classValue;
    }
    /// <summary>
    /// Sessoin 삭제
    /// </summary>
    /// <param name="sessionGuid"></param>
    /// <returns></returns>
    public bool DeleteUserSession(string sessionGuid)
    {
        if (string.IsNullOrEmpty(sessionGuid))
            return false;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Session;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.UserSessionKey(sessionGuid);

        // 해당 키를 삭제
        return db.KeyDelete(key);
    }

    public async Task<bool> DeleteUserSessionAsync(string sessionGuid)
    {
        if (string.IsNullOrEmpty(sessionGuid))
            return false;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Session;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.UserSessionKey(sessionGuid);

        // 해당 키를 삭제
        return await db.KeyDeleteAsync(key);
    }

    /// <summary>
    /// 유저 정보 Redis 에 저장
    /// </summary>
    /// <param name="sessionGuid"></param>
    public async Task<bool> StoreUserSessionAsync(RedisCommonQuery.UserSessionInfo webUserInfo)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Session;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.UserSessionKey(webUserInfo.session_guid);

        {
            var hashEntries = RedisConvertHelper.ToHashEntries(webUserInfo);
            await db.HashSetAsync(key, hashEntries); //set하는 함수                
            await db.KeyExpireAsync(key, ConstInfo.LoginSessionRedisExpireDate);
        }

        return true;
    }


}
