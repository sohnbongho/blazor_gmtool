namespace Library.DBTables
{
    /// <summary>
    /// 레디스에서 사용하는 Query
    /// </summary>
    public class RedisCommonQuery
    {
        public class UserAccountInfo
        {
            public int login_type { get; set; } = 0;
            public string accountid { get; set; } = string.Empty;
            public ulong user_seq { get; set; } = 0;
            public DateTime update_date { get; set; }
        }

        /// <summary>
        /// Session정보
        /// </summary>
        public class UserSessionInfo
        {
            public string session_guid { get; set; } = string.Empty;
            public string accountid { get; set; } = string.Empty;
            public ulong user_seq { get; set; } = 0;
            public ulong char_seq { get; set; } = 0;
            public int login_type { get; set; } = 0;
            public DateTime update_date { get; set; }

            public UserSessionInfo Clone()
            {
                return new UserSessionInfo
                {
                    session_guid = this.session_guid,
                    accountid = this.accountid,
                    user_seq = this.user_seq,
                    char_seq = this.char_seq,
                    login_type = this.login_type,
                    update_date = this.update_date,
                };
            }
        }
        public class Article
        {
            public string Id { get; set; } = string.Empty; // MongoDB의 고유 식별자를 위한 필드
            public string UserSeq { get; set; } = string.Empty; // 작성자 UserSeq 
            public string JsonData { get; set; } = string.Empty;
            public long Liked { get; set; } // 좋아요 수            
            public List<string> HashTags { get; set; } = new();// 해쉬 태그
            public List<string> Mensions { get; set; } = new();// 해쉬 태그
            public List<string> UserSeqTags { get; set; } = new();// 유저 태그
            public int ReportCount { get; set; } = 0;// 신고 횟수
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;

        }
    }
}
