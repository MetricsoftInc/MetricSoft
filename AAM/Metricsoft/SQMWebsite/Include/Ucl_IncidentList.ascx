<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_IncidentList.ascx.cs" Inherits="SQM.Website.Ucl_IncidentList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server">
	<PersistenceSettings>
		<telerik:PersistenceSetting ControlID="rgIncidentList" />
	</PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTIssueSearch" runat="server" Visible="false" Width="99%">
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
		<tr>
			<td class="summaryDataEnd" width="150px">
				<asp:Label runat="server" ID="lblPartSearch" CssClass="prompt" Text="Part Number:"></asp:Label>
			</td>
			<td class="summaryDataEnd">
				<telerik:RadTextBox ID="tbPartSearch" runat="server" Skin="Metro" Width="180" MaxLength="40" EmptyMessage="containing 'string'" EmptyMessageStyle-ForeColor="GrayText"></telerik:RadTextBox>
				<span style="margin-left: 8px;">
					<asp:Label runat="server" ID="lblSeveritySelect" CssClass="prompt" Text="Event Category:"></asp:Label>
					<telerik:RadComboBox ID="ddlSeveritySelect" runat="server" ToolTip="" ZIndex="9000" Skin="Metro" width="140" AutoPostBack="false">
					</telerik:RadComboBox>
				 </span>
				<span style="margin-left: 10px;" class="noprint">
					<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Evidence (Images)" CssClass="prompt"></asp:Label>
					<asp:CheckBox id="cbShowImage" runat="server" Checked="false"/>
				</span>
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
					<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List incidents" OnClick="btnIncidentsSearchClick" />
					<asp:Button ID="btnReceiptSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonLink" Text="List Receipts" ToolTip="List material receipts" OnClick="btnReceiptsSearchClick" />
				</span>
			</td>
		</tr>
	</table>
 </asp:Panel>

 <asp:Panel ID="pnlCSTIssueList" runat="server" Visible="false" Width="99%">
	<telerik:RadGrid ID="rgCSTIssueList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
		AutoGenerateColumns="false" OnItemDataBound="rgCSTIssueList_ItemDataBound" OnSortCommand="rgCSTIssueList_SortCommand"
		OnPageIndexChanged="rgCSTIssueList_PageIndexChanged" OnPageSizeChanged="rgCSTIssueList_PageSizeChanged" GridLines="None" Width="100%">
		<MasterTableView ExpandCollapseColumn-Visible="false">
			<Columns>
				<telerik:GridTemplateColumn HeaderText="Issue ID" ItemStyle-Width="100px" ShowSortIcon="true" SortExpression="Incident.INCIDENT_ID">
					<ItemTemplate>
						 <asp:LinkButton ID="lbIncidentId" OnClick="lnkIssue_Click" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>' Text='<%#string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' CssClass="buttonEditRightBold" runat="server" ToolTip="Edit issue">
						 </asp:LinkButton>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Record Date/<br>Reported By" ShowSortIcon="true" SortExpression="Incident.INCIDENT_DT">
					<ItemTemplate>
						<asp:Label ID="lblIncidentDT" Text='<%# ((DateTime)Eval("Incident.INCIDENT_DT")).ToShortDateString() %>' runat="server"></asp:Label>
						<br />
						<span style="white-space: nowrap;">
							<asp:Label ID="lblReportedBy" runat="server"></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Detected Location/<br>Responsible Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
					<ItemTemplate>
						<asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
						<br />
						<span style="white-space: nowrap;">
							<asp:Image ID="imgRespLocation" Visible="true" runat="server" style="vertical-align:middle;" ImageUrl = "~/images/icon_supplier2.gif" ToolTip="supplier location"/>
							<asp:Label ID="lblRespLocation" runat="server" Text='<%#Eval("PlantResponsible.PLANT_NAME") %>'></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn UniqueName="ReceiptColumn" HeaderText="Part Number/<br>Receipt Number" ShowSortIcon="true" SortExpression="QIIssue.REF_OPERATION">
					<ItemTemplate>
						<asp:Label ID="lblReceiptPartNum" runat="server" Text='<%#Eval("Part.PART_NUM") %>'></asp:Label>
						<br />
						<span style="white-space: nowrap;">
							<asp:Label ID="lblReceiptNum" runat="server" Text='<%#Eval("QIIssue.REF_OPERATION") %>'></asp:Label>
						</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn UniqueName="PartColumn" HeaderText="Part Number" ShowSortIcon="true" SortExpression="Part.PART_NUM">
					<ItemTemplate>
						<asp:Label ID="lblPartNum" runat="server" Text='<%#Eval("Part.PART_NUM") %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Event Category/<br>Problem Area" ShowSortIcon="true" SortExpression="QIIssue.OCCUR_DESC">
					<ItemTemplate>
						<asp:Label ID="lblSeverity" runat="server"></asp:Label>
							<br />
						<span style="white-space: nowrap;">
							<asp:Label ID="lblType" runat="server" Text='<%# (string)Eval("QIIssue.OCCUR_DESC") %>'></asp:Label>
							</span>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="true" SortExpression="Incident.DESCRIPTION">
					<ItemTemplate>
						<asp:Label ID="lblDescription" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn UniqueName="Attach" HeaderText="Initial Evidence" ShowSortIcon="false" >
					<ItemTemplate>
						<asp:Label ID="lblAttach" runat="server"></asp:Label>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
			</Columns>
		</MasterTableView>
		<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
	</telerik:RadGrid>
