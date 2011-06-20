<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ManageUsers
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>ManageUsers</h2>
    
    <iframe id="frame" frameborder="0" src='<%= ConfigurationManager.AppSettings["AdminPageUrl"] %>'
        scrolling="auto" name="frame" style="width:1200px; height:800px; margin-left: -90px; margin-right: 10px; border-right-width: 0px; border-left-width: 0px;">
    </iframe>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
