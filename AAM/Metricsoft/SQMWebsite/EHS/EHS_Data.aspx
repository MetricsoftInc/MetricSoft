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

		.flex
		{
			display: -moz-box;
			display: -ms-flexbox;
			display: -webkit-box;
			display: flex;
		}
		.rwDetails_column
		{
			float: left;
			text-align: center;
		}
		.rwDetails_columnHeader
		{
			font-weight: bold;
			margin-bottom: 5px;
		}
		.rwDetails_row
		{
			align-items: center;
			margin-bottom: 5px;
		}
		.rwDetails_rowLabel
		{
			display: inline-block;
			-ms-flex: 1 0 auto;
			-webkit-flex: 1 0 auto;
			flex: 1 0 auto;
		}
		.rwDetails_rowTextbox
		{
			width: 50px;
			margin-left: 5px;
			display: inline-block;
		}
		.rwDetails_spacing
		{
			float: left;
			position: relative;
			width: 10px;
		}
		.rwDetails_spacing::after
		{
			background-color: #000;
			content: '';
			height: calc(100% - 10px);
			top: 5px;
			width: 1px;
			left: calc(50% - 1px);
			position: absolute;
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
		.myButtonSmall
		{
			background-color: #191970;
			border: 0 none;
			border-radius: 4px;
			color: #fff;
			font-family: Verdana, Arial, Helvetica, sans-serif;
			font-size: 10px;
			font-weight: bold;
			height: 21px;
			padding: 4px 12px;
			text-align: center;
		}
		.myButton:hover, .myButtonSmall:hover
		{
			opacity: 0.6;
		}
		.myButtonDisabled
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
					<input type="button" id="btnDaily" runat="server" value="<%$ Resources:RadScheduler.Main, AdvancedDaily %>" />
					<input type="button" id="btnWeekly" runat="server" value="<%$ Resources:RadScheduler.Main, AdvancedWeekly %>" />
					<input type="button" id="btnMonthly" runat="server" value="<%$ Resources:RadScheduler.Main, AdvancedMonthly %>" />
				</div>
			</div>
			<br />
			<%-- A Telerik RadGrid for the data, called rgData, is added here via Page_Load --%>
			<br id="rgData_placeholder" runat="server" />
			<input type="button" id="btnSave" runat="server" value="<%$ Resources:LocalizedText, Save %>" class="myButton UseSubmitAction" />
			<asp:Label ID="lblSaved" runat="server" Text="Data saved!" ForeColor="Green" Font-Size="1.5em" Font-Bold="true" style="display: none; padding-left: 10px" />
		</asp:Panel>
		<input type="hidden" id="hfCurrDetails" />
		<telerik:RadWindow ID="rwDetails" runat="server" Skin="Metro" Modal="true" Width="1200px" Height="600px" VisibleOnPageLoad="false" Behaviors="None">
			<ContentTemplate>
				<div class="flex" style="margin-top: 5px; overflow-y: auto; max-height: calc(100% - 47px); justify-content: center">
					<div class="rwDetails_column">
						<div class="rwDetails_columnHeader">Type</div>
						<div id="typeColumn" style="text-align: left"></div>
					</div>
					<div class="rwDetails_spacing">&nbsp;</div>
					<div class="rwDetails_column">
						<div class="rwDetails_columnHeader">Body Part</div>
						<div id="bodyPartColumn" style="text-align: left"></div>
					</div>
					<div class="rwDetails_spacing">&nbsp;</div>
					<div class="rwDetails_column">
						<div class="rwDetails_columnHeader">Root Cause</div>
						<div id="rootCauseColumn" style="text-align: left"></div>
					</div>
					<div class="rwDetails_spacing">&nbsp;</div>
					<div class="rwDetails_column">
						<div class="rwDetails_columnHeader">Tenure</div>
						<div id="tenureColumn" style="text-align: left"></div>
					</div>
					<div class="rwDetails_spacing">&nbsp;</div>
					<div class="rwDetails_column">
						<div class="rwDetails_columnHeader">Days To Close</div>
						<div id="daysToCloseColumn" style="text-align: left"></div>
					</div>
				</div>
				<div style="float: right; margin: 5px">
					<input type="button" class="myButton" value="Cancel" onclick="rwDetails_cancel()">
					<input type="button" class="myButton" value="Save" onclick="rwDetails_close()">
				</div>
			</ContentTemplate>
		</telerik:RadWindow>
	</div>
	<asp:HiddenField ID="hfLang" runat="server" />
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

			var dates = null;
			var rwDetails = null;
			var rowsForButtons = {};
			var hfCurrDetails = $('#hfCurrDetails');

			var hfLang = $('#<%= hfLang.ClientID %>');

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

				rwDetails = $find('<%= this.rwDetails.ClientID %>');
				$('[id*="btnDetails"]').each(function()
				{
					rowsForButtons[this.id] = $find($(this).parent().parent().attr('id')).get_itemIndex();
				});
			});

			function rbYesNoNA_ClientCheckedChanged(sender, eventArgs)
			{
				if (eventArgs.get_checked())
					$(sender.get_element()).parent().find('input[type="text"]').val(sender.get_text());
			}

			$(':button[id^="btn"]:not(#btnSave)').click(function()
			{
				var id = $(this).attr('id');
				var day = id.substr(3);
				window.location = '<%= this.Request.Url.AbsolutePath %>?type=' + day + '&plant=' + rcbPlant.get_value();
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
					data: JSON.stringify({ plantID: rcbPlant.get_value(), day: rdpEndOfWeek.get_selectedDate(), lang: hfLang.val() }),
					contentType: 'application/json; charset=UTF-8',
					success: function(data)
					{
						// On success, we'll update the plant dropdown and date with whatever the server said.
						rcbPlant.findItemByValue(data.d.plantID).select();
						rdpEndOfWeek.set_selectedDate(new Date(data.d.endOfWeek));
						// Store the date headers in an array for use later.
						dates = data.d.dates;
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
								var textBox = cell.find('input[type="text"]');
								textBox.val(dayData.value);
								if (cell.find('[id$="rbYes' + day + '"]').length > 0)
								{
									if (dayData.value == 'Yes')
										$find(cell.find('[id$="rbYes' + day + '"]').attr('id')).set_checked(true);
									else if (dayData.value == 'No')
										$find(cell.find('[id$="rbNo' + day + '"]').attr('id')).set_checked(true);
									else
										$find(cell.find('[id$="rbNA' + day + '"]').attr('id')).set_checked(true);
								}
								if (dayData.readOnly)
								{
									textBox.prop('readonly', true);
									$find(textBox.attr('id')).disable();
								}
								else
								{
									textBox.prop('readonly', false);
									$find(textBox.attr('id')).enable();
								}
								var validator = cell.find('span[id$="cmp' + day + '"]')[0];
								if (validator)
								{
									validator.type = dayData.validatorType;
									validator.title = dayData.validatorToolTip;
								}
								if (dayData.ordinal)
									cell.find('input[type="hidden"][name$="hfDetails' + day + '"]').val(JSON.stringify(dayData.ordinal));
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
							var textBox = cell.find('input[type="text"]');
							textBox.val(measureData.value);
							if (cell.find('[id$="rbYesFull"]').length > 0)
							{
								if (measureData.value == 'Yes')
									$find(cell.find('[id$="rbYesFull"]').attr('id')).set_checked(true);
								else if (measureData.value == 'No')
									$find(cell.find('[id$="rbNoFull"]').attr('id')).set_checked(true);
								else
									$find(cell.find('[id$="rbNAFull"]').attr('id')).set_checked(true);
							}
							if (measureData.readOnly)
							{
								textBox.prop('readonly', true);
								$find(textBox.attr('id')).disable();
							}
							else
							{
								textBox.prop('readonly', false);
								$find(textBox.attr('id')).enable();
							}
							var validator = cell.find('span[id$="cmpFull"]')[0];
							if (validator)
							{
								validator.type = measureData.validatorType;
								validator.title = measureData.validatorToolTip;
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
							var textBox = cell.find('input[type="text"]');
							textBox.val(measureData.value);
							if (cell.find('[id$="rbYesFull"]').length > 0)
							{
								if (measureData.value == 'Yes')
									$find(cell.find('[id$="rbYesFull"]').attr('id')).set_checked(true);
								else if (measureData.value == 'No')
									$find(cell.find('[id$="rbNoFull"]').attr('id')).set_checked(true);
								else
									$find(cell.find('[id$="rbNAFull"]').attr('id')).set_checked(true);
							}
							if (measureData.readOnly)
							{
								textBox.prop('readonly', true);
								$find(textBox.attr('id')).disable();
							}
							else
							{
								textBox.prop('readonly', false);
								$find(textBox.attr('id')).enable();
							}
							var validator = cell.find('span[id$="cmpFull"]')[0];
							if (validator)
							{
								validator.type = measureData.validatorType;
								validator.title = measureData.validatorToolTip;
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
					{
						var cell = $(rgData_data[rowNum].get_cell('gtc' + days[dayNum]));
						var dayData =
						{
							value: cell.find('input[type="text"]').val()
						};
						var hfDetails = cell.find('input[type="hidden"][name$="hfDetails' + days[dayNum] + '"]');
						if (hfDetails.length > 0)
							dayData.ordinal = $.parseJSON(hfDetails.val());
						data[days[dayNum] + '|' + measureIDs[rowNum]] = dayData;
					}
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
					error: function(jqXHR, textStatus, errorThrown)
					{
						alert('An error occured while saving.\n\n' + textStatus + ': ' + errorThrown);
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
						unsaved = submitted = false;
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
					error: function(jqXHR, textStatus, errorThrown)
					{
						alert('An error occured while saving.\n\n' + textStatus + ': ' + errorThrown);
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
						unsaved = submitted = false;
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
					error: function(jqXHR, textStatus, errorThrown)
					{
						alert('An error occured while saving.\n\n' + textStatus + ': ' + errorThrown);
					},
					complete: function()
					{
						// No matter what, always hide the loading panel when done.
						radLoading.hide('<%= this.dataPanel.ClientID %>');
						unsaved = submitted = false;
					}
				});
			}

			// From the Mozilla Developer Network website, this is in case a browser is lacking the endsWith prototype on String.
			if (!String.prototype.endsWith)
				String.prototype.endsWith = function(searchString, position)
				{
					var subjectString = this.toString();
					if (position === undefined || position > subjectString.length)
						position = subjectString.length;
					position -= searchString.length;
					var lastIndex = subjectString.indexOf(searchString, position);
					return lastIndex !== -1 && lastIndex === position;
				};

			function rwDetails_populateAndShow(data)
			{
				for (var key1 in data)
				{
					var column = $('#' + key1 + 'Column');
					column.empty();
					for (var key2 in data[key1])
					{
						var value = '';
						if (data[key1][key2].value)
							value = ' value="' + data[key1][key2].value + '"';
						column.append('<div class="flex rwDetails_row"><div class="rwDetails_rowLabel">' + key2 +
							'</div><div class="riSingle RadInput RadInput_Metro rwDetails_rowTextbox"><input class="riTextBox riEnabled WarnIfChanged" type="text"' + value +
							' size="20"></div></div>');
					}
				}

				rwDetails.show();
			}

			function rwDetails_open(dayOfWeek, button)
			{
				var date = dates[dayOfWeek];
				var br = date.indexOf('<br>');
				date = date.substring(6, br);
				var row = rowsForButtons[button.id];
				rwDetails.set_title($(rgData_data[row].get_cell('MEASURE_NAME')).html() + ' Details for ' + date);
				hfCurrDetails.val(JSON.stringify({ dayOfWeek: dayOfWeek, row: row }));
				rwDetails_populateAndShow($.parseJSON($(rgData_data[row].get_element()).find('input[type="hidden"][name$="hfDetails' + dayOfWeek + '"]').val()));
			}

			function rwDetails_cancel()
			{
				rwDetails.close();
			}

			var space_regex = /([a-z])([A-Z])/g;

			function rwDetails_close()
			{
				var details = $.parseJSON(hfCurrDetails.val());
				var count = $(rgData_data[details.row].get_cell('gtc' + details.dayOfWeek)).find('input[type="text"]').val();
				if (!count)
					count = 0;
				else
					count = parseInt(count);

				var errors = '';
				var newData = {};
				$('[id$="Column"]').each(function(i, c)
				{
					var $c = $(c);
					var column = $c.attr('id');
					column = column.substr(0, column.length - 6);
					newData[column] = {};
					var sum = 0;
					$c.find('input[type="text"]').each(function()
					{
						var $this = $(this);
						var type = $this.parent().prev().html();
						newData[column][type] = {};
						var val = $this.val();
						if (!val)
							val = 0;
						else
							newData[column][type].value = val = parseInt(val);
						sum += val;
					});
					if (sum != count)
					{
						var column_uppercase = column.substr(0, 1).toUpperCase() + column.substr(1).replace(space_regex, '$1 $2');
						errors += '\n* ' + column_uppercase + ' (Got ' + sum + ' instead)';
					}
				});

				if (errors && !confirm('Number of ' + $(rgData_data[details.row].get_cell('MEASURE_NAME')).html() + ' incidents for this day was ' + count +
					', but the following totals do not match up:\n' + errors + '\n\nClick OK if this is fine or click Cancel to correct your entries.'))
					return;

				$(rgData_data[details.row].get_element()).find('input[type="hidden"][name$="hfDetails' + details.dayOfWeek + '"]').val(JSON.stringify(newData));
				rwDetails.close();
			}
		</script>
	</telerik:RadCodeBlock>
</asp:Content>