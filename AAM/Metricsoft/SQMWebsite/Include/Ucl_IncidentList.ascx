<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_IncidentList.ascx.cs" Inherits="SQM.Website.Ucl_IncidentList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server" UniqueKey="">
	<PersistenceSettings>
		<telerik:PersistenceSetting ControlID="rgIncidentList" />
	</PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTIssueSearch" runat="server" Visible="False" Width="99%" meta:resourcekey="pnlCSTIssueSearchResource1">
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
		<tr>
			<td class="summaryDataEnd" width="150px">
				<asp:Label runat="server" ID="lblPartSearch" CssClass="prompt" Text="Part Number:" meta:resourcekey="lblPartSearchResource1"></asp:Label>
			</td>
			<td class="summaryDataEnd">
				<telerik:RadTextBox ID="tbPartSearch" runat="server" Skin="Metro" Width="180px" MaxLength="40" EmptyMessage="containing 'string'" LabelCssClass="" LabelWidth="64px" meta:resourcekey="tbPartSearchResource1" Resize="None">
					<EmptyMessageStyle ForeColor="GrayText" Resize="None" />
					<ReadOnlyStyle Resize="None" />
					<FocusedStyle Resize="None" />
					<DisabledStyle Resize="None" />
					<InvalidStyle Resize="None" />
					<HoveredStyle Resize="None" />
					<EnabledStyle Resize="None" />
				</telerik:RadTextBox>
				<span style="margin-left: 8px;">
					<asp:Label runat="server" ID="lblSeveritySelect" CssClass="prompt" Text="Event Category:" meta:resourcekey="lblSeveritySelectResource1"></asp:Label>
					<telerik:RadComboBox ID="ddlSeveritySelect" runat="server" ZIndex="9000" Skin="Metro" width="140px" meta:resourcekey="ddlSeveritySelectResource1">
					</telerik:RadComboBox>
				 </span>
				<span style="margin-left: 10px;" class="noprint">
					<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Evidence (Images)" CssClass="prompt" meta:resourcekey="lblShowImageResource1"></asp:Label>
					<asp:CheckBox id="cbShowImage" runat="server" meta:resourcekey="cbShowImageResource1"/>
				</span>
			</td>
		</tr>
		<tr>
			<td class="summaryDataEnd" width="150px">
				<asp:Label runat="server" ID="lblDateSpan" CssClass="prompt" Text="<%$ Resources:LocalizedText, DateSpan %>"></asp:Label>
			</td>
			<td class="summaryDataEnd">
				<telerik:RadComboBox ID="ddlDateSpan" runat="server" Skin="Metro" Width=180px Font-Size=Small AutoPostBack="True" OnSelectedIndexChanged="ddlDateSpanChange" meta:resourcekey="ddlDateSpanResource1">
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
					<telerik:RadComboBox ID="ddlYearFrom" runat="server" Skin="Metro" Width=100px Font-Size=Small Visible="False" meta:resourcekey="ddlYearFromResource1"></telerik:RadComboBox>
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
					<telerik:RadComboBox ID="ddlYearTo" runat="server" Skin="Metro" Width=100px Font-Size=Small Visible="False" meta:resourcekey="ddlYearToResource1"></telerik:RadComboBox>
				</span>
				<span class="noprint">
					<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListIncidents %>" OnClick="btnIncidentsSearchClick" />
					<asp:Button ID="btnReceiptSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonLink" Text="List Receipts" ToolTip="List material receipts" OnClick="btnReceiptsSearchClick" meta:resourcekey="btnReceiptSearchResource1" />
				</span>
			</td>
		</tr>
	</table>
 </asp:Panel>

 <asp:Panel ID="pnlCSTIssueList" runat="server" Visible="False" Width="99%" meta:resourcekey="pnlCSTIssueListResource1">
	<telerik:RadGrid ID="rgCSTIssueList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
		AutoGenerateColumns="False" OnItemDataBound="rgCSTIssueList_ItemDataBound" OnSortCommand="rgCSTIssueList_SortCommand"
		OnPageIndexChanged="rgCSTIssueList_PageIndexChanged" OnPageSizeChanged="rgCSTIssueList_PageSizeChanged" Width="100%" GroupPanelPosition="Top" meta:resourcekey="rgCSTIssueListResource1">
		<MasterTableView>
			<ExpandCollapseColumn Visible="False">
			</ExpandCollapseColumn>
			<Columns>
				<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Issue ID" meta:resourcekey="GridTemplateColumnResource1" SortExpression="Incident.INCIDENT_ID" UniqueName="TemplateColumn">
					<ItemTemplate>
						<asp:LinkButton ID="lbIncidentId" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' CssClass="buttonEditRightBold" meta:resourcekey="lbIncidentIdResource1" OnClick="lnkIssue_Click" Text='<%# string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' ToolTip="Edit issue"></asp:LinkButton>
					</ItemTemplate>
					<ItemStyle Width="100px" />
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="Record Date/&lt;br&gt;Reported By" meta:resourcekey="GridTemplateColumnResource2" SortExpression="Incident.INCIDENT_DT" UniqueName="TemplateColumn1">
					<ItemTemplate>
						<asp:Label ID="lblIncidentDT" runat="server" meta:resourcekey="lblIncidentDTResource1" Text='<%# ((DateTime)Eval("Incident.INCIDENT_DT")).ToShortDateString() %>'></asp:Label>
						<br />
						<span style="white-space: nowrap;">
						<asp:Label ID="lblReportedBy" runat="server" meta:resourcekey="lblReportedByResource1"></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="Detected Location/&lt;br&gt;Responsible Location" meta:resourcekey="GridTemplateColumnResource3" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn2">
					<ItemTemplate>
						<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource1" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						<br />
						<span style="white-space: nowrap;">
						<asp:Image ID="imgRespLocation" runat="server" ImageUrl="~/images/icon_supplier2.gif" meta:resourcekey="imgRespLocationResource1" style="vertical-align:middle;" ToolTip="supplier location" />
						<asp:Label ID="lblRespLocation" runat="server" meta:resourcekey="lblRespLocationResource1" Text='<%# Eval("PlantResponsible.PLANT_NAME") %>'></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter ReceiptColumn column" HeaderText="Part Number/&lt;br&gt;Receipt Number" meta:resourcekey="GridTemplateColumnResource4" SortExpression="QIIssue.REF_OPERATION" UniqueName="ReceiptColumn">
					<ItemTemplate>
						<asp:Label ID="lblReceiptPartNum" runat="server" meta:resourcekey="lblReceiptPartNumResource1" Text='<%# Eval("Part.PART_NUM") %>'></asp:Label>
						<br />
						<span style="white-space: nowrap;">
						<asp:Label ID="lblReceiptNum" runat="server" meta:resourcekey="lblReceiptNumResource1" Text='<%# Eval("QIIssue.REF_OPERATION") %>'></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter PartColumn column" HeaderText="Part Number" meta:resourcekey="GridTemplateColumnResource5" SortExpression="Part.PART_NUM" UniqueName="PartColumn">
					<ItemTemplate>
						<asp:Label ID="lblPartNum" runat="server" meta:resourcekey="lblPartNumResource1" Text='<%# Eval("Part.PART_NUM") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="Event Category/&lt;br&gt;Problem Area" meta:resourcekey="GridTemplateColumnResource6" SortExpression="QIIssue.OCCUR_DESC" UniqueName="TemplateColumn3">
					<ItemTemplate>
						<asp:Label ID="lblSeverity" runat="server" meta:resourcekey="lblSeverityResource1"></asp:Label>
						<br />
						<span style="white-space: nowrap;">
						<asp:Label ID="lblType" runat="server" meta:resourcekey="lblTypeResource1" Text='<%# (string)Eval("QIIssue.OCCUR_DESC") %>'></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="<%$ Resources:LocalizedText, Description %>" SortExpression="Incident.DESCRIPTION" UniqueName="TemplateColumn4">
					<ItemTemplate>
						<asp:Label ID="lblDescription" runat="server" meta:resourcekey="lblDescriptionResource1" Text='<%# HttpUtility.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter Attach column" HeaderText="Initial Evidence" meta:resourcekey="GridTemplateColumnResource8" ShowSortIcon="False" UniqueName="Attach">
					<ItemTemplate>
						<asp:Label ID="lblAttach" runat="server" meta:resourcekey="lblAttachResource1"></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
			</Columns>
			<PagerStyle AlwaysVisible="True" />
		</MasterTableView>
		<PagerStyle AlwaysVisible="True"></PagerStyle>
	</telerik:RadGrid>
