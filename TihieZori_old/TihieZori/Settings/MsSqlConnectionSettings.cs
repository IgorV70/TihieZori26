using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TihieZori.Settings
{
    public class MsSqlConnectionSettings : SqlConnectionSettings
    {
        private bool _trusted = true;
        private bool _serverLocal = true;
        private string _serverName = "(local)";
        private string _bdName = "zori";
        private string _login = "sa";
        private string _password = "sa1234567";
        public bool Trusted
        {
            get { return _trusted; }
            set
            {
                if (SetProperty(ref _trusted, value))
                {
                    if (value)
                    {
                        Login = "";
                        Password = "";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(_login))
                            Login = "sa";
                    }
                }
            }
        }

        public override bool ServerLocal
        {
            get { return _serverLocal; }
            set
            {
                if (SetProperty(ref _serverLocal, value))
                    ServerName = "(local)";
            }
        }

        public override string ServerName
        {
            get { return _serverName; }
            set { SetProperty(ref _serverName, value); }
        }

        public override string BdName
        {
            get { return _bdName; }
            set { SetProperty(ref _bdName, value); }
        }

        public override string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        public override string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public override bool Eq(object obj)
        {
            var cs = obj as MsSqlConnectionSettings;
            if (cs == null) return false;
            if (_trusted != cs._trusted) return false;
            if (_serverLocal != cs._serverLocal) return false;
            if (!_serverLocal)
            {
                if (_serverName != cs._serverName) return false;
            }
            if (_bdName != cs._bdName) return false;
            if (!_trusted)
            {
                if (_login != cs._login) return false;
                if (_password != cs._password) return false;
            }
            return true;
        }

        public override string GetConnectionString()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                ConnectTimeout = 0,
                InitialCatalog = BdName
            };
            if (Trusted)
                builder.IntegratedSecurity = Trusted;
            else
            {
                builder.Password = Password;
                builder.UserID = Login;
            }
            return builder.ConnectionString;
        }

        public override string GetMasterConnectionString(string login, string password)
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                ConnectTimeout = 0,
                InitialCatalog = "master"
            };
            if (string.IsNullOrEmpty(login))
                builder.IntegratedSecurity = Trusted;
            else
            {
                builder.Password = password;
                builder.UserID = login;
            }
            return builder.ConnectionString;
        }
    }
}