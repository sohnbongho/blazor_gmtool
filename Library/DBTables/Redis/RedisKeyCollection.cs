using Library.Data.Enums;
using Library.DTO;

namespace Library.DBTables.Redis;

public static class RedisKeyCollection
{
    public static string RankingKey(GameModeType gameModeType) => $"ranking:{(int)gameModeType}";
    public static string RankingBackupKey(GameModeType gameModeType) => $"rankingbackup:{(int)gameModeType}";
    public static string RankingBackupDateTime => $"rankingbackuptime";
    public static string RankingMember(ulong charSeq) => $"character:{charSeq}";

    public static string UserSessionKey(string sessionGuid) => $"usersession:{sessionGuid}";

    public static string DbTableLockKey(string tblName, ulong userSeq) => $"dbtablelock:{tblName}:{userSeq}";
    public static string ControllerLockKey(LoginType loginType, string accountId, string controllerName) => $"controllerlock:{controllerName}:{(int)loginType}:{accountId}";
    public static string TableLockKey(string tableLockKey) => $"dbtablelock:{tableLockKey}";

    public static string ServerUserCountKey(int serverId) => $"serverusercount:{serverId}";
    // 존 유저 수
    public static string ZoneUserCountKey(int mapIndex, int zoneId) => $"zoneusercount:{mapIndex}:{zoneId}";

    // 전체 캐시에 대한 키
    public static string CacheListKey(string key) => $"cache:{key}";


    public static string LikedArticles(DateTime now, int limit)
    {
        var minute = now.Minute / 10; // 10분에 한번씩 갱신하기 위해

        return $"likedarticles:{now.Year}{now.Month}{now.Day}{now.Hour}{minute}:{limit}";
    }

    public static string OpenedItemBoxInfo(ulong charSeq, int mapId) => $"openditembox:{charSeq}:{mapId}";
}
