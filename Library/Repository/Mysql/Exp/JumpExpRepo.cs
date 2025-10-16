using Library.DTO;
using Messages;
using MySqlConnector;

namespace Library.Repository.Mysql.Exp;

public class JumpExpRepo : IGameModeExpRepo, IRewardableExp
{
    public GameModeExpType GameModeExpType { get; set; } = GameModeExpType.OceanUp;
    private readonly IGameModeExpSharedRepo _expCommonRepo;
    public static JumpExpRepo Of(IGameModeExpSharedRepo expCommonRepo)
    {
        return new JumpExpRepo(expCommonRepo);
    }
    private JumpExpRepo(IGameModeExpSharedRepo expCommonRepo)
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