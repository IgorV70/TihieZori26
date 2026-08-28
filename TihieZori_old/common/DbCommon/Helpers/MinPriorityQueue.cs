using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommon.Helpers
{
    public class MinPriorityQueue<TKey, TValue>
         where TKey : IComparable<TKey>
    {
        private KeyValuePair<TKey, TValue>[] _items;
        private const int MinCapacity = 7;
        private int _count = 0;

        #region Constructors

        public MinPriorityQueue(int initialCapacity)
        {
            _items = new KeyValuePair<TKey, TValue>[initialCapacity];
        }

        public MinPriorityQueue()
        {
            _items = new KeyValuePair<TKey, TValue>[MinCapacity];
        }

        #endregion

        public bool IsEmpty { get { return _count == 0; } }

        public int Count { get { return _count; } }

        public int Capacity { get { return _items.Length; } }

        public TKey MinKey { get { return _items[0].Key; } }

        public TValue MinValue { get { return _items[0].Value; } }

        public KeyValuePair<TKey, TValue> MinItem { get { return _items[0]; } }

        private void Add(KeyValuePair<TKey, TValue> elem)
        {
            if (_count == _items.Length)
                Array.Resize(ref _items, _items.Length * 2);
            _items[_count++] = elem;
        }

        private int FindIndex(TKey key)
        {
            if (_count == 0)
                return 0;
            if (_items[_count - 1].Key.CompareTo(key) < 0)
                return _count;
            int i = 0;
            int j = _count;
            while (i + 1 < j)
            {
                int middle = i + (j - i) / 2;
                if (_items[middle].Key.CompareTo(key) >= 0)
                {
                    j = middle;
                }
                else
                {
                    i = middle;
                }
            }
            return _items[i].Key.CompareTo(key) >= 0 ? i : j;
        }


        public void Enqueue(TKey key, TValue value)
        {
            //            Add(new KeyValuePair<TKey, TValue>(key, value));
            if (_count == _items.Length)
                Array.Resize(ref _items, _items.Length * 2);
            var index = FindIndex(key);
            if (index < _count)
                Array.Copy(_items, index, _items, index + 1, _count - index);
            _items[index] = new KeyValuePair<TKey, TValue>(key, value);
            _count++;
        }

        public KeyValuePair<TKey, TValue> DequeueMin()
        {
            var result = _items[0];
            _count--;
            Array.Copy(_items, 1, _items, 0, _count);
            return result;
        }


        public bool RemoveFirst(Predicate<TValue> match)
        {
            for (var i = 0; i < _count; i++)
            {
                if (!match(_items[i].Value)) continue;
                RemoveAtIndex(i);
                return true;
            }
            return false;
        }

        public KeyValuePair<TKey, TValue>? GetFirst(Predicate<TValue> match)
        {
            foreach (var item in _items)
            {
                if (match(item.Value))
                    return item;
            }
            return null;
        }


        public int RemoveAll(Predicate<TValue> match)
        {
            int remCount = 0;
            while (RemoveFirst(match))
                remCount++;
            return remCount;
        }

        /// <summary>
        /// Removes all elements from the queue
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _count; i++)
                _items[i] = new KeyValuePair<TKey, TValue>();
            _count = 0;
        }

        private void RemoveAtIndex(int index)
        {
            if (index != _count - 1)
                Array.Copy(_items, index + 1, _items, index, _count - index - 1);
            _count--;
            _items[_count] = new KeyValuePair<TKey, TValue>();
        }

    }
}
