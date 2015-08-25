<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_NotifyList.ascx.cs" Inherits="SQM.Website.Ucl_NotifyList" %>

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

   <asp:Panel ID="pnlNotifyList" runat="server" Visible = "false">
        <asp:HiddenField id="hfNotifyCompanyID" runat="server"/>
        <asp:HiddenField id="hfNotifyBusorgID" runat="server"/>
        <asp:HiddenField id="hfNotifyPlantID" runat="server"/>
        <asp:HiddenField id="hfNotifyB2BID" runat="server"/>
        <table width="99%" Class="admBkgd">
            <tr>
                <td>
                    <asp:Label ID="lblNotifyInstruct" runat="server" CssClass="instructText" Text="Identify users which receive automatic notifications when Quality or EHS incidents are created. Also define escalation paths for overdue tasks."></asp:Label>
                </td>
            </tr>
            <tr>
                <td  class="optionArea">
                    <asp:Button ID="btnSaveNotify" CSSclass="buttonEmphasis" runat="server" text="Save List" OnClientClick="return confirmChange('Notification List');"  onclick="btnNotifySave_Click"  
                        CommandArgument="team"></asp:Button>
                </td>
            </tr>
            <tr>
                <td align=center>
                    <div id="divNotifyListScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvNotifyList" Name="gvNotifyList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="0" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvNotifyList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellTextLeft" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Incident Type Or Task">
						                <ItemTemplate>
                                            <asp:HiddenField ID="hfNotifyID" runat="server" value='<%#Eval("NOTIFY_ID") %>'/>
                                            <asp:HiddenField ID="hfScope" runat="server" value='<%#Eval("NOTIFY_SCOPE") %>'></asp:HiddenField>
                                            <asp:HiddenField ID="hfNotify1" runat="server" value='<%#Eval("NOTIFY_PERSON1")  %>'/>
                                            <asp:HiddenField ID="hfNotify2" runat="server" value='<%#Eval("NOTIFY_PERSON2")  %>'/>
                                            <asp:HiddenField ID="hfEscalate1" runat="server" value='<%#Eval("ESCALATE_PERSON1")  %>'/>
                                            <asp:HiddenField ID="hfEscalate2" runat="server" value='<%#Eval("ESCALATE_PERSON2")  %>'/>
                                            <asp:HiddenField ID="hfEscalateDays1" runat="server" value='<%#Eval("ESCALATE_DAYS1") %>'/>
                                            <asp:HiddenField ID="hfEscalateDays2" runat="server" value='<%#Eval("ESCALATE_DAYS2")  %>'/>
                                            <asp:Label ID="lblScope" runat="server" CssClass="prompt" text='<%#Eval("NOTIFY_SCOPE") %>'></asp:Label>
                                            <br />
                                            <asp:Label ID="lblScopeDesc" runat="server" CssClass="refTextSmall"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notify User When Incident Is Created" ItemStyle-Width="220px">
						                <ItemTemplate>
                                            <telerik:RadComboBox ID="ddlNotify1" runat="server" Visible="false" style="width: 99%;" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="Select 1st person to notify"></telerik:RadComboBox>
                                            <br />
                                            <telerik:RadComboBox ID="ddlNotify2" runat="server" Visible="false" style="width: 99%;" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="Select 2nd person to notify"></telerik:RadComboBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Escalate To User When Task Is Overdue" ItemStyle-Width="303px">
						                <ItemTemplate>
                                            <telerik:RadComboBox ID="ddlEscalate1" runat="server" Visible="false"  Width=217 Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="Escalate to person"></telerik:RadComboBox>
                                            <telerik:RadComboBox ID="ddlEscalateDays1" runat="server" Visible="false"  Width=80 Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="Days" ToolTip="Number of days over due date"></telerik:RadComboBox>
                                            <br />
                                            <telerik:RadComboBox ID="ddlEscalate2" runat="server" Visible="false"  Width=217 Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="Escalate to person"></telerik:RadComboBox>
                                            <telerik:RadComboBox ID="ddlEscalateDays2" runat="server" Visible="false"  Width=80 Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="Days" ToolTip="Number of days over due date"></telerik:RadComboBox>
                                        </ItemTemplate>
					                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblNotifyListEmpty" Height="40" Text="List is empty." class="GridEmpty" Visible="false"></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlTasksResponsibleList" Visible="true" >
    </asp:Panel>

<asp:Panel ID="pnlNotifyAction" runat="server" Visible="false">
	<asp:HiddenField ID="hfNotifyActionContext" runat="server" />
	<asp:HiddenField ID="hfNotifyActionBusLoc" runat="server" />
	<asp:Button id="btnAddNotifyAction" runat="server" CssClass="buttonAddLarge" style="float: right; margin-right: 25px;" Text="Add Notification" ToolTip="Add a notification plan item" OnClick="btnNotifyItemAdd_Click"/>
	<telerik:RadGrid ID="rgNotifyAction" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
		AutoGenerateColumns="false" OnItemDataBound="rgNotifyAction_ItemDataBound" OnSortCommand="rgNotifyAction_SortCommand"
		OnPageIndexChanged="rgNotifyAction_PageIndexChanged" OnPageSizeChanged="rgNotifyAction_PageSizeChanged" GridLines="None" Width="100%">
		<MasterTableView ExpandCollapseColumn-Visible="false">
			<Columns>
				<telerik:GridTemplateColumn HeaderText="Notification Scope" ShowSortIcon="true" SortExpression="NOTIFY_SCOPE">
					<ItemTemplate>
						<asp:HiddenField id="hfNotifyItemID" runat="server"/>
						<asp:LinkButton ID="lnkNotifyItem" OnClick="lnklNotifyItem_Click" CommandArgument='<%#Eval("NOTIFYACTION_ID") %>' Text='<%#Eval("NOTIFY_SCOPE") %>' runat="server" CssClass="buttonLink" Font-Bold="true" ForeColor="#000066" ToolTip="Edit incident"></asp:LinkButton>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Notify For" ShowSortIcon="false">
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
				<telerik:GridTemplateColumn HeaderText="Notify Jobcode">
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
                        <telerik:RadComboBox ID="ddlNotifyScope" runat="server" Skin="Metro" ZIndex="9000" EmptyMessage="Select"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditScopeTask" runat="server" text="Notify For"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScopeTask" runat="server" Skin="Metro" ZIndex="9000" EmptyMessage="Select"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditScopeStatus" runat="server" text="Notify When"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScopeStatus" runat="server" Skin="Metro" ZIndex="9000" EmptyMessage="Select"></telerik:RadComboBox>
                    </td>
                </tr>
               <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblEditNotifyTiming" runat="server" text="Send Notification"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlScopeTiming" runat="server" Skin="Metro" ZIndex="9000" EmptyMessage="Select"></telerik:RadComboBox>
                    </td>
                </tr>
               <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblNotifyDist" runat="server" text="Notify Jobcode"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <telerik:RadComboBox ID="ddlNotifyJobcode" runat="server" Skin="Metro" ZIndex="9000" width=300 Height="400" EmptyMessage="Select Jobcode"></telerik:RadComboBox>
                    </td>
                </tr>
			</table>
		</telerik:RadAjaxPanel>
            <span style="float: right; margin: 10px;">
                <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="Cancel" style="width: 70px;" onclick="OnCancelNotifyAction_Click"></asp:Button>
                <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text ="Save" style="width: 70px;" OnClientClick="return confirmChange('Notification');" onclick="OnSaveNotifyAction_Click"></asp:Button>
            </span>
            <br />
            <center>
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
            </center>
	</ContentTemplate>
</telerik:RadWindow>