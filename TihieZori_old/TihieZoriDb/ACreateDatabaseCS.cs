

using System;
using System.Data.Common;
using DbCommon;


// C:\MyProjects\TihieZori\TihieZoriDb\TihieZori.xml
namespace TihieZoriDb
{

    public partial class CDatabaseTihieZori : CDatabase 
	{
	    public CDatabaseTihieZori(): base(DatabaseType.MsSql)
        {
            Init();
        }

        public CDatabaseTihieZori(DatabaseType databaseType)
            : base(databaseType)
        {
            Init();
        }

        public CDatabaseTihieZori(DatabaseType databaseType,string connectionString)
            : base(databaseType)
        {
            _sConnectionString = connectionString;
            Init();
			ConnectionMode = DbCommon.Enums.ConnectionModes.DatabaseConnection;
        }

        public string CDatabaseName = "TihieZori";
      	public CTablePropose Propose;
      	public CTableAccrual Accrual;
      	public CTablePayment Payment;
      	public CTableRoles Roles;
      	public CTableUser User;
      	public CTableSession Session;
      	public CTableDbVersion DbVersion;
      	public CTableSitePage SitePage;
      	public CTableDocuments Documents;
      	public CTableFinDocuments FinDocuments;
      	public CTableAdvert Advert;
      	public CTableDiscuss Discuss;
      	public CTableFeedbacks Feedbacks;
        public void Init()
        {
  			Propose = new CTablePropose(this);
  			Accrual = new CTableAccrual(this);
  			Payment = new CTablePayment(this);
  			Roles = new CTableRoles(this);
  			User = new CTableUser(this);
  			Session = new CTableSession(this);
  			DbVersion = new CTableDbVersion(this);
  			SitePage = new CTableSitePage(this);
  			Documents = new CTableDocuments(this);
  			FinDocuments = new CTableFinDocuments(this);
  			Advert = new CTableAdvert(this);
  			Discuss = new CTableDiscuss(this);
  			Feedbacks = new CTableFeedbacks(this);
	
		  _tables.Add("Propose",Propose);	
		  _tables.Add("Accrual",Accrual);	
		  _tables.Add("Payment",Payment);	
		  _tables.Add("Roles",Roles);	
		  _tables.Add("User",User);	
		  _tables.Add("Session",Session);	
		  _tables.Add("DbVersion",DbVersion);	
		  _tables.Add("SitePage",SitePage);	
		  _tables.Add("Documents",Documents);	
		  _tables.Add("FinDocuments",FinDocuments);	
		  _tables.Add("Advert",Advert);	
		  _tables.Add("Discuss",Discuss);	
		  _tables.Add("Feedbacks",Feedbacks);	
		}

        public override void CopyFrom(CDatabase sourceDb, Action<int> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
