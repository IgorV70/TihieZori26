using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DbCommon.Attributes;
using DbCommon.Enums;
using DbCommon.Helpers;

namespace DbCommon
{
    abstract public class CTable<T, TDatabase, TPKey> : iTable
        where T : BObject, new()
        where TDatabase : CDatabase
    {
        protected Dictionary<string, FieldDescription> _fd = new Dictionary<string, FieldDescription>();
        public CTable()
        {
        }

        private TDatabase _parentDataBase = null;

        protected SortList<T> Cash = null;
        protected Dictionary<TPKey, T> Idcash = null;

        #region Члены iTable

        public void ClearCash()
        {
            lock (this)
            {
                if (Cash != null)
                    Cash.Clear();
                if (Idcash != null)
                    Idcash.Clear();
                _cashDateTime = 0;
            }
        }

        private bool _isChangesLog = false;
        /// <summary>
        /// Признак логирования изменений по таблице
        /// </summary>
        public bool IsChangesLog
        {
            get
            {
                return _isChangesLog;
            }
            set
            {
                _isChangesLog = value;
            }
        }

        #endregion

        private long _cashDateTime = 0;

        public CasheType Ct
        {
            get
            {
                if (_ct == CasheType.Empty)
                    _ct = GetCashType();
                return _ct;
            }
            private set { _ct = value; }
        }

        public CTable(TDatabase db)
        {
            _parentDataBase = db;
        }

        protected void CashInit()
        {
            if (typeof(T).GetInterface(typeof(IPrimaryKey<TPKey>).Name) != null)
                Idcash = new Dictionary<TPKey, T>();
            else
            {
                // Инициализируем порядок в массиве
                Cash = new SortList<T>(this);
                Cash.Sort(GetPrimaryKeysList().ToArray());
            }
        }


        CDatabase iTable.parentDataBase
        {
            get
            {
                return _parentDataBase;
            }
            set
            {
                _parentDataBase = (TDatabase)value;
            }
        }

        public TDatabase parentDataBase
        {
            get
            {
                return _parentDataBase;
            }
            set
            {
                _parentDataBase = value;
            }
        }

        public IEnumerable<FieldDescription> GetLangPropertyList()
        {
            return _fd.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.MultiLang) != 0);
        }

        public IEnumerable<FieldDescription> GetPrimaryKeysList()
        {
            return _fd.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.PrimaryKey) != 0);
        }

        public FieldDescription GetIdentity()
        {
            return _fd.Values.FirstOrDefault(fd => (fd.Properties & FieldDescription.fieldProp.Identity) != 0);
        }

        public int GetTotal(Type t)
        {
            return parentDataBase.GetTotal(this);
        }


        public virtual CasheType GetCashType()
        {
            return CasheType.QueryResultsCash;
        }

        protected FieldDescription[] _fieldDescription;
        public UInt64 lastTS = 0;

        public virtual string TablePrefix()
        {
            return "";
        }

        public virtual string TableName()
        {
            return TablePrefix() + typeof(T).Name;
        }

        public virtual string TableName_select()
        {
            return this.TableName();
        }

        public virtual string OwnerName()
        {
            return "dbo";
        }

        public SortList<T> EmptySortList()
        {
            return new SortList<T>(this);
        }

        /// <summary>
        /// Результат непустой - нехер его на null проверять !
        /// </summary>
        /// <returns>Список строк</returns>
        public virtual SortList<T> GetAll()
        {
            CasheType ct = this.GetCashType();
            switch (ct)
            {
                case CasheType.DoNotCache:
                    return (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this));
                //case CasheType.QueryResultsCash:
                //case CasheType.EntireTableCach:
                default:
                    {
                        lock (this)
                        {
                            if (Idcash == null && Cash == null)
                                CashInit();
                            if (Idcash != null)
                            {
                                int count = _parentDataBase.GetTotal(this);
                                if (count == Idcash.Count)
                                    return new SortList<T>(this, Idcash.Values);
                            }
                            else
                            //if (Cash != null)
                            {
                                int count = _parentDataBase.GetTotal(this);
                                // ReSharper disable once PossibleNullReferenceException
                                if (count == Cash.Count)
                                    return (SortList<T>)Cash.Clone();
                            }
                            _cashDateTime = DateTime.Now.Ticks;
                            SortList<T> ret = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this));
                            _updateCash(ret);
                            return ret;
                        }
                    }

            }
        }

        public BObject GetById(object id)
        {
            return (BObject)GetById((TPKey)id);
        }

        public T GetById(TPKey id)
        {
            var primary =
                _fd.Values.Single(fd => (fd.Properties & FieldDescription.fieldProp.PrimaryKey) > 0);

            Ct = this.GetCashType();
            switch (Ct)
            {
                case CasheType.DoNotCache:
                    {
                        SortList<T> retlist = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, primary.Name + "=" + QuaterId(id), 1));
                        return retlist.Count > 0 ? retlist[0] : null;
                    }
                case CasheType.QueryResultsCash:
                    lock (this)
                    {
                        if (Idcash == null)
                            CashInit();
                        try
                        {
                            return Idcash[id];
                        }
                        catch
                        {
                            SortList<T> retlist = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, primary.Name + "=" + QuaterId(id), 1));
                            var ret = retlist.Count > 0 ? retlist[0] : null;
                            if (ret == null) return null;
                            lock (this)
                            {
                                Idcash[id] = ret;
                            }
                            return ret;
                        }
                    }
                case CasheType.EntireTableCach:
                    lock (this) // предотвращаем паралельный запрос данных из одной таблицы
                    {
                        if (_cashDateTime == 0)
                        {
                            _cashDateTime = DateTime.Now.Ticks;
                            _updateCash((SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this)));
                        }
                    }
                    try
                    {
                        return Idcash[id];
                    }
                    catch { return null; }
            }
            throw new NotImplementedException(); // сюда не придем
        }

        private string QuaterId(TPKey id) => id is Guid ? $"'{id}'" : id.ToString();


        Type _rowType = typeof(T);
        private CasheType _ct = CasheType.Empty;

        public virtual Type RowType
        {
            get { return _rowType; }
            set
            {
                if (typeof(T).IsAssignableFrom(value))
                    _rowType = value;
                else
                    throw new Exception("Ошибка, можно использовать только производный тип!");
            }
        }

        public T NewInstance()
        {
            //T ret = new T();
            T ret = (T)Activator.CreateInstance(RowType);
            ret._table = this;
            return ret;
        }

        #region Члены iTable

        BObject iTable.CreateInstance()
        {
            return NewInstance();
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <returns></returns>
        public Type GetRowType()
        {
            return typeof(T);
        }


        public iSortList CreateSortList()
        {
            Type generic = typeof(SortList<>);
            Type tlist = generic.MakeGenericType(new Type[] { typeof(T) });
            return (iSortList)Activator.CreateInstance(tlist, new object[] { this });
        }


        public ulong LastTS
        {
            get
            {
                return lastTS;
            }
            set
            {
                lastTS = value;
            }
        }

        /// <summary>
        /// Возвращает имена полей, разделенные запятыми, с завершающей запятой
        /// </summary>
        public string GetFieldStringList(FieldDescription.fieldProp fProp)
        {
            StringBuilder sbRet = new StringBuilder();
            foreach (FieldDescription f in Columns.Values)
            {
                if ((f.Properties & fProp) > 0)
                {
                    sbRet.Append(f.Name);
                    sbRet.Append(",");
                }
            }
            return sbRet.ToString();
        }



        public string GetFieldStringListU(FieldDescription.fieldProp fieldProp)
        {
            StringBuilder sbRet = new StringBuilder();
            foreach (FieldDescription f in Columns.Values)
            {
                if ((f.Properties & fieldProp) > 0)
                {
                    sbRet.Append("lc.");
                    sbRet.Append(f.Name);
                    sbRet.Append("=i.");
                    sbRet.Append(f.Name);
                    sbRet.Append(",");
                }
            }
            return sbRet.ToString();

        }

        #endregion

        public T GetObjectByExpr(Expression<Func<T, bool>> expr)
        {
            return GetObjectByExpr(expr, null);
        }

        public T GetObjectByExpr(Expression<Func<T, bool>> expr, string OrderBy)
        {
            return GetObjectByExpr(expr, OrderBy, null);
        }


        public T GetObjectByExpr(Expression<Func<T, bool>> expr, string OrderBy, string With)
        {
            switch (this.GetCashType())
            {
                case CasheType.DoNotCache:
                    {
                        string sFiltr = _parentDataBase.GetWhereFromExpr(expr.Body);
                        SortList<T> retList = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, 1));
                        return retList.Count > 0 ? retList[0] : null;
                    }
                case CasheType.EntireTableCach:
                    {
                        lock (this) // предотвращаем паралельный запрос данных из одной таблицы
                        {
                            if (Idcash == null && Cash == null)
                            {
                                _cashDateTime = DateTime.Now.Ticks;
                                _updateCash((SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this)));
                            }
                            if (Idcash != null)
                            {
                                T ret = Idcash.Values.FirstOrDefault(expr.Compile());
                                if (ret != null)
                                    return ret;
                            }
                            return Cash != null ? Cash.FirstOrDefault(expr.Compile()) : null;
                        }
                    }
                default:
                    {
                        lock (this)
                        {
                            if (Idcash != null)
                            {
                                T ret = Idcash.Values.FirstOrDefault(expr.Compile());
                                if (ret != null)
                                    return ret;
                            }
                            if (Cash != null)
                            {
                                T ret = Cash.FirstOrDefault(expr.Compile());
                                if (ret != null)
                                    return ret;
                            }
                            _cashDateTime = DateTime.Now.Ticks;
                            string sFiltr = _parentDataBase.GetWhereFromExpr(expr.Body);
                            SortList<T> retList = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, 1));
                            _updateCash(retList);
                            return retList.Count > 0 ? retList[0] : null;
                        }
                    }
            }
        }


        public SortList<T> GetObjectListByExpr(Expression<Func<T, bool>> expr, string OrderBy = null, string With = null, int RowCount = 0)
        {
            CasheType ct;
            if (_parentDataBase.ConnectionMode != ConnectionModes.DatabaseConnection)
            {
                ct = CasheType.EntireTableCach;
                _cashDateTime = long.MaxValue;
            }
            else
                ct = this.GetCashType();
            switch (ct)
            {
                case CasheType.DoNotCache:
                    {
                        string sFiltr = _parentDataBase.GetWhereFromExpr(expr.Body);
                        return (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, RowCount));
                    }
                case CasheType.QueryResultsCash:
                    {
                        lock (this)
                        {
                            string sFiltr = _parentDataBase.GetWhereFromExpr(expr.Body);
                            if (_cashDateTime == 0)
                            {
                                _cashDateTime = DateTime.Now.Ticks;
                                SortList<T> ret = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, RowCount));
                                _updateCash(ret);
                                return ret;
                            }
                            else
                            {
                                int count = _parentDataBase.GetCount(this, sFiltr);
                                SortList<T> ret = null;
                                if (Idcash != null)
                                {
                                    var eRet = Idcash.Values.Where(expr.Compile()).ToList();
                                    if (count == eRet.Count)
                                        ret = new SortList<T>(this, eRet);
                                }
                                else
                                    if (Cash != null)
                                {
                                    var eRet = Cash.Where(expr.Compile()).ToList();
                                    if (count == eRet.Count)
                                        ret = new SortList<T>(this, eRet);
                                }
                                if (ret != null)
                                {
                                    if (OrderBy != null) ret.Sort(OrderBy);
                                    return ret;
                                }
                                ret = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, RowCount));
                                _updateCash(ret);
                                return ret;
                            }
                        }
                    }
                case CasheType.EntireTableCach:
                    {
                        lock (this) // предотвращаем паралельный запрос данных из одной таблицы
                        {
                            if (_cashDateTime == 0)
                            {
                                _cashDateTime = DateTime.Now.Ticks;
                                _updateCash((SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, (string)null, OrderBy)));
                            }
                            SortList<T> ret = Cash.Where(expr.Compile());
                            if (OrderBy != null) ret.Sort(OrderBy);
                            return ret;
                        }
                    }
                default:
                    break;
            }
            throw new NotFiniteNumberException();
        }


        public SortList<T> GetObjectListByCustom(ICustomFiltr customFiltr, string OrderBy = null, string With = null, int RowCount = 0)
        {
            CasheType ct;
            if (_parentDataBase.ConnectionMode != ConnectionModes.DatabaseConnection)
            {
                ct = CasheType.EntireTableCach;
                _cashDateTime = long.MaxValue;
            }
            else
                ct = this.GetCashType();
            switch (ct)
            {
                case CasheType.DoNotCache:
                    {
                        string sFiltr = customFiltr.WhereTerm(_parentDataBase.DatabaseType, "t");
                        return (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, RowCount));
                    }
                case CasheType.QueryResultsCash:
                    {
                        lock (this)
                        {
                            string sFiltr = customFiltr.WhereTerm(_parentDataBase.DatabaseType, "t");
                            if (_cashDateTime == 0)
                            {
                                _cashDateTime = DateTime.Now.Ticks;
                                SortList<T> ret = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, RowCount));
                                _updateCash(ret);
                                return ret;
                            }
                            else
                            {
                                int count = _parentDataBase.GetCount(this, sFiltr);
                                SortList<T> ret = null;
                                if (Idcash != null)
                                {
                                    var eRet = Idcash.Values.Where(customFiltr.Predicate()).Cast<T>().ToList();
                                    if (count == eRet.Count)
                                        ret = new SortList<T>(this, eRet);
                                }
                                else
                                    if (Cash != null)
                                {
                                    var eRet = Cash.Where(customFiltr.Predicate()).ToList();
                                    if (count == eRet.Count)
                                        ret = new SortList<T>(this, eRet);
                                }
                                if (ret != null)
                                {
                                    if (OrderBy != null) ret.Sort(OrderBy);
                                    return ret;
                                }
                                ret = (SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this, sFiltr, OrderBy, With, RowCount));
                                _updateCash(ret);
                                return ret;
                            }
                        }
                    }
                case CasheType.EntireTableCach:
                    {
                        lock (this) // предотвращаем паралельный запрос данных из одной таблицы
                        {
                            if (_cashDateTime == 0)
                            {
                                _cashDateTime = DateTime.Now.Ticks;
                                _updateCash((SortList<T>)_parentDataBase.ReadObjectList(new CQueryDesc(this)));
                            }
                            if (Idcash != null)
                            {
                                var predicate = customFiltr.Predicate();
                                SortList<T> ret0 = new SortList<T>(this, Idcash.Values.Where(tobj => predicate(tobj)));
                                if (OrderBy != null) ret0.Sort(OrderBy);
                                return ret0;
                            }
                            SortList<T> ret = Cash.Where(customFiltr.Predicate());
                            if (OrderBy != null) ret.Sort(OrderBy);
                            return ret;
                        }
                    }
                default:
                    break;
            }
            throw new NotFiniteNumberException();
        }

        public void UpdateCash(SortList<T> ret2)
        {
            lock (this)
            {
                _updateCash(ret2);
            }
        }

        /// <summary>
        /// Добавляет в кеш свежепрочитанные записи
        /// </summary>
        /// <param name="ret2"></param>
        private void _updateCash(SortList<T> ret2)
        {
            lock (this)
            {
                if (Cash == null && Idcash == null)
                    CashInit();
                List<T> retList = new List<T>();
                bool retChange = false;
                if (Idcash != null)
                {
                    foreach (T obj in ret2)
                    {
                        try
                        {
                            Idcash.Add(((IPrimaryKey<TPKey>)obj).Id, obj);
                            //_cash.Add(obj);
                            retList.Add(obj);
                        }
                        catch (ArgumentException)
                        {
                            T obj0 = Idcash[((IPrimaryKey<TPKey>)obj).Id];
                            retList.Add(obj0);
                            if (obj0.ObjectState == BObject.ObjectStateType.Unchanged)
                                obj0.RestoreFrom(obj);
                            retChange = true;
                        }
                    }
                }
                else
                {
                    foreach (T obj in ret2)
                    {
                        int index = Cash.AddOrder(obj);
                        if (index >= 0)
                        {
                            T obj0 = Cash[index];
                            retList.Add(obj0);
                            if (obj0.ObjectState == BObject.ObjectStateType.Unchanged)
                                obj0.RestoreFrom(obj);
                            retChange = true;
                        }
                        else
                        {
                            retList.Add(obj);
                        }
                    }
                    if (retChange)
                    {
                        ret2.Clear();
                        ret2.AddRange(retList);
                    }
                }
            }
        }


        #region Члены iTable

        public void Save2(BObject bObject)
        {
            BObject.ObjectStateType st = bObject.ObjectState;
            try
            {
                this.parentDataBase.Save2(bObject);
            }
            catch (SqlException ex)
            {
                Log.Trace("Sql error :" + ex.Message);
                throw;
            }
            switch (st)
            {
                case BObject.ObjectStateType.Added:
                    switch (Ct)
                    {
                        case CasheType.DoNotCache:
                            break;
                        case CasheType.QueryResultsCash:
                            lock (this)
                            {
                                SortList<T> list = new SortList<T>(this);
                                list.Add(bObject);
                                _updateCash(list);
                            }
                            break;
                        case CasheType.EntireTableCach:
                            lock (this)
                            {
                                SortList<T> list = new SortList<T>(this);
                                list.Add(bObject);
                                _updateCash(list);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case BObject.ObjectStateType.Deleted:
                    switch (Ct)
                    {
                        case CasheType.DoNotCache:
                            break;
                        case CasheType.QueryResultsCash:
                            if (Cash != null) Cash.Remove(bObject);
                            if (Idcash != null) Idcash.Remove(((IPrimaryKey<TPKey>)bObject).Id);
                            break;
                        case CasheType.EntireTableCach:
                            if (Cash != null) Cash.Remove(bObject);
                            if (Idcash != null) Idcash.Remove(((IPrimaryKey<TPKey>)bObject).Id);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }




        public iSortList ReadSortList(BinaryReader br)
        {
            int count = br.ReadInt32();
            SortList<T> ret = new SortList<T>(this);
            for (int i = 0; i < count; i++)
            {
                T obj = NewInstance();
                obj._read(br);
                ret.Add(obj);
            }
            return ret;
        }



        public Type GetTableType()
        {
            return typeof(T);
        }

        public void Create() => Create2(false);

        public void Create2(bool startFilling = true)
        {
            _parentDataBase.ExecCmd(_parentDataBase.Dialect.CreateTable(this));
            if (startFilling)
                RestorePredefined();
        }

        public void Truncate()
        {
            _parentDataBase.ExecCmd(_parentDataBase.Dialect.TruncateTable(this));
        }

        public void RestorePredefined()
        {
            var tableType = GetType();
            foreach (var method in tableType.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                StartValueAttribute a = method.GetCustomAttribute<StartValueAttribute>();
                if (a != null)
                {
                    object[] parameters = method.GetParameters().Length == 0 ? new object[] { } : new object[] { _parentDataBase };
                    object retval = method.Invoke(null, parameters);
                    IEnumerable<T> PredefinedList = null;
                    switch (retval)
                    {
                        case IEnumerable<T> pl: PredefinedList = pl; break;
                        case IEnumerable<object> pl: PredefinedList = pl.Cast<T>(); break;
                        case T row: PredefinedList = new T[] { row }; break;
                        default: continue;
                    }
                    foreach (var o in PredefinedList)
                    {
                        var inst = NewInstance();
                        inst.RestoreFrom(o);
                        inst.ObjectState = BObject.ObjectStateType.Added;
                        inst.Save2();
                    }
                }
            }
        }


        public bool Exists()
        {
            var res = _parentDataBase.GetScalar(_parentDataBase.Dialect.TableExists(this));
            try
            {
                return Convert.ToBoolean(res);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateTable(string DatabaseName)
        {
            StringBuilder createSql = new StringBuilder();
            createSql.AppendFormat("CREATE TABLE [{0}].[{1}].[{2}](", DatabaseName, OwnerName(), TableName());
            var fds = Columns.Values;
            foreach (var fieldDescription in fds)
            {
                createSql.AppendLine();
                createSql.AppendFormat("[{0}] [{1}] ", fieldDescription.Name, fieldDescription.SqlType);
                if ((fieldDescription.Properties & FieldDescription.fieldProp.Identity) > 0)
                    createSql.Append("IDENTITY(1,1) ");
                if (fieldDescription.Size > 0 && fieldDescription.Size2 > 0)
                    createSql.AppendFormat("({0},{1}) ", fieldDescription.Size, fieldDescription.Size2);
                else if (fieldDescription.Size > 0)
                    createSql.AppendFormat("({0}) ", fieldDescription.Size);
                createSql.Append((fieldDescription.Properties & FieldDescription.fieldProp.Nullable) > 0
                    ? "NULL "
                    : "NOT NULL ");
                createSql.Append(",");
            }
            var primary = fds.Where(fd => (fd.Properties & FieldDescription.fieldProp.PrimaryKey) > 0).ToList();
            if (primary.Any())
            {
                createSql.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ", TableName());
                createSql.AppendFormat("(");
                foreach (var fieldDescription in primary)
                {
                    createSql.AppendFormat("[{0}] ASC,", fieldDescription.Name);
                }
                createSql.Length--;
                createSql.AppendLine();
                createSql.AppendLine(") WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
            }
            else
                createSql.Length--;

            createSql.AppendLine(") ON [PRIMARY]");

            try
            {
                _parentDataBase.ExecCmd(createSql.ToString());
                if (!string.IsNullOrEmpty(DatabaseName))
                    _parentDataBase.ExecCmd(string.Format("create view [{1}].[{2}] as select * from [{0}].[{1}].[{2}]",
                        DatabaseName, OwnerName(), TableName()));
            }
            catch (Exception ex)
            {
                var sqlEx = ex as SqlException;
                if (sqlEx != null && sqlEx.Number == 2714)
                    return true;
                Log.Error(ex, "CreateTable");
                return false;
            }


            return true;
        }

        #endregion


        public int GetCount(string filtr)
        {
            return _parentDataBase.GetCount(this, filtr);
        }


        public iSortList GetObjectList(CQueryDesc queryPager)
        {
            CasheType ct = this.GetCashType();
            switch (ct)
            {
                case CasheType.DoNotCache:
                    {
                        return (SortList<T>)_parentDataBase.ReadObjectList(queryPager);
                    }
                case CasheType.QueryResultsCash:
                case CasheType.EntireTableCach:
                    {
                        lock (this)
                        {
                            _cashDateTime = DateTime.UtcNow.Ticks;
                            SortList<T> ret = (SortList<T>)_parentDataBase.ReadObjectList(queryPager);
                            _updateCash(ret);
                            return ret;
                        }
                    }
                default:
                    break;
            }
            throw new NotFiniteNumberException();
        }

        private Dictionary<string, Type> _customFiltres = new Dictionary<string, Type>();
        private bool identityInsert;

        public void AddCustomFiltr(string name, Type customFiltrType)
        {
            if (!typeof(ICustomFiltr).IsAssignableFrom(customFiltrType))
                throw new ArgumentException("второй параметр должен быть типом поддерживающим ICustomFiltr");
            _customFiltres[name] = customFiltrType;
        }

        public ICustomFiltr GetCustomFiltr(string name, string[] @params)
        {
            Type customFiltrType = _customFiltres[name];
            return (ICustomFiltr)Activator.CreateInstance(customFiltrType, new object[] { @params });
        }

        iSortList iTable.GetObjectListByCustom(ICustomFiltr customFiltr, string OrderBy, string With, int RowCount)
        {
            return GetObjectListByCustom(customFiltr, OrderBy, With, RowCount);
        }

        public string Name
        {
            get { return TableName(); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<string, FieldDescription> Columns
        {
            get { return _fd; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IdentityInsert
        {
            get
            {
                return identityInsert;
            }
            set
            {
                identityInsert = value;
                _parentDataBase.ExecCmd(_parentDataBase.Dialect.IdentityInsert(this)
            );

            }
        }

        public void CustomsInit()
        {
            var tableType = GetType();
            foreach (var method in tableType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                InitMethodAttribute a = method.GetCustomAttribute<InitMethodAttribute>();
                if (a != null)
                    method.Invoke(this, null);
            }
        }

        public void CopyFrom(CTable<T, TDatabase, TPKey> source)
        {
            IdentityInsert = true;
            FieldDescription idfd = GetIdentity();
            if (idfd != null)
                idfd.Properties -= FieldDescription.fieldProp.Identity;
            try
            {

                foreach (T el in source.GetAll())
                {
                    var bobj = el as BObject;
                    T dest = this.NewInstance();
                    dest.RestoreFrom(el);
                    (dest as IPrimaryKey<TPKey>).Id = (el as IPrimaryKey<TPKey>).Id;
                    (dest as BObject).SetDirtyFlag();
                    dest.Save2();
                }
            }
            finally
            {

                if (idfd != null)
                    idfd.Properties -= FieldDescription.fieldProp.Identity;
                IdentityInsert = false;
            }
        }

        public void RevisionOnAddColumn()
        {
            _parentDataBase.RevisionOnAddColumn(this);
        }


    }
}
