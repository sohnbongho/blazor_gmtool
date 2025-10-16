using Library.DTO;
using Library.messages;

namespace AdminTool.Models;

public class UserMember
{
    public ulong UserSeq { get; set; }
    public ulong CharSeq { get; set; }
    public LoginType LoginType { get; set; } = LoginType.None;
    public string UserId { get; set; } = string.Empty;
    public string SessionGuid { get; set; } = string.Empty;
    public string UserHandle { get; set; } = string.Empty;
    public string FirebaseUid { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string BackgroundImageUrl { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
    public DateTime? BanExpiryDate { get; set; } = null!;
    public DateTime UserHandleUpdatedDate { get; set; }
    public DateTime? DeactiveDate { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}
public class LogGameplay
{
    public ulong Id { get; set; }           // 고유 ID
    public int ServerId { get; set; }
    public ulong UserSeq { get; set; }
    public string GameGuid { get; set; } = string.Empty;
    public GameModeType GameMode { get; set; } = GameModeType.None;
    public PlayType PlayType { get; set; } = PlayType.None;
    public string ItemName { get; set; } = string.Empty;
    public long Amount { get; set; }
    public DateTime? CreatedDate { get; set; } = null!;
    public string JsonData { get; set; } = string.Empty;
}

public class InventoryData
{
    public ulong Id { get; set; }
    public ulong CharSeq { get; set; }
    public ulong ItemSeq { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int SlotId { get; set; }
    public int Amount { get; set; }
    public short Equipped { get; set; }
    public int ColorId { get; set; }
    public string JsonData { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class BackgroundData
{
    public ulong Id { get; set; }
    public ulong CharSeq { get; set; }
    public ulong ItemSeq { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int SlotId { get; set; }
    public short Equipped { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class InventoryShowRoom
{
    public ulong Id { get; set; }
    public ulong CharSeq { get; set; }
    public ulong ItemSeq { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int SlotId { get; set; }
    public short Equipped { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class LogPurchase
{
    public ulong Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public ulong UserSeq { get; set; }
    public ulong TargetUserSeq { get; set; }

    public uint ItemPrice { get; set; }
    public CurrencyType CurrencyType { get; set; } = CurrencyType.None;// '제화 종류(1:골드, 2:다이아)',		
    public PurchaseType PurchaseType { get; set; } = PurchaseType.None;// 1:일반구매, 2:선물하기

    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class LogPurchaseRank
{
    public string ItemName { get; set; } = string.Empty;
    public CurrencyType PriceType { get; set; } = CurrencyType.None;
    public uint ItemPrice { get; set; } = 0;
    public int SaleCount { get; set; }

}

public class InventorySeasonitem
{
    public ulong Id { get; set; }
    public ulong CharSeq { get; set; }
    public ulong ItemSeq { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public GameModeType GameModeType { get; set; } = GameModeType.None;
    public short Favorites { get; set; }
    public short Equipped { get; set; }
    public long StoredAmount { get; set; }         // 가방에서 사용하는 값으로 가지고 있는 눈의 양
    public string ExtraOption { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class InventorySeasonitemParts
{
    public ulong Id { get; set; }
    public ulong CharSeq { get; set; }
    public GameModeType GameModeType { get; set; } = GameModeType.None;
    public string IngameParts { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class InventoryModeSoftcat
{
    public ulong Id { get; set; }
    public ulong CharSeq { get; set; }
    public ulong ItemSeq { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int SlotId { get; set; }
    public int Amount { get; set; }
    public short Equipped { get; set; }
    public int ColorId { get; set; }
    public string JsonData { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class BlockedDevice
{
    public long Id { get; set; }           // 고유 ID
    public string Title { get; set; } = string.Empty;
    public UserDeviceType UserDeviceType { get; set; } = UserDeviceType.None;
    public string DeviceUUid { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; } = null!;

}

public class LogBanHistory
{
    public long Id { get; set; }           // 고유 ID
    public ulong UserSeq { get; set; }
    public UserBanType UserBanType { get; set; } = UserBanType.None;
    public string Title { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;

}

public class LogGmGiveitem
{
    public ulong Id { get; set; }
    public ulong UserSeq { get; set; }
    public ulong MailSeq { get; set; }
    public string? MailGuid { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Content { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; } = null!;
}

/// <summary>
/// 아이템 지급 로그
/// </summary>
public class LogItemDistribution
{
    public ulong Id { get; set; }
    public int ServerId { get; set; }
    public ulong UserSeq { get; set; }
    public string? Guid { get; set; } = string.Empty;
    public ItemGainReason ReasonCode { get; set; } = ItemGainReason.None;
    public string ItemName { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string JsonData { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; } = null!;
}

public class LogCurrencyChange
{
    public ulong Id { get; set; }           // 고유 ID
    public ulong UserSeq { get; set; }
    public ItemGainReason ItemGainReason { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public long AddedAmount { get; set; }
    public long Amount { get; set; }
    public DateTime? CreatedDate { get; set; } = null!;
}

/// <summary>
/// 데일리 스탬프 로그
/// </summary>
public class LogMemberDailyStamp
{
    public ulong Id { get; set; }
    public ulong UserSeq { get; set; }
    public short Year { get; set; }
    public short Month { get; set; }
    public short Count { get; set; }
    public int PayDiamond { get; set; }
    public int PayClover { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Amount { get; set; }

    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}
public class MailPresent
{
    public ulong Id { get; set; }
    public ulong MailSeq { get; set; }
    public string? Guid { get; set; } = string.Empty;
    public ulong ToUserSeq { get; set; }
    public ulong FromUserSeq { get; set; }
    public MailType Type { get; set; } = MailType.None;

    public string Content { get; set; } = string.Empty;
    public short IsChecked { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class NoticeImmediate
{
    public ulong Id { get; set; }                 // 공지사항의 고유 ID
    public string ServerName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;            // 공지 제목
    public string Content { get; set; } = string.Empty;          // 공지 내용        
    public short Showed { get; set; }
    public DateTime? NoticeDate { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}

/// <summary>
/// 긴급 공지
/// </summary>
public class NoticeEmergency
{
    public ulong Id { get; set; }
    public DateTime? CheckedDate { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}

/// <summary>
/// 로그인 로그
/// </summary>
public class LogLogin
{
    public ulong Id { get; set; }
    public ulong UserSeq { get; set; }
    public string RemoteAddress { get; set; } = string.Empty;
    public UserDeviceType UserDeviceType { get; set; } = UserDeviceType.None;
    public string DeviceUuid { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; } = null!;
}

public class FeatureControl
{
    public ulong Id { get; set; }
    public FeatureControlType FeatureControlType { get; set; }
    public bool Blocked { get; set; }

    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}

public class LogConcurrentUser
{
    public long Id { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public DateTime? CreatedDate { get; set; } = null!;
}

public class ConcurrentUserItem
{
    public long ServerId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public int UserCount { get; set; }
}

public class PurchaseReceipt
{
    public ulong Id { get; set; }
    public ulong UserSeq { get; set; }
    public ReceiptType ReceiptType { get; set; } = ReceiptType.None;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public MainItemType ItemType { get; set; } = MainItemType.None;
    public int ItemSn { get; set; }
    public long Amount { get; set; }
    public short Received { get; set; }

    public DateTime? UpdatedDate { get; set; } = null;
    public DateTime? CreatedDate { get; set; } = null;
}

public class LogConnectedTime
{
    public ulong Id { get; set; }           // 고유 ID
    public string ServerName { get; set; } = string.Empty;
    public GameModeType GameModeType { get; set; } = GameModeType.None;
    public ulong UserSeq { get; set; }
    public ulong DurationSecond { get; set; }
    public DateTime? CreatedDate { get; set; } = null!;
}


public class TblChekcedCount
{
    public int Count { get; set; }

}


public class LogPurchaseReceipt
{
    public ulong Id { get; set; }
    public ulong UserSeq { get; set; }
    public ReceiptType ReceiptType { get; set; } = ReceiptType.None;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }

    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;

}

public class SeasonResetSchedule
{
    public ulong Id { get; set; }
    public int SeasonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public short IsExecuted { get; set; }

    public DateTime? StartedDate { get; set; } = null!;
    public DateTime? ResetDate { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}

public class NoticeBoardContentData
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

}

public class NoticeBoardData
{
    public ulong Id { get; set; }                 // 공지사항의 고유 ID
    public ulong NoticeSeq { get; set; }                 // 공지사항의 고유 ID
    public NoticeBoardType NoticeBoardType { get; set; }
    public string Title { get; set; } = string.Empty;            // 공지 제목
    public Dictionary<string, NoticeBoardContentData> Contents { get; set; } = new Dictionary<string, NoticeBoardContentData>();          // 공지 내용        
    public DateTime? ExpiryDate { get; set; } = null!;
    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}


public class EventList
{
    public ulong Id { get; set; }                 // 공지사항의 고유 ID
    public EventType EventType { get; set; } = EventType.None;                // 공지사항의 고유 ID    
    public string Title { get; set; } = string.Empty;            // 공지 제목    
    public DateTime? StartDate { get; set; } = null!;
    public DateTime? EndDate { get; set; } = null!;
    public bool Enable { get; set; }
    public DateTime? UpdatedDate { get; set; } = null!;
    public DateTime? CreatedDate { get; set; } = null!;
}

public class GameModeRankInfo
{
    public int Rank { get; set; }
    public GameModeType GameModeType { get; set; } = GameModeType.None;                // 공지사항의 고유 ID
    public string NickName { get; set; } = string.Empty;
    public ulong CharSeq { get; set; }
    public long Score { get; set; }
}



