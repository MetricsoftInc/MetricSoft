﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_NotifyList.ascx.cs" Inherits="SQM.Website.Ucl_NotifyList" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
    function OpenNotifyEditWindow() {
        $find("<%=winNotifyEdit.ClientID %>").show();
    }

    function CloseUSerEditWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window
        oWindow.Close();
    }

	</script>


<asp:Panel ID="pnlNotifyAction" runat="server" CssClass="admBkgd" Visible="false">
	<asp:HiddenField ID="hfNotifyActionContext" runat="server" />
	<asp:HiddenField ID="hfNotifyActionBusLoc" runat="server" />
	<asp:Button id="btnAddNotifyAction" runat="server" CssClass="buttonAddLarge" style="float: left; margin: 5px;" Text="Add Notification" ToolTip="Add a notification plan item" OnClick="btnNotifyItemAdd_Click"/>
	<telerik:RadGrid ID="rgNotifyAction" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
		AutoGenerateColumns="false" OnItemDataBound="rgNotifyAction_ItemDataBound" OnSortCommand="rgNotifyAction_SortCommand"
		OnPageIndexChanged="rgNotifyAction_PageIndexChanged" OnPageSizeChanged="rgNotifyAction_PageSizeChanged" GridLines="None" Width="100%">
		<MasterTableView ExpandCollapseColumn-Visible="false">
			<Columns>
				<telerik:GridTemplateColumn HeaderText="Notification Scope" ShowSortIcon="true" SortExpression="NOTIFY_SCOPE">
					<ItemTemplate>
						<asp:HiddenField id="hfNotifyItemID" runat="server"/>
						<asp:LinkButton ID="lnkNotifyItem" OnClick="lnklNotifyItem_Click" CommandArgument='<%#Eval("NOTIFYACTION_ID") %>' Text='<%#Eval("NOTIFY_SCOPE") %>' runat="server" CssClass="buttonLink" Font-Bold="true" ForeColor="#000066" ToolTip="<%$ Resources:LocalizedText, EditIncident %>"></asp:LinkButton>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="For Event" ShowSortIcon="false">
					<ItemTemplate>
						<asp:Label ID="lblScopeTask" runat="server" Text='<%# Eval("SCOPE_TASK") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Notify When" ShowSortIcon="false">
					<ItemTemplate>
						<asp:Label ID="lblScopeStatus" runat="server" Text='<%# Eval("TASK_STATUS") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Send Notification">
					<ItemTemplate>
						<asp:Label ID="lblNotifyTiming" runat="server" Text='<%# Eval("NOTIFY_TIMING") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Notify To Group(s)">
					<ItemTemplate>
						<asp:Label ID="lblNotifyDist" runat="server" Text='<%# Eval("NOTIFY_DIST") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
			</Columns>
		</MasterTableView>
		<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
	</telerik:RadGrid>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winNotifyEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="600" Title="Notification Item" Behaviors="Move,Close">
    <ContentTemplate>
		<asp:HiddenField id="hfNotifyActionID" runat="server"/>
		<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft editArea" style="margin: 8px 0 5px 0; ">
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditNotifyScope" runat="server" text="Notification Scope"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlNotifyScope" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnSelectedIndexChanged="ddlEdit_OnIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditScopeTask" runat="server" text="For Event"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScopeTask" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnSelectedIndexChanged="ddlEdit_OnIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditScopeStatus" runat="server" text="Notify When"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScopeStatus" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnSelectedIndexChanged="ddlEdit_OnIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </td>
                </tr>
               <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditNotifyTiming" runat="server" text="Send Notification"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScopeTiming" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>"></telerik:RadComboBox>
                    </td>
                </tr>
               <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblNotifyDist" runat="server" text="Notify To Group(s)"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlNotifyPrivGroup" runat="server" Skin="Metro" ZIndex="9000" width=300 Height=300 EmptyMessage="Select privilege group" CheckBoxes="true" EnableCheckAllItemsCheckBox="false"></telerik:RadComboBox>
                    </td>
                </tr>
			</table>
		</telerik:RadAjaxPanel>
            <span style="float: right; margin: 10px;">
                <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" onclick="OnCancelNotifyAction_Click"></asp:Button>
                <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text ="<%$ Resources:LocalizedText, Save %>" style="width: 70px;" OnClientClick="return confirmChange('Notification');" onclick="OnSaveNotifyAction_Click"></asp:Button>
				&nbsp;
				<asp:Button ID="btnDelete" class="buttonLink" runat="server" Text="<%$ Resources:LocalizedText, Delete %>" style="width: 70px;" OnClientClick="return confirmAction('Delete This Notification');" onclick="OnDeleteNotifyAction_Click"></asp:Button>
            </span>
            <br />
            <center>
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
            </center>
	</ContentTemplate>
</telerik:RadWindow>