using Dapper;
using Library.Component;
using Library.DBTables.MySql;
using Library.DTO;

namespace Library.Repository.Mysql;

public class SnowSharedRepo : MySqlDbCommonRepo
{
    public static SnowSharedRepo Of() => new SnowSharedRepo();
    private SnowSharedRepo()
    {

    }

    // 눈 관련 정보
    public List<TblZoneSnowObject> FetchObjects(int mapIndex)
    {
        using (var db = ConnectionFactory(DbConnectionType.Design))
        {
            if (db == null)
            {
                return new();
            }

            var query = $"select * from tbl_zone_snow_object where map_id=@map_id";

            var objects = db.Query<TblZoneSnowObject>(query, new TblZoneSnowObject { map_id = mapIndex }).ToList();
            return objects;
        }
    }
}
