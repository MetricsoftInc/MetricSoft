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

		.RadAjax .raDiv
		{
			background-position: center 20px !important;
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
					<!--<telerik:RadButton ID="btnExportAll" runat="server" Text="Export all to PDF" Skin="Metro" OnClick="btnExportAll_Click" OnClientClicked="btnExportAll_ClientClicked" />-->
					<input type="button" id="btnExport" runat="server" value="<%$ Resources:RadGrid.Main, ExportToPdfText %>" class="myButton" />
				</div>
			</div>
			<br />
			<div id="divExportAll" runat="server" style="display: none"></div>
			<div id="divExport" runat="server">
				<asp:Panel ID="pnlPyramidOutput" runat="server" />
				<asp:Panel ID="pnlTRIRBusinessOutput" runat="server" />
				<asp:Panel ID="pnlTRIRPlantOutput" runat="server" />
				<asp:Panel ID="pnlRecPlantOutput" runat="server" />
				<asp:Panel ID="pnlBalancedScorecardOutput" runat="server" />
				<asp:Panel ID="pnlMetricsOutput" runat="server" />
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
				$('body').append(form);
				form[0].submit();
				form.remove();
			});

			function exportAll_endRequest()
			{
				console.log("exportAll_endRequest called");
				Sys.WebForms.PageRequestManager.getInstance().remove_endRequest(exportAll_endRequest);
				var form = $('<form method="POST" action="/Shared/PdfDownloader.ashx" />');
				var div = $('#divExportAll');
				div.css('transform', 'scale(0.5, 0.5) translate(-50%, -50%)');
				form.append($('<input type="text" name="html" />').val(div[0].outerHTML));
				$('body').append(form);
				/*form[0].submit();
				form.remove();
				div.empty();*/
			}

			function btnExportAll_ClientClicked(sender, eventArgs)
			{
				console.log("btnExportAll_ClientClicked called");
				Sys.WebForms.PageRequestManager.getInstance().add_endRequest(exportAll_endRequest);
			}

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