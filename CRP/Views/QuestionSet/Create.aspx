<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Controllers"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Question Set
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
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
            
            <div style='display:<%= Model.Item != null || Model.ItemType != null ? Html.Encode("Block") : Html.Encode("None") %>'>
            
                <p>
                    <label for="Transaction">Transaction Level:</label>
                    <%= Html.CheckBox("transaction") %>
                </p>
                
                <p>
                    <label for="Quantity">Quantity Level:</label>
                    <%= Html.CheckBox("quantity") %>
                </p>
            
            </div>
            
            <div style='display:<%= Model.Item == null && Model.ItemType == null ? Html.Encode("Block"): Html.Encode("None") %>'>
            
            <% if (Model.IsAdmin) {%>
            <p>
                
                <label for="SystemReusable">SystemReusable:</label>
                <%= Html.CheckBox("QuestionSet.SystemReusable")%>
                <%= Html.ValidationMessage("QuestionSet.SystemReusable", "*")%>
            </p>
            <%} %>            
            <% if (Model.IsAdmin || Model.IsSchoolAdmin) {%>
            <p>
                <label for="CollegeReusable">CollegeReusable: (This will be restricted to your college)</label>
                <%= Html.CheckBox("QuestionSet.CollegeReusable")%>
                <%= Html.ValidationMessage("QuestionSet.CollegeReusable", "*")%>
                
                <%= this.Select("school").Options(Model.Schools, x=>x.Id,x=>x.LongDescription) %>
            </p>
            <%} %>            
            <% if (Model.IsAdmin || Model.IsSchoolAdmin) {%>
            <p>
                <label for="UserReusable">UserReusable:</label>
                <%= Html.CheckBox("QuestionSet.UserReusable")%>
                <%= Html.ValidationMessage("QuestionSet.UserReusable", "*")%>
            </p>
            <%} %>            
            </div>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
    
        <% if(Model.ItemType != null) { %>
            <%= Html.ActionLink<ApplicationManagementController>(a => a.EditItemType(Model.ItemType.Id), "Back to Item Type") %>
        <% } else if (Model.Item != null) { %> 
            <%= Url.EditItemUrl(Model.Item.Id, StaticValues.Tab_Questions) %>
        <% } else { %> 
            <%= Html.ActionLink<QuestionSetController>(a => a.List(), "Back to List") %>
        <% }%>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

