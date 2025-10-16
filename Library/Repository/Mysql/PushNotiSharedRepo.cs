using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using log4net;
using System.Reflection;
using System.Text;

namespace Library.Repository.Mysql;

public class PushNotiSharedRepo : MySqlDbCommonRepo
{
    public static PushNotiSharedRepo Of() => new PushNotiSharedRepo();
    private PushNotiSharedRepo()
    {

    }
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public async Task<bool> UpdateSetupAsync(ulong userSeq, bool maketAgreed, bool quiteMode,
                DateTime startQuiteDate, DateTime endQuiteDate, bool newFollow, bool present)
    {
        if (0 == userSeq)
        {
            return true;
        }
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }
            await using var transaction = await db.BeginTransactionAsync();

            try
            {
                var strBuilder = new StringBuilder();

                strBuilder.Append("UPDATE tbl_member_push_setting ");
                strBuilder.Append("SET maket_agreed= @maket_agreed, quite_mode=@quite_mode, ");
                strBuilder.Append("start_quite_date=@start_quite_date, end_quite_date=@end_quite_date, ");
                strBuilder.Append("new_follow=@new_follow, present=@present ");
                strBuilder.Append("WHERE user_seq = @user_seq;");

                var query = strBuilder.ToString();

                var result = await db.ExecuteAsync(query, new TblMemberPushSetting
                {
                    user_seq = userSeq,

                    maket_agreed = maketAgreed ? (short)1 : (short)0,
                    quite_mode = quiteMode ? (short)1 : (short)0,

                    start_quite_date = startQuiteDate,
                    end_quite_date = endQuiteDate,

                    new_follow = newFollow ? (short)1 : (short)0,
                    present = present ? (short)1 : (short)0,
                }, transaction);

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("UpdateSetup", ex);
                await transaction.RollbackAsync();
                return false;
            }
        }
    }

    public async Task<TblMemberPushSetting> FetchSetupAsync(ulong userSeq)
    {
        if (0 == userSeq)
        {
            return new();
        }
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new TblMemberPushSetting();
            }

            try
            {
                var query = $"select * from tbl_member_push_setting where user_seq=@user_seq limit 1;";
                var result = await db.QueryAsync<TblMemberPushSetting>(query, new
                {
                    user_seq = userSeq,
                });
                var tbl = result.FirstOrDefault();
                return tbl != null ? tbl : new TblMemberPushSetting { user_seq = 0 };
            }
            catch (Exception ex)
            {
                _logger.Error("FetchSetup", ex);
                return new TblMemberPushSetting();
            }
        }
    }

}
