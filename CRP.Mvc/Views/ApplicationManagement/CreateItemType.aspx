<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemTypeViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>

<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	CreateItemType
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">

        $(document).ready(function() {
            $("img#addExtendedProperty").click(function() { AddExtendedProperty(); });
        });

        function AddExtendedProperty() {
        
            var count = $("div#ExtendedProperties").children().length;
            var div = $("<div>");
            div.append($("<label>").html("Property Name:"));
            div.append($("<input>").attr("id", "ExtendedProperties[" + count + "]_Name").attr("name", "ExtendedProperties[" + count + "].Name"));
            div.append($("span#QuestionTypeBase").clone().css("display", "block").attr("id", "ExtendedProperties[" + count + "]_QuestionType"));
            div.find("select").attr("id", "ExtendedProperties[" + count + "]_QuestionType").attr("name", "ExtendedProperties[" + count + "].QuestionType");

            $("div#ExtendedProperties").append(div);
        }
    </script>

</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>CreateItemType</h2>

        <%= Html.ClientSideValidation<ItemType>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend>Name</legend>
            <p>
                <%= Html.TextBox("ItemType.Name") %>
                <%= Html.ValidationMessage("ItemType.Name", "*") %>
            </p>
        </fieldset>
        <fieldset>
            <legend>Extended Properties</legend>
                       
            <!-- This is the container for the extended properties -->
            <div id="ExtendedProperties">
            
                <%  if (Model.ItemType != null)
                    {
                        //foreach (var prop in Model.ItemType.ExtendedProperties)
                        for (var i = 0; i < Model.ItemType.ExtendedProperties.Count; i++ )
                        {
                            var prop = Model.ItemType.ExtendedProperties.ElementAt(i);
                            %>
                            <div>
                           <label>Property Name:</label>
                           <input id='<%= Html.Encode("ExtendedProperties[" + i + "]_Name") %>' name='<%= Html.Encode("ExtendedProperties[" + i + "].Name") %>' value='<%= Html.Encode(prop.Name) %>' />
                           <span id='<%= Html.Encode("ExtendedProperties[" + i + "]_QuestionType") %>' style="display:block">
                               <label>Question Type:</label>
                               <select id='<%= Html.Encode("ExtendedProperties[" + i + "]_QuestionType") %>' name='<%= Html.Encode("ExtendedProperties[" + i + "].QuestionType") %>' >
                                    <option value="">Select a Question Type</option>
                                    <% foreach(var o in Model.QuestionTypes) { %>
                                        <option value='<%= Html.Encode(o.Id) %>'><%= Html.Encode(o.Name) %></option>
                                    <% } %>
                               </select>
                           </span>
                           </div>
                           <%--<%= this.Select("QuestionType").Options(Model.QuestionTypes, x => x.Id, x => x.Name).Selected(prop.QuestionType.Id).Label("Question Type: ")%>               --%>
                    <% }
                    } %>
            
            </div>
            
            <img id="addExtendedProperty" src="<%= Url.Content("~/Images/plus.png") %>" style="width:24px; height:24px" />
        </fieldset>
        
            <span id="QuestionTypeBase" style="display:none;">
            <%= this.Select("QuestionTypeBase").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("Select a Question Type").Label("Question Type: ") %>
            </span>
            
            <p>
                <input type="submit" value="Create" />
            </p>

    <% } %>

    <div>
        <%= Html.ActionLink<ApplicationManagementController>(a => a.ListItemTypes(), "Back to List") %>
    </div>

</asp:Content>


