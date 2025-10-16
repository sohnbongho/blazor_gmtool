using Library.Helper;
using Messages;
using MySqlConnector;
using Library.DTO;
using Library.Data.Models;
using Library.Provider.Item.Interface;
using Library.Repository.Item;
using Library.DBTables.MySql;
using Library.Repository.Mysql;


namespace Library.Provider.Item.Provider;

public class SoftCatItemPolicy : IItemPolicy, IRewardableItem, IDistributableItem, IInventoryHasItem, IInGameEquipableItem
{
    public MainItemType ItemType { get; } = MainItemType.SoftCat;
    public string TblName { get; } = "tbl_inventory_mode_softcat";
    private ItemSharedRepo? _itemRepository;
    private SeasonItemInvenSharedRepo? _seasonItemRepository;

    public SoftCatItemPolicy()
    {
        _itemRepository = new ItemSharedRepo(ItemType, TblName);
        _seasonItemRepository = SeasonItemInvenSharedRepo.Of();
    }
    public void Dispose()
    {
        _itemRepository = null;
        _seasonItemRepository = null;
    }
    public IDesignBaseItemInfo GetShopItemInfo(int itemId)
    {
        if (_itemRepository == null)
            return DesignNoneItemInfo.Instance;

        return _itemRepository.GetShopItemInfo(itemId);
    }

    /// <summary>
    /// 아이템 지급 DB 쿼리
    /// </summary>        
    public async Task<(ErrorCode, string, DateTime)> DistributeItemAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        if (_itemRepository == null)
            return (ErrorCode.DbInsertedError, string.Empty, DateTime.MinValue);

        var designItems = ItemDBLoaderHelper.Instance.DesignBaseItemInfo;
        if (false == designItems.TryGetValue(itemId, out var itemInfo))
        {
            return (ErrorCode.NotFoundItem, string.Empty, new DateTime());
        }
        if (itemInfo is DesignSoftCatItemInfo info)
        {
            if (info.StoreDb == false)
            {
                var tempExpiredDate = DateTimeHelper.Now + TimeSpan.FromDays(365);

                // DB에 저장할 필요가 없으면 무조건 성공
                return (ErrorCode.Succeed, string.Empty, tempExpiredDate);
            }
        }

