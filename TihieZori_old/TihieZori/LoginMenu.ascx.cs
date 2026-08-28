using TihieZoriDb;
using System;
using System.Web.UI;

namespace TihieZori
{
    public partial class LoginMenu : UserControl
    {
        User _user = null;
        public User curUser
        {
            get
            {
                if (_user == null)
                    _user = (User)Session["user"];
                return _user;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (curUser == null)
                writer.Write("<li id = \"" + ID + "\" ><a href = \"LoginPage.aspx\" > Вход </a></li>");
            else
                writer.Write("<li id = \"" + ID + "\" ><a href = \"LoginPage.aspx?exit=1\"> Выход(" + (curUser.Login != null ? curUser.Login : curUser.Email) + ") </a></li>");

        }
    }
}