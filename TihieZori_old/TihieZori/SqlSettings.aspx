<%@ page title="" language="C#" masterpagefile="~/Site.Master" autoeventwireup="true" codebehind="SqlSettings.aspx.cs" inherits="TihieZori.SqlSettingsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
    if (Session["database"] == null)
    {
%>
    <div class="container">
        <div class="row">
            <div class="col-lg-3"></div>
            <div class="col-lg-6">
                <form method="post" action="SqlSettings.aspx" >
                    <table>
                        <tr><td><label>Имя базы данных</label></td><td><input type="text" name="bdname" id="bdname" maxlength="50" value="" /></td></tr>
                        <tr><td><label>Имя пользователя</label></td><td><input type="text" name="login" id="login" maxlength="50" value="" /></td></tr>
                        <tr><td><label>Пароль</label></td><td><input type="text" name="password" id="password" maxlength="50" value="" /></td></tr>
                        <tr><td>&nbsp</td><td><input type="submit" name="submit" id="submit" style="width: 150px;" value="Готово"/></td></tr>
                    </table>
                </form>
            </div>
            <div class="col-lg-3"></div>
        </div>
    </div>
<%
    }
%>
</asp:Content>
