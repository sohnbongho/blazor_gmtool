using Library.DTO;
using Library.Repository.Mysql.Exp;

namespace Library.Repository.Mysql;

public static class ExpRepoFactory
{
    private static readonly Dictionary<GameModeExpType, IGameModeExpRepo> _exps;

    static ExpRepoFactory()
    {
        _exps = new Dictionary<GameModeExpType, IGameModeExpRepo>
        {
            { GameModeExpType.None, new NoneExpRepo() },
            { GameModeExpType.DiggingWarrior, DiggingWarriorExpRepo.Of(GameModeExpSharedRepo.Of()) },
            { GameModeExpType.BubblePop, BubblePopExpRepo.Of(GameModeExpSharedRepo.Of()) },
            { GameModeExpType.CutShroom, CutShroomExpRepo.Of(GameModeExpSharedRepo.Of()) },
            { GameModeExpType.Cleaning, CleaningExpRepo.Of(GameModeExpSharedRepo.Of()) },
            { GameModeExpType.SoftCat, SoftCatExpRepo.Of(GameModeExpSharedRepo.Of()) },
            { GameModeExpType.OceanUp, JumpExpRepo.Of(GameModeExpSharedRepo.Of()) },
        };
    }

    public static IGameModeExpRepo Of(GameModeExpType type)
    {
        return _exps.TryGetValue(type, out var repo) ? repo : _exps[GameModeExpType.None];
    }
}
