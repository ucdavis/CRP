<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ReportViewModel>" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ViewReport
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(Model.ReportName) %></h2>

    <p>
        <%= Html.DetailItemUrl(Model.ItemId, StaticValues.Tab_Reports) %>
        
        
    </p>

    <div id="Report" class="t-widget t-grid">
        <table cellpadding="0">
            <thead>
                <tr>
                <% foreach(var ch in Model.ColumnNames) { %>
                    <td class="t-header"><%= Html.Encode(ch) %></td>
                <% } %>
                </tr>
            </thead>
            <tbody>
                <% foreach(var row in Model.RowValues) { %>
                    <tr>
                    <% foreach(var cell in row) { %> 
                        <td><%= Html.Encode(cell) %></td>
                    <% } %>
                    </tr>
                <% } %>
                <tr></tr>
            </tbody>
        </table>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
</asp:Content>
