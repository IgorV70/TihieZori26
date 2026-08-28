using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using TihieZoriDb;

namespace TihieZori
{
    public partial class loginError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user"] != null)
            {
                if (!string.IsNullOrEmpty(Request.Params["exit"]))
                {
                    Session["user"] = null;
                    Session s = (Session)Session["session"];
                    s.User = null;
                    s.Save2();
                    Response.Redirect("loginError.aspx");
                    return;
                }
            }
        } 

    }
}
