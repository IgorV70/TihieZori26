using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TihieZoriDb;

namespace TihieZori
{
    public partial class PageMap : System.Web.UI.Page
    {
        CDatabaseTihieZori _db;


        protected void Page_Load(object sender, EventArgs e)
        {
            _db = (CDatabaseTihieZori)Session["database"];
            //AdvList = _db.Advert.GetAll().Where(adv => adv.Active > 0).OrderByDescending(adv => adv.DatM).ToList();
        }
    }
}