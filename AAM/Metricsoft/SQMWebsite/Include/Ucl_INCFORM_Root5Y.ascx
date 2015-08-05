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

</script>


 <asp:Panel ID="pnlRoot5Y" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

<%--		<span style="display:inline-block;">This is the new ROOT CAUSE user control</span><br />--%>

		<asp:Repeater runat="server" ID="rptRootCause" ClientIDMode="AutoID" OnItemDataBound="rptRootCause_OnItemDataBound" OnItemCommand="rptRootCause_ItemCommand">
			<HeaderTemplate></HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 text-left">
						<span><asp:Label ID="lbWhyPrompt" Text="Why " runat="server"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:</span>
					</div>
					<div class="col-xs-12 text-left">
						<asp:TextBox ID="tbRootCause" Rows="5" Height="95px" Width="50%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="rfvRootCause" ControlToValidate="tbRootCause" Display="None" ErrorMessage="Required"></asp:RequiredFieldValidator>
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
						<asp:Button ID="btnAddRootCause" CssClass="buttonAdd" runat="server" ToolTip="Add Another Root Cause" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</div>

</asp:Panel>

