<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_DashboardArea.ascx.cs" Inherits="SQM.Website.Ucl_DashboardArea" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_Progress.ascx" TagName="Progress" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
</script>
<asp:Panel ID="pnlDashboardArea" runat="server" >
    <div id="divPerspective" runat="server" style="margin-top: 3px; margin-bottom: 3px;" class="noprint">
        <asp:Label runat="server" ID="lblPerspective" CssClass="instructText" Text="Select a view representing the detailed analysis you wish to explore. Views are listed according to system topics or data types represented." style="margin: 5px;"></asp:Label>
        <br />
    </div>

    <div id="divDashboardSelects" runat="server" style="margin-top: 4px;">
 <%--       <table width="100%" border="0" cellspacing="0" cellpadding="0">
		    <tr>
			    <td class="navSectionBar" style="width: 20px;">
                    <button type="button" id="btnToggleSelects" onclick="ToggleSection('pnlDashboardSelects');" class="navSectionBtn" >
                        <img id="pnlDashboardSelects_img" src="/images/defaulticon/16x16/arrow-8-right.png"/>
                    </button>
                </td>
                <td class="navSectionBar">
                    <asp:Label runat="server" ID="lblDashboardSelects" CssClass="prompt" Text="Display Criteria" ></asp:Label>
                </td>
               <td class="navSectionBar">
                    <asp:Button id="btnRefreshDashboard2" runat="server" CssClass="buttonEmphasis" style="float:left; margin-left: 40px; width: 200px;" Text="Refresh View" OnClick="btnRefreshDashboard_Click"/>
                </td>
		    </tr>
	    </table>--%>
        <asp:Panel ID="pnlDashboardSelects" runat="server">
            <table cellspacing=0 cellpadding=1 border=0 width="100%" style="margin-top: 2px;">
                <tr>
                    <td class=summaryDataEnd width="90px">
                        <asp:Label ID="lblView" runat="server" Text="View:" CssClass="prompt"></asp:Label>
                    </td>
                    <td class=summaryDataEnd>
                        <telerik:RadComboBox ID="ddlViewList" runat="server" Skin="Metro" Width=650  Font-Size=Small autopostback="True" onselectedindexchanged="ddlViewList_Select"></telerik:RadComboBox>
                        <asp:Button id="btnNewView" runat="server" Text="New" visible="false" onClick="onViewLayoutClick" CssClass="buttonStd" style="margin-left: 10px;" CommandArgument="new"/>
                        <asp:Button id="btnViewLayout" runat="server" Text="Layout" visible="false" enabled="false" onClick="onViewLayoutClick" CssClass="buttonStd" CommandArgument="edit"/>
                    </td>
                </tr>
                <tr>
                    <td class=summaryDataEnd width="90px">
                        <asp:Label ID="lblPlantSelect" runat="server" Text="Locations:" CssClass="prompt"></asp:Label>
                    </td>
                     <td class=summaryDataEnd>
                        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
                            <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex=9000 Skin="Metro" height="350" Width="650" OnClientLoad="DisableComboSeparators" ></telerik:RadComboBox>
                        </telerik:RadAjaxPanel>
                    </td>
                </tr>
                <tr>
                    <td class=summaryDataEnd width="90px">
                        <asp:Label runat="server" ID="lblDateSpan" CssClass="prompt" Text="Date Span:"></asp:Label>
                    </td>
                    <td class=summaryDataEnd>
                       <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2">
                            <telerik:RadComboBox ID="ddlDateSpan" runat="server" Skin="Metro" Width=250 Font-Size=Small AutoPostBack="true" OnSelectedIndexChanged="ddlDateSpanChange">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select Range" Value="0"/> 
                                    <telerik:RadComboBoxItem Text="Year To Date" Value="1" /> 
                                    <telerik:RadComboBoxItem Text="FY Year To Date" Value="4" /> 
                                    <telerik:RadComboBoxItem Text="FY Year Over Year" Value="5" /> 
                                    <telerik:RadComboBoxItem Text="FY Metrics Effective Range" Value="6"/> 
                                </Items>
                            </telerik:RadComboBox>
                            <span style="margin-left: 5px;">
                                <asp:PlaceHolder ID="phPeriodSpan" runat="server">
                                    <asp:Label runat="server" ID="lblPeriodFrom"  CssClass="prompt" Text="From: "></asp:Label>
                                    <telerik:RadMonthYearPicker ID="dmPeriodFrom" runat="server" CssClass="textStd" Width=155 Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadMonthYearPicker>
                                    <asp:Label runat="server" ID="lblPeriodTo" CssClass="prompt" Text="To: " style="margin-left: 5px;"></asp:Label>
                                    <telerik:RadMonthYearPicker ID="dmPeriodTo" runat="server" CssClass="textStd" Width=155 Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadMonthYearPicker>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phYearSpan" runat="server">
                                    <asp:Label runat="server" ID="lblYearFrom"  CssClass="prompt" Text="From: "></asp:Label>
                                    <telerik:RadComboBox ID="ddlYearFrom" runat="server" Skin="Metro" Width=80 Font-Size=Small AutoPostBack="false"></telerik:RadComboBox>
                                    <asp:Label runat="server" ID="lblYearTo" CssClass="prompt" Text="To: " style="margin-left: 5px;"></asp:Label>
                                    <telerik:RadComboBox ID="ddlYearTo" runat="server" Skin="Metro" Width=80 Font-Size=Small AutoPostBack="false"></telerik:RadComboBox> 
                                </asp:PlaceHolder>
                                    <asp:Label runat="server" ID="lblDateInterval" CssClass="prompt" Text="Interval:" style="margin-left: 5px;" Visible="false"></asp:Label>
                                 <telerik:RadComboBox ID="ddlDateInterval" runat="server" Visible="false" Width=120 Skin="Metro" Font-Size=Small ToolTip="select interval for charts calculating points by date">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Default" Value="0"/> 
                                    <telerik:RadComboBoxItem Text="By Year" Value="1"/> 
                                    <telerik:RadComboBoxItem Text="By Month" Value="2"/> 
                                </Items>
                            </telerik:RadComboBox>
                            </span>
                           <span style="float: right; margin-right: 20px;" class="noprint">
                                <asp:LinkButton ID="btnRefreshDashboard" runat="server" ToolTip="Refresh the dashboard display" CSSClass="buttonRefresh"  text="Refresh View"  OnClick="btnRefreshDashboard_Click"></asp:LinkButton>
                                <asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="Print" style="margin-left: 5px;" OnClientClick="javascript:window.print()"></asp:LinkButton>
                                <asp:LinkButton  ID="lnkExport" runat="server" Text="Export" ToolTip="Export Data To Excel Format" CssClass="buttonDownload" style="margin-left: 5px;" OnClick="lnkExportClick"></asp:LinkButton>
                            </span>
                         </telerik:RadAjaxPanel>
                    </td>
                </tr>
                <tr id="trViewOptions" runat="server" visible="false">
                    <td class=summaryDataEnd width="90px">
                        <asp:Label ID="lblOptions" runat="server" Text="Options:" CssClass="prompt"></asp:Label>
                    </td>
                    <td class=summaryDataEnd>
                        <telerik:RadComboBox ID="ddlOptions" runat="server" CheckBoxes="true" Width=160 EnableCheckAllItemsCheckBox="false"  CheckedItemsTexts="DisplayAllInInput" ZIndex=9000 Skin="Metro" EmptyMessage="Select Display Options" >
                            <Items>
                                <telerik:RadComboBoxItem Text="Display Totals Only" Value="2"/> 
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div id="divMessages" runat="server">
        <center>
            <Ucl:Progress id="uclProgress" runat="server"/>
            <telerik:RadAjaxPanel ID="radAjaxMessages" runat="server">
                <asp:Label ID="lblWorking" runat="server" Text="Loading view - please wait..." CssClass="labelEmphasis" visible="false"></asp:Label>
                <asp:Label ID="lblViewLoadError" runat="server" Text="An error occured attemtping to open the selected view." CssClass="labelEmphasis" visible="false"></asp:Label>
                <asp:Label ID="LblViewSaveError" runat="server" Text="An error occured while saving your changes." CssClass="labelEmphasis" visible="false"></asp:Label>
            </telerik:RadAjaxPanel>
        </center>
    </div>
    <asp:HiddenField id="hfContent" runat="server"/>
    <asp:HiddenField ID="hfPerspective" runat="server"/>
    <asp:HiddenField ID="hfViewMode" runat="server"/>
    <asp:HiddenField ID="hfViewStatus" runat="server" />
    <asp:HiddenField ID="hfActive" runat="server" />
    <Ucl:RadGauge id="uclGauge" runat="server"/>
    <div id="divDashboardArea" runat="server" style="text-align:center; margin-top: 10px;">
        <asp:Label ID="lblDashboardTitle" runat="server" CssClass="refText" style="display:block; margin-bottom: 10px;" ></asp:Label>
    </div>
    
    <div id="divLayoutArea" runat="server" visible="false" style="margin-top: 10px; margin-bottom: 10px;">
        <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
            <tr>
                <td class="columnHeader" width="24%">
                    <asp:Label ID="lblViewName" runat="server" text="View Name"></asp:Label>
                </td>
                <td class="required" width="1%">&nbsp;</td>
                <td class="tableDataAlt textStd" width="75%">
                    <telerik:RadTextBox ID="tbViewName" runat="server" Columns=60 Skin="Metro" MaxLength=100 Font-Size=Small></telerik:RadTextBox>
                    <span style="float:right; margin-right: 5px;">
                        <asp:Button id="btnTestView" runat="server" CssClass="buttonStd" text="Preview" OnClick="onTestViewClick" Visible=false/>
                        <asp:Button id="btnViewCancel" runat="server" Text="Cancel" onClick="onViewEditClick" CssClass="buttonStd" Visible=false CommandArgument="cancel"/>
                        <asp:Button ID="btnViewSave" runat="server" Text="Save View" OnClientClick="return confirmChange('View');" onClick="onViewEditClick" CssClass="buttonEmphasis" Visible=false CommandArgument = "save"/>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="lblViewDesc" runat="server" text="Title"></asp:Label>
                </td>
                <td class="tableDataAlt">&nbsp;</td>
                <td class="tableDataAlt">
                    <telerik:RadTextBox ID="tbViewDesc" runat="server" Columns=60 Skin="Metro" MaxLength=400 Font-Size=Small></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="Label1" runat="server" text="Perspective"></asp:Label>
                </td>
                <td class="required">&nbsp;</td>
                <td class="tableDataAlt">
                    <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel3">
                        <telerik:RadComboBox ID="ddlPerspective" runat="server" Skin="Metro" Width=250 ZIndex="9000" Font-Size=Small AutoPostBack="true" OnSelectedIndexChanged="ddlPerspectiveChange">
                            <Items>
                                    <telerik:RadComboBoxItem Text="Environmental Metrics" Value="E" /> 
                                    <telerik:RadComboBoxItem Text="Environmental YTD Performance" Value="EP" /> 
                                    <telerik:RadComboBoxItem Text="Health & Safety" Value="HS" /> 
                                    <telerik:RadComboBoxItem Text="Health & Safety YTD Performance" Value="HSP" /> 
                                    <telerik:RadComboBoxItem Text="Quality" Value="QS" /> 
                            </Items>
                        </telerik:RadComboBox>
                    </telerik:RadAjaxPanel>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="lblViewAvailability" runat="server" text="Availability"></asp:Label>
                </td>
                <td class="tableDataAlt">&nbsp;</td>
                <td class="tableDataAlt">
                    <telerik:RadComboBox id="ddlViewAvailability" runat="server" Skin="Metro" Font-Size=Small Width=300>
                        <Items>
                            <telerik:RadComboBoxItem Text="Owner" Value="1" /> 
                            <telerik:RadComboBoxItem Text="Plant" Value="2"/> 
                            <telerik:RadComboBoxItem Text="Business Org" Value="3" /> 
                            <telerik:RadComboBoxItem Text="Company" Value="4"/> 
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="lblDfltParams" runat="server" text="Display Criteria"></asp:Label>
                </td>
                <td class="required">&nbsp;</td>
                <td class="tableDataAlt textStd">
                        <telerik:RadComboBox id="ddlDfltCriteria" runat="server" Skin="Metro" Font-Size=Small Width=300>
                        <Items>
                            <telerik:RadComboBoxItem Text="No Defaults" Value="0" /> 
                            <telerik:RadComboBoxItem Text="Use Current / Allow Override" Value="1"/> 
                            <telerik:RadComboBoxItem Text="Use Current / No Override" Value="2"/> 
                            <telerik:RadComboBoxItem Text="Select Locations" Value="3" />  
                            <telerik:RadComboBoxItem Text="Select Time Span" Value="4" />  
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr height="22px;">
                <td class="columnHeader">
                    <asp:Label ID="lblLastUpdate" runat="server" text="Last Updated By"></asp:Label>
                </td>
                <td class="tableDataAlt">&nbsp;</td>
                <td class="tableDataAlt">
                    <asp:Label id="lblLastUpdate_out" runat="server" CssClass="textStd"></asp:Label>
                </td>
            </tr>
        </table>
            <div class="borderSoft" style="margin-top: 3px;">
                <asp:Button id="btnAddChart1" runat="server" text="Add Chart" OnClick="onAddChartClick"/>
                <asp:Repeater runat="server" ID="rptViewItem" ClientIDMode="AutoID" OnItemDataBound="rptViewItem_OnItemDataBound">
			        <HeaderTemplate>
				        <table cellspacing="0" cellpadding="1" border="0" width="100%" >
				    </HeaderTemplate>
				    <ItemTemplate>
                        <tr>
                            <td width="95px;">
                                <asp:Label ID="lblVISeq" runat="server" Text="Chart" CssClass="prompt"></asp:Label>
                            </td>
                            <td>
                                <telerik:RadAjaxPanel runat="server" ID="radPnlGraph">
                                    <telerik:RadComboBox id="ddlVISeq" runat="server" Skin="Metro" Font-Size=Small  ZIndex=9000 Height=300 Width=75 ToolTip="display sequence">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Delete" Value="0"/> 
                                        </Items>
                                    </telerik:RadComboBox>
                                    <telerik:RadComboBox id="ddlVIGaugeType" runat="server" Skin="Metro" Font-Size=Small EmptyMessage="chart type" ToolTip="select chart or gauge type">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Section Area" Value="1"/> 
                                            <telerik:RadComboBoxItem Text="Vernier" Value="10"/> 
                                            <telerik:RadComboBoxItem Text="Vernier Array" Value="210"/> 
                                            <telerik:RadComboBoxItem Text="Bar Graph" Value="11"/> 
                                            <telerik:RadComboBoxItem Text="Stacked-Bar Graph" Value="12"/> 
                                            <telerik:RadComboBoxItem Text="Bar Pareto Chart" Value="15"/> 
                                            <telerik:RadComboBoxItem Text="Column Gauge" Value="20"/> 
                                            <telerik:RadComboBoxItem Text="Column Gauge Array" Value="220"/> 
                                            <telerik:RadComboBoxItem Text="Column Chart" Value="21"/> 
                                            <telerik:RadComboBoxItem Text="Stacked-Column Chart" Value="22"/> 
                                            <telerik:RadComboBoxItem Text="Grouped-Column Chart" Value="23"/> 
                                            <telerik:RadComboBoxItem Text="Column Pareto Chart" Value="25"/> 
                                            <telerik:RadComboBoxItem Text="Line Chart" Value="32"/> 
                                            <telerik:RadComboBoxItem Text="Line Chart Array" Value="232"/> 
                                            <telerik:RadComboBoxItem Text="Pie Chart" Value="50"/> 
                                            <telerik:RadComboBoxItem Text="Pie Chart Array" Value="250"/> 
                                            <telerik:RadComboBoxItem Text="Radial Gauge" Value="60"/> 
                                            <telerik:RadComboBoxItem Text="Radial Gauge Array" Value="260"/> 
                                        </Items>
                                    </telerik:RadComboBox>
                                    <telerik:RadTextBox ID="tbVIHeight" runat="server" Columns=4 Skin="Metro" MaxLength=5 Font-Size=Small ToolTip="height" EmptyMessage="height"></telerik:RadTextBox>
                                    <asp:Label id="lblX" runat="server" text="x" CssClass="textSmall"></asp:Label>
                                    <telerik:RadTextBox ID="tbVIWidth" runat="server" Columns=4 Skin="Metro" MaxLength=5 Font-Size=Small ToolTip="width" EmptyMessage="width"></telerik:RadTextBox>
                                    <telerik:Radbutton ID="cbVINewRow" runat="server" Text="Start New Row" ButtonType=ToggleButton ToggleType=CheckBox CssClass="textSmall"></telerik:Radbutton>
                                </telerik:RadAjaxPanel>
                            </td>
                        </tr>
                        <tr>
                            <td width="95px;">&nbsp;</td>
                            <td>
                                <telerik:RadTextBox ID="tbVITitle" runat="server" Columns=60 Skin="Metro" MaxLength=100 Font-Size=Small Text = "" EmptyMessage="chart title"></telerik:RadTextBox>
                            </td>
                        </tr>
                        <tr>
                             <td width="95px;">
                                <asp:Label ID="lblVIScope" runat="server" Text="Data" CssClass="prompt"></asp:Label>
                            </td>
                            <td>
                                <telerik:RadAjaxPanel runat="server" ID="radPnlScope">
                                    <telerik:RadComboBox id="ddlVIStat" runat="server" Skin="Metro" Font-Size=Small EmptyMessage="statistic" ToolTip="select metric to display"
                                        AutoPostBack=true OnSelectedIndexChanged="ddlVIStatChange">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Total" Value="sum"/> 
                                            <telerik:RadComboBoxItem Text="Cost" Value="sumCost"/> 
                                            <telerik:RadComboBoxItem Text="Percent Change" Value="pctChange" /> 
                                            <telerik:RadComboBoxItem Text="Days Elapsed" Value="deltaDy"/> 
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:Label id="lblOf" runat="server" text="of" CssClass="textSmall"></asp:Label>
                                    <telerik:RadComboBox id="ddlVIScope" runat="server" CheckBoxes="true"  Width=250  Height=350 ZIndex=9000 Skin="Metro" Font-Size=Small EmptyMessage="for metric or category" ToolTip="select metrics by category, type or specific measure">
                                    </telerik:RadComboBox>
                                    <telerik:RadComboBox id="ddlVISeriesOrder" runat="server" Skin="Metro" Font-Size=Small EmptyMessag="order by" ToolTip="data series ordering">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Sum All" Value="0"/> 
                                            <telerik:RadComboBoxItem Text="Order By Measure-Location" Value="1"/> 
                                            <telerik:RadComboBoxItem Text="Order By Location-Measure" Value="2" /> 
                                            <telerik:RadComboBoxItem Text="Order By Period-Location" Value="3"/> 
                                            <telerik:RadComboBoxItem Text="Order By Period-Measure" Value="4"/> 
                                            <telerik:RadComboBoxItem Text="Order By Year-Location" Value="5"/> 
                                            <telerik:RadComboBoxItem Text="Order By Year-Measure" Value="6"/> 
                                          <%--SumAll, MeasurePlant, PlantMeasure, PeriodMeasurePlant, YearMeasurePlant, PeriodMeasure, YearMeasure};--%>
                                        </Items>
                                    </telerik:RadComboBox>
                                </telerik:RadAjaxPanel>
                            </td>
                        </tr>
                        <tr>
                            <td width="95px;">
                                <asp:Label ID="lblVIXAxis" runat="server" Text="Variable Axis" CssClass="prompt"></asp:Label>
                            </td>
                            <td>
                                <telerik:RadAjaxPanel runat="server" ID="radPnlXAxis">
                                    <telerik:RadComboBox id="ddlVIXAxisScale" runat="server" Skin="Metro" Width=120 Font-Size=Small EmptyMessag="set scale" ToolTip="choose auto scaling or enter min/max scale values"
                                      AutoPostBack=true OnSelectedIndexChanged="ddlVIScaleChange">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Auto Scale" Value="0"/> 
                                            <telerik:RadComboBoxItem Text="Set Scales" Value="1" /> 
                                        </Items>
                                    </telerik:RadComboBox>
                                    <telerik:RadTextBox ID="tbVIXAxisMin" runat="server" Columns=9 Skin="Metro" MaxLength=20 Font-Size=Small ToolTip="axis minimum or orign value" EmptyMessage="scale min"></telerik:RadTextBox>
                                        <asp:Label id="lblXTo" runat="server" text="to" CssClass="textSmall"></asp:Label>
                                    <telerik:RadTextBox ID="tbVIXAxisMax" runat="server" Columns=9 Skin="Metro" MaxLength=20 Font-Size=Small ToolTip="axis maximum value" EmptyMessage="scale max"></telerik:RadTextBox>
                                    <asp:Label id="lblXBy" runat="server" text="by" CssClass="textSmall"></asp:Label>
                                    <telerik:RadTextBox ID="tbVIXAxisUnit" runat="server" Columns=9 Skin="Metro" MaxLength=20 Font-Size=Small ToolTip="scale unit" EmptyMessage="scale unit" ></telerik:RadTextBox>
                                    <asp:Label ID="lblTarget" runat="server" Text="Target:" CssClass="textSmall"></asp:Label>
                                    <telerik:RadComboBox id="ddlVITarget" runat="server" Skin="Metro" Font-Size=Small ZIndex=9000 EmptyMessag="overlay target" ToolTip="overlay metric target">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="None" Value="0"/> 
                                            <telerik:RadComboBoxItem Text="Target Value" Value="1"/>
                                            <telerik:RadComboBoxItem Text="Current Value" Value="1"/> 
                                        </Items>
                                    </telerik:RadComboBox>
                                </telerik:RadAjaxPanel>
                            </td>
                        </tr>
                        <tr>
                            <td width="95px;">&nbsp;</td>
                            <td>
                                <telerik:RadTextBox ID="tbVIXAxisLabel" runat="server" Columns=60 Skin="Metro" MaxLength=200 Font-Size=Small Text="" ToolTip="variable axis label" EmptyMessage="axis label"></telerik:RadTextBox>
                            </td>
                        </tr>
                        <tr id="trVIYAxis" runat="server">
                            <td width="95px;">
                                <asp:Label ID="lblVIYAxis" runat="server" Text="Ordinal Axis" CssClass="prompt"></asp:Label>
                            </td>
                            <td>
                                <telerik:RadTextBox ID="tbVIYAxisLabel" runat="server" Columns=60 Skin="Metro" MaxLength=200 Font-Size=Small Text="" ToolTip="ordinal label" EmptyMessage="axis label"></telerik:RadTextBox>
                            </td>
                        </tr>
                        <tr><td colspan="2"><hr width="99%"  size="1" color="#DCDCDC"></td></tr>
                    </ItemTemplate>
			    <FooterTemplate>
			    </table></FooterTemplate>
		    </asp:Repeater>
            <asp:Button id="btnAddChart2" runat="server" text="Add Chart" OnClick="onAddChartClick"/>
        </div>
    </div>
    <div id="divExport" runat="server">
         <Ucl:Export id="uclExport" runat="server"/>
    </div>
</asp:Panel>

<asp:Panel ID="pnlStats" runat="server" Visible="false">
    <div>             
        <%--<center>--%>
            <div style="margin-top: 5px;">
                <asp:Label id="lblStats" runat="server" CssClass="prompt" ></asp:Label>
            </div>
            <div id="divStatsArea" runat="server" style ="width: 850px; overflow-x: scroll; overflow:auto; margin-top: 4px;">
            </div>  
        <%--</center>--%>
    </div>     
</asp:Panel>