</asp:Panel>


<asp:Panel ID="pnlQualityIssueList" runat="server" Visible="false">
	<table width="100%">
		<tr>
			<td>
				<div id="divGVIssueListScroll" runat="server" class="">
					<asp:GridView runat="server" ID="gvIssueList" Name="gvIssueList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvIssueList_OnRowDataBound">
						<HeaderStyle CssClass="HeadingCellText" />
						<RowStyle CssClass="DataCell" />
						<Columns>
							<asp:TemplateField HeaderText="Issue ID" ItemStyle-Width="7%">
								<ItemTemplate>
									<asp:HiddenField ID="hfIssueID" runat="server" Value='<%#Eval("Incident.INCIDENT_ID") %>'></asp:HiddenField>
									<asp:LinkButton ID="lnkViewIssue_out" runat="server" Text='<%#Eval("Incident.INCIDENT_ID") %>' CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'
										OnClick="lnkIssue_Click" CssClass="linkUnderline"></asp:LinkButton>
									<asp:HiddenField ID="hfIssueStatus" runat="server" Value='<%#Eval("QIIssue.STATUS") %>'></asp:HiddenField>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Date" ItemStyle-Width="7%">
								<ItemTemplate>
									<asp:LinkButton ID="lnkIssueDate_out" runat="server" Text='<%#Eval("Incident.INCIDENT_DT") %>' CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'
										OnClick="lnkIssue_Click" CssClass="linkUnderline"></asp:LinkButton>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Problem Area" ItemStyle-Width="10%">
								<ItemTemplate>
									<asp:Label ID="lblProblemArea" runat="server" Text='<%#Eval("QIIssue.OCCUR_DESC") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Description" ItemStyle-Width="20%">
								<ItemTemplate>
									<asp:Label ID="lblIssueDesc" runat="server" Text='<%#Server.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Part Number" ItemStyle-Width="15%">
								<ItemTemplate>
									<asp:Label ID="lblPartNum_out" runat="server" Text='<%#Eval("Part.PART_NUM") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Disposition" ItemStyle-Width="10%">
								<ItemTemplate>
									<asp:Label ID="lblDisposition_out" runat="server" Text='<%#Eval("QIIssue.DISPOSITION") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Select" ItemStyle-Width="5%">
								<ItemTemplate>
									<asp:CheckBox ID="cbSelect" runat="server" Style="margin-left: 33%;"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateField>
						</Columns>
					</asp:GridView>
					<asp:Label runat="server" ID="lblIssueListEmpty" Height="40" Text="No Quality Issues matching your search criteria." class="GridEmpty" Visible="false"></asp:Label>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>


