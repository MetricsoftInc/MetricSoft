<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PrivGroupList.ascx.cs" Inherits="SQM.Website.Ucl_PrivGroupList" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
    function OpenPrivGroupEditWindow() {
        $find("<%=winPrivGroupEdit.ClientID %>").show();
    }

    function OpenPrivEditWindow() {
        $find("<%=winPrivEdit.ClientID %>").show();
    }

    function CloseUSerEditWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window
        oWindow.Close();
    }

	</script>


<asp:Panel ID="pnlPrivGroups" runat="server" CssClass="admBkgd" Visible="false">
	<asp:HiddenField ID="hfPrivGroupContext" runat="server" />
	<asp:HiddenField ID="hfPrivGroupBusLoc" runat="server" />
	<asp:Button id="btnAddPrivGroup" runat="server" CssClass="buttonAddLarge" style="float: left; margin: 5px;" Text="Add Privilege Group" ToolTip="Add a privilege group" OnClick="btnPrivGroupAdd_Click"/>
	<telerik:RadGrid ID="rgPrivGroup" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
		AutoGenerateColumns="false" OnItemDataBound="rgPrivGroup_ItemDataBound" OnSortCommand="rgPrivGroup_SortCommand"
		OnPageIndexChanged="rgPrivGroup_PageIndexChanged" OnPageSizeChanged="rgPrivGroup_PageSizeChanged" GridLines="None" Width="100%" OnItemCommand="rgPrivList_ItemCommand">
		<MasterTableView ExpandCollapseColumn-Visible="true" DataKeyNames="PRIV_GROUP" GroupsDefaultExpanded="false">
            <NestedViewTemplate>
                <telerik:RadGrid runat="server" ID="rgPrivList" OnNeedDataSource="rgPrivList_NeedDataSource" Width="100%" AutoGenerateColumns="False" OnItemDataBound="rgPrivList_ItemDataBound" GroupPanelPosition="Top">
                    <MasterTableView DataKeyNames="PRIV, SCOPE" GroupsDefaultExpanded="true">
                        <ExpandCollapseColumn Visible="False">
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" UniqueName="TemplateColumn">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnPrivGroup" runat="server" Value='<%# Eval("PRIV_GROUP") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Scope" ShowSortIcon="false" FilterControlAltText="Filter Scope column">
                                <ItemTemplate>
                                    <asp:Label ID="lblScope" runat="server" Text='<%# Eval("SCOPE") %>'></asp:Label><asp:HiddenField runat="server" ID="hdnScope" Value='<%# Eval("SCOPE") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Privilege" ShowSortIcon="false" FilterControlAltText="Filter Privilege column">
                                <ItemTemplate>
                                    <asp:Label ID="lblPrivDesc" runat="server" Text='<%# Eval("PRIV") %>'></asp:Label><asp:HiddenField runat="server" ID="hdnPriv" Value='<%# Eval("PRIV") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" UniqueName="TemplateColumn3">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDeletePriv" runat="server" meta:resourcekey="lnkDeletePrivResource1" OnClientClick="return confirmAction('Delete This Privilege/Scope');" onclick="OnDeletePriv_Click" Text="Delete Privilege" ToolTip="Delete this Privilege/Scope record from the group" CssClass="buttonLink"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </NestedViewTemplate>
			<Columns>
				<telerik:GridTemplateColumn HeaderText="Privilege Group" ShowSortIcon="true" SortExpression="PRIV_GROUP">
					<ItemTemplate>
						<asp:LinkButton ID="lnkPrivGroupItem" OnClick="lnklPrivGroupItem_Click" CommandArgument='<%#Eval("PRIV_GROUP") %>' Text='<%#Eval("PRIV_GROUP") %>' runat="server" CssClass="buttonLink" Font-Bold="true" ForeColor="#000066" ToolTip="<%$ Resources:LocalizedText, Edit %>"></asp:LinkButton>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="false">
					<ItemTemplate>
						<asp:Label ID="lblDescription" runat="server" Text='<%# Eval("DESCRIPTION") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Status" ShowSortIcon="false">
					<ItemTemplate>
						<asp:Label ID="lblGroupStatus" runat="server" Text='<%# Eval("STATUS") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" UniqueName="TemplateColumn3">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkAddPriv" runat="server" meta:resourcekey="lnkAddPrivResource1" OnClick="lnkAddPriv_Click" Text="Add Privilege" ToolTip="Add a Privilege/Scope record for the group" CssClass="buttonLink"></asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
		</MasterTableView>
		<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
	</telerik:RadGrid>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winPrivGroupEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="600" Title="Privilege Group Item" Behaviors="Move,Close">
    <ContentTemplate>
		<asp:HiddenField id="hfPrivGroupID" runat="server"/>
		<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft editArea" style="margin: 8px 0 5px 0; ">
                <tr>
                    <td class="columnHeader" width="24%" style="vertical-align: top;">
                        <asp:Label ID="lblEditPrivGroup" runat="server" text="Privilege Group"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:TextBox ID="tbEditPrivGroup"  runat="server" MaxLength="20" CssClass="textStd" style="width: 97%;"/>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%" style="vertical-align: top;">
                        <asp:Label ID="lblEditdescription" runat="server" text="Description"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:TextBox ID="tbEditDescription"  runat="server" TextMode="multiline" rows="3" MaxLength="400" CssClass="textStd" style="width: 97%;"/>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%" style="vertical-align: top;">
                        <asp:Label ID="lblEditPrivGroupStatus" runat="server" text="Status"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlPrivGroupStatus" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnSelectedIndexChanged="ddlEdit_OnIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </td>
                </tr>
			</table>
		</telerik:RadAjaxPanel>
            <span style="float: right; margin: 10px;">
                <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" onclick="OnCancelPrivGroup_Click"></asp:Button>
                <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text ="<%$ Resources:LocalizedText, Save %>" style="width: 70px;" OnClientClick="return confirmChange('PrivGroup');" onclick="OnSavePrivGroup_Click"></asp:Button>
				&nbsp;
				<%--<asp:Button ID="btnDelete" class="buttonLink" runat="server" Text="<%$ Resources:LocalizedText, Delete %>" style="width: 70px;" OnClientClick="return confirmAction('Delete This Privilege Group');" onclick="OnDeletePrivGroup_Click"></asp:Button>--%>
            </span>
            <br />
            <center>
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
            </center>
	</ContentTemplate>
