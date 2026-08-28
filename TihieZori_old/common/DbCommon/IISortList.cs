using System.Collections.Generic;

namespace DbCommon
{
    public interface IISortList<out T> : iSortList, IEnumerable<T>
    {
    }
}