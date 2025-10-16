using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;

namespace AdminTool.Repositories.Season;

public interface ISeasonRepo
{
    Task<List<TblSeasonResetSchedule>> FetchResetSchedulesAsync();
    Task<bool> AddResetScheduleAsync(int seasonId, string title, string content, DateTime startedDate, DateTime resetDate);
    Task<bool> DeleteResetScheduleAsync(ulong id);
}
public class SeasonRepo : ISeasonRepo
{
    public async Task<List<TblSeasonResetSchedule>> FetchResetSchedulesAsync()
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblSeasonResetSchedule>();
            }

            var query = $"select * from tbl_season_reset_schedule order by id desc limit 1000;";
            var result = await db.QueryAsync<TblSeasonResetSchedule>(query);

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblSeasonResetSchedule>();
        }
    }
    public async Task<bool> AddResetScheduleAsync(int seasonId, string title, string content, DateTime startedDate, DateTime resetDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return false;
            }

            var query = $"INSERT INTO tbl_season_reset_schedule VALUES(NULL, @season_id, @title, @content, 0, @started_date, @reset_date, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            var affected = await db.ExecuteAsync(query, new
            {
                season_id = seasonId,
                title = title,
                content = content,
                started_date = startedDate,
                reset_date = resetDate,
            });

            return affected > 0;

        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> DeleteResetScheduleAsync(ulong id)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return false;
            }

            var query = $"DELETE FROM tbl_season_reset_schedule WHERE id = @id;";
            var affected = await db.ExecuteAsync(query, new
            {
                id = id,
            });

            return affected > 0;

        }
        catch (Exception)
        {
            return false;
        }
    }
}
