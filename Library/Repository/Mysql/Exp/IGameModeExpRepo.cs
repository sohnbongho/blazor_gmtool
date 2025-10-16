using Library.DTO;
using Messages;
using MySqlConnector;

namespace Library.Repository.Mysql.Exp;

public interface IGameModeExpRepo
{
    GameModeExpType GameModeExpType { get; }
}
public interface IRewardableExp
{    
    (ErrorCode, ModeExpInfo) Increase(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long exp);
    (int, long) FetchGameModeExp(MySqlConnection db, ulong userSeq);
}
