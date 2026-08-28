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
  public partial class CTableRelation: CTable< Relation, CDatabaseEntity,Guid>
  {
  	CTableRelation()
	{}
	        public CTableRelation(CDatabaseEntity db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Relation), "Id", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(Relation), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Relation), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["TableId"] = new FieldDescription(typeof(Relation), "TableId", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.ForeignKey );
			_fd["RType"] = new FieldDescription(typeof(Relation), "RType", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["PrimaryKeyTable"] = new FieldDescription(typeof(Relation), "PrimaryKeyTable", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["PrimaryKeyFieldList"] = new FieldDescription(typeof(Relation), "PrimaryKeyFieldList", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["ForeignKeyTable"] = new FieldDescription(typeof(Relation), "ForeignKeyTable", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["ForeignKeyFieldList"] = new FieldDescription(typeof(Relation), "ForeignKeyFieldList", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Описание связей
	/// </summary>
	[XmlType("CRelation")]
	public partial class Relation : BObject,IPrimaryKey<Guid> 
    {
        public Relation() { }
        public Relation(CTableRelation t)
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

		private string _rType = "";
		
		/// <summary>
		/// Тип связи
		/// </summary>
		[XmlAttribute]
        public string RType
        {
            get { return _rType; }
            set { 
					SetProperty(ref _rType, value); 
					OnPropertyChanged("RType");
				}
        }

		private string _primaryKeyTable = "";
		
		/// <summary>
		/// Таблица с ключом
		/// </summary>
		[XmlAttribute]
        public string PrimaryKeyTable
        {
            get { return _primaryKeyTable; }
            set { 
					SetProperty(ref _primaryKeyTable, value); 
					OnPropertyChanged("PrimaryKeyTable");
				}
        }

		private string _primaryKeyFieldList = "";
		
		/// <summary>
		/// Название поля ключа
		/// </summary>
		[XmlAttribute]
        public string PrimaryKeyFieldList
        {
            get { return _primaryKeyFieldList; }
            set { 
					SetProperty(ref _primaryKeyFieldList, value); 
					OnPropertyChanged("PrimaryKeyFieldList");
				}
        }

		private string _foreignKeyTable = "";
		
		/// <summary>
		/// Таблица со ссылкой
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

		private string _foreignKeyFieldList = "";
		
		/// <summary>
		/// Поле со ссылкой
		/// </summary>
		[XmlAttribute]
        public string ForeignKeyFieldList
        {
            get { return _foreignKeyFieldList; }
            set { 
					SetProperty(ref _foreignKeyFieldList, value); 
					OnPropertyChanged("ForeignKeyFieldList");
				}
        }
    }

}
	
