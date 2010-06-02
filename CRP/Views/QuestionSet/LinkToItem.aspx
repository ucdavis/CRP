<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetLinkViewModel>" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Link To Item
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
    $(document).ready(function() {
        $("a.SelectLinkToForm").click(function(event) { $(this).parents("form").submit(); });
    });
    </script>
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.Hidden("transaction", Model.Transaction) %>
    <%= Html.Hidden("quantity", Model.Quantity) %>

    <p>
        <%= Html.ActionLink<QuestionSetController>(a => a.Create(Model.ItemId, null, Model.Transaction, Model.Quantity), "Create New Question Set")%>
    </p>

    <% Html.Grid(Model.QuestionSets)
           .Transactional()
           .Name("QuestionSets")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Template(x =>
                                        {%>
                                            <% using (Html.BeginForm<QuestionSetController>(a => a.LinkToItem(x.Id, Model.ItemId, Model.Transaction, Model.Quantity), FormMethod.Post))
                                               {%>
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="SelectLinkToForm">Select</a>
                                            <%} %>
                                        <%});
                            col.Bound(x => x.Name);
                            col.Bound(x => x.SystemReusable);
                            col.Bound(x => x.CollegeReusable);
                            col.Bound(x => x.UserReusable);
                            col.Bound(x => x.Questions.Count).Title("# of Questions");
                        })
            .Pageable()
            .Sortable()
            .Render(); %>

    <p>       
        <%= Url.EditItemLink(Model.ItemId, StaticValues.Tab_Questions) %>
    </p>


</asp:Content>

