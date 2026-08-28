using TihieZori.Code;
using TihieZori.Settings;
using TihieZoriDb;
using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace TihieZori
{
    public class Global : HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            CDatabaseTihieZori db = SqlSettings.ReadFromFile(AppDomain.CurrentDomain.BaseDirectory, true).GetDatabase();
            Application["database"] = db;
            CPathProvider.AppInitialize(db);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //            string spath = Request.PhysicalApplicationPath;
            CDatabaseTihieZori db = SqlSettings.ReadFromFile(Request.PhysicalApplicationPath, true).GetDatabase();
            Session["database"] = db;

            //if (Request.UserHostAddress == "213.180.206.197") return;

            string cookie = CLogon.SetCookie(Response, Request.Cookies.Get("client"));

            try
            {
                User user = db.User.GetObjectByExpr(sss => sss.Cookie == cookie);
                string sessionId = Session.SessionID;
                Session session = db.Session.GetObjectByExpr(sss => sss.SessionId == sessionId) ?? db.Session.NewInstance();

                session.SessionId = Session.SessionID;
                session.IP = Request.UserHostAddress;
                if (user != null)
                {
                    user.Cookie = cookie;
                    session.UserId = user.Id;
                    user.LastSessionId = session.Id;
                    session.Save2();
                    user.Save2();
                    Session["user"] = user;
                }
                Session["cookie"] = cookie;
                Session["session"] = session;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208) // не найдена таблица
                {
                    db.CreateShema();
                }
                else
                    //if (ex.Number ==4060)  // не найдена база
                    Session["database"] = null;
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            TihieZoriDb.Session s = (Session)Session["session"];
            if (s != null)
            {
                s.SessionEnd = DateTime.UtcNow;
                s.Save2();
            }
        }
    }
}