using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Repository.Mysql.Exp;
using MySqlConnector;
using System.Collections.Concurrent;

namespace Library.Helper;

public class ExpDBLoaderHelper
{
    private static readonly Lazy<ExpDBLoaderHelper> lazy = new Lazy<ExpDBLoaderHelper>(() => new ExpDBLoaderHelper());

    public static ExpDBLoaderHelper Instance { get { return lazy.Value; } }

    // 경험치 정보
    public ConcurrentDictionary<GameModeExpType, ConcurrentDictionary<int, long>> TblDesignGamestageLevelExp => _tblDesignGamestageLevelExp;
    private ConcurrentDictionary<GameModeExpType, ConcurrentDictionary<int, long>> _tblDesignGamestageLevelExp = new();

    // 경헙치 종류
    public ConcurrentDictionary<int, TblDesignExpType> TblDesignExpType => _tblDesignExpType;
    private ConcurrentDictionary<int, TblDesignExpType> _tblDesignExpType = new();
        
    private ConcurrentDictionary<GameModeExpType, int> _gameModeExpTypes = new();

    private ExpDBLoaderHelper()
    {

    }

    public bool Load()
    {
        using (var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design))
        {
            if (db == null)
                return false;

            if (false == LoadTblDesignGamestageLevelExp(db))
                return false;

            if (false == LoadTblDesignExpType(db))
                return false;

        }

        return true;
    }

    public bool LoadTblDesignGamestageLevelExp(MySqlConnection db)
    {
        var query = $"select * from tbl_design_gamestage_level_exp";

        var objects = db.Query<TblDesignGamestageLevelExp>(query);
        foreach (var obj in objects)
        {
            var level = obj.level;
            var levelExpRequired = obj.level_exp_required;

            var gameModeType = ConvertHelper.ToEnum<GameModeExpType>(obj.game_mode_id);
            if (false == _tblDesignGamestageLevelExp.ContainsKey(gameModeType))
            {
                _tblDesignGamestageLevelExp.TryAdd(gameModeType, new ConcurrentDictionary<int, long>());
            }

            if (_tblDesignGamestageLevelExp[gameModeType].ContainsKey(level))
            {
                // 중복되는 경험치
                return false;
            }
            _tblDesignGamestageLevelExp[gameModeType][level] = levelExpRequired;
        }

        return true;
    }

    public bool LoadTblDesignExpType(MySqlConnection db)
    {
        var query = $"select * from tbl_design_exp_type";

        var objects = db.Query<TblDesignExpType>(query);
        foreach (var obj in objects)
        {
            _tblDesignExpType[obj.item_sn] = obj;

            var gameModeType = ConvertHelper.ToEnum<GameModeExpType>(obj.game_mode_id);
            _gameModeExpTypes[gameModeType] = obj.item_sn;
        }

        return true;
    }
    

    public static GameModeExpType ConvertGameModeExpType(GameModeType gameModeType)
    {
        return gameModeType switch
        {
            GameModeType.None => GameModeExpType.None,
            GameModeType.DiggingWarrior => GameModeExpType.DiggingWarrior,
            GameModeType.BubblePop => GameModeExpType.BubblePop,
            GameModeType.CutShroom => GameModeExpType.CutShroom,
            GameModeType.Cleaning => GameModeExpType.Cleaning,
            GameModeType.SoftCat => GameModeExpType.SoftCat,
            GameModeType.OceanUp => GameModeExpType.OceanUp,
            _ => GameModeExpType.None
        };
    }
    public static GameModeType ConvertGameModeType(GameModeExpType gameModeExpType)
    {
        return gameModeExpType switch
        {
            GameModeExpType.None => GameModeType.None,
            GameModeExpType.DiggingWarrior => GameModeType.DiggingWarrior,
            GameModeExpType.BubblePop => GameModeType.BubblePop,
            GameModeExpType.CutShroom => GameModeType.CutShroom,
            GameModeExpType.Cleaning => GameModeType.Cleaning,
            GameModeExpType.SoftCat => GameModeType.SoftCat,
            GameModeExpType.OceanUp => GameModeType.OceanUp,
            _ => GameModeType.None
        };
    }
    public bool ValidateAccumulate(GameModeExpType gameModeExpType)
    {
        return _tblDesignGamestageLevelExp.ContainsKey(gameModeExpType);
    }
    public bool ValidateLevel(GameModeExpType gameModeExpType, int level)
    {
        if (false == _tblDesignGamestageLevelExp.TryGetValue(gameModeExpType, out var gameModeLevels))
            return false;

        if (false == gameModeLevels.TryGetValue(level, out var levelUpExp))
            return false;

        return true;
    }

    public long GetLevelUpExp(GameModeExpType gameModeExpType, int level)
    {
        if (false == _tblDesignGamestageLevelExp.TryGetValue(gameModeExpType, out var gameModeLevels))
            return 0;
        
        if(false == gameModeLevels.TryGetValue(level, out var levelUpExp))
            return 0;

        return levelUpExp;
    }
}
