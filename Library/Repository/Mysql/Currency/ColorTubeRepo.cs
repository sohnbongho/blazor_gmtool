using Library.DTO;
using Library.Helper;
using MySqlConnector;

namespace Library.Repository.Mysql.Currency;

public class ColorTubeRepo : ICurrencyRepo, IPayableCurrency
{
    public CurrencyType CurrencyType { get; } = CurrencyType.ColorTube;
    private readonly CurrencyCommonRepo _currency = CurrencyCommonRepo.Of();

    public static ColorTubeRepo Of() => new ColorTubeRepo();
    private ColorTubeRepo()
    {        
    }

    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.Increase(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxBell);
    }
    public Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.IncreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxBell);
    }
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.Decrease(db, transaction, reason, userSeq, CurrencyType, itemPrice);
    }
    public Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice);
    }
    public bool HasEnough(MySqlConnection db, ulong userSeq, long itemPrice)
    {
        return _currency.HasEnough(db, CurrencyType, userSeq, itemPrice);
    }
    public Task<bool> HasEnoughAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        return _currency.HasEnoughAsync(db, transaction, CurrencyType, userSeq, itemPrice);
    }
    public bool HasEnough(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        return _currency.HasEnough(db, transaction, CurrencyType, userSeq, itemPrice);
    }
    public long FetchTotalAmount(MySqlConnection db, ulong userSeq)
    {
        return _currency.FetchTotalAmount(db, CurrencyType, userSeq);
    }
    public long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        return _currency.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
    }

    public Task<long> FetchTotalAmountAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        return _currency.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
    }
}



