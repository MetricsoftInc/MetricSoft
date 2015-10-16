<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_PerformanceReport.aspx.cs" Inherits="SQM.Website.EHS.EHS_PerformanceReport" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="SQMWebsite" Namespace="SQM.Website" TagPrefix="SQM" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.RadGrid_Metro .rgRow > td
		{
			border-color: #e5e5e5 !important;
		}
		.rgMasterTable tr.rgMultiHeaderRow:nth-child(2) .rgHeader
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

		.k-pdf-export svg[data-creator="SQM_PieChart"] + img
		{
			display: inline !important;
		}
		.k-pdf-export svg[data-creator="SQM_PieChart"]
		{
			display: none;
		}

		#divExport
		{
			margin: 0 auto;
			/*display: none;*/
		}
		.k-pdf-export #divExport
		{
			display: block;
			-moz-transform: scale(0.5, 0.5) translate(-50%, -50%);
			-ms-transform: scale(0.5, 0.5) translate(-50%, -50%);
			-o-transform: scale(0.5, 0.5) translate(-50%, -50%);
			-webkit-transform: scale(0.5, 0.5) translate(-50%, -50%);
			transform: scale(0.5, 0.5) translate(-50%, -50%);
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
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<asp:HiddenField ID="hfCompanyID" runat="server" />
	<telerik:RadClientExportManager ID="radExport" runat="server" OnClientPdfExported="clientPDFExported">
		<PdfSettings FileName="Test.pdf" PaperSize="Letter" Landscape="true" MarginTop="0.25in" MarginLeft="0.25in" MarginBottom="0.25in" MarginRight="0.25in" PageBreakSelector=".pageBreak" />
	</telerik:RadClientExportManager>
	<div class="container-fluid">
		<span class="pageTitles" style="float: left; margin-top: 6px">Health & Safety Performance Report</span>
		<br class="clear" /><br />
		<telerik:RadAjaxLoadingPanel ID="radLoading" runat="server" Skin="Metro" />
		<telerik:RadAjaxPanel ID="radAjaxPanel" runat="server" LoadingPanelID="radLoading">
			<div class="container-fluid blueCell" style="position: relative">
				<telerik:RadComboBox ID="rcbPlant" runat="server" Skin="Metro" Height="350" Width="400" CausesValidation="false" AutoPostBack="true"
					OnSelectedIndexChanged="rcbPlant_SelectedIndexChanged" />
				<br /><br />
				<input type="button" id="btnExport" value="Export to PDF" class="myButton" />
			</div>
			<br />
			<div id="divExport" runat="server">
				<telerik:RadGrid ID="rgReport" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500px" OnItemDataBound="rgReport_ItemDataBound">
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
							<telerik:GridBoundColumn DataField="ManHours" HeaderText="Man-hours" ColumnGroupName="None1" />
							<telerik:GridBoundColumn DataField="Incidents" HeaderText="Total<br>Recordable<br>Cases" ColumnGroupName="Incidents" />
							<telerik:GridBoundColumn DataField="Frequency" HeaderText="Total Lost<br>Time Cases" ColumnGroupName="Frequency" />
							<telerik:GridBoundColumn DataField="Restricted" HeaderText="Total<br>Days<br>Restricted" ColumnGroupName="Restricted" />
							<telerik:GridBoundColumn DataField="Severity" HeaderText="Total Lost<br>Time Days" ColumnGroupName="Severity" />
							<telerik:GridBoundColumn DataField="FirstAid" HeaderText="First Aid<br>Cases" ColumnGroupName="None2" />
							<telerik:GridBoundColumn DataField="Leadership" HeaderText="Leadership<br>Safety<br>Walks" ColumnGroupName="None2" />
							<telerik:GridBoundColumn DataField="JSAs" HeaderText="JSAs<br>Completed" ColumnGroupName="None2" />
							<telerik:GridBoundColumn DataField="SafetyTraining" HeaderText="Total Safety<br>Training<br>Hours" ColumnGroupName="None2" />
						</Columns>
					</MasterTableView>
				</telerik:RadGrid>
				<div id="divTRIR" runat="server" class="chartMarginTop"></div>
				<span class="pageBreak" style="display: none"></span>
				<div id="divFrequencyRate" runat="server" class="chartMarginTop"></div>
				<div id="divSeverityRate" runat="server" class="chartMarginTop"></div>
				<span class="pageBreak" style="display: none"></span>
				<div style="overflow: hidden" class="chartMarginTop">
					<SQM:PieChart ID="pieRecordableType" runat="server" Title="Recordable Injuries by Type" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
					<SQM:PieChart ID="pieRecordableBodyPart" runat="server" Title="Recordable Injuries by Body Part" Width="740" Height="500" StartAngle="45" Style="float: right"
						CssClass="pieChart" />
				</div>
				<div style="overflow: hidden" class="chartMarginTop">
					<SQM:PieChart ID="pieRecordableRootCause" runat="server" Title="Injury Root Causes" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
					<SQM:PieChart ID="pieRecordableTenure" runat="server" Title="Tenure of Injured Associate" Width="740" Height="500" StartAngle="45" Style="float: right" CssClass="pieChart" />
				</div>
				<span class="pageBreak" style="display: none"></span>
				<div class="chartMarginTop">
					<SQM:PieChart ID="pieRecordableDaysToClose" runat="server" Title="Days to Close Investigations" Width="740" Height="500" StartAngle="45" CssClass="pieChart" />
				</div>
			</div>
		</telerik:RadAjaxPanel>
	</div>
	<Ucl:RadGauge ID="uclChart" runat="server" />
	<telerik:RadCodeBlock ID="radCodeBlock" runat="server">
		<script type="text/javascript">
			var radLoading = null;

			// This will get the controls we need later and then get the initial set of data.
			Sys.Application.add_load(function ()
			{
				radLoading = $find('<%= this.radLoading.ClientID %>');
			});

			$('#btnExport').click(function ()
			{
				radLoading.show('<%= this.radAjaxPanel.ClientID %>');
				$find('<%= this.radExport.ClientID %>').exportPDF($telerik.$('#divExport'));
			});

			function clientPDFExported(sender, args)
			{
				radLoading.hide('<%= this.radAjaxPanel.ClientID %>');
			}
		</script>
	</telerik:RadCodeBlock>
</asp:Content>