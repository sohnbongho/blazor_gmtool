using Dapper;
using Library.Helper;
using Messages;
using MySqlConnector;
using Library.DTO;
using Library.Repository.Mysql;
using Library.Data.Models;
using Library.Provider.Item.Interface;
using Library.Repository.Item;
using Library.DBTables.MySql;
using Library.Repository.Mysql.Currency;

namespace Library.Provider.Item.Provider;


/// <summary>
/// Currency 타입
/// </summary>

public class CurrencyItemPolicy : IItemPolicy, IRewardableCurrencyItem, IDistributableCurrencyItem
{
    public MainItemType ItemType { get; } = MainItemType.Currency;
    public string TblName { get; } = "tbl_member";
    private ItemSharedRepo? _itemRepo;
    private CurrencySharedRepo? _currencyRepo;

    public CurrencyItemPolicy()
    {
        _itemRepo = new ItemSharedRepo(ItemType, TblName);
        _currencyRepo = CurrencySharedRepo.Of();
    }
    public void Dispose()
    {
        _itemRepo = null;
        _currencyRepo = null;
    }

    public IDesignBaseItemInfo GetShopItemInfo(int itemId)
    {
        if (_itemRepo == null)
            return DesignNoneItemInfo.Instance;

        return _itemRepo.GetShopItemInfo(itemId);
    }

    public async Task<(ErrorCode, string, DateTime)> DistributeItemAsync(MySqlConnection db, MySqlTransaction transaction, 
        ItemGainReason reason, ulong charSeq, int itemId, int itemCategory, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        DateTime expiredTime = DateTimeHelper.Now;

        var query = $"select * from tbl_member where char_seq = @char_seq limit 1;";
        var result = await db.QueryAsync<TblMember>(query, new TblMember { char_seq = charSeq }, transaction);
        var tblMember = result.FirstOrDefault();
        if (tblMember == null)
        {
            return (ErrorCode.NotFoundCharacter, query, expiredTime);
        }
        var userSeq = tblMember.user_seq;
        var (errorCode, inventoryItem) = await RewardItemAsync(db, transaction, reason, userSeq, charSeq, itemId, amount);
        return (errorCode, query, expiredTime);
    }
    
    public (ErrorCode, string, DateTime) DistributeItem(MySqlConnection db, MySqlTransaction transaction, 
        ItemGainReason reason, ulong charSeq, int itemId, int itemCategory, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        DateTime expiredTime = DateTimeHelper.Now;

        var query = $"select * from tbl_member where char_seq = @char_seq limit 1;";
        var result = db.Query<TblMember>(query, new TblMember { char_seq = charSeq }, transaction);
        var tblMember = result.FirstOrDefault();
        if (tblMember == null)
        {
            return (ErrorCode.NotFoundCharacter, query, expiredTime);
        }
        var userSeq = tblMember.user_seq;
        var (errorCode, inventoryItem) = RewardItem(db, transaction, reason, userSeq, charSeq, itemId, amount);
        return (errorCode, query, expiredTime);
    }

    public async Task<(ErrorCode, InventoryItem)> RewardItemAsync(MySqlConnection db, MySqlTransaction transaction, 
        ItemGainReason reason, ulong userSeq, ulong charSeq, int itemId, int amount)
    {
        var currencyType = ConvertHelper.ItemSnToCurrencyType(itemId);
        if(currencyType == CurrencyType.None)
        {
            return (ErrorCode.NotFoundCurrencyType, new InventoryItem { });
        }
        if (_currencyRepo == null)
            return (ErrorCode.NotFoundCurrencyType, new InventoryItem { });

        var (errorCode, totalAmount) = await _currencyRepo.IncreaseAsync(db, transaction, reason, currencyType, userSeq, amount);
        if (errorCode != ErrorCode.Succeed)
        {
            return (errorCode, new InventoryItem { });
        }

        return (ErrorCode.Succeed, new InventoryItem
        {
            ItemSeq = string.Empty,
            ItemType = (int)ItemType,
            ItemSn = itemId,
            Equip = 0,
            Amount = (int)totalAmount,
            SlotId = 0,
            JsonData = string.Empty,
            ColorId = 0,

        });
    }
    public (ErrorCode, InventoryItem) RewardItem(MySqlConnection db, MySqlTransaction transaction, ItemGainReason reason, ulong userSeq, ulong charSeq, int itemId, int amount)
    {
        var currencyType = ConvertHelper.ItemSnToCurrencyType(itemId);
        if (currencyType == CurrencyType.None)
        {
            return (ErrorCode.NotFoundCurrencyType, new InventoryItem { });
        }

        if (_currencyRepo == null)
            return (ErrorCode.NotFoundCurrencyType, new InventoryItem { });

        var (errorCode, totalAmount) = _currencyRepo.Increase(db, transaction, reason,currencyType, userSeq, amount);
        if (errorCode != ErrorCode.Succeed)
        {
            return (errorCode, new InventoryItem { });
        }

        return (ErrorCode.Succeed, new InventoryItem
        {
            ItemSeq = string.Empty,
            ItemType = (int)ItemType,
            ItemSn = itemId,
            Equip = 0,
            Amount = (int)totalAmount,
            SlotId = 0,
            JsonData = string.Empty,
            ColorId = 0,

        });
    }

}