<asp:Panel ID="pnlIncidentList" runat="server" Visible="true">
	<table width="99%">
		<tr>
			<td>
				<div id="divGVIncidentListScroll" runat="server" class="">
					<asp:GridView runat="server" ID="gvIncidentList" Name="gvIncidentList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvIncidentList_OnRowDataBound">
						<HeaderStyle CssClass="HeadingCellText" />
						<RowStyle CssClass="DataCell" />
						<Columns>
							<asp:TemplateField HeaderText="Issue ID" ItemStyle-Width="20%">
								<ItemTemplate>
									<asp:HiddenField ID="hfIncidentID" runat="server" Value='<%#Eval("INCIDENT_ID") %>' />
									<asp:Label ID="lblIncidentID" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Incident Date" ItemStyle-Width="20%">
								<ItemTemplate>
									<asp:HiddenField ID="hfIncidentDate" runat="server" Value='<%#Eval("INCIDENT_DT") %>' />
									<asp:Label ID="lblIncidentDate" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Problem Area" ItemStyle-Width="20%">
								<ItemTemplate>
									<asp:Label ID="lblIssueType" runat="server" Text='<%#Eval("ISSUE_TYPE") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField DataField="DESCRIPTION" HeaderText="Description" HtmlEncode="true" ItemStyle-Width="40%" />
						</Columns>
					</asp:GridView>
					<asp:Label runat="server" ID="lblIncidentListEmpty" Height="40" Text="No Incidents exist matching your search criteria." class="GridEmpty" Visible="false"></asp:Label>
				</div>
			</td>
		</tr>
	</table>

</asp:Panel>

<asp:Panel ID="pnlProbCaseListRepeater" runat="server" Visible="false">
	<div style="padding: 5px;">
		<telerik:RadGrid ID="rgCaseList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="false" OnItemDataBound="rgCaseList_ItemDataBound" OnSortCommand="rgCaseList_SortCommand"
			OnPageIndexChanged="rgCaseList_PageIndexChanged" OnPageSizeChanged="rgCaseList_PageSizeChanged" GridLines="None" Width="100%">
			<MasterTableView ExpandCollapseColumn-Visible="false">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Problem Case ID" ShowSortIcon="true" SortExpression="ProbCase.PROBCASE_ID">
						<ItemTemplate>
							<asp:LinkButton ID="lbCaseId" OnClick="lbIncidentId_Click" CommandArgument='<%#Eval("ProbCase.PROBCASE_ID") %>' runat="server">
								<asp:Label ID="lblCaseId" Font-Bold="true" ForeColor="#000066" Text='<%#string.Format("{0:000000}", Eval("ProbCase.PROBCASE_ID")) %>' runat="server"></asp:Label>
							</asp:LinkButton>
							<asp:HiddenField ID="hfStatus" runat="server" Value='<%#Eval("ProbCase.STATUS") %>' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn UniqueName="colIncident" HeaderText="Incident ID" ShowSortIcon="true" SortExpression="ProbCase.PROBCASE_ID">
						<ItemTemplate>
							<asp:Label ID="lblIncidentID" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="true" SortExpression="ProbCase.DESC_SHORT">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" Text='<%#Server.HtmlEncode((string)Eval("ProbCase.DESC_SHORT")) %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Created" ShowSortIcon="true" SortExpression="ProbCase.CREATE_DT">
						<ItemTemplate>
							<asp:Label ID="lblCreated" Text='<%# ((DateTime)Eval("ProbCase.CREATE_DT")).ToShortDateString() %>' runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Updated" ShowSortIcon="true" SortExpression="ProbCase.LAST_UPD_DT">
						<ItemTemplate>
							<asp:Label ID="lblUpdated" Text='<%# ((DateTime)Eval("ProbCase.LAST_UPD_DT")).ToShortDateString() %>' runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Status" ShowSortIcon="true" SortExpression="ProbCase.CLOSE_DT">
						<ItemTemplate>
							<asp:Label ID="lblStatus" runat="server"></asp:Label>
							<asp:Image ID="imgStatus" runat="server" Visible="false" ToolTip="Case is inactive" Style="vertical-align: middle;" />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn UniqueName="Reports" HeaderText="Reports">
						<ItemTemplate>
							<asp:LinkButton ID="lbReport" runat="server" ForeColor="#000066" OnClick="lbReport_Click" Visible="false"
								CommandArgument='<%#Eval("ProbCase.PROBCASE_ID") %>'>
							<img src="/images/defaulticon/16x16/files.png" alt="" style="vertical-align: middle; margin-left: 4px;" /> 8D Report
							</asp:LinkButton>
							&nbsp;
						<asp:HyperLink ID="hlReport" runat="server" ForeColor="#000066" Target="_blank" Visible="false"
							NavigateUrl='<%# "/EHS/EHS_Alert_PDF.aspx?pcid=" + Eval("ProbCase.PROBCASE_ID") %>'>
							<img src="/images/defaulticon/16x16/open-in-new-window.png" alt="" style="vertical-align: middle;" /> Incident Alert
						</asp:HyperLink>
							<asp:HiddenField ID="hfProblemCaseType" Value='<%#Eval("ProbCase.PROBCASE_TYPE")%>' runat="server" />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
			</MasterTableView>
			<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>


