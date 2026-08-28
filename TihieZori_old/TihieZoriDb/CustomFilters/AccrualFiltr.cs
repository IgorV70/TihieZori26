using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon;
using DbCommon.Attributes;

namespace TihieZoriDb
{
    class AccrualFiltr : ICustomFiltr
    {

        public AccrualFiltr(string[] @params)
        {
            if (@params == null)
                return;
            if (@params.Length == 1 && int.TryParse(@params[0], out period))
                return;
            throw new ArgumentException("Некорректные параметры фильтра period");
        }

        readonly int period = 0;

        public Func<BObject, bool> Predicate()
        {
            return
                ((BObject obj) =>
                {
                    if (period == 0)return true;
                    Accrual u = obj as Accrual;
                    return u.AccDate.Year == period;
                });
        }

        public string WhereTerm(DatabaseType databaseType, string tableAlias)
        {
            if (period == 0 )
                return null;
            return $"datepart(year,{tableAlias}.AccDate)={period}";
        }
    }

    public partial class CTableAccrual
    {
        [InitMethod]
        private void AddAdmOrderFiltr()
        {
            AddCustomFiltr("period", typeof(AccrualFiltr));
        }
    }
}

