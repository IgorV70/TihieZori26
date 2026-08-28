using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommon
{
    public class SortList<T> : IList<T>, ICloneable, IBindingListView, IISortList<T>
         ,INotifyCollectionChanged, INotifyPropertyChanged
     where T : BObject, new()
    {
        private T[] _data = null;
        private int _count = 0;

        protected iTable SourceTable;

        public iTable Table
        {
            get { return SourceTable; }
        }

        CDatabase Database
        {
            get
            {
                return SourceTable.parentDataBase;
            }
        }

        public SortList(iTable t, int capasity)
        {
            _data = new T[capasity];
            SourceTable = t;
        }

        public SortList(iTable t)
        {
            SourceTable = t;
        }

        public SortList(iTable t, IEnumerable<T> eRet)
        {
            SourceTable = t;
            _data = eRet.ToArray();
            _count = _data.Length;
        }

        private bool _listenitems = false;
        public bool ListenItems
        {
            get { return _listenitems; }
            set
            {
                if (value & !_listenitems)
                    for (int i = 0; i < _count; i++)
                        _data[i].PropertyDataChanged += new EventHandler(item_PropertyDataChanged);
                if (!value & _listenitems)
                    for (int i = 0; i < _count; i++)
                        _data[i].PropertyDataChanged -= new EventHandler(item_PropertyDataChanged);
            }
        }


        #region Члены IList<T>

        public int IndexOf(T item)
        {
            int ret = -1;
            if (_data != null)
            {
                ret = Array.IndexOf(_data, item);
                if (ret >= _count) ret = -1;
            }
            return ret;
        }

        public void Insert(int index, T item)
        {
            int len = _data == null ? 0 : _data.Length;
            if (len < _count + 1)
                Array.Resize(ref _data, len + 10);
            Array.Copy(_data, index, _data, index + 1, _count - index);
            _data[index] = item;
            _count++;
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item,index));
            if (ListenItems)
                item.PropertyDataChanged += new EventHandler(item_PropertyDataChanged);
        }

        void item_PropertyDataChanged(object sender, EventArgs e)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOf(sender)));
        }

        public void RemoveAt(int index)
        {
            if (ListenItems)
                _data[index].PropertyDataChanged -= new EventHandler(item_PropertyDataChanged);
            Array.Copy(_data, index + 1, _data, index, _count - index - 1);
            _count--;
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        public T this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        #endregion

        #region Члены ICollection<T>

        public void Add(T item)
        {
            int len = _data == null ? 0 : _data.Length;
            if (len <= _count)
                Array.Resize(ref _data, len + 10);
            _data[_count] = item;
            _count++;
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, _count - 1));
            if (ListenItems)
                item.PropertyDataChanged += new EventHandler(item_PropertyDataChanged);
        }

        public int AddOrder(T item)
        {
            int top = 0;
            int bottom = _count;
            int aver = 0;
            while (top < bottom)
            {
                aver = top + (bottom - top) / 2;
                int order = sortComparer.Compare(item, _data[aver]);
                if (order == 0)
                {
                    return aver;
                }
                if (order < 0) bottom = aver;
                else top = aver + 1;
            }
            Insert(bottom, item);
            return -1;
        }

        public void Clear()
        {
            if (ListenItems)
            {
                for (int i = 0; i < _count; i++)
                    _data[i].PropertyDataChanged -= new EventHandler(item_PropertyDataChanged);
            }
            _count = 0;
            _data = null;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public bool Contains(T item)
        {
            return ((IList<T>)_data).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_data, 0, array, arrayIndex, _count);
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0) return false;
            RemoveAt(index);
            return true;
        }

        #endregion

        #region Члены IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>(_data, _count);
            // return null;
        }

        #endregion

        #region Члены IEnumerable

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ListEnumerator<T>(_data, _count);
        }

        #endregion

        #region Члены ICloneable

        public object Clone()
        {
            SortList<T> ret = new SortList<T>(SourceTable);
            if (_count > 0)
            {
                ret._data = (T[])_data.Clone();
                ret._count = _count;
            }
            return ret;
        }

        #endregion

        // Выполняет фильтрацию последовательности значений на основе заданного предиката.
        public SortList<T> Where(Func<T, bool> predicate)
        {
            SortList<T> ret = new SortList<T>(SourceTable);
            foreach (T rec in this)
                if (predicate(rec))
                    ret.Add(rec);
            return ret;
        }


        // Полное клонирование списка
        public SortList<T> CloneAll(int deep)
        {
            SortList<T> ret = new SortList<T>(SourceTable);
            if (_count > 0)
            {
                ret._data = (T[])_data.Clone();
                ret._count = _count;
                for (int i = 0; i < _count; i++)
                    ret._data[i] = (T)_data[i].DeepClone(deep);
            }
            return ret;
        }

        public T AddNew()
        {
            T ret = new T();
            ret._table = SourceTable;
            this.Add(ret);
            if (ListenItems)
                ret.PropertyDataChanged += new EventHandler(item_PropertyDataChanged);
            return ret;
        }


        #region Члены IBindingList

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        object IBindingList.AddNew()
        {
            return AddNew();
        }

        public bool AllowEdit
        {
            get { return true; }
        }

        public bool AllowNew
        {
            get { return true; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            Sort(new ObjectPropertyComparer<T>(property.Name, direction));
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        IComparer<T> sortComparer = null;
        public void SetSortComparer(IComparer<T> cmp)
        {
            sortComparer = cmp;
        }

        public void SetSortComparer(string Expression)
        {
            string[] SplittedExpr = Expression.Split(',');
            ListSortDirection lsd = ListSortDirection.Ascending;
            if (SplittedExpr.Length > 1)
            //throw new NotImplementedException("реализуем если очень будет нужно :)");
            {
                ListSortDesc[] sorts = new ListSortDesc[SplittedExpr.Length];
                Type thisType = GetType();
                int i = 0;
                foreach (string sd in SplittedExpr)
                {
                    string[] sd2 = SplittedExpr[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    lsd = ListSortDirection.Ascending;
                    if (sd2.Length > 1)
                        if (sd2[1].ToUpper() == "DESC")
                            lsd = ListSortDirection.Descending;
                    sorts[i] = new ListSortDesc(thisType.GetProperty(sd2[0]), lsd);
                }
                SetSortComparer(new ObjectPropertyListComparer<T>(sorts));
                return;
            }

            string[] PropertyName2 = SplittedExpr[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (PropertyName2.Length > 1)
                if (PropertyName2[1].ToUpper() == "DESC")
                    lsd = ListSortDirection.Descending;

            SetSortComparer(new ObjectPropertyComparer<T>(PropertyName2[0], lsd));
        }

        bool IBindingList.IsSorted
        {
            get { return sortComparer != null; }
        }


        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            sortComparer = null;
        }

        ListSortDirection IBindingList.SortDirection
        {
            get
            {
                return sortComparer == null ? ListSortDirection.Ascending :
                    sortComparer is ObjectPropertyComparer<T> ? ((ObjectPropertyComparer<T>)sortComparer).lsd : ListSortDirection.Ascending;
            }
        }

        PropertyDescriptor sortProperty = null;

        PropertyDescriptor IBindingList.SortProperty
        {
            get { return sortProperty; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public bool SupportsSorting
        {
            get { return true; }
        }

        #endregion

        #region Члены IList

        public int Add(object value)
        {
            this.Add((T)value);
            return _count - 1;
        }

        public bool Contains(object value)
        {
            return ((IList<T>)_data).Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return Array.IndexOf(_data, (T)value);
        }

        public void Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.Remove((T)value);
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return _data[index];
            }
            set
            {
                _data[index] = (T)value;
            }
        }

        #endregion

        #region Члены ICollection

        public void CopyTo(Array array, int index)
        {
            Array.Copy(_data, 0, array, index, _count);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region Члены IBindingListView

        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            Sort(new ObjectPropertyListComparer<T>(sorts));
        }

        public string Filter
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void RemoveFilter()
        {
            throw new NotImplementedException();
        }

        public ListSortDescriptionCollection SortDescriptions
        {
            get
            {
                //if (sortComparer != null && sortComparer is ObjectPropertyListComparer<T>)
                //    return ((ObjectPropertyListComparer<T>)sortComparer)._sorts;
                return null;
            }
        }

        public bool SupportsAdvancedSorting
        {
            get { return true; }
        }

        public bool SupportsFiltering
        {
            get { return false; }
        }

        #endregion

        #region Члены iSortList

        public void Add(BObject obj)
        {
            this.Add((T)obj);
        }


        public BObject Find(Predicate<BObject> pr)
        {
            for (int i = 0; i < _count; i++)
            {
                BObject item = (BObject)this[i];
                if (pr.Invoke(item)) return item;
            }
            return null;
        }

        public iSortList FindAll(Predicate<BObject> pr)
        {
            var ret = new SortList<T>(SourceTable, _count);
            for (int i = 0; i < _count; i++)
            {
                T item = this[i];
                if (pr(item))
                    ret.Add(item);
            }
            return ret;
        }


        public void WriteAll(BinaryWriter bw)
        {
            if (this.IsFiltering)
                this.funFilter = null;
            bw.Write(typeof(T).Name);
            bw.Write(this.Count);
            foreach (T obj in this)
                ((BObject)obj)._write(bw);
        }

        //public void ChangeLang(string mnem)
        //{
        //    foreach (T obj in this)
        //        ((BlangObject)(Object)obj).CurrentLang = mnem;
        //}

        public int GetCount()
        {
            return this.Count();
        }

        public string TableName()
        {
            return SourceTable.TableName();
        }

        public void Remove(BObject obj)
        {
            this.Remove((T)obj);

        }
        #endregion


        public void AddRange(IEnumerable<T> collection)
        {
            int addCount = collection.Count();
            int len = (_data == null ? 0 : _data.Length);
            if (len < _count + addCount)
                Array.Resize(ref _data, len + addCount);
            int i = 0;
            foreach (T item in collection)
                _data[_count + i++] = item;
            _count += addCount;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            if (ListenItems)
                foreach (T item in collection)
                    item.PropertyDataChanged += new EventHandler(item_PropertyDataChanged);
        }

        private bool _IsFiltering = false;
        public bool IsFiltering
        {
            get { return _IsFiltering; }
            set { _IsFiltering = value; }
        }

        private SortList<T> deleteditems; // clone de la Collection Cre juste aprs le chargement de la base de donnes
        public SortList<T> DeletedItems
        {
            get
            {
                if (deleteditems == null)
                    deleteditems = new SortList<T>(SourceTable);
                return deleteditems;
            }
            set
            {
                deleteditems = value;
            }
        }

        private string filter = string.Empty;
        internal SortList<T> filtreditems; // Collection contenant les lments filtrs (Supprims);
        public SortList<T> FiltredItems
        {
            get
            {
                if (filtreditems == null)
                    filtreditems = new SortList<T>(SourceTable);
                return filtreditems;
            }
            set
            {
                filtreditems = value;
            }

        }

        public IFilter<T> funFilter
        {
            set
            {
                Lock();
                //if (value == filter) return;
                this.IsFiltering = false;
                //2 - восстановление коллекции
                this.AddRange(this.FiltredItems);
                this.FiltredItems.Clear();

                this.IsFiltering = true;
                if (value != null)
                {

                    for (int i = 0; i < this.Count; i++)
                    {
                        T item = this[i];
                        if (!value.Test(item))
                        {
                            this.RemoveAt(i--);
                            this.FiltredItems.Add(item);
                        }
                    }
                }
                UnLock();
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        public event ListChangedEventHandler ListChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnListChanged(ListChangedEventArgs ev)
        {
            if (_lockCounter == 0)
                ListChanged?.Invoke(this, ev);
        }

        public void Sort(ListSortDesc[] sorts)
        {
            Sort(new ObjectPropertyListComparer<T>(sorts));
        }

        public void Sort(FieldDescription[] fields)
        {
            ListSortDesc[] sorts = new ListSortDesc[fields.Length];
            for (int i = 0; i < sorts.Length; i++)
                sorts[i] = new ListSortDesc(fields[i].PropInfo, ListSortDirection.Ascending);
            Sort(new ObjectPropertyListComparer<T>(sorts));
        }

        public void Sort(string Expression)
        {
            string[] SplittedExpr = Expression.Split(',');
            ListSortDirection lsd = ListSortDirection.Ascending;
            if (SplittedExpr.Length > 1)
            //throw new NotImplementedException("реализуем если очень будет нужно :)");
            {
                ListSortDesc[] sorts = new ListSortDesc[SplittedExpr.Length];
                Type thisType = typeof(T);
                int i = 0;
                foreach (string sd in SplittedExpr)
                {
                    string[] sd2 = sd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    lsd = ListSortDirection.Ascending;
                    if (sd2.Length > 1)
                        if (sd2[1].ToUpper() == "DESC")
                            lsd = ListSortDirection.Descending;
                    sorts[i] = new ListSortDesc(thisType.GetProperty(sd2[0]), lsd);
                    i++;
                }
                Sort(new ObjectPropertyListComparer<T>(sorts));
                return;
            }

            string[] PropertyName2 = SplittedExpr[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (PropertyName2.Length > 1)
                if (PropertyName2[1].ToUpper() == "DESC")
                    lsd = ListSortDirection.Descending;

            Sort(new ObjectPropertyComparer<T>(PropertyName2[0], lsd));
        }

        public void Sort(IComparer<T> comparer)
        {
            if (comparer.Equals(sortComparer))
                return;
            sortComparer = comparer;
            if (_data != null)
                Array.Sort(_data, 0, _count, sortComparer);

            //OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            //DataGridViev неправильно работает если вызывать
        }

        public T Find(Predicate<T> match)
        {
            if (_data == null) return null;
            for (int i = 0; i < _count; i++)
                if (match(_data[i])) return _data[i];
            return null;
        }

        public bool Exists(Predicate<T> match)
        {
            if (_data == null) return false;
            for (int i = 0; i < _count; i++)
                if (match(_data[i])) return true;
            return false;
        }

        public void Save()
        {
            string messages = "";
            for (int i = 0; i < this.Count; i++)
            {
                T item = this[i];
                try
                {
                    item.Save2();
                    if (!item.IsPersistent)
                        this.RemoveAt(i--);
                }
                catch (BOSaveExeption bex)
                {
                    messages += "\n" + bex.Message;
                    if (item.ObjectState == BObject.ObjectStateType.Deleted)
                        item.Rollback();
                }
            }
            if (this.filtreditems != null)
                try
                {
                    filtreditems.Save();
                }
                catch (BOSaveExeption bex)
                {
                    messages += "\n" + bex.Message;
                }
            if (!string.IsNullOrEmpty(messages))
                throw new BOSaveExeption(messages);

            if (this.deleteditems != null)
                try
                {
                    deleteditems.Save();
                }
                catch (BOSaveExeption bex)
                {
                    messages += "\n" + bex.Message;
                }
            if (!string.IsNullOrEmpty(messages))
                throw new BOSaveExeption(messages);
            else
                this.deleteditems = null;
        }

        internal void DeleteAll()
        {
            this.DeletedItems.AddRange(this);
            this.Clear();
            if (filtreditems != null)
            {
                this.DeletedItems.AddRange(filtreditems);
                filtreditems.Clear();
            }
            foreach (T item in this.DeletedItems)
                item.MarkForDeletion();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// Сохранение данных с диалогом об ошибке
        /// </summary>
        public bool SaveTry()
        {
            try
            {
                this.Save();
                return true;
            }
            catch (BOSaveExeption ex)
            {
                ex.Show();
                return false;
            }
            catch (System.Exception ex)
            {
                Database.SaveLogString(new string[] { 
                    "Message:" + ex.Message,
                    "Source:" + ex.Source,
                    "StackTrace:" + ex.StackTrace,
                    "TargetSite:" + ex.TargetSite
                });
                if (BObject.ShowErrorMessage != null)
                    BObject.ShowErrorMessage(ex.Message);
                return false;
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            Sort(new DelegateComparer<T>(comparison));
        }

        public SortList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
            where TOutput : BObject, new()
        {
            SortList<TOutput> ret = new SortList<TOutput>(SourceTable, _count);
            for (int i = 0; i < _count; i++)
                ret._data[i] = converter.Invoke(_data[i]);
            ret._count = this._count;
            return ret;
        }

        public void DeleteObject(T obj)
        {
            if (obj == null) return;
            int index = IndexOf(obj);
            if (index < 0) return;
            this.RemoveAt(index);
            obj.MarkForDeletion();
            if (obj.IsPersistent)
                this.DeletedItems.Add(obj);
        }

        public void DeleteObject(int index)
        {
            T obj = this[index];
            this.RemoveAt(index);
            if (obj == null) return;
            obj.MarkForDeletion();
            if (obj.IsPersistent)
                this.DeletedItems.Add(obj);
        }


        /// <summary>
        /// проверяем есть ли в списке модифицированные записи
        /// </summary>
        /// <returns></returns>
        public BObject.ObjectStateType ObjectListState()
        {
            if (deleteditems != null)
                if (deleteditems._count > 0)
                    return BObject.ObjectStateType.Changed;
            if (this.Any(obj => obj.ObjectState != BObject.ObjectStateType.Unchanged))
            {
                return BObject.ObjectStateType.Changed;
            }
            if (filtreditems == null) return BObject.ObjectStateType.Unchanged;
            return filtreditems.Any(obj => obj.ObjectState != BObject.ObjectStateType.Unchanged) 
                ? BObject.ObjectStateType.Changed : BObject.ObjectStateType.Unchanged;
        }

        //public Expression<Func<T, bool>> Expression = null;
        public void Rollback()
        {
            Lock();
            if (deleteditems != null)
            {
                AddRange(deleteditems);
                deleteditems.Clear();
            }
            if (filtreditems != null)
            {
                AddRange(filtreditems);
                filtreditems.Clear();
            }
            T[] d = _data;
            for (int i = 0; i < _count; i++)
            {
                T item = d[i];
                if (!item.IsPersistent)
                    Remove(item);
                else
                    item.Rollback();
            }
            if (sortComparer != null)
                Sort(sortComparer);
            UnLock();
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        int _lockCounter = 0;

        private void UnLock()
        {
            _lockCounter--;
        }

        private void Lock()
        {
            _lockCounter++;
        }


        //public void DeleteObject()
        //{
        //    throw new NotImplementedException();
        //}

    }
}
