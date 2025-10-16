using Library.DTO;

namespace Library.Repository.Mysql.Exp;

public class NoneExpRepo : IGameModeExpRepo
{
    public GameModeExpType GameModeExpType { get; set; } = GameModeExpType.None;
}
