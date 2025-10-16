using AdminTool.Models;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.Repository.Mysql;
using MongoDB.Driver.Core.Servers;

namespace AdminTool.Repositories.Notice;

public interface INoticeManageRepository
{
    Task<List<TblNoticePersistent>> FetchPersistentNoticesAsync();
    Task<bool> AddPersistentNoticeAsync(string title, string content, DateTime noticeTime);
    Task<bool> DeletePersistentNoticeAsync(ulong noticeId);

    Task<List<NoticeImmediate>> FetchImmediateNoticesAsync();
    Task<bool> AddImmediateNoticeAsync(string title, string content, DateTime noticeTime);
    Task<bool> DeleteImmediateNoticeAsync(ulong noticeId);
    Task<List<TblServerList>> FetchCommunityServerInfosAsync();

    /// <summary>
    /// 콘텐츠 조절
    /// </summary>
    /// <returns></returns>
    Task<List<TblFeatureControl>> FetchFeatureInfosAsync();
    Task<bool> AddFeatureBlockAsync(FeatureControlType controlType);
    Task<bool> DeleteFeatureBlockAsync(ulong id);

    Task<List<TblNoticeEmergency>> FetchEmergencyNoticesAsync();
    Task<bool> AddEmergencyNoticeAsync(DateTime noticeTime);
    Task<bool> DeleteEmergencyNoticeAsync(ulong noticeId);
}

public class NoticeManageRepository : INoticeManageRepository
{
    private ServerInfoSharedRepo _serverRepo = new ServerInfoSharedRepo();
    private ServerInfoSharedRepo _serverInfoRepo = new();

    public async Task<List<TblNoticePersistent>> FetchPersistentNoticesAsync()
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }

        try
        {

            var query = $"select * from tbl_notice_persistent order by id desc;";
            var result = await db.QueryAsync<TblNoticePersistent>(query);
            return result.ToList();

        }
        catch (Exception)
        {
            return new();

        }
    }
    public async Task<bool> AddPersistentNoticeAsync(string title, string content, DateTime noticeTime)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return false;
        }

        try
        {
            var now = DateTimeHelper.Now;

            var query = $"INSERT INTO tbl_notice_persistent VALUES(NULL, @title, @content, @expiry_date, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            var affected = await db.ExecuteAsync(query, new TblNoticePersistent
            {
                title = title,
                content = content,

                expiry_date = noticeTime,
                updated_date = now,
                created_date = now,
            });

            return affected > 0;

        }
        catch (Exception)
        {
            return false;

        }
    }

    public async Task<bool> DeletePersistentNoticeAsync(ulong noticeId)
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

                var query = $"DELETE FROM tbl_notice_persistent WHERE id = @id;";
                var affected = await db.ExecuteAsync(query, new TblNoticePersistent
                {
                    id = noticeId,
                });
                return (affected > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 즉시 공지
    /// </summary>
    /// <returns></returns>
    public async Task<List<NoticeImmediate>> FetchImmediateNoticesAsync()
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }

        try
        {
            var notices = new List<NoticeImmediate>();
            var serverInfos = await _serverRepo.FetchCommunityServerInfosAsync(db);

            var query = $"select * from tbl_notice_immediate order by id desc;";
            var result = await db.QueryAsync<TblNoticeImmediate>(query);
            var tbls = result.ToList();

            foreach (var tbl in tbls)
            {
                var serverId = tbl.server_id;
                var serverName = serverInfos.FirstOrDefault(x => x.server_id == serverId)?.server_name ?? string.Empty;
                notices.Add(new NoticeImmediate
                {
                    Id = tbl.id,
                    ServerName = serverName,
                    Title = tbl.title,
                    Content = tbl.content,
                    Showed = tbl.showed,
                    NoticeDate = tbl.notice_date,
                    UpdatedDate = tbl.updated_date,
                    CreatedDate = tbl.created_date,
                });
            }

            return notices;

        }
        catch (Exception)
        {
            return new();

        }
    }

    public async Task<bool> AddImmediateNoticeAsync(string title, string content, DateTime noticeTime)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return false;
        }

        try
        {
            // 커뮤니티 서버에만 공지를 등록한다.
            var now = DateTimeHelper.Now;
            var serverInfos = await _serverRepo.FetchCommunityServerInfosAsync(db);
            foreach (var serverInfo in serverInfos)
            {
                var serverId = serverInfo.server_id;
                var query = $"INSERT INTO tbl_notice_immediate VALUES(NULL, @server_id, @title, @content, @showed, @notice_date, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
                var affected = await db.ExecuteAsync(query, new TblNoticeImmediate
                {
                    server_id = serverId,
                    title = title,
                    content = content,
                    showed = 0,

                    notice_date = noticeTime,
                    updated_date = now,
                    created_date = now,
                });
                if (affected <= 0)
                    return false;
            }

            return true;

        }
        catch (Exception)
        {
            return false;

        }
    }

    public async Task<bool> DeleteImmediateNoticeAsync(ulong noticeId)
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

                var query = $"DELETE FROM tbl_notice_immediate WHERE id = @id;";
                var affected = await db.ExecuteAsync(query, new TblNoticeImmediate
                {
                    id = noticeId,
                });
                return (affected > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public Task<List<TblServerList>> FetchCommunityServerInfosAsync()
    {
        return _serverInfoRepo.GetCommunityServerInfosAsync();
    }

    public async Task<List<TblFeatureControl>> FetchFeatureInfosAsync()
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }

        try
        {
            var query = $"select * from tbl_feature_control order by id desc;";
            var result = await db.QueryAsync<TblFeatureControl>(query);
            return result.ToList();
        }
        catch (Exception)
        {
            return new();

        }
    }

    public async Task<bool> AddFeatureBlockAsync(FeatureControlType controlType)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
            return false;

        try
        {
            var query = $"INSERT INTO tbl_feature_control VALUES(NULL, @control_type, @blocked, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
            var affected = await db.ExecuteAsync(query, new TblFeatureControl
            {
                control_type = (int)controlType,
                blocked = 1,                
            });
            if (affected <= 0)
                return false;

            return true;
        }
        catch (Exception)
        {
            return true;

        }
    }
    public async Task<bool> DeleteFeatureBlockAsync(ulong id)
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

                var query = $"DELETE FROM tbl_feature_control WHERE id = @id;";
                var affected = await db.ExecuteAsync(query, new{ id = id, });
                return (affected > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public async Task<List<TblNoticeEmergency>> FetchEmergencyNoticesAsync()
    {
        await using (var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
                return new List<TblNoticeEmergency>();

            try
            {
                var query = $"select * from tbl_notice_emergency order by id desc;";
                var result = await db.QueryAsync<TblNoticeEmergency>(query);
                return result.ToList();
            }
            catch (Exception)
            {
                return new List<TblNoticeEmergency>();
            }
        }
    }
    public async Task<bool> AddEmergencyNoticeAsync(DateTime noticeTime)
    {
        await using (var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            try
            {
                var query = $"INSERT INTO tbl_notice_emergency VALUES(NULL, @checked_date, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
                var affected = await db.ExecuteAsync(query, new TblNoticeEmergency
                {
                    checked_date = noticeTime,                    
                });

                return affected > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
    public async Task<bool> DeleteEmergencyNoticeAsync(ulong noticeId)
    {
        await using (var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
                return false;

            try
            {
                var query = $"DELETE FROM tbl_notice_emergency WHERE id = @id;";
                var affected = await db.ExecuteAsync(query, new TblNoticeEmergency
                {
                    id = noticeId,
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
