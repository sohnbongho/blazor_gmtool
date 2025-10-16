using AdminTool.Models;
using Dapper;
using Library.Connector;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.messages;
using Library.Repository.Mysql;
using log4net;
using Newtonsoft.Json;
using System.Reflection;

namespace AdminTool.Repositories.User;

public class UserRepository : IUserRepository
{
    private UserSharedRepo _userRepo = UserSharedRepo.Of();
    private UserSessionSharedRepo _userSession = UserSessionSharedRepo.Of();
    private UserLogSharedRepo _userLogRepo = UserLogSharedRepo.Of();
    private ServerInfoSharedRepo _serverInfoRepo = new();
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private ServerInfoSharedRepo _serverRepo = new ServerInfoSharedRepo();

    /// <summary>
    /// 유저 정보
    /// </summary>    
    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserName(string name)
    {
        var users = new List<(TblMember, TblCharacter, TblLogMember)>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return users;
        }
        var query = $"select * from tbl_character where nickname = @nickname limit 100;";
        var resultChracter = await db.QueryAsync<TblCharacter>(query, new TblCharacter { nickname = name });
        var tblCharacters = resultChracter?.ToList() ?? new List<TblCharacter>();

        foreach (var character in tblCharacters)
        {
            var charSeq = character.char_seq;

            query = $"select * from tbl_member where char_seq = @char_seq limit 1;";
            var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { char_seq = charSeq });
            var tblMember = resultMember?.FirstOrDefault() ?? null;
            if (tblMember == null)
            {
                tblMember = new TblMember { char_seq = 0 };
            }

