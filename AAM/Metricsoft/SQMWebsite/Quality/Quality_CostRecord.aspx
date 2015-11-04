<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Quality_CostRecord.aspx.cs" Inherits="SQM.Website.Quality_CostRecord" enableEventValidation="false" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_StreamList.ascx" TagName="StreamList" TagPrefix="Ucl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<link type="text/css" href="../css/redmond/jquery-ui-1.8.20.custom.css" rel="Stylesheet" />
  <script type="text/javascript">
      function CalcCost(tbF1, tbF2, tbTot) {
          var f1 = +(document.getElementById(tbF1).value.replace(/,/, '.'));
          var f2 = +(document.getElementById(tbF2).value.replace(/,/, '.'));
          document.getElementById(tbTot).value = ( f1 * f2).toString();
      }
  </script>

<script type="text/javascript" src="../scripts/jquery-1.4.1.min.js"></script>
<script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>
<div class="admin_tabs">
    <table width="100%" border="0" cellspacing="0" cellpadding="1">
        <tr>
            <td class="tabActiveTableBg" colspan="10" align="center">
			    <BR/>
                <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:Panel runat="server" ID="pnlSearchBar">
                        <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                    </asp:Panel>
                    <table width="99%">
			            <tr>
                            <td>
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Identify costs associated with quality issues"></asp:Label>
                                    <asp:Label ID="lblCRTitle" runat="server" Text="Cost Reporting" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>

                    <div id="divMessages" runat="server">
                        <asp:Label ID="lblNoIncidents" runat="server" Text="There are no Quality Issues included in this report" CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblInputError" runat="server" Text="Some cost values have not been entered correctly. Please review your entries and press the Calculate button to update" CssClass="labelEmphasis" visible="false"></asp:Label>
                    </div>

                    <div id="divPageBody" runat="server">

                        <div ID="divSearchList" runat="server" Visible="false">
                            <Ucl:StreamList id="uclReportList" runat="server"/>
                        </div>

                        <div id="divNavArea" runat="server"  class="navAreaLeft">
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr align="center" height="24">
                                    <td class="navMenuTitles">
                                        <asp:Label ID="lblTabsTitle" runat="server" Text="Report Options" CssClass="navMenuTitles"></asp:Label>
                                    </td>
                                </tr>
                                <tr align="center" align="center" height="24">
                                    <td>
                                        <asp:LinkButton id="btnCreateReport" runat="server" CssClass = "buttonLink" Text="Create Cost Report"
                                            OnClientClick="return confirmAction('Create The Cost Report');" ></asp:LinkButton>
                                    </td>
                                    </tr>
                                    <tr align="center" align="center" height="24">
                                    <td>
                                        <asp:LinkButton id="btnSubmitReport" runat="server" CssClass = "buttonLink" Text="Submit Report"
                                            OnClientClick="return confirmAction('Submit The Cost Report');"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divWorkArea" runat="server" class="workAreaRight">

                           <table width="99%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                <tr>
                                    <td class="columnHeader" width="25%">
                                        <asp:Label ID="lblCRName" runat="server" text="Report Name" ></asp:Label>
                                    </td>
                                    <td class="required" width="1%">&nbsp;</td>
                                    <td class="tableDataAlt" width="74%">
                                       <asp:TextBox ID="tbCRName" size="40" maxlength="100"  CssClass="textStd" runat="server"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblCRDesc" runat="server" text="Report Description" ></asp:Label>
                                    </td>
                                    <td class="required">&nbsp;</td>
                                    <td class="tableDataAlt">
                                       <asp:TextBox ID="tbCRDesc" runat="server" TextMode="multiline" rows="2" maxlength="400" runat="server" CssClass="commentAreaSmall"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblCRType" runat="server" text="Cost Incident(s) Type" ></asp:Label>
                                    </td>
                                    <td class="required">&nbsp;</td>
                                    <td class="tableDataAlt">
                                       <asp:DropDownList ID="ddlCRType" runat="server"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblCRIncidents" runat="server" text="Issuess Included In This Report" ></asp:Label>
                                    </td>
                                    <td class="required">&nbsp;</td>
                                    <td class="tableDataAlt">
                                        <Ucl:IssueList id="uclIncidents" runat="server"/>
                                        <input type="button" class="buttonAdd"  style="margin: 5px;" id="btnAddIncident" onclick="PopupCenter('../Quality/QualityIssueList.aspx?', 'newPage', 880, 600);" value="Add Issue" /> </input>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader" style="height: 24px;">
                                        <asp:Label ID="lblCRTotalCosts" runat="server" text="Report Total Costs" ></asp:Label>
                                        &nbsp;
                                        <asp:Label ID="lblCRTotalCostCurrency" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td class="tableDataAlt">
                                        <span>
                                            <asp:Label ID="lblCRTotalCostActualLbl" runat="server" CssClass="prompt" Text="Actual:" ></asp:Label>
                                            <asp:Label ID="lblCRTotalCostActual" size="12" maxlength="12" runat="server" CssClass="labelEmphasis" Enabled="false"></asp:Label>
                                        </span>
                                        &nbsp;&nbsp;
                                        <span>
                                            <asp:Label ID="lblCRTotalCostAvoidLbl" runat="server" CssClass="prompt" Text="Potential:" ></asp:Label>
                                            <asp:Label ID="lblCRTotalCostAvoid" size="12" maxlength="12" runat="server" CssClass="labelEmphasis" Enabled="false"></asp:Label>
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblCRUpdated" runat="server" Text="<%$ Resources:LocalizedText, LastUpdate %>"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td class="tableDataAlt">
                                        <asp:Label ID="lblUpdateBy" runat="server" style="font-weight: normal !important;"/>
                                        &nbsp;&nbsp;
                                        <asp:Label ID="lblUpdateDate" runat="server" style="font-weight: normal !important;"></asp:Label>
                                    </td>
                                </tr>
                            </table>

                            <div id="divCostTable" runat="server"  class="borderSoft" style="width: 99%; margin-top: 5px;">
                                <table width="100%" align="center" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="tableDataHdr"  width="25%" >
                                            &nbsp;<asp:Label ID="lblCRIncidentHdr" runat="server" Text="<%$ Resources:LocalizedText, Issue %>" CssClass="prompt"></asp:Label>
                                            &nbsp;<asp:Label ID="lblCRIncidentIDHdr" runat="server" text="" CssClass="prompt"></asp:Label>
                                            &nbsp;<asp:Label ID="lblCRCostHdr" runat="server" text="Costs" CssClass="prompt"></asp:Label>
                                        </td>
                                        <td class="tableDataHdr" width="1%"">&nbsp</td>
                                        <td class="tableDataHdr" width="20%"><asp:Label ID="lblCRActual" runat="server" text="Actual" CssClass="prompt" style="margin-left: 10px;"></asp:Label></td>
                                        <td class="tableDataHdr" width="20%"><asp:Label ID="lblCRAvoid" runat="server" text="Potential" CssClass="prompt" style="margin-left: 10px;"></asp:Label></td>
                                        <td class="tableDataHdr" width="34%"><asp:Label ID="lblCRNotes" runat="server" text="Notes" CssClass="prompt" style="margin-left: 10px;"></asp:Label></td>
                                    </tr>
                                </table>
                                <table width="99%" align="center" border="0" cellspacing="0" cellpadding="1">
                                    <tr>
                                        <td class="tableDataAlt" colspan="5" width="25%" style="height: 28px;">
                                            <asp:Label ID="lblCRCostType1" runat="server" text="Material:" CssClass="prompt"></asp:Label>
                                            <asp:Label ID="lblCRCostType1Instruction" runat="server" class="instructText" style="float: left; margin-left 7px;" Text="Part or component costs typically measured in the purchase price (per item)."></asp:Label>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader" width="25%">
                                            <asp:Label ID="lblCRItemQty" runat="server" text="Reject Qty"></asp:Label>
                                        </td>
                                        <td class="required" width="1%">&nbsp;</td>
                                        <td class="tableDataAlt"  width="20%">
                                            <asp:TextBox ID="tbCRItemQtyActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRItemQtyActual','tbCRUnitCostActual','tbCRItemCostActual');"/>
                                        </td>
                                        <td class="tableDataAlt"  width="20%">
                                            <asp:TextBox ID="tbCRItemQtyAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRItemQtyAvoid','tbCRUnitCostAvoid','tbCRItemCostAvoid');"/>
                                        </td>
                                        <td class="tableDataAlt" rowspan="3">
                                            <asp:TextBox ID="tbCRItemNote" runat="server" TextMode="multiline" rows="4" maxlength="400" runat="server" CssClass="commentAreaSmall"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRUnitCost" runat="server" text="Unit Cost"></asp:Label>
                                        </td>
                                        <td class="required">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRUnitCostActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRItemQtyActual','tbCRUnitCostActual','tbCRItemCostActual');"/>
                                                <asp:Label ID="lblUnitCostCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRUnitCostAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRItemQtyAvoid','tbCRUnitCostAvoid','tbCRItemCostAvoid');"/>
                                                <asp:Label ID="lblUnitCostCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRItemCost" runat="server" text="Reject Qty Cost"></asp:Label>
                                        </td>
                                        <td class="required">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRItemCostActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="lblItemCostCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRItemCostAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="lblItemCostCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                </table>

                                <table width="99%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 5px;">
						            <tr>
							            <td style="width: 20px;">
                                            <button type="button" id="btnToggleDowntimeCost" onclick="ToggleSectionVisible('pnlDowntimeCost','110px');">
                                                    <img id="pnlDowntimeCost_img" src="/images/defaulticon/16x16/arrow-8-right.png"/>
                                            </button>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblCostType2" runat="server" text="Downtime:" CssClass="prompt"></asp:Label>
                                            &nbsp;
                                            <asp:Label ID="lblCostType2Instruction" runat="server" class="instructText" Text="Costs resulting from an idle or starved production line due to part availability or acceptance."></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlDowntimeCost" runat="server" style="height:1px; visibility: hidden; margin-top: 2px;" >
                                <table width="99%" align="center" border="0" cellspacing="0" cellpadding="0">
                                    <tr >
                                        <td class="columnHeader" width="25%">
                                            <asp:Label ID="lblCRPlantLine" runat="server" text="Plant Line/Operation"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                        <td class="tableDataAlt" width="20%">
                                            <asp:DropDownList ID="ddlCRPlantLineActual" runat="server" OnSelectedIndexChanged="ddlSelectChanged" AutoPostback="true"></asp:DropDownList>
                                        </td>
                                        <td class="tableDataAlt" width="20%">
                                                <asp:DropDownList ID="ddlCRPlantLineAvoid" runat="server" OnSelectedIndexChanged="ddlSelectChanged" AutoPostback="true"></asp:DropDownList>
                                        </td>
                                        <td class="tableDataAlt" rowspan="4">
                                            <asp:TextBox ID="tbCRDowntimeNote" runat="server" TextMode="multiline" rows="5" maxlength="400" runat="server" CssClass="commentAreaSmall"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRDowntimeRate" runat="server" text="Burden Rate"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <asp:UpdatePanel ID="udpBurdenActual" runat="server" UpdateMode=Conditional>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ddlCRPlantLineActual" />
                                                </Triggers>
                                                <ContentTemplate>
                                            <span>
                                                <asp:TextBox ID="tbCRDowntimeRateActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRDowntimeRateActual','tbCRDowntimeActual','tbCRDowntimeCostActual');"/>
                                                <asp:Label ID="lblDowntimeCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td class="tableDataAlt">
                                            <asp:UpdatePanel ID="udpBurdenAvoid" runat="server" UpdateMode=Conditional>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ddlCRPlantLineAvoid" />
                                                </Triggers>
                                                <ContentTemplate>
                                            <span>
                                                <asp:TextBox ID="tbCRDowntimeRateAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');  CalcCost('tbCRDowntimeRateAvoid','tbCRDowntimeAvoid','tbCRDowntimeCostAvoid');"/>
                                                <asp:Label ID="lblDowntimeCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader" width="25%">
                                            <asp:Label ID="lblCRDowntimeActual" runat="server" text="Downtime (hours)"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"  width="20%">
                                            <asp:TextBox ID="tbCRDowntimeActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');  CalcCost('tbCRDowntimeRateActual','tbCRDowntimeActual','tbCRDowntimeCostActual');"/>
                                        </td>
                                        <td class="tableDataAlt"  width="20%">
                                            <asp:TextBox ID="tbCRDowntimeAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRDowntimeRateAvoid','tbCRDowntimeAvoid','tbCRDowntimeCostAvoid');"/>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRDowntimeCost" runat="server" text="Downtime Cost"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRDowntimeCostActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="lblDowntimeCostCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRDowntimeCostAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="lblDowntimeCostCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>

                                <table width="99%" border="0" cellspacing="0" cellpadding="0">
						            <tr>
							            <td style="width: 20px;">
                                            <button type="button" id="btnToggleLaborCost" onclick="ToggleSectionVisible('pnlLaborCost','133px');">
                                                    <img id="pnlLaborCost_img" src="/images/defaulticon/16x16/arrow-8-right.png"/>
                                            </button>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblCostType3" runat="server" text="Labor:" CssClass="prompt"></asp:Label>
                                            &nbsp;
                                            <asp:Label ID="lblCostType3Instruction" runat="server" class="instructText" Text="Cost of any labor expended as a result of this incident. E.g. material handling, rework, administrative, etc..."></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlLaborCost" runat="server" style="height:1px; visibility: hidden; margin-top: 2px;" >
                                <table width="99%" align="center" border="0" cellspacing="0" cellpadding="0">
                                    <tr >
                                        <td class="columnHeader" width="25%">
                                            <asp:Label ID="lblCRDLaborDept" runat="server" Text="<%$ Resources:LocalizedText, Department %>"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                        <td class="tableDataAlt" width="20%">
                                            <asp:DropDownList ID="ddlCRLaborDeptActual" runat="server" ></asp:DropDownList>
                                        </td>
                                        <td class="tableDataAlt" width="20%">
                                                <asp:DropDownList ID="ddlCRLaborDeptAvoid" runat="server"></asp:DropDownList>
                                        </td>
                                        <td class="tableDataAlt" rowspan="4">
                                            <asp:TextBox ID="tbCRLaborNote" runat="server" TextMode="multiline" rows="5" maxlength="400" runat="server" CssClass="commentAreaSmall"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRDLaborType" runat="server" text="Labor Type"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <asp:DropDownList ID="ddlCRLaborTypeActual" runat="server" OnSelectedIndexChanged="ddlSelectChanged" AutoPostback="true"></asp:DropDownList>
                                        </td>
                                        <td class="tableDataAlt">
                                                <asp:DropDownList ID="ddlCRLaborTypeAvoid" runat="server" OnSelectedIndexChanged="ddlSelectChanged" AutoPostback="true"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRLaborRate" runat="server" text="Labor Rate"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <asp:UpdatePanel ID="udpLaborActual" runat="server" UpdateMode=Conditional>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ddlCRLaborTypeActual" />
                                                </Triggers>
                                                <ContentTemplate>
                                            <span>
                                                <asp:TextBox ID="tbCRLaborRateActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRLaborRateActual','tbCRLaborActual','tbCRLaborCostActual');"/>
                                                <asp:Label ID="lblLaborCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td class="tableDataAlt">
                                            <asp:UpdatePanel ID="udpLaborAvoid" runat="server" UpdateMode=Conditional>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="ddlCRLaborTypeAvoid" />
                                                </Triggers>
                                                <ContentTemplate>
                                            <span>
                                                <asp:TextBox ID="tbCRLaborRateAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRLaborRateAvoid','tbCRLaborAvoid','tbCRLaborCostAvoid');"/>
                                                <asp:Label ID="lblLaborCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader" width="25%">
                                            <asp:Label ID="lblCRLaborActual" runat="server" text="Labor (hours)"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"  width="20%">
                                            <asp:TextBox ID="tbCRLaborActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only'); CalcCost('tbCRLaborRateActual','tbCRLaborActual','tbCRLaborCostActual');"/>
                                        </td>
                                        <td class="tableDataAlt"  width="20%">
                                            <asp:TextBox ID="tbCRLaborAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');  CalcCost('tbCRLaborRateAvoid','tbCRLaborAvoid','tbCRLaborCostAvoid');"/>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRLaborCost" runat="server" text="Labor Cost"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRLaborCostActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="lblLaborCostCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRLaborCostAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="lblLaborCostCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>

                                <table width="99%" border="0" cellspacing="0" cellpadding="0">
						            <tr>
							            <td style="width: 20px;">
                                            <button type="button" id="btnToggleShipCost" onclick="ToggleSectionVisible('pnlShipCost','80px');">
                                                    <img id="pnlShipCost_img" src="/images/defaulticon/16x16/arrow-8-right.png"/>
                                            </button>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblCostType4" runat="server" text="Shipping and Freight:" CssClass="prompt"></asp:Label>
                                            &nbsp;
                                            <asp:Label ID="lblCostType4Instruction" runat="server" class="instructText" Text="Identify any shipping or freight expenses incurred as result of this incident."></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Panel ID="pnlShipCost" runat="server" style="height:1px; visibility: hidden; margin-top: 2px;" >
                                <table width="99%" align="center" border="0" cellspacing="0" cellpadding="0">
                                    <tr >
                                        <td class="columnHeader" width="25%">
                                            <asp:Label ID="lblCRShipDesc" runat="server" text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                        <td class="tableDataAlt" width="20%">
                                                <asp:TextBox ID="tbCRShipDescActual" TextMode="multiline" rows="2" maxlength="400" runat="server" CssClass="commentAreaSmall" style="width: 90%;"/>
                                        </td>
                                        <td class="tableDataAlt" width="20%">
                                            <asp:TextBox ID="tbCRShipDescAvoid" TextMode="multiline" rows="2" maxlength="400" runat="server" CssClass="commentAreaSmall" style="width: 90%;"/>
                                        </td>
                                        <td class="tableDataAlt" rowspan="3">
                                            <asp:TextBox ID="tbCRShipNote" runat="server" TextMode="multiline" rows="3" maxlength="400" runat="server" CssClass="commentAreaSmall"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr >
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCRShipCost" runat="server" text="Shipping Cost"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRShipCostActual" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="tbShipCostCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbCRShipCostAvoid" size="12" maxlength="12" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                <asp:Label ID="tbShipCostCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>

                                <asp:UpdatePanel ID="udpTotals" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnCRCalculate" />
                                    </Triggers>
                                    <ContentTemplate>
                                    <table width="99%" align="center" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="tableDataAlt" width="25%" colspan="5" style="height: 28px;">
                                                <asp:Label ID="lblCostTotal" runat="server" text="Total Costs:" CssClass="prompt"></asp:Label>
                                                &nbsp;
                                                <asp:Label ID="lblCostTotalInstruction" runat="server" class="instructText" Text="The sum total of all the costs above, associated with this incident."></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style="height: 20px;">
                                            <td class="columnHeader" width="25%">
                                                <asp:Label ID="lblCRIncidentCost" runat="server" text="Issue Total"></asp:Label>
                                                <asp:Button id="btnCRCalculate" runat="server" Text="Calculate" OnClick="btnCalculateClick" CssClass="buttonEmphasis" style="margin-left: 10px;"/>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="20%">
                                                <span>
                                                    <asp:TextBox ID="tbCRIncidentCostActual" size="12" maxlength="12" runat="server" CssClass="labelEmphasis" Enabled="false"/>
                                                    <asp:Label ID="lblCRIncidentCostCurrencyActual" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                                </span>
                                            </td>
                                            <td class="tableDataAlt" width="20%">
                                                <span>
                                                    <asp:TextBox ID="tbCRIncidentCostAvoid" size="12" maxlength="12" runat="server" CssClass="labelEmphasis"  Enabled="false"/>
                                                    <asp:Label ID="lblCRIncidentCostCurrencyAvoid" runat="server" CssClass="refTextSmall" Text="USD" ></asp:Label>
                                                </span>
                                            </td>
                                            <td class="tableDataAlt" rowspan="1">
                                                <asp:TextBox ID="tbCRTotalNote" runat="server" TextMode="multiline" rows="2" maxlength="400" runat="server" CssClass="commentAreaSmall"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </FORM>
            </td>
        </tr>
    </table>
</div>
</asp:Content>
