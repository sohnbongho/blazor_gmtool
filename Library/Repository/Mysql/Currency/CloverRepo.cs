using Library.DTO;
using Library.Helper;
using MySqlConnector;

namespace Library.Repository.Mysql.Currency;

public class CloverRepo : ICurrencyRepo, IPayableCurrency
{
    public CurrencyType CurrencyType { get; } = CurrencyType.Clover;
    private readonly CurrencyCommonRepo _currencyCommonRepo = CurrencyCommonRepo.Of();

    public static CloverRepo Of() => new CloverRepo();

    private CloverRepo()
    {
    }

    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.Increase(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxClover);
    }
    public Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.IncreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxClover);
    }
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.Decrease(db, transaction, reason, userSeq, CurrencyType, itemPrice);
    }
    public Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice);
    }
    public bool HasEnough(MySqlConnection db, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.HasEnough(db, CurrencyType, userSeq, itemPrice);
    }
    public Task<bool> HasEnoughAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.HasEnoughAsync(db, transaction, CurrencyType, userSeq, itemPrice);
    }
    public bool HasEnough(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        return _currencyCommonRepo.HasEnough(db, transaction, CurrencyType, userSeq, itemPrice);
    }
    public long FetchTotalAmount(MySqlConnection db, ulong userSeq)
    {
        return _currencyCommonRepo.FetchTotalAmount(db, CurrencyType, userSeq);
    }
    public long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        return _currencyCommonRepo.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
    }
    public Task<long> FetchTotalAmountAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        return _currencyCommonRepo.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
    }
}
