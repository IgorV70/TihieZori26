using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon;
using DbCommon.Attributes;

namespace TihieZoriDb
{
    class PaymentFiltrPeriod : ICustomFiltr
    {

        public PaymentFiltrPeriod(string[] @params)
        {
            if (@params == null)
                return;
            if (@params.Length == 1 && int.TryParse(@params[0], out period))
            {
                _params = @params;
                return;
            }
            throw new ArgumentException("Некорректные параметры фильтра period");
        }

        readonly int period = 0;
        readonly string[] _params ;
        private SortList<Accrual> _accList =null;

        public Func<BObject, bool> Predicate()
        {
            return
                ((BObject obj) =>
                {
                    if (period == 0)return true;
                    Payment u = obj as Payment;
                    if (u.PayDate.Year < period)
                        return false;
                    if (_accList == null)
                    {
                        CDatabaseTihieZori db = (CDatabaseTihieZori)obj._table.parentDataBase;
                        _accList = db.Accrual.GetObjectListByCustom(new AccrualFiltr(_params));
                    }
                    int userId = u.UserId;
                    int proposeId = u.ProposeId;
                    return _accList.Exists(acc => acc.UserId == userId && acc.ProposeId == proposeId);
                });
        }

        public string WhereTerm(DatabaseType databaseType, string tableAlias)
        {
            if (period == 0 )
                return null;
            return $"exists(select * from dbo.Accrual where datepart(year,AccDate)={period} and {tableAlias}.userid = userid and {tableAlias}.proposeid";
        }
    }

    public partial class CTablePayment
    {
        [InitMethod]
        private void AddPeriodFiltr()
        {
            AddCustomFiltr("period", typeof(PaymentFiltrPeriod));
        }
    }
}

