<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionSetLinkViewModel>" %>
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

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.Hidden("transaction", Model.Transaction) %>
    <%= Html.Hidden("quantity", Model.Quantity) %>

    <p>
        <%= Html.ActionLink<QuestionSetController>(a => a.Create(Model.ItemId, null), "Create New Question Set")%>
    </p>

    <% Html.Grid(Model.QuestionSets)
           .Transactional()
           .Name("QuestionSets")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Add(x =>
                                        {%>
                                            <% using (Html.BeginForm<QuestionSetController>(a => a.LinkToItem(x.Id, Model.ItemId, Model.Transaction, Model.Quantity), FormMethod.Post))
                                               {%>
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="SelectLinkToForm">Select</a>
                                            <%} %>
                                        <%});
                            col.Add(x => x.Name);
                            col.Add(x => x.SystemReusable);
                            col.Add(x => x.CollegeReusable);
                            col.Add(x => x.UserReusable);
                            col.Add(x => x.Questions.Count).Title("# of Questions");
                        })
            .Pageable()
            .Sortable()
            .Render(); %>

    <p>
    <%= Html.ActionLink<ItemManagementController>(a => a.Edit(Model.ItemId), "Back to Item")%>
    </p>


</asp:Content>

