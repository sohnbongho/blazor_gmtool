using Library.DTO;
using Library.Helper;
using MySqlConnector;

namespace Library.Repository.Mysql.Currency;
public class GoldRepo : ICurrencyRepo, IPayableCurrency
{
    public CurrencyType CurrencyType { get; } = CurrencyType.Gold;
    private readonly CurrencyCommonRepo _currency = CurrencyCommonRepo.Of();
    private readonly GoldDiamondFestaSharedRepo _festa = GoldDiamondFestaSharedRepo.Of();

    public static GoldRepo Of() => new GoldRepo();
    private GoldRepo()
    {
    }

    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        var maxValue = ConstInfo.MaxGameMoney;
        return _currency.Increase(db, transaction, reason, userSeq, CurrencyType, itemPrice, maxValue);
    }
    public Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
        return _currency.IncreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGameMoney);
    }
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long itemPrice)
    {
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
        var hasPremiumCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType.PremiumGold, userSeq);
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
        var (errorCode2, totalPremium) = _currency.Decrease(db, transaction, reason, userSeq, CurrencyType.PremiumGold, payPremium);
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
        // 부족하면 유료 캐쉬에서 차감
        //return _currency.DecreaseAsync(db, transaction, userSeq, CurrencyType, itemPrice);
        var hasFreeCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
        if (hasFreeCurrency >= itemPrice)
        {
            // 무료 재화로도 충분
            var (errorCode0, totalCurrency) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, itemPrice);
            if (errorCode0 == ErrorCode.Succeed)
            {
                await _festa.AccumulateAsync(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
            }
            return (errorCode0, totalCurrency);
        }
        var hasPremiumCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType.PremiumGold, userSeq);
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

        var (errorCode1, totalFree) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType, hasFreeCurrency);
        if (errorCode1 != ErrorCode.Succeed)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }
        var (errorCode2, totalPremium) = await _currency.DecreaseAsync(db, transaction, reason, userSeq, CurrencyType.PremiumGold, payPremium);
        if (errorCode2 != ErrorCode.Succeed)
        {
            return (ErrorCode.NotEnoughPrice, hasTotalCurrency);
        }

        await _festa.AccumulateAsync(db, transaction, userSeq, CurrencyType, itemPrice, ConstInfo.MaxGoldDiaFestAmount);
        return (ErrorCode.Succeed, totalFree);
    }
    public bool HasEnough(MySqlConnection db, ulong userSeq, long itemPrice)
    {
        // 부족하면 유료 캐쉬도 체크
        var totalCurrency = _currency.FetchTotalAmount(db, CurrencyType, userSeq);

        if (totalCurrency < itemPrice)
        {
            var premiumCurrency = _currency.FetchTotalAmount(db, CurrencyType.PremiumGold, userSeq);
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
            var premiumCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType.PremiumGold, userSeq);
            totalCurrency += premiumCurrency;
        }
        return totalCurrency >= itemPrice;
    }
    public bool HasEnough(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, long itemPrice)
    {
        // 부족하면 유료 캐쉬도 체크        
        var totalCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
        if (totalCurrency < itemPrice)
        {
            var premiumCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType.PremiumGold, userSeq);
            totalCurrency += premiumCurrency;
        }
        return totalCurrency >= itemPrice;
    }
    public long FetchTotalAmount(MySqlConnection db, ulong userSeq)
    {
        // 유료 캐시도 체크
        var totalCurrency = _currency.FetchTotalAmount(db, CurrencyType, userSeq);
        return totalCurrency;
    }
    public long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        // 유료 캐시도 체크
        var totalCurrency = _currency.FetchTotalAmount(db, transaction, CurrencyType, userSeq);
        return totalCurrency;

    }
    public async Task<long> FetchTotalAmountAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        // 유료 캐시도 체크
        var totalCurrency = await _currency.FetchTotalAmountAsync(db, transaction, CurrencyType, userSeq);
        return totalCurrency;
    }
}
