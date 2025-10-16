using Dapper;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using Messages;
using MySqlConnector;

namespace Library.Repository.Mysql.Inventory;

public class DiggingWarriorInventory : IGameModeInventory, IEquippedInventory
{
    public static DiggingWarriorInventory Of() => new DiggingWarriorInventory();
    private DiggingWarriorInventory()
    {

    }


    private GameModeType _gameModeType = GameModeType.DiggingWarrior;
    public (List<SeasonUniqueItem>, List<SeasonOverlapItem>) FetchItems(MySqlConnection db, ulong charSeq)
    {
        var uniqueItems = new List<SeasonUniqueItem>();
        var overlapItems = new List<SeasonOverlapItem>();
        var gameModeType = _gameModeType;

        var query = $"select * from tbl_inventory_seasonitem where char_seq = @char_seq and game_mode_type = @game_mode_type;";
        var items = db.Query<TblInventorySeasonitem>(query, new
        {
            char_seq = charSeq,
            game_mode_type = (int)gameModeType,
        });
        foreach (var item in items)
        {
            uniqueItems.Add(new SeasonUniqueItem
            {
                BaseItem = new BaseItem
                {
                    ItemSeq = item.item_seq,
                    ItemSn = item.item_sn,
                    Count = 1,
                },
                CharSeq = charSeq,
                GameModeType = gameModeType,
                Favorites = item.favorites,
                Equipped = item.equipped,
                DbStoredAmount = item.stored_amount,
                AddedStoredAmount = 0,
                SubStoredAmount = 0,
            });
        }
        return (uniqueItems, overlapItems);
    }
    public bool DeleteItem(MySqlConnection db, ulong charSeq, int itemSn)
    {
        var query = $"DELETE FROM tbl_inventory_seasonitem WHERE char_seq = @char_seq AND item_sn = @item_sn;";
        int rowsAffected = db.Execute(query, new { char_seq = charSeq, item_sn = itemSn });
        if (rowsAffected <= 0)
            return false;

        return true;
    }

    public int UpdateEquippedItem(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, ulong itemSeq, short equipped, string inGameParts)
    {
        var query = string.Empty;
        int affected = 0;

        {
            query = $"UPDATE tbl_inventory_seasonitem SET equipped = @equipped WHERE char_seq = @char_seq and item_seq = @item_seq";

            affected = db.Execute(query, new TblInventorySeasonitem
            {
                char_seq = charSeq,
                item_seq = itemSeq,
                equipped = equipped,
            }, transaction);
            if (affected <= 0)
            {
                return affected;
            }
        }

        return affected;
    }
}
