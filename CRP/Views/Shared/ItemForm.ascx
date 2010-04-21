<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CRP.Controllers.ViewModels.ItemViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <%= Html.ClientSideValidation<Item>("Item") %>

        <%= Html.AntiForgeryToken() %>
        <fieldset>
            <legend>Fields</legend>
            
            <% if (Model.Item == null) {%>
            <p>
                <%= this.Select("Item.ItemType").Options(Model.ItemTypes,x=>x.Id, x=>x.Name).FirstOption("--Select an Item Type--")
                        .Selected(Model.Item != null ? Model.Item.ItemType.Id : 0) 
                        .Label("Item Type:")
                    %>
            </p>
            <%} else {%>
                <p>
                    <label for="ItemType">Item Type:</label>
                    <%= Html.Encode(Model.Item.ItemType.Name) %>
                </p>
            <% } %>
            
            <p>
                <label for="Item.Name">Name:</label>
                <%= Html.TextBox("Item.Name") %>
                <%= Html.ValidationMessage("Item.Name", "*")%>
            </p>
            <p>
                <%= this.Select("Item.Unit").Options(Model.CurrentUser.Units, x=>x.Id, x=>x.FullName)
                        .Selected(Model.Item != null ? Model.Item.Unit.Id : 0)
                        .Label("Unit:")%>
            </p>
            <p>
                <label for="Item.Description">Description:</label>
                <%= Html.TextArea("Item.Description")%>
                <%= Html.ValidationMessage("Item.Description", "*")%> 
            </p>
            <p>
                <label for="Item.CostPerItem">CostPerItem:</label>
                <%= Html.TextBox("Item.CostPerItem", Model.Item != null ? string.Format("{0:0.00}", Model.Item.CostPerItem) : string.Empty)  %>
                <%= Html.ValidationMessage("Item.CostPerItem", "*")%>
            </p>
            <p>
                <label for="Item.Quantity">Quantity:</label>
                <%= Html.TextBox("Item.Quantity") %>
                <%= Html.ValidationMessage("Item.Quantity", "*")%>
            </p>
            <p>
                <label for="Item.Expiration">Expiration:</label>
                <%= Html.TextBox("Item.Expiration", Model.Item != null && Model.Item.Expiration.HasValue ? Model.Item.Expiration.Value.ToString("d") : string.Empty)%>
                <%= Html.ValidationMessage("Item.Expiration", "*")%>
            </p>
            <p>
                <label for="Item.Link">Link:</label>
                <%= Html.TextBox("Item.Link")%>
                <%= Html.ValidationMessage("Item.Link", "*")%>
            </p>

            <p>
                <label for="Item.Available">Availabe to public:</label>
                <%= Html.CheckBox("Item.Available") %>
                <%= Html.ValidationMessage("Item.Available", "*") %>
            </p>
            
            <p>
                <label for="Item.Private">Private Conference:</label>
                <%= Html.CheckBox("Item.Private") %>
                <%= Html.ValidationMessage("Item.Private", "*") %>
            </p>            

        </fieldset>
        
        <fieldset>
            <legend>Extended Properties</legend>
            
            <div id="ExtendedProperties">
            
                <%  if (Model.Item != null)
                    {
                        for (int i = 0; i < Model.Item.ItemType.ExtendedProperties.Count; i++)
                        {
                            var ep = Model.Item.ItemType.ExtendedProperties.ToArray()[i];
                            var ans = Model.Item.ExtendedPropertyAnswers.Where(a => a.ExtendedProperty == ep).FirstOrDefault();
                    %>
                                    <p>
                                        <label for="extendedProperty"><%=Html.Encode(ep.Name)%></label>
                                        <input type="text" id='<%=Html.Encode("ExtendedProperties[" + i + "]_value")%>' 
                                                name='<%=Html.Encode("ExtendedProperties[" + i + "].value")%>' 
                                                value='<%=Html.Encode(ans != null ? ans.Answer : string.Empty)%>'
                                                />
                                        <input type="hidden" id='<%=Html.Encode("ExtendedProperties[" + i + "]_propertyId")%>' 
                                                name='<%=Html.Encode("ExtendedProperties[" + i + "].propertyId")%>' 
                                                value='<%= Html.Encode(ep.Id) %>'
                                                />
                                    </p>
                                    <%
                        }
                    }%>
            
            </div>
            
        </fieldset>
        
        <fieldset>
            <legend>Upload Picture</legend>
            
            <p>
                <img src='<%= Url.Action("GetImage", "Item", new {id = Model.Item != null ? Model.Item.Id : -1}) %>' />
            </p>
            
            <input type="file" id="file" name="file" />
        </fieldset>
        
        <fieldset>
            <legend>Tags</legend>
            
            <input type="text" id="tagInput" />  <img id="tagAddButton" src="../../Images/plus.png" style="height:24px; width: 24px" />
            <div id="tagContainer">
            
                <% if (Model.Item != null)
                   {
                       foreach (var tag in Model.Item.Tags)
                       { %>
                    <input type="text" id="tags" name="tags" style="cursor:pointer" value='<%= tag.Name %>' />
                <% }
                   } %>
            
            </div>
            
        </fieldset>
        
        <p>
            <input type="submit" value="Save" />
        </p>
