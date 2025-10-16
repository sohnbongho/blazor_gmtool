using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Enums
{
    /// <summary>
    /// tbl_user_log 관련 Type정보
    /// </summary>
    public enum DbUserLogType : int
    {
        None = 0,

        DeletedCharacter = 1, // 캐릭터 삭제
    }
    
}
