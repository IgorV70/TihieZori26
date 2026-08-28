<%@ Page Title="Объявления" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PageAdvert.aspx.cs" Inherits="TihieZori.PageAdvert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% foreach (var adv in AdvList)
        { %>
    <div class="container advert">
        <div class="row">
            <h3 class="col-xs-12"><span class="date"><%=adv.DatM.AddHours(7).ToShortDateString()%></span>&nbsp;&nbsp;<%=adv.Title%></h3>
        </div>
        <div class="row">
            <div class="col-xs-12"><%=adv.Comment%></div>
        </div>
    </div>
    <% } %>
</asp:Content>
