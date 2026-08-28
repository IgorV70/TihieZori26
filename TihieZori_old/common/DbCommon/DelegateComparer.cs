using System;
using System.Collections.Generic;

namespace DbCommon
{
    public class DelegateComparer<T> : IComparer<T>
    {
        public Comparison<T> _comp;

        /// <summary>
        /// Обертка для делегата
        /// </summary>
        public DelegateComparer(Comparison<T> comp)
        {
            _comp = comp;
        }
        #region Члены IComparer<T>

        public int Compare(T x, T y)
        {
            return _comp(x, y);
        }

        #endregion
    }
}