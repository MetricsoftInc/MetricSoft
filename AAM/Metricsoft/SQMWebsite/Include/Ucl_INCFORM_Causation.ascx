<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Causation.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Causation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

 <asp:Panel ID="pnlCausation" Visible="False" runat="server">

	<div class="container-fluid" style="margin-top: 10px;">
		<asp:Label ID="lblIncidentDesc" runat="server"  CssClass="refText"></asp:Label>
		<br /><br />
		<asp:Label ID="lblCausationTitle" runat="server" Text="Root Cause Determinations ('why' did this incident occur ?)" CssClass="prompt"></asp:Label>
		<br />
		<div id="divRootCause" runat="server" style="margin-top: 5px;">
			<asp:Label ID="lblNoneRootCause" runat="server" CssClass="labelEmphasis" Text="Root Cause determinations have not yet been defined."></asp:Label>
			<asp:Repeater runat="server" ID="rptRootCause" ClientIDMode="AutoID" OnItemDataBound="rptRootCause_OnItemDataBound">
				<FooterTemplate>
				</FooterTemplate>
				<HeaderTemplate></HeaderTemplate>
				<ItemTemplate>
					<div class="row">
						<div class="col-sm-1 text-left">
							<span><asp:Label ID="lbWhyPrompt" Text="Why " runat="server" meta:resourcekey="lbWhyPromptResource1"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:</span>
						</div>
						<div class="col-sm-9 text-left">
							<asp:Label ID="lblRootCause" runat="server" CssClass="prompt"></asp:Label>
						</div>
					</div>
				</ItemTemplate>
				<SeparatorTemplate><br /></SeparatorTemplate>
			</asp:Repeater>
		</div>
		<br />
		<div id="divCausation" runat="server" class="row" style="margin-left: 3px; margin-bottom: 20px;">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 60px;">
				<span>
					<asp:Label ID="lblCausation" runat="server" Text="Operational Root Cause" CssClass="prompt" ></asp:Label>
					<br />
					<asp:Label ID="lblCausationInstruct" runat="server" Text="Select the ultimate root cause of the incident based on the 'Why' determinations above." CssClass="textStd"></asp:Label>
				</span>
			</div>
			<div class="col-xs-12 col-sm-6 text-left greyControlCol" style="height: 60px;">
				<telerik:RadComboBox ID="ddlCausation" runat="server" Skin="Metro" ZIndex="9000" Height="300px" Width="300px" Font-Size="Small"
					ToolTip="Select the ultimate root cause of this incident" EmptyMessage="<%$ Resources:LocalizedText, Select %>"></telerik:RadComboBox>
			</div>
		</div>
	</div>

</asp:Panel>