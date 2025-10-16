using AdminTool.Repositories;
using AdminTool.Repositories.Account;
using AdminTool.Repositories.ConcurrentUser;
using AdminTool.Repositories.Device;
using AdminTool.Repositories.Event;
using AdminTool.Repositories.GameStatus;
using AdminTool.Repositories.Inventory;
using AdminTool.Repositories.Notice;
using AdminTool.Repositories.Rank;
using AdminTool.Repositories.Season;
using AdminTool.Repositories.User;
using AdminTool.Services;
using AdminTool.Services.Account;
using AdminTool.Services.Ai;
using AdminTool.Services.ConcurrentUser;
using AdminTool.Services.Device;
using AdminTool.Services.Event;
using AdminTool.Services.GameStatus;
using AdminTool.Services.Inventory;
using AdminTool.Services.Notice;
using AdminTool.Services.Rank;
using AdminTool.Services.Season;
using AdminTool.Services.Translation;
using AdminTool.Services.User;
using Library.Component;
using Library.DBTables.MySql;
using Library.Helper;

namespace AdminTool;

/// <summary>
/// Controller와 Service(비지니스 파트)를 별도로 관리
/// </summary>
public static class ServiceCollectionExtensions
{
    //서비스 수명(Scope) 옵션
    //AddScoped: 요청마다 서비스의 새 인스턴스를 생성합니다.하나의 HTTP 요청 내에서는 동일한 인스턴스가 사용됩니다.
    //AddTransient: 서비스를 요청할 때마다 새로운 인스턴스를 생성합니다.이는 매우 짧은 수명의 서비스에 적합합니다.
    //AddSingleton: 애플리케이션 수명 동안 단 하나의 인스턴스만 생성하고 모든 요청에서 이 인스턴스를 공유합니다.

    /// <summary>
    /// Controller에서 사용하는 Service(비지니스 로직)들
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        var configService = ConfigService.Instance;
        services.AddSingleton<ConfigService>(configService);
        if (false == configService.Load())
        {
            throw new Exception("failed config Load");
        }        

        // 기획 데이터 저장소
        services.AddSingleton<IDataRepository<TblDesignItemAccessories>, DesignDataRepository<TblDesignItemAccessories>>(provider
            => new DesignDataRepository<TblDesignItemAccessories>("tbl_design_item_accessories"));

        services.AddSingleton<IDataRepository<TblDesignItemCharacter>, DesignDataRepository<TblDesignItemCharacter>>(provider
            => new DesignDataRepository<TblDesignItemCharacter>("tbl_design_item_character"));

        services.AddSingleton<IDataRepository<TblDesignItemClothing>, DesignDataRepository<TblDesignItemClothing>>(provider
            => new DesignDataRepository<TblDesignItemClothing>("tbl_design_item_clothing"));

        services.AddSingleton<IDataRepository<TblDesignItemColorPalette>, DesignDataRepository<TblDesignItemColorPalette>>(provider
            => new DesignDataRepository<TblDesignItemColorPalette>("tbl_design_item_color_palette"));

        services.AddSingleton<IDataRepository<TblDesignUnequippedSetItem>, DesignDataRepository<TblDesignUnequippedSetItem>>(provider
            => new DesignDataRepository<TblDesignUnequippedSetItem>("tbl_design_unequipped_set_item"));

        services.AddSingleton<IDataRepository<TblDesignSlotToItemType>, DesignDataRepository<TblDesignSlotToItemType>>(provider
            => new DesignDataRepository<TblDesignSlotToItemType>("tbl_design_slot_to_item_type"));

        services.AddSingleton<IDataRepository<TblDesignCurrency>, DesignDataRepository<TblDesignCurrency>>(provider
            => new DesignDataRepository<TblDesignCurrency>("tbl_design_currency"));

        services.AddSingleton<IDataRepository<TblDesignDiggingWarrior>, DesignDataRepository<TblDesignDiggingWarrior>>(provider
            => new DesignDataRepository<TblDesignDiggingWarrior>("tbl_design_digging_warrior"));

        services.AddSingleton<IDataRepository<TblDesignInAppCurrency>, DesignDataRepository<TblDesignInAppCurrency>>(provider
            => new DesignDataRepository<TblDesignInAppCurrency>("tbl_design_in_app_currency"));

        services.AddSingleton<IDataRepository<TblDesignPackage>, DesignDataRepository<TblDesignPackage>>(provider
            => new DesignDataRepository<TblDesignPackage>("tbl_design_package"));

        services.AddSingleton<IDataRepository<TblDesignMiniGame>, DesignDataRepository<TblDesignMiniGame>>(provider
            => new DesignDataRepository<TblDesignMiniGame>("tbl_design_mini_game"));

        // 배경
        services.AddSingleton<IDataRepository<TblDesignSpaceBg>, DesignDataRepository<TblDesignSpaceBg>>(provider
            => new DesignDataRepository<TblDesignSpaceBg>("tbl_design_space_bg"));
        services.AddSingleton<IDataRepository<TblDesignSpaceProp>, DesignDataRepository<TblDesignSpaceProp>>(provider
            => new DesignDataRepository<TblDesignSpaceProp>("tbl_design_space_prop"));

