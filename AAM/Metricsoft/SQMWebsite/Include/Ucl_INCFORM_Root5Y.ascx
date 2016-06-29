<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Root5Y.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Root5Y" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<script type="text/javascript">
	window.onload = function () {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclroot5y_hfChangeUpdate').value = "";
	}
	window.onbeforeunload = function () {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclroot5y_hfChangeUpdate').value == '1') {
			return 'You have unsaved changes on this page.';
		}
	}
	function ChangeUpdate() {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclroot5y_hfChangeUpdate').value = '1';
		return true;
	}
	function ChangeClear(sender, args) {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclroot5y_hfChangeUpdate').value = '0';
	}

</script>


 <asp:Panel ID="pnlRoot5Y" runat="server">
	 <asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	 <div id="divTitle" runat="server" visible="false" class="container" style="margin: 5px 0 5px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>


	<div class="container-fluid">

		<telerik:RadAjaxPanel ID="rapRoot5Y" runat="server" HorizontalAlign="NotSet">

			<asp:Repeater runat="server" ID="rptRootCause" ClientIDMode="AutoID" OnItemDataBound="rptRootCause_OnItemDataBound" OnItemCommand="rptRootCause_ItemCommand">
				<FooterTemplate>
					<div class="row">
						<div class="col-xs-12 text-left-more">
							<br />
						</div>
					</div>
					<div class="row">
						<center>
							<span>
								<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
									OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
								<asp:Button ID="btnAddStatement" runat="server" CommandArgument="AddStatement" OnClientClick="ChangeUpdate()" CssClass="buttonAdd" Text="<%$ Resources:LocalizedText, AddProblemSeries %>" Style="margin-left: 15px;" ToolTip="New problem statement" />
							</span>
						</center>
					</div>
				</FooterTemplate>
				<HeaderTemplate>
					<div class="row" style="margin-top: 1px;">
						<div class="col-sm-10 text-left">
							<asp:Label ID="lblProblemDesc" runat="server"  CssClass="refText"></asp:Label>
						</div>
					</div>
				</HeaderTemplate>
				<ItemTemplate>
					<div class="row" style="margin-top: 17px;">
						<asp:HiddenField id="hfItemType" runat="server"/>
						<asp:HiddenField ID ="hfProblemSeries" runat="server" />
						<asp:HiddenField id="hfItemSeq" runat="server"/>
						<div class="col-sm-3 text-left" id="divPrompt" runat="server" style="height: 60px;">
							<span>
								<asp:Image id="imgProblem" runat="server" Visible="true" ImageUrl="~/images/defaulticon/16x16/blank.png" style="vertical-align: middle; border: 0px; margin-right: 4px;"/>
								<asp:Label ID="lbWhyPrompt" Text="Why " runat="server" CssClass="prompt" meta:resourcekey="lbWhyPromptResource1"></asp:Label><asp:Label ID="lbItemSeq" runat="server" CssClass="prompt"></asp:Label>:
							</span>
							<br />
							<span>
								<asp:Button id="btnAddRootCause" runat="server" CssClass="buttonAdd" Text="<%$ Resources:LocalizedText, AddCause %>" ToolTip="Add root cause" CommandArgument="AddAnother" Style="margin-top: 4px; margin-left: 20px;"/>
							</span>
						</div>
						<div id="divRootCause" runat="server" class="col-sm-6 text-left">
							<asp:TextBox ID="tbRootCause" Rows="3" Height="60px" Width="98%" TextMode="MultiLine" SkinID="Metro" runat="server" onChange="ChangeUpdate()"></asp:TextBox>
							<asp:Label ID="lblProblemStatement" runat="server"  CssClass="refText"></asp:Label>
						</div>
						<div class="col-sm-2 text-left-more">
							<asp:Panel ID="pnlIsRootCause" runat="server">
								<span>
									<asp:Label ID="lblIsRootCause" runat="server" Text="Is Root Cause" CssClass="prompt"></asp:Label>
									&nbsp;
									<asp:CheckBox id="cbIsRootCause" runat="server" onChange="return ChangeUpdate();"/>
								</span>
								<br />
							</asp:Panel>
							<telerik:RadButton ID="btnItemDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" ForeColor="DarkRed"  CommandArgument="Delete" Style="margin-top: 3px;"
								Text="<%$ Resources:LocalizedText, DeleteItem %>" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>" OnClientClicking="DeleteConfirmItem" />
						</div>
					</div>
				</ItemTemplate>
			</asp:Repeater>

		</telerik:RadAjaxPanel>

	</div>

</asp:Panel>

