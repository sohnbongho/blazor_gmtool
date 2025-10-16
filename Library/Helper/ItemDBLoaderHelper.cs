using Dapper;
using Library.Connector;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using MySqlConnector;
using System.Collections.Concurrent;

namespace Library.Helper;

/// <summary>
/// DB에서 사전에 미리 아이템 정보를 읽어오는 곳
/// </summary>
public sealed class ItemDBLoaderHelper
{
    private static readonly Lazy<ItemDBLoaderHelper> lazy = new Lazy<ItemDBLoaderHelper>(() => new ItemDBLoaderHelper());

    public static ItemDBLoaderHelper Instance { get { return lazy.Value; } }

    // 전체 아이템 정보
    public ConcurrentDictionary<int, IDesignBaseItemInfo> DesignBaseItemInfo => _designBaseItemInfo;
    private ConcurrentDictionary<int, IDesignBaseItemInfo> _designBaseItemInfo = new();

    // 아이템 컬러 정보
    public ConcurrentDictionary<int, TblDesignItemColorPalette> TblDesignItemColorPalette => _tblDesignItemColorPalette;
    private ConcurrentDictionary<int, TblDesignItemColorPalette> _tblDesignItemColorPalette = new();

    public ConcurrentDictionary<string, TblDesignInAppCurrency> TblDesignInAppCurrency => _tblDesignInAppCurrency;
    private ConcurrentDictionary<string, TblDesignInAppCurrency> _tblDesignInAppCurrency = new();

    public ConcurrentDictionary<int, List<TblDesignPackage>> TblDesignPackage => _tblDesignPackage;
    private ConcurrentDictionary<int, List<TblDesignPackage>> _tblDesignPackage = new();

    // 아이템 탈착 정보
    public ConcurrentDictionary<int, List<DesignTblUnequippedSetItem>> TblDesignUnequippedSetItem => _tblDesignUnequippedSetItem;
    private ConcurrentDictionary<int, List<DesignTblUnequippedSetItem>> _tblDesignUnequippedSetItem = new();

    // slot별에 ItemType정보
    public ConcurrentDictionary<int, DesignSlotToItemType> TblDesignSlotToItemType => _tblDesignSlotToItemType;
    private ConcurrentDictionary<int, DesignSlotToItemType> _tblDesignSlotToItemType = new();

    // 아이템 초기 지급 정보
    public ConcurrentBag<TblDesignInitialProvide> TblDesignInitialProvide => _tblDesignInitialProvide;
    private ConcurrentBag<TblDesignInitialProvide> _tblDesignInitialProvide = new();

    // 다이아 페스트
    public ConcurrentDictionary<string, TblDesignDiaFesta> TblDesignDiaFesta => _tblDesignDiaFesta;
    private ConcurrentDictionary<string, TblDesignDiaFesta> _tblDesignDiaFesta = new();

    public ConcurrentDictionary<string, TblDesignGoldFesta> TblDesignGoldFesta => _tblDesignGoldFesta;
    private ConcurrentDictionary<string, TblDesignGoldFesta> _tblDesignGoldFesta = new();

    // 아이템 세일 정보
    public ConcurrentDictionary<int, TblDesignItemSale> TblDesignItemSale => _tblDesignItemSale;
    private ConcurrentDictionary<int, TblDesignItemSale> _tblDesignItemSale = new();


    private ItemDBLoaderHelper()
    {
    }
    public bool Load()
    {
        using (var db = MySqlConnectionHelper.Instance.ConnectionFactory(DbConnectionType.Design))
        {
            if (db == null)
                throw new Exception($"db is null");

            if (false == LoadTblDesignInAppCurrency(db))
                throw new Exception($"fail LoadTblDesignInAppCurrency");

            var (succeed3, errorStr3) = LoadDesignInitialProvide(db);
            if (false == succeed3)
                throw new Exception(errorStr3);
        }

        return true;
    }


    public bool LoadTblDesignInAppCurrency(MySqlConnection db)
    {
        var query = $"select * from tbl_design_in_app_currency";

        var objects = db.Query<TblDesignInAppCurrency>(query);
        foreach (var obj in objects)
        {
            _tblDesignInAppCurrency[obj.product_id] = obj;
        }

        return true;
    }

    public (bool, TblDesignItemSale) GetSaleItemInfo(int itemSn)
    {
        if (_tblDesignItemSale.TryGetValue(itemSn, out var itemInfo))
        {
            return (true, itemInfo);
        }
        return (false, new TblDesignItemSale());
    }

