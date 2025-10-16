using Library.DTO;
using Library.Helper;
using MySqlConnector;

namespace Library.Repository.Mysql.Currency;

public class PremiumDiamondRepo : ICurrencyRepo, IPayableCurrency
{
    public CurrencyType CurrencyType { get; } = CurrencyType.PremiumDiamond;
    private readonly CurrencyCommonRepo _currency = CurrencyCommonRepo.Of();
    private readonly GoldDiamondFestaSharedRepo _festa = GoldDiamondFestaSharedRepo.Of();

    public static PremiumDiamondRepo Of() => new PremiumDiamondRepo();
    private PremiumDiamondRepo()
    {
    }

    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.Increase(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxPremiumDiamond);
    }
    public Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.IncreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxPremiumDiamond);
    }
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        var (errorCode0, totalCurrency) = _currency.Decrease(db, transaction, reason, userSeq, CurrencyType, itemPrice);
        if (errorCode0 == ErrorCode.Succeed)
        {
            _festa.Accumulate(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
        }
        return (errorCode0, totalCurrency);
    }
    public async Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        var (errorCode0, totalCurrency) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice);
        if (errorCode0 == ErrorCode.Succeed)
        {
            await _festa.AccumulateAsync(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
        }
        return (errorCode0, totalCurrency);
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
