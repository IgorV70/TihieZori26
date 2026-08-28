<%@ Page Title="Ошибка регистрации" Language="C#" AutoEventWireup="true" CodeBehind="loginError.aspx.cs" Inherits="TihieZori.loginError" MasterPageFile="~/Site.Master"%>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" Runat="Server">

<form class="login_bg_form" id="login_form" action="loginError.aspx" method="POST">
    <br/>
	<table class="login_bg_table">
	<tr><td style="padding-left: 30px;"></td><td valign="middle">
	    <table width="300px">
		<tr align="left">
			<td>
				&nbsp;<span style="color:Red;">Неправильно набран логин или пароль!</span></td>
		</tr>
		<tr align="left">
			<td align="center"><hr size="1px" /><a style="width: 70px;" href="loginPage.aspx">Повторить вход</a></td>
		</tr>
		
		<tr>
			<td height="30">
    		</td>
		</tr>
		<tr>
			<td height="30" align="left"><a class="ajax" id="reset_password_link" href="Registration.aspx">Регистрация</a>
			</td>
		</tr>
		<tr>
			<td height="30" align="left"><a class="reset_password_link" id="A1" href="reset_password.html">Восстановление пароля</a>
			</td>
		</tr>
	</table>
	</td></tr>
	</table>
	<input type="hidden" name="action" value="login"/>
<script type="text/javascript" src="js/login.js"></script>
</form>
</asp:Content>
