using AdminTool.Models;
using Library.DBTables;
using Library.DBTables.MySql;

namespace AdminTool.Services.Inventory;

public interface ISeasonItemService
{
    /// <summary>
    /// 시즌 인텐보리 정보
    /// </summary>    
    Task<List<InventorySeasonitem>> FetchSeasonItemsByUserName(string name);
    Task<List<InventorySeasonitem>> FetchSeasonItemsByUserId(string id);
    Task<List<InventorySeasonitem>> FetchSeasonItemsByUserSeq(string userSeq);
    Task<List<InventorySeasonitem>> FetchSeasonItemsByCharSeq(string charSeq);

    /// <summary>
    /// 시즌 인텐보리 정보
    /// </summary>    
    Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByUserName(string name);
    Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByUserId(string id);
    Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByUserSeq(string userSeq);
    Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByCharSeq(string charSeq);


    Task<List<InventoryModeSoftcat>> FetchSoftCatItems(string charSeqStr);
}
