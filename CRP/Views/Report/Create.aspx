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
            // rename the controls for input as an array
            RenameControls($("fieldset.indexedControlContainer"), "createReportParameters");
            // change the look of the checkboxes
            $("input[type='CheckBox']").CaesMutioptionControl();
        });
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

        <fieldset class="indexedControlContainer">
            <legend>Transaction Level</legend>
            
            <% foreach(var qs in Model.Item.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
                    
                    <% foreach (var q in qs.QuestionSet.Questions) {%>
                        <span>
                        <%= Html.CheckBox("_Selected", new { @class = "indexedControl" })%>  
                        <label for="Selected" class="indexedControl"><%= Html.Encode(q.Name) %></label>
                        <%= Html.Hidden("_Quantity", true, new { @class = "indexedControl" })%>
                        <%= Html.Hidden("_QuestionId", q.Id, new { @class = "indexedControl" })%>
                        <%= Html.Hidden("_QuestionSetId", qs.Id, new { @class = "indexedControl" })%>
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
                        <%= Html.Hidden("_QuestionSetId", qs.Id, new { @class = "indexedControl" })%>
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



