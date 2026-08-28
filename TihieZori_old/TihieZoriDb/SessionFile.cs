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
  public partial class CTableSession: CTable< Session, CDatabaseTihieZori,int>
  {
  	CTableSession()
	{}
	        public CTableSession(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Session), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["SessionId"] = new FieldDescription(typeof(Session), "SessionId", "string", "nvarchar", 200, 0, FieldDescription.fieldProp.Empty);
			_fd["SessionStart"] = new FieldDescription(typeof(Session), "SessionStart", "DateTime", "smalldatetime", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["SessionEnd"] = new FieldDescription(typeof(Session), "SessionEnd", "DateTime?", "smalldatetime", 0, 0, FieldDescription.fieldProp.Nullable );
			_fd["IP"] = new FieldDescription(typeof(Session), "IP", "string", "nvarchar", 50, 0, FieldDescription.fieldProp.Nullable );
			_fd["UserId"] = new FieldDescription(typeof(Session), "UserId", "int", "int", 0, 0, FieldDescription.fieldProp.Nullable  | FieldDescription.fieldProp.ForeignKey );
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// активные сессии
	/// </summary>
	public partial class Session : BObject,IPrimaryKey<int> 
    {
        public Session() { }
        public Session(CTableSession t)
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

		private string _sessionId = "";
		
		/// <summary>
		/// Идентификатор сесии
		/// </summary>
		[XmlAttribute]
        public string SessionId
        {
            get { return _sessionId; }
            set { 
					SetProperty(ref _sessionId, value); 
					OnPropertyChanged();
				}
        }

		private DateTime _sessionStart = DateTime.Now;
		
		/// <summary>
		/// Начало сесии
		/// </summary>
		[XmlAttribute]
        public DateTime SessionStart
        {
            get { return _sessionStart; }
            set { 
					SetProperty(ref _sessionStart, value); 
					OnPropertyChanged();
				}
        }

		private DateTime? _sessionEnd = null;
		
		/// <summary>
		/// Конец сессии
		/// </summary>
		[XmlAttribute]
        public DateTime? SessionEnd
        {
            get { return _sessionEnd; }
            set { 
					SetProperty(ref _sessionEnd, value); 
					OnPropertyChanged();
				}
        }

		private string _iP = null;
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public string IP
        {
            get { return _iP; }
            set { 
					SetProperty(ref _iP, value); 
					OnPropertyChanged();
				}
        }

		private int _userId = 0;
		
		/// <summary>
		/// Основной пользователь
		/// </summary>
		[XmlAttribute]
        public int UserId
        {
            get { return _userId; }
            set { 
					SetProperty(ref _userId, value); 
					OnPropertyChanged();
				}
        }
		/// <summary>
		/// ссылка на внешний объект(только для случая когда 1 первичный ключ)
		/// </summary>
		[XmlIgnore]
        public User User
        {
		
            get { return ((CDatabaseTihieZori)_table.parentDataBase).User.GetById(_userId); }
            set { this.UserId = value == null ? 0 : value.Id ; }
        }
    }

}
	
