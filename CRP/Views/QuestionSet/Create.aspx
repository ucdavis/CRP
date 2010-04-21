<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Question Set
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ClientSideValidation<QuestionSet>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("QuestionSet.Name") %>
                <%= Html.ValidationMessage("QuestionSet.Name", "*") %>
            </p>
            <div style='display:<%= Model.Item == null ? Html.Encode("Block"): Html.Encode("None") %>'>
            <p>
                <label for="SystemReusable">SystemReusable:</label>
                <%= Html.CheckBox("QuestionSet.SystemReusable")%>
                <%= Html.ValidationMessage("QuestionSet.SystemReusable", "*")%>
            </p>
            <p>
                <label for="CollegeReusable">CollegeReusable: (This will be restricted to your college)</label>
                <%= Html.CheckBox("QuestionSet.CollegeReusable")%>
                <%= Html.ValidationMessage("QuestionSet.CollegeReusable", "*")%>
            </p>
            <p>
                <label for="UserReusable">UserReusable:</label>
                <%= Html.CheckBox("QuestionSet.UserReusable")%>
                <%= Html.ValidationMessage("QuestionSet.UserReusable", "*")%>
            </p>
            </div>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

