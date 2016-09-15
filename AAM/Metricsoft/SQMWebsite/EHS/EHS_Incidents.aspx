<%@ Page Title=""  Language="C#" MasterPageFile="~/RspPSMaster.Master"
	AutoEventWireup="True" EnableEventValidation="false" CodeBehind="EHS_Incidents.aspx.cs" ClientIDMode="AutoID"
	Inherits="SQM.Website.EHS_Incidents" ValidateRequest="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_IncidentList.ascx" TagName="IncidentList" TagPrefix="Ucl" %>

<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>


<%@ Reference Control="~/Include/Ucl_PreventionLocation.ascx" %>
<%@ Reference Control="~/Include/Ucl_RadAsyncUpload.ascx" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		$(window).load(function () {
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfwidth').value = $(window).width();
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfheight').value = $(window).height();
		});

		$(window).resize(function () {
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfwidth').value = $(window).width();
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfheight').value = $(window).height();
		});

		function OpenNewIncidentWindow() {
			$find("<%=winNewIncident.ClientID %>").show();
		    }

		function StandardConfirm(sender, args) {

			// Some pages will have no validators, so skip
			if (typeof Page_ClientValidate === "function") {
				var validated = Page_ClientValidate('Val');

				if (!validated)
					alert("Please fill out all required fields.");
			}
		}
		function DeleteConfirm(button, args) {
			args.set_cancel(!confirm("Delete incident - are you sure?  Incidents cannot be undeleted."));
		}

		function DeleteConfirmItem(button, args) {
			args.set_cancel(!confirm("Delete item - are you sure?  Items cannot be undeleted."));
		}

		// For prevention verification

		function PVOnClientClicked(sender, eventArgs) {

			var radGrid = $find(sender.get_id().replace("rbSelectAll", "rgPlantContacts"));
			var masterTable = radGrid.get_masterTableView();
			var i;

			if (sender.get_text() === "Select All") {
				//sender.set_text("Clear All");
				for (i = 0; i < masterTable.get_dataItems().length; i++) {
					masterTable.selectItem(i);
				}
			} else {
				//sender.set_text("Select All");
				for (i = 0; i < masterTable.get_dataItems().length; i++) {
					masterTable.get_dataItems()[i].set_selected(false);
				}
			}

		}

		function PVRowSelectedChanged(sender, eventArgs) {

			var radGrid = sender;
			var masterTable = radGrid.get_masterTableView();
			var count = 0;

			for (var i = 0; i < masterTable.get_dataItems().length; i++) {
				if (masterTable.get_dataItems()[i].get_selected() === true)
					count++;
			}

			var radButton = $find(sender.get_id().replace("rgPlantContacts", "rbSelectAll"));

			if (count === 0)
				radButton.set_text("Select All");

			if (count === masterTable.get_dataItems().length)
				radButton.set_text("Clear All");

		}
	</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<asp:HiddenField ID="hfwidth" runat="server" />
	<asp:HiddenField ID="hfheight" runat="server" />
	<div class="pageWrapper">

		<div class="container-fluid tabActiveTableBg">

			<div class="row-fluid">

				<div class="col-xs-12 col-sm-12">

					<span style="float: left; margin-top: 6px;">
						<asp:Label ID="lblViewEHSRezTitle" runat="server" CssClass="pageTitles" Text="Manage Environmental Health &amp; Safety Incidents" meta:resourcekey="lblViewEHSRezTitleResource1"></asp:Label></span>

					<br class="clearfix visible-xs-block" />

					<div class="col-xs-7 col-sm-3">
						<br />
						<span style="clear: both; float: left; margin-top: -14px;">
							<telerik:RadButton ID="rbNew" runat="server" Text="New Incident" Icon-PrimaryIconUrl="/images/ico-plus.png"
								CssClass="metroIconButton" Skin="Metro" OnClick="rbNew_Click" CausesValidation="False" meta:resourcekey="rbNewResource1" style="position: relative;" />
						</span>
					</div>

					<br class="clearfix visible-xs-block" />

					<asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="Add or update EH&amp;S Incidents below." meta:resourcekey="lblPageInstructionsResource1"></asp:Label>
				</div>
			</div>

			<br style="clear: both;" />
			<telerik:RadPersistenceManager ID="RadPersistenceManager1" runat="server"></telerik:RadPersistenceManager>

			<div id="divIncidentList" runat="server" visible="true">

				<%--	$$$$$$$$$$$$$$ Incident Selection START $$$$$$$$$$$$$$$$$$$$$$$ --%>

				<div class="container-fluid summaryDataEnd" style="padding: 3px 4px 7px 0">

					<div class="row-fluid">

						<span style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt"></asp:Label>
						</span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
						<telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="256px" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>

						<div class="visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

					</div>

					<asp:PlaceHolder ID="phIncident" runat="server">

						<div class="row-fluid">

							<span style="float: left; width: 160px;">
								<asp:Label runat="server" ID="lblIncidentType" CssClass="prompt"></asp:Label>
							</span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
							<telerik:RadComboBox ID="rcbIncidentType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="<%$ Resources:LocalizedText, SelectIncidentTypes %>" Width="256" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" style="margin-top: 7px;" />

							<span style="padding-left:12px;">
							<asp:Label runat="server" ID="lblStatus" CssClass="prompt"></asp:Label>
							<telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="Select incident status to list" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false" meta:resourcekey="rcbStatusSelectResource1">
								<Items>
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="" />
									<telerik:RadComboBoxItem Text="All Open" Value="A" meta:resourcekey="RadComboBoxItemResource2" />
									<telerik:RadComboBoxItem Text="All Closed" Value="C" meta:resourcekey="RadComboBoxItemResource3" />
								</Items>
							</telerik:RadComboBox></span>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" />

							<span style="padding-left:12px;">
							<asp:Label runat="server" ID="lblOSHACategory" CssClass="prompt"></asp:Label>
							<telerik:RadComboBox ID="rcbOSHACategory" runat="server" ToolTip="Select incident status to list" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" >
								<Items>
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="" />
									<telerik:RadComboBoxItem Text="All Open" Value="A" meta:resourcekey="RadComboBoxItemResource2" />
									<telerik:RadComboBoxItem Text="All Closed" Value="C" meta:resourcekey="RadComboBoxItemResource3" />
								</Items>
							</telerik:RadComboBox></span>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" />
						</div>
					</asp:PlaceHolder>

					<div class="row" style="margin-top: 7px;">

						<span style="margin-top: 4px; margin-left: 14px;">
							<span style="padding-right:44px;"><asp:Label runat="server" ID="lblIncidentDate" Text="Incident Date From:" CssClass="prompt" meta:resourcekey="lblIncidentDateResource1"></asp:Label></span>
							<asp:Label runat="server" ID="lblInspectionDate" Text="Inspection Date From:" CssClass="prompt" meta:resourcekey="lblInspectionDateResource1"></asp:Label>
							<span style="margin-right: -10px !important;"><telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="115px" Height="21px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small">
