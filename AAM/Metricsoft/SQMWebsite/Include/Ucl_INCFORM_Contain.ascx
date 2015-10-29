<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Contain.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Contain" %>
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


<asp:Panel ID="pnlContain" Visible="False" runat="server" meta:resourcekey="pnlContainResource1">
    <div class="container-fluid">
        <telerik:RadAjaxPanel ID="rapContain"  runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapContainResource1">
            <asp:Repeater runat="server" ID="rptContain" ClientIDMode="AutoID" OnItemDataBound="rptContain_OnItemDataBound" OnItemCommand="rptContain_ItemCommand">
                <FooterTemplate>
                    </table>
                    <div class="row" style="padding-top:0;margin-top:-10px;">
                        <div class="col-xs-12 text-left-more">
                            <asp:Button ID="btnAddContain" CssClass="buttonAdd" runat="server" ToolTip="Add Another Initial Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" meta:resourcekey="btnAddContainResource1"></asp:Button>
                        </div>
                    </div>
                </FooterTemplate>
            	<HeaderTemplate>
					<table border="0" class="table">
						<thead>
							<tr>
								<th class="col-sm-1 text-center"><b>
									<asp:Label ID="lbhdItem" runat="server" meta:resourcekey="lbhdItemResource1" Text="Item"></asp:Label>
									</b></th>
								<th class="col-sm-3 text-left-more"><b>
									<asp:Label ID="lbhdConAction" runat="server" meta:resourcekey="lbhdConActionResource1" Text="<%$ Resources:LocalizedText, InitialAction %>"></asp:Label>
									</b></th>
								<th class="col-sm-2 text-left-more"><b>
									<asp:Label ID="lbhdConAssignedTo" runat="server" meta:resourcekey="lbhdConAssignedToResource1" Text="Assigned To"></asp:Label>
									</b></th>
								<th class="col-sm-2 text-left-more"><b>
									<asp:Label ID="lbhdConStartDate" runat="server" meta:resourcekey="lbhdConStartDateResource1" Text="Date Performed"></asp:Label>
									</b></th>
							</tr>
						</thead>
					</table>
				</HeaderTemplate>
				<ItemTemplate>
					<tbody>
						<tr>
							<td class="text-center">
								<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1"></asp:Label>
							</td>
							<td class="text-left-more">
								<asp:TextBox ID="tbContainAction" runat="server" Height="65px" meta:resourcekey="tbContainActionResource1" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="90%"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfvContainAction" runat="server" ControlToValidate="tbContainAction" Display="None" ErrorMessage="Required" meta:resourcekey="rfvContainActionResource1"></asp:RequiredFieldValidator>
							</td>
							<td class="text-left-more">
								<telerik:RadComboBox ID="rddlContainPerson" runat="server" CssClass="WarnIfChanged" DropDownHeight="350" ExpandDirection="Up" meta:resourcekey="rddlContainPersonResource1" Skin="Metro" Width="200px" ZIndex="9000">
								</telerik:RadComboBox>
								<asp:RequiredFieldValidator ID="rfvContainPerson" runat="server" ControlToValidate="rddlContainPerson" Display="None" EmptyMessage="[Select One]" ErrorMessage="Required" meta:resourcekey="rfvContainPersonResource1"></asp:RequiredFieldValidator>
							</td>
							<td class="text-left-more">
								<telerik:RadDatePicker ID="rdpStartDate" runat="server" CssClass="WarnIfChanged" meta:resourcekey="rdpStartDateResource1" ShowPopupOnFocus="True" Skin="Metro">
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
								<asp:RequiredFieldValidator ID="rvfStartDate" runat="server" ControlToValidate="rdpStartDate" Display="None" ErrorMessage="Required" meta:resourcekey="rvfStartDateResource1"></asp:RequiredFieldValidator>
							</td>
							<td class="text-left-more">
								<telerik:RadButton ID="btnItemDelete" runat="server" BorderStyle="None" ButtonType="LinkButton" CommandArgument="Delete" ForeColor="DarkRed" OnClientClicking="DeleteConfirmItem" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" Text="Delete Item">
								</telerik:RadButton>
							</td>
						</tr>
					</tbody>
				</ItemTemplate>
            </asp:Repeater>
        	&nbsp;
        </telerik:RadAjaxPanel>
    </div>
</asp:Panel>
