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
  public partial class CTableUser: CTable< User, CDatabaseTihieZori,int>
  {
  	CTableUser()
	{}
	        public CTableUser(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(User), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Login"] = new FieldDescription(typeof(User), "Login", "string", "nvarchar", 20, 0, FieldDescription.fieldProp.Nullable );
			_fd["Email"] = new FieldDescription(typeof(User), "Email", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["Fio"] = new FieldDescription(typeof(User), "Fio", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["FioDover"] = new FieldDescription(typeof(User), "FioDover", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["Phone"] = new FieldDescription(typeof(User), "Phone", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["UserRole"] = new FieldDescription(typeof(User), "UserRole", "RoleEnum", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["LandNumber"] = new FieldDescription(typeof(User), "LandNumber", "string", "varchar", 20, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(User), "Comment", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Password"] = new FieldDescription(typeof(User), "Password", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["salt"] = new FieldDescription(typeof(User), "salt", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Nullable );
			_fd["Cookie"] = new FieldDescription(typeof(User), "Cookie", "string", "nvarchar", 200, 0, FieldDescription.fieldProp.Empty);
			_fd["LastSessionId"] = new FieldDescription(typeof(User), "LastSessionId", "int", "int", 0, 0, FieldDescription.fieldProp.Nullable  | FieldDescription.fieldProp.ForeignKey );
			CustomsInit();
        }		
	        public override string TableName()
        {
            return TablePrefix()+"tUser";
        }


				
		public override CasheType GetCashType()
        {
            return CasheType.FullMemCash;
        }
				
	}
	

	/// <summary>
	/// Учетные записи
	/// </summary>
	public partial class User : BObject,IPrimaryKey<int> 
    {
        public User() { }
        public User(CTableUser t)
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

		private string _login = null;
		
		/// <summary>
		/// Логин
		/// </summary>
		[XmlAttribute]
        public string Login
        {
            get { return _login; }
            set { 
					SetProperty(ref _login, value); 
					OnPropertyChanged();
				}
        }

		private string _email = null;
		
		/// <summary>
		/// емайл
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

		private string _fio = null;
		
		/// <summary>
		/// ФИО
		/// </summary>
		[XmlAttribute]
        public string Fio
        {
            get { return _fio; }
            set { 
					SetProperty(ref _fio, value); 
					OnPropertyChanged();
				}
        }

		private string _fioDover = null;
		
		/// <summary>
		/// Довереннон лицо
		/// </summary>
		[XmlAttribute]
        public string FioDover
        {
            get { return _fioDover; }
            set { 
					SetProperty(ref _fioDover, value); 
					OnPropertyChanged();
				}
        }

		private string _phone = null;
		
		/// <summary>
		/// Номер телефона
		/// </summary>
		[XmlAttribute]
        public string Phone
        {
            get { return _phone; }
            set { 
					SetProperty(ref _phone, value); 
					OnPropertyChanged();
				}
        }

		private RoleEnum _userRole = 0;
		
		/// <summary>
		/// Роль в системе
		/// </summary>
		[XmlAttribute]
        public RoleEnum UserRole
        {
            get { return _userRole; }
            set { 
					SetProperty(ref _userRole, value); 
					OnPropertyChanged();
				}
        }

		private string _landNumber = "";
		
		/// <summary>
		/// Номер участка
		/// </summary>
		[XmlAttribute]
        public string LandNumber
        {
            get { return _landNumber; }
            set { 
					SetProperty(ref _landNumber, value); 
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

		private string _password = null;
		
		/// <summary>
		/// Пароль
		/// </summary>
		[XmlAttribute]
        public string Password
        {
            get { return _password; }
            set { 
					SetProperty(ref _password, value); 
					OnPropertyChanged();
				}
        }

		private string _salt = null;
		
		/// <summary>
		/// секретное поле для шифровки пароля
		/// </summary>
		[XmlAttribute]
        public string salt
        {
            get { return _salt; }
            set { 
					SetProperty(ref _salt, value); 
					OnPropertyChanged();
				}
        }

		private string _cookie = "";
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public string Cookie
        {
            get { return _cookie; }
            set { 
					SetProperty(ref _cookie, value); 
					OnPropertyChanged();
				}
        }

		private int _lastSessionId = 0;
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public int LastSessionId
        {
            get { return _lastSessionId; }
            set { 
					SetProperty(ref _lastSessionId, value); 
					OnPropertyChanged();
				}
        }
		/// <summary>
		/// ссылка на внешний объект(только для случая когда 1 первичный ключ)
		/// </summary>
		[XmlIgnore]
        public Session LastSession
        {
		
            get { return ((CDatabaseTihieZori)_table.parentDataBase).Session.GetById(_lastSessionId); }
            set { this.LastSessionId = value == null ? 0 : value.Id ; }
        }
    }

}
	
