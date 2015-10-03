<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_TaskStatus.ascx.cs" Inherits="SQM.Website.Ucl_TaskStatus" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

	<script type="text/javascript">
		function OpenAssignTaskWindow() {
			$find("<%=winAssignTask.ClientID %>").show();
		}
	</script> 

<asp:Panel ID="pnlUpdateTask" runat="server" Visible = "false">
	<div class="container-fluid" style="margin-top: 10px;">
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskType" runat="server" Text="Task" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblTaskTypeValue" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskDescription" runat="server" Text="Description" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblTaskDescriptionValue" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:Label ID="lblTaskDetail" runat="server" Text="Original Details" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblTaskDetailValue" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblTaskDueDT" runat="server" Text="Due Date" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpTaskDueDT" Skin="Metro" Width="278" runat="server" ShowPopupOnFocus="true" Enabled="false"></telerik:RadDatePicker>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblTaskStatus" runat="server" Text="Current Status" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblTaskStatusValue" runat="server" CssClass="textEmphasis"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 84px;">
				<asp:Label ID="lblTaskComments" runat="server" Text="Comments" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbTaskComments" Rows="4" Width="98%" TextMode="MultiLine" runat="server" CssClass="textStd"></asp:TextBox>
			</div>
		</div>
		<br />
		<div style="float: right; margin: 5px;">
			<span>
				<asp:Button ID="btnTaskComplete" CSSclass="buttonStd" runat="server" text="Completed" style="margin: 5px;" OnClientClick="return confirmAction('update this task as Complete');" onclick="btnTaskComplete_Click" ToolTip="update this Task as completed"></asp:Button>
				<asp:Button ID="btnTaskAssign" CSSclass="buttonStd" runat="server" text="Re-Assign" style="margin: 5px;" OnClientClick="return confirmAction('re-assign this Task');" onclick="btnTaskAssign_Click" ToolTip="re-assign this task to another person"></asp:Button>
				<asp:Button ID="btnTaskCancel" CSSclass="buttonEmphasis" runat="server" text="Cancel" style="margin: 5px;" OnClick="btnTaskCancel_Click"></asp:Button>
			</span>
        </div>					
	</div>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winAssignTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="300" Width="700" Behaviors="Move" Title="Re-Assign Task">
	<ContentTemplate>
		<div class="container-fluid" style="margin-top: 10px;">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
					<asp:Label ID="lblAssignPerson" runat="server" Text="Assign To Person" CssClass="prompt"></asp:Label>
				</div>	
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<telerik:RadComboBox ID="ddlAssignPerson" runat="server" Skin="Metro" ZIndex="9000" Width="90%" Height="330" AutoPostBack="false" EmptyMessage="select person"></telerik:RadComboBox>
				</div>
			</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 84px;">
					<asp:Label ID="lblAssignComment" runat="server" Text="Comments" CssClass="prompt"></asp:Label>
				</div>	
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbAssignComment" Rows="4" Width="98%" TextMode="MultiLine" runat="server" CssClass="textStd"></asp:TextBox>
				</div>
			</div>
			<br />
			<div style="float: right; margin: 5px;">
				<span>
					<asp:Button ID="btnAssignSave" CSSclass="buttonStd" runat="server" text="Re-Assign" style="margin: 5px;" OnClientClick="return confirmAction('re-assign this Task');" onclick="btnTaskAssignUpdate_Click" ToolTip="re-assign this task to another person"></asp:Button>
					<asp:Button ID="btnAssignCancel" CSSclass="buttonEmphasis" runat="server" text="Cancel" style="margin: 5px;" OnClick="btnTaskCancel_Click"></asp:Button>
				</span>
			</div>
		</div>
	</ContentTemplate>
</telerik:RadWindow>