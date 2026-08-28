using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Data.Common;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using DbCommon.Attributes;
using DbCommon.Dialects;
using DbCommon.Enums;
using DbCommon.Helpers;
using DbCommon.Json;

namespace DbCommon
{
    public interface iMultiLangTable
    {
        int CurrentLang { get; set; }
    }

    public interface iLangDatabase
    {
        iTable TableLang { get; }
        int BaseLang { get; set; }
        int CurrentLang { get; set; }
    }

    public interface IConnectionString
    {
        string BuildConnectionString(int timeout);

        string GetDBName();

        string GetPassword();

        void SetPassword(string newPwd);
    }


    abstract public class CDatabase
    {
        public DatabaseType DatabaseType;

        public IDialect Dialect = null;
        protected CDatabase(DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            switch (DatabaseType)
            {
                case DatabaseType.NotSql:
                    break;
                case DatabaseType.MsSql:
                    Dialect = new DialectMsSql();
                    break;
                case DatabaseType.PostgreeSql:
                    Dialect = new DialectPostGree();
                    break;
                case DatabaseType.MySql:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _semafor = new Semaphore(1, 1);
        }


        private IConnectionString _iConnectionString = null;
        protected string _sConnectionString = null;
        private string _sDataBaseName = "";
        private Semaphore _semafor = null;

        public ConnectionModes ConnectionMode = ConnectionModes.NoConnection;

        /// <summary>
        /// Тестирует соединение и при успехе устанавливает новую строку соединения
        /// Если нет - оставляет старое
        /// </summary> 
        public bool SetConnectionString(IConnectionString ics)
        {
            string newCs = ics.BuildConnectionString(30);
            DbConnection con = GetDbConnection(newCs);
            {
                int tryCount = 0;
            tryLabel:
                try
                {
                    string dbName = ics.GetDBName();
                    if (BObject.Compare(dbName, _sDataBaseName) != 0)
                    {
                        // сбросим все кешированые объекты
                        //RootCash = new Hashtable();
                    }
                    _sDataBaseName = dbName;
                    ConnectionMode = ConnectionModes.DatabaseConnection;
                }
                catch (SqlException ex)
                {
                    tryCount++;
                    Log.Error(ex, "SetConnectionString попытка подключения:" + tryCount);
                    if (tryCount <= 20)
                    {
                        Thread.Sleep(1000);
                        goto tryLabel;
                    }
                    if (string.IsNullOrEmpty(_sDataBaseName))
                        _sDataBaseName = ics.GetDBName();
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "SetConnectionString");
                    if (string.IsNullOrEmpty(_sDataBaseName))
                        _sDataBaseName = ics.GetDBName();
                    throw;
                }
            }
            _iConnectionString = ics;
            _sConnectionString = newCs;

            return true;
        }

        public bool SetConnectionString(string p)
        {
            _sConnectionString = p;
            return true;
        }


        public bool TestConnection()
        {
            DbConnection con = GetDbConnection(_sConnectionString);
            {
                try
                {
                    ExecCmd(Dialect.TestQuery());
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "TestConnection");
                    return false;
                }
            }
        }

        public void CreateDatabase(string name)
        {
            try
            {
                string sSql = string.Format(Dialect.CreateDatabase(), name);
                ExecCmd(sSql);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CreateDatabase");
                throw;
            }
        }

        public void DeleteDatabase(string name)
        {
            try
            {
                ClearAllPools();
                string sSql = string.Format(Dialect.DeleteDatabase(), name);
                ExecCmd(sSql);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeleteDatabase");
                throw;
            }
        }

        public void CreateShema(iTable table = null)
        {
            IEnumerable<iTable> tList = table == null ? (IEnumerable<iTable>)Tables.Values : new iTable[] { table };
            ExecCmd(tList.Select(t => Dialect.CreateTable(t)));
            foreach (var t in tList)
                t.RestorePredefined();
        }


