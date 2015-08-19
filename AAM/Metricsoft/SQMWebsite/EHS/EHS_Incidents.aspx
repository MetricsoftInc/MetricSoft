<%@ Page Title="EHS Incidents"  Language="C#" MasterPageFile="~/RspPSMaster.Master"
	AutoEventWireup="True" EnableEventValidation="false" CodeBehind="EHS_Incidents.aspx.cs" ClientIDMode="AutoID"
	Inherits="SQM.Website.EHS_Incidents" ValidateRequest="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %> 
<%@ Register Src="~/Include/Ucl_IncidentList.ascx" TagName="IncidentList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSIncidentForm.ascx" TagName="IncidentForm" TagPrefix="Ucl" %>
<%--<%@ Register Src="~/Include/Ucl_EHSIncidentCustomForm.ascx" TagName="IncidentCustomForm" TagPrefix="Ucl" %>--%>
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
						<asp:Label ID="lblViewEHSRezTitle" runat="server" CssClass="pageTitles" Text="Manage Environmental Health &amp; Safety Incidents"></asp:Label></span>

					<br class="clearfix visible-xs-block" />

					<div class="col-xs-7 col-sm-3">
						<br />
						<span style="clear: both; float: left; margin-top: -14px;">
							<telerik:RadButton ID="rbNew" runat="server" Text="New Incident" Icon-PrimaryIconUrl="/images/ico-plus.png"
								CssClass="metroIconButton" Skin="Metro" OnClick="rbNew_Click" CausesValidation="false" />
						</span>
					</div>

					<br class="clearfix visible-xs-block" />

					<asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="Add or update EH&amp;S Incidents below."></asp:Label>
				</div>
			</div>

			<br style="clear: both;" />
			<telerik:RadPersistenceManager ID="RadPersistenceManager1" runat="server"></telerik:RadPersistenceManager>

			<div id="divIncidentList" runat="server" visible="true">

				<%--	$$$$$$$$$$$$$$ Incident Selection START $$$$$$$$$$$$$$$$$$$$$$$ --%>

				<div class="container-fluid summaryDataEnd" style="padding: 3px 4px 7px 0">

					<div class="row-fluid">

						<span style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lblPlantSelect" Text="Locations:" CssClass="prompt"></asp:Label>
						</span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
						<telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex="9000" Skin="Metro" Height="350" Width="256" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>

						<div class="visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

					</div>

					<asp:PlaceHolder ID="phIncident" runat="server">

						<div class="row-fluid">


							<span style="float: left; width: 160px;">
								<asp:Label runat="server" ID="lblIncidentType" Text="Incident Type:" CssClass="prompt"></asp:Label>
							</span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
							<telerik:RadComboBox ID="rcbIncidentType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="Select incident types to list" Width="256" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" style="margin-top: 7px;" />

							<span style="padding-left:12px;">
							<asp:Label runat="server" ID="lblStatus" Text="Status: " CssClass="prompt"></asp:Label>
											<telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="Select incident status to list" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false">
												<Items>
													<telerik:RadComboBoxItem Text="Open" Value="A" />
													<telerik:RadComboBoxItem Text="Closed" Value="C" />
													<telerik:RadComboBoxItem Text="8D Complete" Value="C8" />
													<telerik:RadComboBoxItem Text="All" Value="" />
													<telerik:RadComboBoxItem Text="Data Incomplete" Value="N" />
													<telerik:RadComboBoxItem Text="Actions Pending" Value="T" />
												</Items>
											</telerik:RadComboBox></span>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" />

						</div>
					</asp:PlaceHolder>

					<asp:PlaceHolder ID="phPrevent" runat="server">

						<div class="row-fluid">

							<span style="clear: both; float: left; width: 160px;">
								<asp:Label runat="server" ID="lblInspectCategory" Text="Inspection Catetory:" CssClass="prompt"></asp:Label>
							</span>

							<span class="summaryDataEnd">
								<telerik:RadComboBox ID="rcbInspectCatetory" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="Select inspection catetories types to list" Width="325" ZIndex="9000" Skin="Metro" AutoPostBack="false">
									<Items>
										<telerik:RadComboBoxItem Text="AGCS Recommendation" Value="AGCS Recommendation" Checked="true" />
										<telerik:RadComboBoxItem Text="Non-Compliance" Value="Non-Compliance" Checked="true" />
										<telerik:RadComboBoxItem Text="Non-Conformance" Value="Non-Conformance" Checked="true" />
										<telerik:RadComboBoxItem Text="Safety Issue" Value="Safety Issue" Checked="true" />
										<telerik:RadComboBoxItem Text="RMCAR" Value="RMCAR" Checked="true" />
										<telerik:RadComboBoxItem Text="Other" Value="Other" Checked="true" />
									</Items>
								</telerik:RadComboBox>
							</span>

						</div>

						<div class="row-fluid">


							<span style="clear: both; float: left; width: 160px;">
								<asp:Label runat="server" ID="lblRecommendType" Text="Recommendation Type:" CssClass="prompt"></asp:Label>
							</span>

							<telerik:RadComboBox ID="rcbRecommendationType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="Select issue recommendation types to list" Width="325" ZIndex="9000" Skin="Metro" AutoPostBack="false">
								<Items>
									<telerik:RadComboBoxItem Text="Behavioral" Value="Behavioral" Checked="true" />
									<telerik:RadComboBoxItem Text="Infrastructure" Value="Infrastructure" Checked="true" />
								</Items>
							</telerik:RadComboBox>

							<span>
								<asp:Label ID="lblPreventStatus" runat="server" CssClass="prompt" Text="Status: "></asp:Label>
								<telerik:RadComboBox ID="rcbFindingsSelect" runat="server" ToolTip="Select findings status to list" Width="200" ZIndex="9000" Skin="Metro" AutoPostBack="false" CheckBoxes="true" EnableCheckAllItemsCheckBox="true">
									<Items>
										<telerik:RadComboBoxItem Text="Open" Value="A" />
										<telerik:RadComboBoxItem Text="In Progress" Value="P" />
										<telerik:RadComboBoxItem Text="Closed" Value="C" />
										<telerik:RadComboBoxItem Text="Audited" Value="U" />
										<telerik:RadComboBoxItem Text="Awaiting Funding" Value="F" />
									</Items>
								</telerik:RadComboBox>
							</span>

						</div>

					</asp:PlaceHolder>

					<div class="row" style="margin-top: 7px;">


						<span style="float: left; margin-top: 4px; margin-left: 14px;">
							<span style="padding-right:44px;"><asp:Label runat="server" ID="lblIncidentDate" Text="Incident Date From:" CssClass="prompt"></asp:Label></span>
							<asp:Label runat="server" ID="lblInspectionDate" Text="Inspection Date From:" CssClass="prompt"></asp:Label>
							<span style="margin-right: -10px !important;"><telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="115" Height="21" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadDatePicker></span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" />

						<span>
							<span style="margin-left: 14px; padding-right:8px;"><asp:Label runat="server" ID="lblToDate" Text="To:" CssClass="prompt"></asp:Label>
							<telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="115" Height="21" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadDatePicker></span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

						<span class="noprint">
							<span style="margin-left: 14px;">
							<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt"></asp:Label>
							<span style="padding-top: 10px;""><asp:CheckBox ID="cbShowImage" runat="server" Checked="false"  /></span>
							<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List incidents" OnClick="btnIncidentsSearchClick" /></span>
						</span>

					</div>
				</div>

				<%--	$$$$$$$$$$$$$$ Incident Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>


				<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2">

					<div class="clearfix visible-xs"></div>
					<br class="visible-xs-block" />

					<div class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">

						<span class="noprint">
							<asp:Label ID="lblChartType" runat="server" CssClass="prompt" Text="View Statistics: "></asp:Label>
							<telerik:RadComboBox ID="ddlChartType" runat="server" ZIndex="9000" Width="256" Skin="Metro" EmptyMessage="Select a chart" AutoPostBack="true" OnSelectedIndexChanged="ddlChartTypeChange">
							</telerik:RadComboBox>
							<p style="float: right; margin-right: 5px; width: 100px;">
								<asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="Print" Style="margin-right: 5px;" Visible="false" OnClientClick="javascript:window.print()"></asp:LinkButton>
								<asp:LinkButton ID="lnkChartClose" runat="server" CssClass="buttonCancel" Visible="false" OnClick="lnkCloseChart" ToolTip="Close"></asp:LinkButton>
							</p>
						</span>

					</div>

					<asp:Panel ID="pnlChartSection" runat="server" Width="100%">
						<div class="row-fluid">
							<%--<div id="divChart" runat="server" class="borderSoft" style="height: 320px; width: 99%; padding: 10px 0;">
									<Ucl:RadGauge ID="uclChart" runat="server" />
								</div>--%>
							<div id="divChart" runat="server" class="borderSoft" style="width: 99%; padding: 10px 0;">
								<Ucl:RadGauge ID="uclChart" runat="server" />
							</div>
						</div>
					</asp:Panel>

					<asp:Panel ID="pnlIncidentDetails" runat="server" Width="100%" Visible="false">
						<div class="row-fluid">
							<br />
							<asp:HiddenField ID="hfIncidentDetails" runat="server" Value="Incident # Summary" />
							<asp:Label ID="lblIncidentDetails" runat="server" CssClass="prompt"></asp:Label>
							<asp:LinkButton ID="lnkIncidentDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkCloseDetails" ToolTip="Close">
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
				</div>
			</div>

			<Ucl:IncidentForm ID="uclIncidentForm" runat="server"/>
<%--			<Ucl:IncidentForm ID="IncidentCustomForm"  Visible="false" runat="server" />--%>

			<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
			</telerik:RadAjaxManager>
	</div>
	</div>
<%--	</div>
	</div>
	</div>
	</div>
	</div>
	</div>
	</div>--%>
</asp:Content>
