<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_DashboardArea.ascx.cs" Inherits="SQM.Website.Ucl_DashboardArea" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_Progress.ascx" TagName="Progress" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
</script>
<asp:Panel ID="pnlDashboardArea" runat="server">
    <div id="divPerspective" runat="server" style="margin-top: 3px; margin-bottom: 3px;" class="noprint">
        <asp:Label runat="server" ID="lblPerspective" CssClass="instructText" Text="Select a view representing the detailed analysis you wish to explore. Views are listed according to system topics or data types represented." style="margin: 5px;" meta:resourcekey="lblPerspectiveResource1"></asp:Label>
        <br />
    </div>

    <div id="divDashboardSelects" runat="server" style="margin-top: 4px;">
        <asp:Panel ID="pnlDashboardSelects" runat="server">
            <table cellspacing=0 cellpadding=1 border=0 width="100%" style="margin-top: 2px;">
                <tr>
                    <td class=summaryDataEnd width="90px">
                        <asp:Label ID="lblView" runat="server" Text="View:" CssClass="prompt" meta:resourcekey="lblViewResource1"></asp:Label>
                    </td>
                    <td class=summaryDataEnd>
                        <telerik:RadComboBox ID="ddlViewList" runat="server" Skin="Metro" Width=650px  Font-Size=Small autopostback="True" onselectedindexchanged="ddlViewList_Select"></telerik:RadComboBox>
                        <asp:Button id="btnNewView" runat="server" Text="<%$ Resources:LocalizedText, New %>" visible="False" onClick="onViewLayoutClick" CssClass="buttonStd" style="margin-left: 10px;" CommandArgument="new"/>
                        <asp:Button id="btnViewLayout" runat="server" Text="Layout" visible="False" enabled="False" onClick="onViewLayoutClick" CssClass="buttonStd" CommandArgument="edit" meta:resourcekey="btnViewLayoutResource1"/>
                    </td>
                </tr>
                <tr>
                    <td class=summaryDataEnd width="90px">
                        <asp:Label ID="lblPlantSelect" runat="server" CssClass="prompt"></asp:Label>
                    </td>
                     <td class=summaryDataEnd>
                        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" HorizontalAlign="NotSet">
                            <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex=9000 Skin="Metro" height="350px" Width="650px" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>
                        </telerik:RadAjaxPanel>
                    </td>
                </tr>
                <tr>
                    <td class=summaryDataEnd width="90px">
                        <asp:Label runat="server" ID="lblDateSpan" CssClass="prompt" Text="<%$ Resources:LocalizedText, DateSpan %>"></asp:Label>
                    </td>
                    <td class=summaryDataEnd>
                       <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2" HorizontalAlign="NotSet">
                            <telerik:RadComboBox ID="ddlDateSpan" runat="server" Skin="Metro" Width=250px Font-Size=Small AutoPostBack="True" OnSelectedIndexChanged="ddlDateSpanChange">
                                <Items>
                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, SelectRange %>" Value="0" runat="server"/>
                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, YearToDate %>" Value="1" runat="server" />
                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, FYYearToDate %>" Value="4" runat="server" />
                                    <telerik:RadComboBoxItem Text="FY Year Over Year" Value="5" runat="server" meta:resourcekey="RadComboBoxItemResource4" />
                                    <telerik:RadComboBoxItem Text="FY Metrics Effective Range" Value="6" runat="server" meta:resourcekey="RadComboBoxItemResource5"/>
                                </Items>
                            </telerik:RadComboBox>
                            <span style="margin-left: 5px;">
                                <asp:PlaceHolder ID="phPeriodSpan" runat="server">
                                    <asp:Label runat="server" ID="lblPeriodFrom" CssClass="prompt"></asp:Label>
                                    <telerik:RadMonthYearPicker ID="dmPeriodFrom" runat="server" CssClass="textStd" Width=155px Skin="Metro">
										<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" Font-Size="Small" LabelWidth="64px" Skin="Metro" Width="">
											<EmptyMessageStyle Resize="None" />
											<ReadOnlyStyle Resize="None" />
											<FocusedStyle Resize="None" />
											<DisabledStyle Resize="None" />
											<InvalidStyle Resize="None" />
											<HoveredStyle Resize="None" />
											<EnabledStyle Resize="None" />
										</DateInput>
										<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
										<MonthYearNavigationSettings DateIsOutOfRangeMessage="<%$ Resources:LocalizedText, Cancel %>" />
									</telerik:RadMonthYearPicker>
                                    <asp:Label runat="server" ID="lblPeriodTo" CssClass="prompt" style="margin-left: 5px;"></asp:Label>
                                    <telerik:RadMonthYearPicker ID="dmPeriodTo" runat="server" CssClass="textStd" Width=155px Skin="Metro">
										<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" Font-Size="Small" LabelWidth="64px" Skin="Metro" Width="">
											<EmptyMessageStyle Resize="None" />
											<ReadOnlyStyle Resize="None" />
											<FocusedStyle Resize="None" />
											<DisabledStyle Resize="None" />
											<InvalidStyle Resize="None" />
											<HoveredStyle Resize="None" />
											<EnabledStyle Resize="None" />
										</DateInput>
										<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
										<MonthYearNavigationSettings DateIsOutOfRangeMessage="<%$ Resources:LocalizedText, Cancel %>" />
									</telerik:RadMonthYearPicker>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phYearSpan" runat="server">
                                    <asp:Label runat="server" ID="lblYearFrom" CssClass="prompt"></asp:Label>
                                    <telerik:RadComboBox ID="ddlYearFrom" runat="server" Skin="Metro" Width=80px Font-Size=Small></telerik:RadComboBox>
                                    <asp:Label runat="server" ID="lblYearTo" CssClass="prompt" style="margin-left: 5px;"></asp:Label>
                                    <telerik:RadComboBox ID="ddlYearTo" runat="server" Skin="Metro" Width=80px Font-Size=Small></telerik:RadComboBox>
                                </asp:PlaceHolder>
                                    <asp:Label runat="server" ID="lblDateInterval" CssClass="prompt" Text="Interval:" style="margin-left: 5px;" Visible="False" meta:resourcekey="lblDateIntervalResource1"></asp:Label>
                                 <telerik:RadComboBox ID="ddlDateInterval" runat="server" Visible="False" Width=120px Skin="Metro" Font-Size=Small ToolTip="Select interval for charts calculating points by date" meta:resourcekey="ddlDateIntervalResource1">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Default" Value="0" runat="server" meta:resourcekey="RadComboBoxItemResource6"/>
                                    <telerik:RadComboBoxItem Text="By Year" Value="1" runat="server" meta:resourcekey="RadComboBoxItemResource7"/>
                                    <telerik:RadComboBoxItem Text="By Month" Value="2" runat="server" meta:resourcekey="RadComboBoxItemResource8"/>
                                </Items>
                            </telerik:RadComboBox>
                            </span>
                           <span style="float: right; margin-right: 20px;" class="noprint">
                                <asp:LinkButton ID="btnRefreshDashboard" runat="server" ToolTip="Refresh the dashboard display" CSSClass="buttonRefresh"  text="Refresh View"  OnClick="btnRefreshDashboard_Click" meta:resourcekey="btnRefreshDashboardResource1"></asp:LinkButton>
                                <asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="<%$ Resources:LocalizedText, Print %>" style="margin-left: 5px;" OnClientClick="javascript:window.print()"></asp:LinkButton>
                                <asp:LinkButton  ID="lnkExport" runat="server" Text="<%$ Resources:LocalizedText, Export %>" ToolTip="<%$ Resources:LocalizedText, ExportDataToExcelFormat %>" CssClass="buttonDownload" style="margin-left: 5px;" OnClick="lnkExportClick"></asp:LinkButton>
                            </span>
                         </telerik:RadAjaxPanel>
                    </td>
                </tr>
                <tr id="trViewOptions" runat="server" visible="False">
                    <td class=summaryDataEnd width="90px" runat="server">
                        <asp:Label ID="lblOptions" runat="server" Text="Options:" CssClass="prompt"></asp:Label>
                    </td>
                    <td class=summaryDataEnd runat="server">
                        <telerik:RadComboBox ID="ddlOptions" runat="server" CheckBoxes="True" Width=160px  CheckedItemsTexts="DisplayAllInInput" ZIndex=9000 Skin="Metro" EmptyMessage="Select Display Options" >
                            <Items>
                                <telerik:RadComboBoxItem Text="Display Totals Only" Value="2" runat="server"/>
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
            <telerik:RadAjaxPanel ID="radAjaxMessages" runat="server" HorizontalAlign="NotSet">
                <asp:Label ID="lblWorking" runat="server" Text="Loading view - please wait..." CssClass="labelEmphasis" visible="False" meta:resourcekey="lblWorkingResource1"></asp:Label>
                <asp:Label ID="lblViewLoadError" runat="server" Text="An error occured attemtping to open the selected view." CssClass="labelEmphasis" visible="False" meta:resourcekey="lblViewLoadErrorResource1"></asp:Label>
                <asp:Label ID="LblViewSaveError" runat="server" Text="An error occured while saving your changes." CssClass="labelEmphasis" visible="False" meta:resourcekey="LblViewSaveErrorResource1"></asp:Label>
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
        <asp:Label ID="lblDashboardTitle" runat="server" CssClass="refText" style="display:block; margin-bottom: 10px;"></asp:Label>
    </div>

    <div id="divLayoutArea" runat="server" visible="False" style="margin-top: 10px; margin-bottom: 10px;">
        <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
            <tr>
                <td class="columnHeader" width="24%">
                    <asp:Label ID="lblViewName" runat="server" text="View Name" meta:resourcekey="lblViewNameResource1"></asp:Label>
                </td>
                <td class="required" width="1%">&nbsp;</td>
                <td class="tableDataAlt textStd" width="75%">
                    <telerik:RadTextBox ID="tbViewName" runat="server" Columns=60 Skin="Metro" MaxLength=100 Font-Size=Small LabelCssClass="" LabelWidth="64px" Resize="None">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</telerik:RadTextBox>
                    <span style="float:right; margin-right: 5px;">
                        <asp:Button id="btnTestView" runat="server" CssClass="buttonStd" text="Preview" OnClick="onTestViewClick" Visible=False meta:resourcekey="btnTestViewResource1"/>
                        <asp:Button id="btnViewCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" onClick="onViewEditClick" CssClass="buttonStd" Visible=False CommandArgument="cancel" />
                        <asp:Button ID="btnViewSave" runat="server" Text="Save View" OnClientClick="return confirmChange('View');" onClick="onViewEditClick" CssClass="buttonEmphasis" Visible=False CommandArgument = "save" meta:resourcekey="btnViewSaveResource1"/>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="lblViewDesc" runat="server" text="Title" meta:resourcekey="lblViewDescResource1"></asp:Label>
                </td>
                <td class="tableDataAlt">&nbsp;</td>
                <td class="tableDataAlt">
                    <telerik:RadTextBox ID="tbViewDesc" runat="server" Columns=60 Skin="Metro" MaxLength=400 Font-Size=Small LabelCssClass="" LabelWidth="64px" Resize="None">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="Label1" runat="server" text="Perspective" meta:resourcekey="Label1Resource1"></asp:Label>
                </td>
                <td class="required">&nbsp;</td>
                <td class="tableDataAlt">
                    <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel3" HorizontalAlign="NotSet">
                        <telerik:RadComboBox ID="ddlPerspective" runat="server" Skin="Metro" Width=250px ZIndex="9000" Font-Size=Small AutoPostBack="True" OnSelectedIndexChanged="ddlPerspectiveChange">
                            <Items>
                                    <telerik:RadComboBoxItem Text="Environmental Metrics" Value="E" runat="server" meta:resourcekey="RadComboBoxItemResource9" />
                                    <telerik:RadComboBoxItem Text="Environmental YTD Performance" Value="EP" runat="server" meta:resourcekey="RadComboBoxItemResource10" />
                                    <telerik:RadComboBoxItem Text="Health &amp; Safety" Value="HS" runat="server" meta:resourcekey="RadComboBoxItemResource11" />
                                    <telerik:RadComboBoxItem Text="Health &amp; Safety YTD Performance" Value="HSP" runat="server" meta:resourcekey="RadComboBoxItemResource12" />
                                    <telerik:RadComboBoxItem Text="Quality" Value="QS" runat="server" meta:resourcekey="RadComboBoxItemResource13" />
                            </Items>
                        </telerik:RadComboBox>
                    </telerik:RadAjaxPanel>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="lblViewAvailability" runat="server" text="Availability" meta:resourcekey="lblViewAvailabilityResource1"></asp:Label>
                </td>
                <td class="tableDataAlt">&nbsp;</td>
                <td class="tableDataAlt">
                    <telerik:RadComboBox id="ddlViewAvailability" runat="server" Skin="Metro" Font-Size=Small Width=300px>
                        <Items>
                            <telerik:RadComboBoxItem Text="Owner" Value="1" runat="server" meta:resourcekey="RadComboBoxItemResource14" />
                            <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Plant %>" Value="2" runat="server" />
                            <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, BusinessOrg %>" Value="3" runat="server" />
                            <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Company %>" Value="4" runat="server" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="columnHeader">
                    <asp:Label ID="lblDfltParams" runat="server" text="Display Criteria" meta:resourcekey="lblDfltParamsResource1"></asp:Label>
                </td>
                <td class="required">&nbsp;</td>
                <td class="tableDataAlt textStd">
                        <telerik:RadComboBox id="ddlDfltCriteria" runat="server" Skin="Metro" Font-Size=Small Width=300px>
                        <Items>
                            <telerik:RadComboBoxItem Text="No Defaults" Value="0" runat="server" meta:resourcekey="RadComboBoxItemResource18" />
                            <telerik:RadComboBoxItem Text="Use Current / Allow Override" Value="1" runat="server" meta:resourcekey="RadComboBoxItemResource19"/>
                            <telerik:RadComboBoxItem Text="Use Current / No Override" Value="2" runat="server" meta:resourcekey="RadComboBoxItemResource20"/>
                            <telerik:RadComboBoxItem Text="Select Locations" Value="3" runat="server" meta:resourcekey="RadComboBoxItemResource21" />
                            <telerik:RadComboBoxItem Text="Select Time Span" Value="4" runat="server" meta:resourcekey="RadComboBoxItemResource22" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr height="22px;">
                <td class="columnHeader">
                    <asp:Label ID="lblLastUpdate" runat="server" text="Last Updated By" meta:resourcekey="lblLastUpdateResource1"></asp:Label>
                </td>
                <td class="tableDataAlt">&nbsp;</td>
                <td class="tableDataAlt">
                    <asp:Label id="lblLastUpdate_out" runat="server" CssClass="textStd"></asp:Label>
                </td>
            </tr>
        </table>
            <div class="borderSoft" style="margin-top: 3px;">
                <asp:Button id="btnAddChart1" runat="server" Text="Add Chart" OnClick="onAddChartClick" meta:resourcekey="btnAddChartResource1"/>
                <asp:Repeater runat="server" ID="rptViewItem" ClientIDMode="AutoID" OnItemDataBound="rptViewItem_OnItemDataBound">
			    <FooterTemplate>
			    </table></FooterTemplate>
		    		<HeaderTemplate>
						<table border="0" cellpadding="1" cellspacing="0" width="100%">
						</table>
					</HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td width="95px;">
								<asp:Label ID="lblVISeq" runat="server" CssClass="prompt" meta:resourcekey="lblVISeqResource1" Text="Chart"></asp:Label>
							</td>
							<td>
								<telerik:RadAjaxPanel ID="radPnlGraph" runat="server" HorizontalAlign="NotSet">
									<telerik:RadComboBox ID="ddlVISeq" runat="server" Font-Size="Small" Height="300px" meta:resourcekey="ddlVISeqResource1" Skin="Metro" ToolTip="Display sequence" Width="75px" ZIndex="9000">
										<Items>
											<telerik:RadComboBoxItem runat="server" Text="<%$ Resources:LocalizedText, Delete %>" Value="0" />
										</Items>
									</telerik:RadComboBox>
									<telerik:RadComboBox ID="ddlVIGaugeType" runat="server" EmptyMessage="Chart type" Font-Size="Small" meta:resourcekey="ddlVIGaugeTypeResource1" Skin="Metro" ToolTip="Select chart or gauge type">
										<Items>
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource24" Text="Section Area" Value="1" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource25" Text="Vernier" Value="10" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource26" Text="Vernier Array" Value="210" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource27" Text="Bar Graph" Value="11" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource28" Text="Stacked-Bar Graph" Value="12" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource29" Text="Bar Pareto Chart" Value="15" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource30" Text="Column Gauge" Value="20" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource31" Text="Column Gauge Array" Value="220" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource32" Text="Column Chart" Value="21" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource33" Text="Stacked-Column Chart" Value="22" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource34" Text="Grouped-Column Chart" Value="23" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource35" Text="Column Pareto Chart" Value="25" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource36" Text="Line Chart" Value="32" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource37" Text="Line Chart Array" Value="232" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource38" Text="Pie Chart" Value="50" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource39" Text="Pie Chart Array" Value="250" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource40" Text="Radial Gauge" Value="60" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource41" Text="Radial Gauge Array" Value="260" />
										</Items>
									</telerik:RadComboBox>
									<telerik:RadTextBox ID="tbVIHeight" runat="server" Columns="4" EmptyMessage="Height" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="5" meta:resourcekey="tbVIHeightResource1" Resize="None" Skin="Metro" ToolTip="Height">
										<EmptyMessageStyle Resize="None" />
										<ReadOnlyStyle Resize="None" />
										<FocusedStyle Resize="None" />
										<DisabledStyle Resize="None" />
										<InvalidStyle Resize="None" />
										<HoveredStyle Resize="None" />
										<EnabledStyle Resize="None" />
									</telerik:RadTextBox>
									<asp:Label ID="lblX" runat="server" CssClass="textSmall" Text="x"></asp:Label>
									<telerik:RadTextBox ID="tbVIWidth" runat="server" Columns="4" EmptyMessage="Width" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="5" meta:resourcekey="tbVIWidthResource1" Resize="None" Skin="Metro" ToolTip="Width">
										<EmptyMessageStyle Resize="None" />
										<ReadOnlyStyle Resize="None" />
										<FocusedStyle Resize="None" />
										<DisabledStyle Resize="None" />
										<InvalidStyle Resize="None" />
										<HoveredStyle Resize="None" />
										<EnabledStyle Resize="None" />
									</telerik:RadTextBox>
									<telerik:RadButton ID="cbVINewRow" runat="server" ButtonType="ToggleButton" CssClass="textSmall" meta:resourcekey="cbVINewRowResource1" Text="Start New Row" ToggleType="CheckBox">
									</telerik:RadButton>
								</telerik:RadAjaxPanel>
							</td>
						</tr>
						<tr>
							<td width="95px;">&nbsp;</td>
							<td>
								<telerik:RadTextBox ID="tbVITitle" runat="server" Columns="60" EmptyMessage="chart title" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="100" Resize="None" Skin="Metro">
									<EmptyMessageStyle Resize="None" />
									<ReadOnlyStyle Resize="None" />
									<FocusedStyle Resize="None" />
									<DisabledStyle Resize="None" />
									<InvalidStyle Resize="None" />
									<HoveredStyle Resize="None" />
									<EnabledStyle Resize="None" />
								</telerik:RadTextBox>
							</td>
						</tr>
						<tr>
							<td width="95px;">
								<asp:Label ID="lblVIScope" runat="server" CssClass="prompt" Text="<%$ Resources:LocalizedText, Data %>"></asp:Label>
							</td>
							<td>
								<telerik:RadAjaxPanel ID="radPnlScope" runat="server" HorizontalAlign="NotSet">
									<telerik:RadComboBox ID="ddlVIStat" runat="server" AutoPostBack="True" EmptyMessage="Statistic" Font-Size="Small" meta:resourcekey="ddlVIStatResource1" OnSelectedIndexChanged="ddlVIStatChange" Skin="Metro" ToolTip="Select metric to display">
										<Items>
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource42" Text="Total" Value="sum" />
											<telerik:RadComboBoxItem runat="server" Text="<%$ Resources:LocalizedText, Cost %>" Value="sumCost" />
											<telerik:RadComboBoxItem runat="server" Text="<%$ Resources:LocalizedText, PercentChange %>" Value="pctChange" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource45" Text="Days Elapsed" Value="deltaDy" />
										</Items>
									</telerik:RadComboBox>
									<asp:Label ID="lblOf" runat="server" CssClass="textSmall" meta:resourcekey="lblOfResource1" Text="of"></asp:Label>
									<telerik:RadComboBox ID="ddlVIScope" runat="server" CheckBoxes="True" EmptyMessage="For metric or category" Font-Size="Small" Height="350px" meta:resourcekey="ddlVIScopeResource1" Skin="Metro" ToolTip="Select metrics by category, type or specific measure" Width="250px" ZIndex="9000">
									</telerik:RadComboBox>
									<telerik:RadComboBox ID="ddlVISeriesOrder" runat="server" EmptyMessag="order by" Font-Size="Small" meta:resourcekey="ddlVISeriesOrderResource1" Skin="Metro" ToolTip="Data series ordering">
										<Items>
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource46" Text="Sum All" Value="0" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource47" Text="Order By Measure-Location" Value="1" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource48" Text="Order By Location-Measure" Value="2" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource49" Text="Order By Period-Location" Value="3" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource50" Text="Order By Period-Measure" Value="4" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource51" Text="Order By Year-Location" Value="5" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource52" Text="Order By Year-Measure" Value="6" />
										</Items>
									</telerik:RadComboBox>
								</telerik:RadAjaxPanel>
							</td>
						</tr>
						<tr>
							<td width="95px;">
								<asp:Label ID="lblVIXAxis" runat="server" CssClass="prompt" meta:resourcekey="lblVIXAxisResource1" Text="Variable Axis"></asp:Label>
							</td>
							<td>
								<telerik:RadAjaxPanel ID="radPnlXAxis" runat="server" HorizontalAlign="NotSet">
									<telerik:RadComboBox ID="ddlVIXAxisScale" runat="server" AutoPostBack="True" EmptyMessag="set scale" Font-Size="Small" meta:resourcekey="ddlVIXAxisScaleResource1" OnSelectedIndexChanged="ddlVIScaleChange" Skin="Metro" ToolTip="Choose auto scaling or enter min/max scale values" Width="120px">
										<Items>
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource53" Text="Auto Scale" Value="0" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource54" Text="Set Scales" Value="1" />
										</Items>
									</telerik:RadComboBox>
									<telerik:RadTextBox ID="tbVIXAxisMin" runat="server" Columns="9" EmptyMessage="Scale min" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="20" meta:resourcekey="tbVIXAxisMinResource1" Resize="None" Skin="Metro" ToolTip="Axis minimum or orign value">
										<EmptyMessageStyle Resize="None" />
										<ReadOnlyStyle Resize="None" />
										<FocusedStyle Resize="None" />
										<DisabledStyle Resize="None" />
										<InvalidStyle Resize="None" />
										<HoveredStyle Resize="None" />
										<EnabledStyle Resize="None" />
									</telerik:RadTextBox>
									<asp:Label ID="lblXTo" runat="server" CssClass="textSmall" meta:resourcekey="lblXToResource1" Text="to"></asp:Label>
									<telerik:RadTextBox ID="tbVIXAxisMax" runat="server" Columns="9" EmptyMessage="Scale max" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="20" meta:resourcekey="tbVIXAxisMaxResource1" Resize="None" Skin="Metro" ToolTip="Axis maximum value">
										<EmptyMessageStyle Resize="None" />
										<ReadOnlyStyle Resize="None" />
										<FocusedStyle Resize="None" />
										<DisabledStyle Resize="None" />
										<InvalidStyle Resize="None" />
										<HoveredStyle Resize="None" />
										<EnabledStyle Resize="None" />
									</telerik:RadTextBox>
									<asp:Label ID="lblXBy" runat="server" CssClass="textSmall" meta:resourcekey="lblXByResource1" Text="by"></asp:Label>
									<telerik:RadTextBox ID="tbVIXAxisUnit" runat="server" Columns="9" EmptyMessage="Scale unit" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="20" meta:resourcekey="tbVIXAxisUnitResource1" Resize="None" Skin="Metro" ToolTip="Scale unit">
										<EmptyMessageStyle Resize="None" />
										<ReadOnlyStyle Resize="None" />
										<FocusedStyle Resize="None" />
										<DisabledStyle Resize="None" />
										<InvalidStyle Resize="None" />
										<HoveredStyle Resize="None" />
										<EnabledStyle Resize="None" />
									</telerik:RadTextBox>
									<asp:Label ID="lblTarget" runat="server" CssClass="textSmall"></asp:Label>
									<telerik:RadComboBox ID="ddlVITarget" runat="server" EmptyMessag="overlay target" Font-Size="Small" meta:resourcekey="ddlVITargetResource1" Skin="Metro" ToolTip="Overlay metric target" ZIndex="9000">
										<Items>
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource55" Text="None" Value="0" />
											<telerik:RadComboBoxItem runat="server" Text="<%$ Resources:LocalizedText, TargetValue %>" Value="1" />
											<telerik:RadComboBoxItem runat="server" meta:resourcekey="RadComboBoxItemResource57" Text="Current Value" Value="1" />
										</Items>
									</telerik:RadComboBox>
								</telerik:RadAjaxPanel>
							</td>
						</tr>
						<tr>
							<td width="95px;">&nbsp;</td>
							<td>
								<telerik:RadTextBox ID="tbVIXAxisLabel" runat="server" Columns="60" EmptyMessage="Axis label" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="200" meta:resourcekey="tbVIXAxisLabelResource1" Resize="None" Skin="Metro" ToolTip="Variable axis label">
									<EmptyMessageStyle Resize="None" />
									<ReadOnlyStyle Resize="None" />
									<FocusedStyle Resize="None" />
									<DisabledStyle Resize="None" />
									<InvalidStyle Resize="None" />
									<HoveredStyle Resize="None" />
									<EnabledStyle Resize="None" />
								</telerik:RadTextBox>
							</td>
						</tr>
						<tr id="trVIYAxis" runat="server">
							<td runat="server" width="95px;">
								<asp:Label ID="lblVIYAxis" runat="server" CssClass="prompt" Text="Ordinal Axis"></asp:Label>
							</td>
							<td runat="server">
								<telerik:RadTextBox ID="tbVIYAxisLabel" runat="server" Columns="60" EmptyMessage="axis label" Font-Size="Small" LabelCssClass="" LabelWidth="64px" MaxLength="200" Resize="None" Skin="Metro" ToolTip="ordinal label">
									<EmptyMessageStyle Resize="None" />
									<ReadOnlyStyle Resize="None" />
									<FocusedStyle Resize="None" />
									<DisabledStyle Resize="None" />
									<InvalidStyle Resize="None" />
									<HoveredStyle Resize="None" />
									<EnabledStyle Resize="None" />
								</telerik:RadTextBox>
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<hr color="#DCDCDC" size="1" width="99%"></hr>
							</td>
						</tr>
					</ItemTemplate>
		    </asp:Repeater>
            <asp:Button id="btnAddChart2" runat="server" Text="Add Chart" OnClick="onAddChartClick" meta:resourcekey="btnAddChartResource1"/>
        </div>
    </div>
    <div id="divExport" runat="server">
         <Ucl:Export id="uclExport" runat="server"/>
    </div>
</asp:Panel>

<asp:Panel ID="pnlStats" runat="server" Visible="False">
    <div>
            <div style="margin-top: 5px;">
                <asp:Label id="lblStats" runat="server" CssClass="prompt"></asp:Label>
            </div>
            <div id="divStatsArea" runat="server" style ="width: 850px; overflow-x: scroll; overflow:auto; margin-top: 4px;">
            </div>
    </div>
</asp:Panel>
