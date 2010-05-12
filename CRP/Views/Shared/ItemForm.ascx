<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Controllers.ViewModels.ItemViewModel>" %>

    <script type="text/javascript">
        $(document).ready(function() {
            //$("#Item_Link").watermark("Example: http://www.ucdavis.edu/index.html", { className: 'watermark' });
            $("#Item_Link").bt('You need the http:// or https:// at the start for a valid URL. For example: http://www.ucdavis.edu/index.html');
            //$("#Item_Expiration").watermark("mm/dd/yyyy", { className: "watermark" });
            $("#Item_Expiration").bt('mm/dd/yyyy format');
            $.each($("input.isDate"), function(index, item) {
                //$(item).watermark("mm/dd/yyyy", { className: "watermark" });
                $(item).bt('mm/dd/yyyy format');
                $(item).datepicker();
            });
        });
    </script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.UrlValidator.js") %>"></script>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <%= Html.ClientSideValidation<Item>("Item") %>

        <%= Html.AntiForgeryToken() %>
        <fieldset>
            <legend>Fields</legend>
            <ul>
            <li><label for="ItemType">Item Type:</label><br />
            <% if (Model.Item == null) {%>
            
                <%= this.Select("Item.ItemType").Options(Model.ItemTypes,x=>x.Id, x=>x.Name).FirstOption("--Select an Item Type--")
                        .Selected(Model.Item != null ? Model.Item.ItemType.Id : 0) 
                        
                    %>
            
            <%} else {%>
                
                    
                    <%= Html.Encode(Model.Item.ItemType.Name) %>
                
            <% } %>
            </li>
            
            <li>
                <label for="Item.Name">Name:</label><br />
                <%= Html.TextBox("Item.Name") %>
                <%= Html.ValidationMessage("Item.Name", "*")%>
            </li>
            <li>
            <label for="Item.Unit">Unit:</label><br />
                <%= this.Select("Item.Unit").Options(Model.Units, x=>x.Id, x=>x.FullName)
                        .Selected(Model.Item != null ? Model.Item.Unit.Id : 0)%>
            </li>
            <li>
                <label for="Item.Description">Description:</label><br />
                <%= Html.TextArea("Item.Description")%>
                <%= Html.ValidationMessage("Item.Description", "*")%> 
            </li>
            <li>
                <label for="Item.CheckPaymentInstructions">Check Payment Instructions:</label><br />
                <%= Html.TextArea("Item.CheckPaymentInstructions", Model.Item == null || Model.Item.CheckPaymentInstructions == null ? Html.HtmlEncode("<h1>Thank you for your purchase!</h1> <h2>Please mail your payment to:</h2> <address>College of Agricultural and Environmental Sciences<br />150 Mrak Hall<br />One Shields Ave.<br />Davis, CA 95616<br /></address>") : Html.HtmlEncode(Model.Item.CheckPaymentInstructions))%>
                <%= Html.ValidationMessage("Item.CheckPaymentInstructions", "*")%> 
            </li>
            <li>
                <label for="Item.CostPerItem">CostPerItem:</label><br />
                <%= Html.TextBox("Item.CostPerItem", Model.Item != null ? string.Format("{0:0.00}", Model.Item.CostPerItem) : string.Empty)  %>
                <%= Html.ValidationMessage("Item.CostPerItem", "*")%>
            </li>
            <li>
                <label for="Item.Quantity">Quantity:</label><br />
                <%= Html.TextBox("Item.Quantity") %>
                <%= Html.ValidationMessage("Item.Quantity", "*")%>
            </li>
            <li>
                <label for="Item.QuantityName">Quantity Name:</label><br />
                <%= Html.TextBox("Item.QuantityName") %>
                <%= Html.ValidationMessage("Item.QuantityName", "*") %>
            </li>
            <li>
                <label for="Item.Expiration">Expiration:</label> <br />
                <%= Html.TextBox("Item.Expiration", Model.Item != null && Model.Item.Expiration.HasValue ? Model.Item.Expiration.Value.ToString("d") : string.Empty)%>
                <%= Html.ValidationMessage("Item.Expiration", "*")%>
            </li>
            <li>
                <label for="Item.Link">Link:</label><br />
                <%= Html.TextBox("Item.Link",string.Empty , new { @class = "validateLink"})%>
                <%= Html.ValidationMessage("Item.Link", "*")%>
            </li>
            <li>
                <label for="Item.MapLink">Map Link:</label><br  />
                <%= Html.TextBox("MapLink") %>
                <%= Html.ValidationMessage("MapLink", "*") %>
            </li>
            <li><table><tbody><tr><td>
            <!-- NEEDS BALLOON-->
                <label for="Item.Available">Available to public:</label></td><td>
                <%= Html.CheckBox("Item.Available") %>
                <%= Html.ValidationMessage("Item.Available", "*") %></td></tr>
                <tr><td>
                <!-- NEEDS BALLOON-->
                <label for="Item.Private">Private Conference:</label></td><td>
                <%= Html.CheckBox("Item.Private") %>
                <%= Html.ValidationMessage("Item.Private", "*") %>
            </td></tr></tbody></table></li>
            <li>
                <label for="Item.RestrictedKey">Restricted Password:</label><br />
                <%= Html.TextBox("Item.RestrictedKey") %>
                <%= Html.ValidationMessage("Item.RestrictedKey", "*") %>
            </li>

        </fieldset>
                
        <fieldset>
            <legend>Extended Properties</legend>
            <ul>
            
            <div id="ExtendedProperties">            
                <%  if (Model.Item != null)
                    {
                        for (int i = 0; i < Model.Item.ItemType.ExtendedProperties.Count; i++)
                        {
                            var ep = Model.Item.ItemType.ExtendedProperties.ToArray()[i];
                            var ans = Model.Item.ExtendedPropertyAnswers.Where(a => a.ExtendedProperty == ep).FirstOrDefault();
                            var isDate = "";
                            if(Model.Item.ItemType.ExtendedProperties.ElementAtOrDefault(i).QuestionType.Name == "Date")
                            {
                                isDate = "isDate";                                
                            }                                           
                    %>
                                    <li>
                                        <label for="extendedProperty"><%=Html.Encode(ep.Name)%></label>
                                        <input type="text" id='<%=Html.Encode("ExtendedProperties[" + i + "]_value")%>' 
                                                name='<%=Html.Encode("ExtendedProperties[" + i + "].value")%>' 
                                                value='<%=Html.Encode(ans != null ? ans.Answer : string.Empty)%>'
                                                class='<%=Html.Encode(isDate)%>'
                                                />
                                        <input type="hidden" id='<%=Html.Encode("ExtendedProperties[" + i + "]_propertyId")%>' 
                                                name='<%=Html.Encode("ExtendedProperties[" + i + "].propertyId")%>' 
                                                value='<%= Html.Encode(ep.Id) %>'
                                                />
                                    </li>
                             
                                    <%
                            
                        }
                    }%>
            
            </div>
        </ul>    
        </fieldset>
        
        <fieldset>
            <legend>Upload Picture</legend>
            <!-- NEEDS BALLOON-->
            
            <ul>
            <li>
                <img src='<%= Url.Action("GetImage", "Item", new {id = Model.Item != null ? Model.Item.Id : -1}) %>' /> <br />
            
            <input type="file" id="file" name="file" />
            </li></ul>
        </fieldset>
        
        <fieldset>
            <legend>Tags</legend>
            <ul>
            <!-- NEEDS BALLOON-->
            <li>
            <input type="text" id="tagInput" />  <img id="tagAddButton" src="<%= Url.Content("~/Images/plus.png") %>" style="height:24px; width: 24px" />
            </li>
            <li>
            <div id="tagContainer">
            
                <% if (Model.Item != null)
                   {
                       foreach (var tag in Model.Item.Tags)
                       { %>
                    <input type="text" id="tags" name="tags" style="cursor:pointer" value='<%= tag.Name %>' />
                <% }
                   } %>
            
            </div>
            </li>
        </ul>    
        </fieldset>
            <input type="submit" value="Save" class="save_btn"/>
            <input type="submit" value="Clear" class="clear_btn"/>
