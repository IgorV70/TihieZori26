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
  public partial class CTableDiscuss: CTable< Discuss, CDatabaseTihieZori,int>
  {
  	CTableDiscuss()
	{}
	        public CTableDiscuss(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Discuss), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["DatM"] = new FieldDescription(typeof(Discuss), "DatM", "DateTime", "smalldatetime", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Author"] = new FieldDescription(typeof(Discuss), "Author", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Email"] = new FieldDescription(typeof(Discuss), "Email", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Ip"] = new FieldDescription(typeof(Discuss), "Ip", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Title"] = new FieldDescription(typeof(Discuss), "Title", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Discuss), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Active"] = new FieldDescription(typeof(Discuss), "Active", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Обсуждения
	/// </summary>
	public partial class Discuss : BObject,IPrimaryKey<int> 
    {
        public Discuss() { }
        public Discuss(CTableDiscuss t)
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

		private DateTime _datM = DateTime.Now;
		
		/// <summary>
		/// Дата модификации
		/// </summary>
		[XmlAttribute]
        public DateTime DatM
        {
            get { return _datM; }
            set { 
					SetProperty(ref _datM, value); 
					OnPropertyChanged();
				}
        }

		private string _author = "";
		
		/// <summary>
		/// Автор
		/// </summary>
		[XmlAttribute]
        public string Author
        {
            get { return _author; }
            set { 
					SetProperty(ref _author, value); 
					OnPropertyChanged();
				}
        }

		private string _email = "";
		
		/// <summary>
		/// Автор
		/// </summary>
		[XmlAttribute]
        public string Email
        {
            get { return _email; }
            set { 
					SetProperty(ref _email, value); 
					OnPropertyChanged();
				}
        }

		private string _ip = "";
		
		/// <summary>
		/// Ip
		/// </summary>
		[XmlAttribute]
        public string Ip
        {
            get { return _ip; }
            set { 
					SetProperty(ref _ip, value); 
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
	
