using Dapper;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using MySqlConnector;
using System.Text;

namespace Library.Repository.Mysql.Currency;

public class CurrencyCommonRepo
{
    public static CurrencyCommonRepo Of() => new CurrencyCommonRepo();
    private CurrencyCommonRepo()
    {

    }

    /// <summary>
    /// 제화 증가
    /// </summary>    
    public (ErrorCode, long) Increase(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, CurrencyType currencyType, long itemPrice, long maxValue)
    {
        var absPrice = Math.Abs(itemPrice);
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_currency SET amount = LEAST(amount + @amount, {maxValue}) ");
            strBuilder.Append($"WHERE user_seq=@user_seq and currency_type=@currency_type;");

            var query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblInventoryCurrency
            {
                user_seq = userSeq,
                currency_type = (short)currencyType,
                amount = absPrice,
            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, 0);
            }
        }


        // 총골드의 양
        var totalCurrency = FetchTotalAmount(db, transaction, currencyType, userSeq);

        // 재화 변경 로그에 추가
        {
            var query = $"INSERT INTO tbl_log_currency_change VALUES (null, @user_seq, @item_gain_reason, @currency_type, @added_amount, @amount, CURRENT_TIMESTAMP)";
            int rowsAffected = db.Execute(query, new TblLogCurrencyChange
            {
                user_seq = userSeq,
                item_gain_reason = (int)reason,
                currency_type = (short)currencyType,
                added_amount = absPrice,
                amount = totalCurrency,
            }, transaction);
        }

        return (ErrorCode.Succeed, totalCurrency);
    }
    public async Task<(ErrorCode, long)> IncreaseAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, ulong userSeq, CurrencyType currencyType, long itemPrice, long maxValue)
    {
        var absPrice = Math.Abs(itemPrice);
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_currency SET amount = LEAST(amount + @amount, {maxValue}) ");
            strBuilder.Append($"WHERE user_seq=@user_seq and currency_type=@currency_type;");



            var query = strBuilder.ToString();
            int rowsAffected = await db.ExecuteAsync(query, new TblInventoryCurrency
            {
                user_seq = userSeq,
                currency_type = (short)currencyType,
                amount = absPrice,
            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, 0);
            }

        }
        // 총골드의 양
        var totalCurrency = await FetchTotalAmountAsync(db, transaction, currencyType, userSeq);

        // 재화 변경 로그에 추가
        {
            var query = $"INSERT INTO tbl_log_currency_change VALUES (null, @user_seq, @item_gain_reason, @currency_type, @added_amount, @amount, CURRENT_TIMESTAMP)";
            int rowsAffected = await db.ExecuteAsync(query, new TblLogCurrencyChange
            {
                user_seq = userSeq,
                item_gain_reason = (int)reason,
                currency_type = (short)currencyType,
                added_amount = absPrice,
                amount = totalCurrency,
            }, transaction);
        }

        return (ErrorCode.Succeed, totalCurrency);
    }

    /// <summary>
    /// 가지고 있는 총 제화
    /// </summary>    
    public long FetchTotalAmount(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq)
    {
        var query = $"SELECT amount FROM tbl_inventory_currency WHERE user_seq=@user_seq and currency_type=@currency_type limit 1;";
        var result = db.Query<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
            currency_type = (short)currencyType,
        }, transaction);

        var tblMember = result.FirstOrDefault();
        return tblMember != null ? tblMember.amount : 0;
    }
    public long FetchTotalAmount(MySqlConnection db, CurrencyType currencyType, ulong userSeq)
    {
        var query = $"SELECT amount FROM tbl_inventory_currency WHERE user_seq=@user_seq and currency_type=@currency_type limit 1;";
        var result = db.Query<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
            currency_type = (short)currencyType,
        });

        var tblMember = result.FirstOrDefault();
        return tblMember != null ? tblMember.amount : 0;
    }
    public List<CurrencyItem> FetchTotalAmounts(MySqlConnection db, ulong userSeq)
    {
        var query = $"SELECT * FROM tbl_inventory_currency WHERE user_seq=@user_seq;";
        var result = db.Query<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
        });

        var itmes = new List<CurrencyItem>();
        var tbls = result.ToList();
        foreach (var tbl in tbls)
        {
            var currencyType = ConvertHelper.ToEnum<CurrencyType>(tbl.currency_type);
            itmes.Add(new CurrencyItem
            {
                CurrencyType = currencyType,
                Amount = tbl.amount
            });
        }
        return itmes;
    }

    public List<CurrencyItem> FetchTotalAmounts(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        var query = $"SELECT * FROM tbl_inventory_currency WHERE user_seq=@user_seq;";
        var result = db.Query<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
        }, transaction);

        var itmes = new List<CurrencyItem>();
        var tbls = result.ToList();
        foreach (var tbl in tbls)
        {
            var currencyType = ConvertHelper.ToEnum<CurrencyType>(tbl.currency_type);
            itmes.Add(new CurrencyItem
            {
                CurrencyType = currencyType,
                Amount = tbl.amount
            });
        }
        return itmes;
    }

    public async Task<List<CurrencyItem>> FetchTotalAmountsAsync(MySqlConnection db, ulong userSeq)
    {
        var query = $"SELECT * FROM tbl_inventory_currency WHERE user_seq=@user_seq;";
        var result = await db.QueryAsync<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
        });

        var itmes = new List<CurrencyItem>();
        var tbls = result.ToList();
        foreach (var tbl in tbls)
        {
            var currencyType = ConvertHelper.ToEnum<CurrencyType>(tbl.currency_type);
            itmes.Add(new CurrencyItem
            {
                CurrencyType = currencyType,
                Amount = tbl.amount
            });
        }
        return itmes;
    }

    public async Task<List<CurrencyItem>> FetchTotalAmountsAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        var query = $"SELECT * FROM tbl_inventory_currency WHERE user_seq=@user_seq;";
        var result = await db.QueryAsync<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
        }, transaction);

        var itmes = new List<CurrencyItem>();
        var tbls = result.ToList();
        foreach (var tbl in tbls)
        {
            var currencyType = ConvertHelper.ToEnum<CurrencyType>(tbl.currency_type);
            itmes.Add(new CurrencyItem
            {
                CurrencyType = currencyType,
                Amount = tbl.amount
            });
        }
        return itmes;
    }
    public async Task<long> FetchTotalAmountAsync(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq)
    {
        var query = $"SELECT amount FROM tbl_inventory_currency WHERE user_seq=@user_seq and currency_type=@currency_type limit 1;";
        var result = await db.QueryAsync<TblInventoryCurrency>(query, new TblInventoryCurrency
        {
            user_seq = userSeq,
            currency_type = (short)currencyType,
        }, transaction);

        var tblMember = result.FirstOrDefault();
        return tblMember != null ? tblMember.amount : 0;
    }
    /// <summary>
    /// 제화 소모
    /// </summary>    
    public (ErrorCode, long) Decrease(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, CurrencyType currencyType, long itemPrice)
    {
        var absPrice = Math.Abs(itemPrice);
        if (absPrice == 0)
        {
            var currentCurrency = FetchTotalAmount(db, transaction, currencyType, userSeq);
            return (ErrorCode.Succeed, currentCurrency);
        }

        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_currency SET amount = GREATEST(amount - @amount, 0) ");
            strBuilder.Append($"WHERE user_seq=@user_seq and currency_type=@currency_type;");

            var query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblInventoryCurrency
            {
                user_seq = userSeq,
                currency_type = (short)currencyType,
                amount = absPrice,
            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, 0);
            }
        }


        // 총골드의 양
        var totalCurrency = FetchTotalAmount(db, transaction, currencyType, userSeq);

        // 제화 사용 누적
        AccumulateSpent(db, transaction, userSeq, currencyType, itemPrice);

        // 재화 변경 로그에 추가
        {
            var query = $"INSERT INTO tbl_log_currency_change VALUES (null, @user_seq, @item_gain_reason, @currency_type, @added_amount, @amount, CURRENT_TIMESTAMP)";
            int rowsAffected = db.Execute(query, new TblLogCurrencyChange
            {
                user_seq = userSeq,
                item_gain_reason = (int)reason,
                currency_type = (short)currencyType,
                added_amount = -1 * absPrice,
                amount = totalCurrency,
            }, transaction);
        }

        return (ErrorCode.Succeed, totalCurrency);
    }
    public async Task<(ErrorCode, long)> DecreaseAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, CurrencyType currencyType, long itemPrice)
    {
        var absPrice = Math.Abs(itemPrice);
        if (absPrice == 0)
        {
            var currentCurrency = await FetchTotalAmountAsync(db, transaction, currencyType, userSeq);
            return (ErrorCode.Succeed, currentCurrency);
        }

        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_inventory_currency SET amount = GREATEST(amount - @amount, 0) ");
            strBuilder.Append($"WHERE user_seq=@user_seq and currency_type=@currency_type;");

            var query = strBuilder.ToString();
            int rowsAffected = await db.ExecuteAsync(query, new TblInventoryCurrency
            {
                user_seq = userSeq,
                currency_type = (short)currencyType,
                amount = absPrice,
            }, transaction);
            if (rowsAffected <= 0)
            {
                return (ErrorCode.DbInsertedError, 0);
            }
        }

        // 총골드의 양
        var totalCurrency = await FetchTotalAmountAsync(db, transaction, currencyType, userSeq);

        // 제화 사용 누적
        await AccumulateSpentAsync(db, transaction, userSeq, currencyType, itemPrice);

        // 재화 변경 로그에 추가
        {
            var query = $"INSERT INTO tbl_log_currency_change VALUES (null, @user_seq, @item_gain_reason, @currency_type, @added_amount, @amount, CURRENT_TIMESTAMP)";
            int rowsAffected = await db.ExecuteAsync(query, new TblLogCurrencyChange
            {
                user_seq = userSeq,
                item_gain_reason = (int)reason,
                currency_type = (short)currencyType,
                added_amount = -1 * absPrice,
                amount = totalCurrency,
            }, transaction);
        }
        return (ErrorCode.Succeed, totalCurrency);
    }

    private bool AccumulateSpent(MySqlConnection db, MySqlTransaction transaction,
        ulong userSeq, CurrencyType currencyType, long itemPrice)
    {
        var queryBuilder = new StringBuilder();
        queryBuilder.Append($"UPDATE tbl_log_currency SET ");
        queryBuilder.Append("spent_amount = spent_amount + @spent_amount ");
        queryBuilder.Append($"where user_seq=@user_seq and currency_type = @currency_type;");
        var updatedQuery = queryBuilder.ToString();

        var absPrice = Math.Abs(itemPrice);

        var rowsAffected = db.Execute(updatedQuery, new TblLogCurrency
        {
            user_seq = userSeq,
            currency_type = (short)currencyType,
            spent_amount = (ulong)absPrice,

        }, transaction);
        return true;
    }


    private async Task<bool> AccumulateSpentAsync(MySqlConnection db, MySqlTransaction transaction,
        ulong userSeq, CurrencyType currencyType, long itemPrice)
    {
        var queryBuilder = new StringBuilder();
        queryBuilder.Append($"UPDATE tbl_log_currency SET ");
        queryBuilder.Append($"spent_amount = spent_amount + @spent_amount ");
        queryBuilder.Append($"where user_seq=@user_seq and currency_type = @currency_type;");
        var updatedQuery = queryBuilder.ToString();

        var absPrice = Math.Abs(itemPrice);

        var rowsAffected = await db.ExecuteAsync(updatedQuery, new TblLogCurrency
        {
            user_seq = userSeq,
            currency_type = (short)currencyType,
            spent_amount = (ulong)absPrice,

        }, transaction);

        return true;
    }
    public bool HasEnough(MySqlConnection db, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var hasTotal = FetchTotalAmount(db, currencyType, userSeq);
        if (hasTotal >= itemPrice)
            return true;

        return false;
    }

    public bool HasEnough(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var hasTotal = FetchTotalAmount(db, transaction, currencyType, userSeq);
        if (hasTotal >= itemPrice)
            return true;

        return false;
    }

    public async Task<bool> HasEnoughAsync(MySqlConnection db, MySqlTransaction transaction, CurrencyType currencyType, ulong userSeq, long itemPrice)
    {
        var hasTotal = await FetchTotalAmountAsync(db, transaction, currencyType, userSeq);
        if (hasTotal >= itemPrice)
            return true;

        return false;
    }
}