</asp:Panel>


<asp:Panel ID="pnlQualityIssueList" runat="server" Visible="False" meta:resourcekey="pnlQualityIssueListResource1">
	<table width="100%">
		<tr>
			<td>
				<div id="divGVIssueListScroll" runat="server" class="">
					<asp:GridView runat="server" ID="gvIssueList" Name="gvIssueList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="False" CellPadding="1" PageSize="20" AllowSorting="True" Width="100%" OnRowDataBound="gvIssueList_OnRowDataBound" meta:resourcekey="gvIssueListResource1">
						<Columns>
							<asp:TemplateField HeaderText="Issue ID" meta:resourcekey="IssueIDTemplateFieldResource1">
								<ItemTemplate>
									<asp:HiddenField ID="hfIssueID" runat="server" Value='<%# Eval("Incident.INCIDENT_ID") %>' />
									<asp:LinkButton ID="lnkViewIssue_out" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' CssClass="linkUnderline" meta:resourcekey="lnkViewIssue_outResource1" OnClick="lnkIssue_Click" Text='<%# Eval("Incident.INCIDENT_ID") %>'></asp:LinkButton>
									<asp:HiddenField ID="hfIssueStatus" runat="server" Value='<%# Eval("QIIssue.STATUS") %>' />
								</ItemTemplate>
								<ItemStyle Width="7%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Date" meta:resourcekey="TemplateFieldResource2">
								<ItemTemplate>
									<asp:LinkButton ID="lnkIssueDate_out" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' CssClass="linkUnderline" meta:resourcekey="lnkIssueDate_outResource1" OnClick="lnkIssue_Click" Text='<%# Eval("Incident.INCIDENT_DT") %>'></asp:LinkButton>
								</ItemTemplate>
								<ItemStyle Width="7%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Problem Area" meta:resourcekey="TemplateFieldResource3">
								<ItemTemplate>
									<asp:Label ID="lblProblemArea" runat="server" meta:resourcekey="lblProblemAreaResource1" Text='<%# Eval("QIIssue.OCCUR_DESC") %>'></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="10%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Description %>">
								<ItemTemplate>
									<asp:Label ID="lblIssueDesc" runat="server" meta:resourcekey="lblIssueDescResource1" Text='<%# Server.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="20%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Part Number" meta:resourcekey="TemplateFieldResource5">
								<ItemTemplate>
									<asp:Label ID="lblPartNum_out" runat="server" meta:resourcekey="lblPartNum_outResource1" Text='<%# Eval("Part.PART_NUM") %>'></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="15%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Disposition" meta:resourcekey="TemplateFieldResource6">
								<ItemTemplate>
									<asp:Label ID="lblDisposition_out" runat="server" meta:resourcekey="lblDisposition_outResource1" Text='<%# Eval("QIIssue.DISPOSITION") %>'></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="10%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Select %>">
								<ItemTemplate>
									<asp:CheckBox ID="cbSelect" runat="server" meta:resourcekey="cbSelectResource1" Style="margin-left: 33%;" />
								</ItemTemplate>
								<ItemStyle Width="5%" />
							</asp:TemplateField>
						</Columns>
						<HeaderStyle CssClass="HeadingCellText" />
						<RowStyle CssClass="DataCell" />
					</asp:GridView>
					<asp:Label runat="server" ID="lblIssueListEmpty" Height="40px" Text="No Quality Issues matching your search criteria." class="GridEmpty" Visible="False" meta:resourcekey="lblIssueListEmptyResource1"></asp:Label>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>


