using Library.Data.Models;
using MySqlConnector;

namespace Library.Repository.Mysql.Inventory;

public interface IGameModeInventory
{
    (List<SeasonUniqueItem>, List<SeasonOverlapItem>) FetchItems(MySqlConnection db, ulong charSeq);

    bool DeleteItem(MySqlConnection db, ulong charSeq, int itemSn);

}

public interface IEquippedInventory
{
    int UpdateEquippedItem(MySqlConnection db, MySqlTransaction transaction, ulong charSeq, ulong itemSeq, short equipped, string inGameParts);
}