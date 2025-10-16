using Dapper;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Messages;
using MySqlConnector;
using System.Text;
using ErrorCode = Library.DTO.ErrorCode;

namespace Library.Repository.Mysql.Exp;

public interface IGameModeExpSharedRepo
{
    (ErrorCode, ModeExpInfo) Increase(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, GameModeExpType gameModeExpType, long addedExp);
    (int, long) FetchGameModeExp(MySqlConnection db, ulong userSeq, GameModeExpType gameModeExpType);
}

public class GameModeExpSharedRepo : IGameModeExpSharedRepo
{
    public static GameModeExpSharedRepo Of()
    {
        return new GameModeExpSharedRepo();
    }
    private GameModeExpSharedRepo()
    {
    }

    /// <summary>
    /// 경험치 증가
    /// </summary>
    public (ErrorCode, ModeExpInfo) Increase(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, GameModeExpType gameModeExpType, long addedExp)
    {
        var instance = ExpDBLoaderHelper.Instance;

        var gameModeType = ExpDBLoaderHelper.ConvertGameModeType(gameModeExpType);
        int currentLevel = 0;
        int nextLevel = 0;
        long nextExp = 0;

        {
            var query = $"select * from tbl_inventory_gamemode_exp where user_seq = @user_seq and game_mode_id = @game_mode_id";

            var tbl = db.Query<TblInventoryGamemodeExp>(query, new TblInventoryGamemodeExp
            {
                user_seq = userSeq,
                game_mode_id = (int)gameModeExpType,
            }, transaction);
            var result = tbl.FirstOrDefault();
            if (result == null)
            {
                return (ErrorCode.DbSelectedError, new ModeExpInfo());
            }

            currentLevel = result.level;
            nextLevel = result.level;
            nextExp = result.exp + addedExp; // 다음 경험치 수치            
        }

        {
            // 유효한 레벨인지 체크
            if (false == instance.ValidateLevel(gameModeExpType, currentLevel))
                return (ErrorCode.Succeed, new ModeExpInfo());

            for (int i = 0; i < ConstInfo.MaxGameModeLevel; ++i)
            {
                var levelupExp = instance.GetLevelUpExp(gameModeExpType, nextLevel); // 레벨업하는 경험치
                if (levelupExp == 0)
                    break; // 최대 레벨 달성

                if (nextExp < levelupExp)
                    break;

                ++nextLevel; // 레벨업
                nextExp -= levelupExp; // 
                nextExp = Math.Max(nextExp, 0);
            }
        }

        if (currentLevel == nextLevel)
        {
            // 경험치 업
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_gamemode_exp SET exp = exp + @exp ");
            strBuilder.Append($"WHERE user_seq=@user_seq and game_mode_id = @game_mode_id;");

            addedExp = Math.Max(0, addedExp);
            var query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblInventoryGamemodeExp
            {
                user_seq = userSeq,
                game_mode_id = (int)gameModeExpType,
                exp = addedExp,
            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, new ModeExpInfo());
            }

            return (ErrorCode.Succeed, new ModeExpInfo
            {
                GameMode = (int)gameModeType,
                PrevLevel = currentLevel,
                CurrentLevel = nextLevel,
                CurrentExp = nextExp,
            });
        }
        else if (currentLevel < nextLevel)
        {
            // 레벨업
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_gamemode_exp SET exp = @exp, level = @level ");
            strBuilder.Append($"WHERE user_seq=@user_seq and game_mode_id = @game_mode_id;");

            var query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblInventoryGamemodeExp
            {
                exp = nextExp,
                level = nextLevel,
                user_seq = userSeq,
                game_mode_id = (int)gameModeExpType,

            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, new ModeExpInfo());
            }
            return (ErrorCode.Succeed, new ModeExpInfo
            {
                GameMode = (int)gameModeType,
                PrevLevel = currentLevel,
                CurrentLevel = nextLevel,
                CurrentExp = nextExp,
            });
        }

        // 실패
        return (ErrorCode.DbInsertedError, new ModeExpInfo());
    }

    public (int, long) FetchGameModeExp(MySqlConnection db, ulong userSeq, GameModeExpType gameModeExpType)
    {
        var query = $"select * from tbl_inventory_gamemode_exp where user_seq = @user_seq and game_mode_id = @game_mode_id";

        var tbl = db.Query<TblInventoryGamemodeExp>(query, new TblInventoryGamemodeExp
        {
            user_seq = userSeq,
            game_mode_id = (int)gameModeExpType,
        });
        var result = tbl.FirstOrDefault();
        if (result == null)
        {
            return (0, 0);
        }

        return (result.level, result.exp);
    }
}
