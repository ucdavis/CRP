<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IQueryable<CRP.Core.Domain.ApplicationKey>>" %>
<%@ Import Namespace="CRP.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Application Keys
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Application Keys</h2>

    <p><%: Html.ActionLink<ApplicationKeyController>(a=>a.Create(), "Create New") %></p>

    <% Html.Grid(Model)
           .Name("Keys")
           .Columns(col=>
                        {
                            col.Add(a => { %>
                                         <%: Html.ActionLink<ApplicationKeyController>(b=>b.ToggleActive(a.Id), "Toggle Active") %>   
                                        <% });
                            col.Bound(a => a.Application);
                            col.Bound(a => a.Key);
                            col.Bound(a => a.IsActive);
                        })
           .Render();
            %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

