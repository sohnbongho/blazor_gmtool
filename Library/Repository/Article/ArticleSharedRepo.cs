using Library.Connector;
using Library.DBTables.MongoDb;
using Library.DTO;
using Library.Helper;
using Library.Helper.Firebase;
using Library.Model;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Reflection;
using static Library.DBTables.MongoDb.MongoDbQuery;

namespace Library.Repository.Article;

public class ArticleSharedRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public ArticleSharedRepo()
    {
    }

    public void DeletedArticles(List<MongoDbQuery.Article> articles)
    {
        var mongoDb = MongoDbConnectorHelper.Instance;
        var databaseName = MongoDbDataBase.CommunityBoard;
        var articleId = string.Empty;
        var database = mongoDb.GetDatabase(databaseName);

        var deletedArticleIds = articles.Select(x => x.Id).ToList();
        var deletedArticleIdsStr = articles.Select(x => x.Id.ToString()).ToList();

        DeleteImages(articles); // 이미지 삭제

        // Article : 게시글 삭제
        {
            var collectionName = MongoDbCollection.Article;
            var collection = database.GetCollection<MongoDbQuery.Article>(collectionName);
            var filter = Builders<MongoDbQuery.Article>.Filter.In(x => x.Id, deletedArticleIds);
            var result = collection.DeleteMany(filter);
        }

        // articleComment : 댓글
        {
            var collectionName = MongoDbCollection.ArticleComment;
            var collection = database.GetCollection<MongoDbQuery.ArticleComment>(collectionName);
            var filter = Builders<MongoDbQuery.ArticleComment>.Filter.In(x => x.ArticleId, deletedArticleIds);
            var result = collection.DeleteMany(filter);
        }

        // articleLiked : 좋아요
        {
            var collectionName = MongoDbCollection.ArticleLiked;
            var collection = database.GetCollection<MongoDbQuery.ArticleLiked>(collectionName);
            var filter = Builders<MongoDbQuery.ArticleLiked>.Filter.In(x => x.ArticleId, deletedArticleIds);
            var result = collection.DeleteMany(filter);
        }

        // ArticleReported
        {
            var collectionName = MongoDbCollection.ArticleReported;
            var collection = database.GetCollection<MongoDbQuery.ArticleReported>(collectionName);
            var filter = Builders<MongoDbQuery.ArticleReported>.Filter.In(x => x.ArticleId, deletedArticleIds);
            var result = collection.DeleteMany(filter);
        }

        // ArticleUserSeqTag
        {
            var collectionName = MongoDbCollection.ArticleUserSeqTag;
            var collection = database.GetCollection<MongoDbQuery.ArticleUserSeqTag>(collectionName);
            var filter = Builders<MongoDbQuery.ArticleUserSeqTag>.Filter.In(x => x.ArticleId, deletedArticleIdsStr);
            var result = collection.DeleteMany(filter);
        }


    }

    private void DeleteImages(List<MongoDbQuery.Article> articles)
    {
        foreach (var article in articles)
        {
            var jsonData = JsonConvert.DeserializeObject<ArticleUserPost>(article.JsonData);
            if (jsonData == null)
                continue;

            try
            {
                var instance = FirebaseStorageHelper.Instance;

                var imageUrl = jsonData.url;
                instance.DeleteFile(imageUrl);

                var prevViewUrl = StringHelper.RemoveLastJpeg(jsonData.url) + "_preview.jpeg";
                instance.DeleteFile(prevViewUrl);
            }
            catch (Exception ex)
            {
                _logger.Error("fail CleanUpArticle", ex);
            }
        }
    }

    private async Task<bool> DeleteImagesAsync(List<MongoDbQuery.Article> articles)
    {
        foreach (var article in articles)
        {
            var jsonData = JsonConvert.DeserializeObject<ArticleUserPost>(article.JsonData);
            if (jsonData == null)
                continue;

            try
            {
                var instance = FirebaseStorageHelper.Instance;

                var imageUrl = jsonData.url;
                await instance.DeleteFileAsync(imageUrl);

                var prevViewUrl = StringHelper.RemoveLastJpeg(jsonData.url) + "_preview.jpeg";
                await instance.DeleteFileAsync(prevViewUrl);
            }
            catch (Exception ex)
            {
                _logger.Error("fail CleanUpArticle", ex);
            }
        }
        return true;
    }

    public async Task<ErrorCode> DeletedArticleAsync(string articleId)
    {
        var mongoDb = MongoDbConnectorHelper.Instance;
        var databaseName = MongoDbDataBase.CommunityBoard;
        var database = mongoDb.GetDatabase(databaseName);

        try
        {
            var articleObjectId = new ObjectId(articleId);

            // 이미지 삭제
            {
                var collectionName = MongoDbCollection.Article;
                var collection = database.GetCollection<MongoDbQuery.Article>(collectionName);
                var filter = Builders<MongoDbQuery.Article>.Filter.Eq(a => a.Id, articleObjectId); // articleUid는 대상 게시글의 Uid
                var result = await collection.FindAsync(filter); // 문서 찾기
                var find = result?.FirstOrDefault() ?? null;
                if (find != null)
                {
                    await DeleteImagesAsync(new List<MongoDbQuery.Article> { find });
                }
            }

            // 게시글 삭제
            {
                var collectionName = MongoDbCollection.Article;
                var collection = database.GetCollection<MongoDbQuery.Article>(collectionName);
                var filter = Builders<MongoDbQuery.Article>.Filter.Eq(a => a.Id, articleObjectId); // articleUid는 대상 게시글의 Uid
                var deleteResult = await collection.DeleteOneAsync(filter); // 문서 삭제
                if (deleteResult.DeletedCount <= 0)
                {
                    return ErrorCode.NotFoundArticle;
                }
            }

            // 댓글 삭제
            {
                var collectionName = MongoDbCollection.ArticleComment;
                var collection = database.GetCollection<ArticleComment>(collectionName);
                var filter = Builders<ArticleComment>.Filter.Eq(a => a.ArticleId, articleObjectId); // articleUid는 대상 게시글의 Uid
                var deleteResult = await collection.DeleteManyAsync(filter); // 문서 삭제
            }

            // 좋아요 정보 삭제
            {
                var collectionName = MongoDbCollection.ArticleLiked;
                var collection = database.GetCollection<ArticleLiked>(collectionName);
                var filter = Builders<ArticleLiked>.Filter.Eq(a => a.ArticleId, articleObjectId); // articleUid는 대상 게시글의 Uid
                var deleteResult = await collection.DeleteManyAsync(filter); // 문서 삭제
            }

            // 유저 UserTag삭제
            {
                var collectionName = MongoDbCollection.ArticleUserSeqTag;
                var collection = database.GetCollection<ArticleUserSeqTag>(collectionName);
                var filter = Builders<ArticleUserSeqTag>.Filter.Eq(a => a.ArticleId, articleId); // articleUid는 대상 게시글의 Uid
                var deleteResult = await collection.DeleteManyAsync(filter); // 문서 삭제
            }

            return ErrorCode.Succeed;
        }
        catch (MongoConnectionException mEx)
        {
            // 연결 관련 예외가 발생했을 때 재연결 시도
            MongoDbConnectorHelper.HandleMongoConnectionException(mEx);
            _logger.Error("reconnect to MongoDB.");
            return ErrorCode.DbDeletedError;
        }
        catch (Exception ex)
        {
            _logger.Error("failed to DeletedArticleAsync.", ex);
            return ErrorCode.DbDeletedError;
        }

    }


}
