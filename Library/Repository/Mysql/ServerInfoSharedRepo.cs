using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using log4net;
using MySqlConnector;
using System.Reflection;

namespace Library.Repository.Mysql;

public class ServerInfoSharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    /// <summary>
    /// 서버 정보
    /// </summary>        
    public TblServerList GetServerInfo(int serverId)
    {
        using (var db = ConnectionFactory(DbConnectionType.System))
        {
            var query = $"select * from tbl_server_list where server_id={serverId} limit 1";
            if (db == null)
                return new();

            var tbl = db.Query<TblServerList>(query).FirstOrDefault();

            return tbl != null ? tbl : new TblServerList();
        }
    }
    public List<TblServerList> GetServerInfos()
    {
        using (var db = ConnectionFactory(DbConnectionType.System))
        {
            if (db == null)
                return new List<TblServerList>();

            var query = $"select * from tbl_server_list";
            var tbls = db.Query<TblServerList>(query).ToList();
            return tbls;
        }
    }
    
    public async Task<List<TblServerList>> GetServerInfosAsync(MySqlConnection db)
    {
        var query = $"select * from tbl_server_list";
        var result = await db.QueryAsync<TblServerList>(query);
        return result.ToList();
    }

    public TblServerList GetLobbyServerInfo(int worldId)
    {
        using (var db = ConnectionFactory(DbConnectionType.System))
        {
            if (db == null)
            {
                return new();
            }

            var serverType = (int)ServerType.Lobby;
            var query = $"select * from tbl_server_list where world_id={worldId} and server_type = {serverType} limit 1";
            var tbl = db.Query<TblServerList>(query).FirstOrDefault();

            return tbl != null ? tbl : new TblServerList
            {
                ipaddr = string.Empty,
                port = 0,
                op_port = 0,
            };
        }
    }
    
    public async Task<List<TblServerList>> GetCommunityServerInfosAsync()
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.System))
        {
            if (db == null)
            {
                return new();
            }

            var serverType = (int)ServerType.Community;
            var query = $"select * from tbl_server_list where server_type = @server_type;";

            var result = await db.QueryAsync<TblServerList>(query, new
            {
                server_type = serverType,
            });
            return result.ToList();
        }
    }
    public async Task<List<TblServerList>> FetchCommunityServerInfosAsync(MySqlConnection db)
    {
        var serverType = (int)ServerType.Community;
        var query = $"select * from tbl_server_list where server_type = {serverType}";

        var result = await db.QueryAsync<TblServerList>(query);
        return result.ToList();
    }

    /// <summary>
    /// 게임 모드 횟수 추가
    /// </summary>
    /// <param name="gameMode"></param>
    /// <returns></returns>
    public bool AddedGameModeCount(int gameMode)
    {
        if (gameMode == 0)
            return true;

        var query = string.Empty;
        var has = false;
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            query = $"select * from tbl_gamemode_info where game_mode = @game_mode limit 1";

            var tblGameMode = db.Query<TblGamemodeInfo>(query, new TblGamemodeInfo
            {
                game_mode = gameMode,
            }).FirstOrDefault();
            has = (tblGameMode != null);

            try
            {
                if (false == has)
                {
                    query = $"INSERT INTO tbl_gamemode_info VALUES(NULL, @game_mode, 0, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);";

                    var affected = db.Execute(query, new TblGamemodeInfo
                    {
                        game_mode = gameMode,
                    });
                    if (affected <= 0)
                    {
                        _logger.Error($"failed to tbl_character_rank. {query}");
                        return false;
                    }
                }
                else
                {
                    var count = 1;
                    query = $"UPDATE tbl_gamemode_info SET played_count = CASE WHEN played_count + {count} < 0 THEN 0 ELSE played_count + {count} END, updated_date = CURRENT_TIMESTAMP WHERE game_mode = @game_mode;";

                    var affected = db.Execute(query, new TblGamemodeInfo
                    {
                        game_mode = gameMode,
                    });
                    if (affected <= 0)
                    {
                        _logger.Error($"failed to tbl_character_rank. {query}");
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Error($"failed to tbl_character_rank. {query}", ex);
                return false;
            }

        }
        return true;
    }
}
