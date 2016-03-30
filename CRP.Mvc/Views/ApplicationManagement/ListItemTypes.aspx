<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.ItemType>>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ListItemTypes
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Item Types</h2>

    <p>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.CreateItemType(), "Create") %>
    </p>

    <% Html.Grid(Model)
           .Transactional()
           .Name("Items")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Template(x =>
                                        {%>
                                            <%= Html.ActionLink<ApplicationManagementController>(a => a.EditItemType(x.Id), "Edit") %> |
                                            
                                            <% using(Html.BeginForm<ApplicationManagementController>(a => a.ToggleActive(x.Id), FormMethod.Post, new {@class = "inline"})) {%>
                                            
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="FormSubmit"><%= x.IsActive ? "Deactivate" : "Activate" %></a>
                                            
                                            <%} %>
                                            
                                        <%});
                            col.Bound(x => x.Name);
                            col.Bound(x => x.IsActive);
                        })
            .Pageable()
            .Sortable()
            .Render(); %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("a.FormSubmit").click(function() { $(this).parents("form").submit(); });
        });
    </script>
</asp:Content>
