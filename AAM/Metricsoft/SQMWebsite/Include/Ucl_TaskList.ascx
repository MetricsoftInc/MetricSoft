<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_TaskList.ascx.cs" Inherits="SQM.Website.Ucl_TaskList" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IncidentList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_EHSList.ascx" TagName="EHSInput" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_TaskStatus.ascx" TagName="Task" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
	function OpenTaskDetailWindow(details) {
		$get("<%=lblTaskDetail.ClientID %>").innerHTML = details;
		$find("<%=RadWindow_TaskDetail.ClientID %>").show();
	}

	function OpenTaskWindow() {
			$find("<%=winUpdateTask.ClientID %>").show();
		}


	function OnClientAppointmentEditing(sender, eventArgs) {
		eventArgs.set_cancel(true);
	}

	function CloseTaskDetailWindow() {
		var oWindow = GetRadWindow();  //Obtaining a reference to the current window
		oWindow.Close();
	}
</script>

<asp:Panel ID="pnlTaskSchedule" runat="server" Visible="False">
	<telerik:RadAjaxPanel runat="server" ID="RadAjaxSchedule" HorizontalAlign="NotSet">
		<telerik:RadScheduler ID="scdTaskSchedule" name="scdTaskSchedule" RenderMode="Auto" runat="server" Font-Size="X-Small" Skin="Metro"
			SelectedView="MonthView" OnAppointmentDataBound="scdTaskSchedule_OnDataBound"  OnAppointmentCreated="scdTaskSchedule_OnCreated"
			 DataKeyField ="RecordKey" DataSubjectField="LongTitle" DataStartField="StartDate" DataEndField="EndDate" DataDescriptionField="Description"
			 AllowEdit="False" AllowInsert="False" ReadOnly="True" RowHeight = "60"
			OverflowBehavior="Expand" EnableAdvancedForm="False" EnableResourceEditing="False" >
				<ExportSettings>
					<Pdf PageBottomMargin="1in" PageLeftMargin="1in" PageRightMargin="1in" PageTopMargin="1in" />
				</ExportSettings>
				<AdvancedForm Modal="True" EnableResourceEditing="False" Enabled="False" ></AdvancedForm>
				<TimelineView UserSelectable="False" />
				<DayView UserSelectable="true" />
				<MonthView UserSelectable="true" AdaptiveRowHeight="false" HeaderDateFormat="Y" VisibleAppointmentsPerDay="3" />
				<AppointmentContextMenuSettings EnableDefault="True" />
				<TimeSlotContextMenuSettings EnableDefault="True"></TimeSlotContextMenuSettings>
				<AppointmentTemplate>
					<div id="divSchedule" runat="server" class="taskCalendar">
						<asp:LinkButton ID="lnkScheduleItem" runat="server" Text='<%# Eval("Subject").ToString().Substring(0,Eval("Subject").ToString().IndexOf("-")) %>' CommandArgument='<%# Eval("ID") %>' OnClick="TaskItem_Click" Style="font-weight: bold; color: black;"></asp:LinkButton>
						<asp:LinkButton ID="lnkScheduleItem2" runat="server" Text='<%# Eval("Subject").ToString().Substring(Eval("Subject").ToString().IndexOf("-")+0,(Eval("Subject").ToString().Length - Eval("Subject").ToString().IndexOf("-")-0 )) %>' CommandArgument='<%# Eval("ID") %>' OnClick="TaskItem_Click" Style="color: black;"></asp:LinkButton>
						<div id="divInactive" runat="server" visible="False">
							<strong><%#Eval("Subject").ToString().Substring(0,Eval("Subject").ToString().IndexOf('-')) %></strong>
							<%#Eval("Subject").ToString().Substring(Eval("Subject").ToString().IndexOf('-')+0,(Eval("Subject").ToString().Length - Eval("Subject").ToString().IndexOf('-')-0))%>
						</div>
						<asp:ImageButton ID="btnTaskDetails" ImageUrl="~/images/defaulticon/16x16/zoom.png" runat="server" Visible="False" OnClientClick='<%# String.Format("OpenTaskDetailWindow(\"{0}\"); return false;", Eval("Description")) %>' ToolTip="View task details" style="margin-left: 3px; vertical-align: middle; border: 0px;" meta:resourcekey="btnTaskDetailsResource1"/>
					</div>
				</AppointmentTemplate>
		</telerik:RadScheduler>
		<asp:HiddenField id="hfScheduleScope" runat="server"/>
	</telerik:RadAjaxPanel>
</asp:Panel>

