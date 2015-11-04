<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Root5Y.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Root5Y" %>
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


 <asp:Panel ID="pnlRoot5Y" Visible="False" runat="server">

	<br />

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapRoot5Y" runat="server" HorizontalAlign="NotSet">

		<asp:Repeater runat="server" ID="rptRootCause" ClientIDMode="AutoID" OnItemDataBound="rptRootCause_OnItemDataBound" OnItemCommand="rptRootCause_ItemCommand">
			<FooterTemplate>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddRootCause" runat="server" CommandArgument="AddAnother" CssClass="buttonAdd" meta:resourcekey="btnAddRootCauseResource1" Style="margin: 7px;" Text="<%$ Resources:LocalizedText, AddAnother %>" ToolTip="Add Another Root Cause" />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
			<HeaderTemplate></HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-sm-1 text-left">
						<span><asp:Label ID="lbWhyPrompt" Text="Why " runat="server" meta:resourcekey="lbWhyPromptResource1"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:</span>
					</div>
					<div class="col-sm-5 text-left">
						<asp:TextBox ID="tbRootCause" Rows="5" Height="95px" Width="95%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="rfvRootCause" ControlToValidate="tbRootCause" Display="None" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
					</div>
					<div class="col-sm-2 text-left-more">
						<telerik:RadButton ID="btnItemDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" ForeColor="DarkRed"  CommandArgument="Delete"
							Text="<%$ Resources:LocalizedText, DeleteItem %>" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" OnClientClicking="DeleteConfirmItem" />
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate><br /><br /></SeparatorTemplate>
		</asp:Repeater>

	</telerik:RadAjaxPanel>

	</div>

</asp:Panel>

