using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using MySqlConnector;
using System.Collections.Concurrent;

namespace Library.Helper;

public class ConstDbHelper
{
    private static readonly Lazy<ConstDbHelper> lazy = new Lazy<ConstDbHelper>(() => new ConstDbHelper());

    public static ConstDbHelper Instance { get { return lazy.Value; } }

    private ConcurrentDictionary<string, int> _tblDesignConst = new();

    public bool Load()
    {
        using (var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design))
        {
            if (db == null)
                throw new Exception($"db is null");

            var (succeed1, errorStr1) = LoadDesignConst(db);
            if (false == succeed1)
                throw new Exception(errorStr1);

        }

        return true;
    }

    public (bool, string) LoadDesignConst(MySqlConnection db)
    {
        try
        {
            var query = $"select * from tbl_design_const";

            var objects = db.Query<TblDesignConst>(query);
            foreach (var obj in objects)
            {
                var key = obj.key;
                var value = obj.value;

                _tblDesignConst[key] = value;
            }
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public int GetValue(string key, int defaultValue = 1)
    {
        if (false == _tblDesignConst.TryGetValue(key, out var value))
        {
            return defaultValue;
        }
        if (value <= 0)
        {
            return defaultValue;
        }

        return value;
    }

    public long ScorePerGold(GameModeType gameModeType)
    {
        if (gameModeType == GameModeType.BubblePop)
        {
            if (false == _tblDesignConst.TryGetValue("BubblePopScoreRatio", out var value))
            {
                return ConstInfo.ScorePerGold;
            }
            if (value <= 0)
            {
                return 1;
            }
            return (long)value;
        }
        else if (gameModeType == GameModeType.CutShroom)
        {
            if (false == _tblDesignConst.TryGetValue("CutShroomScoreRatio", out var value))
            {
                return ConstInfo.ScorePerGold;
            }
            if (value <= 0)
            {
                return 1;
            }
            return (long)value;
        }
        return 1;
    }

    public int DiggingWarriorSnowPerGold => GetValue("DiggingWarriorSnowPerGold");
    public int DiggingWarriorSnowRepair => GetValue("DiggingWarriorSnowRepair");
    public int DiggingWarriorSnowHealth => GetValue("DiggingWarriorSnowHealth");
    public long GoldPerBell => GetValue("GoldPerBell");
    //[청소반장] 플레이어의 미션 완료 보상 가격
    public long PurgerMissonGold => GetValue("PurgerMissonGold");
    //청소반장 클리너가 플레이어 잡을 시 보상 가격
    public long PurgerPlayerCatchGold => GetValue("PurgerPlayerCatchGold");
    public int SeasonPassId => GetValue("SeasonPassId");
    public int MindAnalyzerClover => GetValue("MindAnalyzerClover"); // 심리테스트 결과보기 시 클로버 소비량

    public int GrowClover => GetValue("GrowClover"); // 쑥쑥 클로버 완료 시 지급 개수
    public int DailyStampClover => GetValue("DailyStampClover"); // 데일리 스탬프 스킵을 위한 클로버 사용 개수
    public int DailyStampDia => GetValue("DailyStampDia"); // 데일리 스탬프 스킵을 위한 다이아 사용 개수
    public int PurgerReviveClover => GetValue("PurgerReviveClover"); // 청소반장 부활을 위한 클로버 사용개수
    public int PurgerEndReward5 => GetValue("PurgerEndReward5"); // 청소반장 단체보상(5분 풀로 했을 시)
    public int PurgerEndReward4 => GetValue("PurgerEndReward4"); // 청소반장 단체보상(4분)
    public int PurgerEndReward3 => GetValue("PurgerEndReward3"); // 청소반장 단체보상(3분)
    public int PurgerEndReward2 => GetValue("PurgerEndReward2"); // 청소반장 단체보상(2분)
    public int PurgerEndReward1 => GetValue("PurgerEndReward1"); // 청소반장 단체보상(2분)
    public int PurgerSteamReviveTimeSec => GetValue("PurgerSteamReviveTime", 30); // 스팀 부활시간

    // 말랑 캣
    public int MallangCat_Normal => GetValue("MallangCat_Normal", 0); 
    public int MallangCat_Tiger => GetValue("MallangCat_Tiger", 0); // 
    public int MallangCat_Rainbow => GetValue("MallangCat_Rainbow", 0); // 

    // 삽투사
    public int DiggingWarriorIceLegacyHealth => GetValue("DiggingWarriorIceLegacyHealth", 50); // 
    public int DiggingWarriorIceLegacySpawn => GetValue("DiggingWarriorIceLegacySpawn", 10); // 
}
