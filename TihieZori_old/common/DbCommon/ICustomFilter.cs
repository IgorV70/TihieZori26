using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommon
{
    public interface ICustomFiltr
    {
        Func<BObject, bool> Predicate();
        string WhereTerm(DatabaseType databaseType, string tableAlias);
    }
}