<asp:Panel ID="pnlQualityIncidentHdr" runat="server" Visible="false">
	<table cellspacing="0" cellpadding="1" border="0" width="99%" class="">
		<tr>
			<td class="columnHeader" width="30%">
				<asp:Label runat="server" ID="lblDetectedLocation" CssClass="prompt" Text="Business Location"></asp:Label>
			</td>
			<td class="tableDataAlt" width="70%">
				<asp:Label runat="server" ID="lblDetectedLocation_out"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="columnHeader">
				<asp:Label runat="server" ID="lblIssueDescription" Text="Issue"></asp:Label>
			</td>
			<td class="tableDataAlt">
				<span>
					<asp:Label runat="server" ID="lblIssueID_out"></asp:Label>
					&nbsp;-&nbsp;
			<asp:Label runat="server" ID="lblIssueDesc_out"></asp:Label>
				</span>
			</td>
		</tr>
		<tr>
			<td class="columnHeader">
				<asp:Label runat="server" ID="lblIssuePartNum" CssClass="prompt" Text="Part Number"></asp:Label>
			</td>
			<td class="tableDataAlt">
				<asp:Label runat="server" ID="lblIssuePartNum_out"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="columnHeader">
				<asp:Label runat="server" ID="lblIssueResponsible" CssClass="prompt" Text="Responsible"></asp:Label>
			</td>
			<td class="tableDataAlt">
				<asp:Label runat="server" ID="lblIssueResponsible_out"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlProbCaseHdr" runat="server" Visible="false">
	<asp:HiddenField id="hfLblCaseIDEHS" runat="server" value="Incident ID"/>
	<table cellspacing="0" cellpadding="1" border="0" width="99%">
		<tr>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseID" Text="Problem Case ID"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseID_out"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseType" Text="Case Type"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseType_out"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top" colspan="2">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseStatus" Text="Status"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseStatus_out"></asp:Label>
				<asp:Image ID="imgCaseStatus" runat="server" Visible="false" ToolTip="Case is inactive" Style="vertical-align: middle; margin-left: 4px;" />
			</td>
		</tr>
		<tr>
			<td class="summaryDataTop" valign="top" colspan="2">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCaseDesc" Text="Description"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCaseDesc_out"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblCreateDate" Text="Created"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblCreateDate_out"></asp:Label>
			</td>
			<td class="summaryDataTop" valign="top">
				<span class="summaryHeader">
					<asp:Label runat="server" ID="lblUpdateDate" Text="Last Update"></asp:Label>
				</span>
				<br>
				<asp:Label runat="server" ID="lblUpdateDate_out"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlIncidentTaskHdr" runat="server" Visible="false">
	<table id="tblIncidentTaskHdr" runat="server" cellspacing="0" cellpadding="1" border="0" width="99%" class="">
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
				<asp:Label runat="server" ID="lblIncidentDescription" Text="Incident"></asp:Label>
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

