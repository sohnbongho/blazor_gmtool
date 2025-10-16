using AdminTool.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Library.DBTables;
using Library.DBTables.MySql;
using Library.DTO;

namespace AdminTool.Repositories.User;

public interface IUserRepository
{
    /// <summary>
    /// 유저 정보
    /// </summary>    
    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserName(string name);
    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserId(string id);
    Task<TblMember> FetchMemberAsync(ulong userSeq);
    Task<(bool, TblMemberSession)> FetchUserSessionInfoByUserSeqAsync(ulong userSeq);

    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByUserSeq(string userSeq);
    Task<List<(TblMember, TblCharacter, TblLogMember)>> FetchUserByCharSeq(string charSeq);

    /// <summary>
    /// 구매 로그
    /// </summary>    
    Task<List<TblLogPurchase>> FetchPurchaseLogsAsync(string userSeq, DateTime startDate, DateTime endDate);
    Task<List<TblPurchaseReceipt>> FetchPurchaseRecepitAsync(string userSeq, DateTime startDate, DateTime endDate);
    Task<List<LogConnectedTime>> FetchPlayTimeLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    Task<List<TblInventoryCurrency>> FetchInventoryCurrencys(string userSeq);
    Task<List<TblInventoryGamemodeExp>> FetchGameModeExps(string userSeq);
    Task<List<TblLogCurrency>> FetchLogCurrencys(string userSeq);

    Task<List<TblMemberDeactive>> FetchDeactiveAccount(string userSeq);
    Task<List<TblLogMemberDeactive>> FetchDeactiveAccountLogs(string userSeq);

    /// <summary>
    /// 유저 밴
    /// </summary>    
    Task<bool> BanUser(string userSeqStr, DateTime banExpiryDate);


    Task<bool> BanArticleCommentAsync(ulong userSeq, string title, DateTime banExpiryDate);

    /// <summary>
    /// 게임 보상 로그
    /// </summary>    
    Task<List<TblLogGameplay>> FetchGamePlayLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    // Community 서버 정보들
    Task<List<TblServerList>> FetchCommunityServerInfosAsync();

    Task<bool> AddLogBanAsync(ulong userSeq, UserBanType userBanType, string title, DateTime banExpiryDate);
    Task<List<TblLogBanHistory>> FetchUserBanHistoryAsync(string userSeq);

    /// <summary>
    /// 아이템 지급
    /// </summary>    
    Task<ErrorCode> GiftAppearanceItemAsync(ulong targetUserSeq, int itemSn, int amount, string text);
    Task<List<TblLogGmGiveitem>> FetchGmGiftLogsAsync(string userSeq, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 로그 관련
    /// </summary>    
    Task<List<TblLogPurchaseRank>> FetchSellItemDescLogsAsync(DateTime startDate, DateTime endDate);
    Task<int> FetchSellItemCountAsync(int itemSn, DateTime startDate, DateTime endDate);


    Task<List<TblLogItemDistribution>> FetchItemAwardLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate);
    Task<List<TblLogCurrencyChange>> FetchCurrencyLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// 로그인 로그
    /// </summary>    
    Task<List<TblLogLogin>> FetchLoginLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 데일리스탬프 로그
    /// </summary>    
    Task<List<TblLogMemberDailyStamp>> FetchDailyStampLogsAsync(ulong userSeq, DateTime startDate, DateTime endDate);

}
