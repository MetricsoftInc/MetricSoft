<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AuditList.ascx.cs" Inherits="SQM.Website.Ucl_AuditList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server" UniqueKey="">
    <PersistenceSettings>
        <telerik:PersistenceSetting ControlID="rgAuditList" />
    </PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTAuditSearch" runat="server" Visible="False" Width="99%" meta:resourcekey="pnlCSTAuditSearchResource1">
    <asp:HiddenField id="hfCSTPlantSelect" runat="server" value="Responsible Location:"/>
    <asp:HiddenField id="hfRCVPlantSelect" runat="server" value="Detected Location:"/>
    <table cellspacing="0" cellpadding="1" border="0" width="100%">
        <tr>
            <td class="summaryDataEnd" width="150px">
                <asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt" Text="Locations:" meta:resourcekey="lblPlantSelectResource1"></asp:Label>
            </td>
            <td class="summaryDataEnd">
                <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="650px" OnClientLoad="DisableComboSeparators" EmptyMessage="Select responsible/supplier locations" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>
            </td>

        </tr>
        <tr>
            <td class="summaryDataEnd" width="150px">
                <asp:Label runat="server" ID="lblDateSpan" CssClass="prompt" Text="Date Span:" meta:resourcekey="lblDateSpanResource1"></asp:Label>
            </td>
            <td class="summaryDataEnd">
                <telerik:RadComboBox ID="ddlDateSpan" runat="server" Skin="Metro" Width=180px Font-Size=Small AutoPostBack="True" OnSelectedIndexChanged="ddlDateSpanChange" meta:resourcekey="ddlDateSpanResource1">
                    <Items>
                        <telerik:RadComboBoxItem Text="Select Range" Value="0" runat="server" meta:resourcekey="RadComboBoxItemResource1"/> 
                        <telerik:RadComboBoxItem Text="Year To Date" Value="1" runat="server" meta:resourcekey="RadComboBoxItemResource2" /> 
                        <telerik:RadComboBoxItem Text="Previous Year" Value="3" runat="server" meta:resourcekey="RadComboBoxItemResource3" /> 
                        <telerik:RadComboBoxItem Text="FY Year To Date" Value="4" runat="server" meta:resourcekey="RadComboBoxItemResource4" /> 
                    </Items>
                </telerik:RadComboBox>
                <span style="margin-left: 8px;">
                    <asp:Label runat="server" ID="lblPeriodFrom"  CssClass="prompt" Text="From:" meta:resourcekey="lblPeriodFromResource1"></asp:Label>
                    <telerik:RadMonthYearPicker ID="dmPeriodFrom" runat="server" CssClass="textStd" Width=155px Skin="Metro" Culture="en-US" HiddenInputTitleAttibute="Visually hidden input created for functionality purposes." meta:resourcekey="dmPeriodFromResource1">
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
						<MonthYearNavigationSettings DateIsOutOfRangeMessage="Cancel" />
				</telerik:RadMonthYearPicker>
                    <telerik:RadComboBox ID="ddlYearFrom" runat="server" Skin="Metro" Width=100px Font-Size=Small Visible="False" meta:resourcekey="ddlYearFromResource1"></telerik:RadComboBox>
                    <asp:Label runat="server" ID="lblPeriodTo" CssClass="prompt" Text="To:" style="margin-left: 5px;" meta:resourcekey="lblPeriodToResource1"></asp:Label>
                    <telerik:RadMonthYearPicker ID="dmPeriodTo" runat="server" CssClass="textStd" Width=155px Skin="Metro" Culture="en-US" HiddenInputTitleAttibute="Visually hidden input created for functionality purposes." meta:resourcekey="dmPeriodToResource1">
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
						<MonthYearNavigationSettings DateIsOutOfRangeMessage="Cancel" />
				</telerik:RadMonthYearPicker>
                    <telerik:RadComboBox ID="ddlYearTo" runat="server" Skin="Metro" Width=100px Font-Size=Small Visible="False" meta:resourcekey="ddlYearToResource1"></telerik:RadComboBox> 
                </span>
                <span class="noprint">
                    <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List assessments" OnClick="btnAuditsSearchClick" meta:resourcekey="btnSearchResource1" />
                </span>
            </td>
        </tr>
    </table>
 </asp:Panel>

