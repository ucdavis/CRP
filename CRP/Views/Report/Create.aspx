<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.CreateReportViewModel>" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="Resources"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script src="../../Scripts/jquery.CaesMutioptionControl.js" type="text/javascript"></script>
    <script src="../../Scripts/RenameForArray.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            // change the look of the checkboxes
            $("input[type='CheckBox']").CaesMutioptionControl();

            $("div.button").live("click", function() {
                CreateRow($(this).parent(), this);

                RenameControls($("div#selectedColumns"), "createReportParameters", "tr.dataRow");
            });
        });

        function CreateRow(span, button) {

            if ($(button).hasClass("selected")) {
                var tbody = $("div#selectedColumns").find("tbody");

                var tr = $("<tr>").addClass("dataRow").attr("id", $(span).find("input#_QuestionId").val());

                var cell1 = $("<td>");
                cell1.append($(span).find("input.indexedControl[type='hidden']").clone());

                tr.append(cell1);
                tr.append($("<td>").html($(span).find("label.indexedControl").html()));
                tr.append($("<td>").html($("<input>").attr("type", "textbox").attr("id", "_Format").attr("name", "_Format")));

                tbody.append(tr);
            }
            else {
                $("tr#" + $(span).find("input#_QuestionId").val()).remove();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Fields</legend>
            
            <p>
                <label for="Name">Name: </label>
                <%= Html.TextBox("Name") %>
            </p>
            
            
        </fieldset>

        <fieldset>
            <legend>Selected Columns</legend>
            
            <div id="selectedColumns" class="t-widget t-grid">
                <table cellspacing=0>
                    <thead>
                        <tr>
                            <td class="t-header"></td>
                            <td class="t-header">Field Name</td>
                            <td class="t-header">Format</td>
                        </tr>
                    </thead>
                    <tbody>
                        
                    </tbody>
                </table>
            </div>
            
        </fieldset>


        <fieldset class="indexedControlContainer">
            <legend>Transaction Level</legend>
            
            <% foreach(var qs in Model.Item.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
                    
                    <% foreach (var q in qs.QuestionSet.Questions) {%>
                        <span>
                        <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                        <label for="Selected" class="indexedControl"><%= Html.Encode(q.Name) %></label>
                        <%= Html.Hidden("_Quantity", true, new { @class = "indexedControl" })%>
                        <%= Html.Hidden("_QuestionId", q.Id, new { @class = "indexedControl" })%>
                        </span>
                    <% } %>
                    
            <% } %>
            
        </fieldset>

        <fieldset class="indexedControlContainer">
            <legend>Quantity Level</legend>
            
            <% foreach(var qs in Model.Item.QuestionSets.Where(a => a.QuantityLevel).OrderBy(a => a.Order)) {%>
                    
                    <% foreach (var q in qs.QuestionSet.Questions) {%>
                        <span>
                        <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                        <label for="Selected" class="indexedControl"><%= Html.Encode(q.Name) %></label>
                        <%= Html.Hidden("_Quantity", true, new { @class = "indexedControl" })%>
                        <%= Html.Hidden("_QuestionId", q.Id, new { @class = "indexedControl" })%>
                        </span>
                    <% } %>
                    
            <% } %>
            
        </fieldset>

        <fieldset class="indexedControlContainer">
            <legend>Properties</legend>
            
            <span>
                <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Donation Total") %></label>
                <%= Html.Hidden("_Property", true, new { @class = "indexedControl" })%>
                <%= Html.Hidden("_PropertyName", "DonationTotal", new { @class = "indexedControl" })%>
            </span>
            
            <span>
                <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Amount Total") %></label>
                <%= Html.Hidden("_Property", true, new { @class = "indexedControl" })%>
                <%= Html.Hidden("_PropertyName", "AmountTotal", new { @class = "indexedControl" })%>
            </span>
            
            <span>
                <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Total") %></label>
                <%= Html.Hidden("_Property", true, new { @class = "indexedControl" })%>
                <%= Html.Hidden("_PropertyName", "Total", new { @class = "indexedControl" })%>
            </span>
            
            <span>
                <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Payment Type") %></label>
                <%= Html.Hidden("_Property", true, new { @class = "indexedControl" })%>
                <%= Html.Hidden("_PropertyName", "PaymentType", new { @class = "indexedControl" })%>
            </span>
            
            <span>
                <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Quantity") %></label>
                <%= Html.Hidden("_Property", true, new { @class = "indexedControl" })%>
                <%= Html.Hidden("_PropertyName", "Quantity", new { @class = "indexedControl" })%>
            </span>                                                
            
            <span>
                <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Paid") %></label>
                <%= Html.Hidden("_Property", true, new { @class = "indexedControl" })%>
                <%= Html.Hidden("_PropertyName", "Paid", new { @class = "indexedControl" })%>
            </span>     
            
        </fieldset>

        <p>
            <input type="submit" value="Create" />
        </p>

    <% } %>

    <div>
        <%= Html.DetailItemUrl(Model.Item.Id, StaticValues.Tab_Reports) %>
    </div>

</asp:Content>



