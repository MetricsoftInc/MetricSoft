<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Contain.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Contain" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<script type="text/javascript">

	window.onload = function () {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "";
	}
	window.onbeforeunload = function () {
		if (document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value == '1') {
			return 'You have unsaved changes on this page.';
		}
	}
	function ChangeUpdate(sender, args) {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "1";
		return true;
	}
	function ChangeClear(sender, args) {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "0";
	}

</script>


<asp:Panel ID="pnlContain" Visible="False" runat="server" meta:resourcekey="pnlContainResource1">

	<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	<div id="divTitle" runat="server" visible="false" class="container" style="margin: 5px 0 5px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>

    <div class="container-fluid">
        <telerik:RadAjaxPanel ID="rapContain"  runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapContainResource1">
            <asp:Repeater runat="server" ID="rptContain" ClientIDMode="AutoID" OnItemDataBound="rptContain_OnItemDataBound" OnItemCommand="rptContain_ItemCommand">
            	<HeaderTemplate>
					<table width="99%" border="0"  class="lightTable">
						<thead>
						</thead>
						</HeaderTemplate>
						<ItemTemplate>
						<tbody>
							<tr>
								<td class="columnHeader" width="20%">
									<asp:Label ID="lbhdConAction" runat="server" meta:resourcekey="lbhdConActionResource1" Text="<%$ Resources:LocalizedText, InitialAction %>"></asp:Label>
									&nbsp;
									<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1"></asp:Label>
								</td>
								<td class="required" width="1%">&nbsp;</td>
								<td class="tableDataAlt" width="79%">
									<asp:TextBox ID="tbContainAction" runat="server" Height="65px" meta:resourcekey="tbContainActionResource1" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="98%"  onChange="ChangeUpdate()"></asp:TextBox>
									<asp:RequiredFieldValidator ID="rfvContainAction" runat="server" ControlToValidate="tbContainAction" Display="None" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr>
								<td class="columnHeader">
									<asp:Label ID="lbhdConAssignedTo" runat="server" Text="<%$ Resources:LocalizedText, AssignedTo %>"></asp:Label>
								</td>
								<td class="tableDataAlt">&nbsp;</td>
								<td class="tableDataAlt">
									<telerik:RadComboBox ID="rddlContainPerson" runat="server" DropDownHeight="350" ExpandDirection="Up" meta:resourcekey="rddlContainPersonResource1" Skin="Metro" Width="350px" ZIndex="9000" OnClientSelectedIndexChanged="ChangeUpdate">
									</telerik:RadComboBox>
									<asp:RequiredFieldValidator ID="rfvContainPerson" runat="server" ControlToValidate="rddlContainPerson" Display="None" EmptyMessage="[Select One]" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr>
								<td class="columnHeader">
									<asp:Label ID="lbhdConStartDate" runat="server" meta:resourcekey="lbhdConStartDateResource1" Text="Date Performed"></asp:Label>
								</td>
								<td class="required">&nbsp;</td>
								<td class="tableDataAlt">
									<telerik:RadDatePicker ID="rdpStartDate" runat="server" meta:resourcekey="rdpStartDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="125">
										<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
										</Calendar>
										<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="" OnClientDateChanged="ChangeUpdate">
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
									<asp:RequiredFieldValidator ID="rvfStartDate" runat="server" ControlToValidate="rdpStartDate" Display="None" ErrorMessage="<%$ Resources:LocalizedText, Required %>"></asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr id="trComments" runat="server">
								<td class="columnHeader">
									<asp:Label ID="lblComments" runat="server" Text="<%$ Resources:LocalizedText, Comments %>"></asp:Label>
								</td>
								<td class="tableDataAlt">&nbsp;</td>
								<td class="tableDataAlt">
									<asp:TextBox ID="tbComments" runat="server" Height="65px" Rows="3" SkinID="Metro" TextMode="MultiLine" Width="98%" onChange="ChangeUpdate()"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td class="text-left-more" colspan="3">
									<telerik:RadButton ID="btnItemDelete" runat="server" BorderStyle="None" ButtonType="LinkButton" CommandArgument="Delete" ForeColor="DarkRed" OnClientClicking="DeleteConfirmItem" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" Text="<%$ Resources:LocalizedText, DeleteItem %>">
									</telerik:RadButton>
								</td>
							</tr>
							<tr><td colspan="3" style="height: 10px;"></td></tr>
						</tbody>
					</ItemTemplate>
					<FooterTemplate>
                </table>
                </FooterTemplate>
            </asp:Repeater>
			<div class="row">
				<center>
					<span>
						<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
							OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
						<asp:Button ID="btnAddContain" CssClass="buttonAdd" runat="server" OnClick="AddDelete_Click" ToolTip="Add Another Initial Corrective Action" Text="<%$ Resources:LocalizedText, AddAnother %>" Style="margin-left: 15px;" CommandArgument="AddAnother" meta:resourcekey="btnAddContainResource1"></asp:Button>
					</span>
				</center>
				<asp:Label ID="lblStatusMsg" runat="server" CssClass="labelEmphasis"></asp:Label>
			</div>
        </telerik:RadAjaxPanel>
    </div>
</asp:Panel>

