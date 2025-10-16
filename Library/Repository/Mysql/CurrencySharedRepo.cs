using Library.Component;
using Library.Data.Models;
using Library.DTO;
using Library.Repository.Mysql.Currency;
using log4net;
using MySqlConnector;
using System.Reflection;

namespace Library.Repository.Mysql;

public class CurrencySharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private readonly CurrencyCommonRepo _currencyCommonRepo = CurrencyCommonRepo.Of();

    public static CurrencySharedRepo Of()
    {
        return new CurrencySharedRepo();
    }
    private CurrencySharedRepo()
    {        
    }

    /// <summary>
    /// 골드 증가
    /// </summary>    
    public (ErrorCode, long) Increase(ItemGainReason reason, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return (ErrorCode.DbUpdatedError, 0);

            using var transaction = db.BeginTransaction();
            try
            {
                var (errroCode, total) = Increase(db, transaction, reason, currencyType, userSeq, itemPrice);
                if (errroCode != ErrorCode.Succeed)
                {
                    transaction.Rollback();
                    return (errroCode, 0);
                }

                transaction.Commit();

                return (errroCode, total);

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to IncreaseCurrency", ex);
                transaction.Rollback();
                return (ErrorCode.DbUpdatedError, 0);
            }
        }
    }
    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);

        if (!(repo is IPayableCurrency pay))
        {
            return (ErrorCode.NotFoundCurrencyType, 0);
        }
        return pay.Increase(db, transaction, reason, userSeq, itemPrice);
    }

    /// <summary>
    /// 
    /// </summary>
    public (ErrorCode, long) Decrease(ItemGainReason reason, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return (ErrorCode.DbUpdatedError, 0);

            using var transaction = db.BeginTransaction();
            try
            {
                var (errroCode, total) = Decrease(db, transaction, reason, currencyType, userSeq, itemPrice);
                if (errroCode != ErrorCode.Succeed)
                {
                    transaction.Rollback();
                    return (errroCode, 0);
                }

                transaction.Commit();

                return (errroCode, total);

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to DecreaseCurrency", ex);
                transaction.Rollback();
                return (ErrorCode.DbUpdatedError, 0);
            }

        }
    }
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);

        if (!(repo is IPayableCurrency pay))
        {
            return (ErrorCode.NotFoundCurrencyType, 0);
        }
        return pay.Decrease(db, transaction, reason, userSeq, itemPrice);
    }

    public async Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);

        if (!(repo is IPayableCurrency pay))
        {
            return (ErrorCode.NotFoundCurrencyType, 0);
        }
        return await pay.DecreaseAsync(db, transaction, reason, userSeq, itemPrice);

    }


    /// <summary>
    /// 제화증가
    /// </summary>    

    public async Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, CurrencyType currencyType, ulong userSeq, long amount)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);

        if (!(repo is IPayableCurrency pay))
        {
            return (ErrorCode.NotFoundCurrencyType, 0);
        }
        return await pay.IncreaseAsync(db, transaction, reason, userSeq, amount);
    }

    public (ErrorCode, long) IncreaseGold(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, long addedMoney)
    {
        var currencyType = CurrencyType.Gold;

        var repo = CurrencyRepoFactory.Of(currencyType);
        if (!(repo is IPayableCurrency pay))
        {
            return (ErrorCode.NotFoundCurrencyType, 0);
        }
        return pay.Increase(db, transaction, reason, userSeq, addedMoney);
    }

    public bool HasEnough(MySqlConnection db, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);
        if (!(repo is IPayableCurrency pay))
        {
            return false;
        }
        return pay.HasEnough(db, userSeq, itemPrice);
    }

    public bool HasEnough(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);
        if (!(repo is IPayableCurrency pay))
        {
            return false;
        }
        return pay.HasEnough(db, transaction, userSeq, itemPrice);
    }
    public async Task<bool> HasEnoughAsync(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);
        if (!(repo is IPayableCurrency pay))
        {
            return false;
        }
        return await pay.HasEnoughAsync(db, transaction, userSeq, itemPrice);
    }

    /// <summary>
    /// 제화 양 가져오기
    /// </summary>    
    public long FetchTotalAmount(CurrencyType currencyType, ulong userSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return 0;
            }
            return FetchTotalAmount(db, currencyType, userSeq);
        }
    }
    public long FetchTotalAmount(MySqlConnection db, CurrencyType currencyType, ulong userSeq)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);
        if (!(repo is IPayableCurrency pay))
        {
            return 0;
        }
        return pay.FetchTotalAmount(db, userSeq);
    }
    public long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq)
    {
        var repo = CurrencyRepoFactory.Of(currencyType);
        if (!(repo is IPayableCurrency pay))
        {
            return 0;
        }
        return pay.FetchTotalAmount(db, transaction, userSeq);
    }

    public List<CurrencyItem> FetchTotalAmounts(ulong userSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new List<CurrencyItem>();
            }
            return _currencyCommonRepo.FetchTotalAmounts(db, userSeq);
        }
    }

    public List<CurrencyItem> FetchTotalAmounts(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        return _currencyCommonRepo.FetchTotalAmounts(db, transaction, userSeq);
    }
    public async Task<List<CurrencyItem>> FetchTotalAmountsAsync(ulong userSeq)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new List<CurrencyItem>();
            }
            var total = await _currencyCommonRepo.FetchTotalAmountsAsync(db, userSeq);
            return total;
        }
    }

    public Task<List<CurrencyItem>> FetchTotalAmountsAsync(MySqlConnection db, ulong userSeq)
    {
        return _currencyCommonRepo.FetchTotalAmountsAsync(db, userSeq);
    }

    public Task<List<CurrencyItem>> FetchTotalAmountsAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        return _currencyCommonRepo.FetchTotalAmountsAsync(db, transaction, userSeq);
    }

}
