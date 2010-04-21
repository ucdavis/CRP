<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.QuestionSet>" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(Model.Name) %></h2>

    <fieldset>
        <legend>Fields</legend>
        <p>
            CollegeReusable:
            <%= Html.Encode(Model.CollegeReusable) %>
        </p>
        <p>
            SystemReusable:
            <%= Html.Encode(Model.SystemReusable) %>
        </p>
        <p>
            UserReusable:
            <%= Html.Encode(Model.UserReusable) %>
        </p>
        <p>
            IsActive:
            <%= Html.Encode(Model.IsActive) %>
        </p>
    </fieldset>
    
    <fieldset>
        <legend>Questions</legend>
        
        <%
            Html.Grid(Model.Questions)
                .Transactional()
                .Name("Questions")
                .PrefixUrlParameters(false)
                .Columns(col =>
                             {
                                 col.Add(x => x.Name).Title("Question");
                                 col.Add(x => x.QuestionType.Name).Title("Type");
                                 col.Add(x => string.Join(", ", x.Options.Select(b => b.Name).ToArray())).Title("Options");
                                 col.Add(x => x.Required);
                             }
                )
                .Render(); %>
        
    </fieldset>
    <p>
        <%= Html.ActionLink<QuestionSetController>(a => a.Edit(Model.Id), "Edit") %> |
        <%= Html.ActionLink<QuestionSetController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

