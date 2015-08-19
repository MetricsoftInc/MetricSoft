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

<asp:Panel ID="pnlApproval" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapApprovals" runat="server">  

		<asp:Repeater runat="server" ID="rptApprovals" ClientIDMode="AutoID" OnItemDataBound="rptApprovals_OnItemDataBound" OnItemCommand="rptApprovals_ItemCommand">
			<HeaderTemplate></HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 col-sm-2 text-left">
						<span><b><asp:Label ID="lbApproverJob" SkinID="Metro" runat="server" /><asp:Label ID="lbItemSeq" runat="server"></asp:Label></b>&nbsp;&nbsp;
							<asp:Label ID="lbApprover" Width="75%" SkinID="Metro" runat="server"></asp:Label></span>
					</div>
					<div class="col-xs-12 col-sm-3  text-left">
						<asp:Label ID="lbApproveMessage" Height="95px" Width="95%"  SkinID="Metro" runat="server"></asp:Label>
					</div>
					<div class="col-xs-12  col-sm-1 text-left">
						<span><asp:CheckBox ID="cbIsAccepted" runat="server" Font-Bold="false" Text="Accepted" SkinID="Metro" TextAlign="Right"></asp:CheckBox></span>
					</div>
					<div class="col-xs-12 col-sm-2 text-left">
						<span>Date Accepted:&nbsp;
						<telerik:RadDatePicker ID="rdpAcceptDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="120" runat="server" ShowPopupOnFocus="true"></telerik:RadDatePicker></span>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate><br /><br /></SeparatorTemplate>
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
		</asp:Repeater>

		</telerik:RadAjaxPanel>

	</div>
</asp:Panel>



