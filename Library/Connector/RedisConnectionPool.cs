using log4net;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Reflection;

namespace Library.Connector;

public class RedisConnectionPool
{
    private static readonly Lazy<RedisConnectionPool> lazy = new Lazy<RedisConnectionPool>(() => new RedisConnectionPool());
    public static RedisConnectionPool Instance { get { return lazy.Value; } }
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    private ImmutableList<RedisConnector> _connections = ImmutableList<RedisConnector>.Empty;
    private int _currentIndex = 0;

    private int _poolCount { get; set; } = 1;
    private string _connectionString { get; set; } = string.Empty;

    public void Init(string connectionString, int poolCount)
    {
        _poolCount = poolCount;
        _connectionString = connectionString;

        _connections = ImmutableList<RedisConnector>.Empty; 

        for (int i = 0; i < poolCount; i++)
        {
            var connection = new RedisConnector(connectionString);
            _connections = _connections.Add(connection);
        }
    }

    public ConnectionMultiplexer GetConnection()
    {
        // 원자적으로 인덱스를 증가시키고 현재 인덱스 값을 가져옵니다.
        int currentIndex = Interlocked.Increment(ref _currentIndex);

        // 인덱스가 최대값을 초과하지 않도록 
        if (currentIndex >= _poolCount)
        {
            Interlocked.Exchange(ref _currentIndex, 0);// 원자적으로 리셋되었음을 보장합니다.
            currentIndex = 0; 
        }

        // 원자적으로 인덱스 설정 후, 연결을 반환합니다.
        
        var connection = _connections[currentIndex];
        return connection.Connection;
    }

    public ConnectionMultiplexer GetConnectionWithNormal()
    {
        var connection = ConnectionMultiplexer.Connect(_connectionString);
        return connection;
    }
}

