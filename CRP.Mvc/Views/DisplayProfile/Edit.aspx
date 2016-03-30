<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.DisplayProfile>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	EditDisplayProfile
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>EditDisplayProfile</h2>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm("Edit", "DisplayProfile", FormMethod.Post, new { @enctype = "multipart/form-data" }))
       {%>

        <%= Html.AntiForgeryToken() %>
        <%= Html.ClientSideValidation<DisplayProfile>("") %>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name", Model.Name) %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
            
            <p>
                <img src='<%= Url.Action("GetLogo", new {id = Model.Id}) %>' />
            
                <label for="file">Logo:</label>
                <input type="file" name="file" id="file" />
            </p>
            <p>
                <label for="CustomCss">Custom Css:</label>
                <%= Html.TextArea("CustomCss", Model.CustomCss) %>
            </p>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink<DisplayProfileController>(a => a.List(), "Back to List") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <style type="text/css">
        textarea { width: 600px; height: 10em; }
    </style>

</asp:Content>
