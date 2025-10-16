using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;

namespace AdminTool.Repositories.Event;

public interface IEventRepository
{
    Task<List<TblEventList>> FetchEventsAsync();
    Task<bool> AddEventAsync(string title, EventType eventType, DateTime startDate, DateTime endDate);
    Task<bool> DeleteEventAsync(ulong id);
}
public class EventRepository : IEventRepository
{
    public async Task<List<TblEventList>> FetchEventsAsync()
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblEventList>();
            }

            var query = $"select * from tbl_event_list order by id desc limit 1000;";
            var result = await db.QueryAsync<TblEventList>(query);

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblEventList>();
        }
    }
    public async Task<bool> AddEventAsync(string title, EventType eventType, DateTime startDate, DateTime endDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return false;
            }

            var query = $"INSERT INTO tbl_event_list VALUES(NULL, @event_type, @title, @start_date, @end_date, @enable, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            var affected = await db.ExecuteAsync(query, new TblEventList
            {                
                event_type = (short)eventType,
                title = title,
                start_date = startDate,
                end_date = endDate,
                enable = 1,
            });

            return affected > 0;

        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<bool> DeleteEventAsync(ulong id)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return false;
            }

            var query = $"DELETE FROM tbl_event_list WHERE id = @id;";
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
