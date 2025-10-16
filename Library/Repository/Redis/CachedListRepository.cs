using Library.Connector;
using Library.Data.Enums;
using Library.DBTables.Redis;
using Newtonsoft.Json;

namespace Library.Repository.Redis;

#pragma warning disable CS8602, CS8604 // null 가능 참조에 대한 역참조입니다.
public class CachedListRepository
{
    /// <summary>
    /// 키가 존재하는지 체크
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>

    public async Task<bool> HasKeyAsync(string key)
    {
        try
        {
            var redis = RedisConnectionPool.Instance.GetConnection();
            var dbId = (int)RedisEnum.DataBaseId.CacheList;
            var db = redis.GetDatabase(dbId);
            var cacheKey = RedisKeyCollection.CacheListKey(key);

            return await db.KeyExistsAsync(cacheKey);
        }
        catch (Exception)
        {
            return false;
        }

    }

    /// <summary>
    /// Redis 리스트에 여러 항목 추가
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public async Task SetListAsync<T>(string key, IEnumerable<T> values, TimeSpan expire)
    {
        try
        {
            var redis = RedisConnectionPool.Instance.GetConnection();
            var dbId = (int)RedisEnum.DataBaseId.CacheList;
            var db = redis.GetDatabase(dbId);
            var cacheKey = RedisKeyCollection.CacheListKey(key);

            // 기존 키 삭제
            await db.KeyDeleteAsync(cacheKey);

            foreach (var value in values)
            {
                var jsonValue = JsonConvert.SerializeObject(value);
                await db.ListRightPushAsync(cacheKey, jsonValue);
            }
            if (values.Any())
            {
                await db.KeyExpireAsync(cacheKey, expire);
            }
        }
        catch (Exception)
        {

        }
    }


    /// <summary>
    /// Redis 리스트에서 항목 가져오기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<List<T>> FetchListAsync<T>(string key)
    {
        // null 값 처리 및 안전한 변환
        try
        {
            var redis = RedisConnectionPool.Instance.GetConnection();
            var dbId = (int)RedisEnum.DataBaseId.CacheList;
            var db = redis.GetDatabase(dbId);
            var cacheKey = RedisKeyCollection.CacheListKey(key);

            var length = await db.ListLengthAsync(cacheKey);
            var items = await db.ListRangeAsync(cacheKey, 0, length - 1);

            var rtnList = new List<T>();
            foreach (var item in items)
            {
                if (item.IsNullOrEmpty)
                    continue;
                var json = JsonConvert.DeserializeObject<T>(item);
                if (json != null) // null 반환 방지
                {
                    rtnList.Add(json);
                }
            }
            return rtnList;
        }
        catch (Exception)
        {
            return new List<T>();
        }

    }
}

#pragma warning restore CS8602, CS8604 // null 가능 참조에 대한 역참조입니다.