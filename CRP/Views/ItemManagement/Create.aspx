<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">

    <%
        Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">
        var getExtendedPropertyUrl = '<%= Url.Action("GetExtendedProperties", "ItemManagement") %>';
        var scriptUrl = '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>';
    </script>

    <script src='<%= Url.Content("~/Scripts/ItemEdit.js") %>' type="text/javascript"></script>
    <script src='<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>' type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            $("#Item_Description").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>' });
            $("#Item_CheckPaymentInstructions").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overRideHeigth: '255' });      
        });
   </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm("Create", "ItemManagement", FormMethod.Post, new { @enctype = "multipart/form-data" }))
       {%>
    <% Html.RenderPartial(StaticValues.Partial_ItemForm); %>
    <% } %>

    <div align=right>
        <%=Html.ActionLink<ItemManagementController>(a => a.List(null), "Back to List") %>
    </div>

</asp:Content>



