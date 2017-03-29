<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>
<%@ Import Namespace="CRP.Controllers.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AdminHome
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="adminscreen">

    <h2>Administration Tools</h2>

    <h2>User Screens</h2>
    <ul>
        <%--<li><%= Html.ActionLink<ItemManagementController>(a => a.List(null), "Items") %></li>--%>
        <li><%= Url.ItemManagementListLink("Items")%></li>
        <li><%= Html.ActionLink<QuestionSetController>(a => a.List(), "Question Sets") %></li>
        <li><%= Html.ActionLink<TransactionController>(a => a.AdminLookup(null), "Transaction Lookup") %></li>
    </ul>
    
    <h2>Administrative Screens</h2>
    <ul>                
        <li><%= Html.ActionLink<ApplicationManagementController>(a => a.Index(), "Application Management") %></li>
        <li><%= Html.ActionLink<ReportController>(a => a.ViewSystemReport(null), "System Reports") %></li>
        <li><%= Html.ActionLink<AccountController>(a => a.ManageUsers(), "Manage Users") %></li>      
    </ul>
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>