<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Quality_Issue.aspx.cs" Inherits="SQM.Website.Quality_Issue" enableEventValidation="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="~/Include/Ucl_QualityIssue.ascx" TagName="QualityIssue" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_B2BList.ascx" TagName="ReceiptList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(window).load(function () {
            document.getElementById('hfwidth').value = $(window).width();
            document.getElementById('hfheight').value = $(window).height();
        });

        $(window).resize(function () {
            document.getElementById('hfwidth').value = $(window).width();
            document.getElementById('hfheight').value = $(window).height();
        });
    </script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <asp:HiddenField ID="hfwidth" runat="server" />
    <asp:HiddenField ID="hfheight" runat="server" />
    <div class="admin_tabs">
        <asp:HiddenField id="hfErrRequiredInputs" runat="server" Value="Please enter all required (*) fields before saving."/>
        <asp:HiddenField id="hfErrSaveError" runat="server" Value="An unexpected error ocurred while attempting to save the record."/>
        <asp:HiddenField id="hfErrIncomplete" runat="server" Value="One or more entries are incomplete. Please make sure a currency code is selected if entering estimated issue costs and/or a specfic defect has been selected per the primary problem area identified."/>
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:HiddenField ID="hfAlertSaveIssue" runat="server" Value="Please complete and save the issue before proceeding" />
                        <asp:Label ID="lblReturnLabel" runat="server" Text="Return To List" Visible="false"></asp:Label>
                        <asp:Panel runat="server" ID="pnlSearchBar1">
                            <Ucl:SearchBar id="uclSearchBar1" runat="server"/>
                        </asp:Panel>
                        <table width="99%">
			                <tr>
                                <td class="noprint">
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Document product non-conformances or quality control exceptions. Create new issue or search for exiting issues by issue number, problem type or part number."></asp:Label>
                                     <asp:Label ID="lblQIPRQTitle" runat="server" Text="Quality Issue" Visible="false"></asp:Label>
                                    <asp:Label ID="lblQIRCVTitle" runat="server" Text="Incoming Material Record" Visible="false"></asp:Label>
                                    <asp:Label ID="lblQICSTTitle" runat="server" Text="Reject Avoidance" Visible="false"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <div ID="divSearchList" runat="server">
                            <Ucl:IssueList id="uclIssueSearch" runat="server"/>
                              <%--  <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanelChart">--%>
                                <table cellspacing="2" cellpadding="0" border="0" width="100%" style="margin: 0px 4px 4px 4px;">
                                    <tr>
                                        <td valign="top">
                                            <asp:Label ID="lblChartType" runat="server" CssClass="prompt" Text="<%$ Resources:LocalizedText, ViewStatistics %>"></asp:Label>
                                            <telerik:RadComboBox ID="ddlChartType" runat="server" ZIndex="9000" Width="350" Skin="Metro" EmptyMessage="<%$ Resources:LocalizedText, SelectAChart %>" AutoPostBack="true" OnSelectedIndexChanged="ddlChartTypeChange">
                                            </telerik:RadComboBox>
                                            <div class="noprint" style="float: right; margin-right: 5px; width: 100px;">
                                                <asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="<%$ Resources:LocalizedText, Print %>" style="margin-right: 5px;"  Visible="false" OnClientClick="javascript:window.print()"></asp:LinkButton>
                                                <asp:LinkButton ID="lnkChartClose" runat="server" CssClass="buttonCancel" Visible="false"  OnClick="lnkCloseChart" ToolTip="<%$ Resources:LocalizedText, Close %>"></asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="pnlChartSection" runat="server" Width="100%">
                                                 <div id="divChart" runat="server" class="borderSoft" style="width: 99%; padding: 10px 0;">
                                                    <Ucl:RadGauge ID="uclChart" runat="server" />
                                                </div>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                       <%--         </telerik:RadAjaxPanel>--%>
                            <Ucl:IssueList id="uclIssueList" runat="server"/>
                            <Ucl:ReceiptList id="uclReceiptList" runat="server"/>
                        </div>

                        <Ucl:QualityIssue id="uclQualityIssue" runat="server"/>
                        <asp:Panel runat="server" ID="pnlSearchBar2">
                            <Ucl:SearchBar id="uclSearchBar2" runat="server"/>
                        </asp:Panel>
                    </FORM>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
