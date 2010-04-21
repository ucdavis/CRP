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
                            col.Add(x =>
                                        {%>
                                            <%= Html.ActionLink<ApplicationManagementController>(a => a.EditItemType(x.Id), "Edit") %> |
                                            
                                            <% using(Html.BeginForm<ApplicationManagementController>(a => a.ToggleActive(x.Id))) {%>
                                            
                                                <%= Html.AntiForgeryToken() %>
                                                <a href="javascript:;" class="FormSubmit"><%= x.IsActive ? "Deactivate" : "Activate" %></a>
                                            
                                            <%} %>
                                            
                                        <%});
                            col.Add(x => x.Name);
                            col.Add(x => x.IsActive);
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

