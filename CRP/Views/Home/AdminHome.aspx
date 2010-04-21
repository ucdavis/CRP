<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AdminHome
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>AdminHome</h2>

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

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>
</asp:Content>
