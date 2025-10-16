using AdminTool.Models;
using AdminTool.Repositories.Season;

namespace AdminTool.Services.Season;

public interface ISeasonService
{
    Task<List<SeasonResetSchedule>> FetchResetSchedulesAsync();
    Task<bool> AddResetScheduleAsync(int seasonId, string title, string content, DateTime startedDate, DateTime resetDate);
    Task<bool> DeleteResetScheduleAsync(ulong id);
}

public class SeasonService : ISeasonService
{
    private readonly ISeasonRepo _repo;

    public SeasonService(ISeasonRepo repo)
    {
        _repo = repo;
    }
    public async Task<List<SeasonResetSchedule>> FetchResetSchedulesAsync()
    {
        var tbls = await _repo.FetchResetSchedulesAsync();
        return tbls.Select(x => new SeasonResetSchedule
        {
            Id = x.id,
            SeasonId = x.season_id,
            Title = x.title,
            Desc = x.desc,
            IsExecuted = x.is_executed,
            StartedDate = x.started_date,
            ResetDate = x.reset_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public Task<bool> AddResetScheduleAsync(int seasonId, string title, string content, DateTime startedDate, DateTime resetDate)
    {
        return _repo.AddResetScheduleAsync(seasonId, title, content, startedDate, resetDate);
    }
    public Task<bool> DeleteResetScheduleAsync(ulong id)
    {
        return _repo.DeleteResetScheduleAsync(id);
    }
}
