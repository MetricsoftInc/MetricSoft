<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_TaskStatus.ascx.cs" Inherits="SQM.Website.Ucl_TaskStatus" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

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
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
				<asp:Label ID="lblSetStatus" runat="server" Text="Update Status" CssClass="prompt"></asp:Label>
			</div>	
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadComboBox ID="ddlTaskStatus" runat="server" Skin="Metro" ZIndex="9000" Height="100" Width="300" Font-Size="Small" 
					ToolTip="Select to update this task status" EmptyMessage="update status" AutoPostBack="false">
					<Items>
						<telerik:RadComboBoxItem text="" Value=""/>
						<telerik:RadComboBoxItem text="Completed" Value="2"/>
						<telerik:RadComboBoxItem text="Re-Assign" Value="91"/>
					</Items>
				</telerik:RadComboBox>
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
				<asp:Button ID="btnTaskUpdate" CSSclass="buttonEmphasis" runat="server" text="Update Task" style="margin: 5px;" OnClientClick="return confirmChange('this Task');" onclick="btnTaskUpdate_Click"></asp:Button>
				<asp:Button ID="btnTaskCancel" CSSclass="buttonStd" runat="server" text="Cancel" style="margin: 5px;" OnClick="btnTaskCancel_Click"></asp:Button>
			</span>
        </div>					
	</div>
</asp:Panel>