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
  public partial class CTableDocuments: CTable< Documents, CDatabaseTihieZori,int>
  {
  	CTableDocuments()
	{}
	        public CTableDocuments(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Documents), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["OrderId"] = new FieldDescription(typeof(Documents), "OrderId", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Name"] = new FieldDescription(typeof(Documents), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Title"] = new FieldDescription(typeof(Documents), "Title", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Documents), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Active"] = new FieldDescription(typeof(Documents), "Active", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Страницы сайта
	/// </summary>
	public partial class Documents : BObject,IPrimaryKey<int> 
    {
        public Documents() { }
        public Documents(CTableDocuments t)
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

		private int _orderId = 1000;
		
		/// <summary>
		/// Порядок сортировки
		/// </summary>
		[XmlAttribute]
        public int OrderId
        {
            get { return _orderId; }
            set { 
					SetProperty(ref _orderId, value); 
					OnPropertyChanged();
				}
        }

		private string _name = "";
		
		/// <summary>
		/// Наименование
		/// </summary>
		[XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { 
					SetProperty(ref _name, value); 
					OnPropertyChanged();
				}
        }

		private string _title = "";
		
		/// <summary>
		/// Заголовок
		/// </summary>
		[XmlAttribute]
        public string Title
        {
            get { return _title; }
            set { 
					SetProperty(ref _title, value); 
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

		private int _active = 0;
		
		/// <summary>
		/// Флаг активности
		/// </summary>
		[XmlAttribute]
        public int Active
        {
            get { return _active; }
            set { 
					SetProperty(ref _active, value); 
					OnPropertyChanged();
				}
        }
    }

}
	
