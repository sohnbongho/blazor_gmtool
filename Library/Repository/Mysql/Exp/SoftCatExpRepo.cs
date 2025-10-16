using Library.DTO;
using Messages;
using MySqlConnector;

namespace Library.Repository.Mysql.Exp;


public class SoftCatExpRepo : IGameModeExpRepo, IRewardableExp
{
    public GameModeExpType GameModeExpType { get; set; } = GameModeExpType.SoftCat;
    private readonly IGameModeExpSharedRepo _expCommonRepo;
    public static SoftCatExpRepo Of(IGameModeExpSharedRepo expCommonRepo)
    {
        return new SoftCatExpRepo(expCommonRepo);
    }
    private SoftCatExpRepo(IGameModeExpSharedRepo expCommonRepo)
    {
        _expCommonRepo = expCommonRepo;
    }

    public (ErrorCode, ModeExpInfo) Increase(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long addedExp)
    {
        return _expCommonRepo.Increase(db, transaction, userSeq, GameModeExpType, addedExp);
    }
    public (int, long) FetchGameModeExp(MySqlConnection db, ulong userSeq)
    {
        return _expCommonRepo.FetchGameModeExp(db, userSeq, GameModeExpType);
    }
}