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
  public partial class CTablePayment: CTable< Payment, CDatabaseTihieZori,int>
  {
  	CTablePayment()
	{}
	        public CTablePayment(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Payment), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["PayDate"] = new FieldDescription(typeof(Payment), "PayDate", "DateTime", "smalldatetime", 0, 0, FieldDescription.fieldProp.Empty);
			_fd["ProposeId"] = new FieldDescription(typeof(Payment), "ProposeId", "int", "int", 0, 0, FieldDescription.fieldProp.ForeignKey );
			_fd["UserId"] = new FieldDescription(typeof(Payment), "UserId", "int", "int", 0, 0, FieldDescription.fieldProp.ForeignKey );
			_fd["PaySum"] = new FieldDescription(typeof(Payment), "PaySum", "decimal", "decimal", 18, 2, FieldDescription.fieldProp.Empty);
			CustomsInit();
        }		


				
		public override CasheType GetCashType()
        {
            return CasheType.MemCash;
        }
				
	}
	

	/// <summary>
	/// 
	/// </summary>
	[XmlType("оплаты")]
	public partial class Payment : BObject,IPrimaryKey<int> 
    {
        public Payment() { }
        public Payment(CTablePayment t)
            : base(t)
        { }

		private int _id = 0;
		
		/// <summary>
		/// 
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

		private DateTime _payDate = DateTime.Now;
		
		/// <summary>
		/// UserId
		/// </summary>
		[XmlAttribute]
        public DateTime PayDate
        {
            get { return _payDate; }
            set { 
					SetProperty(ref _payDate, value); 
					OnPropertyChanged();
				}
        }

		private int _proposeId = 0;
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public int ProposeId
        {
            get { return _proposeId; }
            set { 
					SetProperty(ref _proposeId, value); 
					OnPropertyChanged();
				}
        }
		/// <summary>
		/// ссылка на внешний объект(только для случая когда 1 первичный ключ)
		/// </summary>
		[XmlIgnore]
        public Propose Propose
        {
		
            get { return ((CDatabaseTihieZori)_table.parentDataBase).Propose.GetById(_proposeId); }
            set { this.ProposeId = value == null ? 0 : value.Id ; }
        }

		private int _userId = 0;
		
		/// <summary>
		/// 
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

		private decimal _paySum = 0;
		
		/// <summary>
		/// 
		/// </summary>
		[XmlAttribute]
        public decimal PaySum
        {
            get { return _paySum; }
            set { 
					SetProperty(ref _paySum, value); 
					OnPropertyChanged();
				}
        }
    }

}
	
