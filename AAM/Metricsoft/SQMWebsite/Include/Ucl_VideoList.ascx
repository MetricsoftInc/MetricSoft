<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_VideoList.ascx.cs" Inherits="SQM.Website.Ucl_VideoList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadPersistenceManagerProxy ID="RadPersistenceManagerProxy1" runat="server" UniqueKey="">
    <PersistenceSettings>
        <telerik:PersistenceSetting ControlID="rgVideoList" />
    </PersistenceSettings>
</telerik:RadPersistenceManagerProxy>


<asp:Panel ID="pnlCSTVideoSearch" runat="server" Visible="False" Width="99%">
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
                    <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListAssessments %>" OnClick="btnVideoSearchClick" />
                </span>
            </td>
        </tr>
    </table>
 </asp:Panel>

<%--<asp:Panel ID="pnlVideoTaskHdr" runat="server" Visible="False">
    <table id="tblVideoTaskHdr" runat="server" cellspacing="0" cellpadding="1" border="0" width="99%" class="">
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
                <asp:Label runat="server" ID="lblVideoDescription" Text="<%$ Resources:LocalizedText, Video %>"></asp:Label>
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
--%>
<asp:Panel ID="pnlVideoListRepeater" runat="server" Visible="False">
    <div>
        <telerik:RadGrid ID="rgVideoList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
            AutoGenerateColumns="False" OnItemDataBound="rgVideoList_ItemDataBound" OnSortCommand="rgVideoList_SortCommand"
            OnPageIndexChanged="rgVideoList_PageIndexChanged" OnPageSizeChanged="rgVideoList_PageSizeChanged" Width="100%" GroupPanelPosition="Top">
            <MasterTableView>
                <ExpandCollapseColumn Visible="False">
				</ExpandCollapseColumn>
				<Columns>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="<%$ Resources:LocalizedText, Video %>" SortExpression="Video.VIDEO_ID" UniqueName="TemplateColumn">
						<ItemTemplate>
							<table class="innerTable">
								<tr>
									<td>
										<asp:LinkButton ID="lbVideoId" runat="server" CommandArgument='<%# Eval("Video.VIDEO_ID") %>' Font-Bold="True" ForeColor="#000066" meta:resourcekey="lbVideoIdResource1" OnClick="lnkEditVideo" ToolTip="<%$ Resources:LocalizedText, VideoEdit %>" CausesValidation="false">
                                        </asp:LinkButton>
									</td>
								</tr>
							</table>
						</ItemTemplate>
						<ItemStyle Width="100px" />
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, Title %>" SortExpression="Video.VIDEO_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblVideoTitle" runat="server" Text='<%# Eval("Video.TITLE") %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column" HeaderText="<%$ Resources:LocalizedText, VideoDate %>" SortExpression="Video.VIDEO_DT" UniqueName="TemplateColumn1">
						<ItemTemplate>
							<asp:Label ID="lblVideoDT" runat="server" Text='<%# ((DateTime)Eval("Video.VIDEO_DT")).ToShortDateString() %>'></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="<%$ Resources:LocalizedText, VideoOwner %>" SortExpression="Video.VIDEO_PERSON" UniqueName="TemplateColumn5">
						<ItemTemplate>
							<asp:Label ID="lblVideoBy" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn7 column" HeaderText="<%$ Resources:LocalizedText, Status %>" UniqueName="TemplateColumn7">
						<ItemTemplate>
							<asp:Label ID="lblVideoStatus" runat="server"></asp:Label>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn8 column" HeaderText="<%$ Resources:LocalizedText, VideoDownload %>" UniqueName="TemplateColumn8">
						<ItemTemplate>
							<a href='<%# "/Shared/FileHandler.ashx?DOC=v&DOC_ID="+ Eval("Video.VIDEO_ID").ToString() + "&FILE_NAME=" + Eval("Video.FILE_NAME").ToString() %>' target="_blank"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoDownload %>"></asp:Literal></a>
						</ItemTemplate>
					</telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn9 column" HeaderText="<%$ Resources:LocalizedText, Delete %>" UniqueName="TemplateColumn9">
                        <ItemTemplate><asp:LinkButton ID="lbDelete" CausesValidation="false" runat="server" ForeColor="#000066" Visible="true" Text="<%$ Resources:LocalizedText, Delete %>" CommandName='<%# Eval("Video.FILE_NAME") %>' CommandArgument='<%# Eval("Video.VIDEO_ID") %>' OnClick="lbDelete_Click"></asp:LinkButton></ItemTemplate>
                    </telerik:GridTemplateColumn>
				</Columns>
				<PagerStyle AlwaysVisible="True" />
            </MasterTableView>
            <PagerStyle AlwaysVisible="True"></PagerStyle>
        </telerik:RadGrid>
    </div>
</asp:Panel>



