<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About Us
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>About</h2>
    <div style="text-align: center;">
        <h3>Developed By The College Of Agricultural And Environmental Science Dean's Office</h3><br/><br/>
        Version <%=Html.Encode(System.Reflection.Assembly.GetAssembly(ViewContext.Controller.GetType()).GetName().Version.ToString())%>
    </div>
</asp:Content>
