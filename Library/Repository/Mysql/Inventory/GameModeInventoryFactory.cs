using Library.DTO;

namespace Library.Repository.Mysql.Inventory;

public static class GameModeInventoryFactory
{
    private static readonly Dictionary<GameModeType, IGameModeInventory> _gameModes;
    static GameModeInventoryFactory()
    {
        _gameModes = new Dictionary<GameModeType, IGameModeInventory>
        {
            { GameModeType.None, new NoneGameModeInventory() },
            { GameModeType.DiggingWarrior, DiggingWarriorInventory.Of()},
            { GameModeType.SoftCat , SoftCatInventory.Of()}
        };
    }
    public static IGameModeInventory Of(GameModeType type)
    {
        return _gameModes.TryGetValue(type, out var repo) ? repo : _gameModes[GameModeType.None];
    }
}
