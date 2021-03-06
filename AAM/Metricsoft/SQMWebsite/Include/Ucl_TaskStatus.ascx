﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_TaskStatus.ascx.cs" Inherits="SQM.Website.Ucl_TaskStatus" %>

<%@ Register src="~/Include/Ucl_Attach.ascx" TagName="AttachWin" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

	<script type="text/javascript">

		function OpenAssignTaskWindow() {
			$find("<%=winAssignTask.ClientID %>").show();
		}

	</script>


<asp:Panel ID="pnlUpdateTask" runat="server" Visible = "False">
	<div class="container-fluid" style="margin-top: 10px;">
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskType" runat="server" Text="<%$ Resources:LocalizedText, Task %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblTaskTypeValue" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskDescription" runat="server" Text="<%$ Resources:LocalizedText, Description %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<%--<asp:TextBox ID="tbTaskDescription" runat="server" Rows="4" Width="98%" TextMode="MultiLine" CssClass="textStd"></asp:TextBox>--%>
				<asp:Label ID="lbTaskDescription" runat="server" Rows="4" Width="98%" TextMode="MultiLine" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskDetail" runat="server" Text="Original Details" CssClass="prompt" meta:resourcekey="lblTaskDetailResource1"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblTaskDetailValue" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblAssignPersonUpdate" runat="server" Text="Assign To Person" CssClass="prompt" meta:resourcekey="lblAssignPersonResource1"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblAssignPersonValue" runat="server" CssClass="textStd"></asp:Label>
			</div>

		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblCreatedBy" runat="server" Text="<%$ Resources:LocalizedText, CreatedBy %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblCreatedByValue" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblTaskDueDT" runat="server" Text="<%$ Resources:LocalizedText, DueDate %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<telerik:RadDatePicker ID="rdpTaskDueDT" Skin="Metro" Width="278px" runat="server" ShowPopupOnFocus="True">
					<Calendar runat="server" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
					</Calendar>
					<DateInput runat="server" DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</DateInput>
					<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
				</telerik:RadDatePicker>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskStatus" runat="server" Text="Current Status" CssClass="prompt" meta:resourcekey="lblTaskStatusResource1"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblTaskStatusValue" runat="server" CssClass="textEmphasis"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 84px;">
				<asp:Label ID="lblTaskComments" runat="server" Text="<%$ Resources:LocalizedText, Comments %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:TextBox ID="tbTaskComments" Rows="4" Width="98%" TextMode="MultiLine" runat="server" CssClass="textStd"></asp:TextBox>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblAttachments" runat="server" Text="<%$ Resources:LocalizedText, Evidence %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:LinkButton ID="lnkAttachments" runat="server" CssClass="buttonAttach buttonPopupOpen" visible="false" Text="<%$ Resources:LocalizedText, Attachments %>" ToolTip="<%$ Resources:LocalizedText, Attachments %>" OnClick="lnkAddAttach"></asp:LinkButton>
				<Ucl:AttachWin ID="uclAttachWin" runat="server" />
			</div>
		</div>
		<br />
		<div style="float: right; margin: 5px;">
			<span>
				<asp:Button ID="btnTaskUpdate" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin: 5px;" OnClientClick="<%$ Resources:LocalizedText, TaskUpdateConfirm %>" onclick="btnTaskUpdate_Click" ToolTip="Update this Task"></asp:Button>
				<asp:Button ID="btnTaskComplete" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Completed %>" style="margin: 5px;" OnClientClick="<%$ Resources:LocalizedText, TaskCompleteConfirm %>" onclick="btnTaskComplete_Click" ToolTip="Update this Task as completed"></asp:Button>
				<asp:Button ID="btnTaskAssign" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Reassign %>" style="margin: 5px;" OnClientClick="<%$ Resources:LocalizedText, TaskReAssignConfirm %>" onclick="btnTaskAssign_Click" ToolTip="<%$ Resources:LocalizedText, ReassignToolTip %>"></asp:Button>
				<asp:Button ID="btnTaskLink" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, ActionPageLink %>" style="margin: 5px;" onclick="btnTaskLink_Click" ToolTip="<%$ Resources:LocalizedText, ActionPageLinkToolTip %>" Visible="false"></asp:Button>
				<asp:Button ID="btnTaskCancel" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnTaskCancel_Click"></asp:Button>
			</span>
		</div>
	</div>
