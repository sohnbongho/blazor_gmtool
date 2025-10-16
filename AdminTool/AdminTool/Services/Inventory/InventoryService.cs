using AdminTool.Models;
using AdminTool.Repositories.Inventory;
using DocumentFormat.OpenXml.Spreadsheet;
using Library.DBTables.MySql;
using Library.Helper;
using Library.messages;

namespace AdminTool.Services.Inventory;

public interface IInventoryService
{
    /// <summary>
    /// 조회
    /// </summary>    
    Task<TblMember> FetchMemberAsync(string userSeqStr);
    Task<List<TblInventoryInfo>> FetchInventoryInfoByCharSeq(string charSeqStr);
    Task<List<InventoryData>> FetchAccessoryByCharSeq(string charSeqStr);
    Task<List<InventoryData>> FetchCharacterByCharSeq(string charSeqStr);
    Task<List<InventoryData>> FetchClothingByCharSeq(string charSeqStr);
    Task<List<BackgroundData>> FetchBackgroundyByCharSeq(string charSeqStr);
    Task<List<BackgroundData>> FetchBackgroundyPropByCharSeq(string charSeqStr);
    Task<List<InventoryShowRoom>> FetchShowRoomInfosByCharSeq(string charSeqStr);

    /// <summary>
    /// 삭제
    /// </summary>    
    Task<bool> DeleteCharacterByCharSeq(string charSeqStr, ulong itemSeq);
    Task<bool> DeleteAccessoryByCharSeq(string charSeqStr, ulong itemSeq);
    Task<bool> DeleteClothingByCharSeq(string charSeqStr, ulong itemSeq);

    /// <summary>
    /// 메일
    /// </summary>    
    Task<List<MailPresent>> FetchMailAsync(string userSeqStr);
}

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;

    public InventoryService(IInventoryRepository repo)
    {
        _repo = repo;
    }
    public Task<TblMember> FetchMemberAsync(string userSeqStr)
    {
        return _repo.FetchMemberAsync(userSeqStr);
    }
    public async Task<List<TblInventoryInfo>> FetchInventoryInfoByCharSeq(string charSeqStr)
    {
        var result = await _repo.FetchInventoryInfoByCharSeq(charSeqStr);
        return result;
    }

    public async Task<List<InventoryData>> FetchAccessoryByCharSeq(string charSeqStr)
    {
        var result = await _repo.FetchAccessoryByCharSeq(charSeqStr);

        return result.Select(x => new InventoryData
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            SlotId = x.slot_id,
            Amount = x.amount,
            Equipped = x.equipped,
            ColorId = x.color_id,
            JsonData = x.json_data,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    public async Task<List<InventoryData>> FetchCharacterByCharSeq(string charSeqStr)
    {
        var result = await _repo.FetchCharacterByCharSeq(charSeqStr);
        return result.Select(x => new InventoryData
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            SlotId = x.slot_id,
            Amount = x.amount,
            Equipped = x.equipped,
            ColorId = x.color_id,
            JsonData = x.json_data,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList(); ;
    }

    public async Task<List<InventoryData>> FetchClothingByCharSeq(string charSeqStr)
    {
        var result = await _repo.FetchClothingByCharSeq(charSeqStr);
        return result.Select(x => new InventoryData
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            SlotId = x.slot_id,
            Amount = x.amount,
            Equipped = x.equipped,
            ColorId = x.color_id,
            JsonData = x.json_data,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList(); ;
    }

    public async Task<List<BackgroundData>> FetchBackgroundyByCharSeq(string charSeqStr)
    {
        var result = await _repo.FetchBackgroundyByCharSeq(charSeqStr);
        return result.Select(x => new BackgroundData
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            SlotId = x.slot_id,
            Equipped = x.equipped,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList(); ;
    }

    public async Task<List<BackgroundData>> FetchBackgroundyPropByCharSeq(string charSeqStr)
    {
        var result = await _repo.FetchBackgroundyPropByCharSeq(charSeqStr);
        return result.Select(x => new BackgroundData
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            SlotId = x.slot_id,
            Equipped = x.equipped,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList(); ;
    }

    /// <summary>
    /// 삭제
    /// </summary>
    
    public Task<bool> DeleteCharacterByCharSeq(string charSeqStr, ulong itemSeq)
    {
        return _repo.DeleteCharacterByCharSeq(charSeqStr, itemSeq);
    }
    public Task<bool> DeleteAccessoryByCharSeq(string charSeqStr, ulong itemSeq)
    {
        return _repo.DeleteAccessoryByCharSeq(charSeqStr, itemSeq);
    }
    public Task<bool> DeleteClothingByCharSeq(string charSeqStr, ulong itemSeq)
    {
        return _repo.DeleteClothingByCharSeq(charSeqStr, itemSeq);
    }
    public async Task<List<MailPresent>> FetchMailAsync(string userSeqStr)
    {
        var tbls = await _repo.FetchMailAsync(userSeqStr);
        return tbls.Select(x => new MailPresent
        {
            Id = x.id,
            MailSeq = x.mail_seq,
            Guid = x.guid,
            ToUserSeq = x.to_user_seq,
            FromUserSeq = x.from_user_seq,
            Type = ConvertHelper.ToEnum<MailType>(x.type),
            Content = x.content,
            IsChecked = x.is_checked,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    public async Task<List<InventoryShowRoom>> FetchShowRoomInfosByCharSeq(string charSeqStr)
    {
        var tbls = await _repo.FetchShowRoomInfosByCharSeq(charSeqStr);
        return tbls.Select(x => new InventoryShowRoom
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            SlotId = x.slot_id,
            Equipped = x.equipped,
            
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

}
