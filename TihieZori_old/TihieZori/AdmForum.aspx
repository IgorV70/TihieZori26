<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdmForum.aspx.cs" Inherits="TihieZori.AdmUsluga" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="js/AdmForum.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="content">

        <table data-name="Feedbacks" style="margin-left: 3px" border="1">
            <thead>
                <tr>
                    <th>Имя</th>
                    <th>E-mail</th>
                    <th>Ip адрес</th>
                    <th>Дата объявления</th>
                    <th>Заголовок</th>
                    <th>Текст сообщения</th>
                    <th>Показывать</th>
                    <th>Изменить</th>
                    <th>Удалить</th>
                </tr>
                <tr class="s1" style="display: none">
                    <td class="r" data-name="Name" title="Текст" style="max-width:400px"></td>
                    <td class="r" data-name="email" title="E-mail" style="max-width:400px"></td>
                    <td class="r" data-name="ip" title="Ip адрес" style="max-width:400px"></td>
                    <td class="r" data-name="Dat1" title="Дата объявления" data-control="DateControl"></td>
                    <td class="r" data-name="Title" title="Заголовок"></td>
                    <td class="r" data-name="Message" title="Текст" style="max-width:400px"></td>
                    <td class="r" title="Показывать">
                        <input type="checkbox" data-name="Active" data-control="CheckControl" data-action="change:active" /></td>
                    <td class="c"><a data-action="edit">
                        <img src="img/edit.svg" alt="Редактировать" title="Редактировать" width="24"></a></td>
                    <td class="c"><a data-action="del">
                        <img src="img/del.svg" alt="Удалить" title="Удалить" width="24"></a></td>
                </tr>
                <tr class="e1" style="display: none">
                    <td colspan="7">
                        <br />
                        <table>
                            <tr><td>Дата объявления</td><td><input data-name="Dat1" type="text" value="" data-control="DpControl" /></td></tr>
                            <tr><td>Заголовок</td><td><input type="text" data-name="Title" value="" /></td></tr>
                            <tr><td>Текст сообщения</td><td><input type="text" data-name="Message" value="" /></td></tr>
                            <tr><td>Показать</td><td><input type="checkbox" data-name="Active" data-control="CheckControl" /></td></tr></table>
                        <br />
                        <input type="button" id="submit" style="width: 90px;" value="Сохранить" />
                        <input type="button" id="cansel" style="width: 90px;" value="Отмена" />
                        <br />
                        <br />
                    </td>
                </tr>
            </thead>
            <tbody></tbody>
        </table>
        <br>
        <a id="add">
            <img src="img/add.svg" alt="Добавить объявление" title="Добавить объявление" width="24">Добавить объявление</a>
        <br>
        <br>
        <br>
    </div>
</asp:Content>
