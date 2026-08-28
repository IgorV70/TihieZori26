<%@ Page Title="Администрирование объявлений" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdmAdvert.aspx.cs" Inherits="TihieZori.AdmAdvert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="js/Advert.js"></script>
    <script type="text/javascript" src="NicEditor/nicEdit.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="content">

        <table data-name="Advert" style="margin-left: 3px" border="1">
            <thead>
                <tr>
                    <th>Дата объявления</th>
                    <th>Заголовок</th>
                    <th>Текст объявления</th>
                    <th>Показывать</th>
                    <th>Изменить</th>
                    <th>Удалить</th>
                </tr>
                <tr class="s1" style="display: none">
                    <td class="r" data-name="DatM" title="Дата объявления" data-control="DateControl"></td>
                    <td class="r" data-name="Title" title="Заголовок"></td>
                    <td class="r" data-name="Comment" data-control="HtmlCnt" title="Текст" style="max-width:400px"></td>
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
                            <tr><td>Дата объявления</td><td><input data-name="DatM" type="text" value="" data-control="DpControl" /></td></tr>
                            <tr><td>Заголовок</td><td><input type="text" data-name="Title" value="" /></td></tr>
                            <tr><td>Текст</td>
                                <td><textarea id="myArea2" style="width: 800px; max-width:800px ;height: 200px;"  data-name="Comment" data-control="HtmlControl"></textarea>
                                </td>
                            </tr>
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
