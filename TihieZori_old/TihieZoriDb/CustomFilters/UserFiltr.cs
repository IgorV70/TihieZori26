using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon;
using DbCommon.Attributes;

namespace TihieZoriDb
{
    class UserFiltr : ICustomFiltr
    {

        public UserFiltr(string[] @params)
        {
            if (@params == null)
                return;
            throw new ArgumentException("Некорректные параметры фильтра active");
        }

        public Func<BObject, bool> Predicate()
        {
            return
                ((BObject obj) =>
                {
                    User u = obj as User;
                    return !string.IsNullOrEmpty(u.LandNumber) && !string.IsNullOrEmpty(u.Fio);
                });
        }

        public string WhereTerm(DatabaseType databaseType, string tableAlias)
        {
            return string.Format("{0}.landNumber >0 and ISNULL({0}.Fio,'') !=''", tableAlias);
        }
    }

    public partial class CTableUser
    {
        [InitMethod]
        private void AddAdmOrderFiltr()
        {
            AddCustomFiltr("active", typeof(UserFiltr));
        }
    }
}

