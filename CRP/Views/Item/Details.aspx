<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Core.Domain.Item>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(Model.Name) %></h2>

    <fieldset>
        <legend>Details</legend>
        
        <span style="float:right;">
            <img src='<%= Url.Action("GetImage", "Item", new {id = Model.Id}) %>' />
        </span>
        
        <p>
            <strong>CostPerItem:</strong>
            <%= Html.Encode(String.Format("{0:C}", Model.CostPerItem)) %>
        </p>
        <p>
            <strong>Quantity:</strong>
            <%= Html.Encode(Model.Quantity - Model.Sold) %>
        </p>
        <p>
            <strong>Expiration:</strong>
            <%= Html.Encode(String.Format("{0:d}", Model.Expiration)) %>
        </p>

        <% foreach(var ep in Model.ExtendedPropertyAnswers) {%>
        
            <p>
                <strong><%= Html.Encode(ep.ExtendedProperty.Name) %>:</strong>
                <%= Html.Encode(ep.ExtendedProperty.QuestionType.Name == "Text Box"
                                            ? ep.Answer : String.Format("{0:d}", ep.Answer)) %>
            </p>
        
        <% } %>
        
        <p>
        <a href='<%= Url.Action("Register", "Transaction", new {id=Model.Id} ) %>'><img src="../../Images/register.png" style="border:0;" /></a>
        </p>
        
        <p><%= Model.Description %></p>
        
        <p>
            <strong>Link:</strong>
            <%= Html.Encode(Model.Link) %>
        </p>
        
    </fieldset>
    <p>
        <%= Html.ActionLink<ItemController>(a => a.List(), "Back to List") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

