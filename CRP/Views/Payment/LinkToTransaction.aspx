<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.LinkPaymentViewModel>" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Core.Resources"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	LinkToTransaction
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <style type="text/css">
        .deactivated
        {
            background-color:Red;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function() {
            $("input.date").datepicker();

            var fieldsets = $("fieldset.check");
            $.each(fieldsets, function(index, item) {
                var controls = $(item).find("input");
                $.each(controls, function(index2, item2) {
                    $(item2).attr("id", "Checks[" + index + "]_" + $(item2).attr("id"));
                    $(item2).attr("name", "Checks[" + index + "]." + $(item2).attr("name"));
                });

                var controls2 = $(item).find("textarea");
                $.each(controls2, function(index2, item2) {
                    $(item2).attr("id", "Checks[" + index + "]_" + $(item2).attr("id"));
                    $(item2).attr("name", "Checks[" + index + "]." + $(item2).attr("name"));
                });
            });

            $("img#addCheck").click(function(event) {
                var fieldset = $($("fieldset.check")[0]);
                var cloned = fieldset.clone();

                cloned.find("input").val("");
                cloned.find("input.accepted-field").val(true);

                fieldset.after(cloned);
                RenameControls();

                cloned.find("input.date").datepicker();
            });

            $("input.amount").blur(function(event) { RecalculateTotal(); });

            $("img.deactivate-check").click(function(event) {

                var $fieldset = $(this).parent("fieldset");

                if ($fieldset.hasClass("deactivated")) {
                    $fieldset.removeClass("deactivated");
                    $fieldset.find("input.accepted-field").val(true);
                }
                else {
                    $fieldset.addClass("deactivated");
                    $fieldset.find("input.accepted-field").val(false);
                }
            });
        });

        function RenameControls() {
            var fieldsets = $("fieldset.check");

            $.each(fieldsets, function(index, item) {
                $(item).find("span.checkIndex").html(index + 1);

                var controls = $(item).find("input");
                $.each(controls, function(index2, item2) {
                    RenameControl(index, item2);
                });

                var controls2 = $(item).find("textarea");
                $.each(controls2, function(index2, item2) {
                    RenameControl(index, item2);
                });

                $(item).find("input.amount").blur(function(event) { RecalculateTotal(); });
            });
        }

        function RenameControl(index, obj) {
            // pull the last part of the name out
            var charIndex = ($(obj).attr("id")).indexOf("_");
            var nameEnd = ($(obj).attr("id")).substring(charIndex + 1);

            $(obj).attr("id", "Checks[" + index + "]_" + nameEnd);
            $(obj).attr("name", "Checks[" + index + "]." + nameEnd);
        }

        function RecalculateTotal() {
            var sum = 0;
            var total = parseFloat($("input#TotalAmount").val());
            var donationTotal = parseFloat($("input#DonationAmount").val());
            
            $.each($("input.amount"), function(index, item) {
                sum += parseFloat($(item).val());
            });

            var remaining = (total + donationTotal) - sum;

            if (remaining < 0) {
                $("div#message").html("Amount paid is more than transaction total, remaining value will be applied to a donation.");
                $("span#remainingTotal").css("color", "red");
            }
            else {
                $("div#message").html("");
                $("span#remainingTotal").css("color", "");
            }

            $("span#paymentTotal").html(sum.toFixed(2));
            $("span#remainingTotal").html(remaining.toFixed(2));
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <p>
        <%= Html.DetailItemUrl(Model.Transaction.Item.Id, StaticValues.Tab_Checks) %>
    </p>
    
    <% using(Html.BeginForm()){ %>

    <%= Html.AntiForgeryToken() %>

    <table id="Container" style="width:100%">
        <tr>
            <td style="width:40%">
                <div id="TransactionDetails">
                    <p>
                        Transaction Number: <%= Html.Encode(Model.Transaction.TransactionNumber) %>
                    </p>
                    <p>
                        Transaction Date:
                        <%= Html.Encode(Model.Transaction.TransactionDate) %>
                    </p>    
                    <p>
                        Amount:
                        <%= Html.Encode(Model.Transaction.AmountTotal.ToString("C"))%>
                        <%= Html.Hidden("TotalAmount", Model.Transaction.AmountTotal) %>
                    </p>
                    <p>
                        Donation Amount:
                        <%= Html.Encode(Model.Transaction.DonationTotal.ToString("C")) %>
                        <%= Html.Hidden("DonationAmount", Model.Transaction.DonationTotal) %>
                    </p>
                </div>
            </td>
            <td style="width:60%">
                <% for (var i = 0; i < Model.PaymentLogs.Count(); i++ ) { %> 
                    <fieldset class="check">
                        <legend>Check <span class="checkIndex"><%= Html.Encode(i + 1) %></span></legend>
                        <% Html.RenderPartial(StaticValues.Partial_CheckView, Model.PaymentLogs.ToList()[i]); %>
                    </fieldset>
                <% } %>
            
<%--                <fieldset class="check">
                    <legend>Check <span class="checkIndex"><%= Html.Encode(Model.PaymentLogs.Count() + 1)%></span></legend>
                    <% Html.RenderPartial("~/Views/Shared/CheckView.ascx", new PaymentLog(){Accepted = true}); %>
                </fieldset>--%>
                
                <img id="addCheck" src="../../Images/plus.png" style="width:24px; height:24px" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                Transaction Total: $ <span id="transactionTotal"><%= Html.Encode(string.Format("{0:0.00}",Model.Transaction.Total))%></span>
                <br />
                Payment Total: $ <span id="paymentTotal"><%= Html.Encode(string.Format("{0:0.00}", Model.PaymentLogs.Sum(a => a.Amount)))%></span>
                <br />
                Remaining Balance: $ <span id="remainingTotal">
                    <%= Html.Encode(string.Format("{0:0.00}",
                        Model.Transaction.Total
                        -
                        Model.PaymentLogs.Sum(a => a.Amount)                                       
                        )) %>
                </span>
                
                <div id="message"></div>
                
                <div id="saveContainer" style="float:right;">
                    <input type="submit" value="save" />
                </div>
            </td>
        </tr>
    </table>
    
    <% } %>
</asp:Content>


