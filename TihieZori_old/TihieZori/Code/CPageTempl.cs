using System;
using System.IO;
using TihieZoriDb;

namespace TihieZori.Code
{
    public partial class CPageTempl : System.Web.UI.Page
    {


        public User CurUser
        {
            get
            {
                return (User)Session["user"];
            }
        }

        SitePage _sp = null;

        public string Context2
        {
            get
            {
                return _sp == null ? "" : _sp.PageText;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreInit(EventArgs e)
        {
            Response.Expires = 0;
            base.OnPreInit(e);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            string loginpost = "";
            if (CurUser != null)
                loginpost = " - " + CurUser.Login!=null? CurUser.Login:CurUser.Email ;
            try
            {
                if (Master != null) Master.Page.Title = Master.Page.Title + loginpost;
            }
            catch
            {
                // ignored
            }
            string virtualFileName = Path.GetFileName(Request.FilePath);
            _sp = CPathProvider.db.SitePage.GetObjectByExpr(sp0 => sp0.Name == virtualFileName);
        }

        //protected override void Render(HtmlTextWriter writer)
        //{
        //    string virtualFileName = Path.GetFileName(Request.FilePath);
        //    SitePage sp = CGPSVirtualPathProvider.db.SitePage.GetObjectByExpr(sp0 => sp0.Name == virtualFileName);
        //    if (sp != null)
        //    {
        //        writer.WriteLine(sp.PageText);
        //    }
        //    //base.Render(writer);
        //}


    }
}