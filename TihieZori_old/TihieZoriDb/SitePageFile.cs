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
  public partial class CTableSitePage: CTable< SitePage, CDatabaseTihieZori,int>
  {
  	CTableSitePage()
	{}
	        public CTableSitePage(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(SitePage), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(SitePage), "Name", "string", "nvarchar", 128, 0, FieldDescription.fieldProp.Empty);
			_fd["Title"] = new FieldDescription(typeof(SitePage), "Title", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["MasterPage"] = new FieldDescription(typeof(SitePage), "MasterPage", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Nullable );
			_fd["Flags"] = new FieldDescription(typeof(SitePage), "Flags", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(SitePage), "Comment", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Nullable );
			_fd["PageText"] = new FieldDescription(typeof(SitePage), "PageText", "string", "ntext", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Keywords"] = new FieldDescription(typeof(SitePage), "Keywords", "string", "nvarchar", 512, 0, FieldDescription.fieldProp.Empty);
			_fd["Vers"] = new FieldDescription(typeof(SitePage), "Vers", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["DatM"] = new FieldDescription(typeof(SitePage), "DatM", "DateTime", "smalldatetime", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Active"] = new FieldDescription(typeof(SitePage), "Active", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Uid"] = new FieldDescription(typeof(SitePage), "Uid", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
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
	public partial class SitePage : BObject,IPrimaryKey<int> 
    {
        public SitePage() { }
        public SitePage(CTableSitePage t)
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

		private string _title = "";
		
		/// <summary>
		/// 
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

		private string _masterPage = null;
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public string MasterPage
        {
            get { return _masterPage; }
            set { 
					SetProperty(ref _masterPage, value); 
					OnPropertyChanged();
				}
        }

		private int _flags = 0;
		
		/// <summary>
		/// 1-Прикрепить вопросы/ответы
		/// </summary>
		[XmlAttribute]
        public int Flags
        {
            get { return _flags; }
            set { 
					SetProperty(ref _flags, value); 
					OnPropertyChanged();
				}
        }

		private string _comment = null;
		
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

		private string _pageText = "";
		
		/// <summary>
		/// текст страницы
		/// </summary>
		[XmlAttribute]
        public string PageText
        {
            get { return _pageText; }
            set { 
					SetProperty(ref _pageText, value); 
					OnPropertyChanged();
				}
        }

		private string _keywords = "";
		
		/// <summary>
		/// Ключевые слова
		/// </summary>
		[XmlAttribute]
        public string Keywords
        {
            get { return _keywords; }
            set { 
					SetProperty(ref _keywords, value); 
					OnPropertyChanged();
				}
        }

		private int _vers = 0;
		
		/// <summary>
		/// Версия текста
		/// </summary>
		[XmlAttribute]
        public int Vers
        {
            get { return _vers; }
            set { 
					SetProperty(ref _vers, value); 
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

		private int _uid = 0;
		
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		[XmlAttribute]
        public int Uid
        {
            get { return _uid; }
            set { 
					SetProperty(ref _uid, value); 
					OnPropertyChanged();
				}
        }
    }

}
	
