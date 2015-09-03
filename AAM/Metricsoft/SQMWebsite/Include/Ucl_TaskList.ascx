<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_TaskList.ascx.cs" Inherits="SQM.Website.Ucl_TaskList" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IncidentList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_EHSList.ascx" TagName="EHSInput" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
	function OpenTaskDetailWindow(details) {
		//document.getElementById('lblPopupInfo').value = "this is an info block";
		$get("<%=lblTaskDetail.ClientID %>").innerHTML = details;
		$find("<%=RadWindow_TaskDetail.ClientID %>").show();
	}

	function OnClientAppointmentEditing(sender, eventArgs) {
		eventArgs.set_cancel(true);
	}

	function CloseTaskDetailWindow() {
		var oWindow = GetRadWindow();  //Obtaining a reference to the current window 
		oWindow.Close();
	}
</script>

<asp:Panel ID="pnlTaskSchedule" runat="server" Visible="false" >
	<telerik:RadAjaxPanel runat="server" ID="RadAjaxSchedule">
		<telerik:RadScheduler ID="scdTaskSchedule" name="scdTaskSchedule" RenderMode="Auto" runat="server" Font-Size="X-Small" Skin="Metro" 
			SelectedView="MonthView" OnAppointmentDataBound="scdTaskSchedule_OnDataBound"  OnAppointmentCreated="scdTaskSchedule_OnCreated" TimeLabelRowSpan ="2" 
			 DataKeyField ="RecordKey" DataSubjectField="LongTitle" DataStartField="StartDate" DataEndField="EndDate" DataDescriptionField="Description"
			 OnClientAppointmentEditing="OnClientAppointmentEditing" AllowEdit="false" AllowInsert="false" ReadOnly="true" OverflowBehavior="Expand">
				<AdvancedForm Modal="true" EnableResourceEditing="false" Enabled="false" ></AdvancedForm>
				<WeekView UserSelectable="true" />
				<DayView UserSelectable="true" />
				<MultiDayView UserSelectable="false" />
				<TimelineView UserSelectable="false" />
				<MonthView UserSelectable="true" AdaptiveRowHeight="false" HeaderDateFormat="Y" />
				<TimeSlotContextMenuSettings EnableDefault="true"></TimeSlotContextMenuSettings>
				<AppointmentContextMenuSettings EnableDefault="true"></AppointmentContextMenuSettings>
				<AppointmentTemplate>
					<div id="divSchedule" runat="server" title='<%#Eval("Description") %>' class="taskCalendar">
						<asp:LinkButton ID="lnkScheduleItem" runat="server" Text='<%#Eval("Subject").ToString().Substring(0,Eval("Subject").ToString().IndexOf("-")) %>' CommandArgument='<%#Eval("ID")%>' OnClick="TaskItem_Click" Style="font-weight: bold; color: black;"></asp:LinkButton>
						<asp:LinkButton ID="lnkScheduleItem2" runat="server" Text=' <%#Eval("Subject").ToString().Substring(Eval("Subject").ToString().IndexOf("-")+0,(Eval("Subject").ToString().Length - Eval("Subject").ToString().IndexOf("-")-0 ))%>' CommandArgument='<%#Eval("ID")%>' OnClick="TaskItem_Click" Style="color: black;"></asp:LinkButton>
						<div id="divInactive" runat="server" visible="false">
							<strong><%#Eval("Subject").ToString().Substring(0,Eval("Subject").ToString().IndexOf('-')) %></strong>
							<%#Eval("Subject").ToString().Substring(Eval("Subject").ToString().IndexOf('-')+0,(Eval("Subject").ToString().Length - Eval("Subject").ToString().IndexOf('-')-0))%>
						</div>
						<asp:ImageButton ID="btnTaskDetails" ImageUrl="~/images/defaulticon/16x16/zoom.png" runat="server" Visible="false" OnClientClick='<%# String.Format("OpenTaskDetailWindow(\"{0}\"); return false;", Eval("Description")) %>' ToolTip="view task details" style="margin-left: 3px; vertical-align: middle; border: 0px;"/>
					</div>
				</AppointmentTemplate>
		</telerik:RadScheduler>
		<asp:HiddenField id="hfScheduleScope" runat="server"/>
	</telerik:RadAjaxPanel>