        public Type ConnectionType = null;
        //readonly Type _tcon = typeof(NpgsqlConnection);
        //Type tcon = typeof(System.Data.SqlClient.SqlConnection);
        //Type tcon = typeof(MySql.Data.MySqlClient.MySqlConnection);
        [ThreadStatic]
        DbConnection _dbConnection;
        public virtual DbConnection GetDbConnection(string newCs)
        {
            if (ConnectionType == null)
            {
                switch (DatabaseType)
                {
                    case DatabaseType.MsSql:
                        ConnectionType = typeof(SqlConnection);
                        break;
                    case DatabaseType.PostgreeSql:
                        ConnectionType = LoadNpgsqlConnection();
                        break;
                    case DatabaseType.MySql:
                        ConnectionType = LoadMySqlConnection();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (_dbConnection != null && _dbConnection.State != ConnectionState.Open)
                _dbConnection = null;
            if (_dbConnection == null)
            {
                _dbConnection = (DbConnection)Activator.CreateInstance(ConnectionType, newCs);
                _dbConnection.Open();
            }
            return _dbConnection;
        }

        public void ClearAllPools()
        {
            var m = ConnectionType.GetMethod("ClearAllPools");
            m.Invoke(null, BindingFlags.InvokeMethod, null, null, null);
        }

        private Type LoadMySqlConnection()
        {
            throw new NotImplementedException();
        }

        private Type LoadNpgsqlConnection()
        {
            try
            {
                var a = Assembly.LoadFrom("Npgsql.dll");
                return a.GetTypes().Where(typeof(DbConnection).IsAssignableFrom).Single(t => t.Name == "NpgsqlConnection");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при загрузке Npgsql.dll");
                throw;
            }
        }


        /// <summary>
        /// сырое чтение объектов
        /// !!! не использовать в приложении
        /// </summary>
        public virtual iSortList ReadObjectList(CQueryDesc qdesc)
        {
            iSortList list = qdesc.T.CreateSortList();
            if (!string.IsNullOrEmpty(qdesc.Order))
                list.SetSortComparer(qdesc.Order);
            BObject obj = qdesc.T.CreateInstance();
            //obj.CurrentLang = Settings.BaseLang();
            DbConnection con = GetDbConnection(_sConnectionString);
            try
            {
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = qdesc.QuerySelect(null).ToString();
                    using (DbDataReader r = cmd.ExecuteReader())
                    {
                        var columns = qdesc.T.Columns.Values;
                        while (r.Read())
                        {
                            FillObject(obj, r, columns);
                            obj.IsPersistent = true;
                            list.Add(obj);
                            obj = qdesc.T.CreateInstance();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SqlException sqlEx = ex as SqlException;
                if (sqlEx != null && sqlEx.Number == 208)
                {
                    CreateShema(qdesc.T);
                    return ReadObjectList(qdesc);
                }
                Log.Error(ex, "ReadObjectList");
                throw;
            }
            return list;
        }


        public virtual void Save2(BObject obj)
        {
            string sSql;
            switch (obj.ObjectState)
            {
                case BObject.ObjectStateType.Added:
                    DbConnection con = GetDbConnection(_sConnectionString);
                    {
                        using (DbCommand cmd = con.CreateCommand())
                        {
                            cmd.CommandText = GetInsertCommandText(obj);
                            Log.Debug("Save2 CommandText:" + cmd.CommandText);
                            var fd = obj._table.GetIdentity();
                            if (fd != null)
                            {
                                Object res = cmd.ExecuteScalar();
                                fd.PropInfo.SetValue(obj, res);
                            }
                            else
                                cmd.ExecuteNonQuery();
                            obj.IsPersistent = true;
                            obj.ObjectState = BObject.ObjectStateType.Unchanged;
                            return;
                        }
                    }
                case BObject.ObjectStateType.Changed:
                    sSql = GetSetValuesCommandText(obj);
                    if (sSql != "")
                    {
                        Log.Debug("Save2 CommandText:" + sSql);
                        ExecCmd(sSql);
                    }
                    obj.ObjectState = BObject.ObjectStateType.Unchanged;
                    return;
                case BObject.ObjectStateType.Deleted:
                    try
                    {
                        sSql = GetDeleteCommandText(obj);
                        Log.Debug("Save2 CommandText:" + sSql);
                        ExecCmd(sSql);
                    }
                    catch (SqlException sqlException)
                    {
                        if (sqlException.Number == 547)
                            throw new BOSaveExeption("Удаление невозможно. На объект есть ссылки.");
                        throw;
                    }

                    obj.ObjectState = BObject.ObjectStateType.Unchanged;
                    obj.IsPersistent = false;
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// Выполняет комманду без параметров и возвращаемых значений
        /// </summary>
        /// <param name="sSql"></param>
        public void ExecCmd(string sSql)
        {
            var con = GetDbConnection(_sConnectionString);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sSql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Выполняет серию комманд без параметров и возвращаемых значений
        /// </summary>
        /// <param name="sSql"></param>
        public void ExecCmd(IEnumerable<string> sSql)
        {
            var con = GetDbConnection(_sConnectionString);
            using (var cmd = con.CreateCommand())
            {
                foreach (var sCmd in sSql)
                {
                    try
                    {
                        cmd.CommandText = sCmd;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "ExecCmd:" + sCmd);
                        throw;
                    }
                }
            }
        }

        public List<string> ExecCmdStringList(string sSql)
        {
            DbConnection con = GetDbConnection(_sConnectionString);
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sSql;
                using (DbDataReader dbReader = cmd.ExecuteReader())
                {
                    List<string> ret = new List<string>();
                    while (dbReader.Read())
                        ret.Add(dbReader.GetString(0));
                    return ret;
                }
            }
        }


        private string GetDeleteCommandText(BObject obj)
        {
            iTable t = obj._table;
            var listW = t.GetPrimaryKeysList();

            string ret = Dialect.DeleteCommand();
            // "delete from {1} where {2}" - postgree;
            // "delete {0}.{1} where {2}"; - mssql

            ret = String.Format(ret, t.OwnerName(), t.TableName(), GetWhereCommandText(obj));
            return ret;
        }

        private string GetSetValuesCommandText(BObject obj)
        {
            iTable t = obj._table;

            StringBuilder sb = new StringBuilder();
            sb.Append("update ");
            if (DatabaseType == DatabaseType.MsSql)
            {
                sb.Append(t.OwnerName());
                sb.Append(".");
            }
            sb.Append(t.TableName());
            sb.Append(" set ");
            string s = GetSetValuesString(obj);
            if (s != "")
            {
                sb.Append(s);
                sb.Length--;
                sb.Append(" where ");
                sb.AppendLine(GetWhereCommandText(obj).ToString());
            }
            else
                sb.Length = 0;

            return sb.ToString();
        }


        /// <summary>
        /// формирует строку присваиваний новых значений полям
        /// </summary>
        /// <param name="listFD"> список полей </param>
        /// <param name="nValues"> список новых значений </param>
        /// <param name="oldValues"> список прежних значений </param>
        /// <returns></returns>
        private string GetSetValuesString(FieldDescription[] listFD, string[] nValues, string[] oldValues)
        {
            StringBuilder sb = new StringBuilder(500);
            for (int i = 0; i < listFD.Count(); i++)
            {
                FieldDescription fd = listFD[i];
                if (fd.Properties != FieldDescription.fieldProp.Identity)
                {
                    object nVal = nValues[i];
                    object oVal = oldValues == null ? null : oldValues[i];
                    if (BObject.Compare(nVal, oVal) != 0)
                    {
                        sb.Append(fd.Name);
                        sb.Append("=");
                        sb.Append(ConvertValueToSQLString(nVal, fd.Properties));
                        sb.Append(",");
                    }
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Возвращает строку присваиваний значений
        /// </summary>
        private string GetSetValuesString(BObject obj)
        {
            iTable t = obj._table;
            StringBuilder sb = new StringBuilder(500);
            BObject persist = obj.GetPersist();
            if (persist == null)
                throw new ArgumentException("Отсутствует исходный объект!(persist)");
            foreach (FieldDescription fd in t.Columns.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.Identity) == 0))
            {
                try
                {
                    object nVal = fd.PropInfo.GetValue(obj, null);
                    object oVal = fd.PropInfo.GetValue(persist, null);
                    if (BObject.Compare(nVal, oVal) != 0)
                    {
                        sb.Append(fd.Name);
                        sb.Append("=");
                        sb.Append(ConvertValueToSQLString(nVal, fd.Properties));
                        sb.Append(",");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Read property {0} of object {1}", fd.Name, obj.GetType().Name);
                }
            }
            return sb.ToString();
        }

        private string GetInsertCommandText(BObject obj)
        {
            iTable t = obj._table;
            var listFd = t.Columns.Values;
            var listInsertedFields = listFd.Where((FieldDescription fd) => (fd.Properties & FieldDescription.fieldProp.Identity) == 0).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into ");
            if (DatabaseType == DatabaseType.MsSql)
            {
                sb.Append(t.OwnerName());
                sb.Append(".");
            }
            sb.Append(t.TableName());
            sb.Append("(");
            sb.Append(string.Join(",", listInsertedFields.Select(f => f.Name)));
            sb.Append(") values (");
            sb.Append(GetValueStringList(obj, listInsertedFields));
            sb.Length--;
            if (DatabaseType == DatabaseType.MsSql)
                sb.AppendLine(")");
            else
                sb.AppendLine(");");
            FieldDescription identity = listFd.FirstOrDefault(f => (f.Properties & FieldDescription.fieldProp.Identity) > 0);
            if (identity != null)
                sb.AppendLine(Dialect.SelectLastId(identity));
            //mysql: sb.AppendLine("select last_insert_id();");
            return sb.ToString();
        }


        /// <summary>
        /// Возвращает значения полей, разделенные запятыми, с завершающей запятой
        /// </summary>
        private string GetValueStringList(BObject obj, FieldDescription[] fdList)
        {
            StringBuilder sbRet = new StringBuilder();
            foreach (FieldDescription f in fdList)
            {
                sbRet.Append(ConvertValueToSQLString(f.PropInfo.GetValue(obj, null), f.Properties));
                sbRet.Append(",");
            }
            return sbRet.ToString();
        }


        private string ConvertValueToSQLString(Object value, FieldDescription.fieldProp pi)
        {
            if (value == null) return "null";
            switch (value)
            {
                case String svalue:
                    {
                        if ((pi & FieldDescription.fieldProp.Nullable) > 0 && svalue == string.Empty)
                            return "null";
                        string prefiks = DatabaseType == DatabaseType.MsSql ? "N" : "";
                        return prefiks + "'" + Quote(svalue) + "'";
                    }
                case Guid gvalue:
                    if ((pi & FieldDescription.fieldProp.Nullable) > 0 && gvalue == Guid.Empty)
                        return "null";
                    return $"'{gvalue}'";
                case int ivalue:
                    {
                        if ((pi & FieldDescription.fieldProp.Nullable) > 0 && (pi & FieldDescription.fieldProp.ForeignKey) > 0 && ivalue == 0)
                            return "null";
                        return ivalue.ToString();
                    }
                case DateTime dtValue:
                    // return "CONVERT(datetime,'" + dtValue.ToString("s") + "',103)";
                    return "'" + dtValue.ToString("s") + "'";
                case Boolean boolValue:
                    return Dialect.ConvertToBooleanString(boolValue);
                case Bitmap bm:
                    {
                        using (var ms = new MemoryStream())
                        {
                            bm.Save(ms, ImageFormat.Png);
                            return Dialect.ConvertToString(ms.GetBuffer());
                        }
                    }
                //if (vt.Equals(typeof(AOS_OpenGL.DDDImage)))
                //{
                //    byte[] zip_buffer = ((AOS_OpenGL.DDDImage)value).SaveZip();

                //    StringBuilder sb = new StringBuilder(zip_buffer.Length * 2 + 2);
                //    sb.Append("0x");
                //    foreach (byte b in zip_buffer)
                //        sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                //    return sb.ToString();
                //}
                case Int16[] shortArray:
                    {
                        return Dialect.ConvertToString(shortArray);
                    }
                case byte[] byteArray:
                    {
                        return Dialect.ConvertToString(byteArray);
                    }
                case Enum enumValue:
                    return Convert.ToInt32(enumValue).ToString();
                case decimal dValue:
                    {
                        string s = (dValue).ToString("0.000000");
                        s = s.Replace(',', '.');
                        return s;
                    }
            }
            return value.ToString();
        }

        private static string Quote(string p)
        {
            return p.Replace("'", "''");
        }


        public int GetTotal(iTable t)
        {
            if (!ExistsConnection()) return -1;
            string sSql = "select count(*) from {1}";
            sSql = String.Format(sSql, t.OwnerName(), t.TableName_select());
            DbConnection con = GetDbConnection(_sConnectionString);
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sSql;
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public object GetScalar(string sSql)
        {
            if (!ExistsConnection()) return null;
            DbConnection con = GetDbConnection(_sConnectionString);
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sSql;
                return cmd.ExecuteScalar();
            }
        }

        public virtual int GetCount(iTable t, string filtr)
        {
            string sSql = "if exists(select 1 from {0}.{1} t {2}) select count(*)  from {0}.{1} t {2} select 0";
            if (!string.IsNullOrEmpty(filtr))
                filtr = " where " + filtr;
            sSql = String.Format(sSql, t.OwnerName(), t.TableName_select(), filtr);
            DbConnection con = GetDbConnection(_sConnectionString);
            var cmd = con.CreateCommand();
            cmd.CommandText = sSql;
            var ret = cmd.ExecuteScalar();
            try
            {
                return (int)(long)ret;
            }
            catch (InvalidCastException)
            {
                return (int)ret;
            }
        }

        public virtual bool Exists(iTable t, string filtr)
        {
            string sSql = "if exists(select 1 from {0}.{1} t {2}) select 1 select 0";
            if (!string.IsNullOrEmpty(filtr))
                filtr = " where " + filtr;
            sSql = String.Format(sSql, t.OwnerName(), t.TableName_select(), filtr);
            DbConnection con = GetDbConnection(_sConnectionString);
            var cmd = con.CreateCommand();
            cmd.CommandText = sSql;
            var ret = cmd.ExecuteScalar();
            try
            {
                return (int)(long)ret == 1;
            }
            catch (InvalidCastException)
            {
                return (int)ret == 1;
            }
        }

        private UInt64 ReadTimeStamp(iTable t, int id)
        {
            StringBuilder sb = new StringBuilder(255);
            sb.Append("select ");
            int len = sb.Length;
            sb.Append(t.GetFieldStringList(FieldDescription.fieldProp.TimeStamp));
            if (sb.Length == len)
                return 0;
            sb.Length--;
            sb.Append(" from ");
            //sb.Append(t.OwnerName());
            //sb.Append(".");
            sb.AppendLine(t.TableName_select());
            sb.Append(" where id=");
            sb.Append(id);
            try
            {
                DbConnection con = GetDbConnection(_sConnectionString);
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    return ConvertTimeStamp((byte[])cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        private static string _cashPath = string.Empty;

        /// <summary>
        /// Путь к каталогу с кешированными данными и префикс для файла
        /// </summary> 
        public string GetCashPath()
        {
            if (string.IsNullOrEmpty(_cashPath))
            {
                _cashPath = GetApplicationDataPath() + _sDataBaseName + @"\";

                if (!Directory.Exists(_cashPath))
                {
                    Directory.CreateDirectory(_cashPath);
                }
                _cashPath = _cashPath + @"$$$";
            }
            return _cashPath;
        }

        public void SaveLogString(string[] logs)
        {
            using (FileStream logf = new FileStream(GetCashPath() + "log.txt", FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(logf))
                {
                    sw.Write(DateTime.Now.ToLongDateString());
                    sw.Write(" ");
                    sw.Write(DateTime.Now.ToLongTimeString());
                    sw.WriteLine(" : ");
                    foreach (string s in logs)
                        sw.WriteLine(s);
                    sw.Close();
                }
                logf.Close();
            }
        }

        private static string _applicationDataPath = string.Empty;

        public static string GetApplicationDataPath()
        {
            if (string.IsNullOrEmpty(_applicationDataPath))
            {
                _applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\ATPM\";
                if (!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }
            }
            return _applicationDataPath;
        }

        private void WriteObjectToDisk(BObject obj)
        {
            Type t = obj.GetType();
            var o = obj as IPrimaryKey<int>;
            var hashKey = o != null ? o.Id : obj.GetKeyString();

            string fileName = GetCashPath() + t.Name + "_" + hashKey.ToString() + ".dat";

            using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                obj._write(bw);
            }
        }

#if DTYPE
        private void WriteAllObjectToDisk(iTable t, iSortList list)
        {
            DType dt = t.parentDataBase.DType.GetByName(t.TableName());
            string fileName = GetCashPath() + t.TableName() + ".dat";
            if (dt.TS > 0 && dt.TS == t.LastTS)
            {
                try
                {
                    using (BinaryWriter bw =
                    new BinaryWriter(File.Open(fileName, FileMode.Create)))
                    {
                        list.WriteAll(bw);
                        //t.WriteAll(bw);
                    }
                }
                catch { };
                return;
            }
            try
            {
                File.Delete(fileName);
            }
            catch { };
        }
#endif


        private void ClearDiskCash(Type t)
        {
            string fileName = GetCashPath() + t.Name + ".dat";
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// Читает с диска всю таблицу
        /// </summary>
        private iSortList ReadAllObjectFromDisk(iTable t, UInt64 lastTS)
        {
            string fileName = GetCashPath() + t.TableName() + ".dat";

            try
            {
                using (BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    UInt64 lastSaveTS;
                    try
                    {
                        lastSaveTS = br.ReadUInt64();
                    }
                    catch (EndOfStreamException)
                    {
                        return null;
                    }
                    if (lastSaveTS != lastTS)
                        return null; // количество объектов на сервере изменилось, сбрасываем кеш

                    int count;
                    try
                    {
                        count = br.ReadInt32();
                    }
                    catch (EndOfStreamException)
                    {
                        return null;
                    }


                    iSortList list = t.CreateSortList();
                    for (int i = 0; i < count; i++)
                    {
                        BObject obj = (BObject)t.CreateInstance();
                        //obj.CurrentLang = Settings.BaseLang();
                        obj._read(br);
                        list.Add(obj);
                    }
                    return list;
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(GetApplicationDataPath());
                    info.CreateSubdirectory(_sDataBaseName);
                    return ReadAllObjectFromDisk(t, lastTS);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch
            {
                return null;
            }
        }


        private static SortList<T> ReadObjectListFromDisk<T>(iTable t, string sFiltr) where T : BObject, new()
        {
            throw new NotImplementedException();
        }

        private BObject ReadObjectFromDisk(iTable t, object key, UInt64 lastTS)
        {
            string fileName = GetCashPath() + t.TableName() + "_" + key.ToString() + ".dat";

            try
            {
                using (BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open)))
                {
                    BObject obj = (BObject)t.CreateInstance();
                    //obj.CurrentLang = Settings.BaseLang();
                    if (obj._read(br))
                        return obj;
                }
                File.Delete(fileName);
                return null;
            }
            //catch (FileNotFoundException) { return null; }
            //catch (FileLoadException) { return null; }
            catch
            {
                return null;
            }
        }


        public void UpdateObject(BObject obj, string sFields)
        {
            string sSql = GetSelectCommandText(obj._table, sFields + " ", GetWhereCommandText(obj).ToString());
            DbConnection con = GetDbConnection(_sConnectionString);
            string[] aFields = sFields.Split(',');
            FieldDescription[] afd = obj._table.Columns.Values.Where(fd => aFields.Any(s => s == fd.Name)).ToArray();

            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sSql;
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        FillObject(obj, reader, afd);
                }
            }
        }


        internal static void FillObject(BObject obj, System.Data.Common.DbDataReader reader, IEnumerable<FieldDescription> fields)
        {
            obj.IsLoaded = true;
            Type t = obj.GetType();
            foreach (FieldDescription s in fields)
            {
                Object value = reader[s.Name];
                PropertyInfo p = s.PropInfo;
                if (value == System.DBNull.Value)
                {
                    p.SetValue(obj, null, null);
                    continue;
                }
                if (p.PropertyType == typeof(Bitmap) || p.PropertyType == typeof(Image))
                {
                    Bitmap b = (Bitmap)Image.FromStream(new MemoryStream((byte[])value));
                    p.SetValue(obj, b, null);
                    continue;
                }
                if ((s.Properties & FieldDescription.fieldProp.TimeStamp) > 0)
                {
                    p.SetValue(obj, ConvertTimeStamp((byte[])value), null);
                    continue;
                }
                if (p.PropertyType == typeof(short[]))
                {
                    Int16[] b = new Int16[((byte[])value).Length / 2];
                    for (int i = 0; i < b.Length; i++)
                        b[i] = (Int16)((((byte[])value)[i << 1] << 8) | ((byte[])value)[(i << 1) + 1]);
                    p.SetValue(obj, b, null);
                    continue;
                }
                if (p.PropertyType == typeof(bool))
                {
                    bool bValue = Convert.ToBoolean(value);
                    p.SetValue(obj, bValue, null);
                    continue;
                }
                p.SetValue(obj, value, null);
            }
            obj.IsLoaded = false;
            obj.ObjectState = BObject.ObjectStateType.Unchanged;
            obj.IsPersistent = true;
        }

        public static UInt64 ConvertTimeStamp(byte[] val)
        {
            if (val.Length != 8)
                throw new Exception("Ошибка в ConvertTimeStamp");
            UInt64 ret = 0;
            for (int i = 0; i < 8; i++)
                ret = (ret << 8) | val[i];
            return ret;
        }

        private static string GetSelectCommandText(iTable t, string sWhere)
        {
            StringBuilder sb = new StringBuilder(255);
            sb.Append("select ");
            sb.Append(t.GetFieldStringList(FieldDescription.fieldProp.All));
            sb.Length--;
            sb.Append(" from ");
            sb.Append(t.OwnerName());
            sb.Append(".");
            sb.AppendLine(t.TableName());
            sb.Append(" where ");
            sb.Append(sWhere);
            return sb.ToString();
        }

        private static string GetSelectCommandText(iTable t, string sFields, string sWhere)
        {
            StringBuilder sb = new StringBuilder(255);
            sb.Append("select ");
            sb.Append(sFields);
            sb.Length--;
            sb.Append(" from ");
            sb.Append(t.OwnerName());
            sb.Append(".");
            sb.AppendLine(t.TableName());
            sb.Append(" where ");
            sb.Append(sWhere);
            return sb.ToString();
        }

        private StringBuilder GetWhereCommandText(BObject obj)
        {
            iTable t = obj._table;
            var listW = t.GetPrimaryKeysList();

            if (listW.Count() == 0)
                throw new Exception("для таблицы" + " " + t.TableName() + " " + "не заданы ключевые поля");

            StringBuilder sb = new StringBuilder(100);

            foreach (FieldDescription s in listW)
            {
                object oValue = s.PropInfo.GetValue(obj, null);
                sb.Append(s.Name);
                if (oValue == null)
                {
                    sb.Append(" is null");
                }
                else
                {
                    sb.Append("=");
                    sb.Append(ConvertValueToSQLString(oValue, s.Properties));
                }
                sb.Append(" and ");
            }
            sb.Length -= 5;
            return sb;
        }

        public void RevisionOnAddColumn<T, TDatabase, TPKey>(CTable<T, TDatabase, TPKey> t)
            where T : BObject, new()
            where TDatabase : CDatabase
        {
            StringBuilder sb = new StringBuilder("declare @t TABLE(name VARCHAR(MAX))\r\n INSERT @t values('");
            sb.Append(string.Join("'),('", t.Columns.Values.Select(fd => fd.Name)));
            sb.Append("')");
            sb.Append("select * from @t t where not exists(select * from syscolumns where id = object_id('");
            sb.Append($"{t.OwnerName()}.{t.TableName()}");
            sb.Append("') and name = t.name)");
            var columns = ExecCmdStringList(sb.ToString());
            foreach (var colName in columns)
            {
                ExecCmd(Dialect.AddColumn(t, t.Columns.Values.First(fd=>fd.Name==colName)));
            }
        }

        private static int _current_lang = 0;


        /// <summary>
        /// Переключает язык всех объектов и запоминает как текущий
        /// </summary>
        public int CurrentLang
        {
            get { return _current_lang; }
            set
            {
                if (_current_lang != value)
                {
                    foreach (iTable t in _tables.Values)
                    {
                        if (t is iMultiLangTable)
                        {
                            ((iMultiLangTable)t).CurrentLang = value;
                        }
                    }
                    _current_lang = value;
                }
            }
        }

        /// <summary>
        /// Загружает данные на нужном языке для одного объекта
        /// </summary>
        public string[] ReadLangValues(BObject obj, string Lang)
        {
            string sSql = GetSelectLLCommandText(obj, Lang);
            DbConnection con = GetDbConnection(_sConnectionString);
            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sSql;
                string[] ret = null;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ret = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            ret[i] = (string)reader[i];
                        }
                    }
                    return ret;
                }
            }
        }

        /// <summary>
        /// текст запроса для чтения мультиязыковых полей
        /// </summary>
        private string GetSelectLLCommandText(BObject obj, string mnem)
        {
            iTable t = obj._table;
            //            bool langIsBase = mnem == Settings.BaseLang();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            sb.Append(t.GetFieldStringList(FieldDescription.fieldProp.MultiLang));
            sb.Length--;
            sb.Append(" from ");
            //sb.Append(t.OwnerName());
            //sb.Append(".");
            //            if (!langIsBase) sb.Append("lang_");
            sb.Append(t.TableName_select());
            sb.Append(" where ");
            sb.Append(GetWhereCommandText(obj));
            //if (!langIsBase)
            //{
            //    sb.Append(" and Lang=\'");
            //    sb.Append(mnem);
            //    sb.Append("\'");
            //}
            return sb.ToString();
        }

#if DTYPE
    /// <summary>
    /// Сохраняет на диск кешированные данные, 
    /// вызывается 1 раз при закрытии главной формы
    /// </summary>
        public void SaveDiskData()
        {
            foreach (Object objt in RootCash.Keys)
            {
                iTable t = (iTable)objt;
                CasheType ct = t.GetCashType();
                if (ct == CasheType.FullDiskCash)
                {
                    Hashtable _ct = (Hashtable)RootCash[t];
                    iSortList list = (iSortList)_ct[""];
                    if (list == null) continue;
                    if (list.GetCount() == 0) continue;

                    //if (GetCurrentLang() != Settings.BaseLang())
                    //    // Перед записью на диск приведем язык к базовому
                    //    list.ChangeLang(Settings.BaseLang());
                    WriteAllObjectToDisk(t, list);
                }
            }
        }
#endif

        public bool ExistsConnection()
        {
            return _sConnectionString != null;
        }

        internal void CreateLogin(string login, string password, string dbRoleName)
        {
            string sSql = "if not exists (select * from sys.sql_logins where name = '@ServerLogin')\n" + "begin\n" + "	exec master.sys.sp_addlogin '@ServerLogin','@password','AOS',null,null,null\n" + "	exec sys.sp_adduser '@ServerLogin'\n" + "end		\n" + "exec sys.sp_addrolemember '@DbRoleName','@ServerLogin'";
            sSql = sSql.Replace("@ServerLogin", login);
            sSql = sSql.Replace("@password", password);
            sSql = sSql.Replace("@DbRoleName", dbRoleName);
            ExecCmd(sSql);
        }

        internal void ClearLogin(string login)
        {
            string sSql = "if exists (select * from sys.sql_logins where name = '@ServerLogin')\n" + "exec sp_droplogin '@ServerLogin'\n" + "if exists (select * from sys.sysusers where name = '@ServerLogin')\n" + "exec sp_dropuser '@ServerLogin'\n";
            sSql = sSql.Replace("@ServerLogin", login);
            ExecCmd(sSql);
        }

        private string GetFieldDescSqlString(FieldDescription fd)
        {
            StringBuilder sb = new StringBuilder(50);
            sb.Append("[");
            sb.Append(fd.Name);
            sb.Append("] ");
            sb.Append("[");
            sb.Append(fd.sType);
            sb.Append("] ");
            if (fd.sType.Contains("varchar"))
            {
                sb.Append("(");
                sb.Append(fd.Size.ToString());
                sb.Append(") ");
            }
            if ((fd.Properties & FieldDescription.fieldProp.Nullable) == 0)
                sb.Append(" NOT");
            sb.Append(" NULL,");
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает значения полей, разделенные запятыми, с завершающей запятой
        /// </summary>
        private string GetValueStringList(BObject obj, FieldDescription.fieldProp fProp)
        {
            iTable t = obj._table;
            StringBuilder sbRet = new StringBuilder();
            foreach (FieldDescription f in t.Columns.Values)
            {
                if ((f.Properties & fProp) > 0)
                {
                    sbRet.Append(ConvertValueToSQLString(f.PropInfo.GetValue(obj, null), f.Properties));
                    sbRet.Append(",");
                }
            }
            return sbRet.ToString();
        }

        /// <summary>
        /// Возвращает значения полей, разделенные запятыми, с завершающей запятой
        /// </summary>
        private string GetValueStringList(FieldDescription[] listFD, string[] nValues)
        {
            StringBuilder sbRet = new StringBuilder();
            for (int i = 0; i < listFD.Count(); i++)
            {
                FieldDescription fd = listFD[i];
                sbRet.Append(ConvertValueToSQLString(nValues[i], fd.Properties));
                sbRet.Append(",");
            }
            return sbRet.ToString();
        }


        private static string sekret_key = "небо в алмазах";

        public static string EncodePwd(string password)
        {
            if (password == null) return null;
            byte[] res = new byte[password.Length];
            int j = 0;
            for (int i = 0; i < password.Length; i++)
            {
                res[i] = (byte)((byte)password[i] + (byte)sekret_key[j++]);
                if (j >= sekret_key.Length)
                    j = 0;
            }
            return Convert.ToBase64String(res);
        }

        public static string DecodePwd(string p)
        {
            byte[] res;
            try
            {
                res = Convert.FromBase64String(p);
            }
            catch
            {
                return "";
            }
            string password = "";
            int j = 0;
            for (int i = 0; i < res.Length; i++)
            {
                password += (char)((byte)res[i] - (byte)sekret_key[j++]);
                if (j >= sekret_key.Length)
                    j = 0;
            }
            return password;
        }

        public string GetWhereFromExpr(Expression exprBody)
        {
            string ret = "";
            switch (exprBody.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return ConvertBinary((BinaryExpression)exprBody, "+");
                case ExpressionType.And:
                    return ConvertBinary((BinaryExpression)exprBody, " & ");
                case ExpressionType.AndAlso:
                    return ConvertBinary((BinaryExpression)exprBody, " and ");
                case ExpressionType.Divide:
                    return ConvertBinary((BinaryExpression)exprBody, @" / ");
                case ExpressionType.Equal:
                    return ConvertBinaryEQ((BinaryExpression)exprBody);
                case ExpressionType.ExclusiveOr:
                    return ConvertBinary((BinaryExpression)exprBody, " ^ ");
                case ExpressionType.GreaterThan:
                    return ConvertBinary((BinaryExpression)exprBody, " > ");
                case ExpressionType.GreaterThanOrEqual:
                    return ConvertBinary((BinaryExpression)exprBody, " >= ");
                case ExpressionType.LessThan:
                    return ConvertBinary((BinaryExpression)exprBody, " < ");
                case ExpressionType.LessThanOrEqual:
                    return ConvertBinary((BinaryExpression)exprBody, " <= ");
                case ExpressionType.Modulo:
                    return ConvertBinary((BinaryExpression)exprBody, " % ");
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return ConvertBinary((BinaryExpression)exprBody, " * ");
                case ExpressionType.NotEqual:
                    return ConvertBinaryNEQ((BinaryExpression)exprBody);
                case ExpressionType.Or:
                    return ConvertBinary((BinaryExpression)exprBody, " | ");
                case ExpressionType.OrElse:
                    return ConvertBinary((BinaryExpression)exprBody, " OR ");
                case ExpressionType.Power:
                    return "power" + ConvertBinary((BinaryExpression)exprBody, ",");
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return ConvertBinary((BinaryExpression)exprBody, " - ");
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.Call:
                    return ConvertCall((MethodCallExpression)exprBody);
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    return ConvertValueToSQLString(((ConstantExpression)exprBody).Value, FieldDescription.fieldProp.Empty);
                case ExpressionType.ConvertChecked:
                case ExpressionType.Convert:
                    return GetWhereFromExpr(((UnaryExpression)exprBody).Operand);
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.MemberAccess:
                    return ConvertMemberAccess((MemberExpression)exprBody);
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.Parameter:
                    //((ParameterExpression)exprBody).Name
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeIs:
                    break;
            }
            return ret;
        }

        private string ConvertCall(MethodCallExpression mce)
        {
            //string s = "";
            //s.ToLower(
            Type[] emptyarg = new Type[] { };
            if (mce.Method.Equals(typeof(string).GetMethod("ToLower", emptyarg)) || mce.Method.Equals(typeof(string).GetMethod("ToUpper", emptyarg)))
                return GetWhereFromExpr(mce.Object);
            return "";
        }

        private string ConvertMemberAccess(MemberExpression me)
        {
            if (me.Expression.NodeType == ExpressionType.Parameter)
                return me.Member.Name;
            if (me.Expression.NodeType == ExpressionType.Constant)
            {
                ConstantExpression ce = (ConstantExpression)me.Expression;
                if (me.Member.MemberType == MemberTypes.Field)
                {
                    return ConvertValueToSQLString(ce.Type.GetField(me.Member.Name).GetValue(ce.Value), FieldDescription.fieldProp.Empty);
                }
            }
            if (me.Expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression innerMe = (MemberExpression)me.Expression;
                if (me.Member.MemberType == MemberTypes.Field)
                {
                    return ConvertMemberAccess(innerMe);
                }
            }
            return "";
        }

        private string ConvertBinary(BinaryExpression be, string sign)
        {
            return "(" + GetWhereFromExpr(be.Left) + sign + GetWhereFromExpr(be.Right) + ")";
        }

        private string ConvertBinaryEQ(BinaryExpression be)
        {
            string rigth = GetWhereFromExpr(be.Right);
            if (rigth == "null")
                return GetWhereFromExpr(be.Left) + " is null";
            else
                return GetWhereFromExpr(be.Left) + "=" + rigth;
        }

        private string ConvertBinaryNEQ(BinaryExpression be)
        {
            string rigth = GetWhereFromExpr(be.Right);
            if (rigth == "null")
                return GetWhereFromExpr(be.Left) + " is not null";
            else
                return GetWhereFromExpr(be.Left) + " !=" + rigth;
        }


        internal BObject GetObjectNew(Type type)
        {
            throw new NotImplementedException();
        }

        protected Dictionary<string, iTable> _tables = new Dictionary<string, iTable>();

        public Dictionary<string, iTable> Tables
        {
            get { return _tables; }
        }

        [System.Obsolete("Используем свойство Tables")]
        public iTable GetTable(string tableName)
        {
            iTable ret = null;
            _tables.TryGetValue(tableName, out ret);
            return ret;
        }

        public abstract void CopyFrom(CDatabase sourceDb, Action<int> progressCallback);
    }

    public class FieldDescription
    {
        [Flags]
        public enum fieldProp : uint
        {
            Empty = 1,
            Identity = 2,
            PrimaryKey = 4,
            MultiLang = 8,
            TimeStamp = 16,
            ForeignKey = 32,
            Nullable = 64,
            SectionKey = 128,
            ChangesLog = 256,
            All = 65535
        };

        public string Name;
        public PropertyInfo PropInfo;
        public string sType;
        public string SqlType;
        public int Size;
        public int Size2;
        public fieldProp Properties;
        public int Order;
        public int mlOrder;
        public string Comment;

        public FieldDescription()
        {
        }

        public FieldDescription(Type t, string name, string stype, string ftype, int size, int size2, fieldProp prop, string comment = "")
        {
            Name = name;
            sType = stype;
            SqlType = ftype;
            Size = size;
            Size2 = size2;
            Properties = prop;
            PropInfo = t.GetProperty(Name);
            if (PropInfo == null)
            {
                throw new Exception("В классе " + t.Name + " не определено поле " + Name);
            }
            Comment = comment;
        }

        public void ToJson(StringBuilder sb)
        {
            sb.Append("{");
            sb.AppendProperty("Name", Name);
            sb.Append(",");
            sb.AppendProperty("Comment", Comment);
            sb.Append("}");
        }
    }

}

