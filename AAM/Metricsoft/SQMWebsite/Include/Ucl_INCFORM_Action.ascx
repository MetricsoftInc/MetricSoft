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

	$(function () {
		$('#tblCustomers').footable();
	});

</script>

<asp:Panel ID="pnlAction" Visible="false" runat="server">

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapAction" runat="server">

		<asp:Repeater runat="server" ID="rptAction" ClientIDMode="AutoID" OnItemDataBound="rptAction_OnItemDataBound" OnItemCommand="rptAction_ItemCommand">

			<HeaderTemplate>
					<table class="table" border="0" >
						<thead>
							<tr>
								<th class="text-center"><b><asp:Label ID="lbhdItem" runat ="server" Text="Item" /></b></th>
								<th class="col-sm-4 text-left-more"><b><asp:Label ID="lbhdFinAction" runat ="server" Text="Final Action" /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbhdFinAssignTo" runat ="server" Text="Assigned To" /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbhdFinStartDT" runat ="server" Text="Target Date" /></b></th>
								<th class="col-sm-2 text-left-more"><b><asp:Label ID="lbhdFinCompltDT" runat ="server" Text="Completion Date" /></b></th>
								<th class="col-sm-1 text-left-more" style="margin-left:-5px;"><b><asp:Label ID="lbhdFinComplete" runat ="server" Text="Cmpltd" /></b></th>
								<th class="col-sm-1 text-left-more"></th>
							</tr>
						</thead>
			</HeaderTemplate>
			<ItemTemplate>
			<tbody>
				<tr>
					<td class="text-center">
						<asp:Label ID="lbItemSeq" runat="server" />
						<asp:HiddenField id="hfTaskID" runat="server"/>
						<asp:HiddenField id="hfRecordType" runat="server"/>
						<asp:HiddenField id="hfRecordID" runat="server"/>
						<asp:HiddenField id="hfTaskStep" runat="server"/>
						<asp:HiddenField id="hfTaskType" runat="server"/>
						<asp:HiddenField id="hfTaskStatus" runat="server"/>
						<asp:HiddenField id="hfCompleteID" runat="server"/>
						<asp:HiddenField id="hfCreateDT" runat="server"/>
						<asp:HiddenField id="hfDetail" runat="server"/>
						<asp:HiddenField id="hfComments" runat="server"/>
					</td>
					<td class="text-left-more">
						<asp:TextBox ID="tbFinalAction" Rows="3" Height="65px" Width="100%" TextMode="MultiLine" SkinID="Metro" runat="server" />
						<asp:RequiredFieldValidator runat="server" ID="rfvFinalAction" ControlToValidate="tbFinalAction" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDropDownList ID="rddlActionPerson" Skin="Metro" Width="90%" CssClass="WarnIfChanged" ZIndex="9000" DropDownHeight="350" ExpandDirection="Up" runat="server" OnSelectedIndexChanged="rddlActionPerson_SelectedIndexChanged" />
						<asp:RequiredFieldValidator runat="server" ID="rfvActionPerson" ControlToValidate="rddlActionPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpFinalStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true" Width="115"  runat="server" ShowPopupOnFocus="true" />
						<asp:RequiredFieldValidator runat="server" ID="rvfFinalStartDate" ControlToValidate="rdpFinalStartDate" Display="None" ErrorMessage="Required" />
					</td>
					<td class="text-left-more">
						<telerik:RadDatePicker ID="rdpFinalCompleteDate" Skin="Metro" CssClass="WarnIfChanged" Width="115" Enabled="true" runat="server" ShowPopupOnFocus="true" />
					</td>
					<td class="text-left">
						<asp:CheckBox ID="cbFinalIsComplete" runat="server" Width="90%" SkinID="Metro" />
					</td>
					<td class="text-left-more">
						<telerik:RadButton ID="btnItemDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" ForeColor="DarkRed"  CommandArgument="Delete" 
							Text="Delete Item" SingleClick="true" SingleClickText="Deleting..." OnClientClicking="DeleteConfirmItem" />
					</td>
				</tbody>
			</ItemTemplate>
			<FooterTemplate>
				</table> 
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddFinal" CssClass="buttonAdd" runat="server" ToolTip="Add Another Final Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</telerik:RadAjaxPanel>

	</div>   

</asp:Panel>
