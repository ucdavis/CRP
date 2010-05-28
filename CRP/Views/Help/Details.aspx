<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.HelpTopic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            Name:
            <%= Html.Encode(Model.Name) %>
        </p>
        <p>
            Description:
            <%= Html.Encode(Model.Description) %>
        </p>
        <p>
            AvailableToPublic:
            <%= Html.Encode(Model.AvailableToPublic) %>
        </p>
        <p>
            Id:
            <%= Html.Encode(Model.Id) %>
        </p>
    </fieldset>
    <p>
        <%=Html.ActionLink("Edit", "Edit", new { /* id=Model.PrimaryKey */ }) %> |
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

