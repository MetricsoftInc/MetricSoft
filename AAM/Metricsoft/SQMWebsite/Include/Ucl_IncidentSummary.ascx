<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_IncidentSummary.ascx.cs" Inherits="SQM.Website.Ucl_IncidentSummary" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel ID="pnlIncidentSummary" runat="server" Visible = "False">
	<div class="container-fluid" style="margin-top: 10px;">
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="lnkDetail" runat="server" Text="Task" CssClass="prompt" CommandArgument="100" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblDetail" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row" id="divWorkStatus" runat="server">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="LnkWorkStatus" runat="server" Text="Work Status" CssClass="prompt" CommandArgument="105" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="LblWorkStatus" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="lnkContainment" runat="server" Text="Initial Action" CssClass="prompt" CommandArgument="110" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblContainment" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="lnkRootCause" runat="server" Text="Root Cause" CssClass="prompt" CommandArgument="120" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblRootCause" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="lnkCauseation" runat="server" Text="Causeation" CssClass="prompt" CommandArgument="125" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblCauseation" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="lnkAction" runat="server" Text="Corrective Action" CssClass="prompt" CommandArgument="130" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblAction" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<asp:LinkButton ID="lnkSignoff" runat="server" Text="Approvals" CssClass="prompt" CommandArgument="151" OnClick="lnkTopic_Click"></asp:LinkButton>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:Label ID="lblSignoff" runat="server" CssClass="textStd"></asp:Label>
			</div>
		</div>

		<br />
		<div style="float: right; margin: 5px;">
			<span>
				<asp:Button ID="btnCancel" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" CommandArgument="0" OnClick="lnkTopic_Click"></asp:Button>
			</span>
        	</div>
	</div>
</asp:Panel>