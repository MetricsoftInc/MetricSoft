<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AuditExceptionList.ascx.cs" Inherits="SQM.Website.Ucl_AuditExceptionList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server" UniqueKey="">
    <PersistenceSettings>
        <telerik:PersistenceSetting ControlID="rgAuditList" />
    </PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTAuditSearch" runat="server" Visible="False" Width="99%" meta:resourcekey="pnlCSTAuditSearchResource1">
    <asp:HiddenField id="hfCSTPlantSelect" runat="server" value="Responsible Location:"/>
    <asp:HiddenField id="hfRCVPlantSelect" runat="server" value="Detected Location:"/>
    <asp:HiddenField ID="hdnAuditPerson" runat="server" />
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
                    <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List assessment exceptions" OnClick="btnAuditsSearchClick" meta:resourcekey="btnSearchResource1" />
                </span>
            </td>
        </tr>
    </table>
 </asp:Panel>


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
                <asp:Label runat="server" ID="lblAuditDescription" Text="Audit"></asp:Label>
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
            OnPageIndexChanged="rgAuditList_PageIndexChanged" OnPageSizeChanged="rgAuditList_PageSizeChanged" Width="100%" OnItemCommand="rgAuditList_ItemCommand" GroupPanelPosition="Top" meta:resourcekey="rgAuditListResource1">
            <MasterTableView DataKeyNames="Audit.Audit_ID" GroupsDefaultExpanded="False">
                <NestedViewTemplate>
                        <telerik:RadGrid runat="server" ID="rgAuditAnswers" OnNeedDataSource="rgAuditAnswers_NeedDataSource" Width="100%" AllowSorting="True" AutoGenerateColumns="False"
                             OnItemCommand="rgAuditAnswers_ItemCommand" OnItemDataBound="rgAuditAnswers_ItemDataBound" GroupPanelPosition="Top" meta:resourcekey="rgAuditAnswersResource1">
                            <MasterTableView DataKeyNames="QuestionId">
                                <NestedViewTemplate>
                                    <telerik:RadGrid runat="server" ID="rgTasks" OnNeedDataSource="rgTasks_NeedDataSource" Width="100%" AutoGenerateColumns="False" OnItemDataBound="rgTasks_ItemDataBound" GroupPanelPosition="Top" meta:resourcekey="rgTasksResource1">
                                        <MasterTableView DataKeyNames="Task.TASK_ID">
                                            <ExpandCollapseColumn Visible="False">
											</ExpandCollapseColumn>
											<Columns>
												<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Responsible Person" meta:resourcekey="GridTemplateColumnResource1" UniqueName="TemplateColumn">
													<ItemTemplate>
														<asp:Label ID="lblTaskAssignedTo" runat="server" meta:resourcekey="lblTaskAssignedToResource1"></asp:Label>
													</ItemTemplate>
												</telerik:GridTemplateColumn>
												<telerik:GridBoundColumn DataField="Task.Due_Dt" DataFormatString="{0:d}" FilterControlAltText="Filter Task.Due_Dt column" HeaderText="Due Date" meta:resourcekey="GridBoundColumnResource1" UniqueName="Task.Due_Dt">
												</telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="Task.Description" FilterControlAltText="Filter Task.Description column" HeaderText="Description" meta:resourcekey="GridBoundColumnResource2" UniqueName="Task.Description">
												</telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="Taskstatus" FilterControlAltText="Filter Taskstatus column" HeaderText="Status" meta:resourcekey="GridBoundColumnResource3" UniqueName="Taskstatus">
												</telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="Task.Comments" FilterControlAltText="Filter Task.Comments column" HeaderText="Comments" meta:resourcekey="GridBoundColumnResource4" UniqueName="Task.Comments">
												</telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="Task.Complete_dt" DataFormatString="{0:d}" FilterControlAltText="Filter Task.Complete_dt column" HeaderText="Complete Date" meta:resourcekey="GridBoundColumnResource5" UniqueName="Task.Complete_dt">
												</telerik:GridBoundColumn>
											</Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </NestedViewTemplate>
                            	<Columns>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" meta:resourcekey="GridTemplateColumnResource2" UniqueName="TemplateColumn">
										<ItemTemplate>
											<asp:HiddenField ID="hdnAuditID" runat="server" Value='<%# Eval("AuditID") %>' />
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridBoundColumn DataField="TopicTitle" FilterControlAltText="Filter TopicTitle column" HeaderText="Topic" meta:resourcekey="GridBoundColumnResource6" UniqueName="TopicTitle">
									</telerik:GridBoundColumn>
									<telerik:GridBoundColumn DataField="QuestionText" FilterControlAltText="Filter QuestionText column" HeaderText="Question" meta:resourcekey="GridBoundColumnResource7" UniqueName="QuestionText">
									</telerik:GridBoundColumn>
									<telerik:GridBoundColumn DataField="AnswerText" FilterControlAltText="Filter AnswerText column" HeaderText="Answer" meta:resourcekey="GridBoundColumnResource8" UniqueName="AnswerText">
									</telerik:GridBoundColumn>
									<telerik:GridBoundColumn DataField="AnswerComment" FilterControlAltText="Filter AnswerComment column" HeaderText="Comment" meta:resourcekey="GridBoundColumnResource9" UniqueName="AnswerComment">
									</telerik:GridBoundColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="Status" meta:resourcekey="GridTemplateColumnResource3" UniqueName="TemplateColumn1">
										<ItemTemplate>
											<asp:Label ID="lblAnswerStatus" runat="server" meta:resourcekey="lblAnswerStatusResource1"></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridBoundColumn DataField="ResolutionComment" FilterControlAltText="Filter ResolutionComment column" HeaderText="Resolution" meta:resourcekey="GridBoundColumnResource10" UniqueName="ResolutionComment">
									</telerik:GridBoundColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="Completed" meta:resourcekey="GridTemplateColumnResource4" UniqueName="TemplateColumn2">
										<ItemTemplate>
											<asp:Label ID="lblResolutionDate" runat="server" meta:resourcekey="lblResolutionDateResource1"></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" meta:resourcekey="GridTemplateColumnResource5" UniqueName="TemplateColumn3">
										<ItemTemplate>
											<asp:LinkButton ID="lnkAddTask" runat="server" meta:resourcekey="lnkAddTaskResource1" OnClick="lnkAddTask_Click" Text="Assign Task" ToolTip="Create a Task to complete this exception"></asp:LinkButton>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" meta:resourcekey="GridTemplateColumnResource6" UniqueName="TemplateColumn4">
										<ItemTemplate>
											<asp:LinkButton ID="lnkUpdateStatus" runat="server" meta:resourcekey="lnkUpdateStatusResource1" OnClick="lnkUpdateStatus_Click" Text="Update Status" ToolTip="Update the status of this exception"></asp:LinkButton>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
								</Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                </NestedViewTemplate>
            	<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Assessment" meta:resourcekey="GridTemplateColumnResource7" SortExpression="Audit.AUDIT_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<asp:Label ID="lblAuditId" runat="server" Font-Bold="True" ForeColor="#000066" meta:resourcekey="lblAuditIdResource1" Text='<%# string.Format("{0:000000}", Eval("Audit.AUDIT_ID")) %>'></asp:Label>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="Assessment Date" meta:resourcekey="GridTemplateColumnResource8" SortExpression="Audit.AUDIT_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblAuditDT" runat="server" meta:resourcekey="lblAuditDTResource1" Text='<%# ((DateTime)Eval("Audit.AUDIT_DT")).ToShortDateString() %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="Location" meta:resourcekey="GridTemplateColumnResource9" SortExpression="Plant.PLANT_NAME" UniqueName="TemplateColumn2">
						<ItemTemplate>
							<asp:Label ID="lblLocation" runat="server" meta:resourcekey="lblLocationResource1" Text='<%# Eval("Plant.PLANT_NAME") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn3 column" HeaderText="Type" meta:resourcekey="GridTemplateColumnResource10" SortExpression="AuditType.TITLE" UniqueName="TemplateColumn3">
						<ItemTemplate>
							<asp:Label ID="lblType" runat="server" meta:resourcekey="lblTypeResource1" Text='<%# (string)Eval("AuditType.TITLE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn4 column" HeaderText="Assessment By" meta:resourcekey="GridTemplateColumnResource11" SortExpression="Audit.AUDIT_PERSON" UniqueName="TemplateColumn4">
						<ItemTemplate>
							<asp:Label ID="lblAuditBy" runat="server" meta:resourcekey="lblAuditByResource1"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="Score" meta:resourcekey="GridTemplateColumnResource12" SortExpression="Audit.TOTAL_SCORE" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblScore" runat="server" meta:resourcekey="lblScoreResource1" Text='<%# Eval("Audit.TOTAL_SCORE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn6 column" HeaderText="Status" meta:resourcekey="GridTemplateColumnResource13" UniqueName="TemplateColumn6">
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