<asp:Panel ID="pnlIncidentListRepeater" runat="server" Visible="false">
	<div>
		<telerik:RadGrid ID="rgIncidentList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="false" OnItemDataBound="rgIncidentList_ItemDataBound" OnSortCommand="rgIncidentList_SortCommand"
			OnPageIndexChanged="rgIncidentList_PageIndexChanged" OnPageSizeChanged="rgIncidentList_PageSizeChanged" GridLines="None" Width="100%">
			<MasterTableView ExpandCollapseColumn-Visible="false">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Incident/<br>Report" ItemStyle-Width="100px" ShowSortIcon="true" SortExpression="Incident.INCIDENT_ID">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbIncidentId" OnClick="lnkEditIncident" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>' runat="server" ToolTip="Edit incident">
											<span style="white-space: nowrap;">
												<img src="/images/ico16-edit.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" /><asp:Label ID="lblIncidentId" Font-Bold="true" ForeColor="#000066" Text='<%#string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' runat="server"></asp:Label>
											</span>
										</asp:LinkButton>
									</td>
									<%--<td>
										<img alt=">" src="/images/arr-rt-grey.png" runat="server" id="imgEditReport" style="opacity: 0.5;" />
									</td>--%>
								   <%-- <td style="width: 50%;">--%>
								</tr>
								<tr>
									<td>
										<asp:LinkButton ID="lb8d" runat="server" OnClick="lnkProblemCaseRedirect" Visible="false" ToolTip="Edit 8D problem case" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'>
											<span class="tableLink" style="color: #a00000; white-space: nowrap;">Edit 8D</span>
										</asp:LinkButton>
										<asp:LinkButton ID="lbEditReport" runat="server" OnClick="lbEditReport_Click" Visible="false" ToolTip="Edit Incident Report" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'>
											<span class="tableLink" style="color: #006080; white-space: nowrap;">Edit Report</span>
										</asp:LinkButton>

									</td>
								</tr>
							</table>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Incident Date/<br>Reported By" ShowSortIcon="true" SortExpression="Incident.INCIDENT_DT">
						<ItemTemplate>
							<asp:Label ID="lblIncidentDT" Text='<%# ((DateTime)Eval("Incident.INCIDENT_DT")).ToShortDateString() %>' runat="server"></asp:Label>
							<br />
							<span style="white-space: nowrap;">
								<asp:Label ID="lblReportedBy" runat="server"></asp:Label></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Type" ShowSortIcon="true" SortExpression="Incident.ISSUE_TYPE">
						<ItemTemplate>
							<asp:Label ID="lblType" runat="server" Text='<%# (string)Eval("Incident.ISSUE_TYPE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="true" SortExpression="Incident.DESCRIPTION">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" Text='<%# HttpUtility.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn UniqueName="Attach" HeaderText="Initial Evidence" ShowSortIcon="false" >
						<ItemTemplate>
							<asp:Label ID="lblAttach" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Status<br/>(Days)">
						<ItemTemplate>
							<asp:Label ID="lblIncStatus" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn UniqueName="ViewReports" HeaderText="View Reports">
						<ItemTemplate>
							<span style="white-space: nowrap;">
								<asp:HyperLink ID="hlReport" runat="server" ForeColor="#000088" Target="_blank" Visible="false">
							<img src="/images/ico16-pdf-download.png" alt="" style="margin-top: -3px; vertical-align: middle;" /> Alert
								</asp:HyperLink>&nbsp;&nbsp;
							<asp:LinkButton ID="lbReport" runat="server" ForeColor="#000088" OnClick="lbReport_Click" Visible="false">
							<img src="/images/ico16-documents.png" alt="" style="margin-top: -3px; vertical-align: middle;" /> 8D Report
							</asp:LinkButton></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
			</MasterTableView>
			<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>

