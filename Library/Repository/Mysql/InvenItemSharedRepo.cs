using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.Provider.Item;
using Library.Provider.Item.Interface;
using log4net;
using Messages;
using System.Reflection;

namespace Library.Repository.Mysql;

public class InvenItemSharedRepo : MySqlDbCommonRepo
{
    public static InvenItemSharedRepo Of() => new InvenItemSharedRepo();
    private InvenItemSharedRepo()
    {

    }

    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    /// <summary>
    /// 인벤토리 아이템 정보들 얻어오기
    /// </summary>
    /// <param name="charSeq"></param>
    /// <param name="equipped"></param>
    /// <returns></returns>
    public List<InventoryItem> FetchInvenItmes(ulong charSeq, short equipped)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }
            var query = string.Empty;
            var invenItems = new List<InventoryItem>();

            // character 아이템
            {
                query = $"SELECT * from tbl_inventory_character WHERE char_seq = @char_seq and equipped = @equipped;";

                var result = db.Query<TblInventoryCharacter>(query, new TblInventoryitem
                {
                    char_seq = charSeq,
                    equipped = equipped,
                });
                var items = result.ToList();
                items.ForEach(item => invenItems.Add(new InventoryItem
                {
                    ItemType = (int)MainItemType.Character,
                    ItemSeq = item.item_seq.ToString(),

                    ItemSn = item.item_sn,
                    Equip = item.equipped,
                    Amount = item.amount,
                    SlotId = item.slot_id,
                    JsonData = item.json_data,
                    ColorId = item.color_id,

                    ExpiredDate = item.expiration_date.ToString(ConstInfo.TimeFormat),
                }));
            }

            // cloting아이템
            {
                query = $"SELECT * from tbl_inventory_clothing WHERE char_seq = @char_seq and equipped = @equipped;";

                var result = db.Query<TblInventoryClothing>(query, new TblInventoryitem
                {
                    char_seq = charSeq,
                    equipped = equipped,
                });
                var items = result.ToList();
                items.ForEach(item => invenItems.Add(new InventoryItem
                {
                    ItemType = (int)MainItemType.Clothing,
                    ItemSeq = item.item_seq.ToString(),

                    ItemSn = item.item_sn,
                    Equip = item.equipped,
                    Amount = item.amount,
                    SlotId = item.slot_id,
                    JsonData = item.json_data,
                    ColorId = item.color_id,

                    ExpiredDate = item.expiration_date.ToString(ConstInfo.TimeFormat),
                }));
            }

            // Accessory
            {
                query = $"SELECT * from tbl_inventory_accessory WHERE char_seq = @char_seq and equipped = @equipped;";

                var result = db.Query<TblInventoryAccessory>(query, new TblInventoryitem
                {
                    char_seq = charSeq,
                    equipped = equipped,
                });
                var items = result.ToList();
                items.ForEach(item => invenItems.Add(new InventoryItem
                {
                    ItemType = (int)MainItemType.Accessory,
                    ItemSeq = item.item_seq.ToString(),

                    ItemSn = item.item_sn,
                    Equip = item.equipped,
                    Amount = item.amount,
                    SlotId = item.slot_id,
                    JsonData = item.json_data,
                    ColorId = item.color_id,

                    ExpiredDate = item.expiration_date.ToString(ConstInfo.TimeFormat),
                }));
            }
            return invenItems;
        }
    }
    public async Task<List<InventoryItem>> FetchInvenItmesAsync(ulong charSeq, short equipped)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }
            var query = string.Empty;
            var invenItems = new List<InventoryItem>();

            // character 아이템
            {
                query = $"SELECT * from tbl_inventory_character WHERE char_seq = @char_seq and equipped = @equipped;";

                var result = await db.QueryAsync<TblInventoryCharacter>(query, new TblInventoryitem
                {
                    char_seq = charSeq,
                    equipped = equipped,
                });
                var items = result.ToList();
                items.ForEach(item => invenItems.Add(new InventoryItem
                {
                    ItemType = (int)MainItemType.Character,
                    ItemSeq = item.item_seq.ToString(),

                    ItemSn = item.item_sn,
                    Equip = item.equipped,
                    Amount = item.amount,
                    SlotId = item.slot_id,
                    JsonData = item.json_data,
                    ColorId = item.color_id,

                    ExpiredDate = item.expiration_date.ToString(ConstInfo.TimeFormat),
                }));
            }

            // cloting아이템
            {
                query = $"SELECT * from tbl_inventory_clothing WHERE char_seq = @char_seq and equipped = @equipped;";

                var result = await db.QueryAsync<TblInventoryClothing>(query, new TblInventoryitem
                {
                    char_seq = charSeq,
                    equipped = equipped,
                });
                var items = result.ToList();
                items.ForEach(item => invenItems.Add(new InventoryItem
                {
                    ItemType = (int)MainItemType.Clothing,
                    ItemSeq = item.item_seq.ToString(),

                    ItemSn = item.item_sn,
                    Equip = item.equipped,
                    Amount = item.amount,
                    SlotId = item.slot_id,
                    JsonData = item.json_data,
                    ColorId = item.color_id,

                    ExpiredDate = item.expiration_date.ToString(ConstInfo.TimeFormat),
                }));
            }

            // Accessory
            {
                query = $"SELECT * from tbl_inventory_accessory WHERE char_seq = @char_seq and equipped = @equipped;";

                var result = await db.QueryAsync<TblInventoryAccessory>(query, new TblInventoryitem
                {
                    char_seq = charSeq,
                    equipped = equipped,
                });
                var items = result.ToList();
                items.ForEach(item => invenItems.Add(new InventoryItem
                {
                    ItemType = (int)MainItemType.Accessory,
                    ItemSeq = item.item_seq.ToString(),

                    ItemSn = item.item_sn,
                    Equip = item.equipped,
                    Amount = item.amount,
                    SlotId = item.slot_id,
                    JsonData = item.json_data,
                    ColorId = item.color_id,

                    ExpiredDate = item.expiration_date.ToString(ConstInfo.TimeFormat),
                }));
            }
            return invenItems;
        }
    }

    /// <summary>
    /// ingame 파츠
    /// </summary>    
    public async Task<TblInventorySeasonitemParts> FetchInvenSeasonItemPartsAsync(ulong charSeq, GameModeType gameModeType)
    {
        if (gameModeType == GameModeType.None)
        {
            return new TblInventorySeasonitemParts();
        }

        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_inventory_seasonitem_parts where char_seq = @char_seq and game_mode_type = @game_mode_type limit 1";

            var result = await db.QueryAsync<TblInventorySeasonitemParts>(query, new TblInventorySeasonitemParts
            {
                char_seq = charSeq,
                game_mode_type = (int)gameModeType,
            });
            var inven = result.FirstOrDefault();

            return inven != null ? inven : new TblInventorySeasonitemParts();
        }
    }
    public TblInventoryInfo FetchInvenInfo(ulong charSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_inventory_info where char_seq = {charSeq} limit 1";

            var inven = db.Query<TblInventoryInfo>(query).FirstOrDefault();

            return inven != null ? inven : new TblInventoryInfo();
        }
    }


    public List<TblInventoryConsumable> GetInvenConsumables(ulong charSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_inventory_consumable where char_seq={charSeq};";
            return db.Query<TblInventoryConsumable>(query).ToList();
        }
    }
    public bool ExpandItemInven(ulong charSeq, int added)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            using var transaction = db.BeginTransaction();
            try
            {
                //tbl_inventory_info
                var query = $"UPDATE tbl_inventory_info SET inven_size = inven_size + {added} WHERE char_seq={charSeq};";

                int rowsAffected = db.Execute(query, null, transaction);
                if (rowsAffected <= 0)
                {
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to add inven size", ex);
                transaction.Rollback();
                return false;
            }
        }
        return true;
    }
    public List<InventoryItem> FetchGameModeInvenItmes(GameModeType gameModeType, ulong charSeq)
    {
        var mainItemType = MainItemType.None;
        if (gameModeType == GameModeType.SoftCat)
        {
            mainItemType = MainItemType.SoftCat;
        }
        else
        {
            return new List<InventoryItem>();
        }        
        var policy = ItemPolicyFactory.GetPolicy(mainItemType);

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new List<InventoryItem>();
            }

            try
            {
                if (policy is IInventoryHasItem iInven)
                {
                    return iInven.FetchInventoryAllItems(db, charSeq);
                }
                return new List<InventoryItem>();

            }
            catch (Exception ex)
            {
                _logger.Error($"failed to add inven size", ex);
                return new List<InventoryItem>();
            }
        }

    }
    public ErrorCode UpdateInGameEquiped(GameModeType gameModeType, ulong charSeq, int itemId, ulong itemSeq, int slotId, short equipped, string ingameParts)
    {
        var mainItemType = MainItemType.None;
        if (gameModeType == GameModeType.SoftCat)
        {
            mainItemType = MainItemType.SoftCat;
        }
        else
        {
            return ErrorCode.NotFoundItem;
        }        
        var policy = ItemPolicyFactory.GetPolicy(mainItemType);

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return ErrorCode.NotFoundItem;

            using var transaction = db.BeginTransaction();

            try
            {
                if (policy is IInGameEquipableItem iEquip)
                {
                    var errorCode = iEquip.UpdateInventoryEquiped(db, transaction, gameModeType, charSeq, itemId, itemSeq, slotId, equipped, ingameParts);
                    if(errorCode != ErrorCode.Succeed) 
                    {
                        transaction.Rollback();
                        return errorCode;
                    }
                    transaction.Commit();
                }

                return ErrorCode.Succeed;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Error($"failed to UpdateInventoryEquiped", ex);
                return ErrorCode.NotFoundItem;
            }
        }
    }

}
