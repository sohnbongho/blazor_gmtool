using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;

namespace AdminTool.Repositories.Inventory;

public class SeasonItemRepository : ISeasonItemRepository
{
    /// <summary>
    /// SeasonItem
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<TblInventorySeasonitem>> FetchByUserName(string name)
    {
        var items = new List<TblInventorySeasonitem>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return items;
        }

        var query = $"select * from tbl_character where nickname = @nickname limit 100;";
        var resultChracter = await db.QueryAsync<TblCharacter>(query, new TblCharacter { nickname = name });
        var tblCharacters = resultChracter?.ToList() ?? new List<TblCharacter>();

        foreach (var character in tblCharacters)
        {
            var charSeq = character.char_seq;
            query = $"select * from tbl_inventory_seasonitem where char_seq = @char_seq";
            var result = await db.QueryAsync<TblInventorySeasonitem>(query, new TblInventorySeasonitem { char_seq = charSeq });
            var tblItems = result?.ToList() ?? null;
            if (tblItems == null)
            {
                tblItems = new List<TblInventorySeasonitem>();
            }
            items.AddRange(tblItems);
        }
        return items;
    }

    public async Task<List<TblInventorySeasonitem>> FetchByUserId(string id)
    {
        var items = new List<TblInventorySeasonitem>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return items;
        }
        var query = string.Empty;

        query = $"select * from tbl_member where user_id = @user_id limit 100;";
        var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { user_id = id });
        var tblMembers = resultMember?.ToList() ?? new List<TblMember>();

        foreach (var tblMember in tblMembers)
        {
            var charSeq = tblMember.char_seq;
            query = $"select * from tbl_inventory_seasonitem where char_seq = @char_seq";
            var result = await db.QueryAsync<TblInventorySeasonitem>(query, new TblInventorySeasonitem { char_seq = charSeq });
            var tblItems = result?.ToList() ?? null;
            if (tblItems == null)
            {
                tblItems = new List<TblInventorySeasonitem>();
            }
            items.AddRange(tblItems);
        }
        return items;
    }


    public async Task<List<TblInventorySeasonitem>> FetchByCharSeq(string charSeqStr)
    {
        try
        {
            var charSeq = ulong.Parse(charSeqStr);
            var items = new List<TblInventorySeasonitem>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return items;
            }
            var query = string.Empty;

            if (charSeq != 0)
            {
                query = $"select * from tbl_inventory_seasonitem where char_seq = @char_seq";
                var result = await db.QueryAsync<TblInventorySeasonitem>(query, new TblInventorySeasonitem { char_seq = charSeq });
                var tblItems = result?.ToList() ?? null;
                if (tblItems == null)
                {
                    tblItems = new List<TblInventorySeasonitem>();
                }
                items.AddRange(tblItems);
            }
            return items;

        }
        catch (Exception)
        {
            return new List<TblInventorySeasonitem>();
        }
    }

    public async Task<List<TblInventorySeasonitem>> FetchByUserseq(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var items = new List<TblInventorySeasonitem>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return items;
            }
            var query = string.Empty;

            query = $"select * from tbl_member where user_seq = @user_seq limit 1;";
            var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
            var tblMember = resultMember?.FirstOrDefault() ?? new TblMember();

            if (tblMember.char_seq != 0)
            {
                var charSeq = tblMember.char_seq;
                query = $"select * from tbl_inventory_seasonitem where char_seq = @char_seq";
                var result = await db.QueryAsync<TblInventorySeasonitem>(query, new TblInventorySeasonitem { char_seq = charSeq });
                var tblItems = result?.ToList() ?? null;
                if (tblItems == null)
                {
                    tblItems = new List<TblInventorySeasonitem>();
                }
                items.AddRange(tblItems);
            }
            return items;

        }
        catch (Exception)
        {
            return new List<TblInventorySeasonitem>();
        }
    }

    /// <summary>
    /// SeasonItemParts
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<TblInventorySeasonitemParts>> FetchPartsByUserName(string name)
    {
        var items = new List<TblInventorySeasonitemParts>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return items;
        }

        var query = $"select * from tbl_character where nickname = @nickname limit 100;";
        var resultChracter = await db.QueryAsync<TblCharacter>(query, new TblCharacter { nickname = name });
        var tblCharacters = resultChracter?.ToList() ?? new List<TblCharacter>();

        foreach (var character in tblCharacters)
        {
            var charSeq = character.char_seq;
            query = $"select * from tbl_inventory_seasonitem_parts where char_seq = @char_seq";
            var result = await db.QueryAsync<TblInventorySeasonitemParts>(query, new TblInventorySeasonitemParts { char_seq = charSeq });
            var tblItems = result?.ToList() ?? null;
            if (tblItems == null)
            {
                tblItems = new List<TblInventorySeasonitemParts>();
            }
            items.AddRange(tblItems);
        }
        return items;
    }

    public async Task<List<TblInventorySeasonitemParts>> FetchPartsByUserId(string id)
    {
        var items = new List<TblInventorySeasonitemParts>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        var query = string.Empty;
        if (db == null)
        {
            return items;
        }

        query = $"select * from tbl_member where user_id = @user_id limit 100;";
        var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { user_id = id });
        var tblMembers = resultMember?.ToList() ?? new List<TblMember>();

        foreach (var tblMember in tblMembers)
        {
            var charSeq = tblMember.char_seq;
            query = $"select * from tbl_inventory_seasonitem_parts where char_seq = @char_seq";
            var result = await db.QueryAsync<TblInventorySeasonitemParts>(query, new TblInventorySeasonitemParts { char_seq = charSeq });
            var tblItems = result?.ToList() ?? null;
            if (tblItems == null)
            {
                tblItems = new List<TblInventorySeasonitemParts>();
            }
            items.AddRange(tblItems);
        }
        return items;
    }


    public async Task<List<TblInventorySeasonitemParts>> FetchPartsByCharSeq(string charSeqStr)
    {
        try
        {
            var charSeq = ulong.Parse(charSeqStr);
            var items = new List<TblInventorySeasonitemParts>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            var query = string.Empty;
            if (db == null)
            {
                return items;
            }

            if (charSeq != 0)
            {
                query = $"select * from tbl_inventory_seasonitem_parts where char_seq = @char_seq";
                var result = await db.QueryAsync<TblInventorySeasonitemParts>(query, new TblInventorySeasonitemParts { char_seq = charSeq });
                var tblItems = result?.ToList() ?? null;
                if (tblItems == null)
                {
                    tblItems = new List<TblInventorySeasonitemParts>();
                }
                items.AddRange(tblItems);
            }
            return items;

        }
        catch (Exception)
        {
            return new List<TblInventorySeasonitemParts>();
        }
    }

    public async Task<List<TblInventorySeasonitemParts>> FetchPartsByUserseq(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var items = new List<TblInventorySeasonitemParts>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            var query = string.Empty;
            if (db == null)
            {
                return items;
            }

            query = $"select * from tbl_member where user_seq = @user_seq limit 1;";
            var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
            var tblMember = resultMember?.FirstOrDefault() ?? new TblMember();

            if (tblMember.char_seq != 0)
            {
                var charSeq = tblMember.char_seq;
                query = $"select * from tbl_inventory_seasonitem_parts where char_seq = @char_seq";
                var result = await db.QueryAsync<TblInventorySeasonitemParts>(query, new TblInventorySeasonitemParts { char_seq = charSeq });
                var tblItems = result?.ToList() ?? null;
                if (tblItems == null)
                {
                    tblItems = new List<TblInventorySeasonitemParts>();
                }
                items.AddRange(tblItems);
            }
            return items;

        }
        catch (Exception)
        {
            return new List<TblInventorySeasonitemParts>();
        }
    }
    public async Task<List<TblInventoryModeSoftcat>> FetchSoftCatItems(string charSeqStr)
    {
        try
        {
            var items = new List<TblInventorySeasonitemParts>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            var query = string.Empty;
            if (db == null)
            {
                return new List<TblInventoryModeSoftcat>();
            }
            var charSeq = ulong.Parse(charSeqStr);

            query = $"select * from tbl_inventory_mode_softcat where char_seq = @char_seq";
            var result = await db.QueryAsync<TblInventoryModeSoftcat>(query, new TblInventoryModeSoftcat { char_seq = charSeq });
            var tblItems = result?.ToList() ?? new List<TblInventoryModeSoftcat>();

            return tblItems;
        }
        catch (Exception)
        {
            return new List<TblInventoryModeSoftcat>();
        }
    }
}
