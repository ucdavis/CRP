<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.TouchnetFID>>" %>
<%@ Import Namespace="CRP.Core.Resources" %>
<%@ Import Namespace="CRP.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Touchnet FID
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Touchnet FID</h2>
    <p>
        <%= Html.ActionLink<FIDController>(a => a.Create(), "Create New") %>
    </p>
    <% using (Html.BeginForm()) { %>
                <% Html.Grid(Model)
                   .Transactional()
                   .Name("FID Values")
                   .PrefixUrlParameters(false)                                   
                   .Columns(col =>
                                {
                                    col.Template(x =>
                                    { %>
                                        <%= Html.ActionLink<FIDController>(a => a.Details(x.Id),"View")%>  |                                 
                                        <%=Html.ActionLink<FIDController>(a => a.Edit(x.Id), "Edit")%>        
                                    <% });                                    
                                    col.Bound(x => x.FID).Title("FID");
                                    col.Bound(x => x.Description).Title("Description");
                                })                  
                    .Pageable(x=>x.PageSize(20))
                    .Sortable()
                    .Render(); %>
    <%} %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
<% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>
