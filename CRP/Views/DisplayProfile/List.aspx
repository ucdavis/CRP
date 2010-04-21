<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.DisplayProfile>>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ListDisplayProfiles
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial("~/Views/Shared/PageHeader.ascx", new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>ListDisplayProfiles</h2>

    <p>
        <%= Html.ActionLink<DisplayProfileController>(a => a.Create(), "Create") %>
    </p>
    
    <p>
        <%= Html.ActionLink<DisplayProfileController>(a => a.Edit(Model.Where(b=>b.SiteMaster).FirstOrDefault().Id), "Edit Site Master") %>
    </p>
    
    <p>
        Department display profiles:
    </p>
    <% Html.Grid(Model.Where(a => !a.SiteMaster && !a.SchoolMaster))
           .Transactional()
           .Name("DisplayProfiles")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Add(a =>
                                        {%>
                                            <%= Html.ActionLink<DisplayProfileController>(b=>b.Edit(a.Id), "Edit") %>
                                        <%});
                            col.Add(a=>a.Name);
                            col.Add(a => a.Unit.FullName).Title("Unit");
                            col.Add(a => a.Unit.School.LongDescription).Title("College");
                        }) 
            .Pageable()
            .Sortable()
            .Render();
        %>

    <p>
        College Display Profiles
    </p>
    <% Html.Grid(Model.Where(a => a.SchoolMaster))
           .Transactional()
           .Name("DisplayProfiles")
           .PrefixUrlParameters(false)
           .Columns(col =>
                        {
                            col.Add(a =>
                                    {%>
                                        <%= Html.ActionLink<DisplayProfileController>(b=>b.Edit(a.Id), "Edit") %>
                                    <%});
                            col.Add(a=>a.Name);
                            col.Add(a => a.School.LongDescription);
                        }) 
            .Pageable()
            .Sortable()
            .Render();
        %>
        
        
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

