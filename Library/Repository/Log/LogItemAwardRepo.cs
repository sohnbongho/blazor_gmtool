using Dapper;
using Library.Component;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using log4net;
using MySqlConnector;
using Newtonsoft.Json;
using System.Reflection;

namespace Library.Repository.Log;

/// <summary>
/// 아이템 지급 로그 (획득로그)
/// </summary>
public class LogItemAwardRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    private async Task<ErrorCode> AddLogAsync(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason,
        int serverId, ulong userSeq, string guid, int itemSn, long amount, object parameter)
    {
        if (itemSn <= 0 || amount <= 0)
        {
            return ErrorCode.Succeed;
        }

        if (userSeq == 0)
        {
            return ErrorCode.NotFoundUserSeq;
        }
        if (string.IsNullOrEmpty(guid))
        {
            return ErrorCode.InvalidGameGuid;
        }

        var jsonData = JsonConvert.SerializeObject(parameter);

        var query = "INSERT INTO tbl_log_item_distribution VALUES(NULL, @server_id, @user_seq, @guid, @reason_code, @item_sn, @amount, @json_data, CURRENT_TIMESTAMP);";
        int rowsAffected = await db.ExecuteAsync(query, new TblLogItemDistribution
        {
            server_id = serverId,
            user_seq = userSeq,
            guid = guid,
            reason_code = (int)reason,
            item_sn = itemSn,
            amount = amount,
            json_data = jsonData,
        }, transaction);
        if (rowsAffected <= 0)
        {
            return ErrorCode.DbInsertedError;
        }

        return ErrorCode.Succeed;
    }

    public ErrorCode AddLog(MySqlConnection db, ItemGainReason reason, int serverId, ulong userSeq, string guid, int itemSn, long amount, object parameter)
    {
        if (itemSn <= 0 || amount <= 0)
        {
            return ErrorCode.Succeed;
        }
        if (userSeq == 0)
        {
            return ErrorCode.Succeed;
        }
        if (string.IsNullOrEmpty(guid))
        {
            return ErrorCode.InvalidGameGuid;
        }

        var jsonData = JsonConvert.SerializeObject(parameter);

        var query = "INSERT INTO tbl_log_item_distribution VALUES(NULL, @server_id, @user_seq, @guid, @reason_code, @item_sn, @amount, @json_data, CURRENT_TIMESTAMP);";
        int rowsAffected = db.Execute(query, new TblLogItemDistribution
        {
            server_id = serverId,
            user_seq = userSeq,
            guid = guid,
            reason_code = (int)reason,
            item_sn = itemSn,
            amount = amount,
            json_data = jsonData,
        });
        if (rowsAffected <= 0)
        {
            return ErrorCode.DbInsertedError;
        }

        return ErrorCode.Succeed;

    }
    private ErrorCode AddLog(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason,
        int serverId, ulong userSeq, string guid, int itemSn, long amount, object parameter)
    {
        if (itemSn <= 0 || amount <= 0)
        {
            return ErrorCode.Succeed;
        }

        if (userSeq == 0)
        {
            return ErrorCode.NotFoundUserSeq;
        }
        if (string.IsNullOrEmpty(guid))
        {
            return ErrorCode.InvalidGameGuid;
        }

        var jsonData = JsonConvert.SerializeObject(parameter);

        var query = "INSERT INTO tbl_log_item_distribution VALUES(NULL, @server_id, @user_seq, @guid, @reason_code, @item_sn, @amount, @json_data, CURRENT_TIMESTAMP);";
        int rowsAffected = db.Execute(query, new TblLogItemDistribution
        {
            server_id = serverId,
            user_seq = userSeq,
            guid = guid,
            reason_code = (int)reason,
            item_sn = itemSn,
            amount = amount,
            json_data = jsonData,
        }, transaction);
        if (rowsAffected <= 0)
        {
            return ErrorCode.DbInsertedError;
        }

        return ErrorCode.Succeed;
    }

    public async Task<ErrorCode> AddPurchaseLogAsync(MySqlConnection db, MySqlTransaction transaction,
        ItemGainReason reason, string guid, int serverId, ulong userSeq, int itemSn, long amount,
        PurchaseType purchaseType, long itemPrice, List<CurrencyItem> prevCurrencys, List<CurrencyItem> currencys)
    {

        return await AddLogAsync(db, transaction, reason, serverId, userSeq, guid, itemSn, amount,
            new PurchaseLog
            {
                ItemPrice = itemPrice,
                PurchaseType = purchaseType,
                CurrencyLog = CurrencyLog.Of(prevCurrencys, currencys),
            });
    }

    public Task<ErrorCode> AddMailLogAsync(MySqlConnection db, MySqlTransaction transaction,
        string guid, int serverId, ulong userSeq, int itemSn, long amount, MailLog parameter)
    {
        return AddLogAsync(db, transaction, ItemGainReason.Mail, serverId, userSeq, guid, itemSn, amount, parameter);
    }

    public async Task<ErrorCode> AddDailStampAsync(MySqlConnection db, MySqlTransaction transaction,
        int serverId, ulong userSeq, int itemSn, long amount,
        short stampCount, int payDiamond, int payClover,
        List<CurrencyItem> prevCurrencys, List<CurrencyItem> currencys)
    {
        var guid = Guid.NewGuid().ToString();
        return await AddLogAsync(db, transaction, ItemGainReason.DailyStamp, serverId, userSeq, guid, itemSn, amount,
            new DailyStampLog
            {
                ItemSn = itemSn,
                Amount = amount,
                StampCount = stampCount,
                PayDiamond = payDiamond,
                PayClover = payClover,
                CurrencyLog = CurrencyLog.Of(prevCurrencys, currencys),
            });
    }

    public ErrorCode AddSnowToMoney(MySqlConnection db, MySqlTransaction transaction,
        int serverId, ulong userSeq, int itemSn, long amount, AddSnowLog parameter)
    {
        var guid = Guid.NewGuid().ToString();
        return AddLog(db, transaction, ItemGainReason.SnowToExchange, serverId, userSeq, guid, itemSn, amount, parameter);
    }

    public async Task<ErrorCode> AddGrowCloverAsync(MySqlConnection db, MySqlTransaction transaction,
        int serverId, ulong userSeq, int itemSn, long amount, AddGrowCloverLog parameter)
    {
        var guid = Guid.NewGuid().ToString();
        return await AddLogAsync(db, transaction, ItemGainReason.GrowClover, serverId, userSeq, guid, itemSn, amount, parameter);
    }

    public async Task<ErrorCode> AddMoneyToBellAsync(MySqlConnection db, MySqlTransaction transaction,
        int serverId, ulong userSeq, int itemSn, long amount, MoneyToBellLog parameter)
    {
        var guid = Guid.NewGuid().ToString();
        return await AddLogAsync(db, transaction, ItemGainReason.MoneyToBell, serverId, userSeq, guid, itemSn, amount, parameter);
    }

    public async Task<ErrorCode> AddDiamondToBellAsync(MySqlConnection db, MySqlTransaction transaction, 
        int serverId, ulong userSeq, int itemSn, long amount, DiamondToBellLog parameter)
    {
        var guid = Guid.NewGuid().ToString();
        return await AddLogAsync(db, transaction, ItemGainReason.DiamondToBell, serverId, userSeq, guid, itemSn, amount, parameter);
    }



}
