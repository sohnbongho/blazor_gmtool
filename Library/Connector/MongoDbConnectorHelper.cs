using Library.Logger;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Library.Connector
{
    public class MongoDbConnectorHelper
    {
        private static MongoClient? _instance = null;
        private static readonly object padlock = new object();
        private static IMongoDatabase _database = null!;
        public static string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public static string DatabaseName { get; set; } = "testdb";
        private static readonly TimeSpan _delay = TimeSpan.FromSeconds(2);
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public static MongoClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            // 이미 InitializeMongoClient에서 초기화를 하였다. 
                            InitializeMongoClient();
                        }                            
                    }
                }
#pragma warning disable CS8603 // 가능한 null 참조 반환입니다.
                return _instance;
#pragma warning restore CS8603 // 가능한 null 참조 반환입니다.

            }
        }
        private static void InitializeMongoClient()
        {
            _instance = new MongoClient(ConnectionString);
            _database = _instance.GetDatabase(DatabaseName);
            bool isConnected = TryConnectWithRetryAsync().GetAwaiter().GetResult();
            if (!isConnected)
            {
                throw new Exception("Failed to initialize MongoDB connection.");
            }
        }        

        private static async Task<bool> TryConnectWithRetryAsync(int maxRetryAttempts = 3)
        {
            int attempt = 0;
            while (attempt < maxRetryAttempts)
            {
                try
                {
                    // 서버 상태를 확인하여 연결을 시도합니다.
                    await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                    _logger.InfoEx(()=>"Connected to MongoDB.");
                    return true; // 연결 성공
                }
                catch (Exception ex)
                {
                    // 연결 실패를 처리합니다.
                    _logger.InfoEx(()=>$"Failed to connect to MongoDB: {ex.Message}. Attempt {attempt + 1} of {maxRetryAttempts}");
                    attempt++;
                    await Task.Delay(_delay);
                }
            }
            return false; // 모든 재시도가 실패한 경우
        }
        public static void HandleMongoConnectionException(MongoConnectionException ex)
        {
            _logger.Error("MongoConnectionException occurred, attempting to reconnect...", ex);
            lock (padlock)
            {
                _instance = null;
                InitializeMongoClient();
            }
        }
    }
}
