using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommon
{
    public class ObjectPropertyListComparer<T> : IComparer<T>
    {
        public ListSortDesc[] _sorts;

        /// <summary>
        /// Сравнение по нескольким полям
        /// </summary>
        /// <param name="propertyName">Список полей для сравнения</param>
        public ObjectPropertyListComparer(ListSortDesc[] sorts)
        {
            _sorts = sorts;
        }

        public ObjectPropertyListComparer(ListSortDescriptionCollection sorts)
        {
            _sorts = new ListSortDesc[sorts.Count];
            Type thisType = typeof(T);
            int i = 0;
            foreach (ListSortDescription sd in sorts)
                _sorts[i] = new ListSortDesc(thisType.GetProperty(sd.PropertyDescriptor.Name), sd.SortDirection);

        }

        #region Члены IComparer<T>

        /// <summary>
        /// Сравнивает 2 объекта по полям коструктора
        /// </summary>
        /// <param name="x">Первое значение</param>
        /// <param name="y">Второе значение</param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            int ret = 0;
            foreach (ListSortDesc sitem in _sorts)
            {
                object a = sitem.propInfo.GetValue(x, null);
                object b = sitem.propInfo.GetValue(y, null);

                if (a != null && b == null)
                    return sitem.lsd == ListSortDirection.Ascending ? 1 : -1;

                if (a == null && b != null)
                    return sitem.lsd == ListSortDirection.Ascending ? -1 : 1;

                if (a == null && b == null)
                    continue;
                ret = ((IComparable)a).CompareTo(b);
                if (ret != 0) return
                    sitem.lsd == ListSortDirection.Ascending ? ret : -ret; ;
            }
            return 0;
        }

        #endregion
    }
}
