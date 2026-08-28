using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using TihieZoriDb;
using System.Web.Hosting;

namespace TihieZori.Code
{
    public class CLogon
    {
        public static User LogonUser(CDatabaseTihieZori dbGPS, string userLogin, string pwd)
        {
            if (string.IsNullOrEmpty(userLogin) || dbGPS == null || string.IsNullOrEmpty(pwd))
                return null;

            userLogin = userLogin.ToLower();

            User uemail = dbGPS.User.GetObjectByExpr(u => u.Id > 0 && ((u.Email != null && u.Email.ToLower() == userLogin) || (u.Login != null && u.Login.ToLower() == userLogin)));

            if (uemail == null || HostingEnvironment.IsDevelopmentEnvironment) return uemail;

            if (MD5Hash(pwd + uemail.salt) == uemail.Password) return uemail;            

            return null;
        }

        static Random rand = new Random((int)DateTime.Now.Ticks);

        public static string SetCookie(HttpResponse response, HttpCookie hc)
        {
            if (hc == null || hc.Value == "")
                hc = new HttpCookie("client", MD5Hash(rand.NextDouble().ToString(CultureInfo.InvariantCulture)));
            hc.Expires = DateTime.Now.AddMonths(6);
            response.SetCookie(hc);
            return hc.Value;
        }

        public static void ClearCookie(HttpResponse Response)
        {
            HttpCookie hc = new HttpCookie("usercoo", "");
            Response.SetCookie(hc);
        }

        public static string MD5Hash(string password)
        {

            string strHash = string.Empty;
            byte[] p = Encoding.Default.GetBytes(password);
            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(p))
                strHash += b.ToString("X2");
            return strHash;
        }

    }
}