<asp:Panel ID="pnlIncidentList" runat="server" meta:resourcekey="pnlIncidentListResource1">
	<table width="99%">
		<tr>
			<td>
				<div id="divGVIncidentListScroll" runat="server" class="">
					<asp:GridView runat="server" ID="gvIncidentList" Name="gvIncidentList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="False" CellPadding="1" PageSize="20" AllowSorting="True" Width="100%" OnRowDataBound="gvIncidentList_OnRowDataBound" meta:resourcekey="gvIncidentListResource1">
						<Columns>
							<asp:TemplateField HeaderText="Issue ID" meta:resourcekey="IssueIDTemplateFieldResource1">
								<ItemTemplate>
									<asp:HiddenField ID="hfIncidentID" runat="server" Value='<%# Eval("INCIDENT_ID") %>' />
									<asp:Label ID="lblIncidentID" runat="server" meta:resourcekey="lblIncidentIDResource1"></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="20%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, IncidentDate %>">
								<ItemTemplate>
									<asp:HiddenField ID="hfIncidentDate" runat="server" Value='<%# Eval("INCIDENT_DT") %>' />
									<asp:Label ID="lblIncidentDate" runat="server" meta:resourcekey="lblIncidentDateResource1"></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="20%" />
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Problem Area" meta:resourcekey="TemplateFieldResource10">
								<ItemTemplate>
									<asp:Label ID="lblIssueType" runat="server" meta:resourcekey="lblIssueTypeResource1" Text='<%# Eval("ISSUE_TYPE") %>'></asp:Label>
								</ItemTemplate>
								<ItemStyle Width="20%" />
							</asp:TemplateField>
							<asp:BoundField DataField="DESCRIPTION" HeaderText="<%$ Resources:LocalizedText, Description %>">
							<ItemStyle Width="40%" />
							</asp:BoundField>
						</Columns>
						<HeaderStyle CssClass="HeadingCellText" />
						<RowStyle CssClass="DataCell" />
					</asp:GridView>
					<asp:Label runat="server" ID="lblIncidentListEmpty" Height="40px" Text="No Incidents exist matching your search criteria." class="GridEmpty" Visible="False" meta:resourcekey="lblIncidentListEmptyResource1"></asp:Label>
				</div>
			</td>
		</tr>
	</table>

