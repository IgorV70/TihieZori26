using DbCommon;
using DbCommon.Helpers;
using TihieZoriDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TihieZori.Settings
{
    public class SqlSettings : ViewModelBase
    {
        private static string _settingsFilePath;

        private DatabaseType _databaseType = DatabaseType.MsSql;
        private MsSqlConnectionSettings _sqlConnectionSettings = new MsSqlConnectionSettings();
        //public List<FolderSettings> Folders { get; set; }

        public static SqlSettings ReadFromFile(string root,bool createFileIfNotExists = false)
        {
            _settingsFilePath = Path.Combine(root, _settingsFilePath ?? SpecialFiles.SettingsFilePath);

            SqlSettings settings = null;
            try
            {
                using (var reader = new StreamReader(_settingsFilePath))
                {
                    var x = new XmlSerializer(typeof(SqlSettings));
                    settings = (SqlSettings)x.Deserialize(reader);
                }
                return settings;
            }
            catch (Exception)
            {
                settings = new SqlSettings();
                settings.Save(_settingsFilePath);
                return settings;
            }
        }

        public void Save()
        {
            Save(_settingsFilePath);
        }

        public void Save(string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    var s = new XmlSerializer(typeof(SqlSettings));
                    s.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }

        }


        public SqlSettings Clone()
        {
            SqlSettings ret = (SqlSettings)this.MemberwiseClone();
            ret._sqlConnectionSettings = (MsSqlConnectionSettings)_sqlConnectionSettings.Clone();
            return ret;
        }

        public bool Eq(object obj)
        {
            var cp = obj as SqlSettings;
            return cp != null
                   && (_databaseType == cp._databaseType
                               && _sqlConnectionSettings.Eq(cp._sqlConnectionSettings));
        }


        public DbCommon.DatabaseType DatabaseType
        {
            get { return _databaseType; }
            set
            {
                if (SetProperty(ref _databaseType, value))
                {
                    switch (value)
                    {
                        case DatabaseType.MsSql:
                            SqlConnectionSettings = new MsSqlConnectionSettings();
                            break;
                        case DatabaseType.PostgreeSql:
                            //SqlConnectionSettings = new PostgreeSqlConnectionSettings();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("value", value, null);
                    }
                }
            }
        }

        public MsSqlConnectionSettings SqlConnectionSettings
        {
            get { return _sqlConnectionSettings; }
            set { SetProperty(ref _sqlConnectionSettings, value); }
        }

        public CDatabaseTihieZori GetDatabase()
        {
            CDatabaseTihieZori ret = new CDatabaseTihieZori(DatabaseType, SqlConnectionSettings.GetConnectionString());
            return ret;
        }
    }
}