using Library.Data.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repository.Mysql.Inventory;

public class NoneGameModeInventory : IGameModeInventory
{
    public (List<SeasonUniqueItem>, List<SeasonOverlapItem>) FetchItems(MySqlConnection db, ulong charSeq)
    {
        return (new List<SeasonUniqueItem>(), new List<SeasonOverlapItem>());
    }

    public bool DeleteItem(MySqlConnection db, ulong charSeq, int itemSn)
    {
        return false;
    }
}
