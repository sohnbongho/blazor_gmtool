using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using log4net;
using System.Reflection;
using System.Text;

namespace Library.Repository.Mysql;

public class RewardSharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private readonly CurrencySharedRepo _currencyRepo = CurrencySharedRepo.Of();

    public static RewardSharedRepo Of()
    {
        return new RewardSharedRepo();
    }
    private RewardSharedRepo()
    {        
    }

    public void Clear(int serverId)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return;

            var query = $"DELETE FROM tbl_member_reward_status WHERE server_id = @server_id;";

            var affected = db.Execute(query, new TblMemberRewardStatus
            {
                server_id = serverId,
            });
        }
    }

    public ErrorCode ValidateRewaredGameEnd(string gameGuid, ulong userSeq)
    {
        if (string.IsNullOrEmpty(gameGuid))
        {
            return ErrorCode.NotGameGuid;
        }

        var query = string.Empty;
        var rewardType = RewardType.GameEnd;

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return ErrorCode.DbInitializedError;
            }
            try
            {
                var hasReward = false;
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"select * from tbl_member_reward_status ");
                    strBuilder.Append($"where user_seq = @user_seq and game_guid = @game_guid and rewared_type = @rewared_type;");
                    query = strBuilder.ToString();

                    var tbl = db.Query<TblMemberRewardStatus>(query, new TblMemberRewardStatus
                    {
                        user_seq = userSeq,
                        game_guid = gameGuid,
                        rewared_type = (int)rewardType,
                    }).FirstOrDefault();

                    hasReward = tbl != null ? true : false;
                }
                if (hasReward)
                    return ErrorCode.AlreadyReward;

                return ErrorCode.Succeed;
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to [{query}]", ex);

                return ErrorCode.DbInsertedError;
            }
        }
    }

    public (ErrorCode, long) RewaredGameEndByGold(int serverId, string gameGuid, ulong userSeq, long addedMoney)
    {
        if (string.IsNullOrEmpty(gameGuid))
        {
            return (ErrorCode.NotGameGuid, 0);
        }

        long money = 0;
        var query = string.Empty;
        var rewardType = RewardType.GameEnd;
        var currencyType = CurrencyType.Gold;

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (ErrorCode.DbInitializedError, 0);
            }
            using var transaction = db.BeginTransaction();
            try
            {
                var hasReward = false;
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"select * from tbl_member_reward_status ");
                    strBuilder.Append($"where user_seq = @user_seq and game_guid = @game_guid and rewared_type = @rewared_type;");
                    query = strBuilder.ToString();

                    var tbl = db.Query<TblMemberRewardStatus>(query, new TblMemberRewardStatus
                    {
                        user_seq = userSeq,
                        game_guid = gameGuid,
                        rewared_type = (int)rewardType,
                    }, transaction).FirstOrDefault();

                    hasReward = tbl != null ? true : false;
                }
                if (hasReward)
                {
                    transaction.Rollback();
                    return (ErrorCode.AlreadyReward, 0);
                }

                // 보상 상태 저장
                {
                    query = $"INSERT INTO tbl_member_reward_status VALUES(NULL, @server_id, @user_seq, @game_guid, @rewared_type, @currency_type, @amount, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                    var affected = db.Execute(query, new TblMemberRewardStatus
                    {
                        server_id = serverId,
                        user_seq = userSeq,
                        game_guid = gameGuid,
                        rewared_type = (int)rewardType,
                        currency_type = (short)currencyType,
                        amount = addedMoney,
                    }, transaction);

                }
                //tbl_character (머니 갱신)                    
                var (errorCode, total) = _currencyRepo.IncreaseGold(db, transaction, ItemGainReason.GamePlay, userSeq, addedMoney);
                if (errorCode != ErrorCode.Succeed)
                {
                    transaction.Rollback();
                    return (errorCode, 0);
                }

                // 골드 양을 가져온다.
                money = total;

                transaction.Commit();

                return (ErrorCode.Succeed, money);
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to [{query}]", ex);
                transaction.Rollback();

                return (ErrorCode.DbInsertedError, 0);
            }
        }
    }
    public (ErrorCode, long, long) RewaredAdvertisingByGold(int serverId, string gameGuid, ulong userSeq, RewardType bonusReward)
    {
        if (string.IsNullOrEmpty(gameGuid))
        {
            return (ErrorCode.NotGameGuid, 0, 0);
        }

        long money = 0;
        var query = string.Empty;

        var rewardType = RewardType.GameEnd;
        var currencyType = CurrencyType.Gold;
        long addedMoney = 0;

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (ErrorCode.DbInitializedError, 0, 0);
            }
            using var transaction = db.BeginTransaction();
            try
            {
                var hasReward = false;
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"select * from tbl_member_reward_status ");
                    strBuilder.Append($"where user_seq = @user_seq and game_guid = @game_guid;");
                    query = strBuilder.ToString();

                    var tbls = db.Query<TblMemberRewardStatus>(query, new TblMemberRewardStatus
                    {
                        user_seq = userSeq,
                        game_guid = gameGuid,
                    }, transaction).ToList();

                    hasReward = tbls.Any(x => x.rewared_type == (int)RewardType.Advertising
                        || x.rewared_type == (int)RewardType.Clover);
                }
                if (hasReward)
                {
                    transaction.Rollback();
                    return (ErrorCode.AlreadyReward, 0, 0);
                }

                if (bonusReward == RewardType.Clover)
                {
                    var cloverAmount = _currencyRepo.FetchTotalAmount(db, transaction, CurrencyType.Clover, userSeq);
                    if (cloverAmount <= 0)
                    {
                        transaction.Rollback();
                        return (ErrorCode.NotEnoughPrice, 0, 0);
                    }
                    //클로버 감소
                    var (errorCodeClover, totalClover) = _currencyRepo.Decrease(db, transaction,
                        ItemGainReason.Advertising, CurrencyType.Clover, userSeq, 1);
                    if (errorCodeClover != ErrorCode.Succeed)
                    {
                        transaction.Rollback();
                        return (ErrorCode.NotEnoughPrice, 0, 0);
                    }
                }

                // 전에 게임 엔드로 받은 보상
                {
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"select * from tbl_member_reward_status ");
                    strBuilder.Append($"where user_seq = @user_seq and game_guid = @game_guid and currency_type = @currency_type and rewared_type = @rewared_type;");
                    query = strBuilder.ToString();

                    var tbl = db.Query<TblMemberRewardStatus>(query, new TblMemberRewardStatus
                    {
                        user_seq = userSeq,
                        game_guid = gameGuid,
                        rewared_type = (int)rewardType,
                        currency_type = (short)currencyType,
                    }, transaction).FirstOrDefault();

                    if (tbl == null)
                    {
                        transaction.Rollback();
                        return (ErrorCode.NotFoundReward, 0, 0);
                    }
                    addedMoney = tbl.amount;
                }

                // 보상 상태 저장
                {
                    query = $"INSERT INTO tbl_member_reward_status VALUES(NULL, @server_id, @user_seq, @game_guid, @rewared_type, @currency_type, @amount, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                    var affected = db.Execute(query, new TblMemberRewardStatus
                    {
                        server_id = serverId,
                        user_seq = userSeq,
                        game_guid = gameGuid,
                        rewared_type = (int)bonusReward,
                        currency_type = (short)currencyType,
                        amount = addedMoney,
                    }, transaction);

                }
                //tbl_character (머니 갱신)                    
                var (errorCode, total) = _currencyRepo.IncreaseGold(db, transaction, ItemGainReason.Advertising, userSeq, addedMoney);
                if (errorCode != ErrorCode.Succeed)
                {
                    transaction.Rollback();

                    return (errorCode, 0, 0);
                }

                // 골드 양을 가져온다.
                money = total;

                transaction.Commit();

                return (ErrorCode.Succeed, addedMoney, money);
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to [{query}]", ex);
                transaction.Rollback();

                return (ErrorCode.DbInsertedError, 0, 0);
            }
        }
    }
}
