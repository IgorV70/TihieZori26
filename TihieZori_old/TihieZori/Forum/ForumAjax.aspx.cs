using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.UI;
using TihieZoriDb;

namespace TihieZori.Forum
{
    public partial class ForumAjax : System.Web.UI.Page
    {
        CDatabaseTihieZori db;

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
            db = (CDatabaseTihieZori)Session["database"];
            string Name = Request.Params["Author"];
            string email = Request.Params["Email"];
            string title = Request.Params["title"];
            string message = Request.Params["message"];

            if (string.IsNullOrEmpty(Name))
            {
                if (curUser != null)
                {
                    Name = curUser.Login;
                    email = curUser.Email;
                    if (string.IsNullOrEmpty(Name))
                        Name = email;
                }
            }

            string errmessage = "";
            if (string.IsNullOrEmpty(email))
                errmessage += "Необходимо указать почтовый адрес! ";
            else
                if (!VerifyMail(email))
                errmessage += "Необходимо указать корректный почтовый адрес! ";
            if (string.IsNullOrEmpty(Name))
                errmessage += "Необходимо указать имя! ";
            if (string.IsNullOrEmpty(message))
                errmessage += "Необходимо нечто написать в сообщении! ";

            if (errmessage == "")
            {
                string fpath = Request.UrlReferrer.LocalPath;
                if (fpath[0] == '/') fpath = fpath.Substring(1);
                Feedbacks fbs = db.Feedbacks.NewInstance();
                fbs.Dat1 = DateTime.Now;
                fbs.Name = Name;
                fbs.email = email;
                fbs.Title = title;
                fbs.Message = message;
                fbs.fpath = fpath;
                fbs.ip = Request.UserHostAddress;
                fbs.Active = (curUser != null);
                if (curUser != null)
                    fbs.Sender = curUser;
                fbs.Save2();
                //Response.Write("{\"save\":\"ok\"}");
                ForumControl.RenderComment(writer, fbs);
            }
            else
                Response.Write("<div class='comment-error'>" + errmessage + "</div>");

            base.Render(writer);
        }


        private bool VerifyMail(string email)
        {
            if (Regex.IsMatch(email, @"^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(?:\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(?:aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$", RegexOptions.IgnoreCase))
            {
                string domain = email.Substring(email.IndexOf("@") + 1); //вырезаем домен
                IPHostEntry iphe;
                try
                {
                    //ip = Dns.Resolve(domain).AddressList[0].ToString(); //пытаемся получить ай-пи , если получаем то можно считать что есть ящик (хотя тут бы ещо телнетом проверить)
                    iphe = Dns.GetHostEntry(domain);
                    if (iphe != null)
                        return true;
                }
                catch (System.Net.Sockets.SocketException)
                {
                }

            }
            return false;
        }
    }

}