<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>
<%@ Import Namespace="CRP.Controllers.ViewModels"%>

<%@ Import Namespace="CRP.Core.Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, Model.DisplayProfile ?? new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <%= Html.ValidationSummary("Checkout was unsuccessful. Please correct the errors and try again.") %>
    
    <% using (Html.BeginForm("Checkout", "Transaction", FormMethod.Post, new {@id = "CheckoutForm"})) { %>
    
    <%= Html.AntiForgeryToken() %>
    <%= Html.Hidden("displayAmount", Model.Item.CostPerItem)%>
    <%= Html.Hidden("referenceIdHidden", Model.ReferenceId)%>
    
    <!-- t-widget t-grid -->   
    <div id="Items" class="t-widget t-grid">
        <table cellspacing="0">
            <thead>
                <tr>
                <!-- t-header -->
                    <td class="t-header">Qty.</td>
                    <td class="t-header">Item</td>
                    <td class="t-header">Price per <%= !String.IsNullOrEmpty(Model.Item.QuantityName) ? Html.Encode(Model.Item.QuantityName) : Html.Encode(ScreenText.STR_QuantityName) %></td>
                    <td class="t-header">Total Price</td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><%= Html.TextBox("quantity", Model.Quantity, new {@style = "width:20px;", @class="quantityAmount"}) %></td>
                    <td><%= Html.Encode(Model.Item.Name) %></td>
                    <td>$ <span class="perItemAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.Item.CostPerItem)) %></span></td>
                    <td>$ <span class="totalItemAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.Item.CostPerItem * Model.Quantity)) %></span></td>                    
                </tr>  
                <%if (Model.Item.HideDonation)
                  {%>
                <tr style="background-color: #D5D5D5">
                <%}
                else{%>              
                <tr>
                <%}%>
                    <td colspan="4">&nbsp</td>
                </tr>
                <%if (Model.Item.HideDonation)
                  {%>
                <tr style="background-color: #D5D5D5;display:none">
                <%}
                else{%>              
                <tr style="background-color: #D5D5D5">
                <%}%>                
                    <td></td>
                    <td></td>
                    <td style="text-align:right;">Tax-Deductible Contribution:</td>
                    <td>$ <%= Html.TextBox("donation", "0.00", new {@style="width:40px;", @class="donationAmount"}) %></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <label for="Coupon">Coupon Code:</label>
                        <%= Html.TextBox("Coupon", !string.IsNullOrWhiteSpace(Model.Coupon) ? Model.Coupon : string.Empty) %>
                        <img src='<%= Url.Content("~/Images/ajax-loader.gif") %>' id="CouponValidateImage" style="display:none;" />
                        <span id="CouponMessage"></span>
                        
                    </td>
                    <td>$ <span class="discounterPerItemAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.CouponAmountToDisplay))%></span>
                    
                        <span class="discounterMaxQuantity" style="display:none;"></span>
                    </td>
                    <td>$ <span class="discountAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.CouponTotalDiscountToDisplay))%></span></td>
                </tr>
                <tr style="background-color: #D5D5D5">
                    <td></td>
                    <td colspan="2" style="text-align:right;">Total Amount: </td>
                    <td>$ <span class="totalAmount"><%= Html.Encode(string.Format("{0:0.00}", Model.TotalAmountToRedisplay))%></span></td>                    
                </tr>

            </tbody>
            <tfoot>
                <tr>
                    <td colspan="4" style="text-align:right;">
                        <span id = "paymentTypeSpan">
                        <label for="paymentType">Payment Method: </label>
                        <%if (Model.Item.AllowCreditPayment){%>
                            <input type="radio" id="paymentType" name="paymentType" class="required allowBypass" value="<%=StaticValues.CreditCard%>" "<%=Model.CreditPayment ? "checked" : string.Empty%>" /><label for="credit">Credit Card</label>
                        <%}%>
                        <%if (Model.Item.AllowCheckPayment){%>
                            <input type="radio" id="paymentType" name="paymentType" class="required allowBypass" value="<%= StaticValues.Check %>" "<%= Model.CheckPayment ? "checked" : string.Empty %>" /><label for="check">Check</label>
                        <%}%>
                        </span>
                    </td>
                </tr>
                <% if (!String.IsNullOrEmpty(Model.Item.RestrictedKey)) { %>
                <tr>
                    <td colspan="4" style="background-color: #D5D5D5">
                        <label for="restrictedKey">Passphrase to purchase: </label>
                        <%= Html.TextBox("restrictedKey", !string.IsNullOrWhiteSpace(Model.Password) ? Model.Password : string.Empty)%>
                    </td>
                </tr>
                <% } %>
            </tfoot>
        </table>
    </div>
          
    <div id="paymentTypeContainer">
        <p>
        
        </p>
    </div>
    
    <% Html.RenderPartial(StaticValues.Partial_TransactionForm, new ItemTransactionViewModel(Model.Item, Model.OpenIdUser, Model.Quantity, Model.Answers, Model.ReferenceId)); %>

    <p>
        <%= Html.GenerateCaptcha() %>
    </p>

    <p>
        <input type="submit" value="Submit" />
    </p>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript" src="<%= Url.Content("~/Scripts/RenameForArray.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.PhoneValidator.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.ZipValidator.js") %>"></script>
    <script type="text/javascript">
        var class_quantityAmount = "quantityAmount";
        var class_perItemAmount = "perItemAmount";
        var class_totalItemAmount = "totalItemAmount";
        var class_donationAmount = "donationAmount";
        var class_discounterPerItemAmount = "discounterPerItemAmount";
        var class_discounterMaxQuantity = "discounterMaxQuantity";
        var class_discountAmount = "discountAmount";
        var class_totalAmount = "totalAmount";
        var questionSet_Count = '<%= Model.Item.QuestionSets.Where(a => a.QuantityLevel).Count() %>';
    </script>

    <script type="text/javascript">

    
        $(document).ready(function() {
            // do some client side validation on the dynamic fields
            $("form#CheckoutForm").validate({
                errorElement:"span",            // set the tag for the item that contains the message
                errorClass:"failed",            // set the class on that tag of the notification tag
                success:function(label){        // function to execute on passing
                    label.addClass("passed");   // add the passed class
                }
            });

            // validate the controls on blur
            $("input").blur(function(){
                $("form#CheckoutForm").validate().element(this);
            });
    
            $("input.email").blur(function(event){
                if($(this).hasClass("warning")){
                    $(this).removeClass("warning");
                }
                if($(this).hasClass("failed")){
                    //Do nothing
                }
                else{
                    var emailVal = $(this).val().toLowerCase();                
                    //if(emailVal != null && emailVal != "" && emailVal.match(/^[a-z0-9._%+-]+@[a-z0-9.-]+\.(?:[a-z]{2}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)$/) == null){
                    if(emailVal != null && emailVal != "" && emailVal.match(<%=StaticValues.EmailWarningRegEx %>) == null){
                        //alert("no match " + emailVal);
                        $(this).addClass("warning");
                        $(this).next().text("This may be invalid");
                    }
                }
                    
            });

           
            

            $("input#quantity").blur(function(event) {
                var quantity = $(this).val();
                if (isNaN(quantity) || quantity <= 0) { alert("Please enter a valid number."); return false; }
                else {
                    var existingContainers = $("div.QuantityContainer");
                    quantity = parseInt(quantity);

                    var counter = existingContainers.length;

                    // deal with the situation where we have too many of the containers
                    if (existingContainers.length > quantity) {
                        do {
                            $(existingContainers[(counter-1)]).remove();

                            counter--;
                        } while (counter > quantity);
                    }
                    // deal with the situation where we don't have enough containers
                    else if (existingContainers.length > 0 && existingContainers.length < quantity) {
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
                // initialize the question names
                InitializeQuestions();
                
                
                if(questionSet_Count > 0 && $("div.QuantityContainer").length != quantity) { alert("Error on page. Please reselect quantity."); return false;}
                    
            });

            // update the total when a donation is entered
            $("input." + class_donationAmount).blur(function(event) {
                if (isNaN($(this).val())) {
                    alert("Please enter a valid number.");
                    return;
                }
                if($(this).val() < 0) {
                    $(this).val(0);
                }

                CalculateTotal();
            });

            if ($("input#Coupon").val() != "") {
                processCoupon();
            }

            // update the coupon values and validate the coupon
            $("input#Coupon").blur(function(event) {
                processCoupon(event);
            });
            
            
            // initialize the question names
            InitializeQuestions();
            RepopulateRadioButtonAnswers();
        });

        function processCoupon(event) {
            var url = '<%= Url.Action("Validate", "Coupon") %>';//<%= Html.Encode(Model.Item.Id) %>';
            var couponCode = $("input#Coupon").val();
            var quantity = $("#quantity").val();
            
            $("img#CouponValidateImage").show();

            $.getJSON(url, { itemId: <%= Html.Encode(Model.Item.Id) %>, couponCode: couponCode, quantity: quantity }, function(result) {
                var message = result.message;

                // if the message is undefined, we have a valid coupon

                var discountAmount = result.discountAmount;
                var maxQuantity = result.maxQuantity;

                $("span." + class_discounterPerItemAmount).html(parseFloat(discountAmount).toFixed(2));
                $("span." + class_discounterMaxQuantity).html(parseFloat(maxQuantity).toFixed(2));

                $("span#CouponMessage").html("Coupon accepted.");

                CalculateTotal();

                // display error message
                $("span#CouponMessage").html(message);

                // hide the loading image
                $("img#CouponValidateImage").hide();
            });                        
        }

        function CalculateTotal() {
            // get the item price, quantity and discount per item prices
            var costPerItem = parseFloat($("span." + class_perItemAmount).html());
            var quantity = parseInt($("input." + class_quantityAmount).val());
            var discountAmountPerItem = parseFloat($("span." + class_discounterPerItemAmount).html());
            var maxQuantity = parseInt($("span." + class_discounterMaxQuantity).html());

            var donationAmount = parseFloat($("input." + class_donationAmount).val());

            // calculate the totals
            var itemTotal = quantity * costPerItem;          
            var discountTotal = 0;
            if (maxQuantity == -1 || maxQuantity > quantity)
            {
                discountTotal = quantity * discountAmountPerItem;
            }
            else if (maxQuantity <= quantity)
            {
                discountTotal = maxQuantity * discountAmountPerItem;
            }
            
            var total = itemTotal - discountTotal + donationAmount;

            $("#displayAmount").val(total.toFixed(2));

            // update the prices
            $("span." + class_totalItemAmount).html(itemTotal.toFixed(2));
            $("span." + class_discountAmount).html(discountTotal.toFixed(2));
            $("span." + class_totalAmount).html(total.toFixed(2));
            if(total == 0)
            {            
                $(".allowBypass").removeClass("required");
                $("#paymentTypeSpan").hide();
            }
            else
            {
                $("#paymentTypeSpan").show();
                $(".allowBypass").addClass("required");  
            }
        }

        function GenerateQuantityQuestionSet() {
            var $container = $($("div.QuantityContainer")[0]);
            $container.find("input.dateForm").removeClass("hasDatepicker");
            
            $container.after($container.clone());
            RenameControls($("div.QuantityContainer"), "quantityAnswers", "li");
            addQuantityIndex($("div.QuantityContainer"));
        }

        function InitializeQuestions() {
            RenameControls($("div#TransactionContainer"), "transactionAnswers", "li");
            RenameControls($("div.QuantityContainer"), "quantityAnswers", "li");
            addQuantityIndex($("div.QuantityContainer"));
            $.each($("input.dateForm"), function(index, item){    
                //$(item).watermark("mm/dd/yyyy", { className: "watermark" });
                $(item).attr("title", "");  
                $(item).bt('mm/dd/yyyy format');  
                if(!$(item).hasClass("hasDatepicker")){
                    $(item).datepicker();
                }
            });
            $.each($("input.date"), function(index, item){
                //$(item).watermark("mm/dd/yyyy", { className: "watermark" });
                $(item).attr("title", ""); 
                $(item).bt('mm/dd/yyyy format');
            });
            $.each($("input.url"), function(index, item){
                //$(item).watermark("Example: http://www.ucdavis.edu/index.html", { className: "watermark" });
                $(item).attr("title", ""); 
                $(item).bt('You need the http:// or https:// at the start for a valid URL. For example: http://www.ucdavis.edu/index.html');
            });
            $.each($("input.phoneUS"), function(index, item){
                //$(item).watermark("(###) ###-####", { className: "watermark" });
                $(item).attr("title", ""); 
                $(item).bt('(###) ###-#### format');
            });          
//            $("input.dateForm").filter().datepicker();
        }
        
        function RepopulateRadioButtonAnswers(){
            $.each($("input[type=radio][checked=checked]"), function(index, item){
                if($(item).hasClass("indexedControl")){
                    $(item).attr("checked", true);
                }
            });    
        }

        function addQuantityIndex($container)
        {
            var masterIndex = 0;
            var cName = "quantityAnswers";
            
            $.each($container, function(cIndex, cItem){
                var p = $(cItem).find("li");
                
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
    </script>

    <script type="text/javascript">
        $(document).ready(function() {
            $("input#quantity").focus();
        });
    </script>

        <!-- Should a header color have been specified, then throw it in -->
    <% if (Model.DisplayProfile != null && !string.IsNullOrEmpty(Model.DisplayProfile.CustomCss)) { %>
        <style type="text/css">
            <%= Model.DisplayProfile.CustomCss %>
        </style>
    <% } %>
</asp:Content>
