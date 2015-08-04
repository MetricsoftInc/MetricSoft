<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Action.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Action" %>
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

<asp:Panel ID="pnlAction" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<asp:Repeater runat="server" ID="rptAction" ClientIDMode="AutoID" OnItemDataBound="rptAction_OnItemDataBound" OnItemCommand="rptAction_ItemCommand">

			<HeaderTemplate>
					
				<div class="row">

					<div class="col-sm-2 text-center">
						<span><b><asp:Label ID="lbFinAction" runat ="server" Text="Action"></asp:Label></b></span>
					</div>

					<div class="col-sm-3  text-center">
						<span><b><asp:Label ID="lbFinAssignedTo" runat ="server" Text="Assigned To"></asp:Label></b></span>
					</div>

					<div class="col-sm-1 text-left-more">
						<span><b><asp:Label ID="lbFinStartDate" runat ="server" Text="Start Date"></asp:Label></b></span>
					</div>

					<div class="col-sm-2 text-center">
						<span style="padding-left:10px;"><b><asp:Label ID="lbFinCompletionDate" runat ="server" Text="  Completion Date"></asp:Label></b></span>
					</div>

				</div>
				<br />
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row-fluid">

					<div class="col-xs-12  col-sm-3 text-left-more">
						<span><span style="display:inline-block; vertical-align:top;"><asp:Label ID="lbActionPrompt" Text="Action " runat="server"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:&nbsp;</span>
						<asp:TextBox ID="tbFinalAction" Rows="3" Height="65px" Width="300" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox></span>
						<asp:RequiredFieldValidator runat="server" ID="rfvFinalAction" ControlToValidate="tbFinalAction" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDropDownList ID="rddlActionPerson" Skin="Metro" CssClass="WarnIfChanged" Width="198" runat="server" OnSelectedIndexChanged="rddlActionPerson_SelectedIndexChanged"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvActionPerson" ControlToValidate="rddlActionPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more"> 
						<telerik:RadDatePicker ID="rdpFinalStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
						<asp:RequiredFieldValidator runat="server" ID="rvfFinalStartDate" ControlToValidate="rdpFinalStartDate" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDatePicker ID="rdpFinalCompleteDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
					</div>

					<div class="col-xs-12  col-sm-1 text-left-more">
						<asp:CheckBox ID="cbFinalIsComplete" runat="server" Text="Complete" SkinID="Metro" TextAlign="Right"></asp:CheckBox>
					</div>
				</div>
				<br style="float:left; clear:both;"/><br />
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
						<asp:Button ID="btnAddFinal" CssClass="buttonAdd" runat="server" ToolTip="Add Another Final Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
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
