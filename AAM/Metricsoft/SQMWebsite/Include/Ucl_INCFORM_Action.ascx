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

<asp:Panel ID="pnlAction" Visible="False" runat="server" meta:resourcekey="pnlActionResource1">

	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapAction" runat="server" HorizontalAlign="NotSet" meta:resourceKey="rapActionResource1">

		<asp:Repeater runat="server" ID="rptAction" ClientIDMode="AutoID" OnItemDataBound="rptAction_OnItemDataBound" OnItemCommand="rptAction_ItemCommand">

			<FooterTemplate>
				</table>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddFinal" CssClass="buttonAdd" runat="server" ToolTip="Add Another Final Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" meta:resourceKey="btnAddFinalResource1" ></asp:Button>
					</div>
				</div>
			</FooterTemplate>
			<HeaderTemplate>
				<table border="0" class="table">
					<thead>
						<tr>
							<th class="text-center"><b>
								<asp:Label ID="lbhdItem" runat="server" meta:resourceKey="lbhdItemResource1" Text="Item"></asp:Label>
								</b></th>
							<th class="col-sm-4 text-left-more"><b>
								<asp:Label ID="lbhdFinAction" runat="server" meta:resourceKey="lbhdFinActionResource1" Text="Final Action"></asp:Label>
								</b></th>
							<th class="col-sm-2 text-left-more"><b>
								<asp:Label ID="lbhdFinAssignTo" runat="server" meta:resourceKey="lbhdFinAssignToResource1" Text="Assigned To"></asp:Label>
								</b></th>
							<th class="col-sm-2 text-left-more"><b>
								<asp:Label ID="lbhdFinStartDT" runat="server" meta:resourceKey="lbhdFinStartDTResource1" Text="Target Date"></asp:Label>
								</b></th>
							<th class="col-sm-2 text-left-more"><b>
								<asp:Label ID="lbhdFinCompltDT" runat="server" meta:resourceKey="lbhdFinCompltDTResource1" Text="Completion Date"></asp:Label>
								</b></th>
							<th class="col-sm-1 text-left-more" style="margin-left:-5px;"><b>
								<asp:Label ID="lbhdFinComplete" runat="server" meta:resourceKey="lbhdFinCompleteResource1" Text="Cmpltd"></asp:Label>
								</b></th>
							<th class="col-sm-1 text-left-more"></th>
						</tr>
					</thead>
				</table>
			</HeaderTemplate>
			<ItemTemplate>
				<tbody>
					<tr>
						<td class="text-center">
							<asp:Label ID="lbItemSeq" runat="server" meta:resourceKey="lbItemSeqResource1"></asp:Label>
							<asp:HiddenField ID="hfTaskID" runat="server" />
							<asp:HiddenField ID="hfRecordType" runat="server" />
							<asp:HiddenField ID="hfRecordID" runat="server" />
							<asp:HiddenField ID="hfTaskStep" runat="server" />
							<asp:HiddenField ID="hfTaskType" runat="server" />
							<asp:HiddenField ID="hfTaskStatus" runat="server" />
							<asp:HiddenField ID="hfCompleteID" runat="server" />
							<asp:HiddenField ID="hfCreateDT" runat="server" />
							<asp:HiddenField ID="hfDetail" runat="server" />
							<asp:HiddenField ID="hfComments" runat="server" />
						</td>
						<td class="text-left-more">
							<asp:TextBox ID="tbFinalAction" runat="server" Height="65px" meta:resourceKey="tbFinalActionResource1" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="100%"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvFinalAction" runat="server" ControlToValidate="tbFinalAction" Display="None" ErrorMessage="Required" meta:resourceKey="rfvFinalActionResource1"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left-more">
							<telerik:RadDropDownList ID="rddlActionPerson" runat="server" CssClass="WarnIfChanged" DropDownHeight="350px" ExpandDirection="Up" meta:resourceKey="rddlActionPersonResource1" OnSelectedIndexChanged="rddlActionPerson_SelectedIndexChanged" Skin="Metro" Width="90%" ZIndex="9000">
							</telerik:RadDropDownList>
							<asp:RequiredFieldValidator ID="rfvActionPerson" runat="server" ControlToValidate="rddlActionPerson" Display="None" ErrorMessage="Required" InitialValue="[Select One]" meta:resourceKey="rfvActionPersonResource1"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left-more">
							<telerik:RadDatePicker ID="rdpFinalStartDate" runat="server" CssClass="WarnIfChanged" meta:resourceKey="rdpFinalStartDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="115px">
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
							<asp:RequiredFieldValidator ID="rvfFinalStartDate" runat="server" ControlToValidate="rdpFinalStartDate" Display="None" ErrorMessage="Required" meta:resourceKey="rvfFinalStartDateResource1"></asp:RequiredFieldValidator>
						</td>
						<td class="text-left-more">
							<telerik:RadDatePicker ID="rdpFinalCompleteDate" runat="server" CssClass="WarnIfChanged" meta:resourceKey="rdpFinalCompleteDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="115px">
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
						</td>
						<td class="text-left">
							<asp:CheckBox ID="cbFinalIsComplete" runat="server" meta:resourceKey="cbFinalIsCompleteResource1" SkinID="Metro" Width="90%" />
						</td>
						<td class="text-left-more">
							<telerik:RadButton ID="btnItemDelete" runat="server" BorderStyle="None" ButtonType="LinkButton" CommandArgument="Delete" ForeColor="DarkRed" OnClientClicking="DeleteConfirmItem" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" Text="Delete Item">
							</telerik:RadButton>
						</td>
					</tr>
				</tbody>
			</ItemTemplate>
		</asp:Repeater>

	</telerik:RadAjaxPanel>

	</div>

</asp:Panel>
