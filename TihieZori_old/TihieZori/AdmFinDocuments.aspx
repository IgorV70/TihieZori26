<%@ Page Title="Администрирование финансовых документов" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdmFinDocuments.aspx.cs" Inherits="TihieZori.AdmFinDocuments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script  type="text/javascript" src="js/FinDocuments.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div id="content"><div style="align-content:center">

<table data-Name="Documents" style="margin-left:3px" border="1" >
    <thead>
<tr><th>Наименование&nbsp</th>
    <th>Заголовок</th>
    <th>Показать в документах</th>
    <th>Комментарий</th>
    <th>Порядок</th>
    <th>Изменить</th>
    <th>Удалить</th>
</tr>
    <tr class="s1" style="display:none" >
        <td data-Name="Name" title="Наименование"></td>
        <td class="r" data-Name="Title" title="Заголовок"></td>
        <td class="r" title="Показать в документах"><input type="checkbox" data-Name="Active" data-setter="checkboxSetter"  data-getter="checkboxGetter" data-action="change:active" /></td>
        <td class="r" data-Name="Comment" title="Комментарий"></td>
        <td class="с">
            <a data-action="down"><img src="img/arrowDown.svg" alt="Опустить" title="Опустить" width="24"></a>
            <a data-action="up"><img src="img/arrowUp.svg" alt="Поднять вверх" title="Поднять вверх" width="24"></a>
        </td>
        <td class="c">
                <a data-action="edit"><img src="img/edit.svg" alt="Редактировать" title="Редактировать" width="24"></a>
       </td>
        <td class="c">
                <a data-action="del"><img src="img/del.svg" alt="Удалить" title="Удалить" width="24"></a>
       </td>
     </tr>     
	<tr class="e1" style="display:none"><td colspan="7">
<center>
<br/>
<table>
    <tr><td>Название</td><td><input data-Name="Name" type="text" value="" /></td></tr>
    <tr><td>Заголовок</td><td><input type="text" data-Name="Title" value="" /></td></tr>
    <tr><td>Показать в документах</td><td><input type="checkbox" data-Name="Active" data-setter="checkboxSetter" data-getter="checkboxGetter"/></td></tr>
    <tr><td>Комментарий</td><td><input type="text" data-Name="Comment" value="" /> </td></tr>
</table>
<br/>
<input type="button" id="submit" style="width: 90px;" value="Сохранить"/>
<input type="button" id="cansel" style="width: 90px;" value="Отмена"/>
</center>
<br/>
<br/></td>
</tr>
        </thead>
    <tbody></tbody>
</table> 
<br>
<a id="add"><img src="img/add.svg" alt="Добавить документ" title="Добавить документ" width="24">Добавить документ</a>
</div>
<br>
<br>
<br>


</div></asp:Content>