<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;"></Calendar>

<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="">
<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

<FocusedStyle Resize="None"></FocusedStyle>

<DisabledStyle Resize="None"></DisabledStyle>

<InvalidStyle Resize="None"></InvalidStyle>

<HoveredStyle Resize="None"></HoveredStyle>

<EnabledStyle Resize="None"></EnabledStyle>
</DateInput>

<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
						</telerik:RadDatePicker></span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" />

						<span>
							<span style="margin-left: 14px; padding-right:8px;"><asp:Label runat="server" ID="lblToDate" CssClass="prompt"></asp:Label>
							<telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="115px" Height="21px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small">
<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;"></Calendar>

<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="">
<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

<FocusedStyle Resize="None"></FocusedStyle>

<DisabledStyle Resize="None"></DisabledStyle>

<InvalidStyle Resize="None"></InvalidStyle>

<HoveredStyle Resize="None"></HoveredStyle>

<EnabledStyle Resize="None"></EnabledStyle>
</DateInput>

<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
						</telerik:RadDatePicker></span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

						<span class="noprint">
							<span style="margin-left: 14px;">
								<asp:Label ID="lblCreatedByMe" runat="server" Text="Created By Me Only" CssClass="prompt" meta:resourcekey="lblCreatedByMeResource1"></asp:Label>
								<span style="padding-top: 10px;""><asp:CheckBox ID="cbCreatedByMe" runat="server" /></span>
								&nbsp;&nbsp;
								<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt" Visible="False" meta:resourcekey="lblShowImageResource1"></asp:Label>
								<span style="padding-top: 10px;""><asp:CheckBox ID="cbShowImage" runat="server" Visible="False" /></span>
								<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListIncidents %>" OnClick="btnIncidentsSearchClick" />
							</span>
						</span>
					</div>
				</div>
				<br />
				<%--	$$$$$$$$$$$$$$ Incident Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>

				<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2" HorizontalAlign="NotSet">

					<div class="clearfix visible-xs"></div>
					<br class="visible-xs-block" />

					<div id="divChartSelect" runat="server" class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">
						<span class="noprint">
							<asp:Label ID="lblChartType" runat="server" CssClass="prompt" Text="<%$ Resources:LocalizedText, ViewStatistics %>"></asp:Label>
							<telerik:RadComboBox ID="ddlChartType" runat="server" ZIndex="9000" Width="256px" Skin="Metro" EmptyMessage="<%$ Resources:LocalizedText, SelectAChart %>" AutoPostBack="True" OnSelectedIndexChanged="ddlChartTypeChange">
							</telerik:RadComboBox>
							<p style="float: right; margin-right: 5px; width: 100px;">
								<asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="<%$ Resources:LocalizedText, Print %>" Style="margin-right: 5px;" Visible="False" OnClientClick="javascript:window.print()"></asp:LinkButton>
								<asp:LinkButton ID="lnkChartClose" runat="server" CssClass="buttonCancel" Visible="False" OnClick="lnkCloseChart" ToolTip="<%$ Resources:LocalizedText, Close %>"></asp:LinkButton>
							</p>
						</span>
					</div>

					<asp:Panel ID="pnlChartSection" runat="server" Width="100%">
						<div class="row-fluid">
							<div id="divChart" runat="server" class="borderSoft" style="width: 99%; padding: 10px 0;">
								<Ucl:RadGauge ID="uclChart" runat="server" />
							</div>
						</div>
					</asp:Panel>

					<asp:Panel ID="pnlIncidentDetails" runat="server" Width="100%" Visible="False">
						<div class="row-fluid">
							<br />
							<asp:Label ID="lblIncidentDetails" runat="server" CssClass="prompt"></asp:Label>
							<asp:LinkButton ID="lnkIncidentDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkCloseDetails" ToolTip="<%$ Resources:LocalizedText, Close %>">
											 <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle;"/>
							</asp:LinkButton>
							<br />
							<br />
						</div>
					</asp:Panel>
				</telerik:RadAjaxPanel>

				<div class="noprint">
					<Ucl:IncidentList ID="uclIncidentList" runat="server" />
					<br />
					<Ucl:Export ID="uclExport" runat="server" />
					<br />
					<asp:Button ID="btnConversion" runat="server" CssClass="buttonEmphasis" Text="Convert"  OnClick="btnConversion_Click" Visible="false"/>
				</div>
			</div>
		</div>
	</div>

	<telerik:RadWindow runat="server" ID="winNewIncident" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="200px" Width="500px" Behaviors="Close, Move" Title="Record A New Incident" Behavior="None">
		<ContentTemplate>
			<div class="container-fluid" style="margin-top: 10px;">
				<div class="row">
					<div class="col-sm-4 hidden-xs text-left tanLabelCol">
						<asp:Label ID="lbIncidentLocation" runat="server" Text="Incident Location" CssClass="prompt" meta:resourcekey="lbIncidentLocationResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
					</div>
					<div class="col-xs-12 col-sm-8 text-left greyControlCol">
						<telerik:RadComboBox ID="ddlIncidentLocation" runat="server" Skin="Metro" ZIndex="9000" Height="300px" Width="300px" Font-Size="Small"
							ToolTip="Select the location where the incident occured" EmptyMessage="<%$ Resources:LocalizedText, Select %>"></telerik:RadComboBox>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-4 hidden-xs text-left tanLabelCol">
						<asp:Label ID="lblNewIncidentType" runat="server" Text="<%$ Resources:LocalizedText, IncidentType %>" CssClass="prompt"></asp:Label><span class="requiredStarFloat">*</span></span>
					</div>
					<div class="col-xs-12 col-sm-8 text-left greyControlCol">
						<telerik:RadComboBox ID="rddlNewIncidentType" runat="server" Skin="Metro" ZIndex="9000" Height="250px" Width="300px" Font-Size="Small"
							ToolTip="Select the type of incident you wish to report" EmptyMessage="<%$ Resources:LocalizedText, Select %>"> </telerik:RadComboBox>
					</div>
				</div>
				<br />
				<div style="float: right; margin: 5px;">
					<span>
						<asp:Button ID="btnNewIncidentCreate" CSSclass="buttonEmphasis" runat="server" text="Create Incident" style="margin: 5px;" onclick="btnNewIncidentCreate_Click" meta:resourcekey="btnNewIncidentCreateResource1"></asp:Button>
						<asp:Button ID="btnNewIncidentCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnNewIncidentCancel_Click"></asp:Button>
					</span>
                </div>
			</div>
		</ContentTemplate>
	</telerik:RadWindow>
</asp:Content>
