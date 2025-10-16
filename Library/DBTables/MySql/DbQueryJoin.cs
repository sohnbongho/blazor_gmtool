using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 여기서는 DB 컬럼과 동일한 변수명으로 만든다.
/// </summary>
namespace Library.DBTables.MySql
{

    public class JoinQuery
    {
        public class CheckedCount
        {
            public int Count { get; set; }

        }

        public class ZoneInfo
        {
            public int ServerId { get; set; }
            public int ZoneId { get; set; }
            public int MapIndex { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Ip { get; set; } = string.Empty;
            public int Port { get; set; }
            public int UserCount { get; set; }
            public int GameMode { get; set; }
        }

        public class RoomServerInfo
        {
            public int ServerId { get; set; }
            public int GameMode { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Ip { get; set; } = string.Empty;
            public int Port { get; set; }
            public int UserCount { get; set; }

            public string HttpsHost { get; set; } = string.Empty;

        }
    }
}
