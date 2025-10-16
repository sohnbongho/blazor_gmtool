using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DBTables.MySql
{
    public class GMToolDbQuery
    {
        public class TblMember
        {
            public long id { get; set; }
            public string user_id { get; set; } = string.Empty;
            public string user_passwd { get; set; } = string.Empty;
            public string user_desc { get; set; } = string.Empty;

            public DateTime updated_date { get; set; }
            public DateTime created_date { get; set; }

        }
    }
}
