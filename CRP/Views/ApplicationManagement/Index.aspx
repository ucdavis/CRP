<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Application Management
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Application Management</h2>
<div id="adminscreen">    
    <ul>        
        <li><%= Html.ActionLink<ApplicationManagementController>(a => a.ListItemTypes(), "Item Types")%></li>
        <li><%= Html.ActionLink<QuestionSetController>(a => a.List(), "Question Sets") %></li>
        <li><%= Html.ActionLink<DisplayProfileController>(a => a.List(), "Display Profiles") %></li>
        <li><%= Html.ActionLink<TemplateController>(a => a.Edit(), "Edit System Confirmation Template") %></li>
        <li><%=Html.ActionLink<FIDController>(a => a.Index(), "FID management") %></li>
        <li><%=Html.ActionLink<ApplicationKeyController>(a => a.Index(), "Application Key management") %></li>
    </ul>
</div>

</asp:Content>


