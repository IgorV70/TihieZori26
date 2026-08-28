using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DbCommon;
using TihieZoriDb;

namespace TihieZori.Code
{
    public class DocumentLoad : IHttpHandler, IRequiresSessionState
    {

        public bool IsReusable
        {
            get { return true; }
        }

        CDatabaseTihieZori _db;
        HttpRequest _request;
        HttpResponse _response;

        private string _uploadsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Docs");
        public void ProcessRequest(HttpContext context)
        {
            string referer = context.Request.UrlReferrer.AbsolutePath;
            if (referer.Contains("Fin"))
                _uploadsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FinDocs");
            _request = context.Request;
            _response = context.Response;
            _response.ContentType = "text/json";
            _db = (CDatabaseTihieZori)context.Session["database"];

            DirectoryInfo uploads = new DirectoryInfo(_uploadsDir);
            if (!uploads.Exists)
            {
                //uploads.Create();
                context.Response.Write("{\"error\":\"отсутствует папка " + _uploadsDir + "\"}");
                return;
            }

            if (context.Request.Files.Count == 1)
            {
                HttpPostedFile file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    try
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        file.SaveAs(Path.Combine(uploads.FullName, fileName));
                        if (referer.Contains("Fin"))
                        {
                            var docRec = _db.FinDocuments.GetObjectByExpr(doc => doc.Name == fileName);
                            if (docRec == null)
                            {
                                docRec = _db.FinDocuments.NewInstance();
                                docRec.Name = fileName;
                                docRec.Title = Path.GetFileNameWithoutExtension(file.FileName);
                                docRec.OrderId = GetMaxOrderId();
                                docRec.Save2();
                            }
                            context.Response.Write(docRec.ToJsonArray());
                        }
                        else
                        {
                            var docRec = _db.Documents.GetObjectByExpr(doc => doc.Name == fileName);
                            if (docRec == null)
                            {
                                docRec = _db.Documents.NewInstance();
                                docRec.Name = fileName;
                                docRec.Title = Path.GetFileNameWithoutExtension(file.FileName);
                                docRec.OrderId = GetMaxOrderId();
                                docRec.Save2();
                            }
                            context.Response.Write(docRec.ToJsonArray());
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        context.Response.Write("{\"error\":\"" + ex.Message + "\"}");
                        return;
                    }
                }
            }
            context.Response.Write("{\"error\":\"в запросе нет файла, или файл нулевой длины\"}");
            return;
        }

        private int GetMaxOrderId()
        {
            var list = _db.Documents.GetAll();
            if (list.Count == 0)
                return 10;
            return list.Max(doc => doc.OrderId) + 10;
        }
    }
}