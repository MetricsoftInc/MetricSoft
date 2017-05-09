<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Action.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Action" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register Src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="UploadAttachment" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AttachVideoPanel.ascx" TagName="AttachVideoPanel" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadScriptBlock.ascx" TagName="RadScript" TagPrefix="Ucl" %>

<script type="text/javascript">

	window.onload = function () {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "";
	}
	window.onbeforeunload = function () {
		if (document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value == '1') {
			return 'You have unsaved changes on this page.';
		}
	}
	function ChangeUpdate(sender, args) {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "1";
		return true;
	}
	function ChangeClear(sender, args) {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "0";
	}

</script>

<asp:Panel ID="pnlAction" Visible="False" runat="server">

	<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	<div id="divTitle" runat="server" visible="false" class="container" style="margin: 5px 0 5px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapAction" runat="server" HorizontalAlign="NotSet">

		<asp:Repeater runat="server" ID="rptAction" ClientIDMode="AutoID" OnItemDataBound="rptAction_OnItemDataBound" OnItemCommand="rptAction_ItemCommand">

			<HeaderTemplate>
				<table width="99%" border="0"  class="lightTable">
					<thead>
					</thead>
			</HeaderTemplate>
			<ItemTemplate>
				<tbody>
					<tr>
						<td class="columnHeader" width="20%">
						<asp:HiddenField ID="hfTaskID" runat="server" />
						<asp:HiddenField ID="hfRecordType" runat="server" />
						<asp:HiddenField ID="hfRecordID" runat="server" />
						<asp:HiddenField ID="hfTaskStep" runat="server" />
						<asp:HiddenField ID="hfTaskType" runat="server" />
						<asp:HiddenField ID="hfTaskStatus" runat="server" />
						<asp:HiddenField ID="hfCompleteID" runat="server" />
						<asp:HiddenField ID="hfCreateDT" runat="server" />
						<asp:HiddenField ID="hfDetail" runat="server" />
						<asp:HiddenField ID="hfComments" runat="server" />
						<asp:HiddenField ID="hfVerification" runat="server" />

						<asp:Label ID="lblFinalAction" runat="server" meta:resourceKey="lbhdFinActionResource1" Text="Final Corrective Action"></asp:Label>
						&nbsp;
						<asp:Label ID="lbItemSeq" runat="server"></asp:Label>
						</td>
						<td class="required" width="1%">&nbsp;</td>
						<td class="tableDataAlt" width="79%">
							<asp:TextBox ID="tbFinalAction" runat="server" Height="65px" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="98%" onChange="ChangeUpdate()"></asp:TextBox>
						</td>
					</tr>
					<tr id="trActionType" runat="server" visible="false">
						<td class="columnHeader">
							<asp:LinkButton ID="lnkSelectActionType" runat="server"  ToolTip="Select corrective action type" CSSClass="buttonComment"  text="Action Type"  CommandArgument="ActionType"  ></asp:LinkButton>
							<br />
							<asp:Label ID="lblSelectActionType" runat="server" CssClass="instructText" Text="Indicate the type of corrective action to be implemeted and explain why other choices were or were not chosen..."></asp:Label>
						</td>
						<td class="tableDataAlt">&nbsp;</td>
						<td class="tableDataAlt">
							<asp:Panel ID="pnlActionType" runat="server" Visible="false">
								<telerik:RadGrid ID="rgActionTypeList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="False" 
									AutoGenerateColumns="False" OnItemDataBound="rgActionTypeList_ItemDataBound" Width="98%" GroupPanelPosition="Top">
									<MasterTableView>
										<ExpandCollapseColumn Visible="False">
										</ExpandCollapseColumn>
										<Columns>
											<telerik:GridTemplateColumn FilterControlAltText="Effectiveness" HeaderText="Effectiveness" ShowSortIcon="False" UniqueName="ActionType" >
												<ItemTemplate>
													<span>
														<asp:Image ID="imgEffectiveness" runat="server" ImageUrl="" />
														<asp:Label ID="lblEffectiveness" runat="server" CssClass="instructTextEmphasis"></asp:Label></asp:Lagel>
													</span>
												</ItemTemplate>
											</telerik:GridTemplateColumn>
											<telerik:GridTemplateColumn FilterControlAltText="Action Type" HeaderText="Action Type" ShowSortIcon="False" UniqueName="ActionType" >
												<ItemTemplate>
													<asp:HiddenField id="hfActionType" runat="server"/>
													<asp:Label ID="lblActionType" runat="server" CssClass="prompt"></asp:Label>
												</ItemTemplate>
											</telerik:GridTemplateColumn>
											<telerik:GridTemplateColumn FilterControlAltText="SelectType" HeaderText="Select" UniqueName="SelectType" ShowSortIcon ="false">
												<ItemTemplate>
													<asp:CheckBox id="cbSelectType" runat="server" OnCheckedChanged="ActionTypeSelect_Checked" AutoPostBack="true"/>
												</ItemTemplate>
											</telerik:GridTemplateColumn>
											<telerik:GridTemplateColumn FilterControlAltText="ActionTypeComment" UniqueName="ActionTypeComment" HeaderText="Selection/Rejection Reason(s)" ShowSortIcon ="false" ItemStyle-Width="40%">
												<ItemTemplate>
													<asp:TextBox ID="tbActionCriteria" runat="server" Height="45px" Rows="2" SkinID="Metro" TextMode="MultiLine" Width="99%"></asp:TextBox>
												</ItemTemplate>
											</telerik:GridTemplateColumn>
										</Columns>
										<PagerStyle AlwaysVisible="True" />
									</MasterTableView>
									<PagerStyle AlwaysVisible="True"></PagerStyle>
								</telerik:RadGrid>
							</asp:Panel>
							<asp:Panel ID="pnlActionTypeSelect" runat="server">
								<asp:HiddenField id="hfActionType" runat="server"/>
								<span>
									<asp:Label ID="lblActionType" runat="server" CssClass="prompt"></asp:Label>
									&nbsp;
									<asp:Image ID="imgActionType" runat="server" ImageUrl="" />
								</span>
								<br />
								<asp:Label ID="lblActionCriteria" runat="server" CssClass="refText"></asp:Label>
							</asp:Panel>
						</td>
					</tr>
					<tr>
						<td class="columnHeader">
							<asp:Label ID="lblAssignedTo" runat="server" Text="<%$ Resources:LocalizedText, AssignedTo %>"></asp:Label>
						</td>
						<td class="required">&nbsp;</td>
						<td class="tableDataAlt">
							<telerik:RadDropDownList ID="rddlActionPerson" runat="server" DropDownHeight="350px" ExpandDirection="Up" OnSelectedIndexChanged="rddlActionPerson_SelectedIndexChanged" Skin="Metro" Width="350" ZIndex="9000" OnClientSelectedIndexChanged="ChangeUpdate">
							</telerik:RadDropDownList>
						</td>
					</tr>
					<tr>
						<td class="columnHeader">
							<asp:Label ID="lblDueDate" runat="server"  meta:resourceKey="lbhdFinStartDTResource1" Text="Target Date"></asp:Label>
						</td>
						<td class="required" width="1%">&nbsp;</td>
						<td class="tableDataAlt">
							<telerik:RadDatePicker ID="rdpFinalStartDate" runat="server" ShowPopupOnFocus="True" Skin="Metro" Width="125px">
								<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
								</Calendar>
								<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="" OnClientDateChanged="ChangeUpdate">
									<EmptyMessageStyle Resize="None" />
									<ReadOnlyStyle Resize="None" />
									<FocusedStyle Resize="None" />
									<DisabledStyle Resize="None" />
									<InvalidStyle Resize="None" />
									<HoveredStyle Resize="None" />
									<EnabledStyle Resize="None" />
								</DateInput>
								<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
							</telerik:RadDatePicker>
						</td>
					</tr>
					<tr>
						<td class="columnHeader">
							<asp:Label ID="lblCompleteDate" runat="server" meta:resourceKey="lbhdFinCompltDTResource1" Text="Completion Date"></asp:Label>
						</td>
						<td class="tableDataAlt">&nbsp;</td>
						<td class="tableDataAlt">
							<telerik:RadDatePicker ID="rdpFinalCompleteDate" runat="server" ShowPopupOnFocus="True" Skin="Metro" Width="125px">
								<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
								</Calendar>
								<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="" OnClientDateChanged="ChangeUpdate">
									<EmptyMessageStyle Resize="None" />
									<ReadOnlyStyle Resize="None" />
									<FocusedStyle Resize="None" />
									<DisabledStyle Resize="None" />
									<InvalidStyle Resize="None" />
									<HoveredStyle Resize="None" />
									<EnabledStyle Resize="None" />
								</DateInput>
								<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
							</telerik:RadDatePicker>
						</td>
					</tr>
					<tr id="trComments" runat="server" visible="false">
						<td class="columnHeader">
							<asp:Label ID="lblComments" runat="server" Text="<%$ Resources:LocalizedText, Comments %>"></asp:Label>
						</td>
						<td class="tableDataAlt">&nbsp;</td>
						<td class="tableDataAlt">
							<asp:TextBox ID="tbComments" runat="server" Height="65px" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="98%" onChange="ChangeUpdate()"></asp:TextBox>
						</td>
					</tr>
					<tr id="trVerification" runat="server" visible="false">
						<td class="columnHeader">
							<asp:Label ID="lblVerification" runat="server" Text="Verification"></asp:Label>
						</td>
						<td class="tableDataAlt">&nbsp;</td>
						<td class="tableDataAlt">
							<asp:TextBox ID="tbVerification" runat="server" Height="65px" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="98%" onChange="ChangeUpdate()"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="text-left-more" colspan="3">
							<telerik:RadButton ID="btnItemDelete" runat="server" BorderStyle="None" ButtonType="LinkButton" CommandArgument="Delete" ForeColor="DarkRed" OnClientClicking="DeleteConfirmItem" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" Text="<%$ Resources:LocalizedText, DeleteItem %>">
							</telerik:RadButton>
						</td>
					</tr>
					<tr><td colspan="3" style="height: 10px;"></td></tr>
				</tbody>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>
     


		<div id="dvAttach" runat="server" class="borderSoft" style="margin-top: 10px;">
			<center>
				<br />
				<asp:Label ID="lbAttachemnt" runat="server" CssClass="sectionTitlesSmall" Text="<%$ Resources:LocalizedText, Attachments %>"></asp:Label>
				<br />
				<Ucl:UploadAttachment ID="uploaderFinalCorrectiveAction" runat="server" />
			</center>
		</div>

         

			<div class="row"  style="margin-top: 10px;">
				<center>
					<span>
						<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
							OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>" />
						<asp:Button ID="btnAddFinal" CssClass="buttonAdd" runat="server" ToolTip="<%$ Resources:LocalizedText, AddAnotherFinalCorrectiveAction %>" Text="<%$ Resources:LocalizedText, AddAnother %>" Style="margin-left: 15px;" CommandArgument="AddAnother"  OnClick="AddDelete_Click"></asp:Button>
					</span>
				</center>
			</div>
		<asp:Label ID="lblStatusMsg" runat="server" CssClass="labelEmphasis"></asp:Label>

	</telerik:RadAjaxPanel>

    

	</div>

</asp:Panel>



