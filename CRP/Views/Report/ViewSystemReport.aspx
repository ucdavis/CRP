<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.SystemReportViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ViewSystemReport
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>View System Report</h2>

    <select id="reportId" name="reportId">
        <option value="">--Select a Report--</option>    
        <% for(var i = 0; i < Model.Reports.Length; i++) { %>
            <option value="<%= Html.Encode(i) %>"><%= Html.Encode(Model.Reports.GetValue(i)) %></option>    
        <% } %>
    </select>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    
</asp:Content>
