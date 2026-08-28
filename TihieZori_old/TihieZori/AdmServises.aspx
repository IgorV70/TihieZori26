<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdmServises.aspx.cs" Inherits="TihieZori.AdmServises" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script  type="text/javascript" src="js/ServisesPage.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div id="content"><center>

<table id="ServisesPage" style="margin-left:3px" border="1">
<tr><th>Название&nbsp</th>
    <th>Адрес</th>
    <th>Телефон</th>
    <th>Телефон2</th>
    <th>Кол-во очередей</th>
    <th>Редактировать</th>
    <th>Удалить</th>
</tr>
    <tr class="s1" style="display:none" >
        <td id="Name" title="Название">Название</td>
        <td class="c" id="Address" title="Адрес">адрес</td>
        <td class="c" id="Phones" title="Телефон">+7319-999-999</td>
        <td class="c" id="NotePhone" title="Телефон2">+7319-999-999</td>
        <td class="c" id="StreamCount" title="Кол-во очередей">2</td>

        <td class="c">
                <a id="edit"><img src="img/edit.svg" alt="Редактировать" title="Редактировать" width="24"></a>
                <a id="editUsl"><img src="img/screwdriver.svg" alt="Услуги" title="Услуги" width="24"></a>
       </td>
        <td class="c">
                <a id="del"><img src="img/del.svg" alt="Удалить" title="Удалить" width="24"></a>
       </td>
     </tr>     
	<tr class="e1" style="display:none"><td colspan="7">
<center>
<br/>
<table border="1">
    <tr><td>Название</td><td><input id="Name" type="text" value="" /> </td></tr>
    <tr><td>Адрес</td><td><input id="Address" type="text" value="" /> </td></tr>
    <tr><td>Телефон</td><td><input type="text" id="Phones" value="" /> </td></tr>
    <tr><td>Телефон2</td><td><input type="text" id="NotePhone" value="" /> </td></tr>
    <tr><td>Кол-во очередей</td><td><input type="text" id="StreamCount" value="" /> </td></tr>
</table>
<br/>
<input type="button" id="submit" style="width: 90px;" value="Сохранить"/>
<input type="button" id="cansel" style="width: 90px;" value="Отмена"/>
</center>
<br/>
<br/></td>
</tr>
<tr class="e2" style="display:none"><td colspan="7">
<center>
<br/>
<table border="1">
    <tr><th>Название услуги</th><th>есть на этом сервисе</th></tr>
</table>
<br/>
<input type="button" id="submitUsl" style="width: 90px;" value="Сохранить"/>
<input type="button" id="canselUsl" style="width: 90px;" value="Отмена"/>
</center>
<br/>
<br/></td>
</tr>

</table> 
<br>
<a id="add"><img src="img/add.svg" alt="Добавить" title="Добавить" width="24">Добавить</a>
</center>
<br>
<br>
<br>


</div></asp:Content>
