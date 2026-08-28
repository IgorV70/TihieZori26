using System.ComponentModel;
using System.Reflection;

namespace DbCommon
{
    public struct ListSortDesc
    {
        public PropertyInfo propInfo;
        public ListSortDirection lsd;
        public ListSortDesc(PropertyInfo propInfo, ListSortDirection lsd)
        {
            this.propInfo = propInfo;
            this.lsd = lsd;
        }
    }

}