<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Resources"%>

<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2>Shopper Screens</h2>
    <ul>
        <li><%= Html.ActionLink<ItemController>(a => a.List(), "Browse") %></li>
        <li><%= Html.ActionLink<AccountController>(a => a.OpenIdAccount(), "Open Id Account") %></li>
    </ul>
    
    <h2>User Screens</h2>
    <ul>
        <li><%= Html.ActionLink<ItemManagementController>(a => a.List(), "Items") %></li>
        <li><%= Html.ActionLink<QuestionSetController>(a => a.List(), "Question Sets") %></li>
    </ul>
    
    <h2>Administrative Screens</h2>
    <ul>
        <li><%= Html.ActionLink("Application Management", "Index", "ApplicationManagement") %></li>
    </ul>
    
</asp:Content>
