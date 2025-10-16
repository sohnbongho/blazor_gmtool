using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;

namespace Library.Repository.Mysql;

public class UserLogSharedRepo : MySqlDbCommonRepo
{
    public static UserLogSharedRepo Of() => new UserLogSharedRepo();
    private UserLogSharedRepo()
    {

    }

    public async Task<bool> AddBanAsync(ulong userSeq, UserBanType userBanType, string title, DateTime banExpiryDate)
    {
        var banned = false;
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }
            {
                var query = $"INSERT INTO tbl_log_ban_history VALUES(NULL, @user_seq, @ban_type, @title, @ban_expiry_date, CURRENT_TIMESTAMP)";

                int affected = await db.ExecuteAsync(query, new TblLogBanHistory
                {
                    user_seq = userSeq,
                    ban_type = (short)userBanType,
                    title = title,
                    ban_expiry_date = banExpiryDate,
                });
                banned = affected > 0;
            }
        }

        return banned;
    }
    public async Task<List<TblLogBanHistory>> FetchUserBanHistoryAsync(ulong userSeq)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }
            {
                var query = $"select * from tbl_log_ban_history where user_seq = @user_seq order by id desc;";
                var result = await db.QueryAsync<TblLogBanHistory>(query, new TblLogBanHistory { user_seq = userSeq });
                return result.ToList();
            }
        }
    }
}
