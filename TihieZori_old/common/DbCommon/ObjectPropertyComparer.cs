using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DbCommon
{
    public class ObjectPropertyComparer<T> : IComparer<T>
    {
        private readonly string _propertyName;
        public ListSortDirection lsd;


        /// <summary>
        /// Provides Comparison opreations.
        /// </summary>
        /// <param name="propertyName">The property to compare</param>
        public ObjectPropertyComparer(string propertyName, ListSortDirection direction)
        {
            _propertyName = propertyName;
            lsd = direction;
        }
        #region Члены IComparer<T>

        /// <summary>
        /// Compares 2 objects by their properties, given on the constructor
        /// </summary>
        /// <param name="x">First value to compare</param>
        /// <param name="y">Second value to compare</param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            object a = x.GetType().GetProperty(_propertyName).GetValue(x, null);
            object b = y.GetType().GetProperty(_propertyName).GetValue(y, null);

            if (a != null && b == null)
                return lsd == ListSortDirection.Ascending ? 1 : -1;

            if (a == null && b != null)
                return lsd == ListSortDirection.Ascending ? -1 : 1;

            if (a == null && b == null)
                return 0;

            int ret = ((IComparable)a).CompareTo(b);
            return lsd == ListSortDirection.Ascending ? ret : -ret; ;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;
            if (obj == null)
                return false;
            if (obj.GetType() == this.GetType())
            {
                ObjectPropertyComparer<T> cobj = (ObjectPropertyComparer<T>)obj;
                return cobj._propertyName == _propertyName && cobj.lsd == lsd;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _propertyName.GetHashCode() + lsd.GetHashCode();
        }


        #endregion
    }
}