using StackExchange.Redis;

namespace Library.Connector;

public class RedisConnector
{
    private ConnectionMultiplexer? _connection = null;

    private readonly object _lock = new object();
    public string _connectionString { get; set; } = string.Empty;

    public RedisConnector(string connectionString)
    {
        _connectionString = connectionString;
    }


    //ConnectionMultiplexer는 thread-safe하며, 
    // 내부적으로 연결 재사용, 요청 멀티플렉싱, 연결 장애 처리 등의 작업을 수행합니다.
    // 따라서, 매번 새로운 연결을 생성하는 대신에 ConnectionMultiplexer 인스턴스를 
    // 재사용하면 이러한 기능들을 효과적으로 활용할 수 있습니다.
    public ConnectionMultiplexer Connection
    {
        get
        {
            if (_connection == null || !_connection.IsConnected)
            {
                lock (_lock)
                {
                    if (_connection == null || !_connection.IsConnected)
                    {
                        _connection = ConnectionMultiplexer.Connect(_connectionString);
                    }
                }
            }

            return _connection;
        }
    }
}
