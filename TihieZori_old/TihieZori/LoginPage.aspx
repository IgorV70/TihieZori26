<%@ Page Title="Вход в систему" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="TihieZori.LoginPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<meta name="description" content="Вход в систему для членов ДНТ." />
<meta name="Keywords" content="Вход в систему."/>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <br/>
    <h1 style="text-align:center;">Вход в систему</h1>
   <form class="login_bg_form" id="login_form" action="loginPage.aspx" method="POST">
 	<table class="login_bg_table">
	<tr><td style="padding-left: 30px;"></td><td style="vertical-align:middle">
	<table style="width:100%">
		<tr>
			<td id="user_td"  style="padding:5px;text-align:right"> Пользователь </td>
			<td><input type="text" name="user" id="user" size="20" value="" style="width:150px;" /></td>
			<td  style="text-align:left" colspan="2"><input type="submit" name="submit" id="submit" style="width:70px;margin-left:30px;" value="Войти"/></td>
		</tr>
		<tr>
			<td id="password_td" align="right" style="padding:5px;"> Пароль </td>
			<td><input type="password" name="pwd" id="pwd" size="20" style="width:150px;"/></td>
            <td  style="text-align:left" colspan="2"><input type="submit" name="regme" id="regme" style="width:150px;margin-left:30px;" value="Регистрация"/></td>
		</tr>
		<tr style="text-align:left">
			<td colspan="2" style="text-align:right">
				<input type="checkbox" name="store_cookie" style="border: 0px solid #FFFFFF; background: transparent;" checked="checked"/>
				<label id="remeber_on_this_computer_label"> Запомнить меня на этом компьютере </label>
			</td>
		</tr>
	</table>
	</td></tr>
	</table>
	<br />
	<input type="hidden" name="action" value="login"/>
<script type="text/javascript" src="js/login.js"></script>
</form>
</asp:Content>