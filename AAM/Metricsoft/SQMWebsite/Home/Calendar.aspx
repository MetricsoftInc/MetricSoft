<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="SQM.Website.Calendar" %>
<%@ Register src="~/Include/Ucl_TaskList.ascx" TagName="TaskList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminEdit.ascx" TagName="PrefsEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocList" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		window.onresize = function () {
			document.getElementById('hfWidth').value = $(window).width();
			document.getElementById('hfHeight').value = $(window).height();
			//__doPostBack('resize','');
		}

		window.onload = function () {
			// var timeout = document.getElementById('hfTimeout').value;
			// var timeoutWarn = ((parseInt(timeout) - 2) * 60000);
			// window.setTimeout(function () { alert("Your Session Will Timeout In Approximately 2 Minutes.  Please save your work or cancel the page if you are finished.") }, timeoutWarn);
			document.getElementById('hfWidth').value = $(window).width();
			document.getElementById('hfHeight').value = $(window).height();
			//__doPostBack('resize', '');
		}

	</script> 

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<script src="scripts/ps_admin.js" type="text/javascript"></script>
	<link href="css/PSSQM.css" rel="stylesheet" type="text/css" />
	<asp:HiddenField ID="hfBase" runat="server" />
	<asp:HiddenField id="hfTimeout" runat="server"/>
	<asp:HiddenField ID="hdCurrentActiveTab" runat="server" />
	<asp:HiddenField ID="hdCurrentActiveSecondaryTab" runat="server" />
	<asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
	<asp:HiddenField ID="hfWidth" runat="server"/>
	<asp:HiddenField ID="hfHeight" runat="server"/>
	<FORM name="dummy">
		<asp:Panel runat="server" ID="pnlCalendar" Width="100%" Visible="false">
			<div style="width: 99%; margin: 5px;" class="noprint">
				<asp:Label ID="lblPageTitle" runat="server"  CssClass="pageTitles" Text="Task Calendar" ></asp:Label>
				<br />
				<asp:Label ID="lblPageInstruct" runat="server" CssClass="instructText" Text="Tasks assigned to you or related to a selected business location, occuring 12 months prior or 12 months beyond today's date."></asp:Label>
				<br />
			</div>
			<div id="divCalendar" runat="server" style="margin-top: 4px;">
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
					<div class="row">
						<div id="divTasks" runat="server" class="noprint" style="float: left; margin: 5px; width: 98%;">
							<Ucl:TaskList ID="uclTaskStrip" runat="server" />
						</div>
					</div>
				</div>
			</div>
		</asp:Panel>
		<asp:Label ID="lblScheduleRange" runat="server" Text="Future Months:" CssClass="prompt" Visible="false"></asp:Label>
		<telerik:RadSlider runat="server" ID="sldScheduleRange" visible="false" MinimumValue="3" MaximumValue="12" Value="12" SmallChange="1" LargeChange="3" ItemType="Tick" TrackPosition="TopLeft" Skin="Metro" ShowDragHandle="true" 
			width="150" Height="37" AutoPostBack="true" OnValueChanged="ScheduleScope_Select" ToolTip="select number of months in the future to populate in the calendar" ></telerik:RadSlider>
		<br style="clear: both;" />
	</FORM>
</asp:Content>