</asp:Panel>

<asp:Panel ID="pnlProbCaseListRepeater" runat="server" Visible="False" meta:resourcekey="pnlProbCaseListRepeaterResource1">
	<div style="padding: 5px;">
		<telerik:RadGrid ID="rgCaseList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="False" OnItemDataBound="rgCaseList_ItemDataBound" OnSortCommand="rgCaseList_SortCommand"
			OnPageIndexChanged="rgCaseList_PageIndexChanged" OnPageSizeChanged="rgCaseList_PageSizeChanged" Width="100%" GroupPanelPosition="Top" meta:resourcekey="rgCaseListResource1">
			<MasterTableView>
				<ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Problem Case ID" meta:resourcekey="GridTemplateColumnResource9" SortExpression="ProbCase.PROBCASE_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<asp:LinkButton ID="lbCaseId" runat="server" CommandArgument='<%# Eval("ProbCase.PROBCASE_ID") %>' meta:resourcekey="lbCaseIdResource1" OnClick="lbIncidentId_Click"><asp:Label runat="server" Text='<%# string.Format("{0:000000}", Eval("ProbCase.PROBCASE_ID")) %>' Font-Bold="True" ForeColor="#000066" ID="lblCaseId" meta:resourcekey="lblCaseIdResource1"></asp:Label>
