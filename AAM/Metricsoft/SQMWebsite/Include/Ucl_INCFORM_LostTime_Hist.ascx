<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_LostTime_Hist.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_LostTime_Hist" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<script type="text/javascript">

	function OnEditorClientLoad(editor) {
		editor.attachEventHandler("ondblclick", function (e) {
			var sel = editor.getSelection().getParentElement(); //get the currently selected element
			var href = null;
			if (sel.tagName === "A") {
				href = sel.href; //get the href value of the selected link
				window.open(href, null, "height=500,width=500,status=no,toolbar=no,menubar=no,location=no");
				return false;
			}
		}
		);
	}

	function StandardConfirm(sender, args) {

		// Some pages will have no validators, so skip
		if (typeof Page_ClientValidate === "function") {
			var validated = Page_ClientValidate('Val_PowerOutage');

			if (!validated)
				alert("Please fill out all of the required fields.");
		}
	}

	$(function () {
		$('#tblCustomers').footable();
	});

</script>

<asp:Panel ID="pnlLostTime" Visible="False" runat="server" meta:resourcekey="pnlLostTimeResource1">

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapLostTime" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapLostTimeResource1">

		<asp:Repeater runat="server" ID="rptLostTime" ClientIDMode="AutoID" OnItemDataBound="rptLostTime_OnItemDataBound"  OnItemCommand="rptLostTime_ItemCommand">
			<HeaderTemplate>
				<table border="0" class="table">
					<thead>
						<tr class="row">
							<th class="col-sm-1 text-left-more"><b>
								<asp:Label ID="lbWorkStatus" runat="server" meta:resourcekey="lbWorkStatusResource1" Text="Work Status"></asp:Label>
								</b></th>
							<th class="col-sm-3 text-left-more"><b>
								<asp:Label ID="lbRestrictDesc" runat="server" meta:resourcekey="lbRestrictDescResource1" Text="<%$ Resources:LocalizedText, Comments %>"></asp:Label>
								</b></th>
							<th class="col-sm-2 text-left-more"><b>
								<asp:Label ID="lbBeginDate" runat="server" Text="<%$ Resources:LocalizedText, EffectiveDate %>"></asp:Label>
								</b></th>
							<th class="col-sm-2 text-left-more"><b>
								<asp:Label ID="lbNextMedDate" runat="server" meta:resourcekey="lbNextMedDateResource1" Text="Next Medical Appt."></asp:Label>
								</b></th>
							<th class="col-sm-2 text-left-more"><b>
								<asp:Label ID="lbExpectedReturnDT" runat="server" meta:resourcekey="lbExpectedReturnDTResource1" Text="<%$ Resources:LocalizedText, ExpectedReturnDate %>"></asp:Label>
								</b></th>
							<th class="text-left-more"/>
							</th>
						</tr>
					</thead>
			</HeaderTemplate>
			<ItemTemplate>
				<tbody>
					<tr class="row">
						<td class="text-left-more">
							<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1" Visible="False"></asp:Label>
							<telerik:RadDropDownList ID="rddlWorkStatus" runat="server" AutoPostBack="True" CssClass="WarnIfChanged" ExpandDirection="Up" Height="100" meta:resourcekey="rddlWorkStatusResource1" on="" OnSelectedIndexChanged="rddlw_SelectedIndexChanged" Skin="Metro" Width="180px" ZIndex="9000">
							</telerik:RadDropDownList>
							<asp:RequiredFieldValidator ID="rfvWorkStatus" runat="server" ControlToValidate="rddlWorkStatus" Display="None" Enabled="False" ErrorMessage="<%$ Resources:LocalizedText, Required %>" InitialValue="[Select One]"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left-more">
							<asp:TextBox ID="tbRestrictDesc" runat="server" Height="65px" meta:resourcekey="tbRestrictDescResource1" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="90%"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvRestrictDesc" runat="server" ControlToValidate="tbRestrictDesc" Display="None" Enabled="False" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left-more">
							<telerik:RadDatePicker ID="rdpBeginDate" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpBeginDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="95%">
								<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
								</Calendar>
								<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
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
							<asp:RequiredFieldValidator ID="rvfBeginDate" runat="server" ControlToValidate="rdpBeginDate" Display="None" Enabled="False" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left-more">
							<telerik:RadDatePicker ID="rdpNextMedDate" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpNextMedDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="95%">
								<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
								</Calendar>
								<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
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
							<asp:RequiredFieldValidator ID="rfvNextMedDate" runat="server" ControlToValidate="rdpNextMedDate" Display="None" Enabled="False" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left">
							<telerik:RadDatePicker ID="rdpExpectedReturnDT" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpExpectedReturnDTResource1" ShowPopupOnFocus="True" Skin="Metro" Width="95%">
								<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
								</Calendar>
								<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
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
							<asp:RequiredFieldValidator ID="rfvExpectedReturnDT" runat="server" ControlToValidate="rdpExpectedReturnDT" Display="None" Enabled="False" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
						</td>
						<td class="col-xs-12 text-left-more">
							<telerik:RadButton ID="btnItemDelete" runat="server" BorderStyle="None" ButtonType="LinkButton" CommandArgument="Delete" ForeColor="DarkRed" OnClientClicking="DeleteConfirmItem" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" Text="<%$ Resources:LocalizedText, DeleteItem %>">
							</telerik:RadButton>
						</td>
					</tr>
				</tbody>
			</ItemTemplate>
			<FooterTemplate>
				</table>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddLostTime" CssClass="buttonAdd" runat="server" ToolTip="<%$ Resources:LocalizedText, AddAnotherFinalCorrectiveAction %>" Text="<%$ Resources:LocalizedText, AddAnother %>" Style="margin: 7px;" CommandArgument="AddAnother"></asp:Button>
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</telerik:RadAjaxPanel>

	</div>

</asp:Panel>
