using Dapper;
using Library.Connector;
using Library.DBTables;
using Library.DBTables.MySql;
using Library.Helper;

namespace AdminTool.Repositories.Account;

public class AccountRepository : IAccountRepository
{
    public async Task<bool> RegisterAsync(string username, string password, string user_desc)
    {
        var hashedString = HashHelper.ComputeSha256Hash(password);

        await using var db = await MySqlConnectionHelper.Instance.GmtoolConnectionFactoryAsync();
        if(db == null)
        {
            return false;
        }

        var query = $"INSERT INTO dancepartygmtool.tbl_member VALUES(NULL, @user_id, @user_passwd, @user_desc, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
        var affected = await db.ExecuteAsync(query, new GMToolDbQuery.TblMember
        {
            user_id = username,
            user_passwd = hashedString,
            user_desc = user_desc,
        });

        return affected > 0;
    }

    public async Task<bool> ValidateLoginAsync(string username, string password)
    {
        var hashedString = HashHelper.ComputeSha256Hash(password);

        await using var db = await MySqlConnectionHelper.Instance.GmtoolConnectionFactoryAsync();
        if (db == null)
        {
            return false;
        }

        var query = $"SELECT * FROM dancepartygmtool.tbl_member where user_id = @user_id and user_passwd = @user_passwd"; 
        var reuslt = await db.QueryAsync<GMToolDbQuery.TblMember>(query, new GMToolDbQuery.TblMember
        {
            user_id = username,
            user_passwd = hashedString,            
        });
        var data = reuslt?.FirstOrDefault() ?? null;
        return data != null;
    }
    public async Task<bool> HasUserAsync(string username)
    {   
        await using var db = await MySqlConnectionHelper.Instance.GmtoolConnectionFactoryAsync();
        if (db == null)
        {
            return false;
        }
        var query = $"SELECT * FROM dancepartygmtool.tbl_member where user_id = @user_id ";
        var reuslt = await db.QueryAsync<GMToolDbQuery.TblMember>(query, new GMToolDbQuery.TblMember
        {
            user_id = username,            
        });
        var data = reuslt?.FirstOrDefault() ?? null;
        return data != null;
    }

}