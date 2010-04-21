<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2>Shopper Screens</h2>
    <ul>
        <li><%= Html.ActionLink<ItemController>(a => a.List(), "Browse") %></li>
    </ul>
    
    <h2>User Screens</h2>
    <ul>
        <li><%= Html.ActionLink<ItemManagementController>(a => a.List(), "Items") %></li>
    </ul>
    
    <h2>Administrative Screens</h2>
    <ul>
        <li><%= Html.ActionLink("Application Management", "Index", "ApplicationManagement") %></li>
    </ul>
    
</asp:Content>
