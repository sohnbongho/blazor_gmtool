using Dapper;
using Library.Component;
using Library.Connector;
using Library.Data.Enums;
using Library.DBTables.MySql;
using Library.DBTables.Redis;
using Library.DTO;
using log4net;
using StackExchange.Redis;
using System.Reflection;

namespace Library.Repository.Mysql;

public class SeasonRankSharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public static SeasonRankSharedRepo Of() => new SeasonRankSharedRepo();
    private SeasonRankSharedRepo()
    {

    }

    public bool AddRankScore(GameModeType gameModeType, ulong charSeq, ulong score)
    {
        if (0 == charSeq)
            return false;

        var query = string.Empty;
        var has = true;

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            query = $"select * from tbl_character_rank where char_seq = @char_seq and game_mode_type = @game_mode_type limit 1";

            var tblRank = db.Query<TblCharacterRank>(query, new TblCharacterRank
            {
                char_seq = charSeq,
                game_mode_type = (int)gameModeType,
            }).FirstOrDefault();
            has = (tblRank != null);

            try
            {
                if (has)
                {
                    query = $"UPDATE tbl_character_rank SET total_point = total_point + @total_point WHERE char_seq = @char_seq and game_mode_type = @game_mode_type;";
                }
                else
                {
                    query = $"INSERT INTO tbl_character_rank values (NULL, @char_seq, @game_mode_type, @total_point, Now(), Now())";
                }
                var affected = db.Execute(query, new TblCharacterRank
                {
                    char_seq = charSeq,
                    game_mode_type = (int)gameModeType,
                    total_point = score,
                });
                if (affected <= 0)
                {
                    _logger.Error($"failed to tbl_character_rank. {query}");
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to tbl_character_rank. {query}", ex);

                return false;
            }

            IncreaseForRedis(gameModeType, charSeq, (long)score);

            return true;
        }
    }
    private void IncreaseForRedis(GameModeType gameModeType, ulong charSeq, long score)
    {
        if (0 == charSeq)
            return;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        // 점수 추가
        db.SortedSetIncrement(key, member, score);
    }
    public long FetchScore(GameModeType gameModeType, ulong charSeq)
    {
        if (0 == charSeq)
            return 0;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        // 점수 추가
        var myRedisPoint = db.SortedSetScore(key, member);
        long myPoint = myRedisPoint.HasValue ? (long)myRedisPoint.Value : 0;
        return myPoint;
    }

    public async Task<long> FetchScoreAsync(GameModeType gameModeType, ulong charSeq)
    {
        if (0 == charSeq)
            return 0;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        // 점수 추가
        var myRedisPoint = await db.SortedSetScoreAsync(key, member);
        long myPoint = myRedisPoint.HasValue ? (long)myRedisPoint.Value : 0;
        return myPoint;
    }

    public async Task<bool> AddRankScoreAsync(GameModeType gameModeType, ulong charSeq, ulong score)
    {
        if (0 == charSeq)
            return false;

        var query = string.Empty;
        var has = true;

        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            query = $"select * from tbl_character_rank where char_seq = @char_seq and game_mode_type = @game_mode_type limit 1";

            var result = await db.QueryAsync<TblCharacterRank>(query, new TblCharacterRank
            {
                char_seq = charSeq,
                game_mode_type = (int)gameModeType,
            });
            var tblRank = result.FirstOrDefault();
            has = (tblRank != null);

            try
            {
                if (has)
                {
                    query = $"UPDATE tbl_character_rank SET total_point = total_point + @total_point WHERE char_seq = @char_seq and game_mode_type = @game_mode_type;";
                }
                else
                {
                    query = $"INSERT INTO tbl_character_rank values (NULL, @char_seq, @game_mode_type, @total_point, Now(), Now())";
                }
                var affected = await db.ExecuteAsync(query, new TblCharacterRank
                {
                    char_seq = charSeq,
                    game_mode_type = (int)gameModeType,
                    total_point = score,
                });
                if (affected <= 0)
                {
                    _logger.Error($"failed to tbl_character_rank. {query}");
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to tbl_character_rank. {query}", ex);

                return false;
            }

            await IncreaseForRedisAsync(gameModeType, charSeq, (long)score);

            return true;
        }
    }
    private async Task<bool> IncreaseForRedisAsync(GameModeType gameModeType, ulong charSeq, long score)
    {
        if (0 == charSeq)
            return false;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        // 점수 추가
        await db.SortedSetIncrementAsync(key, member, score);

        return true;
    }

    public void SetRankForRedis(GameModeType gameModeType, ulong charSeq, long score)
    {
        if (0 == charSeq)
            return;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        // 점수 추가
        db.SortedSetAdd(key, member, score);
    }

    public long FetchRank(GameModeType gameModeType, ulong charSeq)
    {
        if (0 == charSeq)
            return -1;

        var redis = RedisConnectionPool.Instance.GetConnection();
        var dbId = (int)RedisEnum.DataBaseId.Rank;
        var db = redis.GetDatabase(dbId);
        var key = RedisKeyCollection.RankingKey(gameModeType);
        var member = RedisKeyCollection.RankingMember(charSeq);

        // 내 랭킹 점수
        var myRedisRank = db.SortedSetRank(key, member, order: Order.Descending);
        long myRank = myRedisRank != null ? myRedisRank.Value : -1;
        return myRank;
    }
}
