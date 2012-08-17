<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ItemTransactionViewModel>" %>
<%@ Import Namespace="CRP.Controllers.ViewModels"%>
<%@ Import Namespace="CRP.Core.Resources"%>

<div class="two_col_float">
<%--<h2 class="two_col_float_for_h2">Transaction Level</h2>--%>

<div id="TransactionContainer">
    
<% var showDonationLink = false; %>

<% foreach(var qs in Model.Item.QuestionSets.Where(a => a.TransactionLevel).OrderBy(a => a.Order)) {%>
    <fieldset id='<%= Html.Encode(qs.Id) %>'>
        <%
            var questionSetName = qs.QuestionSet.Name;
            if(questionSetName == "Contact Information")
            {
                questionSetName = questionSetName + " / Billing Information";
                showDonationLink = true;
            }
            else
            {
                showDonationLink = false;
            }
        %>
        <legend><%= Html.Encode(questionSetName) %></legend>
        
        <ul>
        <% foreach (var q in qs.QuestionSet.Questions) {
                var answer = Model.Answers.Where(a => a.Transaction && a.QuestionSetId == q.QuestionSet.Id && a.QuestionId == q.Id).FirstOrDefault();
                var disable = false;
               
                if (q.Name == "Reference Id" && !string.IsNullOrEmpty(Model.ReferenceId))
                {
                    disable = true;
                    answer = new ItemTransactionAnswer() {Answer = Model.ReferenceId};
                }
               
               %>
        
            <% Html.RenderPartial(StaticValues.Partial_QuestionForm, new ItemQuestionViewModel(q, Model.OpenIDUser, answer != null ? answer.Answer : string.Empty, disable)); %>
        
        <% } %></ul>
        
    </fieldset>
    <% if (showDonationLink && !string.IsNullOrWhiteSpace(Model.Item.DonationLinkLink))
       {%>
        <fieldset id="DonationLink">
            <legend><%= Html.Encode(!string.IsNullOrWhiteSpace(Model.Item.DonationLinkLegend) ? Model.Item.DonationLinkLegend : "Donation Information") %></legend>
            <ul>
                <li>
                    <%= Html.Encode(Model.Item.DonationLinkInformation) %>
                </li>
            <li>
                <a href="<%= Html.Encode(Model.Item.DonationLinkLink)%>" target="_blank"><%= Html.Encode(!string.IsNullOrWhiteSpace(Model.Item.DonationLinkText) ? Model.Item.DonationLinkText : "Click Here")%></a>
            </li>
            </ul>
        </fieldset>           
       
    <%} %>
<% } %>

</div>
</div>

<%if(Model.Item.QuestionSets.Where(a => a.QuantityLevel).Count() >= 1) {%>
<div class="two_col_float two_col_float_right">
<%--<h2>Quantity Level</h2>
<p>
    *The following questions must be answered for each of the <%= !String.IsNullOrEmpty(Model.Item.QuantityName) ? Html.Encode(Model.Item.QuantityName) : Html.Encode(ScreenText.STR_QuantityName)%>
</p>--%>


    <% for (var i = 0; i < Model.Quantity; i++ ) { %>
    <div class="QuantityContainer">
    <fieldset>
    <legend><%= Html.Encode(Model.Item.QuantityName)%> <span class="quantityIndex"><%= Html.Encode(i + 1) %></span> </legend>
        <%var questionSetCount = Model.Item.QuestionSets.Where(a => a.QuantityLevel).Count(); %>
        <% foreach (var qs in Model.Item.QuestionSets.Where(a => a.QuantityLevel).OrderBy(a => a.Order))
           { %> 
                <%if (questionSetCount > 1){%>
                <fieldset>
                <legend><%=Html.Encode(qs.QuestionSet.Name)%> <span class="quantityIndex"><%=Html.Encode(i + 1)%></span> </legend>
                <%}%>
                <ul>
                <% foreach (var q in qs.QuestionSet.Questions) {
                        int i1 = i;
                        var answer = Model.Answers.Where(a => !a.Transaction && a.QuestionSetId == q.QuestionSet.Id && a.QuestionId == q.Id && a.QuantityIndex == i1).FirstOrDefault();
                        var disable = false;

                        if (q.Name == "Reference Id" && !string.IsNullOrEmpty(Model.ReferenceId))
                        {
                            disable = true;
                            answer = new ItemTransactionAnswer() { Answer = Model.ReferenceId };
                        }
                   %>
                
                    <% Html.RenderPartial(StaticValues.Partial_QuestionForm, new ItemQuestionViewModel(q, Model.OpenIDUser, answer != null ? answer.Answer : string.Empty, disable)); %>
                
                <% } %>
            </ul>    
            <%if (questionSetCount > 1){%>
            </fieldset>
            <%}%>


        <% } %> <!-- End of foreach loop -->
    </fieldset>
    </div>
    <% } %> <!-- End of for loop -->
</div>
<%}%>
<div style="clear:both;">&nbsp</div>