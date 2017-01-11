<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="SQM.Website.Calendar" %>
<%@ Register src="~/Include/Ucl_TaskList.ascx" TagName="TaskList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_TaskStatus.ascx" TagName="Task" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		window.onresize = function () {
			//__doPostBack('resize','');
		}

		window.onload = function () {
		}

		function OpenUpdateTaskWindow() {
			$find("<%=winUpdateTask.ClientID %>").show();
		}

	</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:HiddenField ID="hfBase" runat="server" />
	<asp:HiddenField id="hfTimeout" runat="server"/>
	<asp:HiddenField ID="hdCurrentActiveTab" runat="server" />
	<asp:HiddenField ID="hdCurrentActiveSecondaryTab" runat="server" />
	<asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>

<%--	<FORM name="dummy">--%>
	<div style="margin: 8px;">
		<asp:Button id="btnCalendarView" runat="server" Text="Calendar View" CssClass="buttonStd" OnClick="btnChangeView_Click" CommandArgument="C" meta:resourcekey="btnCalendarViewResource1"/>
		<asp:Button id="btnTaskView" runat="server" Text="Actions Assigned" CssClass="buttonStd" OnClick="btnChangeView_Click" CommandArgument="T" meta:resourcekey="btnTaskViewResource1"/>
		<asp:Button id="btnEscalateView" runat="server" Text="Task Escalations" CssClass="buttonStd" Visible="false" OnClick="btnChangeView_Click" CommandArgument="E" meta:resourcekey="btnEscalateViewResource1"/>
	</div>

	<asp:Panel runat="server" ID="pnlCalendar" Width="100%">

		<div id="divCalendar" runat="server" style="margin-top: 4px;" visible="False">
			<div style="width: 99%; margin: 5px;" class="noprint">
				<asp:Label ID="lblCalendarTitle" runat="server"  CssClass="pageTitles" Text="My Calendar View" meta:resourcekey="lblCalendarTitleResource1" ></asp:Label>
				<br />
				<asp:Label ID="lblCalendarInstruct" runat="server" CssClass="instructText" Text="Tasks assigned to you or related to a selected business location, occuring 12 months prior or 12 months beyond today's date. Click on the date item to view task details." meta:resourcekey="lblCalendarInstructResource1"></asp:Label>
				<br />
			</div>
			<div class="container-fluid">
				<div class="row">
					<div class="col-xs-12  text-left">
						<span style="float: left; margin-left: -7px; padding-top: 5px; padding-right: 5px;">
							<asp:Label ID="lblScheduleScope" runat="server" Text="Display Calendar For:" CssClass="prompt" meta:resourcekey="lblScheduleScopeResource1"></asp:Label>
							&nbsp;&nbsp;
						</span>

						<span style="padding-right: 3px;">
							<telerik:RadComboBox ID="ddlScheduleScope" runat="server" Skin="Metro" Width="280px" ZIndex="10" Font-Size="Small"
								AutoPostBack="True" OnSelectedIndexChanged="ScheduleScope_Select" ToolTip="<%$ Resources:LocalizedText, SelectYourselfBusinessLocation %>"></telerik:RadComboBox>
							<telerik:RadMenu ID="mnuScheduleScope" runat="server" Width="280px" Style="z-index: 9;" EnableAutoScroll="True" OnItemClick="ScheduleScope_Select">
								<DefaultGroupSettings RepeatDirection="Horizontal" />
						</telerik:RadMenu>
						</span>

						<div class="clearfix visible-xs"></div>

						<span class="logoImgInline noprint">
							<asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" ToolTip="Print current calendar view" OnClientClick="javascript:window.print()" meta:resourcekey="lnkPrintResource1"></asp:LinkButton>
						</span>

					</div>
					<div style="float: left; margin: 5px; width: 98%;">
						<Ucl:TaskList ID="uclTaskSchedule" runat="server" />
					</div>
				</div>
			</div>
		</div>

		<div id="divEscalate" runat="server" style="margin-top: 4px;" visible="False">
			<div style="margin: 5px;" class="noprint">
				<asp:Label ID="lblEscalateTitle" runat="server"  CssClass="pageTitles" Text="Task Escalations" meta:resourcekey="lblEscalateTitleResource1" ></asp:Label>
				<br />
				<asp:Label ID="lblEscalateInstruct" runat="server" CssClass="instructText" Text="Tasks escalated to your attention." meta:resourcekey="lblEscalateInstructResource1"></asp:Label>
			</div>
			<div class="container-fluid">
				<div class="row">
					<div id="divTasks" runat="server" class="noprint" style="float: left; margin: 5px; width: 98%;">
						<Ucl:TaskList ID="uclTaskStrip" runat="server" />
					</div>
				</div>
			</div>
		</div>

		<div id="divTaskList" runat="server" style="margin-top: 4px;" visible="False">
			<div style="width: 99%; margin: 5px;" class="noprint">
				<asp:Label ID="LlblActionsTitle" runat="server"  CssClass="pageTitles" Text="Actions Assigned To Me" meta:resourcekey="LlblActionsTitleResource1" ></asp:Label>
				<br />
				<asp:Label ID="lblActionsInstruct" runat="server" CssClass="instructText" Text="Tasks assigned to you. Click on the Task ID to view details or to update its status." meta:resourcekey="lblActionsInstructResource1"></asp:Label>
			</div>
			<div class="container-fluid">
				<div class="row">
					<div class="col-xs-12  text-left">
						<span style="float: left; margin-left: -7px; padding-top: 5px; padding-right: 5px;">
							<asp:Label ID="lblTaskScope" runat="server" Text="<%$ Resources:LocalizedText, DisplayTasksFor %>" CssClass="prompt"></asp:Label>
							&nbsp;&nbsp;</span>
						<span style="padding-right: 3px;">
							<telerik:RadComboBox ID="ddlTaskScope" runat="server" Skin="Metro" Width="280px" ZIndex="10" Font-Size="Small"
								AutoPostBack="True" OnSelectedIndexChanged="TaskScope_Select" ToolTip="<%$ Resources:LocalizedText, SelectYourselfBusinessLocation %>">
							</telerik:RadComboBox>
							<telerik:RadMenu ID="mnuTaskScope" runat="server" Width="280px" Style="z-index: 9;" EnableAutoScroll="True" OnItemClick="TaskScope_Select">
								<DefaultGroupSettings RepeatDirection="Horizontal" />
							</telerik:RadMenu>
						</span>
						<div class="clearfix visible-xs"></div>
						<span style="padding-right: 3px;">
							<asp:Label runat="server" ID="lblStatus" CssClass="prompt"></asp:Label>
						</span>
						<span style="padding-right: 3px;">
							<telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="<%$ Resources:LocalizedText, SelectAssessmentStatus %>" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false">
								<Items>
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Open %>" Value="o" />
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Closed %>" Value="c" />
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="a" />
								</Items>
							</telerik:RadComboBox>
						</span>
					</div>
				</div>
				<div class="row" style="margin-top: 7px;">
					<div class="col-xs-12  text-left">
						<span style="float: left; margin-left: -7px; padding-right: 5px;">
							<span style="padding-right: 25px;">
								<asp:Label runat="server" ID="lblTaskDate" Text="<%$ Resources:LocalizedText, TaskDateFrom %>" CssClass="prompt"></asp:Label></span>
							<span>
								<telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" meta:resourcekey="dmFromDateResource1">
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
								</telerik:RadDatePicker>
							</span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" />

						<span>
							<span style="margin-left: 14px; padding-right: 8px;">
								<asp:Label runat="server" ID="lblToDate" CssClass="prompt"></asp:Label>
								<telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" meta:resourcekey="dmToDateResource1">
									<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" runat="server"></Calendar>

									<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="" runat="server">
										<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

										<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

										<FocusedStyle Resize="None"></FocusedStyle>

										<DisabledStyle Resize="None"></DisabledStyle>

										<InvalidStyle Resize="None"></InvalidStyle>

										<HoveredStyle Resize="None"></HoveredStyle>

										<EnabledStyle Resize="None"></EnabledStyle>
									</DateInput>

									<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
								</telerik:RadDatePicker>
							</span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

						<span>
							<span style="margin-left: 14px; padding-right: 8px;">
								<label for="cbOnlyCreated" style="padding-right: 5px;"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, OnlyTasksCreated %>"></asp:Literal>:</label>
								<input id="cbCreatedByMe" type="checkbox" runat="server"/>
								<%--<asp:CheckBox runat="server" ID="cbOnlyCreated" TextAlign="Left"/>--%>
							</span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

						<span class="noprint">
							<%--<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt"></asp:Label>
                                        <span style="padding-top: 10px;""><asp:CheckBox id="cbShowImage" runat="server" Checked="false"/></span>--%>
							<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListAssessments %>" OnClick="TaskScope_Select" />
						</span>
					</div>
				</div>
			</div>
			<br />
			<div class="container-fluid">
				<div class="row">
					<Ucl:TaskList ID="uclTaskList" runat="server" />
				</div>
			</div>
		</div>
	</asp:Panel>
	<asp:Label ID="lblScheduleRange" runat="server" Text="Future Months:" CssClass="prompt" Visible="False" meta:resourcekey="lblScheduleRangeResource1"></asp:Label>
	<telerik:RadSlider runat="server" ID="sldScheduleRange" visible="False" MinimumValue="3" MaximumValue="12" Value="12" LargeChange="3" ItemType="Tick" TrackPosition="TopLeft" Skin="Metro"
		width="150px" Height="37px" AutoPostBack="True" OnValueChanged="ScheduleScope_Select" ToolTip="Select number of months in the future to populate in the calendar" DbValue="12" Length="150" meta:resourcekey="sldScheduleRangeResource1" SelectedRegionStartValue="3" SelectionEnd="3" SelectionStart="3" ></telerik:RadSlider>
	<br style="clear: both;" />


	<telerik:RadWindow runat="server" ID="winUpdateTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="400px" Width="700px" Behaviors="Close, Move" Title="View/Update Task" Behavior="Close, Move">
		<ContentTemplate>
			<Ucl:Task ID="uclTask" runat="server" />
		</ContentTemplate>
	</telerik:RadWindow>
<%--	</FORM>--%>
</asp:Content>

