using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.Logger;
using log4net;
using System.Reflection;

namespace Library.Repository.Mysql;

public class UserSessionSharedRepo : MySqlDbCommonRepo
{
    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

    public static UserSessionSharedRepo Of()
    {
        return new UserSessionSharedRepo();
    }
    private UserSessionSharedRepo()
    {

    }

    /// <summary>
    /// 유저 세션
    /// </summary>
    /// <param name="userSeq"></param>
    /// <returns></returns>
    public (bool, TblMemberSession) FetchUserSessionInfoByUserSeq(ulong userSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (false, new());
            }
            var query = string.Empty;

            // character 아이템
            query = $"SELECT * from tbl_member_session WHERE user_seq = @user_seq";

            var result = db.Query<TblMemberSession>(query, new TblMemberSession
            {
                user_seq = userSeq,
            });
            var tbl = result.FirstOrDefault();
            if (tbl == null)
            {
                return (false, new());
            }

            return (true, tbl);
        }
    }
    public async Task<(bool, TblMemberSession)> FetchUserSessionInfoByUserSeqAsync(ulong userSeq)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (false, new());
            }
            var query = string.Empty;

            // character 아이템
            query = $"SELECT * from tbl_member_session WHERE user_seq = @user_seq";

            var result = await db.QueryAsync<TblMemberSession>(query, new TblMemberSession
            {
                user_seq = userSeq,
            });
            var tbl = result.FirstOrDefault();
            if (tbl == null)
            {
                return (false, new());
            }
            return (true, tbl);
        }
    }
    public (bool, TblMemberSession) FetchUserSessionInfoByCharSeq(ulong charSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (false, new());
            }
            var query = string.Empty;

            // character 아이템
            query = $"SELECT * from tbl_member_session WHERE char_seq = @char_seq";

            var result = db.Query<TblMemberSession>(query, new TblMemberSession
            {
                char_seq = charSeq,
            });
            var tbl = result.FirstOrDefault();
            if (tbl == null)
            {
                return (false, new());
            }

            return (true, tbl);
        }
    }

    public async Task<(bool, TblMemberSession)> FetchUserSessionInfoByCharSeqAsync(ulong charSeq)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (false, new());
            }
            var query = string.Empty;

            // character 아이템
            query = $"SELECT * from tbl_member_session WHERE char_seq = @char_seq";

            var result = await db.QueryAsync<TblMemberSession>(query, new TblMemberSession
            {
                char_seq = charSeq,
            });
            var tbl = result.FirstOrDefault();
            if (tbl == null)
            {
                return (false, new());
            }
            return (true, tbl);
        }
    }

    /// <summary>
    /// 게임 서버 연결
    /// </summary>    
    public void ClearUserConnectedServer(int connectedServerId)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return;
            }
            var query = string.Empty;

            // character 아이템            
            query = $"UPDATE tbl_member_session SET connected_serverid = 0 WHERE connected_serverid = @connected_serverid;";

            var result = db.Execute(query, new TblMemberSession
            {
                connected_serverid = connectedServerId,
            });
        }
    }
    public string StoreUserConnectedServerId(ulong userSeq, ulong charSeq, int connectedServerId)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return string.Empty;
            }
            var query = string.Empty;
            var has = false;

            {
                // character 아이템
                query = $"SELECT * from tbl_member_session WHERE user_seq = @user_seq";

                var result = db.Query<TblMemberSession>(query, new TblMemberSession
                {
                    user_seq = userSeq,
                });
                var tbl = result.FirstOrDefault();
                has = tbl != null;
            }

            if (has == false)
            {
                query = $"INSERT INTO tbl_member_session VALUES(NULL, @user_seq, @char_seq, @connected_serverid, @connected_community_serverid, CURRENT_TIMESTAMP);";

                var affected = db.Execute(query, new TblMemberSession
                {
                    user_seq = userSeq,
                    char_seq = charSeq,
                    connected_serverid = connectedServerId,
                    connected_community_serverid = 0,
                });
            }
            else
            {
                query = $"UPDATE tbl_member_session SET connected_serverid = @connected_serverid WHERE user_seq = @user_seq;";

                var affected = db.Execute(query, new TblMemberSession
                {
                    user_seq = userSeq,
                    connected_serverid = connectedServerId,
                });
            }

            {
                // character 아이템
                query = $"SELECT * from tbl_member_session WHERE user_seq = @user_seq";

                var result = db.Query<TblMemberSession>(query, new TblMemberSession
                {
                    user_seq = userSeq,
                });
                var tbl = result.FirstOrDefault();
                return tbl != null ? ConvertHelper.TimeToString(tbl.updated_date) : string.Empty;
            }
        }
    }
    /// <summary>
    /// 커뮤니티 서버 관련
    /// </summary>    
    public void ClearUserConnectedCommunityServer(int communityServerid)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return;
            }
            var query = string.Empty;

            // character 아이템            
            query = $"UPDATE tbl_member_session SET connected_community_serverid = 0 WHERE connected_community_serverid = @connected_community_serverid;";

            var result = db.Execute(query, new TblMemberSession
            {
                connected_community_serverid = communityServerid,
            });
        }
    }

    public void StoreUserConnectedCommunityServerId(ulong userSeq, ulong charSeq, int communityServerid, int currenctServerId)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return;
            }
            var query = string.Empty;
            var has = false;
            var conncetedCommunityServer = 0;

            {
                // character 아이템
                query = $"SELECT * from tbl_member_session WHERE user_seq = @user_seq";

                var result = db.Query<TblMemberSession>(query, new TblMemberSession
                {
                    user_seq = userSeq,
                });
                var tbl = result.FirstOrDefault();
                has = tbl != null;
                conncetedCommunityServer = tbl != null ? tbl.connected_community_serverid : 0;
            }

            if (has == false)
            {
                query = $"INSERT INTO tbl_member_session VALUES(NULL, @user_seq, @char_seq, @connected_serverid, @connected_community_serverid, CURRENT_TIMESTAMP);";

                var affected = db.Execute(query, new TblMemberSession
                {
                    user_seq = userSeq,
                    char_seq = charSeq,
                    connected_serverid = 0,
                    connected_community_serverid = communityServerid,
                });
            }
            else
            {
                // 접속 종료시, 현재 접속한 커뮤니티 서버가 아닐때는 갱신하지 말자
                if (communityServerid == 0 && currenctServerId != conncetedCommunityServer)
                {
                    _logger.DebugEx(() => $"skip update currenctServerId:{currenctServerId} conncetedCommunityServer:{conncetedCommunityServer}");
                    return;
                }

                query = $"UPDATE tbl_member_session SET connected_community_serverid = @connected_community_serverid WHERE user_seq = @user_seq;";

                var affected = db.Execute(query, new TblMemberSession
                {
                    user_seq = userSeq,
                    connected_community_serverid = communityServerid,
                });
            }
        }
    }

}
