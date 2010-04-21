<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>

<%@ Import Namespace="CRP.Core.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, Model.DisplayProfile); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()) { %>
    
    <%= Html.AntiForgeryToken() %>
       
    <div id="Items" class="t-widget t-grid">
        <table cellspacing="0">
            <thead>
                <tr>
                    <td class="t-header">Qty.</td>
                    <td class="t-header">Item</td>
                    <td class="t-header">Price per <%= !String.IsNullOrEmpty(Model.Item.QuantityName) ? Html.Encode(Model.Item.QuantityName) : Html.Encode(ScreenText.STR_QuantityName) %></td>
                    <td class="t-header">Total Price</td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><%= Html.TextBox("quantity", 1, new {@style = "width:20px;", @class="quantityAmount"}) %></td>
                    <td><%= Html.Encode(Model.Item.Name) %></td>
                    <td>$ <span class="perItemAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.Item.CostPerItem)) %></span></td>
                    <td>$ <span class="totalItemAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.Item.CostPerItem)) %></span></td>
                </tr>
                <tr>
                    <td colspan="4">&nbsp</td>
                </tr>
                <tr style="background-color: #D5D5D5">
                    <td></td>
                    <td>Donation</td>
                    <td></td>
                    <td>$ <%= Html.TextBox("donation", "0.00", new {@style="width:40px;", @class="donationAmount"}) %></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <label for="Coupon">Coupon Code:</label>
                        <%= Html.TextBox("Coupon") %>
                    </td>
                    <td>$ <span class="discounterPerItemAmount">0.00</span></td>
                    <td>$ <span class="discountAmount">0.00</span></td>
                </tr>
                <tr style="background-color: #D5D5D5">
                    <td></td>
                    <td colspan="2" style="text-align:right;">Total Amount: </td>
                    <td>$ <span class="totalAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.Item.CostPerItem)) %></span></td>
                </tr>

            </tbody>
            <tfoot>
                <tr>
                    <td colspan="4">
                        <label for="paymentType">Payment Method: </label>
                        <input type="radio" id="paymentType" name="paymentType" value="<%= StaticValues.CreditCard %>" /><label for="credit">Credit Card</label>
                        <input type="radio" id="paymentType" name="paymentType" value="<%= StaticValues.Check %>" /><label for="check">Check</label>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
          
    <div id="paymentTypeContainer">
        <p>
        
        </p>
    </div>
    
    <% Html.RenderPartial("~/Views/Shared/TransactionForm.ascx", Model.Item); %>

    <p>
        <input type="submit" value="Submit" />
    </p>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript" src="../../Scripts/RenameForArray.js"></script>
    <script type="text/javascript">
        var class_quantityAmount = "quantityAmount";
        var class_perItemAmount = "perItemAmount";
        var class_totalItemAmount = "totalItemAmount";
        var class_donationAmount = "donationAmount";
        var class_discounterPerItemAmount = "discounterPerItemAmount";
        var class_discountAmount = "discountAmount";
        var class_totalAmount = "totalAmount";
    </script>

    <script type="text/javascript">

    
        $(document).ready(function() {
            $("input.dateForm").datepicker();

            $("input#quantity").blur(function(event) {
                var quantity = $(this).val();

                if (isNaN(quantity)) { alert("Please enter a valid number."); return false; }
                else {
                    var existingContainers = $("div.QuantityContainer");
                    quantity = parseInt(quantity);

                    var counter = existingContainers.length;

                    // deal with the situation where we have too many of the containers
                    if (existingContainers.length > quantity) {
                        do {
                            $(existingContainers[counter]).remove();

                            counter--;
                        } while (counter >= quantity);
                    }
                    // deal with the situation where we don't have enough containers
                    else if (existingContainers.length < quantity) {
                        do {
                            GenerateQuantityQuestionSet();
                            counter++;
                        } while (counter < quantity);

                        // get the containers again, and rename the header in them
                        var $container = $("div.QuantityContainer");
                        $.each($container, function(index, item) {
                            $(item).find("span.quantityIndex").html(index + 1);
                        });
                    }
                }

                CalculateTotal();
            });

            // update the total when a donation is entered
            $("input." + class_donationAmount).blur(function(event) {
                if (isNaN($(this).val())) {
                    alert("Please enter a valid number.");
                    return;
                }

                CalculateTotal();
            });

            // update the coupon values and validate the coupon
            $("input#Coupon").blur(function(event) {
                var url = '<%= Url.Action("Validate", "Coupon") %>';//<%= Html.Encode(Model.Item.Id) %>';
                var couponCode = $("input#Coupon").val();
                $.getJSON(url, {itemId: <%= Html.Encode(Model.Item.Id) %>, couponCode: couponCode}, function(result) { 
                    if (isNaN(result))
                    {
                        alert("invalid code");
                    } 
                    else{
                        $("span." + class_discounterPerItemAmount).html(parseFloat(result).toFixed(2));
                        CalculateTotal();
                    }
                });
            });


            // initialize the question names
            InitializeQuestions();
        });

        function CalculateTotal() {
            // get the item price, quantity and discount per item prices
            var costPerItem = parseFloat($("span." + class_perItemAmount).html());
            var quantity = parseInt($("input." + class_quantityAmount).val());
            var discountAmountPerItem = parseFloat($("span." + class_discounterPerItemAmount).html());

            var donationAmount = parseFloat($("input." + class_donationAmount).val());

            // calculate the totals
            var itemTotal = quantity * costPerItem;
            var discountTotal = quantity * discountAmountPerItem;
            var total = itemTotal - discountTotal + donationAmount;

            // update the prices
            $("span." + class_totalItemAmount).html(itemTotal.toFixed(2));
            $("span." + class_discountAmount).html(discountTotal.toFixed(2));
            $("span." + class_totalAmount).html(total.toFixed(2));
        }

        function GenerateQuantityQuestionSet() {
            var $container = $($("div.QuantityContainer")[0]);
            
            $container.after($container.clone());
            RenameControls($("div.QuantityContainer"), "quantityAnswers", "p");
            addQuantityIndex($("div.QuantityContainer"));
        }

        function InitializeQuestions() {
            RenameControls($("div#TransactionContainer"), "transactionAnswers", "p");
            RenameControls($("div.QuantityContainer"), "quantityAnswers", "p");
            addQuantityIndex($("div.QuantityContainer"));
        }

        function addQuantityIndex($container)
        {
            var masterIndex = 0;
            var cName = "quantityAnswers";
            
            $.each($container, function(cIndex, cItem){
                var p = $(cItem).find("p");
                
                $.each(p, function(index, item){               
                    // quantity index doesn't exist, create it
                    // search for input ending with "_QuantityIndex"
                    if ($(item).find("input[id$='_QuantityIndex']").length == 0) {
                        //if ($(item).find("input#" + cName + "_QuantityIndex").length == 0) {
                        $(item).append($("<input>").attr("type", "hidden").attr("id", cName + "_QuantityIndex").attr("name", cName + ".QuantityIndex").addClass("indexedControl").val(cIndex));
                    }
                    else {
                        $(item).find("input[id$='_QuantityIndex']").val(cIndex);
                    }
                });
            });
        }

