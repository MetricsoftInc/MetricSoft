<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AuditScheduleList.ascx.cs" Inherits="SQM.Website.Ucl_AuditScheduleList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server">
    <PersistenceSettings>
        <telerik:PersistenceSetting ControlID="rgAuditScheduleList" />
    </PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTAuditSearch" runat="server" Visible="false" Width="99%">
    <asp:HiddenField id="hfCSTPlantSelect" runat="server" value="Responsible Location:"/>
    <asp:HiddenField id="hfRCVPlantSelect" runat="server" value="Detected Location:"/>
    <table cellspacing="0" cellpadding="1" border="0" width="100%">
        <tr>
            <td class="summaryDataEnd" width="150px">
                <asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt" Text="Locations:"></asp:Label>
            </td>
            <td class="summaryDataEnd">
                <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex="9000" Skin="Metro" Height="350" Width="650" OnClientLoad="DisableComboSeparators" EmptyMessage="Select responsible/supplier locations"></telerik:RadComboBox>
            </td>

        </tr>
        <tr>
            <td class="summaryDataEnd" width="150px">
                <asp:Label runat="server" ID="lblDateSpan" CssClass="prompt" Text="Date Span:"></asp:Label>
            </td>
            <td class="summaryDataEnd">
                <telerik:RadComboBox ID="ddlDateSpan" runat="server" Skin="Metro" Width=180 Font-Size=Small AutoPostBack="true" OnSelectedIndexChanged="ddlDateSpanChange">
                    <Items>
                        <telerik:RadComboBoxItem Text="Select Range" Value="0"/> 
                        <telerik:RadComboBoxItem Text="Year To Date" Value="1" /> 
                        <telerik:RadComboBoxItem Text="Previous Year" Value="3" /> 
                        <telerik:RadComboBoxItem Text="FY Year To Date" Value="4" /> 
                    </Items>
                </telerik:RadComboBox>
                <span style="margin-left: 8px;">
                    <asp:Label runat="server" ID="lblPeriodFrom"  CssClass="prompt" Text="From:"></asp:Label>
                    <telerik:RadMonthYearPicker ID="dmPeriodFrom" runat="server" CssClass="textStd" Width=155 Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadMonthYearPicker>
                    <telerik:RadComboBox ID="ddlYearFrom" runat="server" Skin="Metro" Width=100 Font-Size=Small AutoPostBack="false" Visible="false"></telerik:RadComboBox>
                    <asp:Label runat="server" ID="lblPeriodTo" CssClass="prompt" Text="To:" style="margin-left: 5px;"></asp:Label>
                    <telerik:RadMonthYearPicker ID="dmPeriodTo" runat="server" CssClass="textStd" Width=155 Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadMonthYearPicker>
                    <telerik:RadComboBox ID="ddlYearTo" runat="server" Skin="Metro" Width=100 Font-Size=Small AutoPostBack="false" Visible="false"></telerik:RadComboBox> 
                </span>
                <span class="noprint">
                    <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List incidents" OnClick="btnAuditsSearchClick" />
                </span>
            </td>
        </tr>
    </table>
 </asp:Panel>

