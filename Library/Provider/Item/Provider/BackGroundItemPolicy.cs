using Messages;
using MySqlConnector;
using Library.DTO;
using Dapper;
using Library.Data.Models;
using Library.Provider.Item.Interface;
using Library.Repository.Item;
using Library.DBTables.MySql;
using Library.Helper;

namespace Library.Provider.Item.Provider;

public class BackGroundItemPolicy : IItemPolicy, IDistributableItem, IInventoryHasItem, IEquipableItem, IVisualItem
{
    public MainItemType ItemType { get; } = MainItemType.BackGround;
    public string TblName { get; } = "tbl_inventory_background";
    private ItemSharedRepo? _itemRepository;

    public BackGroundItemPolicy()
    {
        _itemRepository = new ItemSharedRepo(ItemType, TblName);
    }
    public void Dispose()
    {
        _itemRepository = null;
    }

    public IDesignBaseItemInfo GetShopItemInfo(int itemId)
    {
        if (_itemRepository == null)
            return DesignNoneItemInfo.Instance;

        return _itemRepository.GetShopItemInfo(itemId);
    }

    public async Task<int> FetchHasCountAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        if (_itemRepository == null)
            return 0;

        var count = await _itemRepository.FetchHasCountAsync<TblInventoryBackground>(db, transaction, charSeq, itemId);
        return count;
    }

    /// <summary>
    /// 아이템 지급
    /// </summary>    
    public async Task<(ErrorCode, string, DateTime)> DistributeItemAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        var query = $"INSERT INTO {TblName} VALUES(NULL, @char_seq, @item_seq, @item_sn, @slot_id, @equipped, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
        DateTime expiredTime = DateTimeHelper.Now + TimeSpan.FromDays(itemPeriod);

        short equipped = 0;

        var affected = await db.ExecuteAsync(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            item_sn = itemId,
            slot_id = slotId,
            equipped,
        }, transaction);

        if (affected <= 0)
            return (ErrorCode.DbInsertedError, query, expiredTime);

        return (ErrorCode.Succeed, query, expiredTime);
    }
    public (ErrorCode, string, DateTime) DistributeItem(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        var query = $"INSERT INTO {TblName} VALUES(NULL, @char_seq, @item_seq, @item_sn, @slot_id, @equipped, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
        DateTime expiredTime = DateTimeHelper.Now + TimeSpan.FromDays(itemPeriod);

        short equipped = 0;

        var affected = db.Execute(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            item_sn = itemId,
            slot_id = slotId,
            equipped,
        }, transaction);

        if (affected <= 0)
            return (ErrorCode.DbInsertedError, query, expiredTime);

        return (ErrorCode.Succeed, query, expiredTime);
    }

    public async Task<InventoryItem> FetchInventoryItemAsync(MySqlConnection db, ulong charSeq, ulong itemSeq)
    {
        if (_itemRepository == null)
            return new InventoryItem();

        var tblItem = await _itemRepository.FetchInventoryItemAsync<TblInventoryBackground>(db, charSeq, itemSeq);
        if (tblItem == null)
            return new InventoryItem();

        return new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = 1,
            SlotId = 0,
            JsonData = string.Empty,
            ColorId = 0,

            ExpiredDate = string.Empty,
        };
    }
    public async Task<List<InventoryItem>> FetchInventoryAllItemsAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq)
    {
        if (_itemRepository == null)
            return new List<InventoryItem>();

        var tblItems = await _itemRepository.FetchInventoryAllItemsAsync<TblInventoryBackground>(db, transaction, charSeq);

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
    
    public List<InventoryItem> FetchInventoryAllItems(MySqlConnection db, ulong charSeq)
    {
        if (_itemRepository == null)
            return new List<InventoryItem>();

        var tblItems = _itemRepository.FetchInventoryAllItems<TblInventoryBackground>(db, charSeq);

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

    public async Task<InventoryItem> FetchItemByIdAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId)
    {
        if (_itemRepository == null)
            return new InventoryItem();

        var tblItem = await _itemRepository.FetchItemByIdAsync<TblInventoryBackground>(db, transaction, charSeq, itemId);
        if (tblItem == null)
            return new InventoryItem();

        return new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = 1,
            SlotId = 0,
            JsonData = string.Empty,
            ColorId = 0,

            ExpiredDate = string.Empty,
        };
    }

    public async Task<ErrorCode> UpdateInventoryEquipedAsync(MySqlConnection db, MySqlTransaction transaction, 
        ulong charSeq, int itemId, ulong itemSeq, int slotId, short equipItem)
    {
        // 기존 아이템들 0으로 설정
        {
            var query = $"UPDATE {TblName} SET equipped = @equipped where char_seq = @char_seq AND slot_id = @slot_id AND item_seq != @item_seq";

            int rowsAffected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                equipped = 0,
                slot_id = slotId,
            }, transaction);
        }

        // 아이템 착용 정보 변경        
        {
            var query = $"UPDATE {TblName} SET equipped = @equipped where char_seq = @char_seq AND item_seq = @item_seq";

            int rowsAffected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                slot_id = slotId,
                equipped = equipItem,
            }, transaction);
        }

        return ErrorCode.Succeed;
    }
    public async Task<InventoryItem> FetchPrevEquippedItemAsync(MySqlConnection db, ulong charSeq, ulong itemSeq, int slotId)
    {
        var query = $"SELECT * FROM {TblName} where char_seq = @char_seq AND equipped = @equipped AND item_seq != @item_seq limit 1";
        var result = await db.QueryAsync<TblInventoryBackground>(query, new
        {
            char_seq = charSeq,
            item_seq = itemSeq,
            equipped = 1,
        });

        var tblItem = result.FirstOrDefault();
        if (tblItem == null)
            return new InventoryItem();

        return new InventoryItem
        {
            ItemSeq = tblItem.item_seq.ToString(),
            ItemType = (int)ItemType,
            ItemSn = tblItem.item_sn,
            Equip = tblItem.equipped,
            Amount = 1,
            SlotId = 0,
            JsonData = string.Empty,
            ColorId = 0,

            ExpiredDate = string.Empty,
        };
    }
}