</asp:LinkButton>
							<asp:HiddenField ID="hfStatus" runat="server" Value='<%# Eval("ProbCase.STATUS") %>' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, Location %>" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource2" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter colIncident column" HeaderText="Incident ID" meta:resourcekey="GridTemplateColumnResource11" SortExpression="ProbCase.PROBCASE_ID" UniqueName="colIncident">
						<ItemTemplate>
							<asp:Label ID="lblIncidentID" runat="server" meta:resourcekey="lblIncidentIDResource2"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, Description %>" SortExpression="ProbCase.DESC_SHORT" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" meta:resourcekey="lblDescriptionResource2" Text='<%# Server.HtmlEncode((string)Eval("ProbCase.DESC_SHORT")) %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="Created" meta:resourcekey="GridTemplateColumnResource13" SortExpression="ProbCase.CREATE_DT" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblCreated" runat="server" meta:resourcekey="lblCreatedResource1" Text='<%# ((DateTime)Eval("ProbCase.CREATE_DT")).ToShortDateString() %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="Updated" meta:resourcekey="GridTemplateColumnResource14" SortExpression="ProbCase.LAST_UPD_DT" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblUpdated" runat="server" meta:resourcekey="lblUpdatedResource1" Text='<%# ((DateTime)Eval("ProbCase.LAST_UPD_DT")).ToShortDateString() %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="<%$ Resources:LocalizedText, Status %>" SortExpression="ProbCase.CLOSE_DT" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblStatus" runat="server" meta:resourcekey="lblStatusResource1"></asp:Label>
							<asp:Image ID="imgStatus" runat="server" meta:resourcekey="imgCaseStatusResource1" Style="vertical-align: middle;" ToolTip="Case is inactive" Visible="False" />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter Reports column" HeaderText="Reports" meta:resourcekey="GridTemplateColumnResource16" UniqueName="Reports">
						<ItemTemplate>
							<asp:LinkButton ID="lbReport" runat="server" CommandArgument='<%# Eval("ProbCase.PROBCASE_ID") %>' ForeColor="#000066" meta:resourcekey="lbReportResource1" OnClick="lbReport_Click" Visible="False">
							<img src="/images/defaulticon/16x16/files.png" alt="" style="vertical-align: middle; margin-left: 4px;" /> 8D Report
							</asp:LinkButton>
							&nbsp;
							<asp:HyperLink ID="hlReport" runat="server" ForeColor="#000066" meta:resourcekey="hlReportResource1" NavigateUrl='<%# "/EHS/EHS_Alert_PDF.aspx?pcid=" + Eval("ProbCase.PROBCASE_ID") %>' Target="_blank" Visible="False">
							<img src="/images/defaulticon/16x16/open-in-new-window.png" alt="" style="vertical-align: middle;" /> Incident Alert
						</asp:HyperLink>
							<asp:HiddenField ID="hfProblemCaseType" runat="server" Value='<%# Eval("ProbCase.PROBCASE_TYPE") %>' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
			</MasterTableView>
			<PagerStyle AlwaysVisible="True"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>


<asp:Panel ID="pnlQualityIncidentHdr" runat="server" Visible="False" meta:resourcekey="pnlQualityIncidentHdrResource1">
	<table cellspacing="0" cellpadding="1" border="0" width="99%" class="">
		<tr>
			<td class="columnHeader" width="30%">
				<asp:Label runat="server" ID="lblDetectedLocation" CssClass="prompt" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
			</td>
			<td class="tableDataAlt" width="70%">
				<asp:Label runat="server" ID="lblDetectedLocation_out" meta:resourcekey="lblDetectedLocation_outResource1"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="columnHeader">
				<asp:Label runat="server" ID="lblIssueDescription" Text="Issue" meta:resourcekey="lblIssueDescriptionResource1"></asp:Label>
			</td>
			<td class="tableDataAlt">
				<span>
					<asp:Label runat="server" ID="lblIssueID_out" meta:resourcekey="lblIssueID_outResource1"></asp:Label>
					&nbsp;-&nbsp;
			<asp:Label runat="server" ID="lblIssueDesc_out" meta:resourcekey="lblIssueDesc_outResource1"></asp:Label>
				</span>
			</td>
		</tr>
		<tr>
			<td class="columnHeader">
				<asp:Label runat="server" ID="lblIssuePartNum" CssClass="prompt" Text="Part Number" meta:resourcekey="lblIssuePartNumResource1"></asp:Label>
			</td>
			<td class="tableDataAlt">
				<asp:Label runat="server" ID="lblIssuePartNum_out" meta:resourcekey="lblIssuePartNum_outResource1"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="columnHeader">
				<asp:Label runat="server" ID="lblIssueResponsible" CssClass="prompt" Text="Responsible" meta:resourcekey="lblIssueResponsibleResource1"></asp:Label>
			</td>
			<td class="tableDataAlt">
				<asp:Label runat="server" ID="lblIssueResponsible_out" meta:resourcekey="lblIssueResponsible_outResource1"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlProbCaseHdr" runat="server" Visible="False" meta:resourcekey="pnlProbCaseHdrResource1">
	<asp:HiddenField id="hfLblCaseIDEHS" runat="server" value="Incident ID"/>
	<table cellspacing="0" cellpadding="1" border="0" width="99%">
		<tr>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseID" Text="Problem Case ID" meta:resourcekey="lblCaseIDResource2"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseID_out" meta:resourcekey="lblCaseID_outResource1"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseType" Text="Case Type" meta:resourcekey="lblCaseTypeResource1"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseType_out" meta:resourcekey="lblCaseType_outResource1"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top" colspan="2">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseStatus" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseStatus_out" meta:resourcekey="lblCaseStatus_outResource1"></asp:Label>
				<asp:Image ID="imgCaseStatus" runat="server" Visible="False" ToolTip="Case is inactive" Style="vertical-align: middle; margin-left: 4px;" meta:resourcekey="imgCaseStatusResource1" />
			</td>
		</tr>
		<tr>
			<td class="summaryDataTop" valign="top" colspan="2">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseDesc" Text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseDesc_out" meta:resourcekey="lblCaseDesc_outResource1"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCreateDate" Text="Created" meta:resourcekey="lblCreateDateResource1"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCreateDate_out" meta:resourcekey="lblCreateDate_outResource1"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblUpdateDate" Text="<%$ Resources:LocalizedText, LastUpdate %>"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblUpdateDate_out" meta:resourcekey="lblUpdateDate_outResource1"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlIncidentTaskHdr" runat="server" Visible="False" meta:resourcekey="pnlIncidentTaskHdrResource1">
	<table id="tblIncidentTaskHdr" runat="server" cellspacing="0" cellpadding="1" border="0" width="99%" class="">
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
				<asp:Label runat="server" ID="lblIncidentDescription" Text="<%$ Resources:LocalizedText, Incident %>"></asp:Label>
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

