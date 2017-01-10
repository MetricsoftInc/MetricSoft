<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Approval.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Approval" %>
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

<asp:Panel ID="pnlApproval" Visible="False" runat="server" meta:resourcekey="pnlApprovalResource1">
	<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	<div id="divTitle" runat="server" visible="false" class="container" style="margin: 10px 0 10px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>

	<div class="container-fluid">

		<div id="divStatus" runat="server" visible="false" style="margin: 10px;">
			<asp:Label ID="lblStatusMsg" runat="server" CssClass="labelEmphasis"></asp:Label>
		</div>
		<telerik:RadAjaxPanel ID="rapApprovals" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapApprovalsResource1">

		<asp:Repeater runat="server" ID="rptApprovals" ClientIDMode="AutoID" OnItemDataBound="rptApprovals_OnItemDataBound" OnItemCommand="rptApprovals_ItemCommand">
			<FooterTemplate>
			</FooterTemplate>
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 col-sm-2 text-left">
						<asp:HiddenField ID="hfApprovalID" runat="server" />
						<asp:HiddenField ID="hfItemSeq" runat="server" />
						<asp:HiddenField ID="hfPersonID" runat="server" />
						<asp:HiddenField ID="hfReqdComplete" runat="server" />
						<asp:HiddenField ID="hfRoleDesc" runat="server" />
						<span>
							<asp:PlaceHolder ID="phOnBehalfOf" runat="server" Visible="false">
								<asp:Label runat="server" ID="lblOnBehalfOf" CssClass="refText" Text="<%$ Resources:LocalizedText, OnBehalfOf %>"></asp:Label>
								<br />
							</asp:PlaceHolder>
							<b>
							<asp:Label ID="lbApproverJob" runat="server" meta:resourcekey="lbApproverJobResource1" SkinID="Metro"></asp:Label>
							<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1"></asp:Label>
							</b>
							<br />
							<asp:Label ID="lbApprover" runat="server" meta:resourcekey="lbApproverResource1" SkinID="Metro" Width="75%"></asp:Label>
						</span>
					</div>
					<div class="col-xs-12 col-sm-3  text-left">
						<asp:Label ID="lbApproveMessage" runat="server" Height="95px" meta:resourcekey="lbApproveMessageResource1" SkinID="Metro" Width="95%"></asp:Label>
					</div>
					<div class="col-xs-12  col-sm-1 text-left">
						<span>
						<asp:CheckBox ID="cbIsAccepted" runat="server" Font-Bold="False" meta:resourcekey="cbIsAcceptedResource1" SkinID="Metro" onChange="return ChangeUpdate();" />
						</span>
					</div>
					<div class="col-xs-12 col-sm-2 text-left">
						<span>Date&nbsp;
						<telerik:RadDatePicker ID="rdpAcceptDate" runat="server" CssClass="WarnIfChanged" Enabled="False" meta:resourcekey="rdpAcceptDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="120px">
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
						</span>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate>
				<br />
			</SeparatorTemplate>
		</asp:Repeater>

		</telerik:RadAjaxPanel>
		<center>
			<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
				OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" Visibl="false" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
		</center>
	</div>
</asp:Panel>



