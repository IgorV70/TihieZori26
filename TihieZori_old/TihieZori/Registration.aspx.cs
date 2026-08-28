using System;
using System.Web.UI;
using TihieZoriDb;
using TihieZori.Code;

namespace TihieZori
{
    public partial class Registration : System.Web.UI.Page
    {
        public string landNumber = "";
        public string fio = "";
        public string login = "";
        public string password = "";
        public string password2 = "";
        public string email = "";


        bool b_LandNumberOk = true;
        bool b_EmptyUserName = false;
        bool b_EmailExist = false;
        bool b_UserNameExist = false;
        bool b_EmptyPassword = false;

        bool b_UserNotExists = false;
        bool b_AlreadyRegistered = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            landNumber = Request.Params["landnumber"];
            fio = Request.Params["fio"];
            login = Request.Params["Login"];
            password = Request.Params["Password"];
            password2 = Request.Params["Password2"];
            email = Request.Params["Email"];
            string submit = Request.Params["submit"];
            if (login != null) login = login.Trim();
            if (landNumber != null) landNumber = landNumber.Trim();
            if (fio != null) fio = fio.Trim();
            if (password != null) password = password.Trim();
            if (password2 != null) password2 = password2.Trim();
            if (email != null) email = email.Trim();
            if (submit != null) submit = submit.Trim();
            if (!string.IsNullOrEmpty(landNumber))
            {
                b_LandNumberOk= true;
                if (string.IsNullOrEmpty(login)) b_EmptyUserName = true;
                if (string.IsNullOrEmpty(password) || password != password2) b_EmptyPassword = true;
                CDatabaseTihieZori db = (CDatabaseTihieZori)Session["database"];
                User uemail = string.IsNullOrEmpty(email) ? null : (User)db.User.GetObjectByExpr(u => u.Email == email);
                User uname = (User)db.User.GetObjectByExpr(u => u.Login == login);
                if (uemail != null) b_EmailExist = true;
                if (uname != null) b_UserNameExist = true;
                if (b_EmptyUserName || b_EmailExist || b_UserNameExist || b_EmptyPassword) return;

                User user = db.User.GetObjectByExpr(u => u.LandNumber == landNumber && (u.Fio == fio || u.FioDover == fio));
                if (user == null)
                {
                    b_UserNotExists = true;
                    return;
                }
                if ((!string.IsNullOrEmpty(user.Login) || !string.IsNullOrEmpty(user.Email)))
                {
                    b_AlreadyRegistered = true;
                    return;
                }
                user.Login = login;
                user.salt = CLogon.MD5Hash(DateTime.Now.ToString());
                user.Password = CLogon.MD5Hash(password + user.salt);
                user.Email = email;
                user.UserRole = RoleEnum.User;
                if (user.Login == "admin@tihiezori.tk")
                {
                    user.UserRole = RoleEnum.Admin;
                }
                user.Save2();
                Session["user"] = user;
                Response.Redirect(".");
            }
        }

        public string InfoError
        {
            get
            {
                if (!b_LandNumberOk|| b_EmptyUserName || b_EmailExist || b_UserNameExist || b_EmptyPassword || b_UserNotExists || b_AlreadyRegistered)
                    return "<span style='color:red'>Ошибка при регистрации пользователя!</span>";
                return "";
            }
        }

        public string EmptyPassword
        {
            get
            {
                if (b_EmptyPassword)
                    return "<tr><td colspan=2>не введен пароль или пароли не совпадают !</tr>";
                return "";
            }
        }

        public string LandnumberError
        {
            get
            {
                if (!b_LandNumberOk)
                    return "<tr><td colspan=2>не указан номер участка(или нет участка с таким номером) !</tr>";
                return "";
            }
        }

        public string UserNotExist
        {
            get
            {
                if (b_UserNotExists)
                    return "<tr><td colspan=2>Данные указаны неверно!(ФИО, номер участка)</tr>";
                return "";
            }
        }

        public string AlreadyRegistered
        {
            get
            {
                if (b_AlreadyRegistered)
                    return "<tr><td colspan=2>Пользователь уже зарегистрирован, если это произошло по ошибке обратитесь к администратору!</tr>";
                return "";
            }
        }

        public string EmptyUserName
        {
            get
            {
                if (b_EmptyUserName)
                    return "<tr><td colspan=2>укажите логин!</tr>";
                return "";
            }
        }

        public string UserNameExist
        {
            get
            {
                if (b_UserNameExist)
                    return "<tr><td colspan=2>Пользователь с таким логином уже зарегистрирован!</tr>";
                return "";
            }
        }

        public string EmailExist
        {
            get
            {
                if (b_EmailExist)
                    return "<tr><td colspan=2>Пользователь с таким почтовым адресом уже зарегистрирован!</tr>";
                return "";
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //if (is_sucsess) Redirect("User.aspx");            else
            base.Render(writer);
        }

    }
}