<%--<asp:Panel ID="pnlAuditList" runat="server" Visible="true">
    <table width="99%">
        <tr>
            <td>
                <div id="divGVAuditListScroll" runat="server" class="">
                    <asp:GridView runat="server" ID="gvAuditList" Name="gvAuditList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvAuditList_OnRowDataBound">
                        <HeaderStyle CssClass="HeadingCellText" />
                        <RowStyle CssClass="DataCell" />
                        <Columns>
                            <asp:TemplateField HeaderText="Assessment ID" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfAuditID" runat="server" Value='<%#Eval("AUDIT_ID") %>' />
                                    <asp:Label ID="lblAuditID" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Assessment Date" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfAuditDate" runat="server" Value='<%#Eval("AUDIT_DT") %>' />
                                    <asp:Label ID="lblAuditDate" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Assessment Type" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label ID="lblAuditType" runat="server" Text='<%#Eval("AUDIT_TYPE") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DEPT_NAME" HeaderText="Department" />
                            <asp:BoundField DataField="DESCRIPTION" HeaderText="Description" HtmlEncode="true" ItemStyle-Width="40%" />
                        </Columns>
                    </asp:GridView>
                    <asp:Label runat="server" ID="lblAuditListEmpty" Height="40" Text="No Assessments exist matching your search criteria." class="GridEmpty" Visible="false"></asp:Label>
                </div>
            </td>
        </tr>
    </table>

</asp:Panel>--%>

<asp:Panel ID="pnlAuditTaskHdr" runat="server" Visible="False" meta:resourcekey="pnlAuditTaskHdrResource1">
    <table id="tblAuditTaskHdr" runat="server" cellspacing="0" cellpadding="1" border="0" width="99%" class="">
        <tr runat="server">
            <td class="columnHeader" width="30%" runat="server">
                <asp:Label runat="server" ID="lblCasePlant" CssClass="prompt" Text="Business Location"></asp:Label>
            </td>
            <td class="tableDataAlt" width="70%" runat="server">
                <asp:Label runat="server" ID="lblCasePlant_out"></asp:Label>
            </td>
        </tr>
        <tr runat="server">
            <td class="columnHeader" runat="server">
                <asp:Label runat="server" ID="lblCaseDescription" Text="Problem Case"></asp:Label>
                <asp:Label runat="server" ID="lblAuditDescription" Text="Assessment"></asp:Label>
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

<asp:Panel ID="pnlAuditListRepeater" runat="server" Visible="False" meta:resourcekey="pnlAuditListRepeaterResource1">
    <div>
        <telerik:RadGrid ID="rgAuditList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
            AutoGenerateColumns="False" OnItemDataBound="rgAuditList_ItemDataBound" OnSortCommand="rgAuditList_SortCommand"
            OnPageIndexChanged="rgAuditList_PageIndexChanged" OnPageSizeChanged="rgAuditList_PageSizeChanged" Width="100%" GroupPanelPosition="Top" meta:resourcekey="rgAuditListResource1">
            <MasterTableView>
                <ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Assessment" meta:resourcekey="GridTemplateColumnResource1" SortExpression="Audit.AUDIT_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbAuditId" runat="server" CommandArgument='<%# Eval("Audit.AUDIT_ID") %>' Font-Bold="True" ForeColor="#000066" meta:resourcekey="lbAuditIdResource1" OnClick="lnkEditAudit" ToolTip="Edit assessment">
                                        </asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="Assessment Date" meta:resourcekey="GridTemplateColumnResource2" SortExpression="Audit.AUDIT_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblAuditDT" runat="server" meta:resourcekey="lblAuditDTResource1" Text='<%# ((DateTime)Eval("Audit.AUDIT_DT")).ToShortDateString() %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="Location" meta:resourcekey="GridTemplateColumnResource3" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource1" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="Department" meta:resourcekey="GridTemplateColumnResource4" SortExpression="Department.DEPT_NAME" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblDepartment" runat="server" meta:resourcekey="lblDepartmentResource1" Text='<%# Eval("Department.DEPT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="Type" meta:resourcekey="GridTemplateColumnResource5" SortExpression="AuditType.TITLE" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblType" runat="server" meta:resourcekey="lblTypeResource1" Text='<%# (string)Eval("AuditType.TITLE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="Assessment By" meta:resourcekey="GridTemplateColumnResource6" SortExpression="Audit.AUDIT_PERSON" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblAuditBy" runat="server" meta:resourcekey="lblAuditByResource1"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn6 column" HeaderText="Score" meta:resourcekey="GridTemplateColumnResource7" SortExpression="Audit.TOTAL_SCORE" UniqueName="TemplateColumn6">
						<ItemTemplate>
							<asp:Label ID="lblScore" runat="server" meta:resourcekey="lblScoreResource1" Text='<%# Eval("Audit.TOTAL_SCORE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn7 column" HeaderText="Status" meta:resourcekey="GridTemplateColumnResource8" UniqueName="TemplateColumn7">
						<ItemTemplate>
							<asp:Label ID="lblAuditStatus" runat="server" meta:resourcekey="lblAuditStatusResource1"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
            </MasterTableView>
            <PagerStyle AlwaysVisible="True"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Panel>