<asp:Panel ID="pnlIncidentListRepeater" runat="server" Visible="False" meta:resourcekey="pnlIncidentListRepeaterResource1">
	<div>
		<telerik:RadGrid ID="rgIncidentList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="False" OnItemDataBound="rgIncidentList_ItemDataBound" OnSortCommand="rgIncidentList_SortCommand"
			OnPageIndexChanged="rgIncidentList_PageIndexChanged" OnPageSizeChanged="rgIncidentList_PageSizeChanged" Width="100%" GroupPanelPosition="Top" meta:resourcekey="rgIncidentListResource1">
			<MasterTableView>
				<ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Incident ID" meta:resourcekey="GridTemplateColumnResource17" SortExpression="Incident.INCIDENT_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbIncidentId" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' Font-Bold="True" ForeColor="#000066" meta:resourcekey="lbIncidentIdResource2" OnClick="lnkEditIncident" ToolTip="Edit incident">
											<span style="white-space: nowrap;">
					<%--							<img src="/images/ico16-edit.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" />--%>
									<%--			<asp:Label runat="server" Text='<%# string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' Font-Bold="True" ForeColor="#000066" ID="lblIncidentId" meta:resourcekey="lblIncidentIdResource3"></asp:Label>--%>
											</span>
										</asp:LinkButton>
									</td>
								</tr>
								<tr>
									<td>
										<asp:LinkButton ID="lb8d" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' meta:resourcekey="lb8dResource1" OnClick="lnkProblemCaseRedirect" ToolTip="Edit 8D problem case" Visible="False">
											<span class="tableLink" style="color: #a00000; white-space: nowrap;">Edit 8D</span>
										</asp:LinkButton>
										<asp:LinkButton ID="lbEditReport" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' meta:resourcekey="lbEditReportResource1" OnClick="lbEditReport_Click" ToolTip="Edit Incident Report" Visible="False">
											<span class="tableLink" style="color: #006080; white-space: nowrap;">Edit Report</span>
										</asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" SortExpression="Incident.INCIDENT_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblIncidentDT" runat="server" meta:resourcekey="lblIncidentDTResource2" Text='<%# ((DateTime)Eval("Incident.INCIDENT_DT")).ToShortDateString() %>'></asp:Label>
							<br />
							<span style="white-space: nowrap;">
							<asp:Label ID="lblReportedBy" runat="server" meta:resourcekey="lblReportedByResource2"></asp:Label>
							</span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, Location %>" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource3" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="<%$ Resources:LocalizedText, Type %>" SortExpression="Incident.ISSUE_TYPE" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblType" runat="server" meta:resourcekey="lblTypeResource2" Text='<%# (string)Eval("Incident.ISSUE_TYPE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="<%$ Resources:LocalizedText, Description %>" SortExpression="Incident.DESCRIPTION" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" meta:resourcekey="lblDescriptionResource3" Text='<%# HttpUtility.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter Attach column" HeaderText="Initial Evidence" meta:resourcekey="GridTemplateColumnResource22" ShowSortIcon="False" UniqueName="Attach">
						<ItemTemplate>
							<asp:Label ID="lblAttach" runat="server" meta:resourcekey="lblAttachResource2"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblIncStatus" runat="server" meta:resourcekey="lblIncStatusResource1"></asp:Label>
							   <asp:HyperLink ID="hlReport" runat="server" ForeColor="#000088" Target="_blank" ToolTip="view Incident summary report">
									<img src="/images/defaulticon/16x16/document.png" alt="Report" style="margin-top: -3px; vertical-align: middle;" /></asp:HyperLink>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
			</MasterTableView>
			<PagerStyle AlwaysVisible="True"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>

