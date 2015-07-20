<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Ucl_EHSIncidentForm.ascx.cs" Inherits="SQM.Website.Ucl_EHSIncidentForm" %>
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

<div id="divIncidentForm" runat="server" visible="false">
	<div style="width: 100%; text-align: center; margin-bottom: 10px;"><a href="EHS_Incidents.aspx" id="ahReturn" runat="server">
		<img src="/images/defaulticon/16x16/arrow-7-up.png" style="vertical-align: middle; border: 0;" border="0" alt="" />
		Return to List</a></div>
	<table style="width: 100%" class="textStd">
		<tr>
			<td>
				<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 0;" runat="server">
					<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
						<asp:Label ID="lblResults" runat="server" />
					<ucl:IncidentDetails id="uclIncidentDetails" runat="server" />
					<asp:Panel ID="pnlAddEdit" runat="server">

						<div class="container-fluid blueCell" style="padding: 7px;"">

							<asp:Panel ID="pnlIncidentHeader" runat="server">

								<div class="row-fluid" >

									<div class="col-xs-12  text-left">


										<asp:Label ID="lblAddOrEditIncident" class="textStd" runat="server"><strong>Add a New Incident:</strong></asp:Label>

										<span  class="hidden-xs"  style="float:right; width: 160px; margin-right:6px;">
											<span class="requiredStar">&bull;</span> - Required to Create
										</span>

										<div class="clearfix visible-xs-block"></div>
										<br style="clear:both;"/>


											<asp:Label ID="lblIncidentType" class="textStd" runat="server">Incident Type:</asp:Label>
											<telerik:RadDropDownList ID="rddlIncidentType" runat="server" Width="268" AutoPostBack="true" CausesValidation="false"
												OnSelectedIndexChanged="rddlIncidentType_SelectedIndexChanged" Skin="Metro">
											</telerik:RadDropDownList>
	

										<span class="hidden-xs" style="float:right; width: 160px;">
											<span class="requiredCloseStar">&bull;</span> - Required to Close
										</span>
										
										<div class="clearfix visible-xs-block"></div>

									</div>
								</div>

							</asp:Panel>
						</div>

							<asp:Panel ID="pnlForm" runat="server">
							</asp:Panel>
						
							<table style="width: 100%;">
								<tr>
									<td style="width: 33%;">
										<telerik:RadButton ID="btnSaveReturn" runat="server" Text="Save &amp; Return" Visible="false"
											CssClass="UseSubmitAction" Width="88%" Skin="Metro" SingleClick="true" SingleClickText="Saving..."
											OnClick="btnSaveReturn_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />
									</td>
									<td style="width: 33%;">
										<telerik:RadButton ID="btnSaveContinue" runat="server" Text="Save &amp; Create Report" Visible="false"
											 Icon-SecondaryIconUrl="/images/ico-arr-rt-wht.png" SingleClick="true" SingleClickText="Saving..."
											CssClass="UseSubmitAction metroIconButtonSecondary" Width="88%" Skin="Metro"
											OnClick="btnSaveContinue_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />
									</td>
									<td style="width: 33%; text-align: center;">
										<telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
											Text="Delete Incident" SingleClick="true" SingleClickText="Deleting..."
											OnClick="btnDelete_Click" OnClientClicking="DeleteConfirm" CssClass="UseSubmitAction" />
									</td>
								</tr>
							</table>

						
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