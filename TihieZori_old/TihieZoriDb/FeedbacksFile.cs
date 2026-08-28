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
  public partial class CTableFeedbacks: CTable< Feedbacks, CDatabaseTihieZori,int>
  {
  	CTableFeedbacks()
	{}
	        public CTableFeedbacks(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Feedbacks), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(Feedbacks), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Dat1"] = new FieldDescription(typeof(Feedbacks), "Dat1", "DateTime", "smalldatetime", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Title"] = new FieldDescription(typeof(Feedbacks), "Title", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Message"] = new FieldDescription(typeof(Feedbacks), "Message", "string", "nvarchar", 1024, 0, FieldDescription.fieldProp.Empty);
			_fd["SenderId"] = new FieldDescription(typeof(Feedbacks), "SenderId", "int", "int", 0, 0, FieldDescription.fieldProp.ForeignKey );
			_fd["email"] = new FieldDescription(typeof(Feedbacks), "email", "string", "nvarchar", 100, 0, FieldDescription.fieldProp.Empty);
			_fd["ip"] = new FieldDescription(typeof(Feedbacks), "ip", "string", "nvarchar", 20, 0, FieldDescription.fieldProp.Empty);
			_fd["fpath"] = new FieldDescription(typeof(Feedbacks), "fpath", "string", "nvarchar", 100, 0, FieldDescription.fieldProp.Nullable );
			_fd["Active"] = new FieldDescription(typeof(Feedbacks), "Active", "bool", "int", 0, 0, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Письма с сайта, комментарии
	/// </summary>
	public partial class Feedbacks : BObject,IPrimaryKey<int> 
    {
        public Feedbacks() { }
        public Feedbacks(CTableFeedbacks t)
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

		private DateTime _dat1 = DateTime.Now;
		
		/// <summary>
		/// Дата, время сообщения
		/// </summary>
		[XmlAttribute]
        public DateTime Dat1
        {
            get { return _dat1; }
            set { 
					SetProperty(ref _dat1, value); 
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

		private string _message = "";
		
		/// <summary>
		/// Текст сообщения
		/// </summary>
		[XmlAttribute]
        public string Message
        {
            get { return _message; }
            set { 
					SetProperty(ref _message, value); 
					OnPropertyChanged();
				}
        }

		private int _senderId = 0;
		
		/// <summary>
		/// Кто редактировал новость
		/// </summary>
		[XmlAttribute]
        public int SenderId
        {
            get { return _senderId; }
            set { 
					SetProperty(ref _senderId, value); 
					OnPropertyChanged();
				}
        }
		/// <summary>
		/// ссылка на внешний объект(только для случая когда 1 первичный ключ)
		/// </summary>
		[XmlIgnore]
        public User Sender
        {
		
            get { return ((CDatabaseTihieZori)_table.parentDataBase).User.GetById(_senderId); }
            set { this.SenderId = value == null ? 0 : value.Id ; }
        }

		private string _email = "";
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public string email
        {
            get { return _email; }
            set { 
					SetProperty(ref _email, value); 
					OnPropertyChanged();
				}
        }

		private string _ip = "";
		
		/// <summary>
		/// ip адрес отправляющего
		/// </summary>
		[XmlAttribute]
        public string ip
        {
            get { return _ip; }
            set { 
					SetProperty(ref _ip, value); 
					OnPropertyChanged();
				}
        }

		private string _fpath = null;
		
		/// <summary>
		/// Страница к которой прикреплен комментарий
		/// </summary>
		[XmlAttribute]
        public string fpath
        {
            get { return _fpath; }
            set { 
					SetProperty(ref _fpath, value); 
					OnPropertyChanged();
				}
        }

		private bool _active = false;
		
		/// <summary>
		/// Флаг активности
		/// </summary>
		[XmlAttribute]
        public bool Active
        {
            get { return _active; }
            set { 
					SetProperty(ref _active, value); 
					OnPropertyChanged();
				}
        }
    }

}
	
