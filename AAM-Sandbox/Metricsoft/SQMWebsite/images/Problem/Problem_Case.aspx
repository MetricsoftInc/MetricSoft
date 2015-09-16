<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Problem_Case.aspx.cs" Inherits="SQM.Website.Problem_Case"  EnableEventValidation="false" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_PageTabs.ascx" TagName="PageTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_CaseEdit.ascx" TagName="CaseEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="CaseList" TagPrefix="Ucl" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
     <script type="text/javascript">

         window.onload = function () {
              var timeout = document.getElementById('hfTimeout').value;
              var timeoutWarn = ((parseInt(timeout)-2) * 60000);
              window.setTimeout(function() {alert("Your Session Will Timeout In Approximately 2 Minutes.  Please save your work or cancel the page if you are finished.")}, timeoutWarn);
         }

    </script>
    <div class="admin_tabs">
        <asp:HiddenField id="hfErrRequiredInputs" runat="server" Value="Please enter all required (*) fields before saving."/>
        <asp:HiddenField id="hfErrIncidentError" runat="server" Value="The Problem Case must contain at least one incident."/>
        <asp:HiddenField id="hfErrIncomplete" runat="server" Value="Some action items are incomplete. Please enter all fields fields before saving."/>
        <asp:HiddenField id="hfErrCompleteError" runat="server" Value="Cannot set this step as completed until all page entries and pre-requisite steps have also been completed."/>
        <asp:HiddenField id="hfErrRootCauseError" runat="server" Value="There can be only one root cause determined."/>
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10">  <%--align="center">--%>
                    <br/>
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:HiddenField id="hfTimeout" runat="server"/>   
                        <asp:Panel runat="server" ID="pnlSearchBar">
                            <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                        </asp:Panel>
                        <table width="99%">
			                <tr>
                                <td>
                                    <asp:Label ID="lblProbCaseInstructions" runat="server" class="instructText" Text="Perform problem analysis and resolution according to the 8D (8 Disciplines) problem solving process. Problem cases are typically created in response to a product or process related quality control exception. Use this methodology to determine root cause, apply corrective actions and ensure preventative measures."></asp:Label>
                                    <asp:Label ID="lblProbCaseInstructionsEHS" runat="server" class="instructText" Text="EH&amp;S Problem Control Analysis: Perform problem analysis and resolution according to the 8D (8 Disciplines) problem solving process. Use this methodology to determine root cause, apply corrective actions and ensure preventative measures."></asp:Label>
                                    <asp:Label ID="lblProbCaseTitle" runat="server" Text="Problem Control Analysis" Visible="false"></asp:Label>
                                    <asp:Label ID="lblReturnCaseList" runat="server" Text="Return To List" Visible="false"></asp:Label>
                                    <asp:Label ID="lblReturnInbox" runat="server" Text="Return To Inbox" Visible="false"></asp:Label>
                                </td>
                            </tr>
                         </table>
                         <asp:Panel ID="pnlSearchList" runat="server" Visible="false">
                            <br />
                            <table cellspacing=0 cellpadding=2 border=0 width="99%">
			                    <tr>
                                    <td class=summaryData valign=top width="60%">
				                        <SPAN CLASS=summaryHeader>
                                            <asp:Label runat="server" ID="lblPlantSelect" Text="Business Locations"></asp:Label>
                                        </SPAN>
                                        <BR>
                                        <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex=9000 Skin="Metro"  Width="450"></telerik:RadComboBox>
		                            </td>
                                    <td class="summaryData" valign="top">
                                        <SPAN CLASS=summaryHeader>
                                            <asp:Label runat="server" ID="lblStatus" Text="Case Status"></asp:Label>
                                        </SPAN>
                                        <br />
                                        <telerik:RadComboBox ID="ddlStatusSelect" runat="server"  ZIndex=9000 Skin="Metro" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusSelectChange">
                                            <Items>
                                                <telerik:RadComboBoxItem Text="All" Value=""/> 
                                                <telerik:RadComboBoxItem Text="Active" Value="A" /> 
                                                <telerik:RadComboBoxItem Text="Inactive" Value="I" /> 
                                                <telerik:RadComboBoxItem Text="Closed" Value="C" /> 
                                            </Items>
                                        </telerik:RadComboBox>
                                        <asp:Button id="btnSearch" runat="server" style="margin-left: 30px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List problem cases" OnClick="btnSearchClick"/>
                                    </td>
                                </tr>
                            </table>
                            <Ucl:CaseList id="uclCaseList" runat="server"/>
                        </asp:Panel>
                         
                         <div id="divPageBody" runat="server">
                            <Ucl:CaseList id="uclCaseHdr" runat="server"/>
                            <br />
                            <div id="divNavArea" runat="server"  class="navAreaLeft" visible="false">
                                <asp:Label ID="lblTabsTitle" runat="server" Text="Steps" visible="false"></asp:Label>
                                <Ucl:PageTabs id="uclPageTabs" runat="server"/>
                            </div>
                            <div id="divWorkArea" runat="server" class="workAreaRight">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td align="center">
                                            <asp:Panel runat="server" ID="pnlCaseEdit">
                                                <Ucl:CaseEdit id="uclCaseEdit" runat="server"/>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    <!--</form>-->
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
