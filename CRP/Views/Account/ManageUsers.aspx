<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ManageUsers
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>ManageUsers</h2>
    
    <iframe id="frame" frameborder="0" src='<%= ConfigurationManager.AppSettings["AdminPageUrl"] %>'
        scrolling="auto" name="frame" style="width:100%; height:100%;">
    </iframe>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
