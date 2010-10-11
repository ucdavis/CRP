<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Core.Domain.TouchnetFID>" %>

    <%= Html.AntiForgeryToken() %>
    <%= Html.ClientSideValidation<TouchnetFID>("TouchnetFID")%>

    <fieldset>
        <legend>Fields</legend>
        <ul>
        <li>
            <label for="FID">FID:</label> <br />
            <%= Html.TextBox("FID",Model != null? Model.FID:string.Empty, new { style = "width: 50px" }) %>
            <%= Html.ValidationMessage("TouchnetFID.FID")%>
        </li>
        <li>
            <label for="Description">Description:</label><br />
            <%= Html.TextBox("Description", Model != null ? Model.Description : string.Empty, new { style = "width: 500px" })%>
            <%= Html.ValidationMessage("TouchnetFID.Description")%>
        </li>
        </ul>
    </fieldset>