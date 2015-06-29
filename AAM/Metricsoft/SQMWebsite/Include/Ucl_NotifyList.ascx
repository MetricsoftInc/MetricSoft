<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_NotifyList.ascx.cs" Inherits="SQM.Website.Ucl_NotifyList" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


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