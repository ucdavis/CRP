<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Controllers.ViewModels.ItemViewModel>" %>
<%@ Import Namespace="CRP.Controllers" %>

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
            $("#file").bt('The maximum picture size is 4 meg. A free online resize tool can be found at: http://www.picresize.com/');
            $("#Item_Summary").bt('A short summary of the item to display on the home page with the other active items. Line breaks will not display. Max 750 characters.');
            $("#tagInput").bt('You must click on the plus button to add the tag you have entered here.', { positions: 'top' });
            $("#tags").bt('Click on the tag to remove it.', { positions: 'bottom' });
            $("#Item_DonationLinkLink").bt('If this is blank, it will not show up to the user. Validation Has been removed for this field. You need to test it before making this available to the public.');
            $("#Item_DonationLinkText").bt('This is the text that will appear in the clickable link.');
            
        });
    </script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.UrlValidator.js") %>"></script>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <%= Html.ClientSideValidation<Item>("Item") %>

        <%= Html.AntiForgeryToken() %>
         <%= Html.Hidden("FidIsDisabled", !Model.CanChangeFID) %>
        <fieldset>
            <ul>
            <li><%= Html.ActionLink<HelpController>(a => a.CreateItem(), "Watch Demo") %></li>
            <li><label for="ItemType">Item Type:</label><br /> 
            <% if (Model.Item == null || Model.IsNew) {%>                               
                <%= this.Select("Item.ItemType").Options(Model.ItemTypes,x=>x.Id, x=>x.Name).FirstOption("--Select an Item Type--")
                        .Selected(Model.Item != null ? Model.Item.ItemType.Id : 0)                         
                    %>
            
            <%} else {%>
            <%= this.Select("Item.ItemType").Options(Model.ItemTypes,x=>x.Id, x=>x.Name).FirstOption("--Select an Item Type--")
                        .Selected(Model.Item != null ? Model.Item.ItemType.Id : 0).Disabled(true)                         
                    %>
                    <%--<%= Html.Encode(Model.Item.ItemType.Name) %>--%>
                
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
                        .Selected(Model.Item != null ? Model.Item.Unit.Id 
                        : (Model.UserUnit != null ? Model.UserUnit.Id : 0)
                                                        )%>
            </li>
            <li>
                <label for="Item.TouchnetFID">Account Number:</label><span id="TouchnetFidExtraLable"><%=Html.Encode(Model.Item != null && Model.Item.TouchnetFID != null ? " " + Model.Item.TouchnetFID : string.Empty)%></span> <br />
                <% if (Model.Item == null || Model.IsNew || Model.CanChangeFID){%> 
                    <%=this.Select("Item.TouchnetFID")
                        .Options(Model.TouchnetFIDs, x=>x.FID, x=>x.Description)
                        .FirstOption("--Select an Account Number--")
                        .Selected(Model.Item != null && Model.Item.TouchnetFID != null ? Model.Item.TouchnetFID : null)%>
                <%} else {%>
                    <%=this.Select("Item.TouchnetFID")
                            .Options(Model.TouchnetFIDs, x=>x.FID, x=>x.Description)
                            .FirstOption("--Select a Touchnet FID--")
                            .Selected(Model.Item != null && Model.Item.TouchnetFID != null ? Model.Item.TouchnetFID : null).Disabled(true)%>
                <% } %>
                <%=Html.Encode("*Note: If you choose the wrong Account Number, you may not recieve your payments made by Credit Card.") %>                
            </li>
            <li>
                <label for="Item.Summary">Summary:</label><br />
                <%= Html.TextArea("Item.Summary", new { style="height:175px; width: 750px" , @title=' '})%>
                <%= Html.ValidationMessage("Item.Summary", "*")%> 
            </li>
            <li>
                <label for="Item.Description">Description:</label><br />
                <%= Html.TextArea("Item.Description")%>
                <%= Html.ValidationMessage("Item.Description", "*")%> 
            </li>
            <li>
                <label for="Item.CheckPaymentInstructions">Check Payment Instructions:</label><br />
                <%= Html.TextArea("Item.CheckPaymentInstructions", Model.Item == null || Model.Item.CheckPaymentInstructions == null ? Html.HtmlEncode("<h1>Thank you for your purchase!</h1> <h2>Please mail your RSVP card and payment to:</h2> <address>College of Agricultural and Environmental Sciences<br />150 Mrak Hall<br />One Shields Ave.<br />Davis, CA 95616<br /></address><h2>Please make checks payable to UC Regents.</h2>") : Html.HtmlEncode(Model.Item.CheckPaymentInstructions))%>
                <%= Html.ValidationMessage("Item.CheckPaymentInstructions", "*")%> 
            </li>
            <li><table><tbody>
            <tr><td>
                <label for="Item.AllowCreditPayment">Allow Credit Payment:</label></td><td>
                <%= Html.CheckBox("Item.AllowCreditPayment", Model.Item != null ? Model.Item.AllowCreditPayment : true)%>
                <%= Html.ValidationMessage("Item.AllowCreditPayment", "*")%></td></tr>
                <tr><td>
                <label for="Item.AllowCheckPayment">Allow Check Payment:</label></td><td>
                <%= Html.CheckBox("Item.AllowCheckPayment", Model.Item != null ? Model.Item.AllowCheckPayment : true)%>
                <%= Html.ValidationMessage("Item.AllowCheckPayment", "*")%></td>
<%--                <tr>
                    <td>
                        <label for="Item.HideDonation">Hide Donation Column:</label>
                    </td>
                    <td>
                        <%= Html.CheckBox("Item.HideDonation")%>
                        <%= Html.ValidationMessage("Item.HideDonation", "*")%>
                    </td>
                </tr>--%>
            </tbody></table></li>
            <li>
                <label for="Item.QuantityName">Name Of Item:</label><br />
                <%= Html.TextBox("Item.QuantityName", Model.Item != null ? Model.Item.QuantityName : "Ticket") %>
                <%= Html.ValidationMessage("Item.QuantityName", "*") %>
            </li>
            <li>
                <label id="CostPerItemLabel" for="Item.CostPerItem">Cost Per <%= Html.Encode(Model.Item != null ? Model.Item.QuantityName : "Ticket") %>:</label><br />
                <%= Html.TextBox("Item.CostPerItem", Model.Item != null ? string.Format("{0:0.00}", Model.Item.CostPerItem) : string.Empty)  %>
                <%= Html.ValidationMessage("Item.CostPerItem", "*")%>
            </li>
            <li>
                <label id="QuantityLabel" for="Item.Quantity">Number of <%= Html.Encode(Model.Item != null ? Model.Item.QuantityName : "Ticket") %>(s) Available:</label><br />
                <%= Html.TextBox("Item.Quantity") %>
                <%= Html.ValidationMessage("Item.Quantity", "*")%>
            </li>
            <li>
                <label for="Item.Expiration">Last Day To Register Online:</label> <br />
                <%= Html.TextBox("Item.Expiration", Model.Item != null && Model.Item.Expiration.HasValue ? Model.Item.Expiration.Value.ToString("d") : string.Empty, new { @title = "" })%>
                <%= Html.ValidationMessage("Item.Expiration", "*")%>
            </li>
            <li>
                <label for="Item.Link">Link:</label><br />
                <%= Html.TextBox("Item.Link", Model.Item != null ? Model.Item.Link : string.Empty, new { @class = "validateLink", @title = ""})%>
                <%= Html.ValidationMessage("Item.Link", "*")%>
            </li>
            
            <li>
                <fieldset>
                    <legend>Optional Donation Link</legend>
                    <ul>
                        <li>
                            <label for="Item.DonationLinkLegend">Donation Legend:</label><br />
                            <%= Html.TextBox("Item.DonationLinkLegend", Model.Item != null ? Model.Item.DonationLinkLegend : string.Empty)%>
                            <%= Html.ValidationMessage("Item.DonationLinkLegend", "*")%>
                        </li>

                        <li>
                            <label for="Item.DonationLinkInformation">Donation Information:</label><br />
                            <%= Html.TextArea("Item.DonationLinkInformation", Model.Item != null ? Model.Item.DonationLinkInformation : string.Empty, new { style = "height:60px; width: 750px", @title = ' ' })%>
                            <%= Html.ValidationMessage("Item.DonationLinkInformation", "*")%>
                        </li>

                        <li>
                            <label for="Item.DonationLinkLink">Donation Link:</label><br />
                            <%= Html.TextBox("Item.DonationLinkLink", Model.Item != null ? Model.Item.DonationLinkLink : string.Empty, new { @title = "" })%>
                            <%= Html.ValidationMessage("Item.DonationLinkLink", "*")%>
                        </li>
                        <li>
                            <label for="Item.DonationLinkText">Donation Link Text:</label><br />
                            <%= Html.TextBox("Item.DonationLinkText", Model.Item != null ? Model.Item.DonationLinkText : "Click here", new { @title = "" })%>
                            <%= Html.ValidationMessage("Item.DonationLinkText", "*")%>
                        </li>
                    </ul>
                </fieldset>
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
            <li>
                Direct Link to Register: 
               <%= Url.ItemDetailsPath(Model.Item != null ? Model.Item.Id:0) %>
               <%--<%= Url.Action("Details", "Item", new { id = Model.Item != null ? Model.Item.Id : 0 }, "https")%> --%>           
            </li>

        </fieldset>
        
<%--        <fieldset>
            <legend>Map Link</legend>
            
            <ul>
            <li><%= Html.ActionLink<HelpController>(a => a.LinkMap(), "Watch Demo to Link a Map") %></li>
            <li>
                <label for="Item.MapLink">Map Link:</label><br  />
                <%= Html.TextBox("MapLink") %>
                <%= Html.ValidationMessage("MapLink", "*") %>
            </li>
            <% if(Model.Item != null && !string.IsNullOrEmpty(Model.Item.MapLink)) {%>
            <li>
                <iframe width="425" height="350" frameborder="0" scrolling="no" marginheight="0" 
                    marginwidth="0" 
                    src="<%= Model.Item.MapLink %>"></iframe><br />
                <small>
                    <a href="<%= Model.Item.LinkLink %>" style="color:#0000FF;text-align:left">View Larger Map</a>
                </small>
            </li>
            <%}%>
            </ul>
        </fieldset>--%>
                
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
                                                class='required <%=Html.Encode(isDate) %>'
                                                title=''
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
            
            <input type="file" id="file" name="file" title=''/>
            </li></ul>
        </fieldset>
        
        <fieldset>
            <legend>Tags</legend>
            <ul>
            <!-- NEEDS BALLOON-->
            <li>
            <input type="text" id="tagInput" title=''/>  <img id="tagAddButton" src="<%= Url.Content("~/Images/plus.png") %>" style="height:24px; width: 24px" />
            </li>
            <li>
            <div id="tagContainer">
            
                <% if (Model.Item != null)
                   {
                       foreach (var tag in Model.Item.Tags)
                       { %>
                    <input type="text" id="tags" name="tags" title='' style="cursor:pointer" value='<%= tag.Name %>' />
                <% }
                   } %>
            
            </div>
            </li>
        </ul>    
        </fieldset>
            <input type="submit" value="Save" class="save_btn"/>
            <input type="submit" value="Clear" class="clear_btn"/>
