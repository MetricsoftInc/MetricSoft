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
		<asp:Button id="btnEscalateView" runat="server" Text="Task Escalations" CssClass="buttonStd" OnClick="btnChangeView_Click" CommandArgument="E" meta:resourcekey="btnEscalateViewResource1"/>
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
								AutoPostBack="True" OnSelectedIndexChanged="ScheduleScope_Select" ToolTip="Select either yourself or an accesible business location" meta:resourcekey="ddlScheduleScopeResource1"></telerik:RadComboBox>
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
			<div style="margin: 5px;" class="noprint">
				<asp:Label ID="LlblActionsTitle" runat="server"  CssClass="pageTitles" Text="Actions Assigned To Me" meta:resourcekey="LlblActionsTitleResource1" ></asp:Label>
				<br />
				<asp:Label ID="lblActionsInstruct" runat="server" CssClass="instructText" Text="Tasks assigned to you. Click on the Task ID to view details or to update its status." meta:resourcekey="lblActionsInstructResource1"></asp:Label>
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

