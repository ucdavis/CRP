<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ExtendedPropertyViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create Extended Property for <%= Html.Encode(Model.ItemType.Name) %></h2>

    <%= Html.ClientSideValidation<ExtendedProperty>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name") %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
            <p>
                <%= this.Select("QuestionType").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("Select a Question Type").Label("Question Type: ") %>
            </p>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.EditItemType(Model.ItemType.Id), "Back to Item Type") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

