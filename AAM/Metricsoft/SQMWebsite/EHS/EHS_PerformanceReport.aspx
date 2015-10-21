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
				<telerik:RadComboBox ID="rcbPlant" runat="server" Skin="Metro" Height="350" Width="400" CausesValidation="false" AutoPostBack="true"
					OnSelectedIndexChanged="rcbPlant_SelectedIndexChanged" />
				<br /><br />
				<span class="prompt">Year: </span>
				<telerik:RadMonthYearPicker ID="rmypYear" runat="server" Skin="Metro" DateInput-Skin="Metro" ShowPopupOnFocus="true" DateInput-CausesValidation="false" AutoPostBack="true"
					DateInput-AutoPostBack="true" DateInput-DateFormat="yyyy" DateInput-DisplayDateFormat="yyyy" OnSelectedDateChanged="rmypYear_SelectedDateChanged" />
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
							<telerik:GridBoundColumn DataField="ManHours" DataFormatString="{0:N0}" HeaderText="Man-hours" ColumnGroupName="None1" />
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
				<div style="page-break-after: always"></div>
				<div id="divFrequencyRate" runat="server" class="chartMarginTop"></div>
				<div id="divSeverityRate" runat="server" class="chartMarginTop"></div>
				<div style="page-break-after: always"></div>
				<div style="overflow: hidden" class="chartMarginTop">
					<SQM:PieChart ID="pieRecordableType" runat="server" Title="Recordable Injuries by Type" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
					<SQM:PieChart ID="pieRecordableBodyPart" runat="server" Title="Recordable Injuries by Body Part" Width="740" Height="500" StartAngle="45" Style="float: right"
						CssClass="pieChart" />
				</div>
				<div style="overflow: hidden" class="chartMarginTop">
					<SQM:PieChart ID="pieRecordableRootCause" runat="server" Title="Injury Root Causes" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
					<SQM:PieChart ID="pieRecordableTenure" runat="server" Title="Tenure of Injured Associate" Width="740" Height="500" StartAngle="45" Style="float: right" CssClass="pieChart" />
				</div>
				<div style="page-break-after: always"></div>
				<div style="overflow: hidden" class="chartMarginTop">
					<SQM:PieChart ID="pieRecordableDaysToClose" runat="server" Title="Days to Close Investigations" Width="740" Height="500" StartAngle="45" CssClass="pieChart" />
				</div>
				<div style="overflow: hidden" class="chartMarginTop">
					<div id="divJSAsAndAudits" runat="server" style="float: left"></div>
					<div id="divSafetyTrainingHours" runat="server" style="float: right"></div>
				</div>
			</div>
		</telerik:RadAjaxPanel>
	</div>
	<Ucl:RadGauge ID="uclChart" runat="server" />
	<telerik:RadCodeBlock ID="radCodeBlock" runat="server">
		<script type="text/javascript">
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
			}

			Sys.Application.add_init(resetCSS);
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(resetCSS);
		</script>
	</telerik:RadCodeBlock>
</asp:Content>