<asp:Panel ID="pnlPreventativeListRepeater" runat="server" Visible="false">
	<div>
		<telerik:RadGrid ID="rgPreventativeList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
			AutoGenerateColumns="false" OnItemDataBound="rgPreventativeList_ItemDataBound" OnSortCommand="rgPreventativeList_SortCommand"
			OnPageIndexChanged="rgIncidentList_PageIndexChanged" OnPageSizeChanged="rgIncidentList_PageSizeChanged" GridLines="None" Width="100%">
			<MasterTableView ExpandCollapseColumn-Visible="false">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Recommendation/<br>Response" ItemStyle-Width="100px" ShowSortIcon="true" SortExpression="Incident.INCIDENT_ID">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbIncidentId" OnClick="lnkEditIncident" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>' runat="server" ToolTip="Edit Recommendation">
											<span style="white-space: nowrap;">
												<img src="/images/ico16-edit.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" /><asp:Label ID="lblIncidentId" Font-Bold="true" ForeColor="#000066" Text='<%#string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' runat="server"></asp:Label>
											</span>
										</asp:LinkButton>
									</td>
								</tr>
								<tr>
									<td>
										<asp:LinkButton ID="lb8d" runat="server" OnClick="lnkProblemCaseRedirect" Visible="false" ToolTip="Edit 8D problem case" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'>
											<span class="tableLink" style="color: #a00000; white-space: nowrap;">Edit 8D</span>
										</asp:LinkButton>
										<asp:LinkButton ID="lbEditReport" runat="server" OnClick="lbEditReport_Click" Visible="false" ToolTip="Edit Response" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'>
											<span class="tableLink" style="color: #006080; white-space: nowrap;">Update Response</span>
										</asp:LinkButton>

									</td>
								</tr>
							</table>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Inspection Date/<br>Entered By" ShowSortIcon="true" SortExpression="Incident.INCIDENT_DT">
						<ItemTemplate>
							<asp:Label ID="lblIncidentDT"  runat="server"></asp:Label>
							<br />
							<span style="white-space: nowrap;">
								<asp:Label ID="lblReportedBy" runat="server"></asp:Label></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Category/<br>Type" ShowSortIcon="true" SortExpression="EntryList[0].ANSWER_VALUE,EntryList[2].ANSWER_VALUE">
						<ItemTemplate>
							<asp:Label ID="lblCategory" runat="server" Text=''></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="true" SortExpression="Incident.DESCRIPTION" ItemStyle-CssClass="tableWithLink">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn UniqueName="Attach" HeaderText="Initial Evidence" ShowSortIcon="false" >
						<ItemTemplate>
							<asp:Label ID="lblAttach" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					  <telerik:GridTemplateColumn HeaderText="Due Date/<br>Assigned To" ShowSortIcon="true" SortExpression="Incident.INCIDENT_DT">
						<ItemTemplate>
							<asp:Label ID="lblDueDT" runat="server"></asp:Label>
							<br />
							<span style="white-space: nowrap;">
								<asp:Label ID="lblAssignedTo" runat="server"></asp:Label></span>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Status<br/>(Days)" SortExpression="DaysOpen">
						<ItemTemplate>
							<asp:Label ID="lblIncStatus" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
			</MasterTableView>
			<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>

