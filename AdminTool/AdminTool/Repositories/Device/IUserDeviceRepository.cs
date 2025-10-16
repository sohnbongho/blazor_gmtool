using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;

namespace AdminTool.Repositories.Device;

public interface IUserDeviceRepository
{
    Task<List<TblBlockedDevice>> FetchBlockDevicesAsync();
    Task<bool> AddBlockDeviceAsync(string title, UserDeviceType userDeviceType, string deviceUUid);
    Task<bool> DeleteBlockDeviceAsync(ulong noticeId);
}

public class UserDeviceRepository : IUserDeviceRepository
{
    public async Task<List<TblBlockedDevice>> FetchBlockDevicesAsync()
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }

        try
        {
            var query = $"select * from tbl_blocked_device order by id desc limit 1024;";
            var result = await db.QueryAsync<TblBlockedDevice>(query);
            return result.ToList();
        }
        catch (Exception)
        {
            return new();

        }

    }
    public async Task<bool> AddBlockDeviceAsync(string title, UserDeviceType userDeviceType, string deviceUUid)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return false;
        }

        try
        {
            var now = DateTimeHelper.Now;

            var query = $"INSERT INTO tbl_blocked_device VALUES(NULL, @title, @device_type, @device_uuid, CURRENT_TIMESTAMP);";
            var affected = await db.ExecuteAsync(query, new TblBlockedDevice
            {
                title = title,
                device_type = (short)userDeviceType,
                device_uuid = deviceUUid
            });

            return affected > 0;

        }
        catch (Exception)
        {
            return false;

        }
    }
    public async Task<bool> DeleteBlockDeviceAsync(ulong id)
    {
        await using (var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            try
            {
                var now = DateTimeHelper.Now;

                var query = $"DELETE FROM tbl_blocked_device WHERE id = @id;";
                var affected = await db.ExecuteAsync(query, new TblBlockedDevice
                {
                    id = (long)id,
                });
                return (affected > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
