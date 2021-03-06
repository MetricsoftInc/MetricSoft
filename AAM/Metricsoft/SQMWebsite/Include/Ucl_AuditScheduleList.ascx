﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AuditScheduleList.ascx.cs" Inherits="SQM.Website.Ucl_AuditScheduleList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server" UniqueKey="">
    <PersistenceSettings>
        <telerik:PersistenceSetting ControlID="rgAuditScheduleList" />
    </PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTAuditSearch" runat="server" Visible="False" Width="99%">
    <asp:HiddenField id="hfCSTPlantSelect" runat="server" value="Responsible Location:"/>
    <asp:HiddenField id="hfRCVPlantSelect" runat="server" value="Detected Location:"/>
    <table cellspacing="0" cellpadding="1" border="0" width="100%">
        <tr>
            <td class="summaryDataEnd" width="150px">
                <asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt"></asp:Label>
            </td>
            <td class="summaryDataEnd">
                <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="650px" OnClientLoad="DisableComboSeparators" EmptyMessage="<%$ Resources:LocalizedText, SelectResponsibleSupplierLocations %>"></telerik:RadComboBox>
            </td>

        </tr>
        <tr>
            <td class="summaryDataEnd" width="150px">
                <asp:Label runat="server" ID="lblDateSpan" CssClass="prompt" Text="<%$ Resources:LocalizedText, DateSpan %>"></asp:Label>
            </td>
            <td class="summaryDataEnd">
                <telerik:RadComboBox ID="ddlDateSpan" runat="server" Skin="Metro" Width=180px Font-Size=Small AutoPostBack="True" OnSelectedIndexChanged="ddlDateSpanChange">
                    <Items>
                        <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, SelectRange %>" Value="0" runat="server"/>
                        <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, YearToDate %>" Value="1" runat="server" />
                        <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, PreviousYear %>" Value="3" runat="server" />
                        <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, FYYearToDate %>" Value="4" runat="server" />
                    </Items>
                </telerik:RadComboBox>
                <span style="margin-left: 8px;">
                    <asp:Label runat="server" ID="lblPeriodFrom" CssClass="prompt"></asp:Label>
                    <telerik:RadMonthYearPicker ID="dmPeriodFrom" runat="server" CssClass="textStd" Width=155px Skin="Metro">
						<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" Font-Size="Small" LabelWidth="64px" Skin="Metro" Width="">
							<EmptyMessageStyle Resize="None" />
							<ReadOnlyStyle Resize="None" />
							<FocusedStyle Resize="None" />
							<DisabledStyle Resize="None" />
							<InvalidStyle Resize="None" />
							<HoveredStyle Resize="None" />
							<EnabledStyle Resize="None" />
						</DateInput>
						<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
						<MonthYearNavigationSettings DateIsOutOfRangeMessage="<%$ Resources:LocalizedText, Cancel %>" />
				</telerik:RadMonthYearPicker>
                    <telerik:RadComboBox ID="ddlYearFrom" runat="server" Skin="Metro" Width=100px Font-Size=Small Visible="False"></telerik:RadComboBox>
                    <asp:Label runat="server" ID="lblPeriodTo" CssClass="prompt" style="margin-left: 5px;"></asp:Label>
                    <telerik:RadMonthYearPicker ID="dmPeriodTo" runat="server" CssClass="textStd" Width=155px Skin="Metro">
						<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" Font-Size="Small" LabelWidth="64px" Skin="Metro" Width="">
							<EmptyMessageStyle Resize="None" />
							<ReadOnlyStyle Resize="None" />
							<FocusedStyle Resize="None" />
							<DisabledStyle Resize="None" />
							<InvalidStyle Resize="None" />
							<HoveredStyle Resize="None" />
							<EnabledStyle Resize="None" />
						</DateInput>
						<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
						<MonthYearNavigationSettings DateIsOutOfRangeMessage="<%$ Resources:LocalizedText, Cancel %>" />
				</telerik:RadMonthYearPicker>
                    <telerik:RadComboBox ID="ddlYearTo" runat="server" Skin="Metro" Width=100px Font-Size=Small Visible="False"></telerik:RadComboBox>
                </span>
                <span class="noprint">
                    <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListAssessmentSchedules %>" OnClick="btnAuditsSearchClick" />
                </span>
            </td>
        </tr>
    </table>
 </asp:Panel>

<asp:Panel ID="pnlAuditTaskHdr" runat="server" Visible="False">
    <table id="tblAuditTaskHdr" runat="server" cellspacing="0" cellpadding="1" border="0" width="99%" class="">
        <tr runat="server">
            <td class="columnHeader" width="30%" runat="server">
                <asp:Label runat="server" ID="lblCasePlant" CssClass="prompt" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
            </td>
            <td class="tableDataAlt" width="70%" runat="server">
                <asp:Label runat="server" ID="lblCasePlant_out"></asp:Label>
            </td>
        </tr>
        <tr runat="server">
            <td class="columnHeader" runat="server">
                <asp:Label runat="server" ID="lblCaseDescription" Text="Problem Case"></asp:Label>
                <asp:Label runat="server" ID="lblAuditDescription" Text="<%$ Resources:LocalizedText, Assessment %>"></asp:Label>
                <asp:Label runat="server" ID="lblActionDescription" Text="Recommendation"></asp:Label>
            </td>
            <td class="tableDataAlt" runat="server">
                <span>
                    <asp:Label runat="server" ID="lblCase2ID_out"></asp:Label>
                    &nbsp;-&nbsp;
				    <asp:Label runat="server" ID="lblCase2Desc_out"></asp:Label>
                </span>
            </td>
        </tr>
        <tr runat="server">
            <td class="columnHeader" runat="server">
                <asp:Label runat="server" ID="lblResponsible" CssClass="prompt" Text="Responsible"></asp:Label>
            </td>
            <td class="tableDataAlt" runat="server">
                <asp:Label runat="server" ID="lblResponsible_out"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlAuditListRepeater" runat="server" Visible="False">
    <div>
        <telerik:RadGrid ID="rgAuditScheduleList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
            AutoGenerateColumns="False" OnItemDataBound="rgAuditScheduleList_ItemDataBound" OnSortCommand="rgAuditScheduleList_SortCommand"
            OnPageIndexChanged="rgAuditScheduleList_PageIndexChanged" OnPageSizeChanged="rgAuditScheduleList_PageSizeChanged" Width="100%" GroupPanelPosition="Top">
            <MasterTableView>
                <ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="<%$ Resources:LocalizedText, Assessment %>" SortExpression="Audit.AUDIT_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbAuditScheduleId" runat="server" CommandArgument='<%# Eval("AuditScheduler.AUDIT_SCHEDULER_ID") %>' Font-Bold="True" ForeColor="#000066" meta:resourcekey="lbAuditScheduleIdResource1" OnClick="lnkEditAuditSchedule" ToolTip="Edit assessment schedule">
                                        </asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, Location %>" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, DayOfWeek %>" SortExpression="AuditScheduler.DAY_OF_WEEK" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblDayOfWeek" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="<%$ Resources:LocalizedText, Type %>" SortExpression="AuditType.TITLE" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblType" runat="server" Text='<%# (string)Eval("AuditType.TITLE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="<%$ Resources:LocalizedText, PrivilegeGroup %>" SortExpression="AuditScheduler.JOBCODE_CD" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblJobcode" runat="server" Text='<%# (string)Eval("Privgroup.DESCRIPTION") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="<%$ Resources:LocalizedText, Status %>" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblAuditScheduleStatus" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
            </MasterTableView>
            <PagerStyle AlwaysVisible="True"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Panel>