        // 말랑캣
        services.AddSingleton<IDataRepository<TblDesignMallangCatItem>, DesignDataRepository<TblDesignMallangCatItem>>(provider
            => new DesignDataRepository<TblDesignMallangCatItem>("tbl_design_mallang_cat_item"));

        services.AddSingleton<IDataRepository<TblDesignMallangCatReward>, DesignDataRepository<TblDesignMallangCatReward>>(provider
            => new DesignDataRepository<TblDesignMallangCatReward>("tbl_design_mallang_cat_reward"));

        services.AddSingleton<IDataRepository<TblDesignMallangCatType>, DesignDataRepository<TblDesignMallangCatType>>(provider
            => new DesignDataRepository<TblDesignMallangCatType>("tbl_design_mallang_cat_type"));

        services.AddSingleton<IDataRepository<TblDesignMallangCatSpawnPoint>, DesignDataRepository<TblDesignMallangCatSpawnPoint>>(provider
            => new DesignDataRepository<TblDesignMallangCatSpawnPoint>("tbl_design_mallang_cat_spawn_point"));

        services.AddSingleton<IDataRepository<TblDesignMallangCatRewardPoint>, DesignDataRepository<TblDesignMallangCatRewardPoint>>(provider
            => new DesignDataRepository<TblDesignMallangCatRewardPoint>("tbl_design_mallang_cat_reward_point"));

        // 패키지 데이터
        services.AddSingleton<IDataRepository<TblDesignItemPackagePrice>, DesignDataRepository<TblDesignItemPackagePrice>>(provider
            => new DesignDataRepository<TblDesignItemPackagePrice>("tbl_design_item_package_price"));

        // 세일 정보
        services.AddSingleton<IDataRepository<TblDesignItemSale>, DesignDataRepository<TblDesignItemSale>>(provider
            => new DesignDataRepository<TblDesignItemSale>("tbl_design_item_sale"));

        // const상수 부분 체크
        services.AddSingleton<IDataRepository<TblDesignConst>, DesignDataRepository<TblDesignConst>>(provider
            => new DesignDataRepository<TblDesignConst>("tbl_design_const"));

        // zone object 정보
        services.AddSingleton<IDataRepository<TblDesignZoneObject>, DesignDataRepository<TblDesignZoneObject>>(provider
            => new DesignDataRepository<TblDesignZoneObject>("tbl_design_zone_object_info"));

        // 데일리 스탬프
        services.AddSingleton<IDataRepository<TblDesignDailyStamp>, DesignDataRepository<TblDesignDailyStamp>>(provider
            => new DesignDataRepository<TblDesignDailyStamp>("tbl_design_daily_stamp"));

        // 게임모드 레벨 관련
        services.AddSingleton<IDataRepository<TblDesignGamestageLevelExp>, DesignDataRepository<TblDesignGamestageLevelExp>>(provider
            => new DesignDataRepository<TblDesignGamestageLevelExp>("tbl_design_gamestage_level_exp"));

        services.AddSingleton<IDataRepository<TblDesignExpType>, DesignDataRepository<TblDesignExpType>>(provider
            => new DesignDataRepository<TblDesignExpType>("tbl_design_exp_type"));

        services.AddSingleton<ExcelService>(); // Add ExcelService as a singleton        

        // 저장 소관련         
        services.AddSingleton<MySqlDbCommonRepo>();        
        services.AddSingleton<RedisCacheCommonComponent>();

        // 계정 관리
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<IAccountRepository, AccountRepository>();

        // 유저 관련 서비스                
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserService, UserService>();

        // 시즌 인벤토리 관련
        services.AddSingleton<ISeasonItemRepository, SeasonItemRepository>();
        services.AddSingleton<ISeasonItemService, SeasonItemService>();

        // 인벤토리 관련
        services.AddSingleton<IInventoryRepository, InventoryRepository>();
        services.AddSingleton<IInventoryService, InventoryService>();

        // AI 관련 서비스
        services.AddSingleton<IAiService, AiClaude3Service>();

        // 공지사항
        services.AddSingleton<INoticeManageRepository, NoticeManageRepository>();
        services.AddSingleton<INoticeManageService, NoticeManageService>();

        // 유저 Device 관련
        services.AddSingleton<IUserDeviceService, UserDeviceService>();
        services.AddSingleton<IUserDeviceRepository, UserDeviceRepository>();

        // 번역 관련
        services.AddSingleton<ITranslationService, TranslationService>();

        // 서버 동접자 수  체크 
        services.AddSingleton<IConcurrentUserRepository, ConcurrentUserRepository>();
        services.AddSingleton<IConcurrentUserService, ConcurrentUserService>();

        // 게임 지표 서비스
        services.AddSingleton<IGameStatusRepo, GameStatusRepo>();
        services.AddSingleton<IGameStatusService, GameStatusService>();

        // 시즌관련 
        services.AddSingleton<ISeasonRepo, SeasonRepo>();
        services.AddSingleton<ISeasonService, SeasonService>();

        // 공지 게시판
        services.AddSingleton<INoticeBoardService, NoticeBoardService>();
        services.AddSingleton<INoticeBoardRepository, NoticeBoardRepository>();

        // 이벤트 관련
        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IEventRepository, EventRepository>();

        // 랭크 관련
        services.AddSingleton<IRankService, RankService>();
        services.AddSingleton<IRankRepository, RankRepository>();

        return services;
    }
}