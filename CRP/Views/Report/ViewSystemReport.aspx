<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.SystemReportViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ViewSystemReport
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("select#reportId").change(function(event) {
                // redirect to this page with the extra parameter
                window.location = '<%= Url.Action("ViewSystemReport") %>' + '?reportId=' + $(this).val();
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>View System Report</h2>

    <p>
        <select id="reportId" name="reportId">
            <option value="">--Select a Report--</option>    
            <% for(var i = 0; i < Model.Reports.Length; i++) { %>
                <option value="<%= Html.Encode(i) %>" '<%= Model.SelectedReport.HasValue && Model.SelectedReport == i ? "selected" : string.Empty %>'><%= Html.Encode(Model.Reports.GetValue(i)) %></option>    
            <% } %>
        </select>
    </p> 
    
    <!-- Hide this report stuff because no report is selected -->
    <% if (Model.SelectedReport.HasValue) {%>
        <p>
            <img src='<%= Url.Action("GenerateChart", "Report", new {reportId = Model.SelectedReport.HasValue ? Model.SelectedReport : 0}) %>'
        </p>
        
        <% Html.Grid(Model.SystemReportData)
               .Name("ReportData")
               .CellAction(cell =>
                {
                    switch (cell.Column.Member)
                    {
                        case "Value":
                            cell.Text = cell.DataItem.Value.ToString(cell.DataItem.ValueFormat);
                            break;
                    }
                })
               .Columns(col =>
                            {
                                col.Bound(a => a.Name);
                                col.Bound(a => a.Value);
                            })
                   
               .Render(); %>
    <% } %>
</asp:Content>


