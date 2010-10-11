<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.TouchnetFID>" %>
<%@ Import Namespace="CRP.Controllers" %>
<%@ Import Namespace="CRP.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        
        <% Html.RenderPartial(StaticValues.Partial_FidForm); %>
        <p>
            <input type="submit" value="Save" />
        </p>

    <% } %>

    <div>
        <%=Html.ActionLink<FIDController>(a => a.Index(), "Back to List") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

