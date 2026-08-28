using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TihieZori.Settings
{
    //[XmlInclude(typeof(PostgreeSqlConnectionSettings))]
    [XmlInclude(typeof(MsSqlConnectionSettings))]
    public abstract class SqlConnectionSettings : ViewModelBase
    {
        public virtual bool ServerLocal { get; set; }
        public virtual string ServerName { get; set; }
        public virtual string BdName { get; set; }
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }

        public abstract bool Eq(object obj);

        public abstract string GetConnectionString();
        public abstract string GetMasterConnectionString(string login, string password);

        public SqlConnectionSettings Clone()
        {
            SqlConnectionSettings ret = (SqlConnectionSettings)MemberwiseClone();
            return ret;
        }
    }
}