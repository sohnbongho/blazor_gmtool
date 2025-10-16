using Library.Connector;
using Library.Data.Enums;
using Library.DBTables.MySql;
using Library.DBTables.Redis;
using Library.DTO;
using Library.Helper;
using Library.messages;
using Library.Repository.Mysql;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace Library.Repository.Redis;

public class UserRankSharedRepo
{
    private readonly UserSharedRepo _userRepository = UserSharedRepo.Of();
    public List<RankInfoData> FetchRank(GameModeType gameModeType, int rankRange)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);

        var rankList = new List<RankInfoData>();

        // 1~10등까지 순위
        var ranking = db.SortedSetRangeByRankWithScores(key, 0, rankRange - 1, Order.Descending);
        for (int i = 0; i < ranking.Length; i++)
        {
            var element = ranking[i].Element.ToString();
            var score = long.Parse(ranking[i].Score.ToString());

            ulong rankCharSeq = 0;
            var match = Regex.Match(element, @"\d+");
            if (match.Success)
            {
                rankCharSeq = ulong.Parse(match.Value);
            }

            rankList.Add(new RankInfoData
            {
                CharSeq = rankCharSeq,
                Score = score,
            });
        }

        return rankList;
    }
    public Task<TblMember> FetchMemberByCharSeqAsync(ulong charSeq)
    {
        return _userRepository.FetchMemberByCharSeqAsync(charSeq);
    }
    public DateTime FetchBackupDateTime()
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);

        var key = RedisKeyCollection.RankingBackupDateTime;
        var value = db.StringGet(key); //set하는 함수        
        if (false == value.HasValue)
        {
            return DateTime.MinValue;
        }
        var getValue = value.ToString();
        var (getted, dateTime) = ConvertHelper.StringToTime(getValue);
        if (getted == false)
        {
            return DateTime.MinValue;
        }
        return dateTime;
    }
    public void UpdateBackupDateTime(DateTime now)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);

        var key = RedisKeyCollection.RankingBackupDateTime;
        var dateTimeString = ConvertHelper.TimeToString(now);
        db.StringSet(key, dateTimeString); //set하는 함수                
    }

    public void BackupRank(int rankRange)
    {
        for (var i = (int)GameModeType.None + 1; i < (int)GameModeType.Max; ++i)
        {
            var redis = RedisConnectionPool.Instance.GetConnection();
            var dbId = (int)RedisEnum.DataBaseId.Rank;
            var db = redis.GetDatabase(dbId);

            var gameModeType = ConvertHelper.ToEnum<GameModeType>(i);
            var rankInfos = FetchRank(gameModeType, rankRange);

            var key = RedisKeyCollection.RankingBackupKey(gameModeType);
            // 기존 랭킹 데이터 삭제
            db.KeyDelete(key);

            foreach (var rankInfo in rankInfos)
            {
                var charSeq = rankInfo.CharSeq;
                var score = rankInfo.Score;

                var member = RedisKeyCollection.RankingMember(charSeq);

                // 점수 추가            
                db.SortedSetIncrement(key, member, score);
            }
        }

    }
}
