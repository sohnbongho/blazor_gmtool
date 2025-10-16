using AdminTool.Models;
using AdminTool.Repositories.Device;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;

namespace AdminTool.Services.Device;

public interface IUserDeviceService
{
    Task<List<BlockedDevice>> FetchBlockDevicesAsync();
    Task<bool> AddBlockDeviceAsync(string title, UserDeviceType userDeviceType, string deviceUUid);
    Task<bool> DeleteBlockDeviceAsync(ulong noticeId);
}

public class UserDeviceService : IUserDeviceService
{
    private readonly IUserDeviceRepository _repo;

    public UserDeviceService(IUserDeviceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BlockedDevice>> FetchBlockDevicesAsync()
    {
        var result = await _repo.FetchBlockDevicesAsync();
        return result.Select(x => new BlockedDevice
        {
            Id = x.id,
            Title = x.title,
            UserDeviceType = ConvertHelper.ToEnum<UserDeviceType>(x.device_type),
            DeviceUUid = x.device_uuid,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public Task<bool> AddBlockDeviceAsync(string title, UserDeviceType userDeviceType, string deviceUUid)
    {
        return _repo.AddBlockDeviceAsync(title, userDeviceType, deviceUUid);
    }
    public Task<bool> DeleteBlockDeviceAsync(ulong id)
    {
        return _repo.DeleteBlockDeviceAsync(id);
    }
}
