using Library.DBTables;
using Library.DBTables.MySql;

namespace AdminTool.Repositories.Inventory;

public interface ISeasonItemRepository
{
    /// <summary>
    /// 시즌 아이템
    /// </summary>    
    Task<List<TblInventorySeasonitem>> FetchByUserName(string name);
    Task<List<TblInventorySeasonitem>> FetchByUserId(string id);
    Task<List<TblInventorySeasonitem>> FetchByUserseq(string userSeqStr);
    Task<List<TblInventorySeasonitem>> FetchByCharSeq(string charSeqStr);


    /// <summary>
    /// 시즌 파츠
    /// </summary>    
    Task<List<TblInventorySeasonitemParts>> FetchPartsByUserName(string name);
    Task<List<TblInventorySeasonitemParts>> FetchPartsByUserId(string id);
    Task<List<TblInventorySeasonitemParts>> FetchPartsByUserseq(string userSeqStr);
    Task<List<TblInventorySeasonitemParts>> FetchPartsByCharSeq(string charSeqStr);

    Task<List<TblInventoryModeSoftcat>> FetchSoftCatItems(string charSeqStr);
}