<asp:Panel ID="pnlPreventativeListRepeater" runat="server" Visible="False" meta:resourcekey="pnlPreventativeListRepeaterResource1">
	<div>
		<telerik:RadGrid ID="rgPreventativeList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="False" OnItemDataBound="rgPreventativeList_ItemDataBound" OnSortCommand="rgPreventativeList_SortCommand"
			OnPageIndexChanged="rgIncidentList_PageIndexChanged" OnPageSizeChanged="rgIncidentList_PageSizeChanged" Width="100%" GroupPanelPosition="Top" meta:resourcekey="rgPreventativeListResource1">
			<MasterTableView>
				<ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Recommendation/&lt;br&gt;Response" meta:resourcekey="GridTemplateColumnResource25" SortExpression="Incident.INCIDENT_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbIncidentId" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' Font-Bold="True" ForeColor="#000066" meta:resourcekey="lbIncidentIdResource3" OnClick="lnkEditIncident" ToolTip="Edit Recommendation">
										</asp:LinkButton>
									</td>
								</tr>
								<tr>
									<td>
										<asp:LinkButton ID="lb8d" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' meta:resourcekey="lb8dResource2" OnClick="lnkProblemCaseRedirect" ToolTip="Edit 8D problem case" Visible="False">
											<span class="tableLink" style="color: #a00000; white-space: nowrap;">Edit 8D</span>
										</asp:LinkButton>
										<asp:LinkButton ID="lbEditReport" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' meta:resourcekey="lbEditReportResource2" OnClick="lbEditReport_Click" ToolTip="Edit Response" Visible="False">
											<span class="tableLink" style="color: #006080; white-space: nowrap;">Update Response</span>
										</asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="Inspection Date/&lt;br&gt;Entered By" meta:resourcekey="GridTemplateColumnResource26" SortExpression="Incident.INCIDENT_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblIncidentDT" runat="server" meta:resourcekey="lblIncidentDTResource3"></asp:Label>
							<br />
							<span style="white-space: nowrap;">
							<asp:Label ID="lblReportedBy" runat="server" meta:resourcekey="lblReportedByResource3"></asp:Label>
							</span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, Location %>" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource4" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="Category/&lt;br&gt;Type" meta:resourcekey="GridTemplateColumnResource28" SortExpression="EntryList[0].ANSWER_VALUE,EntryList[2].ANSWER_VALUE" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblCategory" runat="server" meta:resourcekey="lblCategoryResource1"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="<%$ Resources:LocalizedText, Description %>" SortExpression="Incident.DESCRIPTION" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" meta:resourcekey="lblDescriptionResource4"></asp:Label>
						</ItemTemplate>
						<ItemStyle CssClass="tableWithLink" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter Attach column" HeaderText="Initial Evidence" meta:resourcekey="GridTemplateColumnResource30" ShowSortIcon="False" UniqueName="Attach">
						<ItemTemplate>
							<asp:Label ID="lblAttach" runat="server" meta:resourcekey="lblAttachResource3"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" SortExpression="Incident.INCIDENT_DT" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblDueDT" runat="server" meta:resourcekey="lblDueDTResource1"></asp:Label>
							<br />
							<span style="white-space: nowrap;">
							<asp:Label ID="lblAssignedTo" runat="server" meta:resourcekey="lblAssignedToResource1"></asp:Label>
							</span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn6 column" SortExpression="DaysOpen" UniqueName="TemplateColumn6">
						<ItemTemplate>
							<asp:Label ID="lblIncStatus" runat="server" meta:resourcekey="lblIncStatusResource2"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
			</MasterTableView>
			<PagerStyle AlwaysVisible="True"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>

