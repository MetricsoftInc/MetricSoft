<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Ucl_EHSIncidentForm.ascx.cs" Inherits="SQM.Website.Ucl_EHSIncidentForm" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_EHSIncidentDetails.ascx" TagName="IncidentDetails" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Contain.ascx" TagName="Containment" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Root5Y.ascx" TagName="RootCause" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Causation.ascx" TagName="Causation" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Action.ascx" TagName="Action" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Alert.ascx" TagName="Alert" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Approval.ascx" TagName="Approval" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AttachVideoPanel.ascx" TagName="AttachVideoPanel" TagPrefix="Ucl" %>

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

	window.onload = function () {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = "";
	}
	window.onbeforeunload = function () {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value == '1') {
			return 'You have unsaved changes on this page.';
		}
	}
	function ChangeUpdate(sender, args) {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = '1';
		return true;
	}
	function ChangeClear(sender, args) {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = '0';
	}
	function CheckChange() {
		var ret = true;
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value == '1') {
			ret = confirm('You have unsaved changes on this page. \n\n Are you sure you want to leave this page ?');
			if (ret == true) {
				document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = '0';
			}
		}
		return ret;
	}
</script>

<div id="divIncidentForm" runat="server">

	<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	<table style="width: 100%" class="textStd">
		<tr>
			<td>
				<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 0;" runat="server">
					<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" HorizontalAlign="NotSet" meta:resourcekey="RadAjaxPanel1Resource1">
						<asp:Label ID="lblResults" runat="server" meta:resourcekey="lblResultsResource1" />
						<asp:Panel ID="pnlAddEdit" runat="server" meta:resourcekey="pnlAddEditResource1">
							<div class="container-fluid blueCell" style="padding: 7px; margin-top: 5px;">
								<asp:Panel ID="pnlIncidentHeader" runat="server" meta:resourcekey="pnlIncidentHeaderResource1">
									<div class="row-fluid" >
										<div class="col-xs-12  text-left">
											<span>
											<asp:Label ID="lblAddOrEditIncident" class="prompt" runat="server" Font-Bold="true" Text="<%$ Resources:LocalizedText, AddANewIncident %>" />
											<a href="/EHS/EHS_Incidents.aspx" id="ahReturn" runat="server" style="font-size:medium; margin-left: 40px;">
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
							<div class="container" style="margin-top: 5px;">
								<div class="row text_center">
									<div class="col-xs-12 col-sm-12 text-center">
										<asp:Label ID="lblPageTitle" runat="server" Font-Bold="True" CssClass="pageTitles" meta:resourcekey="lblPageTitleResource1"></asp:Label>
									</div>
								</div>
							</div>
							<div id="divForm" runat="server" visible="False">
								<asp:Panel ID="pnlForm" runat="server" meta:resourcekey="pnlFormResource1">
								</asp:Panel>
								<div id="divSubnav" runat="server">
									<div id="divSubnavPage" runat="server" visible="False">
										<ucl:Containment id="uclcontain" runat="server" Visible="False" />
										<ucl:RootCause id="uclroot5y" runat="server" Visible="False"/>
										<ucl:Causation id="uclCausation" runat="server" Visible="False"/>
										<ucl:Action id="uclaction" runat="server" Visible="False"/>
										<ucl:Approval id="uclapproval" runat="server" Visible="False"/>
										<Ucl:Alert ID="uclAlert" runat="server" Visible="false" />
										<Ucl:AttachVideoPanel id="uclVideoPanel" runat="server" Visible="false"/>
									</div>

									<div>
										<center>
											<telerik:RadButton ID="btnSubnavSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro"
												OnClientClicked="ChangeClear" OnClick="btnSubnavSave_Click" CommandArgument="0" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
											<telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="False" ForeColor="DarkRed" Style="margin-left: 30px; margin-top: 5px;"
												Text="<%$ Resources:LocalizedText, DeleteIncident %>" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
												OnClientClicking="function(sender,args){RadConfirmAction(sender, args, 'Delete this Incident');}" OnClick="btnDelete_Click" CssClass="UseSubmitAction" />
										</center>
									</div>
									<div style="margin-top: 10px;">
										<center>
											<asp:LinkButton ID="btnSubnavIncident" runat="server" Text="<%$ Resources:LocalizedText, Incident %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="1" />

									<asp:LinkButton ID="btnSubnavVideo" runat="server"  Text="<%$ Resources:LocalizedText, VideoUpload %>"  CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClientClick="return CheckChange();" OnClick="btnSubnav_Click" CommandArgument="1.1" visible="false"/>

									<asp:LinkButton ID="btnSubnavContainment" runat="server" Text="<%$ Resources:LocalizedText, InitialAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="2"/>

									<asp:LinkButton ID="btnSubnavInitialActionApproval" runat="server" Text="<%$ Resources:LocalizedText, InitialActionApproval %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="2.5"/>

									<asp:LinkButton ID="btnSubnavRootCause" runat="server" Text="<%$ Resources:LocalizedText, RootCause %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="3"/>

									<asp:LinkButton ID="btnSubnavCausation" runat="server" Text="<%$ Resources:LocalizedText, Causation %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="4"/>

									<asp:LinkButton ID="btnSubnavAction" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="5"/>

									<asp:LinkButton ID="btnSubnavCorrectiveActionApproval" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveActionApproval %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="5.5"/>

									<asp:LinkButton ID="btnSubnavApproval" runat="server" Text="<%$ Resources:LocalizedText, Approvals %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
												OnClick="btnSubnav_Click" CommandArgument="10"/>

									<asp:LinkButton ID="btnSubnavAlert" runat="server" Text="<%$ Resources:LocalizedText, PreventativeMeasure %>"  CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="11" visible="false"/>
										</center>
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