        var result = await _itemRepository.DistributeItemAsync<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId, slotId, itemSeq, amount, itemPeriod, colorId);
        return result;
    }

    public (ErrorCode, string, DateTime) DistributeItem(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        if (_itemRepository == null)
            return (ErrorCode.DbInsertedError, string.Empty, DateTime.MinValue);

        var designItems = ItemDBLoaderHelper.Instance.DesignBaseItemInfo;
        if(false == designItems.TryGetValue(itemId, out var itemInfo))
        {
            return (ErrorCode.NotFoundItem, string.Empty, new DateTime());
        }
        if(itemInfo is DesignSoftCatItemInfo info)
        {
            if(info.StoreDb == false)
            {
                var expiredDate = DateTimeHelper.Now + TimeSpan.FromDays(ConstInfo.DefaultItemExpireDay);

                // DB에 저장할 필요가 없으면 무조건 성공
                return (ErrorCode.Succeed, string.Empty, expiredDate);
            }
        }

        var result = _itemRepository.DistributeItem<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId, slotId, itemSeq, amount, itemPeriod, colorId);
        return result;
    }

    public async Task<InventoryItem> FetchItemByIdAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        if (_itemRepository == null)
            return new InventoryItem();

        var tblItem = await _itemRepository.FetchItemByIdAsync<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId);
        if (tblItem == null)
            return new InventoryItem();

        return new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = tblItem.amount,
            SlotId = tblItem.slot_id,
            JsonData = tblItem.json_data,
            ColorId = tblItem.color_id,

            ExpiredDate = tblItem.expiration_date.ToString(ConstInfo.TimeFormat),
        };
    }
    public async Task<InventoryItem> FetchInventoryItemAsync(MySqlConnection db, ulong charSeq, ulong itemSeq)
    {
        if (_itemRepository == null)
            return new InventoryItem();

        var tblItem = await _itemRepository.FetchInventoryItemAsync<TblInventoryModeSoftcat>(db, charSeq, itemSeq);
        if (tblItem == null)
            return new InventoryItem();

        return new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = tblItem.amount,
            SlotId = tblItem.slot_id,
            JsonData = tblItem.json_data,
            ColorId = tblItem.color_id,

            ExpiredDate = tblItem.expiration_date.ToString(ConstInfo.TimeFormat),
        };
    }
    public async Task<List<InventoryItem>> FetchInventoryAllItemsAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq)
    {
        if (_itemRepository == null)
            return new List<InventoryItem>();

        var tblItems = await _itemRepository.FetchInventoryAllItemsAsync<TblInventoryModeSoftcat>(db, transaction, charSeq);

        return tblItems.Select(tblItem => new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = tblItem.amount,
            SlotId = tblItem.slot_id,
            JsonData = tblItem.json_data,
            ColorId = tblItem.color_id,

            ExpiredDate = tblItem.expiration_date.ToString(ConstInfo.TimeFormat),
        }).ToList();
    }

    public List<InventoryItem> FetchInventoryAllItems(MySqlConnection db, ulong charSeq)
    {
        if (_itemRepository == null)
            return new List<InventoryItem>();

        var tblItems = _itemRepository.FetchInventoryAllItems<TblInventoryModeSoftcat>(db, charSeq);

        return tblItems.Select(tblItem => new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = tblItem.amount,
            SlotId = tblItem.slot_id,
            JsonData = tblItem.json_data,
            ColorId = tblItem.color_id,

            ExpiredDate = tblItem.expiration_date.ToString(ConstInfo.TimeFormat),
        }).ToList();
    }
    public async Task<List<InventoryItem>> FetchInventoryEquippedItemsAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, short equipped)
    {
        if (_itemRepository == null)
            return new List<InventoryItem>();

        var tblItems = await _itemRepository.FetchInventoryEquippedItemsAsync<TblInventoryBackground>(db, transaction, charSeq, equipped);

        return tblItems.Select(tblItem => new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = 1,
            SlotId = tblItem.slot_id,
            JsonData = string.Empty,
            ColorId = 0,

            ExpiredDate = string.Empty,
        }).ToList();
    }
  
    public ErrorCode UpdateInventoryEquiped(MySqlConnection db, MySqlTransaction transaction, GameModeType gameModeType, ulong charSeq, int itemId, ulong itemSeq, int slotId, short equipped, string ingameParts)
    {
        if (_itemRepository == null)
            return ErrorCode.DbInsertedError;
        
        if (_seasonItemRepository == null)
            return ErrorCode.DbInsertedError;

        var errorCode = _itemRepository.UpdateInventoryEquiped<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId, itemSeq, slotId, equipped);
        if (errorCode != ErrorCode.Succeed)
            return errorCode;

        var affected = _seasonItemRepository.UpdateIngameParts(db, transaction, gameModeType, charSeq, ingameParts);
        if(affected <= 0)
            return ErrorCode.DbUpdatedError;

        return errorCode;
    }

    public async Task<int> FetchHasCountAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        if (_itemRepository == null)
            return 0;

        var count = await _itemRepository.FetchHasCountAsync<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId);
        return count;
    }    


    public async Task<(ErrorCode, InventoryItem)> RewardItemAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, ulong charSeq, int itemId, int amount)
    {
        if (_itemRepository == null)
            return (ErrorCode.NotFoundItem, new InventoryItem());

        var itemInfo = _itemRepository.GetShopItemInfo(itemId);
        if (itemInfo.ItemSn == 0)
            return (ErrorCode.NotFoundItem, new InventoryItem());
                
        ulong itemSeq = 0;
        var itemAmount = amount;        
        DateTime expireTime;
        int slotId = 0;
        int colorId = 0;
        int itemPeriodDay = 0;

        if(itemInfo is IDesignHasSlotItemInfo iSlotInfo)
        {
            slotId = iSlotInfo.SlotId;
        }
        if (itemInfo is IDesignPurchasableItemInfo iPurchase)
        {
            itemPeriodDay = iPurchase.ItemPeriodDay;
        }
        if (itemInfo is IDesignDyeItemInfo iDye)
        {
            colorId = iDye.ColorId;
        }        
        

        var hasCount = await FetchHasCountAsync(db, transaction, charSeq, itemId);
        if (hasCount > 0)
        {
            // 기간만 늘린다.
            var (errorCode, query2, expireTime2, tbl) = await ExtendItemUsagePeriodAsync(db, transaction, 
                charSeq, itemId, itemPeriodDay, colorId);
            if (errorCode != ErrorCode.Succeed)
                return (errorCode, new InventoryItem());

            expireTime = tbl != null ? tbl.expiration_date : expireTime2;
            itemSeq = tbl != null ? tbl.item_seq : 0;
        }
        else
        {
            itemSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();

            // 아이템 지급
            var (errorCode, query, expireTime2) = 
                await DistributeItemAsync(db, transaction, charSeq, itemId, slotId, itemSeq, itemAmount, itemPeriodDay, colorId);

            if (errorCode != ErrorCode.Succeed)
                return (errorCode, new InventoryItem());

            expireTime = expireTime2;
        }
        return (ErrorCode.Succeed, new InventoryItem
        {
            ItemSeq = itemSeq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = itemInfo.ItemSn,
            Equip = 0,
            Amount = itemAmount,
            SlotId = slotId,
            JsonData = string.Empty,
            ColorId = colorId,

            ExpiredDate = expireTime.ToString(ConstInfo.TimeFormat),
        });
    }
    public (ErrorCode, InventoryItem) RewardItem(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, ulong charSeq, int itemId, int amount)
    {
        if (_itemRepository == null)
            return (ErrorCode.NotFoundItem, new InventoryItem());

        var itemInfo = _itemRepository.GetShopItemInfo(itemId);
        if (itemInfo.ItemSn == 0)
            return (ErrorCode.NotFoundItem, new InventoryItem());
                
        ulong itemSeq = 0;
        var itemAmount = amount;        
        DateTime expireTime;
        int slotId = 0;
        int colorId = 0;
        int itemPeriodDay = 0;

        if (itemInfo is IDesignHasSlotItemInfo iSlotInfo)
        {
            slotId = iSlotInfo.SlotId;
        }
        if (itemInfo is IDesignPurchasableItemInfo iPurchase)
        {
            itemPeriodDay = iPurchase.ItemPeriodDay;
        }
        if (itemInfo is IDesignDyeItemInfo iDye)
        {
            colorId = iDye.ColorId;
        }

        var hasCount = _itemRepository.FetchHasCount(db, transaction, charSeq, itemId);
        if (hasCount > 0)
        {
            // 기간만 늘린다.
            var (errorCode, query2, expireTime2, tbl) =
                ExtendItemUsagePeriod(db, transaction, charSeq, itemId, itemPeriodDay, colorId);

            if (errorCode != ErrorCode.Succeed)
                return (errorCode, new InventoryItem());

            expireTime = tbl != null ? tbl.expiration_date : expireTime2;
            itemSeq = tbl != null ? tbl.item_seq : 0;
        }
        else
        {
            itemSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();

            // 아이템 지급
            var (errorCode, query, expireTime2) = DistributeItem(db, transaction,
                charSeq, itemId, slotId, itemSeq, itemAmount, itemPeriodDay, colorId);

            if (errorCode != ErrorCode.Succeed)
                return (errorCode, new InventoryItem());

            expireTime = expireTime2;
        }
        return (ErrorCode.Succeed, new InventoryItem
        {
            ItemSeq = itemSeq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = itemInfo.ItemSn,
            Equip = 0,
            Amount = itemAmount,
            SlotId = slotId,
            JsonData = string.Empty,
            ColorId = colorId,

            ExpiredDate = expireTime.ToString(ConstInfo.TimeFormat),
        });
    }

    /// <summary>
    /// 기간 연장
    /// </summary>    
    private (ErrorCode, string, DateTime, TblInventoryModeSoftcat?) ExtendItemUsagePeriod(MySqlConnection db, MySqlTransaction transaction, 
        ulong charSeq, int itemId, int itemPeriodDay, int colorId)
    {
        if (_itemRepository == null)
            return (ErrorCode.DbInsertedError, string.Empty, DateTime.MinValue, new());

        return _itemRepository.ExtendItemUsagePeriod<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId, itemPeriodDay, colorId);
    }

    private async Task<(ErrorCode, string, DateTime, TblInventoryModeSoftcat?)> ExtendItemUsagePeriodAsync(MySqlConnection db, MySqlTransaction transaction,
        ulong charSeq, int itemId, int itemPeriodDay, int colorId)
    {
        if (_itemRepository == null)
            return (ErrorCode.DbInsertedError, string.Empty, DateTime.MinValue, new());

        return await _itemRepository.ExtendItemUsagePeriodAsync<TblInventoryModeSoftcat>(db, transaction, charSeq, itemId, itemPeriodDay, colorId);
    }


}
