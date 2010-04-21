<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.ItemViewModel>" %>
<%@ Import Namespace="CRP.Core.Domain"%>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $("input#Item_Expiration").datepicker();

            $("select#Item_ItemType").change(function(event) {
                var url = '<%= Url.Action("GetExtendedProperties", "ItemManagement") %>';

                $.getJSON(url + '/' + $(this).val(), {},
                    function(result) {

                        var length = result.length;

                        if (length > 0) {
                            $.each(result, function(index, item) {

                                var name = item.Name.replace(/ /g, "");

                                // create the label
                                var label = $("<label>").attr("for", "item." + name).html(item.Name);
                                var textBox = $("<input>").attr("id", "ExtendedProperties[" + index + "]_value")
                                                          .attr("name", "ExtendedProperties[" + index + "].value")
                                                          .attr("type", "text");
                                // create hidden field to store the extended property id
                                var hidden = $("<input>").attr("type", "hidden")
                                                         .attr("id", "ExtendedProperties[" + index + "]_propertyId")
                                                         .attr("name", "ExtendedProperties[" + index + "].propertyId")
                                                         .val(item.Id);

                                if (item.QuestionType.Name == "Date") {
                                    textBox.datepicker();
                                }

                                var p = $("<p>").append(label).append(textBox).append(hidden);
                                $("div#ExtendedProperties").append(p);
                            });
                        }
                    });
            });

            $("img#tagAddButton").click(function(event) {
                var input = $("<input>").attr("id", "tags").attr("name", "tags").val($("input#tagInput").val());
                $("div#tagContainer").append(input);

                // blank the input
                $("input#tagInput").val();
            });

            $("input#tags").click(function(event) {
                $(this).remove();
            });
        });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm("Create", "ItemManagement", FormMethod.Post, new { @enctype = "multipart/form-data" }))
       {%>
    <% Html.RenderPartial("ItemForm"); %>
    <% } %>

    <div>
        <%=Html.ActionLink<ItemManagementController>(a => a.List(), "Back to List") %>
    </div>

</asp:Content>



