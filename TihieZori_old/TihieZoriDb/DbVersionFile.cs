// This is the output code from your template
// you only get syntax-highlighting here - not intellisense

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbCommon;
using System.Drawing;
using System.Xml.Serialization;


namespace TihieZoriDb{
  public partial class CTableDbVersion: CTable< DbVersion, CDatabaseTihieZori,int>
  {
  	CTableDbVersion()
	{}
	        public CTableDbVersion(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(DbVersion), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["VersionNum"] = new FieldDescription(typeof(DbVersion), "VersionNum", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(DbVersion), "Comment", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Карта колонок
	/// </summary>
	public partial class DbVersion : BObject,IPrimaryKey<int> 
    {
        public DbVersion() { }
        public DbVersion(CTableDbVersion t)
            : base(t)
        { }

		private int _id = 0;
		
		/// <summary>
		/// Первичный ключ, автонумератор
		/// </summary>
		[XmlAttribute]
        public int Id
        {
            get { return _id; }
            set { 
					SetProperty(ref _id, value); 
					OnPropertyChanged();
				}
        }

		private int _versionNum = 1;
		
		/// <summary>
		/// Номер версии
		/// </summary>
		[XmlAttribute]
        public int VersionNum
        {
            get { return _versionNum; }
            set { 
					SetProperty(ref _versionNum, value); 
					OnPropertyChanged();
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
					OnPropertyChanged();
				}
        }
    }

}
	
