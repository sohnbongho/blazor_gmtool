using AdminTool.Models;
using AdminTool.Repositories.ConcurrentUser;

namespace AdminTool.Services.ConcurrentUser;

public interface IConcurrentUserService
{
    Task<List<LogConcurrentUser>> FetchConcurrentUserLogsAsync();
    Task<List<ConcurrentUserItem>> FetchServerUserCountsAsync();
}
public class ConcurrentUserService : IConcurrentUserService
{
    private readonly IConcurrentUserRepository _repo;
    public ConcurrentUserService(IConcurrentUserRepository repo)
    {
        _repo = repo;
    }
    public async Task<List<LogConcurrentUser>> FetchConcurrentUserLogsAsync()
    {
        var users = await _repo.FetchConcurrentUserLogsAsync();
        var serverList = await _repo.FetchServerInfosAsync();
        return users.Select(x => new LogConcurrentUser
        {
            Id = x.id,
            ServerName = serverList.FirstOrDefault(s => s.server_id == x.server_id)?.server_name ?? x.server_id.ToString(),
            UserCount = x.count,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<ConcurrentUserItem>> FetchServerUserCountsAsync()
    {
        var users = new List<ConcurrentUserItem>();
        var serverList = await _repo.FetchServerInfosAsync();
        foreach (var server in serverList)
        {
            var serverId = server.server_id;
            var userCount = await _repo.FetchServerUserCountAsync(serverId);
            var serverName = server.server_name;

            users.Add(new ConcurrentUserItem
            {
                ServerId = serverId,
                ServerName = serverName,
                UserCount = userCount,
            });
        }
        return users;
    }


}