//        function RenameControls($container, isQuantity) {
//            var name;
//            if (isQuantity) {name = "quantityAnswers"; } else {name = "transactionAnswers"; }

//            var masterIndex = 0;
//        
//            // go through each container passed
//            $.each($container, function(cIndex, cItem) {
//                // get the paragraph tags that each contain a question
//                var p = $(cItem).find("p");

//                // iterate through the paragraphs
//                $.each(p, function(index, item) {
//                    // construct the new name
//                    var cName = name + "[" + masterIndex + "]";

//                    // get the actual controls
//                    //var tControls = $.merge($(item).find("input"), $(item).find("select"));

//                    var tControls = $(item).find(".QA");

//                    // iterate through each control inside each paragraph
//                    $.each(tControls, function(index2, item2) {
//                        // pull the last part of the name out
//                        var charIndex = ($(item2).attr("id")).indexOf("_");
//                        var nameEnd = ($(item2).attr("id")).substring(charIndex + 1);

//                        $(item2).attr("id", cName + "_" + nameEnd);
//                        $(item2).attr("name", cName + "." + nameEnd);
//                    });

//                    // check for the quantity index id
//                    if (isQuantity) {
//                        // quantity index doesn't exist, create it
//                        // search for input ending with "_QuantityIndex"
//                        if ($(item).find("input[id$='_QuantityIndex']").length == 0) {
//                            //if ($(item).find("input#" + cName + "_QuantityIndex").length == 0) {
//                            $(item).append($("<input>").attr("type", "hidden").attr("id", cName + "_QuantityIndex").attr("name", cName + ".QuantityIndex").addClass("QA").val(cIndex));
//                        }
//                        else {
//                            $(item).find("input[id$='_QuantityIndex']").val(cIndex);
//                        }
//                    }
//                    masterIndex++;
//                }); // end of paragraph     
//            });                    // end of container
//        }
    </script>

</asp:Content>
