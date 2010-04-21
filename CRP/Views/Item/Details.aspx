<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemDetailViewModel>" %>
<%@ Import Namespace="Resources"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.View_PageHeader, Model.DisplayProfile ?? new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(Model.Item.Name)%></h2>

    <% Html.RenderPartial("~/Views/Shared/TagView.ascx", Model.Item.Tags); %>

    <%= Model.Item.Expiration < DateTime.Now ? Html.Encode("This item is now expired.") : string.Empty %>

    <fieldset>
        <legend>Details</legend>
        
        <span style="float:right;">
            <img src='<%= Url.Action("GetImage", "Item", new {id = Model.Item.Id}) %>' />
        </span>
        
        <p>
            <strong>CostPerItem:</strong>
            <%= Html.Encode(String.Format("{0:C}", Model.Item.CostPerItem))%>
        </p>
        <p>
            <strong>Expiration:</strong>
            <%= Html.Encode(String.Format("{0:d}", Model.Item.Expiration))%>
        </p>

        <% foreach (var ep in Model.Item.ExtendedPropertyAnswers)
           {%>
        
            <p>
                <strong><%= Html.Encode(ep.ExtendedProperty.Name) %>:</strong>
                <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                            ? ep.Answer : String.Format("{0:d}", ep.Answer)) %>
            </p>
        
        <% } %>
        
        <% if (Model.Item.Expiration > DateTime.Now) { %>
        <p>
        <a href='<%= Url.Action("Checkout", "Transaction", new {id=Model.Item.Id} ) %>'><img src="../../Images/register.png" style="border:0;" /></a>
        </p>
        <% } %>
        
        <p><%= Model.Item.Description%></p>
        
        <p>
            <strong>Link:</strong>
            <%= Html.Encode(Model.Item.Link)%>
        </p>
        
        <iframe width="425" height="350" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=wickson+hall,+uc+davis&amp;sll=38.541755,-121.750102&amp;sspn=0.004196,0.009645&amp;ie=UTF8&amp;hq=wickson+hall,&amp;hnear=University+of+California&amp;ll=38.54,-121.75&amp;spn=0.020946,0.038418&amp;output=embed"></iframe><br /><small><a href="http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=wickson+hall,+uc+davis&amp;sll=38.541755,-121.750102&amp;sspn=0.004196,0.009645&amp;ie=UTF8&amp;hq=wickson+hall,&amp;hnear=University+of+California&amp;ll=38.54,-121.75&amp;spn=0.020946,0.038418" style="color:#0000FF;text-align:left">View Larger Map</a></small>
        
    </fieldset>
    <p>
        <%= Html.ActionLink<ItemController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

