<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SQM.Website.Dashboard"  %>
<%@ Register src="~/Include/Ucl_DashboardArea.ascx" TagName="Dashboard" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
       <script type="text/javascript">
           window.onload = function () {
               var timeout = document.getElementById('hfTimeout').value;
               var timeoutWarn = ((parseInt(timeout) - 2) * 60000);
               window.setTimeout(function () { alert("Your Session Will Timeout In Approximately 2 Minutes.  Please save your work or cancel the page if you are finished.") }, timeoutWarn);
           }
       </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="scripts/ps_admin.js" type="text/javascript"></script>
    <link href="css/PSSQM.css" rel="stylesheet" type="text/css" />
    <asp:HiddenField ID="hfBase" runat="server" />
    <asp:HiddenField id="hfTimeout" runat="server"/>
    <asp:HiddenField ID="hdCurrentActiveTab" runat="server" />
    <asp:HiddenField ID="hdCurrentActiveSecondaryTab" runat="server" />
     <asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
    <FORM name="dummy">
        <div>
            <table width="100%" border="0" cellspacing="0" cellpadding="1">
                <tr>
                    <td class="tabActiveTableBg" colspan="10" align="center">
                        <asp:Panel ID="pnlMonitor" runat="server" GroupingText="Performance Dashboard" CssClass="sectionTitles" Width="99%" style="margin: 3px;">
                            <Ucl:Dashboard  ID="uclDashbd0" runat="server"/>
                        </asp:Panel>
                    </td>
                </tr>
            </table>  
         </div>
     </FORM>
</asp:Content>
