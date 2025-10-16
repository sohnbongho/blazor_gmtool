using AdminTool.Models;
using AdminTool.Repositories.User;
using Akka.Actor;
using Library.AkkaActors;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.messages;
using log4net;
using Messages;
using System.Reflection;

namespace AdminTool.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// 캐릭터 정보
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserName(string name)
    {
        var result = await _repo.FetchUserByUserName(name);
        return result;
    }
    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserId(string id)
    {
        var result = await _repo.FetchUserByUserId(id);
        return result;
    }

    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserSeq(string userSeq)
    {
        var result = await _repo.FetchUserByUserSeq(userSeq);
        return result;
    }
    public async Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByCharSeq(string charSeq)
    {
        var result = await _repo.FetchUserByCharSeq(charSeq);
        return result;
    }

    /// <summary>
    /// 구매 로그
    /// </summary>    
    public async Task<List<LogPurchase>> FetchPurchaseLogsAsync(string userSeq, DateTime startDate, DateTime endDate)
    {
        var result = await _repo.FetchPurchaseLogsAsync(userSeq, startDate, endDate);
        return result.Select(x => new LogPurchase
        {
            Id = x.id,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            UserSeq = x.user_seq,
            TargetUserSeq = x.target_user_seq,
            ItemPrice = x.item_price,
            CurrencyType = ConvertHelper.ToEnum<CurrencyType>(x.price_type),
            PurchaseType = ConvertHelper.ToEnum<PurchaseType>(x.purchase_type),
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<PurchaseReceipt>> FetchPurchaseRecepitAsync(string userSeq, DateTime startDate, DateTime endDate)
    {
        var result = await _repo.FetchPurchaseRecepitAsync(userSeq, startDate, endDate);
        return result.Select(x => new PurchaseReceipt
        {
            Id = x.id,
            UserSeq = x.user_seq,
            ReceiptType = ConvertHelper.ToEnum<ReceiptType>(x.receipt_type),
            OrderId = x.order_id,
            ProductId = x.product_id,
            ItemType = ConvertHelper.ToEnum<MainItemType>(x.item_type),
            ItemSn = x.item_sn,
            Amount = x.amount,
            Received = x.received,
            UpdatedDate = x.updated_date,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<LogConnectedTime>> FetchPlayTimeLogsAsync(string userSeq, DateTime startDate, DateTime endDate)
    {
        var result = await _repo.FetchPlayTimeLogsAsync(userSeq, startDate, endDate);
        return result;
    }

    public async Task<List<TblInventoryCurrency>> FetchInventoryCurrencys(string userSeq)
    {
        var result = await _repo.FetchInventoryCurrencys(userSeq);
        return result;
    }
    public async Task<List<TblInventoryGamemodeExp>> FetchGameModeExps(string userSeq)
    {
        var result = await _repo.FetchGameModeExps(userSeq);
        return result;
    }
    public async Task<List<TblLogCurrency>> FetchLogCurrencys(string userSeq)
    {
        var result = await _repo.FetchLogCurrencys(userSeq);
        return result;
    }

    public async Task<List<TblMemberDeactive>> FetchDeactiveAccount(string userSeq)
    {
        var result = await _repo.FetchDeactiveAccount(userSeq);
        return result;
    }
    public async Task<List<TblLogMemberDeactive>> FetchDeactiveAccountLogs(string userSeq)
    {
        var result = await _repo.FetchDeactiveAccountLogs(userSeq);
        return result;
    }

    /// <summary>
    /// UserKick처리
    /// </summary>
    /// <param name="userSeqStr"></param>
    /// <returns></returns>
    public async Task<bool> BanUser(string title, string userSeqStr, DateTime banExpiryDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var banned = await _repo.BanUser(userSeqStr, banExpiryDate);
            if (banned == false)
                return banned;

            await _repo.AddLogBanAsync(userSeq, UserBanType.PlayBan, title, banExpiryDate);

            // Mq쪽에 유저 킥을 보내자            
            await NotiUserAsync(userSeq);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("KickUser", ex);
            return false;
        }
    }

    public async Task<bool> BanArticleCommentAsync(string userSeqStr, string title, DateTime banExpiryDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var banned = await _repo.BanArticleCommentAsync(userSeq, title, banExpiryDate);
            if (banned == false)
                return banned;

            await _repo.AddLogBanAsync(userSeq, UserBanType.ArticleComment, title, banExpiryDate);

            // Mq쪽에 유저 킥을 보내자
            await NotiUserAsync(userSeq);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("KickUser", ex);
            return false;
        }
    }

    private async Task<bool> NotiUserAsync(ulong userSeq)
    {
        var nats = ActorRefsHelper.Instance.Get(ActorPaths.MessageQueuePath);
        if (nats == ActorRefs.Nobody)
            return true;

        var tblMember = await _repo.FetchMemberAsync(userSeq);
        if (tblMember.char_seq == 0)
            return true;

        var targetCharSeq = tblMember.char_seq;
        var (fined, tblSession) = await _repo.FetchUserSessionInfoByUserSeqAsync(userSeq);
        if (fined == false)
            return true;

        var toServerId = tblSession.connected_community_serverid;
        if (toServerId == 0)
            return true;

        nats.Tell(new S2SMessage.NatsPubliish
        {
            NatsMessageWrapper = new NatsMessages.NatsMessageWrapper
            {
                FromServerId = -1,
                ToServerId = toServerId,
                TargetCharSeq = targetCharSeq.ToString(),

                GmtoolUserKickNoti = new GmtoolUserKickNoti
                {
                    TargetUserSeq = userSeq.ToString(),
                }
            }
        });
        return true;
    }

    public async Task<List<LogGameplay>> FetchGamePlayLogsAsync(string userSeq, DateTime startDate, DateTime endDate)
    {
        var result = await _repo.FetchGamePlayLogsAsync(userSeq, startDate, endDate);
        return result.Select(x => new LogGameplay
        {
            Id = x.id,
            ServerId = x.server_id,
            UserSeq = x.user_seq,
            GameGuid = x.game_guid,
            GameMode = ConvertHelper.ToEnum<GameModeType>(x.game_mode),
            PlayType = ConvertHelper.ToEnum<PlayType>(x.play_type),
            ItemName = ConvertHelper.GetItemName(x.reward_index),
            Amount = x.amount,
            JsonData = x.json_data,
            CreatedDate = x.created_date,
        }).ToList();
    }

    public async Task<bool> NotiGmChatUsersAsync(string gmName, string gmMessage)
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

                    GmtoolChatNoti = new GmtoolChatNoti
                    {
                        GmName = gmName,
                        Chat = gmMessage,
                    }
                }
            });
        }
        return true;
    }
    public async Task<List<LogBanHistory>> FetchUserBanHistoryAsync(string userSeqStr)
    {
        var result = await _repo.FetchUserBanHistoryAsync(userSeqStr);

        return result.Select(x => new LogBanHistory
        {
            Id = x.id,
            UserSeq = x.user_seq,
            UserBanType = ConvertHelper.ToEnum<UserBanType>(x.ban_type),
            Title = x.title,
            ExpirationDate = x.ban_expiry_date,
            CreatedDate = x.created_date,
        }).ToList();
    }

    /// <summary>
    /// 아이템 지급
    /// </summary>    
    public async Task<ErrorCode> ValidateGiveAppearanceItemAsync(string userSeqStr, string itemSnStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var itemSn = int.Parse(itemSnStr);

            var tblMember = await _repo.FetchMemberAsync(userSeq);
            if (tblMember.user_seq == 0)
                return ErrorCode.NotFoundUserSeq;

            var itemType = ItemDBLoaderHelper.Instance.GetItemType(itemSn);
            switch (itemType)
            {
                case MainItemType.Character:
                case MainItemType.Clothing:
                case MainItemType.Accessory:
                    {
                        return ErrorCode.Succeed;
                    }
            }
            return ErrorCode.InvalidGiveItem;
        }
        catch (Exception ex)
        {
            _logger.Error("faild to FetchMemberAsync", ex);
            return ErrorCode.DbSelectedError;
        }
    }
    public async Task<TblMember> FetchMemberAsync(string userSeqStr)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);

            var tblMember = await _repo.FetchMemberAsync(userSeq);
            return tblMember;
        }
        catch (Exception ex)
        {
            _logger.Error("faild to FetchMemberAsync", ex);
            return new TblMember();
        }
    }
    public async Task<ErrorCode> GiftAppearanceItemAsync(string userSeqStr, string itemSnStr, int amount, string text)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var itemSn = int.Parse(itemSnStr);
            var errorCode = await _repo.GiftAppearanceItemAsync(userSeq, itemSn, amount, text);

            return errorCode;
        }
        catch (Exception ex)
        {
            _logger.Error("faild to GiftItemAsync", ex);
            return ErrorCode.DbInsertedError;
        }
    }
    public async Task<List<LogGmGiveitem>> FetchGmGiftLogsAsync(string userSeq, DateTime startDate, DateTime endDate)
    {
        var result = await _repo.FetchGmGiftLogsAsync(userSeq, startDate, endDate);
        return result.Select(x => new LogGmGiveitem
        {
            Id = x.id,
            UserSeq = x.user_seq,
            MailSeq = x.mail_seq,
            MailGuid = x.guid ?? string.Empty,
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            Amount = x.amount,
            Content = x.content,
            CreatedDate = x.created_date,
        }).ToList();
    }
    public async Task<List<LogPurchaseRank>> FetchSellItemDescLogsAsync(DateTime startDate, DateTime endDate)
    {
        var result = await _repo.FetchSellItemDescLogsAsync(startDate, endDate);
        return result.Select(x => new LogPurchaseRank
        {
            ItemName = ConvertHelper.GetItemName(x.item_sn),
            PriceType = ConvertHelper.GetItemPriceType(x.item_sn),
            ItemPrice = ConvertHelper.GetItemPrice(x.item_sn),
            SaleCount = x.sale_count,
        }).ToList();
    }
    public async Task<List<LogPurchaseRank>> FetchSellItemCountAsync(int itemSn, DateTime startDate, DateTime endDate)
    {
        var sellCount = await _repo.FetchSellItemCountAsync(itemSn, startDate, endDate);

        return new List<LogPurchaseRank> {new LogPurchaseRank
        {
            ItemName = ConvertHelper.GetItemName(itemSn),
            PriceType = ConvertHelper.GetItemPriceType(itemSn),
            ItemPrice = ConvertHelper.GetItemPrice(itemSn),
            SaleCount  = sellCount,

        } };
    }
    
    public async Task<List<LogItemDistribution>> FetchItemAwardLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var result = await _repo.FetchItemAwardLogsAsync(userSeq, startDate, endDate);
            return result.Select(x => new LogItemDistribution
            {
                Id = x.id,
                ServerId = x.server_id,
                UserSeq = x.user_seq,
                Guid = x.guid,
                ReasonCode = ConvertHelper.ToEnum<ItemGainReason>(x.reason_code),
                ItemName = ConvertHelper.GetItemName(x.item_sn),
                Amount = x.amount,
                JsonData = x.json_data,
                CreatedDate = x.created_date,
            }).ToList();

        }
        catch (Exception)
        {
            return new();
        }
    }

    public async Task<List<LogCurrencyChange>> FetchCurrencyLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var result = await _repo.FetchCurrencyLogsAsync(userSeq, startDate, endDate);
            return result.Select(x => new LogCurrencyChange
            {
                Id = x.id,                
                UserSeq = x.user_seq,
                ItemGainReason = ConvertHelper.ToEnum<ItemGainReason>(x.item_gain_reason),
                CurrencyType = ConvertHelper.ToEnum<CurrencyType>(x.currency_type),
                AddedAmount = x.added_amount,
                Amount = x.amount,                
                CreatedDate = x.created_date,
            }).ToList();

        }
        catch (Exception)
        {
            return new();
        }
    }
    public async Task<List<LogLogin>> FetchLoginLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var result = await _repo.FetchLoginLogsAsync(userSeq, startDate, endDate);
            return result.Select(x => new LogLogin
            {
                Id = x.id,
                UserSeq = x.user_seq,
                RemoteAddress = x.remote_address,
                UserDeviceType = ConvertHelper.ToEnum<UserDeviceType>(x.device_type),
                DeviceUuid = x.device_uuid,
                CreatedDate = x.created_date,
            }).ToList();

        }
        catch (Exception)
        {
            return new List<LogLogin>();
        }
    }
    public async Task<List<LogMemberDailyStamp>> FetchDailyStampLogsAsync(string userSeqStr, DateTime startDate, DateTime endDate)
    {
        try
        {
            var userSeq = ulong.Parse(userSeqStr);
            var result = await _repo.FetchDailyStampLogsAsync(userSeq, startDate, endDate);
            return result.Select(x => new LogMemberDailyStamp
            {
                Id = x.id,
                UserSeq = x.user_seq,
                Year = x.year,
                Month = x.month,
                Count = x.count,
                PayDiamond = x.pay_diamond,
                PayClover = x.pay_clover,
                ItemName = ConvertHelper.GetItemName(x.item_sn),
                Amount = x.amount,
                UpdatedDate = x.updated_date,
                CreatedDate = x.created_date,
            }).ToList();

        }
        catch (Exception)
        {
            return new List<LogMemberDailyStamp>();
        }
    }
}
