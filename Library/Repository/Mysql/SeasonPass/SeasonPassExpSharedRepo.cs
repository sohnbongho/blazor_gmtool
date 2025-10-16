using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using log4net;
using Messages;
using MySqlConnector;
using System.Reflection;
using System.Text;

namespace Library.Repository.Mysql.SeasonPass;

public class SeasonPassExpSharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    public static SeasonPassExpSharedRepo Of() => new SeasonPassExpSharedRepo();
    private SeasonPassExpSharedRepo()
    {

    }

    public (ErrorCode, SeasonExpInfo) Increase(int seaonId, ulong userSeq, long exp)
    {

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return (ErrorCode.DbUpdatedError, new SeasonExpInfo());

            using var transaction = db.BeginTransaction();

            try
            {
                var (errorCode, seasonInfo) = Increase(db, transaction, seaonId, userSeq, exp);

                if (errorCode != ErrorCode.Succeed)
                {
                    transaction.Rollback();
                    return (errorCode, new SeasonExpInfo());
                }

                transaction.Commit();

                return (errorCode, seasonInfo);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Error($"failed to IncreaseCurrency", ex);
                return (ErrorCode.DbUpdatedError, new SeasonExpInfo());
            }
        }
    }

    public (ErrorCode, SeasonExpInfo) Increase(MySqlConnection db, MySqlTransaction transaction, int seaonId, ulong userSeq, long exp)
    {
        var instance = ExpDBLoaderHelper.Instance;

        int currentLevel = 0;
        int nextLevel = 0;
        long nextExp = 0;

        {
            var query = $"select * from tbl_inventory_seasonpass_exp where user_seq = @user_seq and season_id = @season_id";

            var tbl = db.Query<TblInventorySeasonpassExp>(query, new TblInventorySeasonpassExp
            {
                user_seq = userSeq,
                season_id = seaonId,
            }, transaction);
            var result = tbl.FirstOrDefault();
            if (result == null)
            {
                return (ErrorCode.DbSelectedError, new SeasonExpInfo());
            }

            currentLevel = result.level;
            nextLevel = result.level;
            nextExp = result.exp + exp; // 다음 경험치 수치            
        }

        if (currentLevel == nextLevel)
        {
            // 경험치 업
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_seasonpass_exp SET exp = exp + @exp ");
            strBuilder.Append($"WHERE user_seq=@user_seq and season_id = @season_id;");

            exp = Math.Max(0, exp);
            var query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblInventorySeasonpassExp
            {
                user_seq = userSeq,
                season_id = seaonId,
                exp = exp,
            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, new SeasonExpInfo());
            }

            return (ErrorCode.Succeed, new SeasonExpInfo
            {
                SeasonId = seaonId,
                PrevLevel = currentLevel,
                CurrentLevel = nextLevel,
                CurrentExp = nextExp,
            });
        }
        else if (currentLevel < nextLevel)
        {
            // 레벨업
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_seasonpass_exp SET exp = @exp, level = @level ");
            strBuilder.Append($"WHERE user_seq=@user_seq and season_id = @season_id;");

            var query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblInventorySeasonpassExp
            {
                exp = nextExp,
                level = nextLevel,
                user_seq = userSeq,
                season_id = seaonId,

            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, new SeasonExpInfo());
            }
            return (ErrorCode.Succeed, new SeasonExpInfo
            {
                SeasonId = seaonId,
                PrevLevel = currentLevel,
                CurrentLevel = nextLevel,
                CurrentExp = nextExp,
            });
        }

        // 실패
        return (ErrorCode.DbInsertedError, new SeasonExpInfo());
    }
}
