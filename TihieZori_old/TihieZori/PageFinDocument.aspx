<%@ Page Title="Финансовые документы" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PageFinDocument.aspx.cs" Inherits="TihieZori.PageFinDocument" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col2">
    <% foreach (var doc in DocList)
        { %>
    <div class="container">
        <div class="row">
            <div class="col-xs-12">
                <a class="adoc" href="/FinDocs/<%=doc.Name%>">
                    <div class="row">
                        <div class="col-xs-3">
                            <img src="<%=ImagePath(doc)%>" alt="Скачать" width="51" height="51">
                        </div>
                        <div class="col-xs-4">
                            <%=doc.Title%>
                        </div>
                        <div class="col-xs-5"><%=doc.Comment%></div>
                    </div>
                </a>
            </div>
        </div>
    </div>
    <% } %>
        </div>
</asp:Content>
