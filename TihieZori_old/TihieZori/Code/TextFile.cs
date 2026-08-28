using System.IO;
using System.Text;
using System.Web.Hosting;
using TihieZoriDb;

namespace TihieZori.Code
{
    /// <summary>
    /// This class returns the ASP.Net markup for the requested virtual file.
    /// </summary>
    public class TextFile : VirtualFile
    {
        private readonly string _path;
        private CPathProvider _provider;
        /// <summary>
        /// constructor to initialize the member values of the local instance and of the base class
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="provider"></param>
        public TextFile(string virtualPath, CPathProvider provider)
            : base(virtualPath)
        {
            _path = virtualPath;
            _provider = provider;
        }

        /// <summary>
        /// This will return the ASP.Net markup in any of the stream format.
        /// </summary>
        /// <returns></returns>
        public override Stream Open()
        {

            string virtualFileName = Path.GetFileName(_path);
            SitePage sp = CPathProvider.db.SitePage.GetObjectByExpr(sp0 => sp0.Name == virtualFileName && sp0.Active > 0);
            Stream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter(stream, Encoding.Default);

            // первая строка
            sw.Write("<%@ Page Title=\"");
            sw.Write(sp.Title);
            sw.Write("\" Language=\"C#\" MasterPageFile=\"~/");
            sw.Write(string.IsNullOrEmpty(sp.MasterPage) ? "Empty" : sp.MasterPage);
            sw.WriteLine(".Master\" AutoEventWireup=\"true\" CodeBehind=\"TihieZori.Code.CPageTempl.cs\" Inherits=\"TihieZori.Code.CPageTempl\" %>");

            if ((sp.Flags & 1) > 0)
                sw.WriteLine("<%@ Register src=\"Comments/ForumComment.ascx\" tagname=\"ForumComment\" tagprefix=\"uc1\" %>");

            sw.Write(@"<asp:Content ID=""Content1"" ContentPlaceHolderID=""head"" runat=""server"">
<meta name=""description"" content="""); sw.Write(sp.Title); sw.WriteLine("\" />");

            sw.Write("<meta name=\"Keywords\" content=\"");
            sw.Write(sp.Keywords);
            sw.WriteLine("\"/>");

            sw.WriteLine("</asp:Content>");
            sw.WriteLine("<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"MainContent\" runat=\"server\">");


            //sw.WriteLine(sp.PageText);
            sw.WriteLine("<%=Context2%>");

            if ((sp.Flags & 1) > 0)
                sw.WriteLine("<uc1:ForumComment ID=\"ForumComment1\" runat=\"server\" />");

            sw.WriteLine("</asp:Content>");

            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Replaces placeholders with proper markup
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string ConvertMarkup(string line)
        {
            string markup;

            if (line.Contains("<PAGE>"))
                markup = "<% @ Page Language=\"C#\" AutoEventWireup=\"true\" Title = \"Virtual Path Provider Sample\" %>";
            else if (line.Contains("<BUTTON>"))
                markup = "<input type=button ID=\"Button1\" value=\"Click Here\" onClick=\"displayMessage();\" />";
            else
                markup = line;

            return markup;
        }
    }
}

