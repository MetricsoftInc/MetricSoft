<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Approval.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Approval" %>
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

</script>

<asp:Panel ID="pnlApproval" Visible="False" runat="server" meta:resourcekey="pnlApprovalResource1">

	<br />

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapApprovals" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapApprovalsResource1">

		<asp:Repeater runat="server" ID="rptApprovals" ClientIDMode="AutoID" OnItemDataBound="rptApprovals_OnItemDataBound" OnItemCommand="rptApprovals_ItemCommand">
			<FooterTemplate>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 col-sm-2 text-left">
						<asp:HiddenField ID="hfItemSeq" runat="server" />
						<asp:HiddenField ID="hfPersonID" runat="server" />
						<span><b>
						<asp:Label ID="lbApproverJob" runat="server" meta:resourcekey="lbApproverJobResource1" SkinID="Metro"></asp:Label>
						<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1"></asp:Label>
						</b>&nbsp;&nbsp;
						<asp:Label ID="lbApprover" runat="server" meta:resourcekey="lbApproverResource1" SkinID="Metro" Width="75%"></asp:Label>
						</span>
					</div>
					<div class="col-xs-12 col-sm-3  text-left">
						<asp:Label ID="lbApproveMessage" runat="server" Height="95px" meta:resourcekey="lbApproveMessageResource1" SkinID="Metro" Width="95%"></asp:Label>
					</div>
					<div class="col-xs-12  col-sm-1 text-left">
						<span>
						<asp:CheckBox ID="cbIsAccepted" runat="server" Font-Bold="False" meta:resourcekey="cbIsAcceptedResource1" SkinID="Metro" />
						</span>
					</div>
					<div class="col-xs-12 col-sm-2 text-left">
						<span>Date&nbsp;
						<telerik:RadDatePicker ID="rdpAcceptDate" runat="server" CssClass="WarnIfChanged" Enabled="False" meta:resourcekey="rdpAcceptDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="120px">
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
						</span>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate>
				<br />
				<br />
			</SeparatorTemplate>
		</asp:Repeater>

		</telerik:RadAjaxPanel>

	</div>
</asp:Panel>



