using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;

namespace Library.Repository.Mysql;

public class OceanUpSharedRepo : MySqlDbCommonRepo
{
    public static OceanUpSharedRepo Of()
    {
        return new OceanUpSharedRepo();
    }
    private OceanUpSharedRepo()
    {

    }

    public async Task<TblOceanupUserStatus> FetchUserStatusAsync(ulong charSeq)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new TblOceanupUserStatus();
            }

            var query = $"select * from tbl_oceanup_user_status WHERE char_seq = @char_seq;";
            var result = await db.QueryAsync<TblOceanupUserStatus>(query, new TblMemberDeactive
            {
                char_seq = charSeq,
            });
            var tbl = result?.FirstOrDefault() ?? null;
            if (tbl == null)
            {
                return new TblOceanupUserStatus();
            }
            return tbl;
        }
    }
}
