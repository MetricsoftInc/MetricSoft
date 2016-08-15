<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Causation.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Causation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

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

 <asp:Panel ID="pnlCausation" Visible="False" runat="server">

	 <asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	 <div id="divTitle" runat="server" visible="false" class="container" style="margin: 5px 0 5px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>

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
					<div class="row" style="margin-top: 7px;">
						<div id="divPrompt" runat="server" class="col-sm-3 text-left">
							<asp:Image id="imgProblem" runat="server" ImageUrl="~/images/defaulticon/16x16/blank.png" style="vertical-align: middle; border: 0px; margin-right: 4px;"/>
							<span><asp:Label ID="lbWhyPrompt" Text="Why " runat="server" CssClass="prompt" meta:resourcekey="lbWhyPromptResource1"></asp:Label><asp:Label ID="lbItemSeq" runat="server" CssClass="prompt"></asp:Label>:</span>
				
						</div>
						<div id="divRootCause" runat="server" class="col-sm-6 text-left">
							<asp:Label ID="lblRootCause" runat="server" CssClass="textStd"></asp:Label>
						</div>
						<div class="col-sm-2 text-left">
							<span>
								<asp:Image ID="imgIsRootCause" runat="server" ImageUrl="~/images/defaulticon/16x16/check.png" style="border: 0px;" Visible="false" AlternateText="Root cause"/>
								<asp:Label ID="lblIsRootCause" runat="server" CssClass="labelEmphasis"></asp:Label>
							</span>
						</div>
					</div>
				</ItemTemplate>
			</asp:Repeater>
		</div>
		<br />
		<div id="divCausation" runat="server" class="row" style="margin-left: 0px; margin-bottom: 10px;">
			<div class="col-sm-3 hidden-xs text-left columnHeader borderSoft" >
				<span>
					<asp:Label ID="lblCausation" runat="server" Text="Operational Root Cause" CssClass="prompt" ></asp:Label>
					<br />
					<asp:Label ID="lblCausationInstruct" runat="server" Text="Select the ultimate root cause of the incident based on the 'Why' determinations above." CssClass="textStd"></asp:Label>
				</span>
			</div>
			<div class="col-xs-12 col-sm-7 text-left" style="height: 60px;">
				<telerik:RadComboBox ID="ddlCausation" runat="server" Skin="Metro" ZIndex="9000" Height="300px" Width="85%" Font-Size="Small" style="margin-top: 18px;"
					ToolTip="Select the ultimate root cause of this incident" EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnClientSelectedIndexChanged="ChangeUpdate"></telerik:RadComboBox>
			</div>
		</div>
		<div id="divTeam" runat="server" visible="false" class="row" style="margin-left: 0px; margin-bottom: 10px;">
			<div class="col-sm-3 hidden-xs text-left columnHeader borderSoft" >
				<span>
					<asp:Label ID="lblTeam" runat="server" Text="Contributing Team Members" CssClass="prompt" ></asp:Label>
					<br />
					<asp:Label ID="lblTeamInstruct" runat="server" Text="Identify the team members who participated in this root-cause analysis." CssClass="textStd"></asp:Label>
				</span>
			</div>
			<div class="col-xs-12 col-sm-7 text-left" style="height: 60px;">
				<asp:TextBox ID="tbTeam" Rows="2" Width="85%" TextMode="MultiLine" SkinID="Metro" runat="server" onChange="ChangeUpdate()"></asp:TextBox>
			</div>
		</div>
		<center>
			<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
				OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
		</center>
	</div>

</asp:Panel>