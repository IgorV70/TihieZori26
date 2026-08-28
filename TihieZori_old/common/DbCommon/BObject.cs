using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using DbCommon.Attributes;
using DbCommon.Helpers;

namespace DbCommon
{
    public delegate void ShowErrorMessageMethod(string Message);

    public abstract class BObject : IBObject, ICloneable, IEditableObject, INotifyPropertyChanged
    {
        public BObject() { }

        public static ShowErrorMessageMethod ShowErrorMessage = null;

        [XmlIgnore]
        public iTable _table = null;

        public BObject(iTable t)
        {
            _table = t;
        }

        [XmlIgnore]
        CDatabase IBObject.Database
        {
            get
            {
                return _table.parentDataBase;
            }
        }

        public override int GetHashCode()
        {
            if (_table == null)
                return base.GetHashCode();
            // используем только первичный ключ
            int ret = 0;
            foreach (FieldDescription f in _table.Columns.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.PrimaryKey) > 0))
                ret ^= f.PropInfo.GetValue(this, null).GetHashCode();
            return ret ^ base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // сравниваем только первичный ключ
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;

            //дальше все неправильно - надо думать
            //throw new NotImplementedException("Equals - недоделано");
            if (!_IsPersistent) return false;
            foreach (FieldDescription f in _table.Columns.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.PrimaryKey) > 0))
            {
                if (!f.PropInfo.GetValue(this, null).Equals(f.PropInfo.GetValue(obj, null)))
                    return false;
            }
            return false;
            //else
            //    if (_table != null)
            //    {
            //        foreach (FieldDescription f in _table.Columns.Values)
            //        {
            //            if (Compare(f.PropInfo.GetValue(this, null), f.PropInfo.GetValue(obj, null)) != 0)
            //                return false;
            //        }
            //    }
            //    else
            //        return ((BObject)obj)._table == null;
            //return true;
        }

        private static Hashtable _holders = new Hashtable();
        //private static Hashtable _instances = new Hashtable();
        //private static Hashtable _instancesDesc = new Hashtable();

        public enum ObjectStateType { Unchanged, Added, Changed, Deleted };

        private bool _IsPersistent = false;
        private bool _IsLoaded = false;
        protected BObject persist = null;


        //        public SortList<BObject> ParentList = null;
        [XmlIgnore]
        public bool IsPersistent { get { return _IsPersistent; } set { _IsPersistent = value; } }
        [XmlIgnore]
        public bool IsLoaded { get { return _IsLoaded; } set { _IsLoaded = value; } }
        [XmlIgnore]
        public virtual bool PosponeFieldsIsLoad { get { return false; } set { } }

        protected ObjectStateType _objectState = ObjectStateType.Unchanged;

        [XmlIgnore]
        public virtual ObjectStateType ObjectState
        {
            get { return _objectState; }
            set
            {
                switch (value)
                {
                    case ObjectStateType.Changed:
                        if (persist == null)
                            persist = (BObject)this.Clone();
                        break;
                    case ObjectStateType.Unchanged:
                        persist = null;
                        break;
                }
                _objectState = value;
            }
        }

        /// <summary>
        /// Отмена изменений в объекте
        /// </summary>
        public virtual void Rollback()
        {
            BObject o = null;
            switch (_objectState)
            {
                case ObjectStateType.Unchanged:
                    return;
                case ObjectStateType.Changed:
                    o = persist;
                    break;
                case ObjectStateType.Added:
                    o = (BObject)Activator.CreateInstance(this.GetType());
                    break;
                case ObjectStateType.Deleted:
                    if (this.IsPersistent)
                    {
                        if (persist == null)
                        {
                            this.ObjectState = ObjectStateType.Unchanged;
                            return;
                        }
                        o = persist;
                    }
                    else
                        o = (BObject)Activator.CreateInstance(this.GetType()); ;
                    break;
                default:
                    break;
            }
            RestoreFrom(o);
            this.ObjectState = ObjectStateType.Unchanged;
        }

        /// <summary>
        /// Копирует значения obj в свои, поименно, если типы разные
        /// </summary>
        /// <param name="obj"></param>
        public void RestoreFrom(BObject obj)
        {
            if (GetType().Equals(obj.GetType()))
            {
                this.IsLoaded = true;
                foreach (FieldDescription f in _table.Columns.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.Identity) == 0))
                    f.PropInfo.SetValue(this, f.PropInfo.GetValue(obj, null), null);
                this.IsLoaded = false;
            }
            else
            {
                this.IsLoaded = true;
                foreach (FieldDescription f in _table.Columns.Values.Where(fd => (fd.Properties & FieldDescription.fieldProp.Identity) == 0))
                {
                    string fname = f.Name;
                    FieldDescription ffrom = obj._table.Columns.Values.FirstOrDefault(ff => ff.Name == fname);
                    if (ffrom != null)
                        f.PropInfo.SetValue(this, ffrom.PropInfo.GetValue(obj, null), null);
                }
                this.IsLoaded = false;

            }

        }

        public virtual void MarkForDeletion()
        {
            this.ObjectState = ObjectStateType.Deleted;
            //if (Parent != null)
            //    Parent. +=  new EventHandler();
        }


        /// <summary>
        /// собирает строку - условие с первичным ключом
        /// </summary>
        public object GetKeyString()
        {
            //if (this is B1Object<>)
            //    return ((B1Object<>)this).Id;
            string hashstring = "";
            foreach (FieldDescription key in _table.GetPrimaryKeysList())
                hashstring += "_" + key.PropInfo.GetValue(this, null).ToString();
            return hashstring;
        }

        /// <summary>
        /// Сохранение данных с диалогом об ошибке
        /// </summary>
        public bool SaveTry()
        {
            try
            {
                this.Save2();
                return true;
            }
            catch (BOSaveExeption ex)
            {
                ex.Show();
                throw new Exception("BOSaveExeption", ex);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "SaveTry");
                if (ShowErrorMessage != null)
                    ShowErrorMessage(ex.Message);
                throw new Exception("BOSaveExeption", ex);
            }
        }


        public void SetDirtyFlag()
        {

            //if (this.Parent != null)
            //{
            //    this.Parent.CollectionChanged(this);
            //}
            if (this.IsLoaded) return; //объект загружается или инициализируется
            if (this.IsPersistent) //объект сохранен в БД
                this.ObjectState = ObjectStateType.Changed; //объект изменился
            else
                this.ObjectState = ObjectStateType.Added;// объект добавлен
        }

        #region Члены IEditableObject
        // Implements IEditableObject
        private bool IsNew = true;
        public void BeginEdit()
        {
        }
        public void EndEdit()
        {
            IsNew = false;
        }
        public void CancelEdit()
        {
            if (IsNew)
            {
                IsNew = false;
            }
        }
        #endregion

        public event EventHandler PropertyDataChanged;

        protected virtual void OnPropertyDataChanged(EventArgs ev)
        {
            if (PropertyDataChanged != null)
                PropertyDataChanged(this, ev);
        }


        protected void SetProperty<TT>(ref TT intVar, TT Val)
        {
            if (Compare(intVar, Val) != 0)
            {
                SetDirtyFlag();
                intVar = Val;
                OnPropertyDataChanged(null);
            }
        }

        protected void SetProperty(ref int intVar, int Val)
        {
            if (intVar != Val)
            {
                SetDirtyFlag();
                intVar = Val;
                OnPropertyDataChanged(null);
            }
        }

        protected void SetProperty(ref bool intVar, bool Val)
        {
            if (intVar != Val)
            {
                SetDirtyFlag();
                intVar = Val;
                OnPropertyDataChanged(null);
            }
        }

        protected void SetProperty(ref string intVar, string Val)
        {
            if (intVar != Val)
            {
                SetDirtyFlag();
                intVar = Val;
                OnPropertyDataChanged(null);
            }
        }

        public static int Compare(object obj1, object obj2)
        {
            if (obj1 == null)
                return obj2 == null ? 0 : -1;
            if (obj2 == null) return 1;
            if (obj1 is IComparable comp)
                return comp.CompareTo(obj2);
            return obj1 == obj2 ? 0 : -1;
        }

        public virtual object DeepClone(int deep = 99)
        {
            return Clone();
        }

        #region Члены ICloneable

        public virtual object Clone()
        {
            if (_table != null)
            {
                BObject ret = (BObject)this._table.CreateInstance();
                ret.RestoreFrom(this);
                return ret;
            }
            return this.MemberwiseClone();

        }

        #endregion

        public BObject GetPersist()
        {
            return persist;
        }

        public void _write(BinaryWriter bw)
        {
            foreach (FieldDescription s in _table.Columns.Values)
            {
                PropertyInfo p = s.PropInfo;
                object value = p.GetValue(this, null);
                Type t = p.PropertyType;

                if (t.Name.StartsWith("Nullable"))
                {
                    bw.Write(value != null);
                    if (value == null)
                        continue;
                    t = Nullable.GetUnderlyingType(t);
                }

                switch (t.Name)
                {
                    case "Int32":
                        bw.Write((Int32)value);
                        break;
                    case "String":
                        bw.Write(value != null);
                        if (value != null)
                            bw.Write((string)value);
                        break;
                    case "Decimal":
                        bw.Write((Decimal)value);
                        break;
                    case "Boolean":
                        bw.Write((Boolean)value);
                        break;
                    case "Bitmap":
                        if (value == null)
                            bw.Write((Int32)0);
                        else
                        {
                            MemoryStream ms = new MemoryStream();
                            ((Bitmap)value).Save(ms, ImageFormat.Jpeg);
                            bw.Write((Int32)ms.Length);
                            bw.Write(ms.GetBuffer());
                        }
                        break;
                    //case "DDDImage":
                    //    bw.Write(value != null);
                    //    if (value != null)
                    //    {
                    //        byte[] zip_data = ((DDDImage)value).SaveZip();
                    //        bw.Write(zip_data.Length);
                    //        bw.Write(((DDDImage)value).SaveZip());
                    //    }
                    //    break;
                    case "UInt64":
                        bw.Write((UInt64)value);
                        break;
                    case "Byte[]":
                        bw.Write(value != null);
                        if (value != null)
                        {
                            bw.Write(((Byte[])value).Length);
                            bw.Write((Byte[])value);
                        }
                        break;
                    case "DateTime":
                        bw.Write(((DateTime)value).ToBinary());
                        break;
                    default:
                        throw new Exception("Не реализвано сохранение типа в поток:" + " " + t.Name);
                }
            }
        }

        public Boolean _read(BinaryReader br)
        {
            foreach (FieldDescription s in _table.Columns.Values)
            {
                PropertyInfo p = s.PropInfo;
                this._IsLoaded = true;
                object value;// = p.GetValue(this, null);
                bool _isnotnull = false;
                Type t = p.PropertyType;

                if (t.Name.StartsWith("Nullable"))
                {
                    _isnotnull = br.ReadBoolean();
                    if (!_isnotnull)
                    {
                        p.SetValue(this, null, null);
                        continue;
                    }
                    t = Nullable.GetUnderlyingType(t);
                }

                switch (t.Name)
                {
                    case "Int32":
                        value = br.ReadInt32();
                        break;
                    case "String":
                        _isnotnull = br.ReadBoolean();
                        value = _isnotnull ? br.ReadString() : null;
                        break;
                    case "Decimal":
                        value = br.ReadDecimal();
                        break;
                    case "Boolean":
                        value = br.ReadBoolean();
                        break;
                    case "Bitmap":
                        int len = br.ReadInt32();
                        if (len > 0)
                        {
                            byte[] buf = br.ReadBytes(len);
                            MemoryStream ms = new MemoryStream(buf);
                            Bitmap b = (Bitmap)Image.FromStream(ms);
                            value = b;
                        }
                        else
                            value = null; break;
                    //case "DDDImage":
                    //    _isnotnull = br.ReadBoolean();
                    //    if (_isnotnull)
                    //    {
                    //        value = AOS_OpenGL.DDDImage.FromStream2(
                    //            new MemoryStream(Zlib.ZipBase.Uncompress(br.ReadBytes(br.ReadInt32()))));
                    //    }
                    //    else
                    //        value = null; break;
                    case "UInt64":
                        value = br.ReadUInt64();
                        break;
                    case "Byte[]":
                        value = br.ReadBoolean() ? br.ReadBytes(br.ReadInt32()) : null;
                        break;
                    case "DateTime":
                        value = new DateTime(br.ReadInt64());
                        break;
                    default:
                        throw new Exception("Не реализвано чтение типа из потока:" + " " + t.Name);
                }
                p.SetValue(this, value, null);
            }
            IsLoaded = false;
            return true;
        }



        /// <summary>
        /// Сохранение данных, в нем реализуем проверку целостности
        /// </summary>
        public virtual void Save2()
        {
            if (this._objectState == ObjectStateType.Unchanged)
                return;
            _table.Save2(this);

        }


        public delegate void ObjectStateEventHandler(object sender, ObjectStateEventArgs e);
        public event ObjectStateEventHandler ObjectStateEvent;

        protected virtual void RaiseObjectStateEvent(BObject.ObjectStateType new_state)
        {
            if (ObjectStateEvent != null)
                ObjectStateEvent(this, new ObjectStateEventArgs(new_state));
        }

        public void Update(string sFields)
        {
            _table.parentDataBase.UpdateObject(this, sFields);
        }

        /// <summary>
        /// Возвращает список изменений подлежащих логированию
        /// </summary>
        public List<ChangedValuesItem> GetChangedValuesToLog()
        {
            List<ChangedValuesItem> result = null;

            foreach (FieldDescription fd in _table.Columns.Values)
            {
                if ((fd.Properties & FieldDescription.fieldProp.ChangesLog) > 0 && persist != null)
                {
                    object nVal = fd.PropInfo.GetValue(this, null);
                    object oVal = fd.PropInfo.GetValue(persist, null);
                    if (BObject.ValidCompare(nVal, oVal) != 0)
                    {
                        if (result == null)
                        {
                            result = new List<ChangedValuesItem>();
                        }
                        ChangedValuesItem item = new ChangedValuesItem();
                        item.TableName = this._table.TableName();
                        item.ColumnName = fd.Name;
                        item.ValueNew = (nVal == null ? "" : nVal.ToString());
                        item.ValueOld = (oVal == null ? "" : oVal.ToString());
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Наименование записи для логирования изменений
        /// </summary>
        public virtual string LogRecordName()
        {
            return string.Empty;
        }

        /// <summary>
        /// Определяет будут ли при сохранении 
        /// </summary>
        public bool IsChangesToLog()
        {
            if (!_table.IsChangesLog || ObjectState == ObjectStateType.Unchanged)
            {
                return false;
            }

            if (ObjectState == ObjectStateType.Added || ObjectState == ObjectStateType.Deleted)
            {
                return true;
            }

            foreach (FieldDescription fd in _table.Columns.Values)
            {
                if ((fd.Properties & FieldDescription.fieldProp.ChangesLog) > 0)
                {
                    object nVal = fd.PropInfo.GetValue(this, null);
                    object oVal = fd.PropInfo.GetValue(persist, null);
                    if (BObject.ValidCompare(nVal, oVal) != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Откорректированный варинт метода Compare.
        /// Считает что пустые строки и null равнозначны.
        /// </summary>
        public static int ValidCompare(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return 0;
            }

            if (obj1 == null && obj2 != null)
            {
                if (obj2 is string && (string)obj2 == string.Empty)
                    return 0;
                return -1;
            }

            if (obj1 != null & obj2 == null)
            {
                if (obj1 is string && (string)obj1 == string.Empty)
                    return 0;
                return 1;
            }

            if (obj1 == obj2)
            {
                return 0;
            }

            if (obj1.Equals(obj2))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public List<FieldDescription> FieldStringToList(string fields)
        {
            iTable t = this._table;
            var columns = t.Columns.Values.ToList();
            // отфильтруем нужные поля
            if (!string.IsNullOrEmpty(fields))
            {
                string[] fieldArr = fields.ToLower().Split(',');
                columns = fieldArr.Select(fname => columns.FirstOrDefault(fdesc => fdesc.Name.ToLower() == fname)).Where(fd => fd != null).ToList();
            }
            return columns;
        }

        public virtual string ToJson(string fields = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (FieldDescription fd in FieldStringToList(fields))
            {
                sb.Append("\"");
                sb.Append(fd.Name);
                sb.Append("\"");
                sb.Append(":");
                ConvertValueToJson(fd.PropInfo.GetValue(this, null), fd.Properties, sb);
                sb.Append(",");
            }
            sb.Length--;
            sb.Append("}");
            return sb.ToString();
        }

        private static void QuoteJson(string p, StringBuilder sb)
        {
            foreach (char c in p)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '"': sb.Append("\\\""); break;
                    default: sb.Append(c); break;
                }
            }
        }

        private static void ConvertValueToJson(Object value, FieldDescription.fieldProp pi, StringBuilder sb)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }
            Type vt = value.GetType();
            if (vt.Equals(typeof(String)))
            {
                sb.Append("\"");
                QuoteJson((string)value, sb);
                sb.Append("\"");
                return;
            }
            if (vt.Equals(typeof(int)))
            {
                sb.Append((int)value);
                return;
            }
            if (vt.Equals(typeof(DateTime)))
            {
                DateTime dtValue = (DateTime)value;
                int posix = (int)(dtValue - (new DateTime(1970, 1, 1))).TotalSeconds;
                sb.Append(posix.ToString("D"));
                return;
            }
            if (vt.Equals(typeof(Boolean)))
            {
                sb.Append(((Boolean)value) ? 1 : 0);
                return;
            }
            //if (vt.Equals(typeof(Bitmap)))
            //{
            //}
            //if (vt.Equals(typeof(AOS_OpenGL.DDDImage)))
            //{
            //}
            //if (vt.Equals(typeof(Int16[])))
            //{
            //}
            //if (vt.Equals(typeof(byte[])))
            //{
            //}
            if (vt.IsEnum)
            {
                //sb.Append(Convert.ToInt32(value));
                sb.Append("\"");
                sb.Append(value);
                sb.Append("\"");
                return;
            }
            if (vt.Equals(typeof(decimal)))
            {
                string s = ((decimal)value).ToString("0.000000");
                sb.Append(s.Replace(',', '.'));
                return;
            }
            sb.Append(value);
        }

        public StringBuilder ToJsonArray(StringBuilder sb = null, string fields = null)
        {
            sb = sb ?? new StringBuilder();
            sb.Append("[");
            foreach (FieldDescription fd in FieldStringToList(fields))
            {
                ConvertValueToJson(fd.PropInfo.GetValue(this, null), fd.Properties, sb);
                sb.Append(",");
            }
            foreach (var attr in _table.GetRowType().GetCustomAttributes(true))
            {
                var dopField = attr as AdditionalJsonPropertyAttribute;
                if (dopField != null)
                {
                    var PropInfo = _table.GetRowType().GetProperty(dopField.PropertyName);
                    if (PropInfo != null)
                        sb.Append(PropInfo.GetValue(this, null)).Append(",");
                }
            }
            sb.Length--;
            sb.Append("]");
            return sb;
        }

        public void UpdateFrom(Dictionary<string, string> dictionary)
        {
            foreach (var fd in _table.Columns.Values)
            {
                if ((fd.Properties & FieldDescription.fieldProp.Identity) > 0)
                    continue;
                if (!dictionary.ContainsKey(fd.Name)) continue;
                string value = dictionary[fd.Name];
                if (value == null) continue;
                if (fd.PropInfo.PropertyType == typeof(int))
                {
                    int ival = 0;
                    if (int.TryParse(value, out ival))
                        fd.PropInfo.SetValue(this, ival);
                    continue;
                }
                if (fd.PropInfo.PropertyType == typeof(DateTime))
                {
                    DateTime dt;
                    if (!DateTime.TryParse(value, out dt))
                    {
                        int posix;
                        if (!int.TryParse(value, out posix))
                            continue;
                        dt = new DateTime(1970, 1, 1).AddSeconds(posix);
                    }
                    fd.PropInfo.SetValue(this, dt);
                    continue;
                }
                if (fd.PropInfo.PropertyType == typeof(Boolean))
                {
                    bool bval = (value == "1");
                    fd.PropInfo.SetValue(this, bval);
                    continue;
                }
                if (fd.PropInfo.PropertyType.IsEnum)
                {
                    var enumValue = Enum.Parse(fd.PropInfo.PropertyType, value);
                    fd.PropInfo.SetValue(this, enumValue);
                    continue;
                }
                if (fd.PropInfo.PropertyType == typeof(Decimal))
                {
                    Decimal ival = 0;
                    if (Decimal.TryParse(value, out ival))
                        fd.PropInfo.SetValue(this, ival);
                    continue;
                }
                fd.PropInfo.SetValue(this, value);
            }
        }

        public IISortList<BObject> GetListProperty(string propetyName)
        {
            PropertyInfo piList = this.GetType().GetProperty(propetyName);
            object ret = piList.GetValue(this);
            return ret as IISortList<BObject>;
        }

        //  ViewModelBase : INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public virtual void Save2Log()
        {
            ObjectStateType state = this._objectState;
            if (state == ObjectStateType.Unchanged)
                return;
            ITableLogger tl = _table.parentDataBase as ITableLogger;
            if (tl == null || !(this is IHasPrimaryKey))
            {
                _table.Save2(this);
                return;
            }


            List<ChangedValuesItem> changes = null;
            if (state == ObjectStateType.Changed)
                changes = GetChangedValuesToLog();

            _table.Save2(this);


            if (changes != null)
            {
                // пока вот-так по жесткому : IPrimaryKey<int>
                tl.SaveLog(changes, ((IPrimaryKey<int>)this).Id, LogRecordName());
            }
            else
                if (state == ObjectStateType.Added || state == ObjectStateType.Deleted)
            {
                changes = new List<ChangedValuesItem>
                    {
                        new ChangedValuesItem
                        {
                            TableName = this._table.TableName(),
                            RecordNew = state == ObjectStateType.Added,
                            RecordDelete = state == ObjectStateType.Deleted
                        }
                    };

                tl.SaveLog(changes, ((IPrimaryKey<int>)this).Id, LogRecordName());
            }

        }

    }
}