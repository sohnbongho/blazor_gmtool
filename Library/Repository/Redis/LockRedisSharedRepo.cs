using Library.Connector;
using Library.Data.Enums;
using Library.DBTables;
using Library.DBTables.Redis;
using Library.DTO;
using Library.Helper;
using log4net;
using StackExchange.Redis;
using System.Reflection;

namespace Library.Repository.Redis;

public class LockRedisSharedRepo
{
    /// <summary>
    /// DB에 한번에 하나만 Update되게 처리
    /// </summary>
    /// <param name="userSeq"></param>
    /// <param name="tblName"></param>
    public async Task<bool> EnterLockForDbTableAsync(ulong userSeq, string tblName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.DbTableLockKey(tblName, userSeq);
        var expiry = ConstInfo.DbLockExpireDate; // 

        bool isSet = await db.StringSetAsync(lockKey, 1, expiry, When.NotExists);
        return isSet;
    }
    public async Task<bool> EnterLockForDbTablesAsync(ulong userSeq, string[] tblNames)
    {
        foreach (var tblName in tblNames)
        {
            if (false == await EnterLockForDbTableAsync(userSeq, tblName))
                return false;
        }
        return true;
    }
    public bool EnterLockForDbTable(ulong userSeq, string tblName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.DbTableLockKey(tblName, userSeq);

        var expiry = ConstInfo.DbLockExpireDate; // 

        bool isSet = db.StringSet(lockKey, 1, expiry, When.NotExists);
        return isSet;
    }
    public bool EnterLockForDbTables(ulong userSeq, string[] tblNames)
    {
        foreach (var tblName in tblNames)
        {
            if (false == EnterLockForDbTable(userSeq, tblName))
                return false;
        }
        return true;
    }

    public async Task<bool> LeaveLockForDbTableAsync(ulong userSeq, string tblName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.DbTableLockKey(tblName, userSeq);

        bool isDeleted = await db.KeyDeleteAsync(lockKey);
        return isDeleted;
    }
    public async Task<bool> LeaveLockForDbTablesAsync(ulong userSeq, string[] tblNames)
    {
        foreach (var tblName in tblNames)
        {
            await LeaveLockForDbTableAsync(userSeq, tblName);
        }

        return true;
    }
    public void LeaveLockForDbTable(ulong userSeq, string tblName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.DbTableLockKey(tblName, userSeq);

        bool isDeleted = db.KeyDelete(lockKey);
    }
    public void LeaveLockForDbTables(ulong userSeq, string[] tblNames)
    {
        foreach (var tblName in tblNames)
        {
            LeaveLockForDbTable(userSeq, tblName);
        }
    }

    /// <summary>
    /// 특정 컨트롤러가 연속으로 들어오는 문제 해결을 위해
    /// </summary>
    /// <param name="tblName"></param>
    public async Task<bool> EnterLockForControllerAsync(LoginType loginType, string accountId, string controllerName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.ControllerLockKey(loginType, accountId, controllerName);
        var expiry = ConstInfo.DbLockExpireDate; // 

        bool isSet = await db.StringSetAsync(lockKey, 1, expiry, When.NotExists);

        return isSet;
    }

    public async Task<bool> LeaveLockForControllerAsync(LoginType loginType, string accountId, string controllerName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.ControllerLockKey(loginType, accountId, controllerName);

        bool isDeleted = await db.KeyDeleteAsync(lockKey);


        return isDeleted;
    }
    

    /// <summary>
    /// DB Table을 위한 lock guard
    /// </summary>
    public async Task<bool> EnterLockForTableAsync(string tableName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.TableLockKey(tableName);
        var expiry = ConstInfo.DbLockExpireDate; // 

        bool isSet = await db.StringSetAsync(lockKey, 1, expiry, When.NotExists);

        return isSet;
    }

    public async Task<bool> LeaveLockForTableAsync(string tableName)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.DBLock;
        var db = redis.GetDatabase(dbId);
        var lockKey = RedisKeyCollection.TableLockKey(tableName);

        bool isDeleted = await db.KeyDeleteAsync(lockKey);


        return isDeleted;
    }

}
