using Messages;
using MySqlConnector;
using Library.DTO;
using Library.Data.Models;
using Library.Repository.Item;
using Library.Provider.Item.Interface;

namespace Library.Provider.Item.Provider;

public class NoneItemPolicy : IItemPolicy
{
    public MainItemType ItemType { get; } = MainItemType.None;
    public string TblName { get; } = string.Empty;
    private ItemSharedRepo? _itemRepository;
    public NoneItemPolicy()
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
}