<asp:Panel ID="pnlIncidentActionList" runat="server" Visible="False" meta:resourcekey="pnlIncidentActionListResource1">
	<div>
		<telerik:RadGrid ID="rgIncidentActionList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True"
			AutoGenerateColumns="False" OnItemDataBound="rgIncidentActionList_ItemDataBound" OnSortCommand="rgIncidentActionList_SortCommand"
			OnPageIndexChanged="rgIncidentActionList_PageIndexChanged" OnPageSizeChanged="rgIncidentActionList_PageSizeChanged" Width="100%" GroupPanelPosition="Top" meta:resourcekey="rgIncidentActionListResource1">
			<MasterTableView>
				<ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="<%$ Resources:LocalizedText, Incident %>" SortExpression="Incident.INCIDENT_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbIncidentId" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' meta:resourcekey="lbIncidentIdResource4" OnClick="lnkEditIncident" ToolTip="Edit incident">
											<span style="white-space: nowrap;">
												<img src="/images/defaulticon/16x16/edit-document.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" /><asp:Label runat="server" Text='<%# string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' Font-Bold="True" ForeColor="#000066" ID="lblIncidentId" meta:resourcekey="lblIncidentIdResource5"></asp:Label>

											</span>
										</asp:LinkButton>
									</td>
								</tr>
								<tr>
									<td>
										<asp:LinkButton ID="lb8d" runat="server" CommandArgument='<%# Eval("Incident.INCIDENT_ID") %>' meta:resourcekey="lb8dResource3" OnClick="lnkProblemCaseRedirect" ToolTip="Edit 8D problem case">
										<span class="tableLink" style="color: #a00000">Edit 8D</span>
										</asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, Location %>" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource5" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, IncidentType %>" SortExpression="Incident.ISSUE_TYPE" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblIncidentType" runat="server" meta:resourcekey="lblIncidentTypeResource1" Text='<%# Eval("Incident.ISSUE_TYPE") %>'></asp:Label>
							<asp:HiddenField ID="hfIncidentType" runat="server" Value='<%# Eval("Incident.ISSUE_TYPE_ID") %>' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="<%$ Resources:LocalizedText, Description %>" SortExpression="Incident.DESCRIPTION" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" meta:resourcekey="lblDescriptionResource5" Text='<%# Server.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="Date Due/&lt;br&gt;Responsible" meta:resourcekey="GridTemplateColumnResource37" SortExpression="Incident.INCIDENT_ID" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblDueDT" runat="server" meta:resourcekey="lblDueDTResource2"></asp:Label>
							<br />
							<asp:Label ID="lblResponsible" runat="server" meta:resourcekey="lblResponsibleResource1"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="Root Cause/&lt;br&gt;Corrective Actions" meta:resourcekey="GridTemplateColumnResource38" SortExpression="Incident.INCIDENT_ID" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<telerik:RadGrid ID="rgIncidentActions" runat="server" AutoGenerateColumns="False" GroupPanelPosition="Top" meta:resourcekey="rgIncidentActionsResource1" OnItemDataBound="rgIncidentActions_ItemDataBound" Skin="Metro" Visible="False" Width="100%">
								<ClientSettings EnableAlternatingItems="False">
								</ClientSettings>
								<MasterTableView ShowHeader="False">
									<ExpandCollapseColumn Visible="False">
									</ExpandCollapseColumn>
									<Columns>
										<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" meta:resourcekey="GridTemplateColumnResource39" ShowSortIcon="False" UniqueName="TemplateColumn">
											<ItemTemplate>
												<asp:Label ID="lblTopic" runat="server" CssClass="refTextSmallBold" meta:resourcekey="lblTopicResource1" Text='<%# Eval("ORIGINAL_QUESTION_TEXT") %>'></asp:Label>
												<br />
												<asp:Label ID="lblEntry" runat="server" CssClass="textStd" meta:resourcekey="lblEntryResource1" Text='<%# Eval("ANSWER_VALUE") %>'></asp:Label>
											</ItemTemplate>
											<ItemStyle BorderStyle="None" Width="100%" />
										</telerik:GridTemplateColumn>
									</Columns>
								</MasterTableView>
							</telerik:RadGrid>
						</ItemTemplate>
						<ItemStyle Width="35%" />
					</telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
			</MasterTableView>
			<PagerStyle AlwaysVisible="True"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>


