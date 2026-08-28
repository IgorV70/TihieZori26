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
  public partial class CTablePropose: CTable< Propose, CDatabaseTihieZori,int>
  {
  	CTablePropose()
	{}
	        public CTablePropose(CDatabaseTihieZori db)
            : base(db)
        { 
			IsChangesLog = false;
			_fd["Id"] = new FieldDescription(typeof(Propose), "Id", "int", "int", 0, 0, FieldDescription.fieldProp.Identity | FieldDescription.fieldProp.PrimaryKey );
			_fd["Name"] = new FieldDescription(typeof(Propose), "Name", "string", "nvarchar", 256, 0, FieldDescription.fieldProp.Empty);
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
	[XmlType("назначение")]
	public partial class Propose : BObject,IPrimaryKey<int> 
    {
        public Propose() { }
        public Propose(CTablePropose t)
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

		private string _name = "";
		
		/// <summary>
		/// 
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
    }

}
	
