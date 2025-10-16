using AdminTool.Models;
using AdminTool.Repositories.Inventory;
using Library.DTO;
using Library.Helper;

namespace AdminTool.Services.Inventory;
public class SeasonItemService : ISeasonItemService
{
    private readonly ISeasonItemRepository _seasonItemRepository;
    public SeasonItemService(ISeasonItemRepository seasonItemRepository)
    {
        _seasonItemRepository = seasonItemRepository;
    }

    /// <summary>
    /// 시즌 인텐보리 정보
    /// </summary>    
    public async Task<List<InventorySeasonitem>> FetchSeasonItemsByUserName(string name)
    {
        var result = await _seasonItemRepository.FetchByUserName(name);
        return result.Select(x => new InventorySeasonitem
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            Favorites = x.favorites,
            Equipped = x.equipped,
            StoredAmount = x.stored_amount,
            ExtraOption = x.extra_option,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<InventorySeasonitem>> FetchSeasonItemsByUserId(string id)
    {
        var result = await _seasonItemRepository.FetchByUserId(id);
        return result.Select(x => new InventorySeasonitem
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            Favorites = x.favorites,
            Equipped = x.equipped,
            StoredAmount = x.stored_amount,
            ExtraOption = x.extra_option,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    public async Task<List<InventorySeasonitem>> FetchSeasonItemsByUserSeq(string userSeq)
    {
        var result = await _seasonItemRepository.FetchByUserseq(userSeq);
        return result.Select(x => new InventorySeasonitem
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            Favorites = x.favorites,
            Equipped = x.equipped,
            StoredAmount = x.stored_amount,
            ExtraOption = x.extra_option,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<InventorySeasonitem>> FetchSeasonItemsByCharSeq(string charSeq)
    {
        var result = await _seasonItemRepository.FetchByCharSeq(charSeq);
        return result.Select(x => new InventorySeasonitem
        {
            Id = x.id,
            CharSeq = x.char_seq,
            ItemSeq = x.item_seq,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            Favorites = x.favorites,
            Equipped = x.equipped,
            StoredAmount = x.stored_amount,
            ExtraOption = x.extra_option,
            ExpirationDate = x.expiration_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    /// <summary>
    /// 시즌 파츠
    /// </summary>    
    public async Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByUserName(string name)
    {
        var result = await _seasonItemRepository.FetchPartsByUserName(name);
        return result.Select(x => new InventorySeasonitemParts
        {
            Id = x.id,
            CharSeq = x.char_seq,
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            IngameParts = x.ingame_parts,

            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByUserId(string id)
    {
        var result = await _seasonItemRepository.FetchPartsByUserId(id);
        return result.Select(x => new InventorySeasonitemParts
        {
            Id = x.id,
            CharSeq = x.char_seq,
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            IngameParts = x.ingame_parts,

            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    public async Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByUserSeq(string userSeq)
    {
        var result = await _seasonItemRepository.FetchPartsByUserseq(userSeq);
        return result.Select(x => new InventorySeasonitemParts
        {
            Id = x.id,
            CharSeq = x.char_seq,
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            IngameParts = x.ingame_parts,

            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<InventorySeasonitemParts>> FetchSeasonItemPartsByCharSeq(string charSeq)
    {
        var result = await _seasonItemRepository.FetchPartsByCharSeq(charSeq);
        return result.Select(x => new InventorySeasonitemParts
        {
            Id = x.id,
            CharSeq = x.char_seq,
            GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode_type),
            IngameParts = x.ingame_parts,

            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    public async Task<List<InventoryModeSoftcat>> FetchSoftCatItems(string charSeqStr)
    {
        var result = await _seasonItemRepository.FetchSoftCatItems(charSeqStr);
        return result.Select(x => new InventoryModeSoftcat
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
}
