using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DBTables.MongoDb;

public static class MongoDbCollection
{    
    public static readonly string Article = $"article";
    public static readonly string ArticleComment = $"articleComment";
    public static readonly string ArticleLiked = $"articleLiked";
    public static readonly string ArticleReported = $"articleReported";
    public static readonly string ArticleUserSeqTag = $"articleUserSeqTag";

    public static readonly string ChatMessage = $"chatMessage";
    public static readonly string ChatRoom = $"chatRoom";
    public static readonly string ChatRoomForUser = $"chatRoomForUser";

    public static readonly string Hashtag = $"hashtag";
}
