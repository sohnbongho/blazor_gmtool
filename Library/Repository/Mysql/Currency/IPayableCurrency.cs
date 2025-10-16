using Library.DTO;
using MySqlConnector;

namespace Library.Repository.Mysql.Currency;

public interface IPayableCurrency
{
    (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice);
    Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice);
    (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice);
    Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice);
    bool HasEnough(MySqlConnection db, ulong userSeq, long itemPrice);
    bool HasEnough(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice);
    Task<bool> HasEnoughAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice);
    long FetchTotalAmount(MySqlConnection db, ulong userSeq);
    long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, ulong userSeq);
    Task<long> FetchTotalAmountAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq);
}
