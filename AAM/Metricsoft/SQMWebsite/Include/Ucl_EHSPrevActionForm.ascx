<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Ucl_EHSPrevActionForm.ascx.cs" Inherits="SQM.Website.Ucl_EHSPrevActionForm" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_EHSIncidentDetails.ascx" TagName="IncidentDetails" TagPrefix="Ucl" %>

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
</script>

<div id="divIncidentForm" runat="server">
	<table style="width: 100%" class="textStd">
		<tr>
			<td>
				<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 0;" runat="server">
					<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" HorizontalAlign="NotSet" meta:resourcekey="RadAjaxPanel1Resource1">
						<asp:Label ID="lblResults" runat="server" meta:resourcekey="lblResultsResource1"/>
						<asp:Panel ID="pnlAddEdit" runat="server" meta:resourcekey="pnlAddEditResource1">
							<div class="container-fluid blueCell" style="padding: 7px; margin-top: 5px; margin-bottom: 5px;">
								<asp:Panel ID="pnlIncidentHeader" runat="server" meta:resourcekey="pnlIncidentHeaderResource1">
									<div class="row-fluid" >
										<div class="col-xs-12  text-left">
											<span>
											<asp:Label ID="lblAddOrEditIncident" class="prompt" runat="server" Font-Bold="True" Text="<%$ Resources:LocalizedText, AddANewIncident %>" meta:resourcekey="lblAddOrEditIncidentResource1" />
											<a href="/EHS/EHS_PrevActions.aspx" id="ahReturn" runat="server" style="font-size:medium; margin-left: 40px;">
												<img src="/images/defaulticon/16x16/arrow-7-up.png" style="vertical-align: middle; border: 0;" border="0" alt="" />
												Return to List</a>
											</span>
											<span class="hidden-xs"  style="float:right; width: 160px; margin-right:6px;">
											<span class="requiredStar">&bull;</span> - Required to Create</span>
											<div style="clear:both;"></div>
												<span class="hidden-xs" style="float:right; width: 160px; margin-right:6px;">
												<span class="requiredCloseStar">&bull;</span> - Required to Close</span>
										</div>
									</div>
									<br class="clearfix" style="clear:both;"/>
									<div class="row-fluid" style="margin-top:-80px;" >
										<div class="col-xs-12 text-left">
											<asp:Label runat="server" ID="lblIncidentLocation" class="textStd" meta:resourcekey="lblIncidentLocationResource1"></asp:Label>
											<br />
											<asp:Label ID="lblIncidentType" class="textStd"  runat="server" meta:resourcekey="lblIncidentTypeResource1">Type:  </asp:Label>
										</div>
									</div>
								</asp:Panel>
							</div>

							<div id="divDetails" runat="server" visible="false">
								<ucl:IncidentDetails id="uclIncidentDetails" runat="server" />
							</div>

							<div class="container" style="margin-top: 0px;">
								<div class="row text_center">
									<div class="col-xs-12 col-sm-12 text-center">
										<asp:Label ID="lblPageTitle" runat="server" Font-Bold="True" CssClass="pageTitles" Text="<%$ Resources:LocalizedText, Recommendation %>"></asp:Label>
									</div>
								</div>
							</div>

							<div id="divForm" runat="server" visible="False">
								<asp:Panel ID="pnlForm" runat="server" meta:resourcekey="pnlFormResource1">
								</asp:Panel>
								<table style="width: 100%;">
									<tr>
										<td style="width: 33%;">
											<telerik:RadButton ID="btnSaveReturn" runat="server" Text="<%$ Resources:LocalizedText, SaveAndReturn %>" Visible="False"
												CssClass="UseSubmitAction" Width="88%" Skin="Metro" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Saving %>"
												OnClick="btnSaveReturn_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" CommandArgument ="0" meta:resourcekey="btnSaveReturnResource1" />
										</td>
										<td style="width: 33%;">
											<telerik:RadButton ID="btnSaveContinue" runat="server" Text="<%$ Resources:LocalizedText, SaveAndCreateReport %>" Visible="False" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Saving %>"
												CssClass="UseSubmitAction metroIconButtonSecondary" Width="88%" Skin="Metro"
												OnClick="btnSaveContinue_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" meta:resourcekey="btnSaveContinueResource1">
												<Icon SecondaryIconUrl="/images/ico-arr-rt-wht.png" />
											</telerik:RadButton>
										</td>
									</tr>
								</table>
								<div id="divSubnav" runat="server">
									<div id="divSubnavPage" runat="server" class="borderSoft" visible="False">
									</div>

									<div style="margin-top: 5px;">
										<telerik:RadButton ID="btnSubnavSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" Style="margin-right: 10px;"
											OnClick="btnSubnavSave_Click" CommandArgument="0" meta:resourcekey="btnSubnavSaveResource1"/>

										<asp:LinkButton ID="btnSubnavIncident" runat="server" Text="<%$ Resources:LocalizedText, Recommendation %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
											OnClick="btnSubnav_Click" CommandArgument="0" meta:resourcekey="btnSubnavIncidentResource1" />
										
										<asp:LinkButton ID="btnSubnavAction" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
											OnClick="btnSubnav_Click" CommandArgument="4" meta:resourcekey="btnSubnavActionResource1"/>
										
										<asp:LinkButton ID="btnSubnavApproval" runat="server" Text="<%$ Resources:LocalizedText, Resolution %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
											OnClick="btnSubnav_Click" CommandArgument="5" meta:resourcekey="btnSubnavApprovalResource1"/>

										<span style="float:right">
											<telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="False" ForeColor="DarkRed"
												Text="<%$ Resources:LocalizedText, DeleteIncident %>" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
												OnClientClicking="function(sender,args){RadConfirmAction(sender, args, 'Delete this Incident');}" OnClick="btnDelete_Click" CssClass="UseSubmitAction" meta:resourcekey="btnDeleteResource1" />
										</span>
									</div>
								</div>
							</div>
						</asp:Panel>

					</telerik:RadAjaxPanel>
					<br />
					<br />
				</div>
			</td>
		</tr>
	</table>
</div>

<div id="divIncidentReportForm" runat="server" visible="false">

</div>