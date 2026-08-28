

using System.Data.Common;
using DbCommon;


// C:\MyProjects\Weigher\Common\EntityDatabase\T4\Entity.xml
namespace EntityData
{

    public partial class CDatabaseEntity : CDatabase 
	{
	    public CDatabaseEntity(): base(DatabaseType.MsSql)
        {
            Init();
        }

        public CDatabaseEntity(DatabaseType databaseType)
            : base(databaseType)
        {
            Init();
        }

        public CDatabaseEntity(DatabaseType databaseType,string connectionString)
            : base(databaseType)
        {
            _sConnectionString = connectionString;
            Init();
			ConnectionMode = DbCommon.Enums.ConnectionModes.DatabaseConnection;
        }

        public string CDatabaseName = "Entity";
      	public CTableDatabase Database;
      	public CTableTable Table;
      	public CTableField Field;
      	public CTableRelation Relation;
        public void Init()
        {
  			Database = new CTableDatabase(this);
  			Table = new CTableTable(this);
  			Field = new CTableField(this);
  			Relation = new CTableRelation(this);
	
		  _tables.Add("Database",Database);	
		  _tables.Add("Table",Table);	
		  _tables.Add("Field",Field);	
		  _tables.Add("Relation",Relation);	
		}
        
        

	}
}
