using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DBTables.MongoDb
{
    public class MongoDbQuery
    {

        // 채팅관련 메시지
        [BsonIgnoreExtraElements]
        public class ChatRoomUser
        {
            public string UserSeq { get; set; } = string.Empty;
            public string CharSeq { get; set; } = string.Empty;
            public string ProfileImage { get; set; } = string.Empty;// 이미지 Url            
            public string Name { get; set; } = string.Empty;// 이미지 Url
        }

        [BsonIgnoreExtraElements]
        public class ChatRoom
        {
            [BsonId]
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public List<ChatRoomUser> Users { get; set; } = new();// 유저들
            public string Name { get; set; } = string.Empty;
            public string LastestChat { get; set; } = string.Empty;
            public string Host { get; set; } = string.Empty; // 채팅룸 정보
            public int Port { get; set; } = 0;
            public DateTime UpdatedTime { get; set; } = DateTime.MinValue;
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;
        }

        [BsonIgnoreExtraElements]
        public class ChatRoomForUser
        {
            [BsonId]
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public string RoomId { get; set; } = string.Empty;
            public string UserSeq { get; set; } = string.Empty;

            public DateTime UpdatedTime { get; set; } = DateTime.MinValue;
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;
        }


        [BsonIgnoreExtraElements]
        public class ChatMessage
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public string RoomId { get; set; } = string.Empty;
            public List<string> UnreadUserSeqs { get; set; } = new();// 읽지 않은 유저Seq            
            public List<string> ReadUserSeqs { get; set; } = new();// 읽지 않은 유저Seq            
            public string WritedUserSeq { get; set; } = string.Empty;// 작성한 유저
            public string Chat { get; set; } = string.Empty;//             
            public int ChatType { get; set; }
            public string JsonData { get; set; } = string.Empty;// 이미지 Url
            public DateTime CreatedDate { get; set; } = DateTime.MinValue; // 생성 시간
        }

        /// <summary>
        /// 게시물
        /// </summary>
        [BsonIgnoreExtraElements]
        public class Article
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public string UserSeq { get; set; } = string.Empty; // 작성자 UserSeq 
            public string JsonData { get; set; } = string.Empty;
            public long Liked { get; set; } // 좋아요 수            
            public List<string> HashTags { get; set; } = new();// 해쉬 태그
            public List<string> Mensions { get; set; } = new();// 해쉬 태그
            public List<string> UserSeqTags { get; set; } = new();// 유저 태그
            public int ReportCount { get; set; } = 0;// 신고 횟수
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;

        }
        /// <summary>
        /// 게시물
        /// </summary>
        [BsonIgnoreExtraElements]
        public class ArticleUserSeqTag
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public string UserSeq { get; set; } = string.Empty; // 작성자 UserSeq 
            public string ArticleId { get; set; } = string.Empty;
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;

        }

        [BsonIgnoreExtraElements]
        public class ArticleComment
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public ObjectId ArticleId { get; set; } // 게시글 Id
            public string ParentCommentId { get; set; } = string.Empty; // 나의 부모 댓글 
            public string UserSeq { get; set; } = string.Empty; // 작성자 UserSeq             
            public string JsonData { get; set; } = string.Empty;
            public long Liked { get; set; } // 좋아요 수
            public List<string> HashTags { get; set; } = new();// 해쉬 태그수
            public int ReportCount { get; set; } = 0;// 신고 횟수
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;
        }

        /// <summary>
        /// 신고하기
        /// </summary>
        [BsonIgnoreExtraElements]
        public class ArticleReported
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public ObjectId ArticleId { get; set; } // 게시글 Id
            public string UserSeq { get; set; } = string.Empty; // 작성자 UserSeq             
            public DateTime CreatedTime { get; set; } = DateTime.MinValue;
        }

        /// <summary>
        /// 해쉬 태그
        /// </summary>
        [BsonIgnoreExtraElements]
        public class HashTag
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public string Tag { get; set; } = string.Empty;
            public long Count { get; set; }
            public List<string> ArticleIds { get; set; } = new();

        }

        [BsonIgnoreExtraElements]
        public class ArticleLiked
        {
            public ObjectId Id { get; set; } // MongoDB의 고유 식별자를 위한 필드
            public ObjectId ArticleId { get; set; } // 게시글 Id
            public string UserSeq { get; set; } = string.Empty; // 좋아요 유저
            public bool Liked { get; set; } // 여부
        }
    }
}
