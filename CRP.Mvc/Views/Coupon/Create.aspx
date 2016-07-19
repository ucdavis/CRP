<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.CouponViewModel>" %>
<%@ Import Namespace="CRP.Controllers.Helpers"%>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Coupon for <%= Html.Encode(Model.Item.Name) %></h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>
    
    <% using(Html.BeginForm()) { %>
    
        <%= Html.AntiForgeryToken() %>
    

    
        <fieldset>
            <legend>Fields</legend>
            
            <p>Coupon Type:</p>
            <%= Html.ValidationMessage("CouponType") %>
            <ul>
                <li><input type="radio" name="couponType" class="couponType" value="Unlimited" />Unlimited</li>
                <li><input type="radio" name="couponType" class="couponType" value="LimitedUsage" />Limited Usage
                
                    <span id="maxUsageContainer" style="display:inline-block; display:none; margin-left: 2em;">
                        <label>Maximum # of times this coupon can be used: </label>
                        <%= Html.TextBox("MaxUsage", Model.Coupon != null ? Model.Coupon.MaxUsage.ToString() : string.Empty) %> 
                        <%= Html.ValidationMessageFor(x => x.Coupon.MaxUsage) %>   
                    </span>

                </li>
                <li><input type="radio" name="couponType" class="couponType" value="SingleUsage" />Single Usage</li>
            </ul>

<%--            <p>
                <label for="Unlimited">Unlimited Usage: </label>
                <%= Html.CheckBox("Unlimited", Model.Coupon != null ? Model.Coupon.Unlimited : false) %>
                <%= Html.ValidationMessage("Unlimited") %>
            </p>  --%>

            <p>
                <label for="Expiration">Expiration Date: </label>
                <%= Html.TextBox("Expiration", Model.Coupon == null ? string.Empty : (Model.Coupon.Expiration.HasValue ? Model.Coupon.Expiration.Value.ToString("D") : string.Empty)) %>
                <%= Html.ValidationMessage("Expiration") %>
            </p>
            <p>
                <label for="Email">E-Mail (Optional):</label>
                <%= Html.TextBox("Email", Model.Coupon != null ? Model.Coupon.Email : string.Empty) %>
                <%= Html.ValidationMessageFor(x => x.Coupon.Email) %>
            </p>
            <p>
                <label for="DiscountAmount">Discount Amount:</label>
                <%= Html.TextBox("DiscountAmount", Model.Coupon != null ? string.Format("{0:0.00}", Model.Coupon.DiscountAmount) : string.Empty) %>
                <%= Html.ValidationMessageFor(x => x.Coupon.DiscountAmount) %>
            </p>
            
            <p>
                <label for="MaxQuantity">Maximum Quantity per Transaction (Optional):</label>
                <%= Html.TextBox("MaxQuantity", Model.Coupon != null ? Model.Coupon.MaxQuantity.ToString() : string.Empty) %>
            </p>

<%--            <p>
                <label for="MaxUsage">Maximum Usage:</label>
                
            </p>--%>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>
    <% } %>
    
    <%= Url.EditItemLink(Model.Item.Id, StaticValues.Tab_Coupons) %>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("input#Expiration").datepicker();

            $(".couponType").click(function () {
                if ($(this).val() == "LimitedUsage") {
                    $("#maxUsageContainer").show();
                }
                else {
                    $("#MaxUsage").val(0);
                    $("#maxUsageContainer").hide(); 
                }
            });
        });
    </script>

    <style type="text/css">
        label  
        {
            display: inline-block;
            min-width: 200px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>
