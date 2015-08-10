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

<asp:Panel ID="pnlLostTime" Visible="false" runat="server">

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapLostTime" runat="server">

		<asp:Repeater runat="server" ID="rptLostTime" ClientIDMode="AutoID" OnItemDataBound="rptLostTime_OnItemDataBound" OnItemCommand="rptLostTime_ItemCommand">

			<HeaderTemplate>
					<table class="table" border="0" >
						<thead>
							<tr>
								<th class="text-center"><b><asp:Label ID="lbhdItem" runat ="server" Text="Item" /></b></th>
								<th class="col-sm-1 text-left-more"><b><asp:Label ID="lbWorkStatus" runat="server" Text="Status" /></b></th>
								<th class="col-sm-3 text-left-more"><b><asp:Label ID="lbRestrictDesc" runat="server" Text="Restriction Desc." /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbBeginDate" runat="server" Text="Begin Date" /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbReturnDate" runat="server" Text="Return Date: " /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbNextMedDate" runat="server" Text="Next Medical Appt." /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbExpectedReturnDT" runat="server" Text="Expected Return Date" /></b></th>
							</tr>
						</thead>
			</HeaderTemplate>
			<ItemTemplate>
	        <tbody>
				<tr>
					<td class="text-center">
						<asp:Label ID="lbItemSeq" runat="server" />
					</td>
					<td class="text-left-more">
						<telerik:RadDropDownList ID="rddlWorkStatus" Skin="Metro" CssClass="WarnIfChanged" Width="90%" runat="server"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvWorkStatus" ControlToValidate="rddlWorkStatus" Display="None" InitialValue="[Select One]" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<asp:TextBox ID="tbRestrictDesc" Rows="3" Height="65px" Width="100%" TextMode="MultiLine" SkinID="Metro" runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rfvRestrictDesc" ControlToValidate="tbRestrictDesc" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpBeginDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rvfBeginDate" ControlToValidate="rdpBeginDate" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpReturnDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rfvReturnDate" ControlToValidate="rdpReturnDate" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpNextMedDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rfvNextMedDate" ControlToValidate="rdpNextMedDate" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left">
						<telerik:RadDatePicker ID="rdpExpectedReturnDT" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rfvExpectedReturnDT" ControlToValidate="rdpExpectedReturnDT" Display="None" ErrorMessage="Required" />
					</td>
				</tbody>
			</ItemTemplate>
			<FooterTemplate>
				</table> 
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddLostTime" CssClass="buttonAdd" runat="server" ToolTip="Add Another Final Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</telerik:RadAjaxPanel>

	</div>   

</asp:Panel>
