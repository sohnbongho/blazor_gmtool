///////////////////////// 기획팀 DB쿼리
namespace Library.DBTables.MySql
{
    /// <summary>
    /// 삽투사 모드 정보
    /// </summary>
    public class TblDesignDiggingWarrior
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public int in_game_item_type { get; set; }
        public uint item_price { get; set; }
        public short price_type { get; set; }

        public int required_item_grade1 { get; set; }
        public int required_item_count1 { get; set; }
        public int required_item_grade2 { get; set; }
        public int required_item_count2 { get; set; }

        public string icon_route { get; set; } = string.Empty;
        public string resource_route { get; set; } = string.Empty;
        public string item_type_key { get; set; } = string.Empty;
        public string item_name_key { get; set; } = string.Empty;
        public string item_des_key { get; set; } = string.Empty;

        public int bag_capacity { get; set; } // 가방 용량
        public int coverage_area { get; set; } // 눈을 파는 범위
        public int digging_count { get; set; } // 눈을 한번 팔때 파지는 횟수
        public float digging_duration { get; set; } // 눈을 한번 팔때 시간
        public float vehicle_speed { get; set; } // 탈것 속도
        public int item_grade { get; set; } // 아이템 등급
        public int item_avail { get; set; } // 0 = 구매 불가능 1=구매 가능
    }

    /// <summary>
    /// 게임 모드 추가 보상
    /// </summary>
    public class TblDesignMiniGame
    {
        public int game_mode_id { get; set; }
        public string id_memo { get; set; } = string.Empty;
        public long easy_final_score { get; set; }
        public long easy_bonus { get; set; }
        public long normal_final_score { get; set; }
        public long normal_bonus { get; set; }
        public long hard_final_score { get; set; }
        public long hard_bonus { get; set; }
        public long extreme_final_score { get; set; }
        public long extreme_bonus { get; set; }

    }

    /// <summary>
    /// 아이템 관련 
    /// </summary>
    public class TblDesignItemAccessories
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public string item_icon_path { get; set; } = string.Empty;
        public uint item_price { get; set; }
        public short price_type { get; set; }

        public int item_period { get; set; }
        public short sell_type { get; set; }
        public short purchase_avail { get; set; }
        public int purchase_count { get; set; }

        public string mesh_name { get; set; } = string.Empty;
        public string texture_name { get; set; } = string.Empty;
        public string acc_dummy { get; set; } = string.Empty;

        public short color_avail { get; set; }
        public string color_default { get; set; } = string.Empty;
        public int color_id { get; set; }

        public short anim_type { get; set; }
        public int slot_id { get; set; }
        public short detail_category { get; set; }
        public short filter_color { get; set; }
        public string search_tag1 { get; set; } = string.Empty;
        public string search_tag2 { get; set; } = string.Empty;
        public float shoe_heel { get; set; }
    }

    public class TblDesignItemCharacter
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public string item_icon_path { get; set; } = string.Empty;
        public uint item_price { get; set; }
        public short price_type { get; set; }

        public int item_period { get; set; }
        public short sell_type { get; set; }
        public short purchase_avail { get; set; }
        public int purchase_count { get; set; }

        public string mesh_name { get; set; } = string.Empty;
        public string texture_name { get; set; } = string.Empty;
        public string acc_dummy { get; set; } = string.Empty;

        public short color_avail { get; set; }
        public string color_default { get; set; } = string.Empty;
        public int color_id { get; set; }

        public short anim_type { get; set; }
        public int slot_id { get; set; }
        public short detail_category { get; set; }
        public short filter_color { get; set; }
        public string search_tag1 { get; set; } = string.Empty;
        public string search_tag2 { get; set; } = string.Empty;
        public float shoe_heel { get; set; }
    }

    public class TblDesignItemClothing
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public string item_icon_path { get; set; } = string.Empty;
        public uint item_price { get; set; }
        public short price_type { get; set; }

        public int item_period { get; set; }
        public short sell_type { get; set; }
        public short purchase_avail { get; set; }
        public int purchase_count { get; set; }

        public string mesh_name { get; set; } = string.Empty;
        public string texture_name { get; set; } = string.Empty;
        public string acc_dummy { get; set; } = string.Empty;

        public short color_avail { get; set; }
        public string color_default { get; set; } = string.Empty;
        public int color_id { get; set; }

        public short anim_type { get; set; }
        public int slot_id { get; set; }
        public short detail_category { get; set; }
        public short filter_color { get; set; }
        public string search_tag1 { get; set; } = string.Empty;
        public string search_tag2 { get; set; } = string.Empty;
        public float shoe_heel { get; set; }
    }

    /// <summary>
    /// 컬러 정보
    /// </summary>
    public class TblDesignItemColorPalette
    {
        public int item_sn { get; set; }
        public string color_hex { get; set; } = string.Empty;
        public uint color_price { get; set; }
        public short price_type { get; set; }
        public short purchase_avail { get; set; }
        public int skin_color { get; set; } // 피부색
    }

    /// <summary>
    /// 제화 아이템
    /// </summary>
    public class TblDesignCurrency
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
    }

    public class TblDesignInAppCurrency
    {
        public string product_id { get; set; } = string.Empty;
        public int store_type { get; set; }
        public int group_id { get; set; }
        public string plan_name_key { get; set; } = string.Empty;
        public int goods { get; set; }
        public int bonus { get; set; }
        public int total { get; set; }
        public string icon_path { get; set; } = string.Empty;
        public int price_won { get; set; }
        public float price_dollar { get; set; }
    }
    public class TblDesignPackage
    {
        public int group_id { get; set; }
        public int item_sn { get; set; }
        public int amount { get; set; }
    }

    /// <summary>
    /// 아이템 탈착 정보
    /// </summary>
    public class TblDesignUnequippedSetItem
    {
        public int slot_id { get; set; }
        public int unequip_slot_id { get; set; }
    }

    /// <summary>
    /// 캐릭터 생성시, 초기 지급 정보
    /// </summary>
    public class TblDesignInitialProvide
    {
        public int item_sn { get; set; }
        public int amount { get; set; }
    }

    /// <summary>
    /// slot별 아이템 타입 정보
    /// </summary>
    public class TblDesignSlotToItemType
    {
        public int slot_id { get; set; }
        public int item_type { get; set; }
    }

    /// <summary>
    /// 배경 아이템
    /// </summary>
    public class TblDesignSpaceBg
    {
        public int item_sn { get; set; }
        public int use_avail { get; set; }
        public short purchase_avail { get; set; }
        public uint item_price { get; set; }
        public short price_type { get; set; }
        public string item_texture_path { get; set; } = string.Empty;
        public string item_icon_path { get; set; } = string.Empty;
        public short slot_id { get; set; }
        public short category { get; set; }
    }

    /// <summary>
    /// 배경 아이템 스티커
    /// </summary>
    public class TblDesignSpaceProp
    {
        public int item_sn { get; set; }
        public int use_avail { get; set; }
        public short purchase_avail { get; set; }
        public uint item_price { get; set; }
        public short price_type { get; set; }
        public string item_texture_path { get; set; } = string.Empty;
        public string item_icon_path { get; set; } = string.Empty;
        public short slot_id { get; set; }
        public short category { get; set; }

        public float px { get; set; }
        public float py { get; set; }
        public float scale { get; set; }

        public int flip_x { get; set; }
        public int flip_y { get; set; }

    }

    /// <summary>
    /// 말랑 캣
    /// </summary>
    public class TblDesignMallangCatItem
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public int in_game_item_type { get; set; }
        public uint item_price { get; set; }
        public short price_type { get; set; }

        public string icon_route { get; set; } = string.Empty;
        public string resource_route { get; set; } = string.Empty;
        public string item_name_key { get; set; } = string.Empty;
        public string item_des_key { get; set; } = string.Empty;

        public int max_ride { get; set; }
        public int duration { get; set; }
        public float speed_var { get; set; }
    }

    public class TblDesignMallangCatReward
    {
        public int cat_group { get; set; }
        public short table_type { get; set; }
        public int item_sn { get; set; }
        public long amount { get; set; }
        public int rate { get; set; }
        public TblDesignMallangCatReward Clone() => new TblDesignMallangCatReward
        {
            cat_group = this.cat_group,
            table_type = this.table_type,
            item_sn = this.item_sn,
            amount = this.amount,
            rate = this.rate,
        };
    }

    public class TblDesignMallangCatType
    {
        public int object_id { get; set; }
        public short cat_type { get; set; }
        public int cat_group { get; set; }
        public string cat_mesh { get; set; } = string.Empty;

        public TblDesignMallangCatType Clone() => new TblDesignMallangCatType
        {
            object_id = this.object_id,
            cat_type = this.cat_type,
            cat_group = this.cat_group,
            cat_mesh = this.cat_mesh,
        };
    }

    /// <summary>
    /// 고양이 위치
    /// </summary>
    public class TblDesignMallangCatSpawnPoint
    {
        public int id { get; set; }
        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }

        public TblDesignMallangCatSpawnPoint Clone() => new TblDesignMallangCatSpawnPoint
        {
            id = this.id,
            pos_x = this.pos_x,
            pos_y = this.pos_y,
            pos_z = this.pos_z,
        };
    }

    /// <summary>
    /// 고양이 목적지 정보
    /// </summary>

    public class TblDesignMallangCatRewardPoint
    {
        public int id { get; set; }
        public int cat_type { get; set; }

        public TblDesignMallangCatRewardPoint Clone() => new TblDesignMallangCatRewardPoint
        {
            id = this.id,
            cat_type = this.cat_type,
        };
    }

    /// <summary>
    /// 다이아 페스트
    /// </summary>
    public class TblDesignDiaFesta
    {
        public string step { get; set; } = string.Empty;
        public int dia_use { get; set; }
        public int total_dia_use { get; set; }
        public int reward_sn { get; set; }
        public int reward_amount { get; set; }
    }

    /// <summary>
    /// 골드 페스트
    /// </summary>
    public class TblDesignGoldFesta
    {
        public string step { get; set; } = string.Empty;
        public int gold_use { get; set; }
        public int total_gold_use { get; set; }
        public int reward_sn { get; set; }
        public int reward_amount { get; set; }
    }

    /// <summary>
    /// 패키지 아이템 타입
    /// </summary>
    public class TblDesignItemPackagePrice
    {
        public int group_id { get; set; }
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public string item_icon_path { get; set; } = string.Empty;

        public uint item_price { get; set; }
        public short price_type { get; set; }
        public short sell_type { get; set; }

        public short purchase_avail { get; set; }
        public int purchase_count { get; set; }
        public int package_category { get; set; }
        public string text_key { get; set; } = string.Empty;
    }

    public class TblDesignItemSale
    {
        public int item_sn { get; set; }
        public string item_name { get; set; } = string.Empty;
        public uint item_sale_price { get; set; }
        public short price_type { get; set; }
        public int sale_category { get; set; }
    }

    public class TblDesignConst
    {
        public string key { get; set; } = string.Empty;
        public int value { get; set; }
        public string memo { get; set; } = string.Empty;

    }

    public class TblDesignShowroomPose
    {
        public int pose_id { get; set; }
        public string pose_name { get; set; } = string.Empty;
        public int pose_num { get; set; }
        public string pose_icon_path { get; set; } = string.Empty;
        public uint item_price { get; set; }
        public short price_type { get; set; }
        public short purchase_avail { get; set; }
        public int use_avail { get; set; }
        public int slot_id { get; set; }

    }

    /// <summary>
    /// 서버 봇정보
    /// </summary>
    public class TblDesignServerBotInfo
    {
        public int game_mode { get; set; }
        public int bot_count { get; set; }
    }

    /// <summary>
    /// 봇 캐릭터 정보
    /// </summary>
    public class TblDesignBotCharacter
    {
        public ulong char_seq { get; set; }
        public int game_mode { get; set; }
        public string nickname { get; set; } = string.Empty;
    }

    /// <summary>
    /// 봇의 인벤토리 처리
    /// </summary>
    public class TblDesignBotInventory
    {
        public ulong char_seq { get; set; }
        public ulong item_seq { get; set; }
        public int item_sn { get; set; }
        public int slot_id { get; set; }
        public int amount { get; set; }
        public short equipped { get; set; }
        public int color_id { get; set; }
    }

    /// <summary>
    /// 맵 동전 정보
    /// </summary>
    public class TblDesignOceanUpCoinReward
    {
        public int map_id { get; set; }
        public int object_id { get; set; }
        public int item_sn { get; set; }
        public int gold_amount { get; set; }
    }
    
    /// <summary>
    /// 아이템 박스 정보
    /// </summary>
    public class TblDesignOceanUpBoxReward
    {
        public int map_id { get; set; }
        public int object_id { get; set; }
        public int box_group { get; set; }
    }

    /// <summary>
    /// 아이템 박스 보상
    /// </summary>
    public class TblDesignOceanUpBoxRewardDetail
    {
        public int box_group { get; set; }
        public int item_sn { get; set; }
        public int amount { get; set; }
        public int rate { get; set; }

    }
}
