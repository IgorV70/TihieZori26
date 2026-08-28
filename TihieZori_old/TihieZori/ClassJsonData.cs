using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using DbCommon;
using TihieZoriDb;

namespace TihieZori
{
    public class ClassJsonData : IHttpHandler, IRequiresSessionState
    {

        #region Члены IHttpHandler

        public bool IsReusable
        {
            get { return true; }
        }

        HttpRequest Request;
        HttpResponse Response;
        CDatabaseTihieZori db;
        public void ProcessRequest(HttpContext context)
        {
            Request = context.Request;
            Response = context.Response;
            db = (CDatabaseTihieZori)context.Session["database"];
            string Source = Request.Params["source"];
            if (Source != null)
            {
                RequestSource(Source);
            }
            else
            {
                string SourceList = Request.Params["list"];
                RequestSourceList(SourceList);

            }
            Response.End();
        }

        private void RequestSourceList(string SourceList)
        {
            Response.ContentType = "text/json";
            SortList<SitePage> splist = null;
            if (SourceList == "SitePage")
            {
                splist = db.SitePage.GetObjectListByExpr(sp0 => sp0.Active > 0, "Active,Name");
            }
            Response.Write("[");
            Boolean first = true;
            foreach (SitePage sp in splist)
            {
                if (!first) Response.Write(",");
                Response.Write(sp.ToJson("id,Name,Title,MasterPage,Flags,Comment,Keywords,Vers,DatM,Active,Uid"));
                first = false;
            }
            Response.Write("]");
        }

        private void RequestSource(string Source)
        {
            string sId = Request.Params["id"];
            if (Source != null && sId != null)
            {
                FieldInfo fi = typeof(CDatabaseTihieZori).GetField(Source);
                if (fi != null)
                {
                    iTable it = (iTable)fi.GetValue(db);
                    int id = 0;
                    if (int.TryParse(sId, out id))
                    {
                        BObject obj = id == -1 ? it.CreateInstance() : (BObject)it.GetById(id);
                        if (obj != null)
                        {
                            string sAction = Request.Params["action"];
                            if (string.IsNullOrEmpty(sAction))
                            {
                                Response.ContentType = "text/json";
                                Response.Write(obj.ToJson(Request.Params["fields"]));
                            }
                            else
                                if (sAction == "save")
                            {
                                SitePage sp = (SitePage)obj;
                                if (sp != null)
                                {
                                    string sPageText = Request.Params["PageText"];
                                    if (sPageText != null)
                                        if (id > 0)
                                        {
                                            if (sp.PageText != sPageText)
                                            {
                                                int Active = sp.Active;
                                                sp.Active = 0;
                                                sp.SaveTry();
                                                sp = (SitePage)sp.Clone();
                                                sp.PageText = sPageText;
                                                sp.Vers++;
                                                sp.Active = Active;
                                            }
                                        }
                                        else
                                            sp.PageText = sPageText;
                                    string sName = Request.Params["Name"];
                                    if (sName != null)
                                        sp.Name = sName;
                                    string sTitle = Request.Params["Title"];
                                    if (sTitle != null)
                                        sp.Title = sTitle;
                                    string sMasterPage = Request.Params["MasterPage"];
                                    if (sMasterPage != null)
                                        sp.MasterPage = sMasterPage;
                                    string sComment = Request.Params["Comment"];
                                    if (sComment != null)
                                        sp.Comment = sComment;
                                    string sKeywords = Request.Params["Keywords"];
                                    if (sKeywords != null)
                                        sp.Keywords = sKeywords;
                                    string sFlags = Request.Params["Flags"];
                                    int iFlags = 0;
                                    if (int.TryParse(sFlags, out iFlags))
                                        sp.Flags = iFlags;
                                    if (sp.ObjectState != BObject.ObjectStateType.Unchanged)
                                        sp.DatM = DateTime.Now;
                                    sp.SaveTry();
                                    if (id < 0)
                                    {
                                        sp.Uid = sp.Id;
                                        sp.SaveTry();
                                    }
                                    StringBuilder ret = new StringBuilder();
                                    ret.Append("{\"id\":");
                                    ret.Append(sp.Id);
                                    ret.Append("}");
                                    Response.Write(ret.ToString());
                                }

                            }
                            //Response.Write(obj.ToJson());
                        }
                    }
                }
            }
        }

        #endregion Члены IHttpHandler
    }
}