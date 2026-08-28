using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TihieZoriDb;

namespace TihieZori
{
    public partial class SiteMaster : MasterPage
    {
        public RoleEnum UserRole
        {
            get
            {
                User user = Session["user"] as User;
                return (user != null) ? user.UserRole : RoleEnum.Guest;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Crumbs.Text = Page.Title;
        }
    }
}