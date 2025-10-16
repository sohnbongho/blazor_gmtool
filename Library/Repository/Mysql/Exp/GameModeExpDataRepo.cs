using Library.Component;
using Library.DTO;
using Library.Helper;
using Library.Repository.Mysql.SeasonPass;
using log4net;
using Messages;
using MySqlConnector;
using System.Reflection;

namespace Library.Repository.Mysql.Exp;

public class GameModeExpDataRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private SeasonPassExpSharedRepo _seasonPassRepo = SeasonPassExpSharedRepo.Of();

    public static GameModeExpDataRepo Of()
    {
        return new GameModeExpDataRepo();
    }
    private GameModeExpDataRepo()
    {

    }

    public (ErrorCode, ModeExpInfo) Increase(GameModeExpType gameModeExpType, ulong userSeq, long exp)
    {
        var instance = ExpDBLoaderHelper.Instance;
        if (false == instance.ValidateAccumulate(gameModeExpType))
        {
            // 누적할수 없는 게임모드
            return (ErrorCode.Succeed, new ModeExpInfo());
        }

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return (ErrorCode.DbUpdatedError, new ModeExpInfo());

            using var transaction = db.BeginTransaction();

            try
            {
                var (errorCode, modeExpInfo) = Increase(db, transaction, gameModeExpType, userSeq, exp);

                if (errorCode != ErrorCode.Succeed)
                {
                    transaction.Rollback();
                    return (errorCode, new ModeExpInfo());
                }

                transaction.Commit();

                return (errorCode, modeExpInfo);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Error($"failed to IncreaseCurrency", ex);
                return (ErrorCode.DbUpdatedError, new ModeExpInfo());
            }
        }
    }
    private (ErrorCode, ModeExpInfo) Increase(MySqlConnection db, MySqlTransaction transaction, GameModeExpType gameModeExpType, ulong userSeq, long exp)
    {
        var instance = ExpDBLoaderHelper.Instance;
        if (false == instance.ValidateAccumulate(gameModeExpType))
        {
            // 누적할수 없는 게임모드
            return (ErrorCode.Succeed, new ModeExpInfo());
        }

        try
        {
            var repo = ExpRepoFactory.Of(gameModeExpType);
            if (!(repo is IRewardableExp iRewardableExp))
            {
                return (ErrorCode.NotFoundGameModeExpType, new ModeExpInfo());
            }

            var (errorCode, mdoeExpInfo) = iRewardableExp.Increase(db, transaction, userSeq, exp);
            if (errorCode != ErrorCode.Succeed)
            {
                return (errorCode, new ModeExpInfo());
            }

            // 시즌 패스 경험치
            //var seasonId = ConstDbHelper.Instance.SeasonPassId();
            //var (errorCode2, seasonPassInfo) = _seasonPassRepo.Increase(db, transaction, seasonId, userSeq, exp);
            //if (errorCode2 != ErrorCode.Succeed)
            //{
            //    return (errorCode2, new ModeExpInfo(), new SeasonExpInfo());
            //}
            return (errorCode, mdoeExpInfo);
        }
        catch (Exception ex)
        {
            _logger.Error($"failed to Increase", ex);
            return (ErrorCode.DbUpdatedError, new ModeExpInfo());
        }
    }

    public (int, long) FetchGameModeExp(GameModeExpType gameModeExpType, ulong userSeq)
    {
        var instance = ExpDBLoaderHelper.Instance;
        if (false == instance.ValidateAccumulate(gameModeExpType))
        {
            // 누적할수 없는 게임모드
            return (0, 0);
        }

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return (0, 0);

            try
            {
                var repo = ExpRepoFactory.Of(gameModeExpType);
                if (!(repo is IRewardableExp iRewardableExp))
                {
                    return (0, 0);
                }

                return iRewardableExp.FetchGameModeExp(db, userSeq);
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to FetchGameModeExp", ex);
                return (0, 0);
            }
        }
    }

}