</asp:Panel>

<asp:Panel ID="pnlTaskStrip" runat="server" Visible="false">
	<div class="navSectionBar"  style ="height: 22px; text-align:center; vertical-align: middle; padding-top: 2px;">
		<asp:Label ID="lblTaskStriptInstruct" runat="server" Text="Tasks Overdue" CssClass="textStd" style="color: black;"></asp:Label>
		<asp:Image ID="Image1" runat="server" ImageUrl="~/images/status/warning.png"  Style="margin: 0px 3px 1px 3px; vertical-align: middle; border: 0px;"/>
		<asp:Label ID="lblTaskStripCount" runat="server" CssClass="textSmall"></asp:Label>
	</div>
	<div id="divTaskStripRepeater" runat="server" class="scrollArea" style="border-style: none; margin-top: 7px;">
		<asp:Repeater runat="server" ID="rptTaskStrip" ClientIDMode="AutoID" OnItemDataBound="rptTaskStrip_OnItemDataBound" OnItemCreated="rptTaskStrip_OnItemCreate">
			<HeaderTemplate>
				<table  cellspacing="0" cellpadding="1" border="0" width="100%" >
			</HeaderTemplate>
			<ItemTemplate>
				<div runat="server" title='<%#Eval("Description") %>' class="taskCalendar">
					<asp:Label runat="server" ID="lblDueDate" Text='<%#Eval("Task.DUE_DT").ToString() %>' Style="font-size: 10px; font-weight: bold;"></asp:Label>
					<asp:ImageButton ID="imgTaskStatus" ImageUrl="~/images/status/warning.png" runat="server" Visible="false" CommandArgument='<%#Eval("RecordKey")%>' OnClick="TaskItem_Click" ToolTip="Pending" style="margin-left: 3px; vertical-align: middle; border: 0px;"/>
					<br />
					<asp:LinkButton ID="LnkTaskItem" runat="server" Text='<%#Eval("LongTitle").ToString().Substring(0,Eval("LongTitle").ToString().IndexOf("-")) %>' CommandArgument='<%#Eval("RecordKey")%>' OnClick="TaskItem_Click" CssClass="radLink" Style="font-weight: bold;"></asp:LinkButton>
					<asp:LinkButton ID="lnkTaskItem1" runat="server" Text=' <%#Eval("LongTitle").ToString().Substring(Eval("LongTitle").ToString().IndexOf("-")+0,(Eval("LongTitle").ToString().Length - Eval("LongTitle").ToString().IndexOf("-")-0 ))%>' CommandArgument='<%#Eval("RecordKey")%>' OnClick="TaskItem_Click" CssClass="radLink" Style="font-weight: normal;"></asp:LinkButton>
					<asp:ImageButton ID="btnTaskDetails" ImageUrl="~/images/defaulticon/16x16/zoom.png" runat="server" Visible="false" OnClientClick='<%# String.Format("OpenTaskDetailWindow(\"{0}\"); return false;", Eval("Description")) %>' ToolTip="view task details" style="margin-left: 3px; vertical-align: middle; border: 0px;"/>
					<br />
					<hr style=" border-style: none; color: #efefef; background-color: #efefef;  height: 3px;"/>
				</div>
			</ItemTemplate>
			<FooterTemplate></table></FooterTemplate>
		</asp:Repeater>
	</div>
	<asp:Label runat="server" ID="lblTaskStripEmpty" Height="40" Text="Task list is empty." class="GridEmpty" Visible="false"></asp:Label>
</asp:Panel>


<telerik:RadWindow runat="server" ID="RadWindow_TaskDetail" RestrictionZoneID="ContentTemplateZone" Modal="true" Height="300" Width="400" Title="Task Details" >
	<ContentTemplate>
		<div>
			<center>
				<br />
				<asp:Label ID="lblTaskDetail" runat="server" Text="info" CssClass="textStd"></asp:Label>
				<br />
				<br />
				<asp:Button runat="server" CssClass="buttonStd" Text="Close" OnClientClick="CloseTaskDetailWindow();"/>
			</center>
		</div>
	</ContentTemplate>
</telerik:RadWindow>
