using System.Collections.Generic;
using DbCommon.Attributes;

namespace TihieZoriDb
{
    public partial class CTableDbVersion
    {
        static DbVersion v = new DbVersion() { VersionNum = 1, Comment = "20180315" };

        [StartValue]
        public static List<object> StartFill()
        {
            return new List<object>() { v };
        }

        public static DbVersion CurrentVersion()
        {
            return v;
        }
    }
}
