<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.BrowseItemsViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	List
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial(StaticValues.Partial_TagView, Model.Tags); %>

    <% Html.RenderPartial(StaticValues.Partial_ItemBrowse, Model.Items); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
