<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.MapPinViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
        <ul>
        <li>
            IsPrimary:
            <%= Html.Encode(Model.MapPin.IsPrimary) %>
        </li>
        <li>
            Latitude:
            <%= Html.Encode(Model.MapPin.Latitude)%>
        </li>
        <li>
            Longitude:
            <%= Html.Encode(Model.MapPin.Longitude)%>
        </li>
        <li>
            Title:
            <%= Html.Encode(Model.MapPin.Title)%>
        </li>
        <li>
            Description:
            <%= Html.Encode(Model.MapPin.Description)%>
        </li>
        <li>
            Id:
            <%= Html.Encode(Model.MapPin.Id)%>
        </li>
        </ul>
    </fieldset>
    <div>
       <%= Url.EditItemLink(Model.Item.Id, StaticValues.Tab_MapPins) %>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

