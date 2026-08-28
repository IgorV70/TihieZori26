using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DbCommon;
using TihieZoriDb;

namespace TihieZori.Forum
{
    public partial class ForumControl : System.Web.UI.UserControl
    {
        private CDatabaseTihieZori _db;

        public User CurUser {
            get { return (User)Session["user"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (_db == null)
            {
                _db = (CDatabaseTihieZori)Session["database"];
            }
            if (_db != null)
            {
                string fpath = Request.FilePath;
                if (fpath[0] == '/') fpath = fpath.Substring(1);
                var ip = Request.UserHostAddress;
                SortList<Feedbacks> flist = _db.Feedbacks.GetObjectListByExpr(fb0 => fb0.fpath == fpath , "Dat1").Where(fb0=>fb0.Active || fb0.ip ==ip);
                writer.Write("<div class='comments'>");
                foreach (Feedbacks fb in flist)
                    RenderComment(writer, fb);
                writer.Write("</div>");
            }
            base.RenderControl(writer);
        }

        public static void RenderComment(HtmlTextWriter writer, Feedbacks fb)
        {
            writer.Write("<div class='comment'>");

            writer.Write("<div class='comment-name'>"); writer.Write(fb.Name);
            if (!fb.Active)
                writer.Write("&nbsp;<span style='color:red'> другие пользователи увидят ваше сообщение после проверки</span>");
            writer.Write("<span class='comment-dat'>"); writer.Write(fb.Dat1); writer.Write("</span>");
            writer.Write("<h4>"); writer.Write(fb.Title); writer.Write("</h4>");
            writer.Write("</div>");
            writer.Write("<div class='comment-text'>");
            writer.Write(fb.Message);
            writer.Write("</div>");

            writer.Write("</div>");
        }
    }

}