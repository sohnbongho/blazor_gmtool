using Library.DTO;
using log4net;
using MySqlConnector;
using System.Reflection;

namespace Library.Connector
{
    public sealed class MySqlConnectionHelper
    {
        private static readonly Lazy<MySqlConnectionHelper> lazy = new Lazy<MySqlConnectionHelper>(() => new MySqlConnectionHelper());
        public static MySqlConnectionHelper Instance { get { return lazy.Value; } }
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string GameDbConnectionString { get; set; } = string.Empty;
        public string SystemDbConnectionString { get; set; } = string.Empty;
        public string DesighDbConnectionString { get; set; } = string.Empty;
        public string GmtoolConnectionString { get; set; } = string.Empty;

        private MySqlConnectionHelper()
        {
        }

        public MySqlConnection? ConnectionFactory(DbConnectionType connectionType)
        {
            //MySqlConnection의 연결 풀링 기능은 기본적으로 활성화되어 있습니다.
            //따라서 별도로 설정할 필요 없이, 연결 문자열을 사용하여 
            //MySqlConnection 객체를 만들면 자동으로 연결 풀링이 활용됩니다.
            // 아래와 같이 해야 MySql에서 설정한 DB 풀링 기능을 사용할 수 있습니다.
            // 싱글턴 객체로 가지고 있으면 연결 풀링 기능을 쓸 수 없다.
            var connectionString = connectionType switch
            {
                DbConnectionType.System => SystemDbConnectionString,
                DbConnectionType.Design => DesighDbConnectionString,
                DbConnectionType.Game => GameDbConnectionString,
                _ => string.Empty
            };

            var mySqlConnection = new MySqlConnection(connectionString);
            try
            {
                mySqlConnection.Open();
            }
            catch (Exception e)
            {
                _logger.Error($"error connectionfactory - connectionString({connectionString}) exceptionMsg:{e.ToString()}", e);
                return null;
            }
            return mySqlConnection;
        }
        public async Task<MySqlConnection?> ConnectionFactoryAsync(DbConnectionType connectionType)
        {
            // MySqlConnection의 연결 풀링 기능은 기본적으로 활성화되어 있습니다.
            // 따라서 별도로 설정할 필요 없이, 연결 문자열을 사용하여 
            // MySqlConnection 객체를 만들면 자동으로 연결 풀링이 활용됩니다.
            // 아래와 같이 해야 MySql에서 설정한 DB 풀링 기능을 사용할 수 있습니다.
            // 싱글턴 객체로 가지고 있으면 연결 풀링 기능을 쓸 수 없다.
            var connectionString = connectionType switch
            {
                DbConnectionType.System => SystemDbConnectionString,
                DbConnectionType.Design => DesighDbConnectionString,
                DbConnectionType.Game => GameDbConnectionString,
                _ => string.Empty
            };

            var mySqlConnection = new MySqlConnection(connectionString);
            try
            {
                await mySqlConnection.OpenAsync();
            }
            catch (Exception e)
            {
                _logger.Error($"error connectionfactory - connectionString({connectionString})", e);
                return null;
            }
            return mySqlConnection;
        }
        public MySqlConnection? ConnectionFactory(string connectionString)
        {
            //MySqlConnection의 연결 풀링 기능은 기본적으로 활성화되어 있습니다.
            //따라서 별도로 설정할 필요 없이, 연결 문자열을 사용하여 
            //MySqlConnection 객체를 만들면 자동으로 연결 풀링이 활용됩니다.
            // 아래와 같이 해야 MySql에서 설정한 DB 풀링 기능을 사용할 수 있습니다.
            // 싱글턴 객체로 가지고 있으면 연결 풀링 기능을 쓸 수 없다.            

            var mySqlConnection = new MySqlConnection(connectionString);
            try
            {
                mySqlConnection.Open();
            }
            catch (Exception e)
            {
                _logger.Error($"error connectionfactory - connectionString({connectionString}) exceptionMsg:{e.ToString()}", e);
                return null;
            }
            return mySqlConnection;
        }

        public MySqlConnection? GmtoolConnectionFactory()
        {
            // GMTOol 연결을 위한 처리

            var mySqlConnection = new MySqlConnection(GmtoolConnectionString);
            try
            {
                mySqlConnection.Open();
            }
            catch (Exception e)
            {
                _logger.Error($"error connectionfactory - connectionString({GmtoolConnectionString})", e);
                return null;
            }
            return mySqlConnection;
        }
        public async Task<MySqlConnection?> GmtoolConnectionFactoryAsync()
        {
            // GMTOol 연결을 위한 처리

            var mySqlConnection = new MySqlConnection(GmtoolConnectionString);
            try
            {
                await mySqlConnection.OpenAsync();
            }
            catch (Exception e)
            {
                _logger.Error($"error connectionfactory - connectionString({GmtoolConnectionString})", e);
                return null;
            }
            return mySqlConnection;
        }
    }
}
