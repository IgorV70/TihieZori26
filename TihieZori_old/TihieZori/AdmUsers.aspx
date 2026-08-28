<%@ Page Title="Администрирование пользователей" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdmUsers.aspx.cs" Inherits="TihieZori.AdmUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="js/AdmUser.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="content">
        <table data-name="User" style="margin-left: 3px" border="1">
            <thead>
                <tr>
                    <!-- th>Роль</th !-->
                    <th>№ уч.</th>
                    <th>Фамилия И.О.</th>
                    <th>Логин</th>
                    <th>е-майл</th>
                    <th>Телефон</th>
                    <th>Изменить</th>
                    <th>Удалить</th>
                </tr>
                <tr class="s1" style="display: none">
                    <!--td class="r" data-name="Role.Name" title="Роль"></!--td!-->
                    <td class="r" data-name="LandNumber" title="№ участка"></td>
                    <td data-name="Fio" title="Фамилия И.О."></td>
                    <td class="r" data-name="Login" title="Логин"></td>
                    <td class="r" data-name="Email" title="е-майл" ></td>
                    <td class="r" data-name="Phone" title="Телефон" ></td>
                    <td class="c"><a data-action="edit">
                        <img src="img/edit.svg" alt="Редактировать" title="Редактировать" width="24"></a></td>
                    <td class="c"><a data-action="del">
                        <img src="img/del.svg" alt="Удалить" title="Удалить" width="24"></a></td>
                </tr>
                <tr class="e1" style="display: none">
                    <td colspan="7">
                        <br />
                        <table>
                            <tr><td>№ участка</td><td><input data-name="LandNumber" type="text" value="" /></td></tr>
                            <tr><td>ФИО</td><td><input data-name="Fio" type="text" value="" /></td></tr>
                            <tr><td>Логин</td><td><input type="text" data-name="Login" value="" /></td></tr>
                            <tr><td>е-майл</td><td><input type="text" data-name="Email" value="" /></td></tr>
                            <tr><td>Телефон</td><td><input type="text" data-name="Phone" value="" /></td></tr>
                         </table>
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
            <img src="img/add.svg" alt="Добавить Пользователя" title="Добавить Пользователя" width="24">Добавить Пользователя</a>
        <br>
        <br>
        <br>
    </div>
</asp:Content>