<asp:Panel ID="pnlIncidentActionList" runat="server" Visible="false">
	<div>
		<telerik:RadGrid ID="rgIncidentActionList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="10"
			AutoGenerateColumns="false" OnItemDataBound="rgIncidentActionList_ItemDataBound" OnSortCommand="rgIncidentActionList_SortCommand"
			OnPageIndexChanged="rgIncidentActionList_PageIndexChanged" OnPageSizeChanged="rgIncidentActionList_PageSizeChanged" GridLines="None" Width="100%">
			<MasterTableView ExpandCollapseColumn-Visible="false">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Incident" ItemStyle-Width="100px" ShowSortIcon="true" SortExpression="Incident.INCIDENT_ID">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbIncidentId" OnClick="lnkEditIncident" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>' runat="server" ToolTip="Edit incident">
											<span style="white-space: nowrap;">
												<img src="/images/defaulticon/16x16/edit-document.png" alt="" style="vertical-align: top; margin-right: 3px; border: 0" /><asp:Label ID="lblIncidentId" Font-Bold="true" ForeColor="#000066" Text='<%#string.Format("{0:000000}", Eval("Incident.INCIDENT_ID")) %>' runat="server"></asp:Label>
											</span>
										</asp:LinkButton>
									</td>
								</tr>
								<tr>
									<%--<td>
										<img alt=">" src="/images/arr-rt-grey.png" runat="server" id="imgEditReport" style="opacity: 0.5;" />
									</td>
									<td style="width: 50%;">--%>
									<td>
										<asp:LinkButton ID="lb8d" runat="server" OnClick="lnkProblemCaseRedirect" ToolTip="Edit 8D problem case" CommandArgument='<%#Eval("Incident.INCIDENT_ID") %>'>
										<span class="tableLink" style="color: #a00000">Edit 8D</span>
										</asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Incident Type" ShowSortIcon="true" SortExpression="Incident.ISSUE_TYPE">
						<ItemTemplate>
							<asp:Label ID="lblIncidentType" runat="server" Text='<%#Eval("Incident.ISSUE_TYPE") %>'></asp:Label>
							<asp:HiddenField ID="hfIncidentType" runat="server" Value='<%#Eval("Incident.ISSUE_TYPE_ID") %>' />
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="true" SortExpression="Incident.DESCRIPTION">
						<ItemTemplate>
							<asp:Label ID="lblDescription" runat="server" Text='<%#Server.HtmlEncode((string)Eval("Incident.DESCRIPTION")) %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Date Due/<br>Responsible" ShowSortIcon="true" SortExpression="Incident.INCIDENT_ID">
						<ItemTemplate>
							<asp:Label ID="lblDueDT" runat="server"></asp:Label>
							<br />
							<asp:Label ID="lblResponsible" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn HeaderText="Root Cause/<br>Corrective Actions" ItemStyle-Width="35%" ShowSortIcon="true" SortExpression="Incident.INCIDENT_ID">
						<ItemTemplate>
							<telerik:RadGrid ID="rgIncidentActions" runat="server" Skin="Metro" ClientSettings-EnableAlternatingItems="false" AutoGenerateColumns="false" AllowSorting="False"
								AllowPaging="False" GridLines="None" Width="100%" Visible="false" OnItemDataBound="rgIncidentActions_ItemDataBound">
								<MasterTableView ExpandCollapseColumn-Visible="false" ShowHeader="false">
									<Columns>
										<telerik:GridTemplateColumn ShowSortIcon="false" ItemStyle-Width="100%" ItemStyle-BorderStyle="None">
											<ItemTemplate>
												<asp:Label ID="lblTopic" runat="server" CssClass="refTextSmallBold" Text='<%#Eval("ORIGINAL_QUESTION_TEXT") %>'></asp:Label>
												<br />
												<asp:Label ID="lblEntry" runat="server" CssClass="textStd" Text='<%#Eval("ANSWER_VALUE") %>'></asp:Label>
											</ItemTemplate>
										</telerik:GridTemplateColumn>
									</Columns>
								</MasterTableView>
							</telerik:RadGrid>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
				</Columns>
			</MasterTableView>
			<PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
		</telerik:RadGrid>
	</div>
</asp:Panel>


