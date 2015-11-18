<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_PerformanceReport.aspx.cs" Inherits="SQM.Website.EHS.EHS_PerformanceReport" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="SQMWebsite" Namespace="SQM.Website" TagPrefix="SQM" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.exportButtonDiv
		{
			position: absolute;
			right: 10px;
			top: 50%;
			-moz-transform: translateY(-50%);
			-ms-transform: translateY(-50%);
			-o-transform: translateY(-50%);
			-webkit-transform: translateY(-50%);
			transform: translateY(-50%);
		}
		.yearDiv > div
		{
			display: inline-block;
		}

		.pyramidTable_header
		{
			background-color: #d9d9d9;
			height: 50px;
			text-align: center;
			width: 100px;
		}
		.pyramidTable_filler
		{
			border-bottom: 1px solid #000;
		}
		.pyramidTable_cell
		{
			border: 1px solid #000;
			text-align: center;
		}
		.pyramidTable_varianceGood
		{
			background-color: #060;
			color: #fff;
		}
		.pyramidTable_varianceBad
		{
			background-color: #f00;
			color: #fff;
		}

		.rgCaption
		{
			color: #000;
			font-size: x-large;
			font-weight: bold;
			margin-bottom: 8px;
			margin-top: 8px;
			text-align: center;
		}
		.RadGrid_Metro .rgRow > td
		{
			border-color: #e5e5e5 !important;
		}
		div[id$="rgReport"] .rgMasterTable tr.rgMultiHeaderRow:nth-child(2) .rgHeader
		{
			background-color: #ff9 !important;
		}
		.RadHtmlChart, .pieChart
		{
			border: 1px solid #000;
		}
		.chartMarginTop
		{
			margin-top: 1em;
		}

		#divExport
		{
			margin: 0 auto;
		}

		/* Comes from http://www.telerik.com/forums/how-to-display-only-years-in-raddatepicker-or-raddatetimepicker-or-radmonthyearpicker#jJQg4dGOfE2zmjC1lVnQhw */
		#rcMView_Jan, #rcMView_Feb, #rcMView_Mar, #rcMView_Apr, #rcMView_May, #rcMView_Jun, #rcMView_Jul, #rcMView_Aug, #rcMView_Sep, #rcMView_Oct, #rcMView_Nov, #rcMView_Dec
		{
			display: none;
		}

		/* Simulate the overridden Metro style, as Telerik's RadButton was acting up and not always registering clicks for me. */
		.myButton
		{
			background-color: #191970;
			border: 0 none;
			border-radius: 4px;
			color: #fff;
			font-family: Verdana, Arial, Helvetica, sans-serif;
			font-size: 12px;
			font-weight: bold;
			height: 32px;
			padding: 6px 24px;
			text-align: center;
		}
		.myButton:hover
		{
			opacity: 0.6;
		}
	</style>
	<link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/themes/smoothness/jquery-ui.css" type="text/css" />
	<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/jquery-ui.min.js" type="text/javascript"></script>
	<script type="text/javascript" src="../scripts/innersvg.js"></script>
	<script type="text/javascript" src="../scripts/jquery.copycss.js"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<asp:HiddenField ID="hfCompanyID" runat="server" />
	<div class="container-fluid">
		<span class="pageTitles" style="float: left; margin-top: 6px">Health & Safety Performance Report</span>
		<br class="clear" /><br />
		<telerik:RadAjaxLoadingPanel ID="radLoading" runat="server" Skin="Metro" />
		<telerik:RadAjaxPanel ID="radAjaxPanel" runat="server" LoadingPanelID="radLoading">
			<div class="container-fluid blueCell" style="position: relative">
				<span class="prompt">Report Type: </span>
				<telerik:RadDropDownList ID="rddlType" runat="server" Skin="Metro" Width="250" OnClientItemSelected="rddlType_ClientItemSelected">
					<Items>
						<telerik:DropDownListItem Text="Pyramid" Value="Pyramid" />
						<telerik:DropDownListItem Text="TRIR By Business Unit" Value="TRIRBusiness" />
						<telerik:DropDownListItem Text="TRIR Comparison By Plant" Value="TRIRPlant" />
						<telerik:DropDownListItem Text="Recordable Comparison By Plant" Value="RecPlant" />
						<telerik:DropDownListItem Text="Balanced Scorecard" Value="BalancedScordcard" />
						<telerik:DropDownListItem Text="Metrics" Value="Metrics" Selected="true" />
					</Items>
				</telerik:RadDropDownList>
				<br /><br />
				<asp:Panel ID="pnlMetrics" runat="server">
					<telerik:RadComboBox ID="rcbPlant" runat="server" Skin="Metro" Height="350" Width="400" CausesValidation="false" />
					<br /><br />
				</asp:Panel>
				<div class="yearDiv">
					<span class="prompt">Year: </span>
					<telerik:RadMonthYearPicker ID="rmypYear" runat="server" Skin="Metro" DateInput-Skin="Metro" ShowPopupOnFocus="true" DateInput-CausesValidation="false"
						DateInput-DateFormat="yyyy" DateInput-DisplayDateFormat="yyyy" />
					<telerik:RadButton ID="btnRefresh" runat="server" Text="<%$ Resources:RadGrid.Main, Refresh %>" Skin="Metro" OnClick="btnRefresh_Click" />
				</div>
				<div class="exportButtonDiv">
					<input type="button" id="btnExport" runat="server" value="<%$ Resources:RadGrid.Main, ExportToPdfText %>" class="myButton" />
				</div>
			</div>
			<br />
			<div id="divExport" runat="server">
				<asp:Panel ID="pnlPyramidOutput" runat="server">
					<div style="position: relative; height: 650px">
						<SQM:AAMPyramidChart ID="pyramid" runat="server" Height="600" style="position: absolute; top: 50px; z-index: 1" />
						<table id="pyramidTable" runat="server" style="position: relative; top: 0; z-index: 0">
							<thead>
								<tr>
									<th id="pyramidTable_column1" runat="server"></th>
									<th class="pyramidTable_header" style="border: 1px solid #000">YTD</th>
									<th id="pyramidTable_columnAnnualized" runat="server" class="pyramidTable_header" style="border: 1px solid #000"></th>
									<th id="pyramidTable_columnPreviousYear" runat="server" class="pyramidTable_header" style="border: 1px solid #000"></th>
									<th class="pyramidTable_header" style="border: 1px solid #000">Variance</th>
								</tr>
							</thead>
							<tbody>
								<tr id="pyramidTable_fatalitiesRow" runat="server">
									<td class="pyramidTable_filler"></td>
									<td id="pyramidTable_fatalitiesYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_fatalitiesAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_fatalitiesPreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_fatalitiesVariance" runat="server" style="border: 1px solid #000"></td>
								</tr>
								<tr id="pyramidTable_lostTimeRow" runat="server">
									<td class="pyramidTable_filler"></td>
									<td id="pyramidTable_lostTimeYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_lostTimeAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_lostTimePreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_lostTimeVariance" runat="server" style="border: 1px solid #000"></td>
								</tr>
								<tr id="pyramidTable_recordableRow" runat="server">
									<td class="pyramidTable_filler"></td>
									<td id="pyramidTable_recordableYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_recordableAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_recordablePreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_recordableVariance" runat="server" style="border: 1px solid #000"></td>
								</tr>
								<tr id="pyramidTable_firstAidRow" runat="server">
									<td class="pyramidTable_filler"></td>
									<td id="pyramidTable_firstAidYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_firstAidAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_firstAidPreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_firstAidVariance" runat="server" style="border: 1px solid #000"></td>
								</tr>
								<tr id="pyramidTable_nearMissesRow" runat="server">
									<td class="pyramidTable_filler"></td>
									<td id="pyramidTable_nearMissesYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_nearMissesAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_nearMissesPreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
									<td id="pyramidTable_nearMissesVariance" runat="server" style="border: 1px solid #000"></td>
								</tr>
							</tbody>
						</table>
					</div>
					<div style="page-break-after: always; padding: 1px"></div>
					<div id="divJSAsAndAudits_Pyramid" runat="server" class="chartMarginTop"></div>
					<div id="divSafetyTrainingHours_Pyramid" runat="server" class="chartMarginTop"></div>
				</asp:Panel>
				<asp:Panel ID="pnlTRIRBusinessOutput" runat="server" />
				<asp:Panel ID="pnlTRIRPlantOutput" runat="server">
					<telerik:RadGrid ID="rgTRIRPlant" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500" OnItemDataBound="rgTRIRPlant_ItemDataBound"
						OnPreRender="rgTRIRPlant_PreRender">
						<ClientSettings EnableAlternatingItems="false" />
						<MasterTableView Caption="TRIR Comparison by Plant">
							<ColumnGroups>
								<telerik:GridColumnGroup Name="PerformanceResults" HeaderText="Performance Results">
									<HeaderStyle HorizontalAlign="Center" />
								</telerik:GridColumnGroup>
							</ColumnGroups>
							<Columns>
								<telerik:GridBoundColumn DataField="BusinessUnit" HeaderText="BU" UniqueName="BusinessUnit">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="Plant" HeaderText="Plant" UniqueName="Plant">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="TRIRGoal" HeaderText="TRIR Goal">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="TRIR2YearsAgo" DataFormatString="{0:F1}" UniqueName="TRIR2YearsAgo">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="TRIRPreviousYear" DataFormatString="{0:F1}" UniqueName="TRIRPreviousYear">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="TRIRYTD" DataFormatString="{0:F1}" UniqueName="TRIRYTD">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn UniqueName="ImprovedOrDeclined" ColumnGroupName="PerformanceResults">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle Font-Bold="true" Font-Size="18px" HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="PercentChange" DataFormatString="{0:P1}" UniqueName="PercentChange" ColumnGroupName="PerformanceResults">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="ProgressToGoal" DataFormatString="{0:P1}" HeaderText="Progress<br/>to Goal" UniqueName="ProgressToGoal"
									ColumnGroupName="PerformanceResults">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>
				</asp:Panel>
				<asp:Panel ID="pnlRecPlantOutput" runat="server">
					<telerik:RadGrid ID="rgRecPlant" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500" OnItemDataBound="rgTRIRPlant_ItemDataBound"
						OnPreRender="rgTRIRPlant_PreRender">
						<ClientSettings EnableAlternatingItems="false" />
						<MasterTableView Caption="Recordable Comparison by Plant">
							<ColumnGroups>
								<telerik:GridColumnGroup Name="PerformanceResults" HeaderText="Performance Results">
									<HeaderStyle HorizontalAlign="Center" />
								</telerik:GridColumnGroup>
							</ColumnGroups>
							<Columns>
								<telerik:GridBoundColumn DataField="BusinessUnit" HeaderText="BU" UniqueName="BusinessUnit">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="Plant" HeaderText="Plant" UniqueName="Plant">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="RecPreviousYear" UniqueName="RecPreviousYear">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="RecYTD" UniqueName="RecYTD">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="RecAnnualized" HeaderText="Recordabled<br/>Annualized">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn UniqueName="ImprovedOrDeclined" ColumnGroupName="PerformanceResults">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle Font-Bold="true" Font-Size="18px" HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
								<telerik:GridBoundColumn DataField="PercentChange" DataFormatString="{0:P1}" HeaderText="% Change" UniqueName="PercentChange" ColumnGroupName="PerformanceResults">
									<HeaderStyle HorizontalAlign="Center" />
									<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
								</telerik:GridBoundColumn>
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>
				</asp:Panel>
				<asp:Panel ID="pnlBalancedScorecardOutput" runat="server">
					<asp:Repeater ID="rptBalancedScorecard" runat="server" OnItemDataBound="rptBalancedScorecard_ItemDataBound">
						<HeaderTemplate>
							<telerik:RadGrid ID="rgBalancedScorescardHeader" runat="server" Skin="Metro" AutoGenerateColumns="false" OnItemDataBound="rgBalancedScorescardHeader_ItemDataBound">
								<MasterTableView TableLayout="Fixed">
									<NoRecordsTemplate></NoRecordsTemplate>
									<ColumnGroups>
										<telerik:GridColumnGroup HeaderText="Above Target" Name="Target">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="Red" ForeColor="White" />
										</telerik:GridColumnGroup>
										<telerik:GridColumnGroup HeaderText="Year" Name="Year">
											<HeaderStyle Font-Size="X-Large" HorizontalAlign="Center" BackColor="White" />
										</telerik:GridColumnGroup>
									</ColumnGroups>
									<Columns>
										<telerik:GridBoundColumn HeaderText="Target or Better" UniqueName="Target" ColumnGroupName="Target">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="Green" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="JAN" UniqueName="Month1" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="FEB" UniqueName="Month2" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="MAR" UniqueName="Month3" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="APR" UniqueName="Month4" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="MAY" UniqueName="Month5" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="JUN" UniqueName="Month6" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="JUL" UniqueName="Month7" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="AUG" UniqueName="Month8" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="SEP" UniqueName="Month9" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="OCT" UniqueName="Month10" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="NOV" UniqueName="Month11" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="DEC" UniqueName="Month12" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn HeaderText="YTD" UniqueName="YTD" ColumnGroupName="Year">
											<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#FFFF99" Width="100" />
										</telerik:GridBoundColumn>
									</Columns>
								</MasterTableView>
							</telerik:RadGrid>
						</HeaderTemplate>
						<ItemTemplate>
							<telerik:RadGrid ID="rgBalancedScorecardItem" runat="server" Skin="Metro" AutoGenerateColumns="false" ShowHeader="false"
								OnItemDataBound="rgBalancedScorecardItem_ItemDataBound">
								<ClientSettings EnableAlternatingItems="false" />
								<MasterTableView TableLayout="Fixed">
									<Columns>
										<telerik:GridBoundColumn DataField="ItemType" UniqueName="ItemType" />
										<telerik:GridBoundColumn DataField="Target" DataFormatString="{0:F1}" UniqueName="Target">
											<HeaderStyle Width="50" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Jan" DataFormatString="{0:F1}" UniqueName="Month1">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Feb" DataFormatString="{0:F1}" UniqueName="Month2">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Mar" DataFormatString="{0:F1}" UniqueName="Month3">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Apr" DataFormatString="{0:F1}" UniqueName="Month4">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="May" DataFormatString="{0:F1}" UniqueName="Month5">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Jun" DataFormatString="{0:F1}" UniqueName="Month6">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Jul" DataFormatString="{0:F1}" UniqueName="Month7">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Aug" DataFormatString="{0:F1}" UniqueName="Month8">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Sep" DataFormatString="{0:F1}" UniqueName="Month9">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Oct" DataFormatString="{0:F1}" UniqueName="Month10">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Nov" DataFormatString="{0:F1}" UniqueName="Month11">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="Dec" DataFormatString="{0:F1}" UniqueName="Month12">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
										<telerik:GridBoundColumn DataField="YTD" DataFormatString="{0:F1}" UniqueName="YTD">
											<HeaderStyle Width="100" />
											<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
										</telerik:GridBoundColumn>
									</Columns>
								</MasterTableView>
							</telerik:RadGrid>
						</ItemTemplate>
						<SeparatorTemplate>
							<div style="height: 5px; background-color: #999; border: 1px solid #e5e5e5"></div>
						</SeparatorTemplate>
					</asp:Repeater>
				</asp:Panel>
				<asp:Panel ID="pnlMetricsOutput" runat="server">
					<telerik:RadGrid ID="rgReport" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500" OnItemDataBound="rgReport_ItemDataBound">
						<AlternatingItemStyle BackColor="Transparent" />
						<MasterTableView>
							<ColumnGroups>
								<telerik:GridColumnGroup Name="None1" />
								<telerik:GridColumnGroup Name="None2" />
								<telerik:GridColumnGroup Name="Incidents" HeaderText="Incidents" />
								<telerik:GridColumnGroup Name="Frequency" HeaderText="Frequency" />
								<telerik:GridColumnGroup Name="Restricted" HeaderText="Restricted" />
								<telerik:GridColumnGroup Name="Severity" HeaderText="Severity" />
							</ColumnGroups>
							<Columns>
								<telerik:GridBoundColumn DataField="Month" HeaderText="Year" />
								<telerik:GridBoundColumn DataField="TRIR" DataFormatString="{0:F1}" HeaderText="TRIR" ColumnGroupName="None1" />
								<telerik:GridBoundColumn DataField="FrequencyRate" DataFormatString="{0:F1}" HeaderText="Frequency<br>Rate" ColumnGroupName="None1" />
								<telerik:GridBoundColumn DataField="SeverityRate" DataFormatString="{0:F1}" HeaderText="Severity<br>Rate" ColumnGroupName="None1" />
								<telerik:GridBoundColumn DataField="ManHours" DataFormatString="{0:N0}" HeaderText="Man-hours" UniqueName="ManHours" ColumnGroupName="None1" />
								<telerik:GridBoundColumn DataField="Incidents" HeaderText="Total<br>Recordable<br>Cases" UniqueName="Incidents" ColumnGroupName="Incidents" />
								<telerik:GridBoundColumn DataField="Frequency" HeaderText="Total Lost<br>Time Cases" UniqueName="Frequency" ColumnGroupName="Frequency" />
								<telerik:GridBoundColumn DataField="Restricted" HeaderText="Total<br>Days<br>Restricted" UniqueName="Restricted" ColumnGroupName="Restricted" />
								<telerik:GridBoundColumn DataField="Severity" HeaderText="Total Lost<br>Time Days" UniqueName="Severity" ColumnGroupName="Severity" />
								<telerik:GridBoundColumn DataField="FirstAid" HeaderText="First Aid<br>Cases" UniqueName="FirstAid" ColumnGroupName="None2" />
								<telerik:GridBoundColumn DataField="Leadership" HeaderText="Leadership<br>Safety<br>Walks" UniqueName="Leadership" ColumnGroupName="None2" />
								<telerik:GridBoundColumn DataField="JSAs" HeaderText="JSAs<br>Completed" UniqueName="JSAs" ColumnGroupName="None2" />
								<telerik:GridBoundColumn DataField="SafetyTraining" HeaderText="Total Safety<br>Training<br>Hours" UniqueName="SafetyTraining" ColumnGroupName="None2" />
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>
					<div id="divTRIR" runat="server" class="chartMarginTop"></div>
					<div style="page-break-after: always; padding: 1px"></div>
					<div id="divFrequencyRate" runat="server" class="chartMarginTop"></div>
					<div id="divSeverityRate" runat="server" class="chartMarginTop"></div>
					<div style="page-break-after: always; padding: 1px"></div>
					<div id="divPie1" runat="server" style="overflow: hidden" class="chartMarginTop">
						<SQM:PieChart ID="pieRecordableType" runat="server" Title="Recordable Injuries by Type" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
						<SQM:PieChart ID="pieRecordableBodyPart" runat="server" Title="Recordable Injuries by Body Part" Width="740" Height="500" StartAngle="45" Style="float: right"
							CssClass="pieChart" />
					</div>
					<div id="divPie2" runat="server" style="overflow: hidden" class="chartMarginTop">
						<SQM:PieChart ID="pieRecordableRootCause" runat="server" Title="Injury Root Causes" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
						<SQM:PieChart ID="pieRecordableTenure" runat="server" Title="Tenure of Injured Associate" Width="740" Height="500" StartAngle="45" Style="float: right" CssClass="pieChart" />
					</div>
					<div id="divBreakPie" runat="server" style="page-break-after: always; padding: 1px"></div>
					<div id="divPie3" runat="server" style="overflow: hidden" class="chartMarginTop">
						<SQM:PieChart ID="pieRecordableDaysToClose" runat="server" Title="Days to Close Investigations" Width="740" Height="500" StartAngle="45" CssClass="pieChart" />
					</div>
					<div style="overflow: hidden" class="chartMarginTop">
						<div id="divJSAsAndAudits_Metrics" runat="server" style="float: left"></div>
						<div id="divSafetyTrainingHours_Metrics" runat="server" style="float: right"></div>
					</div>
				</asp:Panel>
			</div>
		</telerik:RadAjaxPanel>
	</div>
	<Ucl:RadGauge ID="uclChart" runat="server" />
	<telerik:RadCodeBlock ID="radCodeBlock" runat="server">
		<script type="text/javascript">
			function rddlType_ClientItemSelected(sender, eventArgs)
			{
				var item = eventArgs.get_item();
				var pnlMetrics = $('#<%= this.pnlMetrics.ClientID %>');
				if (item.get_value() == 'Metrics')
					pnlMetrics.show();
				else
					pnlMetrics.hide();
			}

			$('body').on('click', '#btnExport', function ()
			{
				var form = $('<form method="POST" action="/Shared/PdfDownloader.ashx" />');
				var div = $('#divExport').clone();
				div.css('transform', 'scale(0.5, 0.5) translate(-50%, -50%)');
				form.append($('<input type="text" name="html" />').val(div[0].outerHTML));
				form.append('<input type="text" name="generator" value="selectpdf" />');
				$('body').append(form);
				form[0].submit();
				form.remove();
			});

			function resetCSS()
			{
				// Sets all the CSS on the RadGrid in its style tag, so it'll export properly to PDF.
				var radGrid = $('.RadGrid');
				radGrid.css(radGrid.getStyles([
					'background-color',
					'border-bottom-color',
					'border-bottom-style',
					'border-bottom-width',
					'border-left-color',
					'border-left-style',
					'border-left-width',
					'border-right-color',
					'border-right-style',
					'border-right-width',
					'border-top-color',
					'border-top-style',
					'border-top-width',
					'color',
					'line-height',
					'overflow'
				]));
				var rgMasterTable = $('.rgMasterTable');
				rgMasterTable.css(rgMasterTable.getStyles([
					'border-collapse',
					'border-spacing'
				]));
				var rgCaption = $('.rgCaption');
				rgCaption.css(rgCaption.getStyles([
					'color',
					'font-family',
					'font-size',
					'font-weight',
					'margin-bottom',
					'margin-top',
					'text-align'
				]));
				$('.rgHeader').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'background-color',
						'border-bottom-color',
						'border-bottom-style',
						'border-bottom-width',
						'border-left-color',
						'border-left-style',
						'border-left-width',
						'border-right-color',
						'border-right-style',
						'border-right-width',
						'border-top-color',
						'border-top-style',
						'border-top-width',
						'color',
						'font-family',
						'font-size',
						'font-weight',
						'padding-bottom',
						'padding-left',
						'padding-right',
						'padding-top',
						'text-align'
					]));
				});
				$('.RadGrid td').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'border-bottom-color',
						'border-bottom-style',
						'border-bottom-width',
						'border-left-color',
						'border-left-style',
						'border-left-width',
						'border-right-color',
						'border-right-style',
						'border-right-width',
						'border-top-color',
						'border-top-style',
						'border-top-width',
						'font-family',
						'padding-bottom',
						'padding-left',
						'padding-right',
						'padding-top'
					]));
				});

				// Same as above but for the charts.
				$('.RadHtmlChart, .pieChart').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'border-bottom-color',
						'border-bottom-style',
						'border-bottom-width',
						'border-left-color',
						'border-left-style',
						'border-left-width',
						'border-right-color',
						'border-right-style',
						'border-right-width',
						'border-top-color',
						'border-top-style',
						'border-top-width',
					]));
				});
				$('.chartMarginTop').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'margin-top'
					]));
				});

				// Same as above but for the pyramid table.
				var pyramidTable = $('#pyramidTable');
				pyramidTable.css(pyramidTable.getStyles([
					'background-color',
					'border-collapse',
					'border-spacing',
					'font-family',
					'font-size'
				]));
				$('.pyramidTable_header').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'background-color',
						'height',
						'text-align',
						'width'
					]));
				});
				$('.pyramidTable_filler').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'border-bottom-color',
						'border-bottom-style',
						'border-bottom-width'
					]));
				});
				$('.pyramidTable_cell').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'text-align'
					]))
				});
				$('.pyramidTable_varianceGood, .pyramidTable_varianceBad').each(function ()
				{
					var $this = $(this);
					$this.css($this.getStyles([
						'background-color',
						'color'
					]));
				});
			}

			Sys.Application.add_init(resetCSS);
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(resetCSS);
		</script>
	</telerik:RadCodeBlock>
</asp:Content>