#pragma warning disable 8625, 8618, 8602, 8604

using Library.Connector;
using Library.Helper;
using log4net;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace AdminTool.Services;

public class ConfigService
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    // Public property to get the singleton instance
    public static ConfigService Instance => _instance.Value;
    private static readonly Lazy<ConfigService> _instance = new Lazy<ConfigService>(() => new ConfigService());

    private JObject _jsonObj = null;

    private string _gameDbConnectionString;
    private string _gmtoolDbConnectionString;
    private string _mongodbConnectString = string.Empty;

    private string _redisConnectString;
    private int _redisPoolCount;

    public int Port => _port;
    private int _port = 5102;

    public bool Https => _https;
    private bool _https = false;

    public int ServerId => _serverId;
    private int _serverId = 0;

    // Nat 관련 
    public string NatsConnectString => _natsConnectString;
    private string _natsConnectString = string.Empty;

    // Private constructor to prevent instantiation
    private ConfigService()
    {
    }

    public bool Load()
    {
        try
        {
            var fullPath = Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(fullPath);

            string filePath = $@"{directoryPath}/Config.json5"; // 수정해야 할 부분
            string jsonString = File.ReadAllText(filePath);

            // Parse JSON string to JObject using Newtonsoft.Json
            _jsonObj = JObject.Parse(jsonString);

            _gameDbConnectionString = _jsonObj["db"]["mySql"]["connectString"]["gameDb"].ToString();
            _gmtoolDbConnectionString = _jsonObj["db"]["mySql"]["connectString"]["gmtool"].ToString();

            _redisConnectString = _jsonObj["db"]["redis"]["connectString"].ToString();
            _redisPoolCount = _jsonObj["db"]["redis"]["poolCount"].Value<int>();

            var systemDbConnectionString = _jsonObj["db"]["mySql"]["connectString"]["system"].ToString();
            var designDbConnectionString = _jsonObj["db"]["mySql"]["connectString"]["design"].ToString();

            MySqlConnectionHelper.Instance.GameDbConnectionString = _gameDbConnectionString;
            MySqlConnectionHelper.Instance.GmtoolConnectionString = _gmtoolDbConnectionString;
            MySqlConnectionHelper.Instance.SystemDbConnectionString = systemDbConnectionString;
            MySqlConnectionHelper.Instance.DesighDbConnectionString = designDbConnectionString;

            RedisConnectionPool.Instance.Init(_redisConnectString, _redisPoolCount);

            _mongodbConnectString = _jsonObj["db"]["mongodb"]["connectString"].ToString();
            MongoDbConnectorHelper.ConnectionString = _mongodbConnectString;

            _https = _jsonObj["swagger"]["https"].Value<bool>();
            _port = _jsonObj["swagger"]["port"].Value<int>();

            _natsConnectString = _jsonObj["nats"]["connectString"].ToString();

            _serverId = _jsonObj["world"]["serverId"].Value<int>();
            SnowflakeIdGenerator.Instance.SetWorkerId(_serverId);

            if (false == string.IsNullOrEmpty(directoryPath))
            {
                if (false == LoadGoogleAppCredetials(directoryPath))
                    return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("failed to load config", ex);
            return false;
        }
    }
    public bool LoadGoogleAppCredetials(string directoryPath)
    {
        // serviceAccountKey.json
        // Goole 번역기 사용이 필요하면 열어줘야 함 (ITranslationService)
        return true;

        //try
        //{
        //    string pathToServiceAccountKeyFile = @$"{directoryPath}/serviceAccountKey.json";
        //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToServiceAccountKeyFile);

        //    _logger.InfoEx(() => "succeed to load GOOGLE_APPLICATION_CREDENTIALS.");

        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    _logger.Error(() => "failed to load GOOGLE_APPLICATION_CREDENTIALS", ex);
        //    return false;
        //}
    }
}

#pragma warning restore 8625, 8618, 8602, 8604