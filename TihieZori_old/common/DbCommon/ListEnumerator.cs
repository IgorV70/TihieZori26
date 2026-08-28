using System.Collections;
using System.Collections.Generic;

namespace DbCommon
{
    public class ListEnumerator<T> : IEnumerator<T>
        where T : BObject
    {
        public ListEnumerator()
        { }
        T[] _data;
        int _count;
        int _current = -1;
        public ListEnumerator(T[] data, int count)
        {
            _data = data;
            _count = count;
        }

        #region Члены IEnumerator<T>

        public T Current
        {
            get { return _data[_current]; }
        }

        #endregion

        #region Члены IDisposable

        public void Dispose()
        {

        }

        #endregion

        #region Члены IEnumerator

        object IEnumerator.Current
        {
            get { return _data[_current]; }
        }

        public bool MoveNext()
        {
            _current++;
            return _current < _count;
        }

        public void Reset()
        {
            _current = -1;
        }

        #endregion
    }


}