</telerik:RadWindow>

<telerik:RadWindow runat="server" ID="winPrivEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="600" Title="Privilege Scope Item" Behaviors="Move,Close">
    <ContentTemplate>
		<asp:HiddenField id="hfPrivPrivGroupID" runat="server"/>
		<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2">
            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft editArea" style="margin: 8px 0 5px 0; ">
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditScope" runat="server" text="Scope"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScope" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnSelectedIndexChanged="ddlScope_OnIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditPriv" runat="server" text="Privilege"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlPriviledge" runat="server" Skin="Metro" ZIndex="9000" width=300 EmptyMessage="<%$ Resources:LocalizedText, Select %>"></telerik:RadComboBox>
                    </td>
                </tr>
			</table>
		</telerik:RadAjaxPanel>
            <span style="float: right; margin: 10px;">
                <asp:Button ID="btnCancelPriv" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" onclick="OnCancelPriv_Click"></asp:Button>
                <asp:Button ID="btnSavePriv" class="buttonEmphasis" runat="server" Text ="<%$ Resources:LocalizedText, Save %>" style="width: 70px;" OnClientClick="return confirmChange('Priv');" onclick="OnSavePriv_Click"></asp:Button>
				<%--&nbsp;
				<asp:Button ID="btnDeletePriv" class="buttonLink" runat="server" Text="<%$ Resources:LocalizedText, Delete %>" style="width: 70px;" OnClientClick="return confirmAction('Delete This Privilege/Scope');" onclick="OnDeletePriv_Click"></asp:Button>--%>
            </span>
            <br />
            <center>
                <asp:Label ID="lblPrivErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
            </center>
	</ContentTemplate>
</telerik:RadWindow>