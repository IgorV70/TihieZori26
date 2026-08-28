using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon;
using DbCommon.Attributes;

namespace TihieZoriDb
{
    class PaymentFiltr : ICustomFiltr
    {

        public PaymentFiltr(string[] @params)
        {
            if (@params == null)
                return;
            if (@params.Length == 1 && int.TryParse(@params[0], out proposeid))
                return;
            if (@params.Length == 2 && int.TryParse(@params[0], out proposeid) && int.TryParse(@params[1], out userid))
                return;
            throw new ArgumentException("Некорректные параметры фильтра custom");
        }

        readonly int userid = 0;
        readonly int proposeid = 0;

        public Func<BObject, bool> Predicate()
        {
            return
                ((BObject obj) =>
                {
                    if (userid == 0 && proposeid == 0)
                        return true;
                    Payment u = obj as Payment;
                    if (userid == 0)
                        return u.ProposeId == proposeid;
                    if (proposeid == 0)
                        return u.UserId == userid;
                    return u.ProposeId == proposeid && u.UserId == userid;
                });
        }

        public string WhereTerm(DatabaseType databaseType, string tableAlias)
        {
            if (userid == 0 && proposeid == 0)
                return null;
            if (userid == 0)
                return $"{tableAlias}.proposeid={proposeid}";
            if (proposeid == 0)
                return $"{tableAlias}.userid={userid}";
            return $"{tableAlias}.userid={userid} and {tableAlias}.proposeid={proposeid}";
        }
    }

    public partial class CTablePayment
    {
        [InitMethod]
        private void AddAdmOrderFiltr()
        {
            AddCustomFiltr("custom", typeof(PaymentFiltr));
        }
    }
}

