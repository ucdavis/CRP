<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CRP.Controllers.ViewModels.SearchViewModel>" %>
<%@ Import Namespace="CRP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Search Results
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Search Results</h2>

    <p>
        <%= Html.ActionLink<HomeController>(a => a.Index(), "View All") %>
    </p>

    <p>
        <span id="suggestion"></span>
    </p>
    
    <% Html.RenderPartial(CRP.Core.Resources.StaticValues.Partial_ItemBrowse, Model.Items.ToList()); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">

        var apiKey = "<%= Model.ApiKey %>";

        $(document).ready(function() {
            var encodedText = escape($("input#searchTerm").val());
            var spellService = "http://api.bing.net/json.aspx?AppId=" + apiKey + "&Query=" + encodedText + "&Sources=Spell&Version=2.0&Market=en-us&Options=EnableHighlighting&JsonType=callback&JsonCallback=SearchCompleted";

            $.getScript(spellService);
        });

        function SearchCompleted(result) {
            console.dir(result);
            var spellSuggestions = result.SearchResponse.Spell;

            if (spellSuggestions != undefined && spellSuggestions.Total != 0) {
                var firstSuggestion = spellSuggestions.Results[0].Value;
                $("#suggestion").html("Did you mean: " + firstSuggestion + " ?");
            }            

        }
    </script>
</asp:Content>
