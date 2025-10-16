using Dapper;
using Library.Connector;
using Library.Data.Models;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using log4net;
using System.Reflection;

namespace AdminTool.Repositories.Notice;

public interface INoticeBoardRepository
{
    Task<List<Models.NoticeBoardData>> FetchNoticesAsync();
    Task<bool> AddNoticeAsync(Models.NoticeBoardData boardData);
    Task<bool> DeleteNoticeAsync(ulong noticeSeq);
}
public class NoticeBoardRepository : INoticeBoardRepository
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    public async Task<List<Models.NoticeBoardData>> FetchNoticesAsync()
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }

        try
        {
            var query = $"select * from tbl_notice_board order by id desc limit 100;";
            var notices = await db.QueryAsync<TblNoticeBoard>(query);

            var noticeBoardDatas = new List<Models.NoticeBoardData>();
            string[] languageCodes = new string[] {
                LanguageCode.Kor,
                LanguageCode.Eng,
                LanguageCode.Zhs,
                LanguageCode.Zht,
                LanguageCode.Spa,
                LanguageCode.Tha,
                LanguageCode.Vie,
                LanguageCode.Jpn, };
            foreach (var notice in notices)
            {
                var boardType = ConvertHelper.ToEnum<NoticeBoardType>(notice.board_type);
                var noticeSeq = notice.notice_seq;
                var noticeBoardData = new Models.NoticeBoardData
                {
                    Id = notice.id,
                    NoticeSeq = noticeSeq,
                    NoticeBoardType = boardType,
                    Title = notice.title,
                    ExpiryDate = notice.expiry_date,
                    UpdatedDate = notice.updated_date,
                    CreatedDate = notice.created_date,
                };

                var contentQuery = $"select * from tbl_notice_board_content where notice_seq = @notice_seq;";
                var results = await db.QueryAsync<TblNoticeBoardContent>(contentQuery, new { notice_seq = noticeSeq });
                var boradCotents = results.ToList();
                foreach (var boardContent in boradCotents)
                {
                    var languageCode = boardContent.language_code;
                    var contentStr = boardContent.content;
                    var contentTitle = boardContent.title;
                    noticeBoardData.Contents[languageCode] = new Models.NoticeBoardContentData
                    {
                        Content = contentStr,
                        Title = contentTitle,
                    };
                }
                noticeBoardDatas.Add(noticeBoardData);
            }

            return noticeBoardDatas;

        }
        catch (Exception)
        {
            return new();

        }
    }
    public async Task<bool> AddNoticeAsync(Models.NoticeBoardData boardData)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }
        await using var transaction = await db.BeginTransactionAsync();

        try
        {
            var noticeSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();

            {
                var query = $"INSERT INTO tbl_notice_board VALUES(NULL, @notice_seq, @board_type, @title, @expiry_date, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
                var result = await db.ExecuteAsync(query, new TblNoticeBoard
                {
                    notice_seq = noticeSeq,
                    board_type = (short)boardData.NoticeBoardType,
                    title = boardData.Title,
                    expiry_date = boardData.ExpiryDate,
                }, transaction);
            }

            // 
            string[] languageCodes = new string[] {
                LanguageCode.Kor,
                LanguageCode.Eng,
                LanguageCode.Zhs,
                LanguageCode.Zht,
                LanguageCode.Spa,
                LanguageCode.Tha,
                LanguageCode.Vie,
                LanguageCode.Jpn, };

            foreach (var languageCode in languageCodes)
            {
                if (boardData.Contents.TryGetValue(languageCode, out var content))
                {
                    var contentStr = content.Content;
                    var title = content.Title;

                    var query = $"INSERT INTO tbl_notice_board_content VALUES (NULL, @notice_seq, @title, @language_code, @content, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";
                    var result = await db.ExecuteAsync(query, new TblNoticeBoardContent
                    {
                        notice_seq = noticeSeq,
                        language_code = languageCode,
                        title = title,
                        content = contentStr,
                    }, transaction);
                }
            }

            await transaction.CommitAsync();

            return true;

        }
        catch (Exception ex)
        {
            _logger.Error("AddNoticeAsync", ex);
            await transaction.RollbackAsync();
            return false;

        }
    }

    public async Task<bool> DeleteNoticeAsync(ulong noticeSeq)
    {
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return new();
        }
        await using var transaction = await db.BeginTransactionAsync();

        try
        {
            {
                var query = $"DELETE FROM tbl_notice_board WHERE notice_seq = @notice_seq;";
                var result = await db.ExecuteAsync(query, new
                {
                    notice_seq = noticeSeq,
                }, transaction);
            }

            // 
            {
                var query = $"DELETE FROM tbl_notice_board_content WHERE notice_seq = @notice_seq;";
                var result = await db.ExecuteAsync(query, new
                {
                    notice_seq = noticeSeq,
                }, transaction);
            }

            await transaction.CommitAsync();

            return true;

        }
        catch (Exception ex)
        {
            _logger.Error("AddNoticeAsync", ex);
            await transaction.RollbackAsync();
            return false;

        }
    }
}