</asp:Panel>

<asp:Panel runat="server" ID="pnlAddTask" Visible="False">
	<asp:Panel runat="server" ID="pnlListTasks" CssClass="container-fluid" >
		<asp:Repeater runat="server" ID="rptTaskList" ClientIDMode="AutoID" OnItemDataBound="rptTaskList_OnItemDataBound" OnItemCreated="rptTaskList_OnItemCreate">
			<FooterTemplate></FooterTemplate>
			<HeaderTemplate>
				<div class="row" style="font-weight:bold;">
					<div class="col-xs-6 col-sm-3 tanLabelCol">Due Date</div>
					<div class="col-xs-6 col-sm-3 tanLabelCol">Responsible</div>
					<div class="tanLabelCol col-xs-10 col-sm-4">Task</div>
					<div class="col-xs-2 tanLabelCol">&nbsp;</div>
				</div>
			</HeaderTemplate>
			<ItemTemplate>
				<div runat="server" class="row">
					<div class="col-xs-6 col-sm-3"><asp:LinkButton ID="lblDueDate" runat="server" Text='<%# Eval("Task.DUE_DT", "{0:MM/dd/yyyy}") %>' CommandArgument='<%# Eval("Task.TASK_ID").ToString() %>' OnClick="lblDueDate_OnClick"></asp:LinkButton><br /><asp:Label runat="server" ID="lblTaskStatus"></asp:Label></div>
					<div class="col-xs-6 col-sm-3"><asp:Label ID="lblLocation" runat="server" Text='<%# formatName(Eval("Person")) %>'></asp:Label></div>
					<div class="col-xs-10 col-sm-4"><asp:Label ID="lblTaskItem" runat="server" Text='<%# Eval("LongTitle").ToString() %>'></asp:Label></div>
					<div class="col-xs-2"><asp:LinkButton runat="server" ID="lnkDeleteTask" CommandArgument='<%# Eval("Task.TASK_ID").ToString() %>' Text="<%$ Resources:LocalizedText, Delete %>" OnClientClick="return confirmAction('Delete Task');" OnClick="lnkDeleteTask_OnClick"></asp:LinkButton></div>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</asp:Panel>

	<div class="container-fluid" style="margin-top: 10px;">
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskTypeAdd" runat="server" Text="<%$ Resources:LocalizedText, Task %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblTaskTypeValueAdd" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskDetailAdd" runat="server" Text="Original Details" CssClass="prompt" meta:resourcekey="lblTaskDetailResource1"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblTaskDetailValueAdd" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskDescriptionAdd" runat="server" Text="<%$ Resources:LocalizedText, Description %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:TextBox ID="tbTaskDescriptionAdd" runat="server" Rows="4" Width="98%" TextMode="MultiLine" CssClass="textStd"></asp:TextBox>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblTaskDueDTAdd" runat="server" Text="<%$ Resources:LocalizedText, DueDate %>" CssClass="prompt"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<telerik:RadDatePicker ID="rdpTaskDueDTAdd" Skin="Metro" Width="278px" runat="server" ShowPopupOnFocus="True">
					<Calendar runat="server" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
					</Calendar>
					<DateInput runat="server" DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</DateInput>
					<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
				</telerik:RadDatePicker>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblPlantAdd" runat="server" Text="<%$ Resources:LocalizedText, BusinessLocation %>" CssClass="prompt" ></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<span>
					<telerik:RadComboBox ID="ddlScheduleScopeAdd" runat="server" Skin="Metro" Width="280px" ZIndex="10" Font-Size="Small"
						AutoPostBack="True" OnSelectedIndexChanged="ScheduleScope_Select" ToolTip="<%$ Resources:LocalizedText, SelectBusinessLocation %>">
					</telerik:RadComboBox>
					<telerik:RadMenu ID="mnuScheduleScopeAdd" runat="server" Width="280px" Style="z-index: 9000 !important;" EnableAutoScroll="True" OnItemClick="ScheduleScope_Select">
						<DefaultGroupSettings RepeatDirection="Horizontal" Flow="Vertical" />
					</telerik:RadMenu>
				</span>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblAssignPersonAdd" runat="server" Text="Assign To Person*" CssClass="prompt" meta:resourcekey="lblAssignPersonResource1"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<telerik:RadComboBox ID="ddlAssignPersonAdd" runat="server" Skin="Metro" ZIndex="9000" Width="90%" Height="330px" EmptyMessage="select person" meta:resourcekey="ddlAssignPersonResource1"></telerik:RadComboBox>
				<div style="background-color: #FFFFFF; background-image: url(/images/requiredAlt.gif); background-repeat: no-repeat; background-position: center; width: 10px; float: right; margin-right: 5px;">&nbsp;</div>
			</div>

		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskStatusAdd" runat="server" Text="Current Status" CssClass="prompt" meta:resourcekey="lblTaskStatusResource1"></asp:Label>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlColX">
				<asp:Label ID="lblTaskStatusValueAdd" runat="server" CssClass="textEmphasis"></asp:Label>
			</div>
		</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol">
					<asp:Label ID="Label1" runat="server" Text="<%$ Resources:LocalizedText, CreatedBy %>" CssClass="prompt"></asp:Label>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlColX">
					<asp:Label runat="server" ID="lblCreatedByAdd" CssClass="textEmphasis"></asp:Label>
				</div>
				
			</div>
		<br />
		<div style="float: right; margin: 5px;">
			<span>
				<asp:Button ID="btnTaskAdd" CSSclass="buttonStd" runat="server" Text="Create" style="margin: 5px;" OnClientClick="<%$ Resources:LocalizedText, TaskCreateConfirm %>" onclick="btnTaskAdd_Click" ToolTip="Create this Task as open" meta:resourcekey="btnTaskAddResource1"></asp:Button>
				<asp:Button ID="btnTaskCancelAdd" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnTaskCancel_Click"></asp:Button>
			</span>
			<br />
			<span style="text-align: center">
				<asp:Label id="lblErrRequiredInputs" runat="server" Visible="false" Text="<%$ Resources:LocalizedText, ENVProfileRequiredsMsg %>"/>
				<asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
			</span>
		</div>
	</div>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winAssignTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="300px" Width="700px" Behaviors="Move" Title="Re-Assign Task" Behavior="Move">
	<ContentTemplate>
		<div class="container-fluid" style="margin-top: 10px;">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
					<asp:Label ID="lblPlant" runat="server" Text="<%$ Resources:LocalizedText, BusinessLocation %>" CssClass="prompt"></asp:Label>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlColX">
					<telerik:RadComboBox ID="ddlScheduleScope" runat="server" Skin="Metro" Width="280px" ZIndex="10" Font-Size="Small"
						AutoPostBack="True" OnSelectedIndexChanged="ScheduleScope_Select" ToolTip="<%$ Resources:LocalizedText, SelectBusinessLocation %>">
					</telerik:RadComboBox>
					<telerik:RadMenu ID="mnuScheduleScope" runat="server" Width="280px" Style="z-index: 9;" EnableAutoScroll="True" OnItemClick="ScheduleScope_Select">
						<DefaultGroupSettings RepeatDirection="Horizontal" />
					</telerik:RadMenu>
				</div>
			</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
					<asp:Label ID="lblAssignPerson" runat="server" Text="Assign To Person" CssClass="prompt" meta:resourcekey="lblAssignPersonResource1"></asp:Label>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlColX">
					<telerik:RadComboBox ID="ddlAssignPerson" runat="server" Skin="Metro" ZIndex="9000" Width="90%" Height="330px" EmptyMessage="select person" meta:resourcekey="ddlAssignPersonResource1"></telerik:RadComboBox>
				</div>
			</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 84px;">
					<asp:Label ID="lblAssignComment" runat="server" Text="<%$ Resources:LocalizedText, Comments %>" CssClass="prompt"></asp:Label>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlColX">
					<asp:TextBox ID="tbAssignComment" Rows="4" Width="98%" TextMode="MultiLine" runat="server" CssClass="textStd"></asp:TextBox>
				</div>
			</div>
			<br />
			<div style="float: right; margin: 5px;">
				<span>
					<asp:Button ID="btnAssignSave" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Reassign %>" style="margin: 5px;" OnClientClick="<%$ Resources:LocalizedText, TaskReAssignConfirm %>" onclick="btnTaskAssignUpdate_Click" ToolTip="<%$ Resources:LocalizedText, ReassignToolTip %>"></asp:Button>
					<asp:Button ID="btnAssignCancel" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnTaskCancel_Click"></asp:Button>
				</span>
			</div>
		</div>
	</ContentTemplate>
</telerik:RadWindow>
