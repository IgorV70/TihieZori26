using TihieZori.Code;
using TihieZoriDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TihieZori
{
    public partial class LoginPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Session["user"] as User;
            if (user != null)
            {
                if ((Request.Params["exit"]) != null)
                {
                    Session["user"] = null;
                    user.Cookie = "";
                    user.Save2();

                    Session s = (Session)Session["session"];
                    if (s != null)
                    {
                        s.User = null;
                        s.Save2();
                    }
                    //                        CLogon.ClearCookie(Response);
                    //                      Response.Redirect("loginPage.aspx");
                }
                Response.Redirect(".");
                return;
            }

            if (!string.IsNullOrEmpty(Request.Params["regme"]))
            {
                Response.Redirect("Registration.aspx");
                return;
            }
            if (!string.IsNullOrEmpty(Request.Params["submit"]))
            {
                string id = Session.SessionID;

                string userLogin = Request.Params["user"];
                string pwd = Request.Params["pwd"];

                string lang = Request.Params["lang"];
                if (string.IsNullOrEmpty(lang))
                {
                    lang = "русский";
                }

                string action = Request.Params["action"];

                string store_cookie;
                store_cookie = Request.Params["store_cookie"];

                var db = Session["database"] as CDatabaseTihieZori;

                user = CLogon.LogonUser(db, userLogin, pwd);
                if (user == null)
                {
                    Response.Redirect("loginError.aspx");
                    return;
                }

                Session["user"] = user;

                Session s = (Session)Session["session"];

                if (s != null && store_cookie == "on")
                {
                    user.Cookie = (string)Session["cookie"];
                    user.LastSession = s;
                    user.Save2();
                    s.User = user;
                    s.Save2();
                }
                Response.Redirect(".");
                return;
            }

        }
    }
}