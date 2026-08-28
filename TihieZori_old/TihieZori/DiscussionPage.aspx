<%@ Page Title="Обсуждение" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DiscussionPage.aspx.cs" Inherits="TihieZori.DiscussionPage" %>
<%@ Register src="Forum/ForumControl.ascx" tagname="Forum" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<uc1:Forum ID="Forum1" runat="server" />
</asp:Content>
