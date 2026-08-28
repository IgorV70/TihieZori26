<%@ Page Title="Регистрация нового пользователя"  Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="TihieZori.Registration" MasterPageFile="~/Site.Master" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="container">
    <div class="row"> 
        <div class="col-lg-3"></div>
        <div class="col-lg-6">
            <form method="post" action="Registration.aspx" id="regform">
            <br />
                Если вы член товарищества, чтобы зарегистрироваться на сайте укажите номер участка и полностья Фамилию Имя Отчество,
                а также задайте свой логин, пароль
            <%=InfoError%>
            <table>
                <tr><td><label for="landnumber"><p align="right">№ участка&nbsp;</p></label></td><td><input type="text" id="landnumber" name="landnumber" maxlength="128" size="60" value="<%=landNumber%>" /></td></tr>
                <%=LandnumberError%>
                <tr><td><label for="fio"><p align="right">Фамилия Имя Отчество&nbsp;</p></label></td><td><input type="text" id="fio" name="fio" maxlength="128" size="60" value="<%=fio%>" /></td></tr>
                <%=UserNotExist%>
                <tr><td><label for="login"><p align="right">Логин&nbsp;</p></label></td><td><input type="text" id="login" name="login" maxlength="128" size="60" value="<%=login%>" /></td></tr>
                <%=EmptyUserName%>
                <%=UserNameExist%>
                <%=AlreadyRegistered%>
                <tr><td><label for="password"><p align="right">Пароль&nbsp;</p></label></td><td><input type="password" id="password" name="password" maxlength="128" size="60" value="<%=password%>" /></td></tr>
                <tr><td><label for="password2"><p align="right">Подтверждение&nbsp;</p></label></td><td><input type="password" id="password2" name="password2" maxlength="128" size="60" value="<%=password2%>" /></td></tr>
                <%=EmptyPassword%>
                <tr><td><label for="email"><p align="right">Электронная почта&nbsp;</p></label></td><td><input type="text" id="email" name="email" maxlength="128" size="60" value="<%=email%>" /></td></tr>
                <%=EmailExist%>
            </table>
            <br />
            <input type="submit" name="submit" id="submit" style="width: 150px;" value="Зарегистрироваться"/>
            <script type="text/javascript" src="js/registration.js"></script>
        </form>
        </div>
        <div class="col-lg-3"></div>
    </div>
</div>
</asp:Content>