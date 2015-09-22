<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_Data.aspx.cs" Inherits="SQM.Website.EHS.EHS_Data" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.dataHeader
		{
			padding-bottom: 3px;
			line-height: 1.5em;
		}
		.frequencyButtonDiv
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

		/* Simulate the overridden Metro style, as Telerik's RadButton was acting up and not always registering clicks for me. */
		.frequencyButton
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
		.frequencyButton:hover
		{
			opacity: 0.6;
		}
		.frequencyButtonDisabled
		{
			background-color: #ddd !important;
			border: 0 none !important;
			border-radius: 4px;
			color: #999;
			font-family: Verdana, Arial, Helvetica, sans-serif;
			font-size: 12px;
			font-weight: bold;
			height: 32px;
			padding: 6px 24px;
			text-align: center;
		}
	</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<div class="container-fluid">
		<span class="pageTitles" style="float: left; margin-top: 6px">Health & Safety Data Input</span>
		<br class="clear" />
		<span class="instructTextFloat">Input Health & Safety values below.</span>
		<br class="clear" />
		<telerik:RadAjaxLoadingPanel ID="radLoading" runat="server" Skin="Metro" />
		<asp:Panel ID="dataPanel" runat="server">
			<div class="container-fluid blueCell" style="position: relative">
				<telerik:RadComboBox ID="rcbPlant" runat="server" Skin="Metro" Height="350" Width="400" CausesValidation="false" />
				<br /><br />
				<span id="spanEndOfWeek" runat="server">
					<span class="prompt">End of Week: </span>
					<telerik:RadDatePicker ID="rdpEndOfWeek" runat="server" Skin="Metro" DateInput-Skin="Metro" ShowPopupOnFocus="true" DateInput-CausesValidation="false">
						<Calendar FirstDayOfWeek="Monday" runat="server" />
					</telerik:RadDatePicker>
				</span>
				<span id="spanMonth" runat="server">
					<span class="prompt">Month and Year: </span>
					<telerik:RadMonthYearPicker ID="rmypMonth" runat="server" Skin="Metro" DateInput-Skin="Metro" ShowPopupOnFocus="true" DateInput-CausesValidation="false" />
				</span>
				<div class="frequencyButtonDiv">
					<input type="button" id="btnDaily" runat="server" value="Daily" />
					<input type="button" id="btnWeekly" runat="server" value="Weekly" />
					<input type="button" id="btnMonthly" runat="server" value="Monthly" />
				</div>
			</div>
			<br />
			<%-- A Telerik RadGrid for the data, called rgData, is added here via Page_Load --%>
			<br />
			<input type="button" id="btnSave" runat="server" value="Save" class="frequencyButton" />
			<asp:Label ID="lblSaved" runat="server" Text="Data saved!" ForeColor="Green" Font-Size="1.5em" Font-Bold="true" style="display: none; padding-left: 10px" />
		</asp:Panel>
	</div>
	<telerik:RadCodeBlock ID="radCodeBlock" runat="server">
		<script type="text/javascript">
			var radLoading = null;
			var rcbPlant = null;
			var rdpEndOfWeek = null;
			var rmypMonth = null;
			var rgData_grid = null;
			var rgData_columns = null;
			var rgData_data = null;
			var measureIDs = null;

			// This is to get around Telerik's RadGrid applying its own class to the rows, by having jQuery remove the classes when the page loads.
			$('.rgHeader, .rgRow, .rgAltRow').removeClass('rgHeader rgRow rgAltRow');

			// This will get the controls we need later and then get the initial set of data.
			Sys.Application.add_load(function()
			{
				radLoading = $find('<%= this.radLoading.ClientID %>');
				rcbPlant = $find('<%= this.rcbPlant.ClientID %>');
				rdpEndOfWeek = $find('<%= this.rdpEndOfWeek.ClientID %>');
				rmypMonth = $find('<%= this.rmypMonth.ClientID %>');
				rgData_grid = $find('<%= this.dataPanel.FindControl("rgData").ClientID %>').get_masterTableView();
				rgData_columns = rgData_grid.get_columns();
				rgData_data = rgData_grid.get_dataItems();
				measureIDs = $.map(rgData_data, function(n)
				{
					return $(n.get_cell('MeasureID')).html();
				});

				<%= this.rcbPlant.OnClientSelectedIndexChanged %>();
			});

			$(':button[id^="btn"]:not(#btnSave)').click(function()
			{
				var id = $(this).attr('id');
				var day = id.substr(3);
				window.location = '<%= this.Request.Url.AbsolutePath %>?type=' + day;
			});

			// A flag to prevent the get*Data functions from recursing.
			var inUpdate = false;

			// Get the daily data
			function getDailyData()
			{
				if (inUpdate)
					return;
				inUpdate = true;
				// Show the loading panel and then make the AJAX call.
				radLoading.show('<%= this.dataPanel.ClientID %>');
				$.ajax({
					method: 'POST',
					url: '<%= this.Request.Url.AbsolutePath %>/GetDailyData',
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rdpEndOfWeek.get_selectedDate() }),
					contentType: 'application/json; charset=UTF-8',
					success: function(data)
					{
						// On success, we'll update the plant dropdown and date with whatever the server said.
						rcbPlant.findItemByValue(data.d.plantID).select();
						rdpEndOfWeek.set_selectedDate(new Date(data.d.endOfWeek));
						// We then write all the data to the RadGrid, including the headers.
						for (var day in data.d.dates)
						{
							$($.grep(rgData_columns, function(n)
							{
								return n.get_uniqueName() == 'gtc' + day;
							})[0].get_element()).html(data.d.dates[day]);
							for (var rowNum = 0; rowNum < rgData_data.length; ++rowNum)
							{
								var dayData = data.d.allData[day + '|' + measureIDs[rowNum]];
								var cell = $(rgData_data[rowNum].get_cell('gtc' + day));
								cell.find('input[type="text"]').val(dayData.value);
								var validator = cell.find('span[id$="cmp' + day + '"]')[0];
								validator.type = dayData.validatorType;
								validator.title = dayData.validatorToolTip;
							}
						}
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
						inUpdate = false;
					}
				});
			}

			// Get the weekly data
			function getWeeklyData()
			{
				if (inUpdate)
					return;
				inUpdate = true;
				// Show the loading panel and then make the AJAX call.
				radLoading.show('<%= this.dataPanel.ClientID %>');
				$.ajax({
					method: 'POST',
					url: '<%= this.Request.Url.AbsolutePath %>/GetWeeklyData',
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rdpEndOfWeek.get_selectedDate() }),
					contentType: 'application/json; charset=UTF-8',
					success: function(data)
					{
						// On success, we'll update the plant dropdown and date with whatever the server said.
						rcbPlant.findItemByValue(data.d.plantID).select();
						rdpEndOfWeek.set_selectedDate(new Date(data.d.endOfWeek));
						// We then write all the data to the RadGrid, including the header.
						$($.grep(rgData_columns, function(n)
						{
							return n.get_uniqueName() == 'gtcFull';
						})[0].get_element()).html(data.d.date);
						for (var rowNum = 0; rowNum < rgData_data.length; ++rowNum)
						{
							var measureData = data.d.allData[measureIDs[rowNum]];
							var cell = $(rgData_data[rowNum].get_cell('gtcFull'));
							cell.find('input[type="text"]').val(measureData.value);
							var validator = cell.find('span[id$="cmpFull"]')[0];
							validator.type = measureData.validatorType;
							validator.title = measureData.validatorToolTip;
						}
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
						inUpdate = false;
					}
				});
			}

			// Get the monthly data
			function getMonthlyData()
			{
				if (inUpdate)
					return;
				inUpdate = true;
				// Show the loading panel and then make the AJAX call.
				radLoading.show('<%= this.dataPanel.ClientID %>');
				$.ajax({
					method: 'POST',
					url: '<%= this.Request.Url.AbsolutePath %>/GetMonthlyData',
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rmypMonth.get_selectedDate() }),
					contentType: 'application/json; charset=UTF-8',
					success: function(data)
					{
						// On success, we'll update the plant dropdown and date with whatever the server said.
						rcbPlant.findItemByValue(data.d.plantID).select();
						rmypMonth.set_selectedDate(new Date(data.d.startOfMonth));
						// We then write all the data to the RadGrid, including the header.
						$($.grep(rgData_columns, function(n)
						{
							return n.get_uniqueName() == 'gtcFull';
						})[0].get_element()).html(data.d.date);
						for (var rowNum = 0; rowNum < rgData_data.length; ++rowNum)
						{
							var measureData = data.d.allData[measureIDs[rowNum]];
							var cell = $(rgData_data[rowNum].get_cell('gtcFull'));
							cell.find('input[type="text"]').val(measureData.value);
							var validator = cell.find('span[id$="cmpFull"]')[0];
							validator.type = measureData.validatorType;
							validator.title = measureData.validatorToolTip;
						}
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
						inUpdate = false;
					}
				});
			}

			// From the Mozilla Developer Network website, this is in case a browser is lacking the startsWith prototype on String.
			if (!String.prototype.startsWith)
				String.prototype.startsWith = function(searchString, position)
				{
					position = position || 0;
					return this.indexOf(searchString, position) === position;
				};

			// Saves the daily data to the server.
			function saveDailyData()
			{
				// Don't do this if the page isn't valid.
				if (!Page_IsValid)
					return;

				// Show the loading panel, generate the data and then make the AJAX call.
				radLoading.show('<%= this.dataPanel.ClientID %>');
				var days = $.map($.grep(rgData_columns, function(n)
				{
					return n.get_uniqueName().startsWith('gtc');
				}), function(n)
				{
					return n.get_uniqueName().substr(3);
				});
				var data = {};
				for (var dayNum = 0; dayNum < days.length; ++dayNum)
					for (var rowNum = 0; rowNum < rgData_data.length; ++rowNum)
						data[days[dayNum] + '|' + measureIDs[rowNum]] = $(rgData_data[rowNum].get_cell('gtc' + days[dayNum])).find('input[type="text"]').val();
				$.ajax({
					method: 'POST',
					url: '<%= this.Request.Url.AbsolutePath %>/SaveDailyData',
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rdpEndOfWeek.get_selectedDate(), allData: data }),
					contentType: 'application/json; charset=UTF-8',
					success: function()
					{
						// On success, show the saved label then have it fade out after 2 seconds.
						$('#<%= this.lblSaved.ClientID %>').show().delay(2000).fadeOut();
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
					}
				});
			}

			// Saves the weekly data to the server.
			function saveWeeklyData()
			{
				// Don't do this if the page isn't valid.
				if (!Page_IsValid)
					return;

				// Show the loading panel, generate the data and then make the AJAX call.
				radLoading.show('<%= this.dataPanel.ClientID %>');
				var data = {};
				for (var rowNum = 0; rowNum < rgData_data.length; ++rowNum)
					data[measureIDs[rowNum]] = $(rgData_data[rowNum].get_cell('gtcFull')).find('input[type="text"]').val();
				$.ajax({
					method: 'POST',
					url: '<%= this.Request.Url.AbsolutePath %>/SaveWeeklyData',
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rdpEndOfWeek.get_selectedDate(), allData: data }),
					contentType: 'application/json; charset=UTF-8',
					success: function()
					{
						// On success, show the saved label then have it fade out after 2 seconds.
						$('#<%= this.lblSaved.ClientID %>').show().delay(2000).fadeOut();
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
					}
				});
			}

			// Saves the monthly data to the server.
			function saveMonthlyData()
			{
				// Don't do this if the page isn't valid.
				if (!Page_IsValid)
					return;

				// Show the loading panel, generate the data and then make the AJAX call.
				radLoading.show('<%= this.dataPanel.ClientID %>');
				var data = {};
				for (var rowNum = 0; rowNum < rgData_data.length; ++rowNum)
					data[measureIDs[rowNum]] = $(rgData_data[rowNum].get_cell('gtcFull')).find('input[type="text"]').val();
				$.ajax({
					method: 'POST',
					url: '<%= this.Request.Url.AbsolutePath %>/SaveMonthlyData',
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rmypMonth.get_selectedDate(), allData: data }),
					contentType: 'application/json; charset=UTF-8',
					success: function()
					{
						// On success, show the saved label then have it fade out after 2 seconds.
						$('#<%= this.lblSaved.ClientID %>').show().delay(2000).fadeOut();
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
					}
				});
			}
		</script>
	</telerik:RadCodeBlock>
</asp:Content>