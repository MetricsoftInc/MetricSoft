<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_RadGauge.ascx.cs" Inherits="SQM.Website.Ucl_RadGauge"  %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="telerik" %>

<script type="text/javascript">
    function OpenDetailWindow(details) {
        //$telerik.$("[id$='spanDetail']").text(details);
        document.getElementById('ctl00_ContentPlaceHolder1_uclDashbd0_uclGauge_winDetail_C_spanDetail').innerHTML = details;
        $find("<%=winDetail.ClientID %>").show();
    }
</script> 

<%--how to print
http://demos.telerik.com/aspnet-ajax/htmlchart/examples/applicationscenarios/printchartonly/defaultcs.aspx?product=htmlchart
    https://www.arclab.com/en/webformbuilder/how-to-print-a-specific-part-of-a-html-page-css-media-screen-print.html
    --%>

<telerik:RadWindow runat="server" ID="winDetail" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="500" Title="User Preferences"  Behaviors="Close">
    <ContentTemplate>
        <span id="spanDetail" runat="server">
        </span>
    </ContentTemplate>
</telerik:RadWindow>

