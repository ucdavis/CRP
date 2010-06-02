<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.QuestionSet>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
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
                .CellAction(cell =>
                {
                    switch (cell.Column.Member)
                    {
                        case "Options":
                            cell.Text = string.Join(", ", cell.DataItem.Options.Select(b => b.Name).ToArray());
                            break;
                        case "Validators":
                            cell.Text = string.Join(", ", cell.DataItem.Validators.Select(b => b.Name).ToArray());
                            break;
                    }
                })
                .Columns(col =>
                             {
                                 col.Bound(x => x.Name).Title("Question");
                                 col.Bound(x => x.QuestionType.Name).Title("Type");
                                 col.Bound(x => x.Options).Title("Options");
                                 col.Bound(x => x.Validators).Title("Validators");
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

