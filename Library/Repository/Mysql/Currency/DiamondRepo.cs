using Library.DTO;
using Library.Helper;
using MySqlConnector;

namespace Library.Repository.Mysql.Currency;

public class DiamondRepo : ICurrencyRepo, IPayableCurrency
{
    public CurrencyType CurrencyType { get; } = CurrencyType.Diamond;
    private readonly CurrencyCommonRepo _currency = CurrencyCommonRepo.Of();
    private readonly GoldDiamondFestaSharedRepo _festa = GoldDiamondFestaSharedRepo.Of();

    public static DiamondRepo Of() => new DiamondRepo();
    private DiamondRepo()
    {        
    }

    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        var maxValue = ConstInfo.MaxDiamond;
        return _currency.Increase(db, transaction, reason, userSeq, CurrencyType, itemPrice, maxValue);
    }
    public Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.IncreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxDiamond);
    }
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        //return _currency.Decrease(db, transaction, userSeq, CurrencyType, itemPrice);
        // 부족하면 유료 캐쉬에서 차감
        var hasFreeCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
        if (hasFreeCurrency >= itemPrice)
        {
            // 무료 재화로도 충분
            var (errorCode0, totalCurrency) = _currency.Decrease(db, transaction, reason, userSeq, CurrencyType, itemPrice);
            if (errorCode0 == ErrorCode.Succeed)
            {
                _festa.Accumulate(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
            }
            return (errorCode0, totalCurrency);
        }
        var hasPremiumCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType.PremiumDiamond, userSeq);
        var hasTotalCurrency = hasFreeCurrency + hasPremiumCurrency;
        if (hasTotalCurrency < itemPrice)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }
        var payPremium = itemPrice - hasFreeCurrency;
        if (payPremium < 0)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }

        var (errorCode1, totalFree) = _currency.Decrease(db, transaction, reason, userSeq, CurrencyType, hasFreeCurrency);
        if (errorCode1 != ErrorCode.Succeed)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }
        var (errorCode2, totalPremium) = _currency.Decrease(db, transaction, reason, userSeq, CurrencyType.PremiumDiamond, payPremium);
        if (errorCode2 != ErrorCode.Succeed)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }

        _festa.Accumulate(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
        return (ErrorCode.Succeed, totalFree);
    }
    public async Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        //return _currency.DecreaseAsync(db, transaction, userSeq, CurrencyType, itemPrice);
        var freeCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
        if (freeCurrency >= itemPrice)
        {
            // 무료 재화로도 충분
            var (errorCode0, total) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice);
            if (errorCode0 == ErrorCode.Succeed)
            {
                await _festa.AccumulateAsync(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
            }
            return (errorCode0, total);
        }
        var premiumCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType.PremiumDiamond, userSeq);
        var hasTotalCurrency = freeCurrency + premiumCurrency;
        if (hasTotalCurrency < itemPrice)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }
        var payPremium = itemPrice - freeCurrency;
        if (payPremium < 0)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }

        var (errorCode1, totalFree) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, freeCurrency);
        if (errorCode1 != ErrorCode.Succeed)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }
        var (errorCode2, totalPremium) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType.PremiumDiamond, payPremium);
        if (errorCode2 != ErrorCode.Succeed)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }

        await _festa.AccumulateAsync(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
        return (ErrorCode.Succeed, totalFree);
    }
    public bool HasEnough(MySqlConnection db, ulong userSeq, long itemPrice)
    {
        var totalCurrency = _currency.FetchTotalAmount(db, CurrencyType, userSeq);
        if (totalCurrency < itemPrice)
        {
            var premiumCurrency = _currency.FetchTotalAmount(db, CurrencyType.PremiumDiamond, userSeq);
            totalCurrency += premiumCurrency;
        }
        return totalCurrency >= itemPrice;
    }
    public async Task<bool> HasEnoughAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        // 부족하면 유료 캐쉬도 체크
        var totalCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
        if (totalCurrency < itemPrice)
        {
            var premiumCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType.PremiumDiamond, userSeq);
            totalCurrency += premiumCurrency;
        }
        return totalCurrency >= itemPrice;
    }
    public bool HasEnough(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        var totalCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
        if (totalCurrency < itemPrice)
        {
            var premiumCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType.PremiumDiamond, userSeq);
            totalCurrency += premiumCurrency;
        }
        return totalCurrency >= itemPrice;
    }
    public long FetchTotalAmount(MySqlConnection db, ulong userSeq)
    {
        var totalCurrency = _currency.FetchTotalAmount(db, CurrencyType, userSeq);
        return totalCurrency;
    }
    public long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        var totalCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
        return totalCurrency;
    }
    public async Task<long> FetchTotalAmountAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        var totalCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
        return totalCurrency;
    }
}
