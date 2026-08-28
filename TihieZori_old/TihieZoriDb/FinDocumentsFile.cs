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
  public partial class CTableFinDocuments: CTable< FinDocuments, CDatabaseTihieZori,int>
  {
  	CTableFinDocuments()
	{}
	        public CTableFinDocuments(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(FinDocuments), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["OrderId"] = new FieldDescription(typeof(FinDocuments), "OrderId", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Name"] = new FieldDescription(typeof(FinDocuments), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Title"] = new FieldDescription(typeof(FinDocuments), "Title", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(FinDocuments), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Active"] = new FieldDescription(typeof(FinDocuments), "Active", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Финансовые документы
	/// </summary>
	public partial class FinDocuments : BObject,IPrimaryKey<int> 
    {
        public FinDocuments() { }
        public FinDocuments(CTableFinDocuments t)
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
	
