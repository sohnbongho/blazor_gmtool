using Library.Data.Models;
using Library.DTO;
using Library.Helper;
using Library.Provider.Item.Interface;
using log4net;
using MySqlConnector;
using System.Reflection;

namespace Library.Repository.Mysql;

/// <summary>
/// 아이템 지급 관련 처리
/// </summary>
public class ItemAwardSharedRepo
{
    public static ItemAwardSharedRepo Of() => new ItemAwardSharedRepo();
    private ItemAwardSharedRepo()
    {

    }

    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    /// <summary>
    /// 아이템 지급 처리
    /// </summary>    
    public async Task<(ErrorCode, DateTime, ulong)> AwardAsync(MySqlConnection db, MySqlTransaction transaction,
        IItemPolicy itemPolicy,
        ulong charSeq, int itemSn, int amount, int itemPeriod)
    {
        var expiredTime = DateTimeHelper.Now;
        var itemInfo = itemPolicy.GetShopItemInfo(itemSn);
        if (itemInfo.ItemSn == 0)
        {
            _logger.Error($"not found item:{itemSn}");
            return (ErrorCode.NotFoundItem, expiredTime, 0);
        }

        // 아이템 구매&지급 처리
        int slotId = 0;
        var colorId = 0;
        ulong itemSeq = 0;
        if (itemInfo is IDesignHasSlotItemInfo iSlotInfo)
        {
            slotId = iSlotInfo.SlotId;
        }

        if (itemInfo is IDesignDyeItemInfo iDye)
        {
            colorId = iDye.ColorId;
        }
        var hasCount = 0;
        if (itemPolicy is IInventoryHasItem iInventoryHasItem)
        {
            hasCount = await iInventoryHasItem.FetchHasCountAsync(db, transaction, charSeq, itemSn);
        }
        if (hasCount > 0 && (itemPolicy is ITimeExtendableItem iTimeExtendableItem))
        {
            // 기간만 늘린다.
            var (errorCode2, query2, expiredTime2, returnItemSeq) =
                await iTimeExtendableItem.ExtendItemUsagePeriod(db, transaction, charSeq, itemSn, itemPeriod, colorId);

            if (errorCode2 != ErrorCode.Succeed)
            {
                _logger.Error($"failed to [{query2}]");
                return (errorCode2, expiredTime, 0);
            }
            expiredTime = expiredTime2;
            itemSeq = returnItemSeq;
        }
        else if (itemPolicy is IDistributableItem iDistributableItem)
        {
            itemSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();

            // 아이템 지급
            var (errorCode2, query2, expiredTime2) =
            await iDistributableItem.DistributeItemAsync(db, transaction, charSeq, itemSn, slotId, itemSeq, amount, itemPeriod, colorId);

            if (errorCode2 != ErrorCode.Succeed)
            {
                _logger.Error($"failed to [{query2}]");                
                return (errorCode2, expiredTime, 0);
            }
            expiredTime = expiredTime2;
        }
        else if (itemPolicy is IDistributableCurrencyItem iDistributableCurrencyItem)
        {
            itemSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();

            // 아이템 지급
            var (errorCode2, query2, expiredTime2) =
            await iDistributableCurrencyItem.DistributeItemAsync(db, transaction, ItemGainReason.DailyStamp, charSeq, itemSn, slotId, itemSeq, amount, itemPeriod, colorId);

            if (errorCode2 != ErrorCode.Succeed)
            {
                _logger.Error($"failed to [{query2}]");                
                return (errorCode2, expiredTime, 0);
            }
            expiredTime = expiredTime2;
        }
        else
        {
            _logger.Error($"not found item:{itemSn}");            
            return (ErrorCode.NotFoundItem, expiredTime, 0);
        }

        return (ErrorCode.Succeed, expiredTime, itemSeq);
    }
}