<asp:Panel ID="pnlTaskStrip" runat="server" Visible="false">
	<div class="navSectionBar"  style ="height: 22px; text-align:center; vertical-align: middle; padding-top: 2px;">
		<asp:Label ID="lblTaskStriptInstruct" runat="server" Text="Tasks Overdue" CssClass="textMedium" style="color: black;" meta:resourcekey="lblTaskStriptInstructResource1"></asp:Label>
		<asp:Image ID="Image1" runat="server" ImageUrl="~/images/status/warning.png"  Style="margin: 0px 3px 1px 3px; vertical-align: middle; border: 0px;" />
		<asp:Label ID="lblTaskStripCount" runat="server" CssClass="textSmall"></asp:Label>
	</div>
	<div id="divTaskStripRepeater" runat="server"  style="border-style: none; margin-top: 7px;">
		<asp:Repeater runat="server" ID="rptTaskStrip" ClientIDMode="AutoID" OnItemDataBound="rptTaskStrip_OnItemDataBound" OnItemCreated="rptTaskStrip_OnItemCreate">
			<FooterTemplate></table></FooterTemplate>
			<HeaderTemplate>
				<table border="0" cellpadding="1" cellspacing="0" width="100%">
				</table>
			</HeaderTemplate>
			<ItemTemplate>
				<div runat="server" class="taskCalendar">
					<asp:Label ID="lblDueDate" runat="server" Style="font-size: 10px; font-weight: bold;" Text='<%# Eval("Task.DUE_DT").ToString() %>'></asp:Label>
					<asp:ImageButton ID="imgTaskStatus" runat="server" CommandArgument='<%# Eval("RecordKey") %>' ImageUrl="~/images/status/warning.png" meta:resourcekey="imgTaskStatusResource1" OnClick="TaskItem_Click" style="margin-left: 3px; vertical-align: middle; border: 0px;" ToolTip="Pending" Visible="False" />
					<br />
					<asp:LinkButton ID="LnkTaskItem" runat="server" CommandArgument='<%# Eval("RecordKey") %>' CssClass="radLink" OnClick="TaskItem_Click" Style="font-weight: bold;" Text='<%# Eval("LongTitle").ToString().Substring(0,Eval("LongTitle").ToString().IndexOf("-")) %>'></asp:LinkButton>
					<asp:LinkButton ID="lnkTaskItem1" runat="server" CommandArgument='<%# Eval("RecordKey") %>' CssClass="radLink" OnClick="TaskItem_Click" Style="font-weight: normal;" Text='<%# Eval("LongTitle").ToString().Substring(Eval("LongTitle").ToString().IndexOf("-")+0,(Eval("LongTitle").ToString().Length - Eval("LongTitle").ToString().IndexOf("-")-0 )) %>'></asp:LinkButton>
					<asp:ImageButton ID="btnTaskDetails" runat="server" ImageUrl="~/images/defaulticon/16x16/zoom.png" OnClientClick='<%# String.Format("OpenTaskDetailWindow(\"{0}\"); return false;", Eval("Description")) %>' style="margin-left: 3px; vertical-align: middle; border: 0px;" ToolTip="View task details" Visible="False" />
					<br />
					<hr style=" border-style: none; color: #efefef; background-color: #efefef;  height: 3px;"/>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
	<asp:Label runat="server" ID="lblTaskStripEmpty" Height="40px" Text="Task list is empty." class="GridEmpty" Visible="False" meta:resourcekey="lblTaskListEmptyResource1"></asp:Label>
</asp:Panel>

<asp:Panel ID="pnlTaskList" runat="server" Visible="False">
	<telerik:RadAjaxPanel>
	<div>
		<telerik:RadGrid ID="rgTaskList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="50"
			AutoGenerateColumns="False" OnItemDataBound="rgTaskList_ItemDataBound" OnSortCommand="rgTaskList_SortCommand"
			OnPageIndexChanged="rgTaskList_PageIndexChanged" OnPageSizeChanged="rgTaskList_PageSizeChanged" Width="100%" GroupPanelPosition="Top">
			<MasterTableView>
				<ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Task ID" meta:resourcekey="GridTemplateColumnResource1" SortExpression="TASK_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbTaskId" runat="server" CommandArgument='<%# Eval("TASK_ID") %>' meta:resourcekey="lbTaskIdResource1" OnClick="lbTaskListItem_Click" Font-Bold="True" ForeColor="#000066" ToolTip="View or update this Task">
											<span style="white-space: nowrap;">
												<img src="/images/ico16-edit.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" /><asp:Label runat="server" Text='<%# string.Format("{0:000000}", Eval("TASK_ID")) %>' Font-Bold="True" ForeColor="#000066" ID="lblTaskId"></asp:Label>

											</span>
										</asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, Type %>" ShowSortIcon="False" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblTaskType" runat="server" Text='<%# Eval("TASK_TYPE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, Description %>" ShowSortIcon="False" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" Text='<%# (string)Eval("DESCRIPTION") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="Create Date" meta:resourcekey="GridTemplateColumnResource4" SortExpression="CREATE_DT" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblCreateDT" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="<%$ Resources:LocalizedText, DueDate %>" SortExpression="DUE_DT" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblDueDT" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="<%$ Resources:LocalizedText, Status %>" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblStatus" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
			</MasterTableView>
			<PagerStyle AlwaysVisible="True"></PagerStyle>
		</telerik:RadGrid>
		<asp:Label runat="server" ID="lblTaskListEmpty" Height="40px" Text="Task list is empty." class="GridEmpty" Visible="False" meta:resourcekey="lblTaskListEmptyResource1"></asp:Label>
	</div>
	</telerik:RadAjaxPanel>
</asp:Panel>


<telerik:RadWindow runat="server" ID="RadWindow_TaskDetail" RestrictionZoneID="ContentTemplateZone" Modal="True" Height="300px" Width="400px" Title="Task Details">
	<ContentTemplate>
		<div>
			<center>
				<br />
				<asp:Label ID="lblTaskDetail" runat="server" Text="" CssClass="textStd"></asp:Label>
				<br />
				<br />
				<asp:Button runat="server" CssClass="buttonStd" Text="<%$ Resources:LocalizedText, Close %>" OnClientClick="CloseTaskDetailWindow();"/>
			</center>
		</div>
	</ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow runat="server" ID="winUpdateTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="400px" Width="700px" Title="View/Update Task" Behavior="Close, Move">
	<ContentTemplate>
		<Ucl:Task ID="uclTask" runat="server" />
	</ContentTemplate>
</telerik:RadWindow>
