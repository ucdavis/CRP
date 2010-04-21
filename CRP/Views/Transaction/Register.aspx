<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial("~/Views/Shared/PageHeader.ascx", Model.DisplayProfile); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()) { %>
    
    <h2><%= Html.Encode(Model.Item.Name) %></h2>
    
    <div id="priceContainer">
        #
        <%= !String.IsNullOrEmpty(Model.Item.QuantityName) ? Html.Encode(Model.Item.QuantityName) : Html.Encode(ScreenText.STR_QuantityName) %>:
        <input type="text" id="Quantity" value="1" style="width: 20px" />
        x
        <%= Html.Encode(Model.Item.CostPerItem.ToString("C")) %>
    </div>
    <% Html.RenderPartial("~/Views/Shared/TransactionForm.ascx", Model.Item); %>

    <p>
        <input type="submit" value="Submit" />
    </p>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $("input.dateForm").datepicker();

            $("input#Quantity").blur(function(event) {
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
                    // we have an equal amount of containers to quantity
                    else {
                    }
                }
            });

            InitializeQuestions();
        });

        function GenerateQuantityQuestionSet() {
            var $container = $($("div.QuantityContainer")[0]);
            
            $container.after($container.clone());
            RenameControls($("div.QuantityContainer"), true);
        }

        function InitializeQuestions() {
            RenameControls($("div#TransactionContainer"), false);
            RenameControls($("div.QuantityContainer"), true);
        }

        function RenameControls($container, isQuantity) {
            var name;
            if (isQuantity) {name = "quantityAnswers"; } else {name = "transactionAnswers"; }
        
            // go through each container passed
            $.each($container, function(cIndex, cItem) {
                // get the paragraph tags that each contain a question
                var p = $(cItem).find("p");

                // iterate through the paragraphs
                $.each(p, function(index, item) {
                    // construct the new name
                    var cName = name + "[" + index + "]";

                    // get the actual controls
                    var tControls = $.merge($(item).find("input"), $(item).find("select"));

                    // iterate through each control inside each paragraph
                    $.each(tControls, function(index2, item2) {
                        // pull the last part of the name out
                        var charIndex = ($(item2).attr("id")).indexOf("_");
                        var nameEnd = ($(item2).attr("id")).substring(charIndex + 1);

                        $(item2).attr("id", cName + "_" + nameEnd);
                        $(item2).attr("name", cName + "." + nameEnd);
                    });

                    // check for the quantity index id
                    if (isQuantity) {
                        // quantity index doesn't exist, create it
                        // search for input ending with "_QuantityIndex"
                        if ($(item).find("input[id$='_QuantityIndex']").length == 0) {
                            //if ($(item).find("input#" + cName + "_QuantityIndex").length == 0) {
                            $(item).append($("<input>").attr("type", "hidden").attr("id", cName + "_QuantityIndex").attr("name", cName + ".QuantityIndex").val(cIndex));
                        }
                        else {
                            $(item).find("input[id$='_QuantityIndex']").val(cIndex);
                        }
                    }
                }); // end of paragraph           
            });    // end of container
        }
    </script>

</asp:Content>
