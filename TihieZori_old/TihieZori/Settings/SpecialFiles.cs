using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TihieZori.Settings
{
    public static class SpecialFiles
    {
        private const string DataSettingsFilePath_ = @"App_Data\TihieZoriSettings.xml";

        //static SpecialFiles()
        //{
        //    if (!Directory.Exists(SettingsFileDirectory))
        //    {
        //        Directory.CreateDirectory(SettingsFileDirectory);
        //    }
        //}

        public static readonly string SettingsFileDirectory
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SqlSettings");
        public static readonly string SettingsFilePath = DataSettingsFilePath_;

        public static string BaseDirectory { get; private set; }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
        {
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(BaseDirectory ?? string.Empty, path);
        }
        
    }
}