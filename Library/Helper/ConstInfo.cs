namespace Library.Helper
{
    public static class ConstInfo
    {
        public const int DefaultMapIndex = 0; // 0: 기본 맵

        //탈퇴 신청 후 7일 이내 로그인 시 계정 탈퇴 접수 해제
        public const int ReactiveDay = 7;

        // 기본 인벤 크기
        public const int DefaultInvenSize = 255;

        // 기본적인 데이터 값 
        public readonly static TimeSpan DefaultRedisExpireDate = TimeSpan.FromDays(30);
        // 로그인 세션 저장일 
        public readonly static TimeSpan LoginSessionRedisExpireDate = TimeSpan.FromDays(1);
        public readonly static TimeSpan CharSessionRedisExpireDate = TimeSpan.FromDays(7);
        public readonly static TimeSpan DbLockExpireDate = TimeSpan.FromSeconds(10); // DBLock관련        

        // 좋아요 관련 캐시 저장 시간
        public readonly static TimeSpan LikedArticleCacheExpireDate = TimeSpan.FromHours(1); // 캐시 저장 기간 (1시간)

        public const int DefaultBatchSize = 50; // 패킷을 한번 보낼때 갯수


        // 게임 Score에 대한 
        public const long ScorePerGold = 2000; // 1골드당 스코어

        public const long GoldPerBell = 10000; // 방울 1개당 골드

        // 눈 관련 포인트
        public const int MaxSnowPoint = 15;
        public const int RepairSnowPoint = 1;   // 10초에 회복되는 눈의 양
        public const int SnowPerGold = 10; // 1골드당 눈의양

        // 최대 입장 수
        public const int MaxZoneUserCount = 8;
        //public const int MaxJumpMapUserCount = 16;
        public const int MaxJumpMapUserCount = 8;

        // 랭킹 수
        public const int MaxRankCount = 50;

        public const int MaxRepairSnowCount = 10000;

        // 최대 지급
        public const long MaxGameMoney = 999999999;
        public const long MaxDiamond = 9999999;
        public const long MaxClover = 9999999;
        public const long MaxBell = 9999999;
        public const long MaxPremiumMoney = 999999999;
        public const long MaxPremiumDiamond = 9999999;

        public const long DefaultGameMoney = 0; // 기본 지급 game money
        public const long DefaultGameDiamond = 0; // 기본 지급 
        public const long DefaultGameClover = 0; // 기본 지급 

        // NatsQM 이름
        public readonly static string MqBroadcastTopic = $"AllServer"; // 전체 서버 
        public readonly static string MqServerSubject = $"Server"; // 특정 서버 (Server{ServerId})

        // Kafca관련 
        public readonly static string KafkaConsumerGroupId = $"consumer-group"; // 특정 서버 (Server{ServerId})

        // Zone Actor에 timeout시간
        public readonly static TimeSpan ZoneActorCheckedTime = TimeSpan.FromSeconds(30); // 눈 체크 간격

        // 눈 체크 시간
        public const int SnowRepairSecond = 10;
        public readonly static TimeSpan ZoneActorSnowCheckedTime = TimeSpan.FromSeconds(SnowRepairSecond);

        // Zone 게임모드 관련 타이머
        public readonly static TimeSpan ZoneActorStartGameTime = TimeSpan.FromSeconds(10);

        public const int ZoneBatchSize = 50; // 존에서 유저 정보 보내주는 양

        // Room Actor 시간
        public readonly static TimeSpan RoomActorCheckedTime = TimeSpan.FromSeconds(10);

        // Batch프로그램 Db정리 주기
        public readonly static TimeSpan DatabaseBatchJobTime = TimeSpan.FromMinutes(10);
        public const int BatchJobAccountDeletedDay = 180; // 180일 뒤에 계정 삭제

        // 알림 유지 기간 
        public const int AlertPeriodDay = 365; // 1년으로 변경
        public const int MaxAlertCount = 1000;

        //메일 관련
        public const int PresentMailPeriodDay = 30;
        public const int MaxMailCount = 100;

        // 아이템 최대 
        public const int DefaultItemExpireYear = 3; //최대 아이템 유지 기간
        public const int DefaultItemExpireDay = DefaultItemExpireYear * 365; //최대 아이템 유지 기간

        public const int UnlimitItemExpireYear = 200; // 무제한 아이템
        public const int UnlimitItemExpireDay = UnlimitItemExpireYear * 365; // 무제한 아이템 유지 기간

        // Follow 관련
        public const int MaxFollowCount = 100; // 최대 팔뤄워 수
        public const int MaxFollowTodayCount = 30; //하루 최대 팔뤄워 수

        // 채팅방
        public const int MaxChatRoomCount = 100; // 최대 채팅 방

        // 해쉬 태그 관련
        public const int MaxHashTagCount = 1000; // 최대 해쉬 태그 게시물수

        // 최대 게시물 조회수
        public const int MaxArticleCount = 30; // 요청 할수 있는 최대 게시물 수
        public const int MaxFatchedArticleMonth = -1; // 인기 게시물 최대 기간 (1달)        
        public const int MaxReportedArticleHour = 3; // 힌시간 1인당 최대 신고 건수
        public const int MaxReportedArticleDay = 10; // 오늘 하루 1인당 최대 신고 건수
        public const int MaxReportedArticleWeek = 20; // 1주일 최대 신고 건수
        public const int BannedReportedArticleCount = 7; // 게시물 신고 횟수 삭제
        public const int BannedReportedArticleCommentCount = 10; // 게시물 신고 횟수 삭제

        // Firebase ID
        public readonly static string FirebaseProjectId = "";
        public const int MaxChatRoomMessage = 1000; // 룸 하나에 최대 채팅 갯수

        public readonly static string FirebaseStorageBucketName = $"{FirebaseProjectId}.appspot.com";
        public readonly static string FirebaseStorageCredintial = $"{FirebaseProjectId}-storage-for-server.json";

        // Time 관련
        public readonly static string TimeFormat = "yyyy-MM-dd HH:mm:ss";

        // 구글 정보
        public readonly static string GoogleCredentialKeyJson = $"{FirebaseProjectId}-a2f1b5c01281.json"; // 구글 결제 API Json파일
        public readonly static string GoogleApplicationName = "Google";

        // 좋아요
        public const int MaxLikedShopItemCount = 200; // 최대 좋아요 아이템 수

        // 좋아요
        public const int LimitRecommandCount = 5; // 최대 좋아요 아이템 수

        // 유저 핸들값 업데이트 가능 시기 - 30일 마다 업데이트 가능
        public const int PossibleUpdatedUserHandleDay = 30;

        // 조회 - 최대 조회수
        public const int MaxSearchedCount = 100;

        public const int MaxGameModeHp = 100;   // HP 값 최대
        public const int HunterDamage = 100;    // 술레 데미지

        // Cluade3 APi Key
        public readonly static string ClaudeApiKey = "";
        public const int ClaudeApiMaxToken = 256;
        public readonly static string Claude35Sonnet = "";

        // ChatGpt
        public readonly static string ChatGptApiKey = "";

        // object 초기화 시간
        public readonly static TimeSpan ObjectExpireTime = TimeSpan.FromMinutes(10);

        // 말랑캣 관련 변수
        //public const int SoftCatBasicSpawnCount = 50; // 말랑캣(일반모드 소환수)
        //public const int SoftCatBuffSpawnCount = 15; // 말랑캣(버프)
        //public const int SoftCatRandomSpawnCount = 5; // 말랑캣(랜덤보상)

        public const int PositionScaleFactor = 1000; // float으로 보낼수 없기에 *1000, /1000으로 계산

        public const int MaxUserDeadLetterCount = 1; // 유저 최대 DeadLetter수

        // 봇관련 처리
        public const int BotMovedTimeMilisecond = 330;            // 봇 이동 범위
        public const float BotMovedTimesecond = (float)BotMovedTimeMilisecond / (float)100.0f;            // 봇 이동 범위
        public readonly static TimeSpan BotMovedTime = TimeSpan.FromMilliseconds(BotMovedTimeMilisecond); // 초당 3번 보낸다.
        public const float BotMovedSpeed = 450.0f;
        public const int BotMovedRange = 10;            // 봇 이동 범위
        public readonly static TimeSpan BotNotchUnlockTime = TimeSpan.FromSeconds(3);   // 봇이 노치 해지 시간
        public readonly static TimeSpan BotRespawnTime = TimeSpan.FromSeconds(20);      // 봇 부활 시간                
        public const int BotEntryUserCount = MaxZoneUserCount - 1;

        // 스탬프 관련
        public const int PayedDailyStampeDaimond = 5;
        public const int PayedDailyStampeClover = 100;

        // Session의 KeepAlive 시간 갱신
        public readonly static TimeSpan SessionKeepAliveTime = TimeSpan.FromSeconds(130);
        public readonly static TimeSpan ClientSessionKeepAliveTime = TimeSpan.FromSeconds(60);

        // 골드 페스타 최대치
        public const int MaxGoldDiaFestAmount = 999999999;
        public readonly static TimeSpan CheckedUserNoticeTime = TimeSpan.FromSeconds(30); // 10초 마다 공지 체크

        // 쑥쑥 클로버잎 이벤트
        public const int MaxFreeDailyCloverLeaf = 1; // 하루에 공짜로 얻을수 있는 수
        public const int MaxAdDailyCloverLeaf = 3; // 광고에 의한 최대 획득 잎
        public const int CloverLeafToClover = 4; // 클로버잎 4개에 클로버1개        

        // 게임모드 레벨
        public const int MaxGameModeLevel = 500; // 최대 500        

        // 유저 동접자 수타이머 주기
        public readonly static TimeSpan CheckedConcurrentUserCountTime = TimeSpan.FromMinutes(10); // 10분마다 저장

        // 원스토어 CliendId
        public readonly static string OneStoreClientId = "onestore";
        public readonly static string OneStoreClientSecret = "";
        public readonly static string OneStoreMarketCode = "MKT_ONE"; // MKT_ONE:원스토어(기본), MKT_GLB:원스토어글로벌
        public readonly static string OneStoreGlobalMarketCode = "MKT_GLB"; // MKT_ONE:원스토어(기본), MKT_GLB:원스토어글로벌

        // 눈 정산 시간        
        public readonly static TimeSpan SnowSettlementDate = TimeSpan.FromMinutes(3);

        public readonly static string FacebookAppId = "";        // 페이스북 App ID

        // 너무 빠른 패킷 요청 시간
        public const int ReqMovePacketMilli = 100;        // 100ms이하로 move패킷 요청시 킥

        // notch 갯수
        public const int CleaningBotNotchCount = 30;        // [청소반장] 주변 노치 체크 수
        public const int NaviPathRandomCount = 1;        // 길찾기 - 가까운 위치 램던 수
        public const int CleaningHunterCatchedCount = 3;        // 청소 반장에서 술레가 잡아야 할 수

        // Rank 백업 시간
        public readonly static TimeSpan RankBackupJobTime = TimeSpan.FromMinutes(5);
        public const int RankBackupCount = 100;

        // 오션맵 - 만료 시간
        public readonly static TimeSpan OpenedItemBoxExpireDate = TimeSpan.FromHours(6); // 각 상자 만료
        public readonly static TimeSpan OpenedItemBoxInfosExpireDate = TimeSpan.FromDays(1); // 전체 상자값 만료 시간

        // 경험치 업데이트 타입
        public readonly static TimeSpan ExpNextUpdatedTime = TimeSpan.FromMinutes(5);

        public const long OceupUpOneFloorScore = 100;
        public const long OceupUpDivingScore = 3800;
        public const long OceupUpDivingPossibleStep = 60;
        public const long OceupUpDivingPossibleSecond = 120;

        // 보고서 메일 체크 타이머
        public readonly static TimeSpan CheckedReportMailTime = TimeSpan.FromMinutes(1);

        public const int MaxGameModeRankerCount = 100;
    }
}
