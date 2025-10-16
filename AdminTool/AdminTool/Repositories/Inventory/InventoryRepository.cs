using AdminTool.Models;
using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;

namespace AdminTool.Repositories.Inventory;

public interface IInventoryRepository
{
    Task<TblMember> FetchMemberAsync(string userSeq);

    Task<List<TblInventoryInfo>> FetchInventoryInfoByCharSeq(string charSeqStr);
    Task<List<TblInventoryAccessory>> FetchAccessoryByCharSeq(string charSeqStr);
    Task<List<TblInventoryCharacter>> FetchCharacterByCharSeq(string charSeqStr);
    Task<List<TblInventoryClothing>> FetchClothingByCharSeq(string charSeqStr);
    Task<List<TblInventoryBackground>> FetchBackgroundyByCharSeq(string charSeqStr);
    Task<List<TblInventoryBackgroundProp>> FetchBackgroundyPropByCharSeq(string charSeqStr);

    /// <summary>
    /// 삭제
    /// </summary>    
    Task<bool> DeleteCharacterByCharSeq(string charSeqStr, ulong itemSeq);
    Task<bool> DeleteAccessoryByCharSeq(string charSeqStr, ulong itemSeq);
    Task<bool> DeleteClothingByCharSeq(string charSeqStr, ulong itemSeq);

    Task<List<TblMailPresent>> FetchMailAsync(string userSeqStr);
    Task<List<TblInventoryShowroom>> FetchShowRoomInfosByCharSeq(string charSeqStr);
}

public class InventoryRepository : IInventoryRepository
{
    public async Task<TblMember> FetchMemberAsync(string userSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }

        try
        {
            var userSeq = ulong.Parse(userSeqStr);

            var query = $"select * from tbl_member where user_seq = @user_seq limit 1;";
            var resultChracter = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
            var tbl = resultChracter?.FirstOrDefault() ?? null;

            return tbl != null ? tbl : new();

        }
        catch (Exception)
        {
            return new();

        }
    }
    public async Task<List<TblInventoryInfo>> FetchInventoryInfoByCharSeq(string charSeqStr)
    {
        var items = new List<TblInventoryInfo>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return items;
        }

        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_info where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryInfo>(query, new TblInventoryInfo { char_seq = charSeq });
            var tbls = resultChracter?.ToList() ?? new List<TblInventoryInfo>();
            items.AddRange(tbls);

            return items;

        }
        catch (Exception)
        {
            return new List<TblInventoryInfo>();

        }
    }

    public async Task<List<TblInventoryAccessory>> FetchAccessoryByCharSeq(string charSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        var items = new List<TblInventoryAccessory>();
        if (db == null)
        {
            return items;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_accessory where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryAccessory>(query, new TblInventoryAccessory { char_seq = charSeq });
            var tbls = resultChracter?.ToList() ?? new List<TblInventoryAccessory>();
            items.AddRange(tbls);

            return items;

        }
        catch (Exception)
        {
            return new List<TblInventoryAccessory>();

        }
    }

    public async Task<List<TblInventoryCharacter>> FetchCharacterByCharSeq(string charSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        var items = new List<TblInventoryCharacter>();
        if (db == null)
        {
            return items;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_character where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryCharacter>(query, new TblInventoryCharacter { char_seq = charSeq });
            var tbls = resultChracter?.ToList() ?? new List<TblInventoryCharacter>();
            items.AddRange(tbls);

            return items;

        }
        catch (Exception)
        {
            return new List<TblInventoryCharacter>();

        }
    }
    public async Task<List<TblInventoryClothing>> FetchClothingByCharSeq(string charSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        var items = new List<TblInventoryClothing>();
        if (db == null)
        {
            return items;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_clothing where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryClothing>(query, new TblInventoryClothing { char_seq = charSeq });
            var tbls = resultChracter?.ToList() ?? new List<TblInventoryClothing>();
            items.AddRange(tbls);

            return items;

        }
        catch (Exception)
        {
            return new List<TblInventoryClothing>();

        }
    }

    public async Task<List<TblInventoryBackground>> FetchBackgroundyByCharSeq(string charSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        var items = new List<TblInventoryBackground>();
        if (db == null)
        {
            return items;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_background where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryBackground>(query, new { char_seq = charSeq });
            var tbls = resultChracter?.ToList() ?? new List<TblInventoryBackground>();
            items.AddRange(tbls);

            return items;

        }
        catch (Exception)
        {
            return new List<TblInventoryBackground>();

        }
    }
    public async Task<List<TblInventoryBackgroundProp>> FetchBackgroundyPropByCharSeq(string charSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        var items = new List<TblInventoryBackgroundProp>();
        if (db == null)
        {
            return items;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_backgroundprop where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryBackgroundProp>(query, new { char_seq = charSeq });
            var tbls = resultChracter?.ToList() ?? new List<TblInventoryBackgroundProp>();
            items.AddRange(tbls);

            return items;

        }
        catch (Exception)
        {
            return new List<TblInventoryBackgroundProp>();

        }
    }

    /// <summary>
    /// 삭제
    /// </summary>    
    public async Task<bool> DeleteCharacterByCharSeq(string charSeqStr, ulong itemSeq)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return false;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"DELETE FROM tbl_inventory_character WHERE char_seq = @char_seq and item_seq = @item_seq;";
            var affected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
            });
            return affected > 0;
        }
        catch (Exception)
        {
            return false;

        }
    }
    public async Task<bool> DeleteAccessoryByCharSeq(string charSeqStr, ulong itemSeq)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return false;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"DELETE FROM tbl_inventory_accessory WHERE char_seq = @char_seq and item_seq = @item_seq;";
            var affected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
            });
            return affected > 0;
        }
        catch (Exception)
        {
            return false;

        }
    }
    public async Task<bool> DeleteClothingByCharSeq(string charSeqStr, ulong itemSeq)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return false;
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"DELETE FROM tbl_inventory_clothing WHERE char_seq = @char_seq and item_seq = @item_seq;";
            var affected = await db.ExecuteAsync(query, new
            {
                char_seq = charSeq,
                item_seq = itemSeq,
            });
            return affected > 0;
        }
        catch (Exception)
        {
            return false;

        }
    }

    public async Task<List<TblMailPresent>> FetchMailAsync(string userSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new List<TblMailPresent>();
        }
        try
        {
            var userSeq = ulong.Parse(userSeqStr);

            var query = $"select * from tbl_mail_present where to_user_seq = @to_user_seq;";
            var resultChracter = await db.QueryAsync<TblMailPresent>(query, new TblMailPresent
            {
                to_user_seq = userSeq
            });
            return resultChracter?.ToList() ?? new List<TblMailPresent>();
        }
        catch (Exception)
        {
            return new List<TblMailPresent>();
        }
    }
    public async Task<List<TblInventoryShowroom>> FetchShowRoomInfosByCharSeq(string charSeqStr)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new List<TblInventoryShowroom>();
        }
        try
        {
            var charSeq = ulong.Parse(charSeqStr);

            var query = $"select * from tbl_inventory_showroom where char_seq = @char_seq;";
            var resultChracter = await db.QueryAsync<TblInventoryShowroom>(query, new TblInventoryShowroom
            {
                char_seq = charSeq
            });
            return resultChracter.ToList();
        }
        catch (Exception)
        {
            return new List<TblInventoryShowroom>();
        }
    }
}
