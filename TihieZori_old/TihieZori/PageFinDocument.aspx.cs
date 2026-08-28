using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TihieZoriDb;

namespace TihieZori
{
    public partial class PageFinDocument : System.Web.UI.Page
    {
        CDatabaseTihieZori _db;

        public List<FinDocuments> DocList { get; set; }

        public string ImagePath(FinDocuments d)
        {
            string s = Path.GetExtension(d.Name);
            switch (s) {
                case ".doc":
                case ".docx": return @"img/docx.png";
                case "jpg": return @"img/jpg.png";
            }
            return @"img/jpg.png";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _db = (CDatabaseTihieZori)Session["database"];
            DocList = _db.FinDocuments.GetAll().Where(doc => doc.Active > 0).OrderBy(doc => doc.OrderId).ToList();

        }
    }
}