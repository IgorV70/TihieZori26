<%@ Page Title="Взносы" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdmAccruals.aspx.cs" Inherits="TihieZori.AdmAccruals" %>

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
                <select data-source="User" data-name="UserId" data-value="Id" data-text="LandFio" data-control="SelectControl2">
                </select>
                <label>Назначение взноса :</label>
                <select data-source="Propose" data-name="ProposeId" data-value="Id" data-text="Name" data-control="SelectControl2">
                </select>
            </div>
            <div class="col-xs-12">
                <label>Только не оплаченные :</label>
                <input type="checkbox" data-name="NotPayment" data-control="CheckControl" />
            </div>
            <div class="col-xs-4" ></div>
            <div class="col-xs-4">
                <input type="button" id="filter-action" value="Применить" class="button"/>
            </div>
            <div class="col-xs-4" ></div>
        </div>
        <div>
            <table id="Accrual" data-name="Accrual" style="margin-left: 3px" border="1">
                <thead>
                    <tr>
                        <th>Дата взноса</th>
                        <th>Назначение взноса</th>
                        <th>Участок</th>
                        <th>Сумма</th>
                        <th>Оплачено</th>
                        <th>Задолженность</th>
                        <th>Оплата</th>
                        <th>Изменить</th>
                        <th>Удалить</th>
                    </tr>
                    <tr class="s1" style="display: none">
                        <td class="r" data-name="AccDate" title="Дата взноса" data-control="DateControl"></td>
                        <td class="r" data-name="Propose.Name" title="Назначение взноса" style="max-width: 400px"></td>
                        <td class="r" data-name="User.LandNumber" title="Участок" style="max-width: 400px"></td>
                        <td class="sum" data-name="AccSum" title="Сумма" data-control="FloatControl"></td>
                        <td class="sum" data-name="PaySum" title="Оплачено" data-control="FloatControl"></td>
                        <td class="sum" data-name="Itog" title="Задолженность" data-control="FloatControl"></td>
                        <td class="c"><a data-action="edit">
                            <img src="img/edit.svg" alt="Редактировать" title="Редактировать" width="24"></a></td>
                        <td class="c"><a data-action="money">
                            <img src="img/money.svg" alt="Оплата" title="Оплата" width="24"></a></td>
                        <td class="c"><a data-action="del">
                            <img src="img/del.svg" alt="Удалить" title="Удалить" width="24"></a></td>
                    </tr>
                    <tr class="e1" style="display: none">
                        <td colspan="8">
                            <br />
                            <table>
                                <tr>
                                    <td>Участок(ФИО)</td>
                                    <td>
                                        <select data-source="User" data-name="UserId" data-value="Id" data-text="LandFio" data-control="SelectControl">
                                        </select>
                                    </td>
                                </tr>
                                <tr class="error errorUser" style="display: none" data-error="UserId">
                                    <td colspan="2">Необходимо указать участок</td>
                                </tr>
                                <tr>
                                    <td>Дата взноса</td>
                                    <td>
                                        <input data-name="AccDate" type="text" value="" data-control="DpControl" /></td>
                                </tr>
                                <tr>
                                    <td>Назначение взноса</td>
                                    <td>
                                        <input type="text" data-source="Propose" data-name="ProposeId" data-value="Id" data-text="Name" data-control="InputListControl" list="l100" />
                                        <datalist id="l100"></datalist>
                                    </td>
                                </tr>
                                <tr class="error errorPropose" style="display: none" data-error="ProposeId">
                                    <td colspan="2">Необходимо указать назначение</td>
                                </tr>
                                <tr>
                                    <td>Сумма</td>
                                    <td>
                                        <input type="text" data-name="AccSum" value="0" data-control="FloatControl" /></td>
                                </tr>
                                <tr class="error errorAccSum" style="display: none" data-error="AccSum">
                                    <td colspan="2">Введите сумму большую 0!</td>
                                </tr>
                            </table>
                            <br />
                            <input type="button" id="submit" style="width: 90px;" value="Сохранить" />
                            <input type="button" id="cansel" style="width: 90px;" value="Отмена" />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr class="f1" style="display: none">
                        <td colspan="8">
                            <br />
                            <table id="Payment" data-name="Payment" style="margin-left: 3px" border="1">
                                <thead>
                                    <tr>
                                        <th>Дата оплаты</th>
                                        <th>Назначение взноса</th>
                                        <th>Участок</th>
                                        <th>Сумма</th>
                                        <th>Изменить</th>
                                        <th>Удалить</th>
                                    </tr>
                                    <tr class="fs1" style="display: none">
                                        <td class="r" data-name="PayDate" title="Дата оплаты" data-control="DateControl"></td>
                                        <td class="r" data-name="Propose.Name" title="Назначение взноса" style="max-width: 400px"></td>
                                        <td class="r" data-name="User.LandNumber" title="Участок" style="max-width: 400px"></td>
                                        <td class="r" data-name="PaySum" title="Сумма"></td>
                                        <td class="c"><a data-action="edit">
                                            <img src="img/edit.svg" alt="Редактировать" title="Редактировать" width="24"></a></td>
                                        <td class="c"><a data-action="del">
                                            <img src="img/del.svg" alt="Удалить" title="Удалить" width="24"></a></td>
                                    </tr>
                                    <tr class="fe1" style="display: none">
                                        <td colspan="7">
                                            <br />
                                            <table>
                                                <tr>
                                                    <td>Участок(ФИО)</td>
                                                    <td data-name="User.LandFio" data-control="TextControl" style="max-width: 400px"></td>
                                                </tr>
                                                <tr>
                                                    <td>Дата опллаты</td>
                                                    <td>
                                                        <input data-name="PayDate" type="text" value="" data-control="DpControl" /></td>
                                                </tr>
                                                <tr>
                                                    <td>Назначение взноса</td>
                                                    <td data-name="Propose.Name" data-control="TextControl" title="Назначение взноса" style="max-width: 400px"></td>
                                                    <!--td data-name="Propose.Name"></td-->
                                                </tr>
                                                <tr>
                                                    <td>Сумма</td>
                                                    <td>
                                                        <input type="text" data-name="PaySum" value="0" data-control="FloatControl" /></td>
                                                </tr>
                                                <tr class="error errorPaySum" style="display: none" data-error="PaySum">
                                                    <td colspan="2">Введите сумму большую 0!</td>
                                                </tr>
                                            </table>
                                            <br />
                                            <input type="button" id="submit1" style="width: 90px;" value="Сохранить" />
                                            <input type="button" id="cansel1" style="width: 90px;" value="Отмена" />
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                            <a id="addPay">
                                <img src="img/add.svg" alt="Добавить оплату" title="Добавить оплату" width="24">Добавить оплату</a>
                            <br>
                        </td>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
            <br>
            <a id="addAcc">
                <img src="img/add.svg" alt="Добавить назначение" title="Добавить начисление" width="24">Добавить начисление</a>
            <br>
            <br>
            <br>
        </div>
    </div>
</asp:Content>
