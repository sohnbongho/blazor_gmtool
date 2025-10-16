using Dapper;
using Library.Component;
using Library.Connector;
using Library.Data.Enums;
using Library.DBTables.MySql;
using Library.DBTables.Redis;
using Library.DTO;
using Library.messages;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace AdminTool.Repositories.Rank;

public interface IRankRepository
{
    Task<List<RankInfoData>> FetchRankCharsAsync(GameModeType gameModeType, int rankRange);
    Task<List<TblCharacter>> FetchCharsAsync(List<ulong> charSeqs);

    Task<bool> DeleteRankCharAsync(GameModeType gameModeType, ulong charSeq);
    Task<bool> InitialOceanUpInfoAsync(ulong charSeq);
}

public class RankRepository : MySqlDbCommonRepo, IRankRepository
{
    public async Task<List<RankInfoData>> FetchRankCharsAsync(GameModeType gameModeType, int rankRange)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);

        var rankList = new List<RankInfoData>();

        // 1~10등까지 순위
        var ranking = await db.SortedSetRangeByRankWithScoresAsync(key, 0, rankRange - 1, Order.Descending);
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

    public async Task<List<TblCharacter>> FetchCharsAsync(List<ulong> charSeqs)
    {
        if (charSeqs.Any() == false)
            return new List<TblCharacter>();

        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
                return new List<TblCharacter>();

            var charSeqsStr = string.Join(",", charSeqs);
            var query = $"SELECT * from tbl_character WHERE char_seq in ({charSeqsStr});";
            var result = await db.QueryAsync<TblCharacter>(query);
            var tbls = result.ToList();

            return tbls;
        }
    }
    public async Task<bool> DeleteRankCharAsync(GameModeType gameModeType, ulong charSeq)
    {
        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        return await db.SortedSetRemoveAsync(key, member);
    }
    public async Task<bool> InitialOceanUpInfoAsync(ulong charSeq)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
                return false;

            var query = $"update tbl_oceanup_user_status set max_floor_reached =0, reached_second = 0 where char_seq = @char_seq";
            var affected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
            });

            return affected > 0;
        }
    }
}
