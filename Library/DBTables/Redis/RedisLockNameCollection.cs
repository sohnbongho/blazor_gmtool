namespace Library.DBTables;

public static class RedisLockNameCollection
{
    public readonly static string Member = "tbl_member";
    public readonly static string Character = "tbl_character";
    public readonly static string[] MemberAndCharacter = { "tbl_member", "tbl_character" };

    public readonly static string[] Inven = { "tbl_member", "tbl_inventory_character", "tbl_inventory_clothing", "tbl_inventory_accessory" };
    public readonly static string[] LikedItem = { "tbl_user_favorite_item", "tbl_shop_item_info" };

    public readonly static string[] GameModes = { "tbl_gamemode_info", "tbl_gamemode_user_info" };
    public readonly static string GameMode = "tbl_gamemode_info";
    public readonly static string GameModeExp = "tbl_inventory_gamemode_exp";
    public readonly static string[] GameModePosition = { "tbl_inventory_position" };

    public readonly static string Article = "article";
    public readonly static string ArticleLiked = "articleLiked";

    public readonly static string UserFollow = "tbl_user_follow";

    public readonly static string[] MailPresent = { "tbl_mail_present" };
    public readonly static string[] ShopItem = { "tbl_shop_item_info" };

    public readonly static string[] SeasonInven = { "tbl_inventory_seasonitem", "tbl_inventory_seasonitem_parts" };
    public readonly static string[] BuySeasonItem = { "tbl_inventory_seasonitem", "tbl_inventory_seasonitem_parts", "tbl_member" };

    public readonly static string PushAlarmSetting = "tbl_member_push_setting";
    public readonly static string[] DailyStamp = { "tbl_member_daily_stamp" };
    public readonly static string[] GoldDiaFesta = { "tbl_member", "tbl_member_golddia_festa" };

    public readonly static string[] Chat = { "chatRoom", "chatMessage", "chatRoomForUser" };
    public readonly static string ArticleLikedRank = "article";

    public readonly static string[] GrowClover = { "tbl_member_grow_clover" };

    public readonly static string[] MindAnalyzerUseClover = { "tbl_member_grow_clover", "tbl_inventory_currency" };

    public readonly static string[] ShopSteam = { "tbl_member", "tbl_stream_receipt" };

    public readonly static string[] OceanUpUserStatus = { "tbl_oceanup_user_status" };

    public readonly static string[] AdvisorView = { "tbl_log_advisor_view" };

    public readonly static string[] GamemodeUserAdViewToday = { "tbl_gamemode_user_ad_view_today" };

}
