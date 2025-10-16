using Dapper;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using MySqlConnector;

namespace Library.Repository.Item;

public class ItemSharedRepo
{
    private MainItemType _itemType;
    private string _tblName;
    public ItemSharedRepo(MainItemType itemType, string tblName)
    {
        _itemType = itemType;
        _tblName = tblName;
    }

    public IDesignBaseItemInfo GetShopItemInfo(int itemId)
    {
        if (false == ItemDBLoaderHelper.Instance.DesignBaseItemInfo.TryGetValue(itemId, out var itemInfo))
            return DesignNoneItemInfo.Instance;

        if (itemInfo.ItemType != _itemType)
            return DesignNoneItemInfo.Instance;

        return itemInfo;
    }

    /// <summary>
    /// 만료된 아이템에 대한 처리
    /// </summary>    
    public async Task<ErrorCode> HandleExpiredItem<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, DateTime now)
    {
        var query = $"DELETE FROM {_tblName} WHERE char_seq = @char_seq AND expiration_date < @expiration_date;";
        int rowsAffected = await db.ExecuteAsync(query, new { char_seq = charSeq, expiration_date = now }, transaction);

        return ErrorCode.Succeed;
    }

    // 아이템 지급 관련
    public async Task<(ErrorCode, string, DateTime)> DistributeItemAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        var peoriod = $"DATE_ADD(NOW(), INTERVAL {itemPeriod} DAY)";
        var query = $"INSERT INTO {_tblName} VALUES(NULL, @char_seq, @item_seq, @item_sn, @slot_id, @amount, @equipped, @color_id, @json_data, {peoriod}, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
        DateTime expiredTime = DateTimeHelper.Now + TimeSpan.FromDays(itemPeriod);

        short equipped = 0;

        var affected = await db.ExecuteAsync(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            item_sn = itemId,
            slot_id = slotId,
            amount,
            equipped,
            color_id = colorId,
            json_data = "",
        }, transaction);

        if (affected <= 0)
            return (ErrorCode.DbInsertedError, query, expiredTime);

        return (ErrorCode.Succeed, query, expiredTime);
    }

    public (ErrorCode, string, DateTime) DistributeItem<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        var peoriod = $"DATE_ADD(NOW(), INTERVAL {itemPeriod} DAY)";
        var query = $"INSERT INTO {_tblName} VALUES(NULL, @char_seq, @item_seq, @item_sn, @slot_id, @amount, @equipped, @color_id, @json_data, {peoriod}, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
        DateTime expiredTime = DateTimeHelper.Now + TimeSpan.FromDays(itemPeriod);

        short equipped = 0;

        var affected = db.Execute(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            item_sn = itemId,
            slot_id = slotId,
            amount,
            equipped,
            color_id = colorId,
            json_data = "",
        }, transaction);

        if (affected <= 0)
            return (ErrorCode.DbInsertedError, query, expiredTime);

        return (ErrorCode.Succeed, query, expiredTime);
    }

    // 아이템 기간을 늘린다.
    public async Task<(ErrorCode, string, DateTime, T?)> ExtendItemUsagePeriodAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int itemPeriod, int colorId)
    {
        var peoriod = $"DATE_ADD(expiration_date, INTERVAL {itemPeriod} DAY)";
        DateTime expiredTime = DateTimeHelper.Now + TimeSpan.FromDays(itemPeriod);

        var query = string.Empty;
        var affected = 0;
        if (colorId <= 0)
        {
            query = $"UPDATE {_tblName} SET expiration_date = {peoriod} WHERE char_seq = @char_seq AND item_sn = @item_sn LIMIT 1";
            affected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_sn = itemId,
            }, transaction);
        }
        else
        {
            query = $"UPDATE {_tblName} SET color_id = @color_id, expiration_date = {peoriod} WHERE char_seq = @char_seq AND item_sn = @item_sn LIMIT 1";
            affected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_sn = itemId,
                color_id = colorId,
            }, transaction);
        }

        // 만료 기간을 가져오자.
        {
            query = $"SELECT * FROM {_tblName} where char_seq = @char_seq and item_sn = @item_sn limit 1";

            var result = await db.QueryAsync<T>(query, new
            {
                char_seq = charSeq,
                item_sn = itemId,
            }, transaction);

            var tblItem = result.FirstOrDefault();
            if (tblItem == null)
                return (ErrorCode.DbInsertedError, query, expiredTime, default(T));


            return (ErrorCode.Succeed, query, expiredTime, tblItem);
        }
    }
    public (ErrorCode, string, DateTime, T?) ExtendItemUsagePeriod<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int itemPeriod, int colorId)
    {
        var peoriod = $"DATE_ADD(expiration_date, INTERVAL {itemPeriod} DAY)";
        DateTime expiredTime = DateTimeHelper.Now + TimeSpan.FromDays(itemPeriod);

        var query = string.Empty;
        var affected = 0;
        if (colorId <= 0)
        {
            query = $"UPDATE {_tblName} SET expiration_date = {peoriod} WHERE char_seq = @char_seq AND item_sn = @item_sn LIMIT 1";
            affected = db.Execute(query, new
            {
                char_seq = charSeq,
                item_sn = itemId,
            }, transaction);
        }
        else
        {
            query = $"UPDATE {_tblName} SET color_id = @color_id, expiration_date = {peoriod} WHERE char_seq = @char_seq AND item_sn = @item_sn LIMIT 1";
            affected = db.Execute(query, new
            {
                char_seq = charSeq,
                item_sn = itemId,
                color_id = colorId,
            }, transaction);
        }

        // 만료 기간을 가져오자.
        {
            query = $"SELECT * FROM {_tblName} where char_seq = @char_seq and item_sn = @item_sn limit 1";

            var result = db.Query<T>(query, new
            {
                char_seq = charSeq,
                item_sn = itemId,
            }, transaction);

            var tblItem = result.FirstOrDefault();
            if (tblItem == null)
                return (ErrorCode.DbInsertedError, query, expiredTime, default(T));


            return (ErrorCode.Succeed, query, expiredTime, tblItem);
        }
    }

    public async Task<T?> FetchItemByIdAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        var query = $"SELECT * FROM {_tblName} where char_seq = @char_seq and item_sn = @item_sn limit 1";
        var result = await db.QueryAsync<T>(query, new
        {
            char_seq = charSeq,
            item_sn = itemId,
        }, transaction);

        var tblItem = result.FirstOrDefault();
        return tblItem;


    }
    public async Task<T?> FetchInventoryItemAsync<T>(MySqlConnection db, ulong charSeq, ulong itemSeq)
    {
        var query = $"SELECT * FROM {_tblName} where char_seq = @char_seq and item_seq = @item_seq limit 1";
        var result = await db.QueryAsync<T>(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
        });

        var tblItem = result.FirstOrDefault();
        return tblItem;
    }
    public async Task<List<T>> FetchInventoryAllItemsAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq)
    {
        var query = $"SELECT * FROM {_tblName} where char_seq = @char_seq;";
        var result = await db.QueryAsync<T>(query, new
        {
            char_seq = charSeq,
        }, transaction);

        var tblItems = result.ToList();
        return tblItems;
    }
    public List<T> FetchInventoryAllItems<T>(MySqlConnection db, ulong charSeq)
    {
        var query = $"SELECT * FROM {_tblName} where char_seq = @char_seq;";
        var result = db.Query<T>(query, new
        {
            char_seq = charSeq,
        });

        var tblItems = result.ToList();
        return tblItems;
    }

    public async Task<List<T>> FetchInventoryEquippedItemsAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, short equipItem)
    {
        var equipped = equipItem > 0 ? 1 : 0;
        var query = $"SELECT * FROM {_tblName} where char_seq = @char_seq AND equipped = @equipped ;";
        var result = await db.QueryAsync<T>(query, new
        {
            char_seq = charSeq,
            equipped = (short)equipped,
        }, transaction);

        var tblItems = result.ToList();
        return tblItems;
    }
    public async Task<T?> FetchPrevEquippedItemAsync<T>(MySqlConnection db, ulong charSeq, ulong itemSeq, int slotId)
    {
        var query = $"SELECT * FROM {_tblName} where char_seq = @char_seq AND slot_id = @slot_id AND equipped = @equipped AND item_seq != @item_seq limit 1";
        var result = await db.QueryAsync<T>(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            slot_id = slotId,
            equipped = 1,
        });

        var tblItem = result.FirstOrDefault();
        return tblItem;
    }
    private async Task<ErrorCode> UnEquipedInventoryItemAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, ulong itemSeq, int slotId)
    {
        // 아이템 장착시, 벗기는 정보 추가
        var itemInfo = GetShopItemInfo(itemId);
        if (itemInfo.ItemSn == 0)
        {
            return ErrorCode.NotFoundItem;
        }

        var unEquipKey = slotId;
        var unEquipSlotIds = new HashSet<int> { slotId }; // 이미 장착 아이템을 해제해 준다.
        var instance = ItemDBLoaderHelper.Instance;

        if (instance.TblDesignUnequippedSetItem.TryGetValue(unEquipKey, out var unequippedSetItems))
        {
            foreach (var unequippedSetItem in unequippedSetItems)
            {
                unEquipSlotIds.Add(unequippedSetItem.UnequipSlotId);
            }
        }

        foreach (var unEquipSlotId in unEquipSlotIds)
        {
            if (unEquipSlotId == 0)
                continue;

            if (instance.TblDesignSlotToItemType.TryGetValue(unEquipSlotId, out var slotToType) == false)
                continue;

            var itemType = slotToType.ItemType;
            var tblName = ItemTypeToTblName(itemType);
            if (string.IsNullOrEmpty(tblName))
                continue;

            var query = $"UPDATE {tblName} SET equipped = @equipped where char_seq = @char_seq AND slot_id = @slot_id AND item_seq != @item_seq";

            int rowsAffected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                slot_id = unEquipSlotId,
                equipped = 0,
            }, transaction);
        }

        return ErrorCode.Succeed;
    }

    private ErrorCode UnEquipedInventoryItem(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, ulong itemSeq, int slotId)
    {
        // 아이템 장착시, 벗기는 정보 추가
        var itemInfo = GetShopItemInfo(itemId);
        if (itemInfo.ItemSn == 0)
        {
            return ErrorCode.NotFoundItem;
        }

        var unEquipKey = slotId;
        var unEquipSlotIds = new HashSet<int> { slotId }; // 이미 장착 아이템을 해제해 준다.
        var instance = ItemDBLoaderHelper.Instance;

        if (instance.TblDesignUnequippedSetItem.TryGetValue(unEquipKey, out var unequippedSetItems))
        {
            foreach (var unequippedSetItem in unequippedSetItems)
            {
                unEquipSlotIds.Add(unequippedSetItem.UnequipSlotId);
            }
        }

        foreach (var unEquipSlotId in unEquipSlotIds)
        {
            if (unEquipSlotId == 0)
                continue;

            if (instance.TblDesignSlotToItemType.TryGetValue(unEquipSlotId, out var slotToType) == false)
                continue;

            var itemType = slotToType.ItemType;
            var tblName = ItemTypeToTblName(itemType);
            if (string.IsNullOrEmpty(tblName))
                continue;

            var query = $"UPDATE {tblName} SET equipped = @equipped where char_seq = @char_seq AND slot_id = @slot_id AND item_seq != @item_seq";

            int rowsAffected = db.Execute(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                slot_id = unEquipSlotId,
                equipped = 0,
            }, transaction);
        }

        return ErrorCode.Succeed;
    }
    private string ItemTypeToTblName(MainItemType itemType)
    {
        var tblName = itemType switch
        {
            MainItemType.Clothing => "tbl_inventory_clothing",
            MainItemType.Accessory => "tbl_inventory_accessory",
            MainItemType.Character => "tbl_inventory_character",
            _ => string.Empty
        };
        return tblName;
    }
    public async Task<ErrorCode> UpdateInventoryEquipedAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, ulong itemSeq, int slotId, short equipped)
    {
        if (equipped > 0)
        {
            // 관련 아이템들을 해제 시킨다.
            var errorCode1 = await UnEquipedInventoryItemAsync(db, transaction, charSeq, itemId, itemSeq, slotId);
        }

        // 아이템 착용 정보 변경
        {
            var query = $"UPDATE {_tblName} SET equipped = @equipped where char_seq = @char_seq AND item_seq = @item_seq";

            int rowsAffected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                slot_id = slotId,
                equipped,
            }, transaction);
        }

        return ErrorCode.Succeed;
    }
    public ErrorCode UpdateInventoryEquiped<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, ulong itemSeq, int slotId, short equipped)
    {
        if (equipped > 0)
        {
            // 관련 아이템들을 해제 시킨다.
            var errorCode1 = UnEquipedInventoryItem(db, transaction, charSeq, itemId, itemSeq, slotId);
        }

        // 아이템 착용 정보 변경
        {
            var query = $"UPDATE {_tblName} SET equipped = @equipped where char_seq = @char_seq AND item_seq = @item_seq";

            int rowsAffected = db.Execute(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                slot_id = slotId,
                equipped,
            }, transaction);
        }

        return ErrorCode.Succeed;
    }
    public async Task<int> FetchHasCountAsync<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        var query = $"SELECT COUNT(*) as Count FROM {_tblName} WHERE char_seq = @char_seq AND item_sn = @item_sn";

        var queryResult = await db.QueryAsync<JoinQuery.CheckedCount>(query, new
        {
            char_seq = charSeq,
            item_sn = itemId,
        }, transaction);
        var result = queryResult.FirstOrDefault();
        if (result != null)
            return result.Count;

        return 0;
    }

    public int FetchHasCount(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        var query = $"SELECT COUNT(*) as Count FROM {_tblName} WHERE char_seq = @char_seq AND item_sn = @item_sn";

        var queryResult = db.Query<JoinQuery.CheckedCount>(query, new
        {
            char_seq = charSeq,
            item_sn = itemId,
        }, transaction);
        var result = queryResult.FirstOrDefault();
        if (result != null)
            return result.Count;

        return 0;
    }

    // 염색
    public async Task<(ErrorCode, string, DateTime, T?)> DyeItem<T>(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, ulong itemSeq, string jsonData, int colorId)
    {
        DateTime expiredTime = DateTimeHelper.Now;
        var query = $"UPDATE {_tblName} SET json_data = @json_data, color_id = @color_id where char_seq = @char_seq AND item_seq = @item_seq ";

        int rowsAffected = await db.ExecuteAsync(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            json_data = jsonData,
            color_id = colorId,
        }, transaction);

        // 만료 기간을 가져오자.
        {
            query = $"SELECT * FROM {_tblName} where char_seq = @char_seq AND item_seq = @item_seq limit 1";

            var result = await db.QueryAsync<T>(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
            }, transaction);

            var tblItem = result.FirstOrDefault();
            if (tblItem == null)
                return (ErrorCode.DbInsertedError, query, expiredTime, default(T));


            return (ErrorCode.Succeed, query, expiredTime, tblItem);
        }
    }
}