    public MainItemType GetItemType(int itemId)
    {
        if (_designBaseItemInfo.TryGetValue(itemId, out var info))
            return info.ItemType;

        return MainItemType.None;

    }

    public List<(int, int)> GetInAppItems(string productId)
    {
        var items = new List<(int, int)>();

        if (false == _tblDesignInAppCurrency.TryGetValue(productId, out var tblDesignInAppCurrency))
            return new List<(int, int)>();

        var groupId = tblDesignInAppCurrency.group_id;
        if (false == _tblDesignPackage.TryGetValue(groupId, out var tblDesignPackages))
            return new List<(int, int)>();

        foreach (var tblDesignPackage in tblDesignPackages)
        {
            var itemId = tblDesignPackage.item_sn;
            var amount = tblDesignPackage.amount;

            items.Add((itemId, amount));
        }

        return items;
    }

    public List<(int, int)> GetGroupIdItems(int groupId)
    {
        var items = new List<(int, int)>();

        if (false == _tblDesignPackage.TryGetValue(groupId, out var tblDesignPackages))
            return new List<(int, int)>();

        foreach (var tblDesignPackage in tblDesignPackages)
        {
            var itemId = tblDesignPackage.item_sn;
            var amount = tblDesignPackage.amount;

            items.Add((itemId, amount));
        }

        return items;
    }

    /// <summary>
    /// 캐릭터 생성시, 처음 지급되는 정보
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public (bool, string) LoadDesignInitialProvide(MySqlConnection db)
    {
        try
        {
            var query = $"select * from tbl_design_initial_provide";

            var objects = db.Query<TblDesignInitialProvide>(query);
            foreach (var obj in objects)
            {
                _tblDesignInitialProvide.Add(obj);
            }
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }


    public string GetItemName(int itemId)
    {
        if (0 == itemId)
            return $"{itemId}";

        if (false == _designBaseItemInfo.TryGetValue(itemId, out var info))
            return $"({itemId})";

        if (string.IsNullOrEmpty(info.ItemName))
            return $"noname({itemId})";

        return $"{info.ItemName}({itemId})";
    }

    public CurrencyType GetItemPriceType(int itemId)
    {
        if (0 == itemId)
            return CurrencyType.None;

        if (false == _designBaseItemInfo.TryGetValue(itemId, out var info))
            return CurrencyType.None;

        if (info is DesignWearableItemInfo wearableItemInfo)
        {
            return wearableItemInfo.CurrencyType;
        }
        else if (info is DesignBackgroundItemInfo backgroundItemInfo)
        {
            return backgroundItemInfo.CurrencyType;
        }
        else if (info is DesignShowRoomItemInfo showRoomItemInfo)
        {
            return showRoomItemInfo.CurrencyType;
        }
        else if (info is DesignBackgroundPropItemInfo backgroundPropItemInfo)
        {
            return backgroundPropItemInfo.CurrencyType;
        }
        else if (info is DesignSoftCatItemInfo softCatItemInfo)
        {
            return softCatItemInfo.CurrencyType;
        }
        else if (info is DesignDiggingWarrior diggingWarrior)
        {
            return diggingWarrior.CurrencyType;
        }
        else if (info is PackageItem packageItem)
        {
            return packageItem.CurrencyType;
        }
        return CurrencyType.None;

    }
    public uint GetItemPrice(int itemId)
    {
        if (0 == itemId)
            return 0;

        if (false == _designBaseItemInfo.TryGetValue(itemId, out var info))
            return 0;

        if (info is DesignWearableItemInfo wearableItemInfo)
        {
            return wearableItemInfo.ItemPrice;
        }
        else if (info is DesignBackgroundItemInfo backgroundItemInfo)
        {
            return backgroundItemInfo.ItemPrice;
        }
        else if (info is DesignShowRoomItemInfo showRoomItemInfo)
        {
            return showRoomItemInfo.ItemPrice;
        }
        else if (info is DesignBackgroundPropItemInfo backgroundPropItemInfo)
        {
            return backgroundPropItemInfo.ItemPrice;
        }
        else if (info is DesignSoftCatItemInfo softCatItemInfo)
        {
            return softCatItemInfo.ItemPrice;
        }
        else if (info is DesignDiggingWarrior diggingWarrior)
        {
            return diggingWarrior.ItemPrice;
        }
        else if (info is PackageItem packageItem)
        {
            return packageItem.ItemPrice;
        }
        return 0;

    }
}
