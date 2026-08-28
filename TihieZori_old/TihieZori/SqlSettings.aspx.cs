using DbCommon.Helpers;
using TihieZori.Code;
using TihieZori.Settings;
using TihieZoriDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TihieZori
{
    public partial class SqlSettingsPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["submit"] != null)
            {
                var settings = SqlSettings.ReadFromFile(Request.PhysicalApplicationPath);
                settings.SqlConnectionSettings.BdName = Request.Params["bdname"] ?? "";
                settings.SqlConnectionSettings.Login = Request.Params["login"] ?? "";
                settings.SqlConnectionSettings.Password = Request.Params["password"] ?? "";
                settings.SqlConnectionSettings.Trusted = false;
                settings.SqlConnectionSettings.ServerLocal = true;
                var db = settings.GetDatabase();
                if (db.TestConnection())
                {
                    settings.Save();
                    db.CreateShema();
                    //string cookie = CLogon.SetCookie(Response, Request.Cookies.Get("usercoo"));
                    //try
                    //{
                    //    Session s = db.Session.GetObjectByExpr(sss => sss.Cookie == cookie)?? db.Session.NewInstance();
                    //    if (s.User != null)
                    //    {
                    //        Session["user"] = s.User;
                    //    }
                    //    s.LastIn = DateTime.Now;
                    //    s.SessionId = Session.SessionID;
                    //    s.IP = Request.UserHostAddress;
                    //    s.Save2();
                    //    Session["session"] = s;
                    //    Session["database"] = db;
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.Error(ex, "SqlSettingsPage:");
                    //    throw;
                    //}
                }
            }
        }
    }
}