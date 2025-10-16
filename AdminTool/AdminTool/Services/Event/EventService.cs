using AdminTool.Models;
using AdminTool.Repositories.Event;
using Library.DTO;
using Library.Helper;

namespace AdminTool.Services.Event;

public interface IEventService
{
    Task<List<EventList>> FetchEventsAsync();
    Task<bool> AddEventAsync(string title, EventType eventType, DateTime startDate, DateTime endDate);
    Task<bool> DeleteEventAsync(ulong id);
}
public class EventService : IEventService
{
    private readonly IEventRepository _repo;

    public EventService(IEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<EventList>> FetchEventsAsync()
    {
        var tbls = await _repo.FetchEventsAsync();
        return tbls.Select(x => new EventList
        {
            Id= x.id,
            EventType = ConvertHelper.ToEnum<EventType>(x.event_type),
            Title = x.title,
            StartDate = x.start_date,
            EndDate = x.end_date,
            Enable = x.enable > 0 ? true : false,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public Task<bool> AddEventAsync(string title, EventType eventType, DateTime startDate, DateTime endDate)
    {
        return _repo.AddEventAsync(title, eventType, startDate, endDate);
    }
    public Task<bool> DeleteEventAsync(ulong id)
    {
        return _repo.DeleteEventAsync(id);
    }
}
