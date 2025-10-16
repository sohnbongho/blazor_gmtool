using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;
using Library.Helper;
using Library.Repository.Redis;
using MySqlConnector;


namespace Library.Repository.Mysql;

public class UserSharedRepo : MySqlDbCommonRepo
{
    public static UserSharedRepo Of()
    {
        return new UserSharedRepo();
    }
    private UserSharedRepo()
    {

    }

    private UserInfoSessionSharedRepo _userInfoRedis = UserInfoSessionSharedRepo.Of();
    /// <summary>
    /// tbl_member 조회
    /// </summary>        
    public async Task<TblMember> FetchMemberAsync(ulong userSeq)
    {
        if (0 == userSeq)
        {
            return new TblMember { };
        }
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_member where user_seq=@user_seq limit 1;";
            var result = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
            var tblCharacter = result.FirstOrDefault();
            return tblCharacter != null ? tblCharacter : new TblMember { user_seq = 0 };
        }
    }
    public async Task<TblMember> FetchMemberAsync(MySqlConnection db, MySqlTransaction transaction, ulong userSeq)
    {
        var query = $"select * from tbl_member where user_seq=@user_seq limit 1;";
        var result = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq }, transaction);
        var tblCharacter = result.FirstOrDefault();
        return tblCharacter != null ? tblCharacter : new TblMember { user_seq = 0 };
    }
    public async Task<TblMember> FetchMemberAsync(MySqlConnection db, ulong userSeq)
    {
        var query = $"select * from tbl_member where user_seq=@user_seq limit 1;";
        var result = await db.QueryAsync<TblMember>(query, new TblMember { user_seq = userSeq });
        var tblCharacter = result.FirstOrDefault();
        return tblCharacter != null ? tblCharacter : new TblMember { user_seq = 0 };
    }
    public async Task<TblMember> FetchMemberAsync(LoginType loginType, string userId)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_member where user_id=@user_id and login_type = @login_type";

            var reuslt = await db.QueryAsync<TblMember>(query, new TblMember
            {
                user_id = userId,
                login_type = (short)loginType,

            });
            var tblMember = reuslt.FirstOrDefault();
            return tblMember != null ? tblMember : new TblMember { user_seq = 0 };
        }
    }
    public TblMember FetchMember(ulong userSeq)
    {
        if (0 == userSeq)
        {
            return new TblMember { };
        }
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_member where user_seq=@user_seq limit 1;";
            var result = db.Query<TblMember>(query, new TblMember
            {
                user_seq = userSeq
            });
            var tblCharacter = result.FirstOrDefault();
            return tblCharacter != null ? tblCharacter : new TblMember { user_seq = 0 };
        }
    }


    public bool IsBlockGameModeType(GameModeType gameModeType)
    {
        var featureControl = ConvertHelper.GameModeToFeatureControl(gameModeType);
        if (featureControl == FeatureControlType.None)
            return false;

        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return false;

            var query = $"select * from tbl_feature_control where control_type = @control_type and blocked = 1 limit 1;";
            var result = db.Query<TblFeatureControl>(query, new TblFeatureControl
            {
                control_type = (int)featureControl
            });
            var tblCharacter = result.FirstOrDefault();
            return tblCharacter != null;
        }
    }
    public List<FeatureControlType> BlockGameModeTypes()
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
                return new List<FeatureControlType>();

            var query = $"select * from tbl_feature_control where blocked = 1";
            var result = db.Query<TblFeatureControl>(query);
            return result.Select(x => ConvertHelper.ToEnum<FeatureControlType>(x.control_type)).ToList();
        }
    }

    public TblMember FetchMemberByCharSeq(ulong charSeq)
    {
        if (0 == charSeq)
        {
            return new TblMember { };
        }
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_member where char_seq=@char_seq limit 1;";
            var result = db.Query<TblMember>(query, new TblMember
            {
                char_seq = charSeq
            });
            var tblMember = result.FirstOrDefault();
            return tblMember != null ? tblMember : new TblMember { user_seq = 0 };
        }
    }
    public async Task<TblMember> FetchMemberByCharSeqAsync(ulong charSeq)
    {
        if (0 == charSeq)
        {
            return new TblMember { };
        }
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_member where char_seq=@char_seq limit 1;";
            var result = await db.QueryAsync<TblMember>(query, new TblMember
            {
                char_seq = charSeq
            });
            var tblMember = result.FirstOrDefault();
            return tblMember != null ? tblMember : new TblMember { user_seq = 0 };
        }
    }
    public async Task<TblMember> FetchMemberByCharSeqAsync(MySqlConnection db, MySqlTransaction transaction, ulong charSeq)
    {
        if (0 == charSeq)
        {
            return new TblMember { };
        }
        var query = $"select * from tbl_member where char_seq = @char_seq limit 1;";
        var result = await db.QueryAsync<TblMember>(query, new TblMember
        {
            char_seq = charSeq
        }, transaction);
        var tblCharacter = result.FirstOrDefault();
        return tblCharacter != null ? tblCharacter : new TblMember { user_seq = 0 };
    }

    /// <summary>
    /// tbl_character 조회
    /// </summary>        
    public async Task<TblCharacter> FetchCharacterAsync(ulong charSeq)
    {
        if (0 == charSeq)
        {
            return new TblCharacter { };
        }
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_character where char_seq = @char_seq limit 1;";
            var result = await db.QueryAsync<TblCharacter>(query, new TblCharacter
            {
                char_seq = charSeq,
            });
            var tblCharacter = result.FirstOrDefault();
            return tblCharacter != null ? tblCharacter : new TblCharacter { char_seq = 0 };
        }
    }

    public TblCharacter FetchCharacter(ulong charSeq)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_character where char_seq={charSeq} limit 1";

            var charaterInfo = db.Query<TblCharacter>(query).FirstOrDefault();
            return charaterInfo != null ? charaterInfo : new TblCharacter();
        }
    }

    /// <summary>
    /// 캐릭터 정보 갱신
    /// </summary>    
    public int UpdateCharacterInfo(TblCharacter character)
    {
        using (var db = ConnectionFactory(DbConnectionType.Game))
        {
            if (db == null)
            {
                return 0;
            }

            var query = $"UPDATE tbl_character SET map_id = @map_id, pos_x = @pos_x, pos_y = @pos_y, pos_z = @pos_z WHERE char_seq = @char_seq";

            int affected = db.Execute(query, character);
            return affected;
        }
    }

    public async Task<bool> UpdateUserBanAsync(ulong userSeq, DateTime banExpiryDate)
    {
        var banned = false;
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }


            var query = $"UPDATE tbl_member SET ban_expiry_date = @ban_expiry_date where user_seq = @user_seq";

            int affected = await db.ExecuteAsync(query, new TblMember
            {
                user_seq = userSeq,
                ban_expiry_date = banExpiryDate,
            });
            banned = affected > 0;
        }
        var tblMember = await FetchMemberAsync(userSeq);
        var sessionGuid = tblMember.session_guid;
        await _userInfoRedis.DeleteUserSessionAsync(sessionGuid);

        return banned;
    }

    public async Task<bool> BanArticleCommentAsync(ulong userSeq, string title, DateTime banExpiryDate)
    {
        var banned = false;
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }
            var has = false;
            var tblMember = await FetchMemberAsync(db, userSeq);
            if (tblMember.user_seq == 0)
            {
                return false;
            }

            {
                var query = $"select * from tbl_ban_article_comment where user_seq = @user_seq limit 1";

                var result = db.Query<TblBanArticleComment>(query, new TblBanArticleComment
                {
                    user_seq = userSeq,
                });
                var tbl = result.FirstOrDefault();
                has = tbl != null;
            }
            if (has)
            {
                var query = $"UPDATE tbl_ban_article_comment SET ban_expiry_date = @ban_expiry_date, title = @title where user_seq = @user_seq";

                int affected = await db.ExecuteAsync(query, new TblBanArticleComment
                {
                    user_seq = userSeq,
                    title = title,
                    ban_expiry_date = banExpiryDate,
                });
                banned = affected > 0;
            }
            else
            {
                var query = $"INSERT INTO tbl_ban_article_comment VALUES(NULL, @user_seq, @title, @ban_expiry_date, CURRENT_TIMESTAMP);";

                int affected = await db.ExecuteAsync(query, new TblBanArticleComment
                {
                    user_seq = userSeq,
                    title = title,
                    ban_expiry_date = banExpiryDate,
                });
                banned = affected > 0;
            }
        }

        return banned;
    }

    public async Task<bool> UpdatedMemberFirebaseUidAsync(ulong userSeq, string firebaseUserUid)
    {
        await using (var db = await ConnectionFactoryAsync(DbConnectionType.Game))
        {
            if (db == null)
            {
                return false;
            }

            var query = $"UPDATE tbl_member SET firebase_uid = @firebase_uid WHERE user_seq = @user_seq;";

            int rowsAffected = await db.ExecuteAsync(query, new TblMember
            {
                user_seq = userSeq,
                firebase_uid = firebaseUserUid,
            });
            if (rowsAffected <= 0)
                return false;

            return true;
        }
    }
}
