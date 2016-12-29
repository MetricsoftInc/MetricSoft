<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AuditList.ascx.cs" Inherits="SQM.Website.Ucl_AuditList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server" UniqueKey="">
	<PersistenceSettings>
		<telerik:PersistenceSetting ControlID="rgAuditList" />
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
					<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListAssessments %>" OnClick="btnAuditsSearchClick" />
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
							<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, AssessmentDate %>" ItemStyle-Width="20%">
								<ItemTemplate>
									<asp:HiddenField ID="hfAuditDate" runat="server" Value='<%#Eval("AUDIT_DT") %>' />
									<asp:Label ID="lblAuditDate" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, AssessmentType %>" ItemStyle-Width="20%">
								<ItemTemplate>
									<asp:Label ID="lblAuditType" runat="server" Text='<%#Eval("AUDIT_TYPE") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField DataField="DEPT_NAME" HeaderText="<%$ Resources:LocalizedText, Department %>" />
							<asp:BoundField DataField="DESCRIPTION" HeaderText="<%$ Resources:LocalizedText, Description %>" HtmlEncode="true" ItemStyle-Width="40%" />
						</Columns>
					</asp:GridView>
					<asp:Label runat="server" ID="lblAuditListEmpty" Height="40" Text="No Assessments exist matching your search criteria." class="GridEmpty" Visible="false"></asp:Label>
				</div>
			</td>
		</tr>
	</table>

</asp:Panel>--%>

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
		<telerik:RadGrid ID="rgAuditList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="False" OnItemDataBound="rgAuditList_ItemDataBound" OnSortCommand="rgAuditList_SortCommand"
			OnPageIndexChanged="rgAuditList_PageIndexChanged" OnPageSizeChanged="rgAuditList_PageSizeChanged" Width="100%" GroupPanelPosition="Top">
			<MasterTableView>
				<ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="<%$ Resources:LocalizedText, Assessment %>" SortExpression="Audit.AUDIT_ID" UniqueName="TemplateColumn">
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
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, AssessmentDate %>" SortExpression="Audit.AUDIT_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblAuditDT" runat="server" Text='<%# ((DateTime)Eval("Audit.AUDIT_DT")).ToShortDateString() %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, Location %>" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="<%$ Resources:LocalizedText, Department %>" SortExpression="Department.DEPT_NAME" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("Department.DEPT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="<%$ Resources:LocalizedText, Type %>" SortExpression="AuditType.TITLE" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblType" runat="server" Text='<%# (string)Eval("AuditType.TITLE") %>'></asp:Label>
							<asp:Label ID="lblDescription" runat="server" Visible="false"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="<%$ Resources:LocalizedText, AssessmentBy %>" SortExpression="Audit.AUDIT_PERSON" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblAuditBy" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn6 column" HeaderText="<%$ Resources:LocalizedText, Score %>" SortExpression="Audit.TOTAL_SCORE" UniqueName="TemplateColumn6">
						<ItemTemplate>
							<asp:Label ID="lblScore" runat="server" Text='<%# Eval("Audit.TOTAL_SCORE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn7 column" HeaderText="<%$ Resources:LocalizedText, Status %>" UniqueName="TemplateColumn7">
						<ItemTemplate>
							<asp:Label ID="lblAuditStatus" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn8 column" HeaderText="<%$ Resources:LocalizedText, Print %>" UniqueName="TemplateColumn8">
						<ItemTemplate><asp:HyperLink ID="hlReport" runat="server" ForeColor="#000066" meta:resourcekey="hlReportResource1" NavigateUrl='<%# "/EHS/EHS_Audit_PDF.aspx?aid=" + Eval("Audit.AUDIT_ID") %>' Target="_blank" Visible="true"><img src="/images/defaulticon/16x16/open-in-new-window.png" alt="" style="vertical-align: middle;" /> Print </asp:HyperLink></ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn8 column" HeaderText="" UniqueName="TemplateColumn9">
						<ItemTemplate><asp:LinkButton ID="lbReAudit" runat="server" CommandArgument='<%# Eval("Audit.AUDIT_ID") %>' Text="<%$ Resources:LocalizedText, ReAudit %>" Font-Bold="True" ForeColor="#000066" OnClick="lbReAudit_Click" ToolTip="<%$ Resources:LocalizedText, ReAuditTip %>">
										</asp:LinkButton>
									<asp:Label runat="server" ID="lblAuditingId"></asp:Label>
							<asp:HiddenField runat="server" ID="hdnAuditingId" Value='<%# Eval("Audit.AUDITING_ID") %>' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
			</MasterTableView>
			<PagerStyle AlwaysVisible="True"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>



