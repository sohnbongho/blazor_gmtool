/// <summary>
/// 여기서는 DB 컬럼과 동일한 변수명으로 만든다.
/// </summary>
namespace Library.DBTables.MySql
{
    public class TblServerList
    {
        public int server_id { get; set; }
        public int world_id { get; set; }
        public short server_type { get; set; }
        public string server_name { get; set; } = string.Empty;
        public string ipaddr { get; set; } = string.Empty;
        public int port { get; set; }
        public int op_port { get; set; }
        public string dir { get; set; } = string.Empty;
        public string remote_actor_path { get; set; } = string.Empty;
        public int level_min { get; set; }
        public int level_max { get; set; }
        public string https_host { get; set; } = string.Empty;
        public string parameter { get; set; } = string.Empty;

    }
    public class TblUser
    {
        public long seq { get; set; }
        public long user_uid { get; set; }
        public string user_id { get; set; } = string.Empty;
        public int level { get; set; }
        public TblUser Clone()
        {
            return new TblUser
            {
                seq = seq,
                user_uid = user_uid,
                user_id = user_id,
                level = level,
            };
        }
    }
    public class TblMember
    {
        public ulong user_seq { get; set; }
        public ulong char_seq { get; set; }
        public short login_type { get; set; }
        public string user_id { get; set; } = string.Empty;
        public string user_passwd { get; set; } = string.Empty;
        public string session_guid { get; set; } = string.Empty;
        public string user_handle { get; set; } = string.Empty;
        public string firebase_uid { get; set; } = string.Empty;
        public string image_url { get; set; } = string.Empty;
        public string background_image_url { get; set; } = string.Empty;
        public string profile_json_data { get; set; } = string.Empty;
        public DateTime? ban_expiry_date { get; set; } = null!;
        public DateTime user_handle_updated_date { get; set; }
        public DateTime? deactive_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblInventoryCurrency
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public ulong char_seq { get; set; }
        public short currency_type { get; set; }
        public long amount { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }


    public class TblCharacter
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public short world_id { get; set; }
        public int map_id { get; set; }
        public string user_id { get; set; } = string.Empty;
        public short gender { get; set; }

        public string nickname { get; set; } = string.Empty;
        public string alias { get; set; } = string.Empty;
        public ulong exp { get; set; }
        public short level { get; set; }

        public int byClass { get; set; }

