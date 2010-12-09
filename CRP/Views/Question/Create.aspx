<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.QuestionViewModel>" %>
<%@ Import Namespace="CRP.Core.Resources"%>
<%@ Import Namespace="CRP.Controllers"%>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="pageHeader" ContentPlaceHolderID="PageHeader" runat="server">
    <% Html.RenderPartial(StaticValues.Partial_PageHeader, new DisplayProfile()); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ClientSideValidation<Question>("") %>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>

        <span style="display:none;" id="TypesWithOptions"><%= string.Join(",", Model.QuestionTypes.Where(a => a.HasOptions).Select(a => a.Id.ToString()).ToArray()) %></span>

        <fieldset>
            <legend>Fields</legend>
            
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name", Model.Question != null ? Model.Question.Name : string.Empty, new { style = "width: 700px" })%>
            </p>
            
            <p>
                <label for="Validators">Validators:</label>
                
                <%= this.CheckBoxList("Validators").Options(Model.Validators, x=>x.Id, x=>x.Name) %>
                
            </p>
            
            <p>
                <%= this.Select("QuestionType").Options(Model.QuestionTypes, x=>x.Id, x=>x.Name).FirstOption("--Select a Type--").Label("Question Type:") %>
            </p>
            
            <p id="Option" style="display:none;">
                <span id="OptionsContainer">
                </span>
                <img id="AddOptions" src="<%= Url.Content("~/Images/plus.png") %>" style="width:24px; height:24px;" />
            </p>
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>


    <div>
        <%= Html.ActionLink<QuestionSetController>(a => a.Edit(Model.QuestionSet.Id), "Back to Question Set") %>
    </div>
    <div class="two_col_float two_col_float_left">
        <div class="QuantityContainer">
            <fieldset>
                <legend>Sample</legend>
                <ul>                
                    <li id="SampleTextBox" style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a TextBox Question?")%>
                        <br />
                        <%= Html.TextBox("TextBoxQuestion", "") %>
                    </li>
                    <li id="SampleTextArea"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a TextArea Question?")%>
                        <br />
                        <%= Html.TextArea(".Answer", "") %>
                    </li>
                    <li id="SampleBoolean"style="display:none;" class="hideAndShow">
                        <%= Html.CheckBox(".Answer", true, new {@class="BooleanSample"})%> Sample of a Boolean Question?
                    </li>
                    <li id="SampleRadioButtons"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a Radio Button Question?")%>
                        <br />
                        <%= Html.RadioButton(".Answer", "Option 1", false)%>
                        <%= Html.Encode("Red")%>
                        <%= Html.RadioButton(".Answer", "Option 2", true)%>
                        <%= Html.Encode("Blue")%>
                        <%= Html.RadioButton(".Answer", "Option 3", false)%>
                        <%= Html.Encode("Green")%>
                        <%= Html.RadioButton(".Answer", "Option 4", false)%>
                        <%= Html.Encode("Not any color above but one that cause this to wrap on next line")%>
                    </li>
                    <li id="SampleCheckboxList"style="display:none;" class="hideAndShow">    
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a Checkbox List Question?")%>
                        <br />                              
                        <%= Html.CheckBox(".CblAnswer", false)%>
                        <%= Html.Encode("Checkbox List 1") %>
                        <%= Html.CheckBox(".CblAnswer", true)%>
                        <%= Html.Encode("Checkbox List 2") %>
                        <%= Html.CheckBox(".CblAnswer", true)%>
                        <%= Html.Encode("Checkbox List 3") %>
                        <%= Html.CheckBox(".CblAnswer", false)%>
                        <%= Html.Encode("Checkbox List 4") %>
                        <%= Html.CheckBox(".CblAnswer", true)%>
                        <%= Html.Encode("Checkbox List 5") %>
                    </li>
                    <li id="SampleDropDown"style="display:none;" class="hideAndShow">
                        <% var dropDownPick = new Dictionary<int, string>(3) { { 1, "DropDown 1" }, { 2, "DropDown 2" }, { 3, "DropDown 3" } };%>
                         
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a Dropdown List Question?")%>
                        <br />   
                        <%= this.Select("Test").Options(dropDownPick, x=>x.Key, x=>x.Value).FirstOption("--Select a Type--") %>                         
                    </li>
                    <li id="SampleDate"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a Date Question?")%>
                        <br />  
                        <%= Html.TextBox(".Answer", string.Empty, new { @class = "dateForm"})%>
                    </li>
                    <li id="SampleNoAnswer"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null ? Model.Question.Name : "Sample of a No Answer Question? This can cover several lines up to a maximum of about 200 characters.")%>
                        <br />  
                    </li>
                </ul>    
            </fieldset>
        </div>
    </div>
    <br /><br /><br /><br /><br /><br />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            // attach event handlers
            $("img#AddOptions").click(function() { AddOptionInput(); });
            $("select#QuestionType").change(function(event) { QuestionTypeChange(this); });
        });
        
        function AddOptionInput() {
            var index = $("span#OptionsContainer").children().length;

            var input = $("<input>");
            var name = "questionOptions[" + index + "]";
            input.attr("id", name);
            input.attr("name", name);

            $("span#OptionsContainer").append(input);
        }

        function QuestionTypeChange(obj) {
            var selectedId = $(obj).find("option:selected").val();
            var typesWithOptions = $("span#TypesWithOptions").html().split(",");

            if ($.inArray(selectedId, typesWithOptions) >= 0) {
                // it's in the array, it has options
                $("p#Option").show();
                $("span#OptionsContainer").empty();
            }
            else {
                $("p#Option").hide();
            }
            $(".hideAndShow").hide();
            var myText = $(obj).find("option:selected").text().replace(" ", "");
            $("li#Sample" + myText).show();

        }
    </script>

</asp:Content>

