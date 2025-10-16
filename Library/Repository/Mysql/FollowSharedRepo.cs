using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Messages;

namespace Library.Repository.Mysql;

public class FollowSharedRepo : MySqlDbCommonRepo
{
    public static FollowSharedRepo Of() => new FollowSharedRepo();
    private FollowSharedRepo()
    {

    }

    // <summary>
    /// 팔로워(나를 따라다니는 사람) 유저들
    /// </summary>
    public async Task<(bool, List<FollowUser>)> FetchFollowerUsersAsync(ulong userSeq, int offset, int limit)
    {
        var query = string.Empty;
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return (false, new List<FollowUser>());
            }

            var followUsers = new List<FollowUser>();

            query = $"SELECT * from tbl_user_follow WHERE follow_user_seq = @follow_user_seq LIMIT {limit} OFFSET {offset};";

            var resultFollow = await db.QueryAsync<TblUserFollow>(query, new TblUserFollow
            {
                follow_user_seq = userSeq,
            });
            var tblUserFollows = resultFollow.ToList();

            foreach (var tblFollow in tblUserFollows)
            {
                var nickName = string.Empty;

                query = $"SELECT * from tbl_member WHERE user_seq = @user_seq;";

                var resultMember = await db.QueryAsync<TblMember>(query, new TblMember
                {
                    user_seq = tblFollow.user_seq,
                });
                var followMember = resultMember.FirstOrDefault();

                if (null == followMember)
                    continue;

                query = $"SELECT * from tbl_character WHERE char_seq = @char_seq;";

                var resultCharacter = await db.QueryAsync<TblCharacter>(query, new TblCharacter
                {
                    char_seq = followMember.char_seq,
                });
                var followCharacter = resultCharacter.FirstOrDefault();

                if (followCharacter != null)
                {
                    nickName = followCharacter.nickname;
                    followUsers.Add(new FollowUser
                    {
                        UserSeq = followMember.user_seq.ToString(),
                        Nickname = nickName,
                        CharSeq = followMember.char_seq.ToString(),
                        ImageUrl = followMember.image_url,
                        CreatedDate = ConvertHelper.TimeToString(tblFollow.created_date),
                        UserHandle = followMember.user_handle,
                    });
                }
            }
            return (true, followUsers);
        }
    }

    /// <summary>
    /// 팔로우(내가 따라 다니는 사람) 유저들 수
    /// </summary>        
    /// <returns></returns>
    public async Task<int> FetchFollowUserCountAsync(ulong userSeq)
    {
        var query = string.Empty;
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return 0;
            }

            query = $"SELECT COUNT(*) as Count FROM tbl_user_follow WHERE user_seq  = @user_seq;";

            var resultQuery = await db.QueryAsync<JoinQuery.CheckedCount>(query, new TblUserFollow
            {
                user_seq = userSeq,
            });
            var result = resultQuery.FirstOrDefault();
            if (result != null)
                return result.Count;
        }

        return 0;
    }

    /// <summary>
    ///  팔로워(나를 따라다니는 사람) 유저들 수
    /// </summary>
    /// <param name="userSeq"></param>        
    /// <returns></returns>
    public async Task<int> FetchFollowerUserCountAsync(ulong userSeq)
    {
        var query = string.Empty;
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return 0;
            }

            query = $"SELECT COUNT(*) as Count FROM tbl_user_follow WHERE follow_user_seq  = @follow_user_seq;";

            var resultQuery = await db.QueryAsync<JoinQuery.CheckedCount>(query, new TblUserFollow
            {
                follow_user_seq = userSeq,
            });
            var result = resultQuery.FirstOrDefault();
            if (result != null)
                return result.Count;
        }

        return 0;
    }
}
