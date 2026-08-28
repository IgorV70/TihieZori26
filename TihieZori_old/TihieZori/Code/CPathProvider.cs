using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;
using TihieZoriDb;

namespace TihieZori.Code
{
    public class CPathProvider : VirtualPathProvider
    {
        public static CDatabaseTihieZori db;
        /// <summary>
        /// special method - this is the starting point
        /// </summary>
        /// 
        /// 
        public static void AppInitialize(CDatabaseTihieZori dbGps)
        {
            db = dbGps;
            CPathProvider sampleProvider = new CPathProvider();
            HostingEnvironment.RegisterVirtualPathProvider(sampleProvider);
        }

        /// <summary>
        /// ASP.Net queries this method to check if the file requested exists or not
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override bool FileExists(string virtualPath)
        {
            string virtualFileName = Path.GetFileName(virtualPath);
            //string virtualDirectory = Path.GetDirectoryName(virtualPath);
            if (db != null)
            {
                try
                {
                    if (db.SitePage.GetObjectByExpr(sp => sp.Name == virtualFileName && sp.Active > 0) != null)
                        return true;
                }
                catch
                {
                    // ignored
                }
            }
            return Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// ASP.Net requests the virtual file. So we need to give a VirtualFile instance back
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            string virtualFileName = Path.GetFileName(virtualPath);
            //string virtualDirectory = Path.GetDirectoryName(virtualPath);
            if (db != null)
            {
                try
                {
                    if (db.SitePage.GetObjectByExpr(sp => sp.Name == virtualFileName && sp.Active > 0) != null)
                        return new TextFile(virtualPath, this);
                }
                catch
                {
                    // ignored
                }
            }
            return Previous.GetFile(virtualPath);

        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

    }

}