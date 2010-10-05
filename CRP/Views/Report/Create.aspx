<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.CreateReportViewModel>" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Core.Resources"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script src="<%= Url.Content("~/Scripts/jquery.CaesMutioptionControl.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/RenameForArray.js") %>" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            // change the look of the checkboxes
            $("input[type='CheckBox']").CaesMutioptionControl({ width: '700px' });

            $("div.button").live("click", function() {
                CreateRow($(this).parent(), this);

                RenameControls($("div#selectedColumns"), "createReportParameters", "tr.dataRow");
            });

            $("#toggle_all").click(function() {
                var containers = $(".indexedControlContainer");
                $.each(containers, function(index, item) {
                    var spans = $(item).find("span");
                    $.each(spans, function(index2, item2) {
                        var btn = $(item2).find("div.button");
                        if (btn.hasClass("selected")) { btn.removeClass("selected"); }
                        else { btn.addClass("selected"); }
                        CreateRow(item2, btn);
                        RenameControls($("div#selectedColumns"), "createReportParameters", "tr.dataRow");
                    });
                });
            });
        });

        function CreateRow(span, button) {
            if ($(button).hasClass("selected")) {
                var tbody = $("div#selectedColumns").find("tbody");

                // this only works against properties with real question id, not property
                var tr = $("<tr>").addClass("dataRow").attr("id", $(span).attr("id")); //.find("input#_QuestionId").val());

                var cell1 = $("<td>");
                cell1.append($(span).find("input.indexedControl[type='hidden']").clone());

                tr.append(cell1);
                tr.append($("<td>").html($(span).find("label.indexedControl").html()));
                tr.append($("<td>").html($("<input>").attr("type", "textbox").attr("id", "_Format").attr("name", "_Format")));

                tbody.append(tr);
            }
            else {
                // deal with properties
                if ($(span).hasClass("property")) {
                    $("tr#" + $(span).attr("id")).remove();
                }
                else {
                    $("tr#" + $(span).find("input#_QuestionId").val()).remove();
                }
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
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
                        <%  if (Model.ItemReport != null)
                            {
                                foreach (var irc in Model.ItemReport.Columns)
                                { %>
                            <tr id='<%= irc.Property ? irc.Name : irc.QuestionSet.Questions.Where(a=>a.Name == irc.Name).FirstOrDefault().Id.ToString() %>'>
                                <td>
                                    <% if (!irc.Property) { %>
                                            <%= Html.Hidden("_Quantity", irc.Quantity, new { @class = StaticValues.Class_indexedControl })%>
                                            <%= Html.Hidden("_Transaction", irc.Transaction, new { @class = StaticValues.Class_indexedControl })%>
                                            <%= Html.Hidden("_QuestionId", irc.QuestionSet != null ? irc.QuestionSet.Questions.Where(a => a.Name == irc.Name).FirstOrDefault().Id : -1, new { @class = StaticValues.Class_indexedControl })%>
                                            <%= Html.Hidden("_QuestionSetId", irc.QuestionSet != null ? irc.QuestionSet.Id : -1, new { @class = StaticValues.Class_indexedControl })%>
                                    <% } else { %>
                                            <%= Html.Hidden("_Property", irc.Property, new { @class= StaticValues.Class_indexedControl}) %>
                                            <%= Html.Hidden("_PropertyName", irc.Name, new { @class= StaticValues.Class_indexedControl}) %>                                            
                                    <% } %>
                                </td>
                                <td>
                                    <%= Html.Encode(irc.Name + " (" + (irc.QuestionSet != null ? irc.QuestionSet.Name : string.Empty) + ")")%>
                                </td>
                                <td>
                                    <%= Html.TextBox("_Format")%>
                                </td>
                            </tr>
                        <% }
                            } %>
                    </tbody>
                </table>
            </div>
            
        </fieldset>

        <div id="toggle_all">
            Toggle
        </div>

        <fieldset class="indexedControlContainer">
            <legend>Transaction Level</legend>
            
            <% foreach(var qs in Model.Item.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
                    
                    <% foreach (var q in qs.QuestionSet.Questions.Where(a => a.QuestionType != Model.QuestionTypeNoAnswer)) {%>
                        <span id='<%= q.Id %>'>
                        <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                        <label for="Selected" class="indexedControl"><%= Html.Encode(q.Name + " (" + q.QuestionSet.Name + ")" ) %></label>
                        <%= Html.Hidden("_Transaction", true, new { @class = StaticValues.Class_indexedControl })%>
                        <%= Html.Hidden("_QuestionId", q.Id, new { @class = StaticValues.Class_indexedControl })%>
                        <%= Html.Hidden("_QuestionSetId", q.QuestionSet.Id, new {@class = StaticValues.Class_indexedControl}) %>
                        </span>
                    <% } %>
                    
            <% } %>
            
        </fieldset>

        <fieldset class="indexedControlContainer">
            <legend>Quantity Level</legend>
            <% foreach(var qs in Model.Item.QuestionSets.Where(a => a.QuantityLevel).OrderBy(a => a.Order)) {%>                    
                    <% foreach (var q in qs.QuestionSet.Questions.Where(a => a.QuestionType != Model.QuestionTypeNoAnswer)) {%>
                        <span id='<%= q.Id %>'>
                        <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                        <label for="Selected" class="indexedControl"><%= Html.Encode(q.Name + " (" + q.QuestionSet.Name + ")")%></label>
                        <%= Html.Hidden("_Quantity", true, new { @class = StaticValues.Class_indexedControl })%>
                        <%= Html.Hidden("_QuestionId", q.Id, new { @class = StaticValues.Class_indexedControl })%>
                        <%= Html.Hidden("_QuestionSetId", q.QuestionSet.Id, new {@class = StaticValues.Class_indexedControl}) %>
                        </span>
                    <% } %>                    
            <% } %>
        </fieldset>

        <fieldset class="indexedControlContainer">
            <legend>Properties</legend>
            
            <span id="propertyTransactionNumber" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Transaction Number") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_TransactionNumber, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyTransactionDate" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Transaction Date") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_TransactionDate, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyActive" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Active")%></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_Active, new { @class = StaticValues.Class_indexedControl })%>                
            </span>  
            
            <span id="propertyDonationTotal" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Donation Total") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_DonationTotal, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyAmountTotal" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Amount Total") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_AmountTotal, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyTotal" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Total") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_Total, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyPaymentType" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Payment Type") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_PaymentType, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyQuantity" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Quantity") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_Quantity, new { @class = StaticValues.Class_indexedControl })%>                
            </span>                                                
            
            <span id="propertyPaid" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Paid") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_Paid, new { @class = StaticValues.Class_indexedControl })%>                
            </span>   
            
            <span id="propertyTotalPaid" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Total Paid")%></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_TotalPaid, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
            <span id="propertyRefundIssued" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Refund Issued")%></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_RefundIssued, new { @class = StaticValues.Class_indexedControl })%>                
            </span>  
            
            <span id="propertyRefundAmount" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%= Html.Encode("Refund Amount") %></label>
                <%= Html.Hidden("_Property", true, new { @class = StaticValues.Class_indexedControl })%>
                <%= Html.Hidden("_PropertyName", StaticValues.Report_RefundAmount, new { @class = StaticValues.Class_indexedControl })%>                
            </span>
            
        </fieldset>

        <p>
            <input type="submit" value="Create" />
        </p>

    <% } %>

    <div>
        <%= Url.DetailItemLink(Model.Item.Id, StaticValues.Tab_Reports) %>
    </div>

</asp:Content>



