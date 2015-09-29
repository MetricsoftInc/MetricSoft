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

		<asp:Repeater runat="server" ID="rptLostTime" ClientIDMode="AutoID" OnItemDataBound="rptLostTime_OnItemDataBound"  OnItemCommand="rptLostTime_ItemCommand">

			<HeaderTemplate>
					<table class="table" border="0" >
						<thead>
							<tr class="row">
<%--								<th class="text-center"><b><asp:Label ID="lbhdItem" runat ="server" Text="Item" /></b></th>--%>
								<th class="col-sm-1 text-left-more"><b><asp:Label ID="lbWorkStatus" runat="server" Text="Work Status" /></b></th>
								<th class="col-sm-3 text-left-more"><b><asp:Label ID="lbRestrictDesc" runat="server" Text="Comments" /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbBeginDate" runat="server" Text="Effective Date" /></b></th>
<%--								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbReturnDate" runat="server" Text="Return Date" /></b></th>--%>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbNextMedDate" runat="server" Text="Next Medical Appt." /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbExpectedReturnDT" runat="server" Text="Expected Return Date" /></b></th>
								<th class="text-left-more"/></th>
							</tr>
						</thead>
			</HeaderTemplate>
			<ItemTemplate>
			<tbody>
				<tr class="row">
					<td class="text-left-more">
						<asp:Label ID="lbItemSeq" runat="server" Visible="false" />
						<telerik:RadDropDownList ID="rddlWorkStatus" Skin="Metro" on CssClass="WarnIfChanged" ZIndex="9000" ExpandDirection="Up" Height="100" Width="180" autopostback="true" runat="server" OnSelectedIndexChanged="rddlw_SelectedIndexChanged"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvWorkStatus" ControlToValidate="rddlWorkStatus" Enabled="false" Display="None" InitialValue="[Select One]" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<asp:TextBox ID="tbRestrictDesc" Rows="3" Height="65px" Width="90%" TextMode="MultiLine" SkinID="Metro" runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rfvRestrictDesc" ControlToValidate="tbRestrictDesc"  Enabled="false" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpBeginDate" Skin="Metro" Width="95%" CssClass="WarnIfChanged" runat="server" ShowPopupOnFocus="true" />
						<asp:RequiredFieldValidator runat="server" ID="rvfBeginDate" ControlToValidate="rdpBeginDate"  Enabled="false" Display="None" ErrorMessage="Required" />
					</td>
<%--					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpReturnDate" Skin="Metro" Width="95%"  CssClass="WarnIfChanged" runat="server" ShowPopupOnFocus="true" />
						<asp:RequiredFieldValidator runat="server" ID="rfvReturnDate" ControlToValidate="rdpReturnDate"  Enabled="false" Display="None" ErrorMessage="Required" />
					</td>--%>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpNextMedDate" Skin="Metro" Width="95%"  CssClass="WarnIfChanged" runat="server" ShowPopupOnFocus="true" />
						<asp:RequiredFieldValidator runat="server" ID="rfvNextMedDate" ControlToValidate="rdpNextMedDate"  Enabled="false" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left">
						<telerik:RadDatePicker ID="rdpExpectedReturnDT" Skin="Metro" Width="95%"  CssClass="WarnIfChanged" runat="server" ShowPopupOnFocus="true"/>
						<asp:RequiredFieldValidator runat="server" ID="rfvExpectedReturnDT" ControlToValidate="rdpExpectedReturnDT"  Enabled="false" Display="None" ErrorMessage="Required" />
					</td>
					<td class="col-xs-12 text-left-more">
						<telerik:RadButton ID="btnItemDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" ForeColor="DarkRed"  CommandArgument="Delete" 
							Text="Delete Item" SingleClick="true" SingleClickText="Deleting..." OnClientClicking="DeleteConfirmItem" />
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