<asp:Panel ID="pnlAuditTaskHdr" runat="server" Visible="false">
    <table id="tblAuditTaskHdr" runat="server" cellspacing="0" cellpadding="1" border="0" width="99%" class="">
        <tr>
            <td class="columnHeader" width="30%">
                <asp:Label runat="server" ID="lblCasePlant" CssClass="prompt" Text="Business Location"></asp:Label>
            </td>
            <td class="tableDataAlt" width="70%">
                <asp:Label runat="server" ID="lblCasePlant_out"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Label runat="server" ID="lblCaseDescription" Text="Problem Case"></asp:Label>
                <asp:Label runat="server" ID="lblAuditDescription" Text="Audit"></asp:Label>
                <asp:Label runat="server" ID="lblActionDescription" Text="Recommendation"></asp:Label>
            </td>
            <td class="tableDataAlt">
                <span>
                    <asp:Label runat="server" ID="lblCase2ID_out"></asp:Label>
                    &nbsp;-&nbsp;
				    <asp:Label runat="server" ID="lblCase2Desc_out"></asp:Label>
                </span>
            </td>
        </tr>
        <tr>
            <td class="columnHeader">
                <asp:Label runat="server" ID="lblResponsible" CssClass="prompt" Text="Responsible"></asp:Label>
            </td>
            <td class="tableDataAlt">
                <asp:Label runat="server" ID="lblResponsible_out"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlAuditListRepeater" runat="server" Visible="false">
    <div>
        <telerik:RadGrid ID="rgAuditScheduleList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
            AutoGenerateColumns="false" OnItemDataBound="rgAuditScheduleList_ItemDataBound" OnSortCommand="rgAuditScheduleList_SortCommand"
            OnPageIndexChanged="rgAuditScheduleList_PageIndexChanged" OnPageSizeChanged="rgAuditScheduleList_PageSizeChanged" GridLines="None" Width="100%">
            <MasterTableView ExpandCollapseColumn-Visible="false">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Audit" ItemStyle-Width="100px" ShowSortIcon="true" SortExpression="Audit.AUDIT_ID">
                        <ItemTemplate>
                            <table class="innerTable">
                                <tr>
                                    <td>
                                        <asp:LinkButton ID="lbAuditScheduleId" OnClick="lnkEditAuditSchedule" CommandArgument='<%#Eval("AuditScheduler.AUDIT_SCHEDULER_ID") %>' runat="server" ToolTip="Edit audit schedule">
                                            <span style="white-space: nowrap;">
                                                <img src="/images/ico16-edit.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" /><asp:Label ID="lblAuditScheduleId" Font-Bold="true" ForeColor="#000066" Text='<%#string.Format("{0:000000}", Eval("AuditScheduler.AUDIT_SCHEDULER_ID")) %>' runat="server"></asp:Label>
                                            </span>
                                        </asp:LinkButton>
                                    </td>
                                    <%--<td>
                                        <img alt=">" src="/images/arr-rt-grey.png" runat="server" id="imgEditReport" style="opacity: 0.5;" />
                                    </td>--%>
                                   <%-- <td style="width: 50%;">--%>
                                </tr>
<%--                                <tr>
                                    <td>
                                        <asp:LinkButton ID="lb8d" runat="server" OnClick="lnkProblemCaseRedirect" Visible="false" ToolTip="Edit 8D problem case" CommandArgument='<%#Eval("Audit.INCIDENT_ID") %>'>
                                            <span class="tableLink" style="color: #a00000; white-space: nowrap;">Edit 8D</span>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lbEditReport" runat="server" OnClick="lbEditReport_Click" Visible="false" ToolTip="Edit Audit Report" CommandArgument='<%#Eval("Audit.INCIDENT_ID") %>'>
                                            <span class="tableLink" style="color: #006080; white-space: nowrap;">Edit Report</span>
                                        </asp:LinkButton>

                                    </td>
                                </tr>--%>
                            </table>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
                        <ItemTemplate>
                            <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Day of Week" ShowSortIcon="true" SortExpression="AuditScheduler.DAY_OF_WEEK">
                        <ItemTemplate>
                            <asp:Label ID="lblDayOfWeek" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Type" ShowSortIcon="true" SortExpression="AuditType.TITLE">
                        <ItemTemplate>
                            <asp:Label ID="lblType" runat="server" Text='<%# (string)Eval("AuditType.TITLE") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Jobcode" ShowSortIcon="true" SortExpression="AuditScheduler.JOBCODE_CD">
                        <ItemTemplate>
                            <asp:Label ID="lblJobcode" runat="server" Text='<%# (string)Eval("Jobcode.JOB_DESC") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="lblAuditScheduleStatus" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Panel>



