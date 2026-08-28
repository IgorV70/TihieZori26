using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DbCommon.Metadata
{
    public class CField : DictVComment
    {
        public CField()
        { }

        public CField(CTable tbl)
        {
            Table = tbl;
        }

        [XmlIgnore]
        public CTable Table;
        [XmlAttribute]
        public string ParentInterfaceName;
        [XmlAttribute]
        public string FType;
        [XmlAttribute]
        public string FTypeCS = null;
        [XmlAttribute]
        public int Size;
        [XmlAttribute]
        public int Size2;
        [XmlIgnore]
        public string SizeName
        {
            get
            {
                if (FType == "nvarchar" || FType == "varbinary")
                {
                    if (Size == 0) return "[" + FType + "] (MAX)";
                    return "[" + FType + "] (" + Size + ")";
                }
                if (Size == 0) return "[" + FType + "]";
                if (Size2 == 0) return "[" + FType + "] (" + Size + ")";
                return "[" + FType + "] (" + Size + "," + Size2 + ")";
            }
        }
        [XmlIgnore]
        public string SizeName2
        {
            get
            {
                if (FType == "decimal")
                {
                    return "decimal (" + Size + "," + Size2 + ")";
                }
                if (FType == "nvarchar")
                {
                    return "varchar (" + Size + ")";
                }
                if (Size == 0) return FType;
                return FType + " (" +  Size + ")";
            }
        }
        [XmlAttribute]
        public bool IsIdentity;
        [XmlAttribute]
        public bool IsPrimary;
        [XmlAttribute]
        public bool IsNullable;
        [XmlAttribute]
        public string DefaultValue;
        [XmlAttribute]
        public string ForeignKeyTable;
        [XmlAttribute]
        public bool IsPostPoned;
        [XmlAttribute]
        public bool IsChangesLog;
        [XmlAttribute]
        public bool IsMultiLang;

        [XmlAttribute]
        public string ForeignObjectName;

        [XmlAttribute]
        public bool IsCashForeignObj;
        [XmlIgnore]
        public string _name
        {
            get
            {
                string Name = this.Name;
                Name = "_" + Name.Substring(0, 1).ToLower() + Name.Substring(1);
                return Name;
            }
        }      

        [XmlIgnore]
        public string _typeDefaultValue
        {
            get
            {
                string ftype = FType;
                switch (ftype)
                {
                    case "int": return DefaultValue;
                    case "nvarchar": return "\"" + DefaultValue + "\"";
                    case "ntext": return "\"" + DefaultValue + "\"";
                    case "varchar": return "\"" + DefaultValue + "\"";
                    case "text": return "\"" + DefaultValue + "\"";
                    case "decimal": return DefaultValue;
                    //case "smalldatetime": return "DateTime.Now";
                    //case "datetime": return "new DateTime(0)";
                    case "bit": return DefaultValue == "0" ? "false" : "true";
                    case "varbinary": return "null";
                    case "Guid": return "Guid.Empty";

                }
                return "unknown";
            }
        }

        /// <summary>
        /// Pзначение по умолчанию для поля, для C#
        /// </summary>
        [XmlIgnore]
        public string _sharpDefaultValue
        {
            get
            {
                if (IsNullable)
                {
                    string ftype = FType;
                    switch (ftype)
                    {
                        case "int": return "null";
                        case "nvarchar": return "null";
                        case "ntext": return "null";
                        case "varchar": return "null";
                        case "text": return "null";
                        case "decimal": return "null";
                        case "smalldatetime": return "null";
                        case "datetime": return "null";
                        case "bit": return "false";
                        case "varbinary": return "null";
                        case "image": return "null";
                        case "uniqueidentifier": return "null";
                    }
                    return "unknown";
                }
                if (string.IsNullOrEmpty(DefaultValue))
                {
                    string ftype = FType;
                    switch (ftype)
                    {
                        case "int": return "0";
                        case "nvarchar": return "\"\"";
                        case "ntext": return "\"\"";
                        case "varchar": return "\"\"";
                        case "text": return "\"\"";
                        case "decimal": return "0";
                        case "smalldatetime": return "DateTime.Now";
                        case "datetime": return "DateTime.Now";
                        case "bit": return "false";
                        case "varbinary": return "null";
                        case "image": return "null";
                        case "uniqueidentifier": return "Guid.Empty";

                    }
                    return "unknown";
                }
                else
                {
                    string ftype = FType;
                    switch (ftype)
                    {
                        case "int": return DefaultValue;
                        case "nvarchar": return "\"" + DefaultValue + "\"";
                        case "ntext": return "\"" + DefaultValue + "\"";
                        case "varchar": return "\"" + DefaultValue + "\"";
                        case "text": return "\"" + DefaultValue + "\"";
                        case "decimal": return DefaultValue + "m";
                        //case "smalldatetime": return "DateTime.Now";
                        //case "datetime": return "new DateTime(0)";
                        case "bit": return DefaultValue == "0" ? "false" : "true";
                        case "varbinary": return "null";
                        case "uniqueidentifier": return "Guid.NewGuid()";
                    }
                    return "unknown";
                }
            }
        }


        [XmlIgnore]
        public string _sharpType
        {
            get
            {
                if (!string.IsNullOrEmpty(FTypeCS))
                    return (FTypeCS);

                string ftype = FType;
                string ret = "unknown";
                string nn = IsNullable ? "?" : "";
                switch (ftype)
                {
                    case "int": ret = "int" + nn; break;
                    case "nvarchar": ret = "string"; break;
                    case "ntext": ret = "string"; break;
                    case "varchar": ret = "string"; break;
                    case "text": ret = "string"; break;

                    case "decimal": ret = "decimal" + nn; break;
                    case "smalldatetime": ret = "DateTime" + nn; break;
                    case "datetime": ret = "DateTime" + nn; break;

                    case "bit": ret = "bool" + nn; break;
                    case "varbinary": ret = "Image"; break;
                    case "uniqueidentifier": return "Guid" + nn;
                }
                return ret;
            }
        }

        public string WritePropList()
        {
            String ret = "";
            bool prev = IsIdentity;
            if (IsIdentity)
                ret += "FieldDescription.fieldProp.Identity";
            if (IsPrimary)
            {
                if (prev) ret += " | "; prev = true;
                ret += "FieldDescription.fieldProp.PrimaryKey ";
            }
            if (IsMultiLang)
            {
                if (prev) ret += " | "; prev = true;
                ret += "FieldDescription.fieldProp.MultiLang ";
            }
            if (IsNullable)
            {
                if (prev) ret += " | "; prev = true;
                ret += "FieldDescription.fieldProp.Nullable ";
            }
            if (IsPostPoned)
            {
                if (prev) ret += " | "; prev = true;
                ret += "FieldDescription.fieldProp.PostPoned ";
            }
            if (IsChangesLog)
            {
                if (prev) ret += " | "; prev = true;
                ret += "FieldDescription.fieldProp.ChangesLog ";
            }
            if (!string.IsNullOrEmpty(ForeignKeyTable))
            {
                if (prev) ret += " | "; prev = true;
                ret += "FieldDescription.fieldProp.ForeignKey ";
            }

            if (ret == "")
                ret = "FieldDescription.fieldProp.Empty";
            return ret;
        }

    }
    public class CRelation : DictVComment
    {
        public CRelation()
        { }

        public CRelation(string name)
        {
            Name = name;
        }

        [XmlAttribute]
        public string RType;


        [XmlAttribute]
        public string PrimaryKeyTable;

        [XmlAttribute]
        public string ForeignKeyTable;

        [XmlAttribute]
        public string PrimaryKeyFieldList;

        [XmlAttribute]
        public string ForeignKeyFieldList;

    }

    public class CTable : DictVComment
    {
        [XmlAttribute]
        public string TableName;

        [XmlAttribute]
        public string xmlType;

        [XmlAttribute]
        public string TableName_select;

        private List<CField> _fldlist;
        [XmlArray("FieldList")]
        public List<CField> FieldList
        {
            get
            {
                if (_fldlist == null)
                    _fldlist = new List<CField>();
                return _fldlist;
            }
            set
            { _fldlist = value; }
        }

        private List<CRelation> _rellist;
        [XmlArray("RelationList")]
        public List<CRelation> RelationList
        {
            get
            {
                if (_rellist == null)
                {
                    _rellist = new List<CRelation>();
                }
                return _rellist;
            }
            set
            {
                _rellist = value;
            }
        }


        [XmlAttribute]
        public string CashType;

        [XmlAttribute]
        public bool IsPostPoned;

        [XmlAttribute]
        public bool IsChangesLog;

        [XmlAttribute]
        public bool IsB1Object;

        public CTable()
        {
            PrimaryKeyType = "int";
        }

        [XmlIgnore]
        public bool HavePostPonedField
        {
            get
            {
                foreach (CField f in FieldList)
                    if (f.IsPostPoned)
                        return true;
                return false;
            }
        }

        [XmlAttribute]
        public string PrimaryKeyType { get; set; }

        public string WriteFieldNameList()
        {
            String ret = "";
            bool prev = false;
            foreach (CField f in FieldList)
            {
                if (f.IsPostPoned) continue;
                if (prev) ret += " , "; prev = true;
                ret += f.Name;
            }
            return ret;
        }

    }

    public class Dict
    {
        [XmlAttribute]
        public string Id;
        [XmlAttribute]
        public string Name;
    }
    public class DictVComment : Dict
    {
        [XmlAttribute]
        public string Comment;
    }

    public class CDataBase : DictVComment
    {
        private List<CTable> _tbllist;
        [XmlArray("TableList")]
        public List<CTable> TableList
        {
            get
            {
                if (_tbllist == null)
                    _tbllist = new List<CTable>();
                return _tbllist;
            }
            set
            { _tbllist = value; }
        }
        [XmlElement("TableLang")]
        public string TableLang;
        [XmlElement("TableCaption")]
        public string TableCaption;
        [XmlElement("TableLocString")]
        public string TableLocString;

    }


    [XmlRoot("DataProject", Namespace = "http://www.atpm-air.ru/dpm", IsNullable = false)]
    public class CProject
    {
        private List<CDataBase> _dblist;
        [XmlArray("BaseList")]
        public List<CDataBase> DataBaseList
        {
            get
            {
                if (_dblist == null) _dblist = new List<CDataBase>();
                return _dblist;
            }
            set
            {
                _dblist = value;
            }
        }
    }
}