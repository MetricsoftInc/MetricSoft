<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AuditExceptionList.ascx.cs" Inherits="SQM.Website.Ucl_AuditExceptionList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server">
    <PersistenceSettings>
        <telerik:PersistenceSetting ControlID="rgAuditList" />
    </PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTAuditSearch" runat="server" Visible="false" Width="99%">
    <asp:HiddenField id="hfCSTPlantSelect" runat="server" value="Responsible Location:"/>
    <asp:HiddenField id="hfRCVPlantSelect" runat="server" value="Detected Location:"/>
    <asp:HiddenField ID="hdnAuditPerson" runat="server" />
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
                    <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List assessment exceptions" OnClick="btnAuditsSearchClick" />
<%--                    <asp:Button ID="btnReceiptSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonLink" Text="List Receipts" ToolTip="List material receipts" OnClick="btnReceiptsSearchClick" />--%>
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
        <telerik:RadGrid ID="rgAuditList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
            AutoGenerateColumns="false" OnItemDataBound="rgAuditList_ItemDataBound" OnSortCommand="rgAuditList_SortCommand"
            OnPageIndexChanged="rgAuditList_PageIndexChanged" OnPageSizeChanged="rgAuditList_PageSizeChanged" GridLines="None" Width="100%" OnItemCommand="rgAuditList_ItemCommand">
            <MasterTableView ExpandCollapseColumn-Visible="true" DataKeyNames="Audit.Audit_ID" EnableGroupsExpandAll="false" GroupsDefaultExpanded="false">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Assessment" ItemStyle-Width="100px" ShowSortIcon="true" SortExpression="Audit.AUDIT_ID">
                        <ItemTemplate>
                            <asp:Label ID="lblAuditId" Font-Bold="true" ForeColor="#000066" Text='<%#string.Format("{0:000000}", Eval("Audit.AUDIT_ID")) %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Assessment Date" ShowSortIcon="true" SortExpression="Audit.AUDIT_DT">
                        <ItemTemplate>
                            <asp:Label ID="lblAuditDT" Text='<%# ((DateTime)Eval("Audit.AUDIT_DT")).ToShortDateString() %>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Location" ShowSortIcon="true" SortExpression="Plant.PLANT_NAME">
                        <ItemTemplate>
                            <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("Plant.PLANT_NAME") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Type" ShowSortIcon="true" SortExpression="AuditType.TITLE">
                        <ItemTemplate>
                            <asp:Label ID="lblType" runat="server" Text='<%# (string)Eval("AuditType.TITLE") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Assessment By" ShowSortIcon="true" SortExpression="Audit.AUDIT_PERSON">
                        <ItemTemplate>
                            <asp:Label ID="lblAuditBy" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Score" ShowSortIcon="true" SortExpression="Audit.TOTAL_SCORE">
                        <ItemTemplate>
                            <asp:Label ID="lblScore" runat="server" Text='<%#Eval("Audit.TOTAL_SCORE") %>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="lblAuditStatus" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <NestedViewTemplate>
                    <%--<asp:Panel runat="server" ID="InnerContainer" Visible="true">--%>
                        <telerik:RadGrid runat="server" ID="rgAuditAnswers" OnNeedDataSource="rgAuditAnswers_NeedDataSource" Width="100%" AllowSorting="true" AutoGenerateColumns="false"
                             OnItemCommand="rgAuditAnswers_ItemCommand" OnItemDataBound="rgAuditAnswers_ItemDataBound">
                            <MasterTableView DataKeyNames="QuestionId" ExpandCollapseColumn-Visible="true">
                                <Columns>
                                    <telerik:GridTemplateColumn>
                                        <ItemTemplate>
                                            <asp:HiddenField runat="server" ID="hdnAuditID" Value='<%#Eval("AuditID") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="TopicTitle" HeaderText="Topic"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="QuestionText" HeaderText="Question"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AnswerText" HeaderText="Answer"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="AnswerComment" HeaderText="Comment"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn  HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblAnswerStatus"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                     <telerik:GridBoundColumn DataField="ResolutionComment" HeaderText="Resolution"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn  HeaderText="Completed">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblResolutionDate"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                   <telerik:GridTemplateColumn>
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="lnkAddTask" Text="Assign Task" OnClick="lnkAddTask_Click" ToolTip="Create a Task to complete this exception"></asp:LinkButton>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn>
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="lnkUpdateStatus" Text="Update Status" OnClick="lnkUpdateStatus_Click" ToolTip="Update the status of this exception"></asp:LinkButton>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <NestedViewTemplate>
                                    <telerik:RadGrid runat="server" ID="rgTasks" OnNeedDataSource="rgTasks_NeedDataSource" Width="100%" AllowSorting="false" AutoGenerateColumns="false" OnItemDataBound="rgTasks_ItemDataBound">
                                        <MasterTableView DataKeyNames="Task.TASK_ID" ExpandCollapseColumn-Visible="false">
                                            <Columns>
                                                <telerik:GridTemplateColumn HeaderText="Responsible Person">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTaskAssignedTo" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridBoundColumn DataField="Task.Due_Dt" HeaderText="Due Date" DataFormatString="{0:d}"></telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Task.Description" HeaderText="Description"></telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Taskstatus" HeaderText="Status"></telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Task.Comments" HeaderText="Comments"></telerik:GridBoundColumn>
                                                <telerik:GridBoundColumn DataField="Task.Complete_dt" HeaderText="Complete Date" DataFormatString="{0:d}"></telerik:GridBoundColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </NestedViewTemplate>
                            </MasterTableView>
                        </telerik:RadGrid>
                    <%--</asp:Panel>--%>
                </NestedViewTemplate>
            </MasterTableView>
            <PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Panel>



