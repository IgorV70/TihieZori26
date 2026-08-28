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
  public partial class CTableDatabase: CTable< Database, CDatabaseEntity,Guid>
  {
  	CTableDatabase()
	{}
	        public CTableDatabase(CDatabaseEntity db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Database), "Id", "Guid", "uniqueidentifier", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(Database), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Database), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Базы данных
	/// </summary>
	[XmlType("CDataBase")]
	public partial class Database : BObject,IPrimaryKey<Guid> 
    {
        public Database() { }
        public Database(CTableDatabase t)
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
	
        private SortList< Table > _tablelist = null;
        
        [XmlArray]
        public SortList< Table > TableList
        {
            get
            {
                if (_tablelist != null) return _tablelist;
				if (_table != null)
				{
					var thisid = this.Id ;
					_tablelist = ((CDatabaseEntity)_table.parentDataBase). Table .GetObjectListByExpr(obj => obj.DatabaseId == thisid);
				}
				else
					_tablelist = new SortList< Table >(_table);
				return _tablelist;
            }
        }


        public override object DeepClone(int deep = 99)
        {
            var t = (Database)Clone();
			if (deep > 0)
             t._tablelist = TableList.CloneAll(deep-1);             
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
              
				if (_tablelist != null)
				{  
					list_state = this.TableList.ObjectListState();
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
			
	
            foreach (Table obj in this.TableList)
                obj.DatabaseId = this.Id;
            this.TableList.Save();
			
			

            return;
        }
			

        public override void Rollback()
        {
            base.Rollback();
			
	
            this.TableList.Rollback();
        }
	
    }

}
	
