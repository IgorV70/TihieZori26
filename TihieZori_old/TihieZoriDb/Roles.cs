using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon.Attributes;
using TihieZoriDb;

namespace TihieZoriDb
{
    public partial class CTableRoles
    {
        [StartValue]
        public static List<object> StartFill(CDatabaseTihieZori db)
        {
            var ret = new List<object>
            {
                new Roles{ Id = (int)RoleEnum.Admin,Name = "Администратор"},
                new Roles{ Id = (int)RoleEnum.User,Name = "Пользователь"},
            };
            return ret;
        }
    }
}
