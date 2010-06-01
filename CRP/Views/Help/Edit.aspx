<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.HelpTopic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>
        <%= Html.ClientSideValidation<HelpTopic>("") %>
        
        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name", Model.Name) %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
            <p>
                <label for="Description">Description:</label>
                <%= Html.TextBox("Description", Model.Description) %>
                <%= Html.ValidationMessage("Description", "*") %>
            </p>
            <p>
                <label for="AvailableToPublic">AvailableToPublic:</label>
                <%= Html.TextBox("AvailableToPublic", Model.AvailableToPublic) %>
                <%= Html.ValidationMessage("AvailableToPublic", "*") %>
            </p>
            <p>
                <label for="IsActive">IsActive:</label>
                <%= Html.TextBox("IsActive", Model.IsActive)%>
                <%= Html.ValidationMessage("IsActive", "*")%>
            </p>
            <p>
                <label for="NumberOfReads">NumberOfReads:</label>
                <%= Html.TextBox("NumberOfReads", Model.NumberOfReads)%>
                <%= Html.ValidationMessage("NumberOfReads", "*")%>
            </p>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>

