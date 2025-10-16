using AdminTool.Models;
using AdminTool.Repositories.Notice;
using Akka.Actor;
using DocumentFormat.OpenXml.Office2010.Excel;
using Library.AkkaActors;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.messages;
using Messages;

namespace AdminTool.Services.Notice;

public interface INoticeManageService
{
    /// <summary>
    /// 공지(지속)
    /// </summary>
    /// <returns></returns>
    Task<List<TblNoticePersistent>> FetchPersistentNoticesAsync();
    Task<bool> AddPersistentNoticeAsync(string title, string content, DateTime noticeTime);
    Task<bool> DeletePersistentNoticeAsync(ulong noticeId);

    /// <summary>
    /// 즉시 공지
    /// </summary>
    /// <returns></returns>
    Task<List<NoticeImmediate>> FetchImmediateNoticesAsync();
    Task<bool> AddImmediateNoticeAsync(string title, string content, DateTime noticeTime);
    Task<bool> DeleteImmediateNoticeAsync(ulong noticeId);

    /// <summary>
    /// 알림
    /// </summary>
    /// <param name="gmName"></param>
    /// <param name="gmMessage"></param>
    /// <returns></returns>
    Task<bool> NotiFeatureBlockChangedAsync();
    Task<List<FeatureControl>> FetchFeatureInfosAsync();
    Task<bool> AddFeatureBlockAsync(FeatureControlType controlType);
    Task<bool> DeleteFeatureBlockAsync(ulong id);


    /// <summary>
    /// 긴급공지
    /// </summary>
    /// <returns></returns>
    Task<List<NoticeEmergency>> FetchEmergencyNoticesAsync();
    Task<bool> AddEmergencyNoticeAsync(DateTime noticeTime);
    Task<bool> DeleteEmergencyNoticeAsync(ulong noticeId);
    Task<bool> NotiEmergencyNoticeChangedAsync();
}


public class NoticeManageService : INoticeManageService
{
    private readonly INoticeManageRepository _repo;

    public NoticeManageService(INoticeManageRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// 지속적인 공지
    /// </summary>
    /// <returns></returns>
    public Task<List<TblNoticePersistent>> FetchPersistentNoticesAsync()
    {
        return _repo.FetchPersistentNoticesAsync();
    }

    public Task<bool> AddPersistentNoticeAsync(string title, string content, DateTime noticeTime)
    {
        return _repo.AddPersistentNoticeAsync(title, content, noticeTime);
    }
    public Task<bool> DeletePersistentNoticeAsync(ulong noticeId)
    {
        return _repo.DeletePersistentNoticeAsync(noticeId);
    }

    /// <summary>
    /// 즉시 공지
    /// </summary>
    /// <returns></returns>
    public Task<List<NoticeImmediate>> FetchImmediateNoticesAsync()
    {
        return _repo.FetchImmediateNoticesAsync();
    }
    public Task<bool> AddImmediateNoticeAsync(string title, string content, DateTime noticeTime)
    {
        return _repo.AddImmediateNoticeAsync(title, content, noticeTime);
    }
    public Task<bool> DeleteImmediateNoticeAsync(ulong noticeId)
    {
        return _repo.DeleteImmediateNoticeAsync(noticeId);
    }

    /// <summary>
    /// 콘텐츠 온오프
    /// </summary>    
    public async Task<bool> NotiFeatureBlockChangedAsync()
    {
        var nats = ActorRefsHelper.Instance.Get(ActorPaths.MessageQueuePath);
        if (nats == ActorRefs.Nobody)
            return true;

        var communityServers = await _repo.FetchCommunityServerInfosAsync();
        if (communityServers.Any() == false)
            return true;

        foreach (var communityServer in communityServers)
        {
            var toServerId = communityServer.server_id;
            nats.Tell(new S2SMessage.NatsPubliish
            {
                NatsMessageWrapper = new NatsMessages.NatsMessageWrapper
                {
                    FromServerId = -1,
                    ToServerId = toServerId,
                    TargetCharSeq = string.Empty,

                    FeatureBlockChangedNoti = new FeatureBlockChangedNoti
                    {
                    }
                }
            });
        }
        return true;
    }
    public async Task<List<FeatureControl>> FetchFeatureInfosAsync()
    {
        var tbls = await _repo.FetchFeatureInfosAsync();
        return tbls.Select(x => new FeatureControl
        {
            Id = x.id,
            FeatureControlType = ConvertHelper.ToEnum<FeatureControlType>(x.control_type),
            Blocked = x.blocked > 0,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public Task<bool> AddFeatureBlockAsync(FeatureControlType controlType)
    {
        return _repo.AddFeatureBlockAsync(controlType);
    }
    public Task<bool> DeleteFeatureBlockAsync(ulong id)
    {
        return _repo.DeleteFeatureBlockAsync(id);
    }
    public async Task<List<NoticeEmergency>> FetchEmergencyNoticesAsync()
    {
        var result = await _repo.FetchEmergencyNoticesAsync();

        return result.Select(x => new NoticeEmergency
        {
            Id = x.id,
            CheckedDate = x.checked_date,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,

        }).ToList();
    }
    public Task<bool> AddEmergencyNoticeAsync(DateTime noticeTime)
    {
        return _repo.AddEmergencyNoticeAsync(noticeTime);
    }
    public Task<bool> DeleteEmergencyNoticeAsync(ulong id)
    {
        return _repo.DeleteEmergencyNoticeAsync(id);
    }
    public async Task<bool> NotiEmergencyNoticeChangedAsync()
    {
        var nats = ActorRefsHelper.Instance.Get(ActorPaths.MessageQueuePath);
        if (nats == ActorRefs.Nobody)
            return true;

        var communityServers = await _repo.FetchCommunityServerInfosAsync();
        if (communityServers.Any() == false)
            return true;

        foreach (var communityServer in communityServers)
        {
            var toServerId = communityServer.server_id;
            nats.Tell(new S2SMessage.NatsPubliish
            {
                NatsMessageWrapper = new NatsMessages.NatsMessageWrapper
                {
                    FromServerId = -1,
                    ToServerId = toServerId,
                    TargetCharSeq = string.Empty,

                    EmergencyNoticeChangedNoti = new EmergencyNoticeChangedNoti
                    {
                    }
                }
            });
        }
        return true;
    }
}
