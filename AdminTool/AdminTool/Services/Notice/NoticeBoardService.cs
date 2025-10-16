using AdminTool.Models;
using AdminTool.Repositories.Notice;

namespace AdminTool.Services.Notice;

public interface INoticeBoardService
{
    Task<List<NoticeBoardData>> FetchNoticesAsync();
    Task<bool> AddNoticeAsync(NoticeBoardData boardData);
    Task<bool> DeleteNoticeAsync(ulong noticeSeq);
}

public class NoticeBoardService : INoticeBoardService
{
    private readonly INoticeBoardRepository _repo;

    public NoticeBoardService(INoticeBoardRepository repo)
    {
        _repo = repo;
    }
    public Task<List<NoticeBoardData>> FetchNoticesAsync()
    {
        return _repo.FetchNoticesAsync();                
    }
    public Task<bool> AddNoticeAsync(NoticeBoardData boardData)
    {
        return _repo.AddNoticeAsync(boardData);
    }
    public Task<bool> DeleteNoticeAsync(ulong noticeSeq)
    {
        return _repo.DeleteNoticeAsync(noticeSeq);
    }
}
