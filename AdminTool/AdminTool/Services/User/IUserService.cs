using AdminTool.Models;
using Library.DBTables.MySql;
using Library.DTO;

namespace AdminTool.Services.User;

public interface IUserService
{
    /// <summary>
    /// 캐릭터 정보
    /// </summary>    
    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserName(string name);
    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserId(string id);

    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserSeq(string userSeq);
    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByCharSeq(string charSeq);

    /// <summary>
    /// 구매 로그
    /// </summary>    
    Task<List<LogPurchase>> FetchPurchaseLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 인게임 - 유료 영수증
    /// </summary>    
    Task<List<PurchaseReceipt>> FetchPurchaseRecepitAsync(string userSeq, DateTime startDate, DateTime endDate);
    
    Task<List<LogConnectedTime>> FetchPlayTimeLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    Task<List<TblInventoryCurrency>> FetchInventoryCurrencys(string userSeq);
    Task<List<TblLogCurrency>> FetchLogCurrencys(string userSeq);
    
    // 게임모드 경험치
    Task<List<TblInventoryGamemodeExp>> FetchGameModeExps(string userSeq);

    /// <summary>
    /// 비활성화 계정
    /// </summary>    
    Task<List<TblMemberDeactive>> FetchDeactiveAccount(string userSeq);
    Task<List<TblLogMemberDeactive>> FetchDeactiveAccountLogs(string userSeq);

    /// <summary>
    /// 유저 킥 관련 처리
    /// </summary>    
    Task<bool> BanUser(string title, string userSeq, DateTime banExpiryDate);

    /// <summary>
    /// 유저 채팅 금지
    /// </summary>
    /// <param name="userSeq"></param>
    /// <param name="banExpiryDate"></param>
    /// <returns></returns>
    Task<bool> BanArticleCommentAsync(string userSeq, string title, DateTime banExpiryDate);

    /// <summary>
    /// 개임 보상
    /// </summary>    
    Task<List<LogGameplay>> FetchGamePlayLogsAsync(string userSeq, DateTime startDate, DateTime endDate);


    Task<bool> NotiGmChatUsersAsync(string gmName, string gmMessage);

    Task<List<LogBanHistory>> FetchUserBanHistoryAsync(string userSeqStr);

    /// <summary>
    /// 아이템 지급
    /// </summary>    
    Task<ErrorCode> ValidateGiveAppearanceItemAsync(string userSeqStr, string itemSnStr);
    Task<ErrorCode> GiftAppearanceItemAsync(string userSeqStr, string itemSnStr, int amount, string text);
    Task<List<LogGmGiveitem>> FetchGmGiftLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    Task<TblMember> FetchMemberAsync(string userSeq);

    /// <summary>
    /// 아이템 판매 순위
    /// </summary>    
    Task<List<LogPurchaseRank>> FetchSellItemDescLogsAsync(DateTime startDate, DateTime endDate);

    Task<List<LogPurchaseRank>> FetchSellItemCountAsync(int itemSn, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 아이템 획득로그
    /// </summary>
    
    Task<List<LogItemDistribution>> FetchItemAwardLogsAsync(string userSeq, DateTime startDate, DateTime endDate);
    Task<List<LogCurrencyChange>> FetchCurrencyLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 로그인 로그
    /// </summary>    
    Task<List<LogLogin>> FetchLoginLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    Task<List<LogMemberDailyStamp>> FetchDailyStampLogsAsync(string userSeq, DateTime startDate, DateTime endDate);


}
