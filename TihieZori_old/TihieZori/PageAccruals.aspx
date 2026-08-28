<%@ Page Title="Взносы" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PageAccruals.aspx.cs" Inherits="TihieZori.PageAccruals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="js/Controls.js"></script>
    <script type="text/javascript" src="js/TableViewPm.js"></script>
    <script type="text/javascript" src="js/FilterForm.js"></script>
    <script type="text/javascript" src="js/AdmAccrual.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="content">
        <div id="delete-confirm" title="Удаление записи" class="ui-widget">
            <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>Удалить начисление?</p>
        </div>
        <div id="delete-confirm-pay" title="Удаление записи" class="ui-widget">
            <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>Удалить оплату?</p>
        </div>
        <div id="filtrForm" class="row form">
            <div class="col-xs-12">
                <label>Период расчетов:</label>
                 <select data-name="Period" data-value="Id" data-text="LandFio" data-control="SelectControl">
                    <%var thisyear = DateTime.Now.Year;
                        for (int year = 2017; year < thisyear; year++)
                            Response.Write(String.Format("<option value=\"{0}\">{0}</option>",year));
                        Response.Write(String.Format("<option value=\"{0}\" selected>{0}</option>", thisyear));
                    %>
                </select>
            </div>
            <div class="col-xs-12">
                <label>Участок :</label>
                <select data-source="User" data-name="UserId" data-value="Id" data-text="Land" data-control="SelectControl2">
                </select>
                <label>Назначение взноса :</label>
                <select data-source="Propose" data-name="ProposeId" data-value="Id" data-text="Name" data-control="SelectControl2">
                </select>
            </div>
            <div class="col-xs-12">
                <label>Только не оплаченные :</label>
                <input type="checkbox" data-name="NotPayment" data-control="CheckControl" />
            </div>
            <div class="col-xs-4"></div>
            <div class="col-xs-4">
                <input type="button" id="filter-action" value="Применить" class="button" />
            </div>
            <div class="col-xs-4"></div>
        </div>
        <div class="row" >
            <div style="display: flex;justify-content: center;">
                <table id="Accrual" data-name="Accrual" style="width:90%" border="1" >
                    <thead>
                        <tr>
                            <th>Дата</th>
                            <th>Назначение взноса</th>
                            <th>Участок</th>
                            <th>Сумма</th>
                            <th>Оплачено</th>
                            <th>Задолженность</th>
                        </tr>
                        <tr class="s1" style="display: none; padding:15px" >
                            <td class="r" data-name="AccDate" title="Дата взноса" data-control="DateControl"></td>
                            <td class="r" data-name="Propose.Name" title="Назначение взноса" style="max-width: 400px"></td>
                            <td class="r" data-name="User.LandNumber" title="Участок" style="max-width: 400px"></td>
                            <td class="r" data-name="AccSum" title="Сумма"></td>
                            <td class="r" data-name="PaySum" title="Оплачено"></td>
                            <td class="r" data-name="Itog" title="Задолженность"></td>
                        </tr>
                        <tr class="e1" style="display: none"> 
                        </tr>
                        <tr class="f1" style="display: none">
                            <td colspan="8">
                                <br />
                                <table id="Payment" data-name="Payment" style="margin-left: 3px" border="1">
                                    <thead>
                                         <tr class="fs1" style="display: none">
                                        </tr>
                                        <tr class="fe1" style="display: none">
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </td>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
                <br>
            </div>
        </div>
    </div>

</asp:Content>
