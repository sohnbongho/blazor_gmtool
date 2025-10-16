using Dapper;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using MySqlConnector;
using System.Text;

namespace Library.Repository.Mysql.Currency;

public class GoldDiamondFestaSharedRepo 
{
    public static GoldDiamondFestaSharedRepo Of() => new GoldDiamondFestaSharedRepo();
    private GoldDiamondFestaSharedRepo()
    {

    }

    public async Task<bool> AccumulateAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, CurrencyType currencyType, long itemPrice, long maxValue)
    {
        var festaType = FestaType.None;
        if (CurrencyType.Gold == currencyType)
        {
            festaType = FestaType.Gold;
        }
        else if (CurrencyType.Diamond == currencyType)
        {
            festaType = FestaType.Diamond;
        }
        else
        {
            return false;
        }
        var query = string.Empty;
        var now = DateTimeHelper.Now;
        var year = (short)now.Year;
        var month = (short)now.Month;
        var hasTable = false;

        // 골드 페스타 조회
        {
            query = $"select * from tbl_member_golddia_festa WHERE user_seq = @user_seq and year=@year and month=@month and festa_type = @festa_type ;";
            var result = await db.QueryAsync<TblMemberGolddiaFesta>(query, new TblMemberGolddiaFesta
            {
                user_seq = userSeq,
                year = year,
                month = month,
                festa_type = (short)festaType,
            }, transaction);
            var tbl = result.FirstOrDefault();
            hasTable = tbl != null;
        }
        var absPrice = Math.Abs(itemPrice);

        if (hasTable)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_member_golddia_festa SET accumulated_point = LEAST(accumulated_point + @accumulated_point, {maxValue}) ");
            strBuilder.Append($"WHERE user_seq=@user_seq and year=@year and month=@month and festa_type=@festa_type;");


            query = strBuilder.ToString();
            int rowsAffected = await db.ExecuteAsync(query, new TblMemberGolddiaFesta
            {
                accumulated_point = (ulong)absPrice,
                user_seq = userSeq,
                year = year,
                month = month,
                festa_type = (short)festaType,
            }, transaction);
            if (rowsAffected <= 0)
                return false;
        }
        else
        {
            query = $"INSERT INTO tbl_member_golddia_festa VALUES(NULL, @user_seq, @year, @month, @festa_type, @accumulated_point, @acquire_step, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

            var affected = await db.ExecuteAsync(query, new TblMemberGolddiaFesta
            {
                user_seq = userSeq,
                year = year,
                month = month,
                festa_type = (short)festaType,
                accumulated_point = (ulong)absPrice,
                acquire_step = 0,

            }, transaction);

            if (affected <= 0)
                return false;
        }

        return true;
    }


    public bool Accumulate(MySqlConnection db, MySqlTransaction transaction, ulong userSeq, CurrencyType currencyType, long itemPrice, long maxValue)
    {
        var festaType = FestaType.None;
        if (CurrencyType.Gold == currencyType)
        {
            festaType = FestaType.Gold;
        }
        else if (CurrencyType.Diamond == currencyType)
        {
            festaType = FestaType.Diamond;
        }
        else
        {
            return false;
        }
        var query = string.Empty;
        var now = DateTimeHelper.Now;
        var year = (short)now.Year;
        var month = (short)now.Month;
        var hasTable = false;

        // 골드 페스타 조회
        {
            query = $"select * from tbl_member_golddia_festa WHERE user_seq = @user_seq and year=@year and month=@month and festa_type = @festa_type ;";
            var result = db.Query<TblMemberGolddiaFesta>(query, new TblMemberGolddiaFesta
            {
                user_seq = userSeq,
                year = year,
                month = month,
                festa_type = (short)festaType,
            }, transaction);
            var tbl = result.FirstOrDefault();
            hasTable = tbl != null;
        }
        var absPrice = Math.Abs(itemPrice);

        if (hasTable)
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append($"UPDATE tbl_member_golddia_festa SET accumulated_point = LEAST(accumulated_point + @accumulated_point, {maxValue}) ");
            strBuilder.Append($"WHERE user_seq=@user_seq and year=@year and month=@month and festa_type=@festa_type;");


            query = strBuilder.ToString();
            int rowsAffected = db.Execute(query, new TblMemberGolddiaFesta
            {
                accumulated_point = (ulong)absPrice,
                user_seq = userSeq,
                year = year,
                month = month,
                festa_type = (short)festaType,
            }, transaction);
            if (rowsAffected <= 0)
                return false;
        }
        else
        {
            query = $"INSERT INTO tbl_member_golddia_festa VALUES(NULL, @user_seq, @year, @month, @festa_type, @accumulated_point, @acquire_step, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

            var affected = db.Execute(query, new TblMemberGolddiaFesta
            {
                user_seq = userSeq,
                year = year,
                month = month,
                festa_type = (short)festaType,
                accumulated_point = (ulong)absPrice,
                acquire_step = 0,

            }, transaction);

            if (affected <= 0)
                return false;
        }

        return true;
    }
}
