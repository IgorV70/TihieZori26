using System.Collections.Generic;
using DbCommon;

namespace DbCommon
{
    public interface ITableLogger
    {
        void SaveLog(List<ChangedValuesItem> list, string comment, string avtor, int id, string name);
        void SaveLog(List<ChangedValuesItem> list, int id, string name);
        void SaveLog(List<ChangedValuesItem> list, int id);
        void SetLogInfo(string comment, string avtor);
    }
}