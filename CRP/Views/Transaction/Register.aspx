<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.Item>" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial("~/Views/Shared/PageHeader.ascx", new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Register</h2>
    <%= Html.Encode(Model.Name) %>
    <div id="priceContainer">
        #
        <%= !String.IsNullOrEmpty(Model.QuantityName) ? Html.Encode(Model.QuantityName) : Html.Encode(ScreenText.STR_QuantityName) %>:
        <input type="text" id="Quantity" value="1" style="width: 20px" />
        x
        <%= Html.Encode(Model.CostPerItem.ToString("C")) %>
    </div>
    <% Html.RenderPartial("~/Views/Shared/TransactionForm.ascx"); %>
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

                    debugger;

                    // deal with the situation where we have too many of the containers
                    if (existingContainers.length > quantity) {
                    }
                    // deal with the situation where we don't have enough containers
                    else if (existingContainers.length < quantity) {
                    }
                    // we have an equal amount of containers to quantity
                    else {
                    }
                }
            });
        });

        function GenerateQuantityQuestionSet() {
        }
    </script>

</asp:Content>