        public int birthday_year { get; set; }
        public int birthday_date { get; set; }

        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }
        public DateTime deleted_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }
    public class TblMemberDeactive
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public ulong char_seq { get; set; }
        public short login_type { get; set; } // '1:구글, 2: 페이스북, 3: 애플, 4: 자체 로그인',
        public string user_id { get; set; } = string.Empty;
        public string user_handle { get; set; } = string.Empty; // '유저 핸들값',

        public DateTime? deactive_date { get; set; } = null!;
        public DateTime? deleted_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogMemberDeactive
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int reason_id { get; set; }
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblMemberPushSetting
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public short maket_agreed { get; set; }
        public short quite_mode { get; set; }
        public DateTime? start_quite_date { get; set; } = null!;
        public DateTime? end_quite_date { get; set; } = null!;
        public short new_follow { get; set; }
        public short present { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;

    }


    public class TblInventoryInfo
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public int inven_size { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }

    public class TblInventoryCostume
    {
        public ulong id { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public ulong char_seq { get; set; }
        public int type { get; set; }
        public short favorites { get; set; }
        public string color { get; set; } = string.Empty;
        public string ability_value { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }
    public class TblInventoryConsumable
    {
        public ulong id { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public ulong char_seq { get; set; }
        public int type { get; set; }
        public short favorites { get; set; }
        public int count { get; set; }
        public string ability_value { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblInventoryEtc
    {
        public ulong id { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public ulong char_seq { get; set; }
        public int type { get; set; }
        public short favorites { get; set; }
        public string color { get; set; } = string.Empty;
        public string ability_value { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }


    public class TblZoneList
    {
        public int id { get; set; }
        public int server_id { get; set; }
        public int map_id { get; set; }
        public string zone_name { get; set; } = string.Empty;
        public int game_mode { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }

    }

    public class TblDesignZoneObject
    {
        public long id { get; set; }
        public int map_id { get; set; }
        public int object_id { get; set; }
        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }
        public float rotation_x { get; set; }
        public float rotation_y { get; set; }
        public float rotation_z { get; set; }
        public int is_need_reset { get; set; }
        public int state { get; set; }
    }
    public class TblZoneSnowObject
    {
        public int map_id { get; set; }
        public int object_id { get; set; }
        public float pos_x { get; set; }
        public float pos_z { get; set; }
    }



    public class TblInventoryitem
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int item_type { get; set; }
        public int item_count { get; set; }
        public short equipped { get; set; }
        public string extra_option { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblInventorySeasonitem
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int game_mode_type { get; set; }
        public short favorites { get; set; }
        public short equipped { get; set; }
        public long stored_amount { get; set; }         // 가방에서 사용하는 값으로 가지고 있는 눈의 양
        public string extra_option { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblUserAlert
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public ulong alert_seq { get; set; }
        public int type { get; set; }
        public string data { get; set; } = string.Empty;
        public short is_checked { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }

    public class TblInventorySeasonitemParts
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public int game_mode_type { get; set; }
        public string ingame_parts { get; set; } = string.Empty;
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }
    public class TblCharacterRank
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public int game_mode_type { get; set; }
        public ulong total_point { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 선물하기 메일 - tbl_mail_present
    /// </summary>
    public class TblMailPresent
    {
        public ulong id { get; set; }
        public ulong mail_seq { get; set; }
        public string? guid { get; set; } = string.Empty;
        public ulong to_user_seq { get; set; }
        public ulong from_user_seq { get; set; }
        public int type { get; set; }

        public string content { get; set; } = string.Empty;
        public short is_checked { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 룸정보
    /// </summary>
    public class TblRoomList
    {
        public int id { get; set; }
        public int server_id { get; set; }
        public int game_mode { get; set; }
        public string room_name { get; set; } = string.Empty;
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 룸정보
    /// </summary>
    public class TblUserFollow
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public ulong follow_user_seq { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }

    /// <summary>
    /// 게임모드 정보
    /// </summary>
    public class TblGamemodeInfo
    {
        public ulong id { get; set; }
        public int game_mode { get; set; }
        public ulong liked_count { get; set; }
        public ulong played_count { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 유저별 좋아요 정보
    /// </summary>

    public class TblGameModeUserInfo
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int game_mode { get; set; }
        public int liked { get; set; }
        public ulong played_count { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 인벤토리 아이템들
    /// </summary>
    public class TblInventoryCharacter
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public int amount { get; set; }
        public short equipped { get; set; }
        public int color_id { get; set; }
        public string json_data { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblInventoryClothing
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public int amount { get; set; }
        public short equipped { get; set; }
        public int color_id { get; set; }
        public string json_data { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }
    public class TblInventoryAccessory
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }

        public int amount { get; set; }
        public short equipped { get; set; }
        public int color_id { get; set; }
        public string json_data { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblInventoryBackground
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public short equipped { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblInventoryBackgroundProp
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public short equipped { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }
    public class TblInventoryShowroom
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public short equipped { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblPurchaseReceipt
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int receipt_type { get; set; }
        public string order_id { get; set; } = string.Empty;
        public string product_id { get; set; } = string.Empty;
        public short item_type { get; set; }
        public int item_sn { get; set; }
        public long amount { get; set; }
        public short received { get; set; }

        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }

    public class TblUserFavoriteItem
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int item_sn { get; set; }
        public int liked { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblShopItemInfo
    {
        public ulong id { get; set; }
        public int item_sn { get; set; }
        public long liked_count { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblInventoryModeSoftcat
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public int amount { get; set; }
        public short equipped { get; set; }
        public int color_id { get; set; }
        public string json_data { get; set; } = string.Empty;
        public DateTime expiration_date { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 아이템 구매 횟수
    /// </summary>
    public class TblUserPurchaseCount
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int item_sn { get; set; }
        public int count { get; set; } // 구매 횟수
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 유저 관련 로그
    /// </summary>
    public class TblLogMember
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }

        public ulong total_login_count { get; set; }


        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }
    public class TblLogLogin
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }

        public string remote_address { get; set; } = string.Empty;
        public short device_type { get; set; }
        public string device_uuid { get; set; } = string.Empty;
        public int login_app_store_type { get; set; }

        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogCurrency
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public short currency_type { get; set; }
        public ulong spent_amount { get; set; }
        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblLogPurchase
    {
        public ulong id { get; set; }
        public int item_sn { get; set; }
        public string? guid { get; set; } = string.Empty;
        public ulong user_seq { get; set; }
        public ulong target_user_seq { get; set; }

        public uint item_price { get; set; }
        public short price_type { get; set; } // '제화 종류(1:골드, 2:다이아)',		
        public short purchase_type { get; set; } // 1:일반구매, 2:선물하기

        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    public class TblLogPurchaseRank
    {
        public int item_sn { get; set; }
        public int sale_count { get; set; }

    }

    public class TblMemberSession
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public ulong char_seq { get; set; }
        public int connected_serverid { get; set; }
        public int connected_community_serverid { get; set; }
        public DateTime updated_date { get; set; }

    }

    /// <summary>
    /// 현재 게임중인 룸 정보
    /// </summary>
    public class TblRoomCurrentList
    {
        public ulong id { get; set; }
        public int server_id { get; set; }

        public ulong room_seq { get; set; }
        public int game_mode { get; set; }
        public int visible { get; set; }
        public string room_info { get; set; } = string.Empty;

        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }

    /// <summary>
    /// 유저별 보상
    /// </summary>
    public class TblMemberRewardStatus
    {
        public ulong id { get; set; }
        public int server_id { get; set; }
        public ulong user_seq { get; set; }
        public string game_guid { get; set; } = string.Empty;
        public int rewared_type { get; set; }
        public short currency_type { get; set; }
        public long amount { get; set; }

        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 데일리 스탬프
    /// </summary>
    public class TblMemberDailyStamp
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public short year { get; set; }
        public short month { get; set; }
        public short count { get; set; }
        public int acquired_date { get; set; }

        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 데일리 스탬프 로그
    /// </summary>
    public class TblLogMemberDailyStamp
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public short year { get; set; }
        public short month { get; set; }
        public short count { get; set; }
        public int pay_diamond { get; set; }
        public int pay_clover { get; set; }
        public int item_sn { get; set; }
        public int amount { get; set; }

        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblDesignDailyStamp
    {
        public int month { get; set; }
        public int date { get; set; }
        public int reward_sn { get; set; }
        public int reward_count { get; set; }
        public int vip { get; set; }
        public string item_name_key { get; set; } = string.Empty;
        public string item_descr_key { get; set; } = string.Empty;
    }


    /// <summary>
    /// 골드, 다이아 페스타
    /// </summary>
    public class TblMemberGolddiaFesta
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public short year { get; set; }
        public short month { get; set; }
        public short festa_type { get; set; }
        public ulong accumulated_point { get; set; }
        public int acquire_step { get; set; }

        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 공지사항(지속)
    /// </summary>
    public class TblNoticePersistent
    {
        public ulong id { get; set; }                 // 공지사항의 고유 ID
        public string title { get; set; } = string.Empty;            // 공지 제목
        public string content { get; set; } = string.Empty;          // 공지 내용        
        public DateTime? expiry_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;

    }

    /// <summary>
    /// 공지사항(즉시)
    /// </summary>
    public class TblNoticeImmediate
    {
        public ulong id { get; set; }                 // 공지사항의 고유 ID
        public int server_id { get; set; }
        public string title { get; set; } = string.Empty;            // 공지 제목
        public string content { get; set; } = string.Empty;          // 공지 내용        
        public short showed { get; set; }
        public DateTime? notice_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblNoticeEmergency
    {
        public ulong id { get; set; }

        public DateTime? checked_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 게임 플레이 타입
    /// </summary>

    public class TblLogGameplay
    {
        public ulong id { get; set; }           // 고유 ID
        public int server_id { get; set; }
        public ulong user_seq { get; set; }
        public string game_guid { get; set; } = string.Empty;
        public int game_mode { get; set; }
        public int play_type { get; set; }
        public int reward_index { get; set; }
        public long amount { get; set; }
        public string json_data { get; set; } = string.Empty;

        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 장비 블럭
    /// </summary>
    public class TblBlockedDevice
    {
        public long id { get; set; }           // 고유 ID
        public string title { get; set; } = string.Empty;
        public short device_type { get; set; }
        public string device_uuid { get; set; } = string.Empty;
        public DateTime? created_date { get; set; } = null!;

    }

    /// <summary>
    /// 채팅 밴
    /// </summary>
    public class TblBanArticleComment
    {
        public long id { get; set; }           // 고유 ID
        public ulong user_seq { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTime? ban_expiry_date { get; set; } = null!;

        public DateTime? created_date { get; set; } = null!;

    }

    /// <summary>
    /// 밴 히스토리
    /// </summary>
    public class TblLogBanHistory
    {
        public long id { get; set; }           // 고유 ID
        public ulong user_seq { get; set; }
        public short ban_type { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTime? ban_expiry_date { get; set; } = null!;

        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// GM툴 지급 로그
    /// </summary>
    public class TblLogGmGiveitem
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public ulong mail_seq { get; set; }
        public string? guid { get; set; } = string.Empty;
        public int item_sn { get; set; }
        public int amount { get; set; }
        public string content { get; set; } = string.Empty;

        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 아이템 지급 로그
    /// </summary>
    public class TblLogItemDistribution
    {
        public ulong id { get; set; }
        public int server_id { get; set; }
        public ulong user_seq { get; set; }
        public string? guid { get; set; } = string.Empty;
        public int reason_code { get; set; }
        public int item_sn { get; set; }
        public long amount { get; set; }
        public string json_data { get; set; } = string.Empty;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 쑥쑥 클로버
    /// </summary>
    public class TblMemberGrowClover
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }

        public int total_leaf_count { get; set; }

        public int free_leaf_date { get; set; }
        public int free_leaf_count { get; set; }
        public int ad_leaf_date { get; set; }
        public int ad_leaf_count { get; set; }

        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// 기능 OnOff
    /// </summary>
    public class TblFeatureControl
    {
        public ulong id { get; set; }
        public int control_type { get; set; }
        public int blocked { get; set; }

        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 게임 모드별 경험치
    /// </summary>
    public class TblDesignGamestageLevelExp
    {
        public int level { get; set; }
        public int game_mode_id { get; set; }
        public long level_exp_required { get; set; }
    }

    public class TblDesignExpType
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public int game_mode_id { get; set; }
    }

    public class TblInventoryGamemodeExp
    {
        public long id { get; set; }
        public ulong user_seq { get; set; }
        public ulong char_seq { get; set; }
        public int game_mode_id { get; set; }
        public int level { get; set; }
        public long exp { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 시즌 패스
    /// </summary>
    public class TblInventorySeasonpassExp
    {
        public long id { get; set; }
        public ulong user_seq { get; set; }
        public int season_id { get; set; }
        public int level { get; set; }
        public long exp { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 시즌 패스 보상 정보
    /// </summary>
    public class TblRewardsSeasonpass
    {
        public long id { get; set; }
        public ulong user_seq { get; set; }
        public int season_id { get; set; }
        public short premium { get; set; }
        public int level { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    /// <summary>
    /// 서버 동접자 수
    /// </summary>
    public class TblLogConcurrentUser
    {
        public long id { get; set; }
        public int server_id { get; set; }
        public int count { get; set; }
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogConnectedTime
    {
        public ulong id { get; set; }           // 고유 ID
        public int server_id { get; set; }
        public int game_mode { get; set; }
        public ulong user_seq { get; set; }
        public ulong duration_sec { get; set; }
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogCurrencyChange
    {
        public ulong id { get; set; }           // 고유 ID
        public ulong user_seq { get; set; }
        public int item_gain_reason { get; set; }
        public short currency_type { get; set; }
        public long added_amount { get; set; }
        public long amount { get; set; }
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogPurchaseReceipt
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int receipt_type { get; set; }
        public string order_id { get; set; } = string.Empty;
        public string product_id { get; set; } = string.Empty;
        public string currency_code { get; set; } = string.Empty;
        public decimal purchase_price { get; set; }

        public DateTime updated_date { get; set; }
        public DateTime created_date { get; set; }

    }

    public class TblSeasonResetSchedule
    {
        public ulong id { get; set; }
        public int season_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string desc { get; set; } = string.Empty;
        public short is_executed { get; set; }

        public DateTime? started_date { get; set; } = null!;
        public DateTime? reset_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblInventoryPosition
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public int game_mode { get; set; }
        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }

        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblNoticeBoard
    {
        public ulong id { get; set; }
        public ulong notice_seq { get; set; }
        public short board_type { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTime? expiry_date { get; set; } = null!;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblNoticeBoardContent
    {
        public ulong id { get; set; }
        public ulong notice_seq { get; set; }
        public string title { get; set; } = string.Empty;
        public string language_code { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblOceanupUserStatus
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public int diving_count { get; set; }
        public int max_floor_reached { get; set; }
        public long reached_second { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblEventList
    {
        public ulong id { get; set; }
        public short event_type { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTime? start_date { get; set; } = null!;
        public DateTime? end_date { get; set; } = null!;
        public short enable { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogDeleteArticle
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public string article_id { get; set; } = string.Empty;
        public short deleted_type { get; set; }
        public short is_all_deleted { get; set; }

        public DateTime? created_date { get; set; } = null!;
    }

    public class TblLogReportMail
    {
        public ulong id { get; set; }
        public int report_date { get; set; }
        public DateTime? created_date { get; set; } = null!;
    }

    public class TblGamemodeUserAdViewToday
    {
        public ulong id { get; set; }
        public ulong user_seq { get; set; }
        public int game_mode { get; set; }
        public int ad_view_count { get; set; }
        public int last_update_date { get; set; }
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }
    
    public class TblGamemodeRankHistory
    {
        public ulong id { get; set; }
        public ulong char_seq { get; set; }
        public int game_mode { get; set; }        
        public string video_url { get; set; } = string.Empty;
        public DateTime? updated_date { get; set; } = null!;
        public DateTime? created_date { get; set; } = null!;
    }
}
