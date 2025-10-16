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

public class PackageItemPolicy : IItemPolicy, IDistributableItem
{
    public MainItemType ItemType { get; } = MainItemType.Package;
    public string TblName { get; } = string.Empty;
    private ItemSharedRepo? _itemRepository;

    public PackageItemPolicy()
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

    public Task<(ErrorCode, string, DateTime)> DistributeItemAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {        
        var result = (ErrorCode.NotFoundItem, string.Empty, DateTimeHelper.Now);
        
        return Task.FromResult(result);
    }

    public (ErrorCode, string, DateTime) DistributeItem(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, int itemId, int slotId, ulong itemSeq, int amount, int itemPeriod, int colorId)
    {
        return (ErrorCode.NotFoundItem, string.Empty, DateTimeHelper.Now);
    }
}
