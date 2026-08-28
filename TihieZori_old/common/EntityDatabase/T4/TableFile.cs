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
  public partial class CTableTable: CTable< Table, CDatabaseEntity,Guid>
  {
  	CTableTable()
	{}
	        public CTableTable(CDatabaseEntity db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Table), "Id", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(Table), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["xmlType"] = new FieldDescription(typeof(Table), "xmlType", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["PrimaryKeyType"] = new FieldDescription(typeof(Table), "PrimaryKeyType", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Table), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["DatabaseId"] = new FieldDescription(typeof(Table), "DatabaseId", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.ForeignKey );
			_fd["CashType"] = new FieldDescription(typeof(Table), "CashType", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["TableName"] = new FieldDescription(typeof(Table), "TableName", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["IsInterface"] = new FieldDescription(typeof(Table), "IsInterface", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsReadOnly"] = new FieldDescription(typeof(Table), "IsReadOnly", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsB1Object"] = new FieldDescription(typeof(Table), "IsB1Object", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsDict"] = new FieldDescription(typeof(Table), "IsDict", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsComment"] = new FieldDescription(typeof(Table), "IsComment", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["IsChangesLog"] = new FieldDescription(typeof(Table), "IsChangesLog", "bool", "bit", 0, 0, FieldDescription.fieldProp.Empty);
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Таблицы
	/// </summary>
	[XmlType("CTable")]
	public partial class Table : BObject,IPrimaryKey<Guid> 
    {
        public Table() { }
        public Table(CTableTable t)
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
		/// Имя таблицы
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

		private string _xmlType = null;
		
		/// <summary>
		/// Название элемента в xml
		/// </summary>
		[XmlAttribute]
        public string xmlType
        {
            get { return _xmlType; }
            set { 
					SetProperty(ref _xmlType, value); 
					OnPropertyChanged("xmlType");
				}
        }

		private string _primaryKeyType = "int";
		
		/// <summary>
		/// Тип первичного ключа
		/// </summary>
		[XmlAttribute]
        public string PrimaryKeyType
        {
            get { return _primaryKeyType; }
            set { 
					SetProperty(ref _primaryKeyType, value); 
					OnPropertyChanged("PrimaryKeyType");
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

		private Guid _databaseId = Guid.Empty;
		
		/// <summary>
		/// Ссылка на БД
		/// </summary>
		[XmlAttribute]
        public Guid DatabaseId
        {
            get { return _databaseId; }
            set { 
					SetProperty(ref _databaseId, value); 
					OnPropertyChanged("DatabaseId");
				}
        }
		Database _databaseIdObject ;
		/// <summary>
		/// ссылка на внешний объект(только для случая когда 1 первичный ключ)
		/// </summary>
		[XmlIgnore]
        public Database Database
        {
		
            get 
			{ 
				if (_databaseIdObject == null)
					_databaseIdObject = ((CDatabaseEntity)_table.parentDataBase).Database.GetById(_databaseId);
				return _databaseIdObject ; 
			}
            set 
			{ 
				if (value == null)
				{
					this.DatabaseId = Guid.Empty ;
					this._databaseIdObject = null ;
				}
				else
				{
					this.DatabaseId = value.Id ;
					this._databaseIdObject = value ;
				}
			}
        }

		private string _cashType = "MemCash";
		
		/// <summary>
		/// Тип кеширования
		/// </summary>
		[XmlAttribute]
        public string CashType
        {
            get { return _cashType; }
            set { 
					SetProperty(ref _cashType, value); 
					OnPropertyChanged("CashType");
				}
        }

		private string _tableName = null;
		
		/// <summary>
		/// Имя таблицы в БД
		/// </summary>
		[XmlAttribute]
        public string TableName
        {
            get { return _tableName; }
            set { 
					SetProperty(ref _tableName, value); 
					OnPropertyChanged("TableName");
				}
        }

		private bool _isInterface = false;
		
		/// <summary>
		/// Интерфейс
		/// </summary>
		[XmlAttribute]
        public bool IsInterface
        {
            get { return _isInterface; }
            set { 
					SetProperty(ref _isInterface, value); 
					OnPropertyChanged("IsInterface");
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

		private bool _isB1Object = true;
		
		/// <summary>
		/// B1Object
		/// </summary>
		[XmlAttribute]
        public bool IsB1Object
        {
            get { return _isB1Object; }
            set { 
					SetProperty(ref _isB1Object, value); 
					OnPropertyChanged("IsB1Object");
				}
        }

		private bool _isDict = true;
		
		/// <summary>
		/// Dict
		/// </summary>
		[XmlAttribute]
        public bool IsDict
        {
            get { return _isDict; }
            set { 
					SetProperty(ref _isDict, value); 
					OnPropertyChanged("IsDict");
				}
        }

		private bool _isComment = true;
		
		/// <summary>
		/// Comment
		/// </summary>
		[XmlAttribute]
        public bool IsComment
        {
            get { return _isComment; }
            set { 
					SetProperty(ref _isComment, value); 
					OnPropertyChanged("IsComment");
				}
        }

		private bool _isChangesLog = false;
		
		/// <summary>
		/// Логирование
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
	
        private SortList< Field > _fieldlist = null;
        
        [XmlArray]
        public SortList< Field > FieldList
        {
            get
            {
                if (_fieldlist != null) return _fieldlist;
				if (_table != null)
				{
					var thisid = this.Id ;
					_fieldlist = ((CDatabaseEntity)_table.parentDataBase). Field .GetObjectListByExpr(obj => obj.TableId == thisid);
				}
				else
					_fieldlist = new SortList< Field >(_table);
				return _fieldlist;
            }
        }

	
        private SortList< Relation > _relationlist = null;
        
        [XmlArray]
        public SortList< Relation > RelationList
        {
            get
            {
                if (_relationlist != null) return _relationlist;
				if (_table != null)
				{
					var thisid = this.Id ;
					_relationlist = ((CDatabaseEntity)_table.parentDataBase). Relation .GetObjectListByExpr(obj => obj.TableId == thisid);
				}
				else
					_relationlist = new SortList< Relation >(_table);
				return _relationlist;
            }
        }


        public override object DeepClone(int deep = 99)
        {
            var t = (Table)Clone();
			if (deep > 0)
             t._fieldlist = FieldList.CloneAll(deep-1);             
             t._relationlist = RelationList.CloneAll(deep-1);             
          return t;
        }


		[XmlIgnore]
        public override BObject.ObjectStateType ObjectState
        {
            get
            {
                if (_objectState != BObject.ObjectStateType.Unchanged)
                    return _objectState;
				BObject.ObjectStateType list_state = BObject.ObjectStateType.Unchanged;	
              
				if (_fieldlist != null)
				{  
					list_state = this.FieldList.ObjectListState();
					if (list_state != BObject.ObjectStateType.Unchanged)
						return list_state;
				}
              
				if (_relationlist != null)
				{  
					list_state = this.RelationList.ObjectListState();
					if (list_state != BObject.ObjectStateType.Unchanged)
						return list_state;
				}
					
				return list_state;
            }
		}

			
        public override void Save2()
			
        {
            ObjectStateType state = this.ObjectState;
            if (state == ObjectStateType.Unchanged)
            {
                return;
            }
			
                
            if (state == ObjectStateType.Deleted)
            {
                // удаление каскадное !
                base.Save2();
                return;
            } 
			
			
            base.Save2();
			
	
            foreach (Field obj in this.FieldList)
                obj.TableId = this.Id;
            this.FieldList.Save();
	
            foreach (Relation obj in this.RelationList)
                obj.TableId = this.Id;
            this.RelationList.Save();
			
			

            return;
        }
			

        public override void Rollback()
        {
            base.Rollback();
			
	
            this.FieldList.Rollback();
	
            this.RelationList.Rollback();
        }
	
    }

}
	
