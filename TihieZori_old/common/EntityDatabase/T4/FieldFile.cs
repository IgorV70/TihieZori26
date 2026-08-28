// This is the output code from your template
// you only get syntax-highlighting here - not intellisense

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbCommon;
using System.Drawing;
using System.Xml.Serialization;


namespace EntityData{
  public partial class CTableField: CTable< Field, CDatabaseEntity,Guid>
  {
  	CTableField()
	{}
	        public CTableField(CDatabaseEntity db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Field), "Id", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(Field), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Field), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["TableId"] = new FieldDescription(typeof(Field), "TableId", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.ForeignKey );
			_fd["FType"] = new FieldDescription(typeof(Field), "FType", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Size"] = new FieldDescription(typeof(Field), "Size", "int", "int", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Size2"] = new FieldDescription(typeof(Field), "Size2", "int", "int", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["FTypeCS"] = new FieldDescription(typeof(Field), "FTypeCS", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["IsIdentity"] = new FieldDescription(typeof(Field), "IsIdentity", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsPrimary"] = new FieldDescription(typeof(Field), "IsPrimary", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsNullable"] = new FieldDescription(typeof(Field), "IsNullable", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["DefaultValue"] = new FieldDescription(typeof(Field), "DefaultValue", "string", "nvarchar", 0, 0, FieldDescription.fieldProp.Nullable );
			_fd["IsReadOnly"] = new FieldDescription(typeof(Field), "IsReadOnly", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsChangesLog"] = new FieldDescription(typeof(Field), "IsChangesLog", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsPostPoned"] = new FieldDescription(typeof(Field), "IsPostPoned", "bool", "bit", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["IsMultiLang"] = new FieldDescription(typeof(Field), "IsMultiLang", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["ForeignKeyTable"] = new FieldDescription(typeof(Field), "ForeignKeyTable", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["ForeignObjectName"] = new FieldDescription(typeof(Field), "ForeignObjectName", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["IsCashForeignObj"] = new FieldDescription(typeof(Field), "IsCashForeignObj", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Описание полей
	/// </summary>
	[XmlType("CField")]
	public partial class Field : BObject,IPrimaryKey<Guid> 
    {
        public Field() { }
        public Field(CTableField t)
            : base(t)
        { }

		private Guid _id = Guid.NewGuid();
		
		/// <summary>
		/// Первичный ключ, автонумератор
		/// </summary>
		[XmlAttribute]
        public Guid Id
        {
            get { return _id; }
            set { 
					SetProperty(ref _id, value); 
					OnPropertyChanged("Id");
				}
        }

		private string _name = "";
		
		/// <summary>
		/// Наименование параметра
		/// </summary>
		[XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { 
					SetProperty(ref _name, value); 
					OnPropertyChanged("Name");
				}
        }

		private string _comment = "";
		
		/// <summary>
		/// Комментарий
		/// </summary>
		[XmlAttribute]
        public string Comment
        {
            get { return _comment; }
            set { 
					SetProperty(ref _comment, value); 
					OnPropertyChanged("Comment");
				}
        }

		private Guid _tableId = Guid.Empty;
		
		/// <summary>
		/// Ссылка на таблицу
		/// </summary>
		[XmlAttribute]
        public Guid TableId
        {
            get { return _tableId; }
            set { 
					SetProperty(ref _tableId, value); 
					OnPropertyChanged("TableId");
				}
        }
		Table _tableIdObject ;
		/// <summary>
		/// ссылка на внешний объект(только для случая когда 1 первичный ключ)
		/// </summary>
		[XmlIgnore]
        public Table Table
        {
		
            get 
			{ 
				if (_tableIdObject == null)
					_tableIdObject = ((CDatabaseEntity)_table.parentDataBase).Table.GetById(_tableId);
				return _tableIdObject ; 
			}
            set 
			{ 
				if (value == null)
				{
					this.TableId = Guid.Empty ;
					this._tableIdObject = null ;
				}
				else
				{
					this.TableId = value.Id ;
					this._tableIdObject = value ;
				}
			}
        }

		private string _fType = "";
		
		/// <summary>
		/// Тип SQL
		/// </summary>
		[XmlAttribute]
        public string FType
        {
            get { return _fType; }
            set { 
					SetProperty(ref _fType, value); 
					OnPropertyChanged("FType");
				}
        }

		private int _size = 0;
		
		/// <summary>
		/// Размер
		/// </summary>
		[XmlAttribute]
        public int Size
        {
            get { return _size; }
            set { 
					SetProperty(ref _size, value); 
					OnPropertyChanged("Size");
				}
        }

		private int _size2 = 0;
		
		/// <summary>
		/// Размер2
		/// </summary>
		[XmlAttribute]
        public int Size2
        {
            get { return _size2; }
            set { 
					SetProperty(ref _size2, value); 
					OnPropertyChanged("Size2");
				}
        }

		private string _fTypeCS = null;
		
		/// <summary>
		/// Тип Cs
		/// </summary>
		[XmlAttribute]
        public string FTypeCS
        {
            get { return _fTypeCS; }
            set { 
					SetProperty(ref _fTypeCS, value); 
					OnPropertyChanged("FTypeCS");
				}
        }

		private bool _isIdentity = false;
		
		/// <summary>
		/// Автонумератор
		/// </summary>
		[XmlAttribute]
        public bool IsIdentity
        {
            get { return _isIdentity; }
            set { 
					SetProperty(ref _isIdentity, value); 
					OnPropertyChanged("IsIdentity");
				}
        }

		private bool _isPrimary = false;
		
		/// <summary>
		/// Входит в перв. ключ
		/// </summary>
		[XmlAttribute]
        public bool IsPrimary
        {
            get { return _isPrimary; }
            set { 
					SetProperty(ref _isPrimary, value); 
					OnPropertyChanged("IsPrimary");
				}
        }

		private bool _isNullable = false;
		
		/// <summary>
		/// Nullable
		/// </summary>
		[XmlAttribute]
        public bool IsNullable
        {
            get { return _isNullable; }
            set { 
					SetProperty(ref _isNullable, value); 
					OnPropertyChanged("IsNullable");
				}
        }

		private string _defaultValue = null;
		
		/// <summary>
		/// Значение по умолчанию
		/// </summary>
		[XmlAttribute]
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { 
					SetProperty(ref _defaultValue, value); 
					OnPropertyChanged("DefaultValue");
				}
        }

		private bool _isReadOnly = false;
		
		/// <summary>
		/// Только чтение
		/// </summary>
		[XmlAttribute]
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { 
					SetProperty(ref _isReadOnly, value); 
					OnPropertyChanged("IsReadOnly");
				}
        }

		private bool _isChangesLog = false;
		
		/// <summary>
		/// Логировать
		/// </summary>
		[XmlAttribute]
        public bool IsChangesLog
        {
            get { return _isChangesLog; }
            set { 
					SetProperty(ref _isChangesLog, value); 
					OnPropertyChanged("IsChangesLog");
				}
        }

		private bool _isPostPoned = false;
		
		/// <summary>
		/// Отложенное
		/// </summary>
		[XmlAttribute]
        public bool IsPostPoned
        {
            get { return _isPostPoned; }
            set { 
					SetProperty(ref _isPostPoned, value); 
					OnPropertyChanged("IsPostPoned");
				}
        }

		private bool _isMultiLang = false;
		
		/// <summary>
		/// Мультиязыковое
		/// </summary>
		[XmlAttribute]
        public bool IsMultiLang
        {
            get { return _isMultiLang; }
            set { 
					SetProperty(ref _isMultiLang, value); 
					OnPropertyChanged("IsMultiLang");
				}
        }

		private string _foreignKeyTable = null;
		
		/// <summary>
		/// Ссылка на таблицу
		/// </summary>
		[XmlAttribute]
        public string ForeignKeyTable
        {
            get { return _foreignKeyTable; }
            set { 
					SetProperty(ref _foreignKeyTable, value); 
					OnPropertyChanged("ForeignKeyTable");
				}
        }

		private string _foreignObjectName = null;
		
		/// <summary>
		/// Ссылка объект
		/// </summary>
		[XmlAttribute]
        public string ForeignObjectName
        {
            get { return _foreignObjectName; }
            set { 
					SetProperty(ref _foreignObjectName, value); 
					OnPropertyChanged("ForeignObjectName");
				}
        }

		private bool _isCashForeignObj = false;
		
		/// <summary>
		/// Кешировать объект
		/// </summary>
		[XmlAttribute]
        public bool IsCashForeignObj
        {
            get { return _isCashForeignObj; }
            set { 
					SetProperty(ref _isCashForeignObj, value); 
					OnPropertyChanged("IsCashForeignObj");
				}
        }
    }

}
	
