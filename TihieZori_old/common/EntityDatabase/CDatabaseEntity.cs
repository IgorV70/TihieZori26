using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon;


namespace EntityData
{
    partial class CDatabaseEntity: CDatabase
    {
        public override void CopyFrom(CDatabase sourceDb, Action<int> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
