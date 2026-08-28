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
  public partial class CTableAdvert: CTable< Advert, CDatabaseTihieZori,int>
  {
  	CTableAdvert()
	{}
	        public CTableAdvert(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Advert), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["DatM"] = new FieldDescription(typeof(Advert), "DatM", "DateTime", "smalldatetime", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Title"] = new FieldDescription(typeof(Advert), "Title", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
			_fd["Comment"] = new FieldDescription(typeof(Advert), "Comment", "string", "text", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["Active"] = new FieldDescription(typeof(Advert), "Active", "int", "int", 0, 0, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// Объявления
	/// </summary>
	public partial class Advert : BObject,IPrimaryKey<int> 
    {
        public Advert() { }
        public Advert(CTableAdvert t)
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
	
