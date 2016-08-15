<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_LostTime_Hist.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_LostTime_Hist" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

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

<asp:Panel ID="pnlLostTime" Visible="False" runat="server" meta:resourcekey="pnlLostTimeResource1">

	<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	<div id="divTitle" runat="server" visible="false" class="container" style="margin: 5px 0 5px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapLostTime" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapLostTimeResource1">

		<asp:Repeater runat="server" ID="rptLostTime" ClientIDMode="AutoID" OnItemDataBound="rptLostTime_OnItemDataBound"  OnItemCommand="rptLostTime_ItemCommand">
			<HeaderTemplate>
				<table width="99%" border="0"  class="lightTable">
				<thead>
				</thead>
			</HeaderTemplate>
				<ItemTemplate>
					<tbody>
						<tr>
							<td class="columnHeader" width="20%">
								<asp:Label ID="lbWorkStatus" runat="server" meta:resourcekey="lbWorkStatusResource1" Text="Work Status"></asp:Label>
								&nbsp;
								<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1" Visible="False"></asp:Label>
							</td>
							<td class="required" width="1%">&nbsp;</td>
							<td class="tableDataAlt" width="79%">
								<telerik:RadDropDownList ID="rddlWorkStatus" runat="server" AutoPostBack="True" ExpandDirection="Up" Height="100" meta:resourcekey="rddlWorkStatusResource1"  OnSelectedIndexChanged="rddlw_SelectedIndexChanged" Skin="Metro" Width="300px" ZIndex="9000" OnClientSelectedIndexChanged="ChangeUpdate">
								</telerik:RadDropDownList>
							</td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lbRestrictDesc" runat="server" meta:resourcekey="lbRestrictDescResource1" Text="<%$ Resources:LocalizedText, Comments %>"></asp:Label>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td class="tableDataAlt">
								<asp:TextBox ID="tbRestrictDesc" runat="server" Height="65px" meta:resourcekey="tbRestrictDescResource1" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="98%" onChange="ChangeUpdate()"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lbBeginDate" runat="server" Text="<%$ Resources:LocalizedText, EffectiveDate %>"></asp:Label>
							</td>
							<td class="required">&nbsp;</td>
							<td class="tableDataAlt">
								<telerik:RadDatePicker ID="rdpBeginDate" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpBeginDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="125">
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
						<tr id="trNextMedDate" runat="server">
							<td class="columnHeader">
								<asp:Label ID="lbNextMedDate" runat="server" meta:resourcekey="lbNextMedDateResource1" Text="Next Medical Appt."></asp:Label>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td class="tableDataAlt">
								<telerik:RadDatePicker ID="rdpNextMedDate" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpNextMedDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="125">
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
						<tr id="trExpectedReturnDate" runat="server">
							<td class="columnHeader">
								<asp:Label ID="lbExpectedReturnDT" runat="server" meta:resourcekey="lbExpectedReturnDTResource1" Text="<%$ Resources:LocalizedText, ExpectedReturnDate %>"></asp:Label>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td class="tableDataAlt">
								<telerik:RadDatePicker ID="rdpExpectedReturnDT" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpExpectedReturnDTResource1" ShowPopupOnFocus="True" Skin="Metro" Width="125">
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
			<div class="row">
				<center>
					<span>
						<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
							OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
						<asp:Button ID="btnAddLostTime" CssClass="buttonAdd" runat="server" OnClick="AddDelete_Click" ToolTip="<%$ Resources:LocalizedText, AddAnotherFinalCorrectiveAction %>" Text="<%$ Resources:LocalizedText, AddAnother %>" Style="margin-left: 15px;" CommandArgument="AddAnother"></asp:Button>
					</span>
				</center>
			</div>
		<asp:Label ID="lblStatusMsg" runat="server" CssClass="labelEmphasis"></asp:Label>
	</telerik:RadAjaxPanel>

	</div>

</asp:Panel>
