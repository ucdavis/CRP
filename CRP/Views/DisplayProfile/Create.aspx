<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.DisplayProfileViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	CreateDisplayProfile
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>CreateDisplayProfile</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm("Create", "DisplayProfile", FormMethod.Post, new {@enctype="multipart/form-data"})) {%>

        <%= Html.AntiForgeryToken() %>
        <%= Html.ClientSideValidation<DisplayProfile>("") %>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name") %>
                <%= Html.ValidationMessage("Name", "*") %>
            </p>
            <p>
                <%= this.Select("Unit").Options(Model.Units, x => x.Id, x => x.FullName).FirstOption("--Select a Department--")
                        .Selected(Model.DisplayProfile != null && Model.DisplayProfile.Unit != null ? Model.DisplayProfile.Unit.Id : 0)
                        .Label("Department: ") %>  
            </p>
            <p>
                <%= this.Select("School").Options(Model.Schools, x => x.Id, x=>x.LongDescription)
                    .FirstOption("--Select a School--")
                    .Selected(Model.DisplayProfile != null && Model.DisplayProfile.School != null ? Model.DisplayProfile.School.Id.ToString() : "0")
                    .Label("School:")
                                    %>
            </p>
            <p>
                <label for="file">Logo:</label>
                <input type="file" name="file" id="file" />
            </p>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%= Html.ActionLink<DisplayProfileController>(a => a.List(), "Back to List") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