            var userSeq = tblMember.user_seq;
            query = $"select * from tbl_log_member where user_seq = @user_seq limit 1;";
            var resultLogMember = await db.QueryAsync<TblLogMember>(query, new TblLogMember { user_seq = userSeq });
            var tblLogMember = resultLogMember?.FirstOrDefault() ?? null;
            if (tblLogMember == null)
            {
                tblLogMember = new TblLogMember { user_seq = 0 };
            }
            users.Add((tblMember, character, tblLogMember));
        }
        return users;
    }

    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserId(string id)
    {
        var users = new List<(TblMember, TblCharacter, TblLogMember)>();
        await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
        if (db == null)
        {
            return users;
        }
        var query = string.Empty;

        query = $"select * from tbl_member where user_id = @user_id limit 100;";
        var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { user_id = id });
        var tblMembers = resultMember?.ToList() ?? new List<TblMember>();

        foreach (var tblMember in tblMembers)
        {
            var charSeq = tblMember.char_seq;
            query = $"select * from tbl_character where char_seq = @char_seq limit 1;";
            var resultCharacter = await db.QueryAsync<TblCharacter>(query,
                new TblCharacter { char_seq = charSeq });
            var tblCharacter = resultCharacter?.FirstOrDefault() ?? null;
            if (tblCharacter == null)
            {
                tblCharacter = new TblCharacter { char_seq = 0 };
            }

            var userSeq = tblMember.user_seq;
            query = $"select * from tbl_log_member where user_seq = @user_seq limit 1;";
            var resultLogMember = await db.QueryAsync<TblLogMember>(query, new TblLogMember { user_seq = userSeq });
            var tblLogMember = resultLogMember?.FirstOrDefault() ?? null;
            if (tblLogMember == null)
            {
                tblLogMember = new TblLogMember { user_seq = 0 };
            }

            users.Add((tblMember, tblCharacter, tblLogMember));
        }
        return users;
    }


    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByCharSeq(string charSeqStr)
    {
        try
        {
            var charSeq = ulong.Parse(charSeqStr);
            var users = new List<(TblMember, TblCharacter, TblLogMember)>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return users;
            }
            var query = string.Empty;

            query = $"select * from tbl_member where char_seq = @char_seq limit 1;";
            var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { char_seq = charSeq });
            var tblMembers = resultMember?.ToList() ?? new List<TblMember>();

            foreach (var tblMember in tblMembers)
            {
                query = $"select * from tbl_character where char_seq = @char_seq limit 1;";
                var resultCharacter = await db.QueryAsync<TblCharacter>(query,
                    new TblCharacter { char_seq = charSeq });
                var tblCharacter = resultCharacter?.FirstOrDefault() ?? null;
                if (tblCharacter == null)
                {
                    tblCharacter = new TblCharacter { char_seq = 0 };
                }

                var userSeq = tblMember.user_seq;
                query = $"select * from tbl_log_member where user_seq = @user_seq limit 1;";
                var resultLogMember = await db.QueryAsync<TblLogMember>(query, new TblLogMember { user_seq = userSeq });
                var tblLogMember = resultLogMember?.FirstOrDefault() ?? null;
                if (tblLogMember == null)
                {
                    tblLogMember = new TblLogMember { user_seq = 0 };
                }

                users.Add((tblMember, tblCharacter, tblLogMember));
            }
            return users;

        }
        catch (Exception)
        {
            return new List<(TblMember, TblCharacter, TblLogMember)>();
        }
    }

    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserSeq(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var users = new List<(TblMember, TblCharacter, TblLogMember)>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return users;
            }
            var query = string.Empty;

            query = $"select * from tbl_member where user_seq = @user_seq limit 1;";
            var resultMember = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
            var tblMembers = resultMember?.ToList() ?? new List<TblMember>();

            foreach (var tblMember in tblMembers)
            {
                var charSeq = tblMember.char_seq;
                query = $"select * from tbl_character where char_seq = @char_seq limit 1;";
                var resultCharacter = await db.QueryAsync<TblCharacter>(query,
                    new TblCharacter { char_seq = charSeq });
                var tblCharacter = resultCharacter?.FirstOrDefault() ?? null;
                if (tblCharacter == null)
                {
                    tblCharacter = new TblCharacter { char_seq = 0 };
                }

                query = $"select * from tbl_log_member where user_seq = @user_seq limit 1;";
                var resultLogMember = await db.QueryAsync<TblLogMember>(query, new TblLogMember { user_seq = userSeq });
                var tblLogMember = resultLogMember?.FirstOrDefault() ?? null;
                if (tblLogMember == null)
                {
                    tblLogMember = new TblLogMember { user_seq = 0 };
                }

                users.Add((tblMember, tblCharacter, tblLogMember));
            }
            return users;

        }
        catch (Exception)
        {
            return new List<(TblMember, TblCharacter, TblLogMember)>();
        }
    }

    public async Task<TblMember> FetchMemberAsync(ulong userSeq)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new();
            }
            var query = string.Empty;

            query = $"select * from tbl_member where user_seq = @user_seq limit 1;";
            var result = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
            var tblMember = result?.FirstOrDefault() ?? null;

            return tblMember != null ? tblMember : new();
        }
        catch (Exception)
        {
            return new();
        }
    }
    public Task<(bool, TblMemberSession)> FetchUserSessionInfoByUserSeqAsync(ulong userSeq)
    {
        return _userSession.FetchUserSessionInfoByUserSeqAsync(userSeq);
    }

    /// <summary>
    /// 구매 로그
    /// </summary>    
    public async Task<List<TblLogPurchase>> FetchPurchaseLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblLogPurchase>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return models;
            }
            var query = string.Empty;

            query = $"select * from tbl_log_purchase where user_seq = @user_seq AND created_date > @start_date AND created_date < @end_date order by id desc limit 255";
            var result = await db.QueryAsync<TblLogPurchase>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });
            var rows = result?.ToList() ?? new List<TblLogPurchase>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblLogPurchase>();
        }
    }
    public async Task<List<TblPurchaseReceipt>> FetchPurchaseRecepitAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblPurchaseReceipt>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return models;
            }
            var query = string.Empty;

            query = $"select * from tbl_purchase_receipt where user_seq = @user_seq AND created_date > @start_date AND created_date < @end_date order by id desc limit 255";
            var result = await db.QueryAsync<TblPurchaseReceipt>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });
            var rows = result.ToList();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblPurchaseReceipt>();
        }
    }
    public async Task<List<LogConnectedTime>> FetchPlayTimeLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<LogConnectedTime>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return models;
            }
            var query = string.Empty;
            var serverInfos = await _serverRepo.GetServerInfosAsync(db);

            query = $"select * from tbl_log_connected_time where user_seq = @user_seq AND created_date > @start_date AND created_date < @end_date order by id desc limit 255";
            var result = await db.QueryAsync<TblLogConnectedTime>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });
            var rows = result.ToList();

            return rows.Select(x => new LogConnectedTime
            {
                Id = x.id,
                ServerName = serverInfos.FirstOrDefault(y => y.server_id == x.server_id)?.server_name ?? string.Empty,
                GameModeType = ConvertHelper.ToEnum<GameModeType>(x.game_mode),
                UserSeq = x.user_seq,
                DurationSecond = x.duration_sec,
                CreatedDate = x.created_date,
            }).ToList();

        }
        catch (Exception)
        {
            return new List<LogConnectedTime>();
        }
    }
    /// <summary>
    /// CurrencyInven
    /// </summary>
    /// <param name="userSeq"></param>
    /// <returns></returns>    
    public async Task<List<TblInventoryCurrency>> FetchInventoryCurrencys(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblLogPurchase>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblInventoryCurrency>();
            }
            var query = string.Empty;

            query = $"select * from tbl_inventory_currency where user_seq = @user_seq order by id desc limit 255";
            var result = await db.QueryAsync<TblInventoryCurrency>(query, new TblInventoryCurrency { user_seq = userSeq });
            var rows = result?.ToList() ?? new List<TblInventoryCurrency>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblInventoryCurrency>();
        }

    }

    public async Task<List<TblInventoryGamemodeExp>> FetchGameModeExps(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblInventoryGamemodeExp>();
            }
            var query = string.Empty;

            query = $"select * from tbl_inventory_gamemode_exp where user_seq = @user_seq order by id desc limit 255";
            var result = await db.QueryAsync<TblInventoryGamemodeExp>(query, new { user_seq = userSeq });
            var rows = result?.ToList() ?? new List<TblInventoryGamemodeExp>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblInventoryGamemodeExp>();
        }

    }
    public async Task<List<TblLogCurrency>> FetchLogCurrencys(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblLogPurchase>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogCurrency>();
            }
            var query = string.Empty;

            query = $"select * from tbl_log_currency where user_seq = @user_seq order by id desc limit 255";
            var result = await db.QueryAsync<TblLogCurrency>(query, new TblLogCurrency { user_seq = userSeq });
            var rows = result?.ToList() ?? new List<TblLogCurrency>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblLogCurrency>();
        }
    }
    public async Task<List<TblMemberDeactive>> FetchDeactiveAccount(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblMemberDeactive>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblMemberDeactive>();
            }
            var query = string.Empty;

            query = $"select * from tbl_member_deactive where user_seq = @user_seq order by id desc limit 255";
            var result = await db.QueryAsync<TblMemberDeactive>(query, new TblMemberDeactive { user_seq = userSeq });
            var rows = result?.ToList() ?? new List<TblMemberDeactive>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblMemberDeactive>();
        }
    }

    public async Task<List<TblLogMemberDeactive>> FetchDeactiveAccountLogs(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblLogMemberDeactive>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogMemberDeactive>();
            }
            var query = string.Empty;

            query = $"select * from tbl_log_member_deactive where user_seq = @user_seq order by id desc limit 255";
            var result = await db.QueryAsync<TblLogMemberDeactive>(query, new TblLogMemberDeactive { user_seq = userSeq });
            var rows = result?.ToList() ?? new List<TblLogMemberDeactive>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblLogMemberDeactive>();
        }
    }

    public async Task<bool> BanUser(string userSeqStr, DateTime banExpiryDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            // DB에 값 갱신
            var banned = await _userRepo.UpdateUserBanAsync(userSeq, banExpiryDate);
            return banned;

        }
        catch (Exception ex)
        {
            _logger.Error($"BanUser:{userSeqStr}", ex);
            return false;
        }
    }

    public async Task<bool> BanArticleCommentAsync(ulong userSeq, string title, DateTime banExpiryDate)
    {
        try
        {
            // DB에 값 갱신
            var banned = await _userRepo.BanArticleCommentAsync(userSeq, title, banExpiryDate);
            return banned;

        }
        catch (Exception ex)
        {
            _logger.Error($"KickUser:{userSeq}", ex);
            return false;
        }
    }
    public async Task<List<TblLogGameplay>> FetchGamePlayLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);

            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new();
            }

            var query = "SELECT * FROM tbl_log_gameplay WHERE user_seq = @user_seq AND created_date > @start_date AND created_date < @end_date ORDER BY id desc limit 255";

            var result = await db.QueryAsync<TblLogGameplay>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,

            });
            return result?.ToList() ?? new();
        }
        catch (Exception)
        {
            return new();
        }
    }

    // Community 서버 정보들
    public Task<List<TblServerList>> FetchCommunityServerInfosAsync()
    {
        return _serverInfoRepo.GetCommunityServerInfosAsync();
    }
    public Task<bool> AddLogBanAsync(ulong userSeq, UserBanType userBanType, string title, DateTime banExpiryDate)
    {
        return _userLogRepo.AddBanAsync(userSeq, userBanType, title, banExpiryDate);
    }
    public async Task<List<TblLogBanHistory>> FetchUserBanHistoryAsync(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            return await _userLogRepo.FetchUserBanHistoryAsync(userSeq);
        }
        catch (Exception)
        {
            return new();
        }

    }

    public async Task<ErrorCode> GiftAppearanceItemAsync(ulong targetUserSeq, int itemSn, int amount, string text)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return ErrorCode.DbInitializedError;
            }

            var alarm = new GmGiftItem
            {
                TargetUserSeq = targetUserSeq.ToString(),

                ItemId = itemSn,
                ItemAmount = amount,
                Text = text,
            };
            var now = DateTimeHelper.Now;

            var alarmJson = JsonConvert.SerializeObject(alarm, Formatting.Indented);
            ulong mailSeq = (ulong)SnowflakeIdGenerator.Instance.NextId();
            var mailGuid = Guid.NewGuid().ToString();

            {
                var query = "INSERT INTO tbl_mail_present VALUES(NULL, @mail_seq, @guid, @to_user_seq, @from_user_seq, @type, @content, @is_checked, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                var affected = await db.ExecuteAsync(query, new TblMailPresent
                {
                    mail_seq = mailSeq,
                    guid = mailGuid,

                    to_user_seq = targetUserSeq,
                    from_user_seq = 0,
                    type = (int)MailType.GmTool,
                    content = alarmJson,

                    is_checked = 0,
                    updated_date = now,
                    created_date = now,
                });
                if (affected <= 0)
                {
                    return ErrorCode.DbInsertedError;
                }
            }

            //로그
            {
                var query = "INSERT INTO tbl_log_gm_giveitem VALUES(NULL, @user_seq, @mail_seq, @guid, @item_sn, @amount, @content, CURRENT_TIMESTAMP)";
                var affected = await db.ExecuteAsync(query, new TblLogGmGiveitem
                {
                    user_seq = targetUserSeq,
                    mail_seq = mailSeq,
                    guid = mailGuid,

                    item_sn = itemSn,
                    amount = amount,
                    content = alarmJson,
                    created_date = now,
                });

            }
            return ErrorCode.Succeed;
        }
        catch (Exception)
        {
            return ErrorCode.DbInsertedError;
        }
    }
    public async Task<List<TblLogGmGiveitem>> FetchGmGiftLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var models = new List<TblLogGmGiveitem>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return models;
            }
            var query = string.Empty;

            query = $"select * from tbl_log_gm_giveitem where user_seq = @user_seq AND created_date > @start_date AND created_date < @end_date order by id desc limit 255";
            var result = await db.QueryAsync<TblLogGmGiveitem>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });
            var rows = result?.ToList() ?? new List<TblLogGmGiveitem>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblLogGmGiveitem>();
        }
    }
    public async Task<List<TblLogPurchaseRank>> FetchSellItemDescLogsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var models = new List<TblLogPurchaseRank>();
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return models;
            }
            var query = string.Empty;

            query = $"SELECT item_sn, COUNT(*) AS sale_count FROM tbl_log_purchase WHERE created_date BETWEEN @start_date AND @end_date GROUP BY item_sn ORDER BY sale_count DESC LIMIT 500;";
            var result = await db.QueryAsync<TblLogPurchaseRank>(query, new
            {
                start_date = startDate,
                end_date = endDate,
            });
            var rows = result?.ToList() ?? new List<TblLogPurchaseRank>();

            return rows;

        }
        catch (Exception)
        {
            return new List<TblLogPurchaseRank>();
        }
    }

    public async Task<int> FetchSellItemCountAsync(int itemSn, DateTime startDate, DateTime endDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return 0;
            }
            var query = string.Empty;

            query = $"SELECT item_sn, COUNT(*) AS sale_count FROM tbl_log_purchase WHERE item_sn = @item_sn AND created_date BETWEEN @start_date AND @end_date GROUP BY item_sn;";
            var result = await db.QueryAsync<TblLogPurchaseRank>(query, new
            {
                item_sn = itemSn,
                start_date = startDate,
                end_date = endDate,
            });

            var sellCount = result?.FirstOrDefault()?.sale_count ?? 0;
            return sellCount;

        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<List<TblLogItemDistribution>> FetchItemAwardLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogItemDistribution>();
            }
            var query = string.Empty;

            query = $"SELECT * FROM tbl_log_item_distribution WHERE user_seq = @user_seq AND created_date BETWEEN @start_date AND @end_date order by id desc limit 255;";
            var result = await db.QueryAsync<TblLogItemDistribution>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblLogItemDistribution>();
        }
    }

    public async Task<List<TblLogCurrencyChange>> FetchCurrencyLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogCurrencyChange>();
            }
            var query = string.Empty;

            query = $"SELECT * FROM tbl_log_currency_change WHERE user_seq = @user_seq AND created_date BETWEEN @start_date AND @end_date order by id desc limit 255;";
            var result = await db.QueryAsync<TblLogCurrencyChange>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblLogCurrencyChange>();
        }
    }

    public async Task<List<TblLogLogin>> FetchLoginLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogLogin>();
            }

            var query = $"SELECT * FROM tbl_log_login WHERE user_seq = @user_seq AND created_date BETWEEN @start_date AND @end_date order by id desc limit 255;";
            var result = await db.QueryAsync<TblLogLogin>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblLogLogin>();
        }
    }
    public async Task<List<TblLogMemberDailyStamp>> FetchDailyStampLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate)
    {
        try
        {
            await using var db = await MySqlConnectionHelper.Instance.ConnectionFactoryAsync(DbConnectionType.Game);
            if (db == null)
            {
                return new List<TblLogMemberDailyStamp>();
            }

            var query = $"SELECT * FROM tbl_log_member_daily_stamp WHERE user_seq = @user_seq AND created_date BETWEEN @start_date AND @end_date order by id desc limit 255;";
            var result = await db.QueryAsync<TblLogMemberDailyStamp>(query, new
            {
                user_seq = userSeq,
                start_date = startDate,
                end_date = endDate,
            });

            return result.ToList();

        }
        catch (Exception)
        {
            return new List<TblLogMemberDailyStamp>();
        }
    }

}
