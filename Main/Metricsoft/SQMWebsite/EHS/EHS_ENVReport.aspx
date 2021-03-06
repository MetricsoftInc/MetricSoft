﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_ENVReport.aspx.cs" Inherits="SQM.Website.EHS_ENVReport" %>

<%@ Register src="~/Include/Ucl_EHSList.ascx" TagName="EHSList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSReport.ascx" TagName="GHGReport" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_Progress.ascx" TagName="Progress" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <script type="text/javascript">
        $(window).load(function () {
            document.getElementById('hfwidth').value = $(window).width();
            document.getElementById('hfheight').value = $(window).height();
        });

        $(window).resize(function () {
            document.getElementById('hfwidth').value = $(window).width();
            document.getElementById('hfheight').value = $(window).height();
            //__doPostBack('resize', '');
        });

        function SetViewOption(viewID) {
            document.getElementById('hfViewOption').value = viewID;
        }
    </script>

    <asp:HiddenField ID="hfwidth" runat="server" />
    <asp:HiddenField ID="hfheight" runat="server" />
    <asp:HiddenField ID="hfViewOption" runat="server"/>

    <asp:HiddenField ID="hfCriteriaErr" runat="server" Value="Error in report criteria selection. Please make sure at least one location is selected and the report from/to dates are valid." />
    <asp:HiddenField ID="hfViewOpenErr" runat="server" Value="Error opening or executing the selected report" />

    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <table width="99.5%">
			                <tr>
                                <td class="noprint">
                                    <asp:Label ID="lblPageTitle" runat="server" CssClass="pageTitles" Text="Plant Analytics" style="margin-left: 0px;"></asp:Label>
                                    <br />
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Location specific environmental analytics."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <telerik:RadAjaxPanel ID="ajaxMain" runat="server">
                                    <div class="noprint">
                                        <table cellspacing=0 cellpadding=2 border=0 width="100%" style="margin-bottom: 3px;">
                                            <tr>
                                                <td class="summaryDataEnd">
                                                    <asp:Label runat="server" ID="lblPlantSelect"  CssClass="prompt" Text ="Location:" Visible="false"></asp:Label>
                                                    &nbsp;
                                                    <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="false" EnableCheckAllItemsCheckBox="true" ZIndex="9000" Skin="Metro"  Width="350" height="300" Visible="false" OnClientLoad="DisableComboSeparators" OnSelectedIndexChanged="selectsChanged_Event" AutoPostBack="true" EmptyMessage="select location"></telerik:RadComboBox>
                                                    <telerik:RadMenu ID="mnuPlantSelect" runat="server" Skin="Default" Width=250  EnableAutoScroll="true" style="z-index: 2900"  Visible="false" DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="mnuPlantSelect_Select"></telerik:RadMenu>
		                                        </td>
                                                <td class="summaryDataEnd">
							                        <asp:Label runat="server" ID="lblDateFrom" CssClass="prompt" Text="From: "></asp:Label>
									                <telerik:RadMonthYearPicker ID="radDateFrom" runat="server" CssClass="textStd" Width="165" Skin="Metro" ShowPopupOnFocus="true" OnSelectedDateChanged="selectsChanged_Event" AutoPostBack="true"></telerik:RadMonthYearPicker>
									                &nbsp;
                                                    <asp:Label ID="lblDateTo" runat="server" CssClass="prompt" Text="To: " ></asp:Label>
									                &nbsp;
                                                    <telerik:RadMonthYearPicker ID="radDateTo" runat="server" CssClass="textStd" Width="165" Skin="Metro" ShowPopupOnFocus="true" OnSelectedDateChanged="selectsChanged_Event" AutoPostBack="true"></telerik:RadMonthYearPicker>
								                </td>
                                            </tr>
                                            <tr>
                                                <td class="summaryDataEnd" colspan="2" style="padding: 7px 0 7px 3px;">
                                                    <asp:Panel ID="pnlOptions" runat="server">
                                                        <asp:Label ID="lblOption" runat="server" CssClass="prompt" Text="Select Report:"></asp:Label>
                                                        &nbsp;
                                                        <asp:LinkButton ID="lnkOptInputs" runat="server" CssClass="buttonList" Text="Metric Inputs" CommandArgument="1" OnClick="lnkOpt_Click"></asp:LinkButton>
                                                        &nbsp;&nbsp;
                                                        <asp:LinkButton ID="lnkOptGHG" runat="server" CssClass="buttonTable" Text="GHG Emissions" CommandArgument="5" OnClick="lnkOpt_Click"></asp:LinkButton>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <Ucl:Progress id="uclProgress" runat="server"/>
                                    <div style="text-align: center;">
                                        <asp:Label ID="lblReportTitle" runat="server" CssClass="labelTitle"></asp:Label>
                                    </div>
                                    <div id="divInputs" runat="server" style="margin-top: 4px;" visible="false">
                                        <Ucl:EHSList id="uclInputs" runat="server" />
                                    </div>
                                    <div id="divView" runat="server" style="width: 99%; padding: 10px 0;" visible="false">
                                        <Ucl:RadGauge ID="uclView" runat="server" />
                                    </div>
                                    <div id="divGHG" runat="server" style="margin-top: 4px;" visible="false">
                                        <Ucl:GHGReport id="uclGHG" runat="server"/>
                                    </div>
                                    <div id="divExport" runat="server" visible="false" class="noprint" style="clear: both;">
                                        <br />
                                        <span>                                    
                                            <asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" style="margin-left: 5px;" Text="Print" OnClientClick="javascript:window.print()"></asp:LinkButton>
                                            <asp:LinkButton  ID="lnkExport" runat="server" Text="Export" ToolTip="Export Data To Excel Format" CssClass="buttonDownload" style="margin-left: 5px;" OnClick="lnkExport_Click"></asp:LinkButton>
                                        </span>    
                                        <Ucl:Export id="uclExport" runat="server"/>
                                    </div>
                                </telerik:RadAjaxPanel>
                                </td>
                            </tr>
                        </table>
                    </FORM>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

