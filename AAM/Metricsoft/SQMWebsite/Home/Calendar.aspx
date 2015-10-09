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
		<asp:Button id="btnCalendarView" runat="server" Text="Calendar View" CssClass="buttonStd" OnClick="btnChangeView_Click" CommandArgument="C"/>
		<asp:Button id="btnTaskView" runat="server" Text="Actions Assigned" CssClass="buttonStd" OnClick="btnChangeView_Click" CommandArgument="T"/>
		<asp:Button id="btnEscalateView" runat="server" Text="Task Escalations" CssClass="buttonStd" OnClick="btnChangeView_Click" CommandArgument="E"/>
	</div>

	<asp:Panel runat="server" ID="pnlCalendar" Width="100%">

		<div id="divCalendar" runat="server" style="margin-top: 4px;" visible="false">
			<div style="width: 99%; margin: 5px;" class="noprint">
				<asp:Label ID="lblCalendarTitle" runat="server"  CssClass="pageTitles" Text="My Calendar View" ></asp:Label>
				<br />
				<asp:Label ID="lblCalendarInstruct" runat="server" CssClass="instructText" Text="Tasks assigned to you or related to a selected business location, occuring 12 months prior or 12 months beyond today's date. Click on the date item to view task details."></asp:Label>
				<br />
			</div>
			<div class="container-fluid">
				<div class="row">
					<div class="col-xs-12  text-left">
						<span style="float: left; margin-left: -7px; padding-top: 5px; padding-right: 5px;">
							<asp:Label ID="lblScheduleScope" runat="server" Text="Display Calendar For:" CssClass="prompt"></asp:Label>
							&nbsp;&nbsp;
						</span>
							
						<span style="padding-right: 3px;">
							<telerik:RadComboBox ID="ddlScheduleScope" runat="server" Skin="Metro" Width="280" ZIndex="10" Font-Size="Small"
								AutoPostBack="true" OnSelectedIndexChanged="ScheduleScope_Select" ToolTip="select either yourself or an accesible business location"></telerik:RadComboBox>
							<telerik:RadMenu ID="mnuScheduleScope" runat="server" Skin="Default" Width="280" Style="z-index: 9;" EnableAutoScroll="true" DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="ScheduleScope_Select"></telerik:RadMenu>
						</span>

						<div class="clearfix visible-xs"></div>

						<span class="logoImgInline noprint">
							<asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="" ToolTip="print current calendar view" OnClientClick="javascript:window.print()"></asp:LinkButton>
						</span>

					</div>
					<div style="float: left; margin: 5px; width: 98%;">
						<Ucl:TaskList ID="uclTaskSchedule" runat="server" />
					</div>
				</div>
			</div>
		</div>

		<div id="divEscalate" runat="server" style="margin-top: 4px;" visible="false">
			<div style="margin: 5px;" class="noprint">
				<asp:Label ID="lblEscalateTitle" runat="server"  CssClass="pageTitles" Text="Task Escalations" ></asp:Label>
				<br />
				<asp:Label ID="lblEscalateInstruct" runat="server" CssClass="instructText" Text="Tasks escalated to your attention."></asp:Label>
			</div>
			<div class="container-fluid">
				<div class="row">
					<div id="divTasks" runat="server" class="noprint" style="float: left; margin: 5px; width: 98%;">
						<Ucl:TaskList ID="uclTaskStrip" runat="server" />
					</div>
				</div>
			</div>
		</div>
			
		<div id="divTaskList" runat="server" style="margin-top: 4px;" visible="false">
			<div style="margin: 5px;" class="noprint">
				<asp:Label ID="LlblActionsTitle" runat="server"  CssClass="pageTitles" Text="Actions Assigned To Me" ></asp:Label>
				<br />
				<asp:Label ID="lblActionsInstruct" runat="server" CssClass="instructText" Text="Tasks assigned to you. Click on the Task ID to view details or to update its status."></asp:Label>
			</div>
			<br />
			<div class="container-fluid">
				<div class="row">
					<Ucl:TaskList ID="uclTaskList" runat="server" />
				</div>
			</div>
		</div>
	</asp:Panel>
	<asp:Label ID="lblScheduleRange" runat="server" Text="Future Months:" CssClass="prompt" Visible="false"></asp:Label>
	<telerik:RadSlider runat="server" ID="sldScheduleRange" visible="false" MinimumValue="3" MaximumValue="12" Value="12" SmallChange="1" LargeChange="3" ItemType="Tick" TrackPosition="TopLeft" Skin="Metro" ShowDragHandle="true" 
		width="150" Height="37" AutoPostBack="true" OnValueChanged="ScheduleScope_Select" ToolTip="select number of months in the future to populate in the calendar" ></telerik:RadSlider>
	<br style="clear: both;" />


	<telerik:RadWindow runat="server" ID="winUpdateTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="700" Behaviors="Move,Close" Title="View/Update Task">
		<ContentTemplate>
			<Ucl:Task ID="uclTask" runat="server" />
		</ContentTemplate>
	</telerik:RadWindow>
<%--	</FORM>--%>
</asp:Content>

