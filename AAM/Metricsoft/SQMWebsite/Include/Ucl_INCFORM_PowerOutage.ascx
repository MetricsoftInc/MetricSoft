<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_PowerOutage.ascx.cs"
	Inherits="SQM.Website.Ucl_INCFORM_PowerOutage" %>
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

</script>

<%--<asp:Panel ID="pnlCase" Visible="true" runat="server">
<table width="100%" cellpadding="5" cellspacing="0" style="border-collapse: collapse;">
	<tr>
		<td class="tanCell" style="width: 30%;">
			Case:
		</td>
		<td class="greyCell">
			<telerik:RadComboBox ID="rcbCases" runat="server" Skin="Metro" OnSelectedIndexChanged="rcbCases_SelectedIndexChanged" 
				AutoPostBack="true" Width="250" DropDownAutoWidth="Enabled">
			</telerik:RadComboBox>
		</td>
	</tr>
</table>
<br />
</asp:Panel>--%>

<asp:Label ID="lblRequired" runat="server" Text="Required Fields Must be Completed." ForeColor="#cc0000" Font-Bold="true" Height="25" Visible="false"></asp:Label>
<asp:Label ID="lblSubmitted" runat="server" Text="Power Outage submitted." Font-Bold="true" Visible="false"></asp:Label>


<div class="container-fluid">
	<div class="row">
		<div class="col-xs-12 col-sm-4" style="padding: 3px 1px">
			<asp:Label ID="lblFormStepNumber" runat="server" Font-Bold="true" CssClass="textStd"></asp:Label>
		</div>
		<div class="col-xs-12 col-sm-8">
			<asp:Label ID="lblFormTitle" runat="server" Font-Bold="true" CssClass="pageTitles"></asp:Label>
		</div>
	</div>
</div>


 <asp:Panel ID="pnlBaseForm" Visible="true" runat="server">

	<br />

	<div class="container-fluid">

		<%-- INCIDENT DATE question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbIncidentDateSM" runat ="server" Text="<%$ Resources:lbIncidentDateSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbIncidentDateXS" runat ="server" Text="<%$ Resources:lbIncidentDateXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpIncidentDate" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDatePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentDate" ControlToValidate="rdpIncidentDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- REPORT DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbReportDateSM" runat ="server" Text="<%$ Resources:lbReportDateSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbReportDateXS" runat ="server" Text="<%$ Resources:lbReportDateXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpReportDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="278" runat="server"></telerik:RadDatePicker>
			</div>
		</div>


		<%-- LOCATION question --%>	

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbLocationSM" runat ="server"  Text="<%$ Resources:lbLocationSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbLocationXS" runat ="server" Text="<%$ Resources:lbLocationXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlLocation" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" OnSelectedIndexChanged="rddlLocation_SelectedIndexChanged"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvLocation" ControlToValidate="rddlLocation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>
		

		
		<%-- DESCRIPTION question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="lbDescriptionSM" runat ="server" Text="<%$ Resources:lbDescriptionSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbDescriptionXS" runat ="server" Text="<%$ Resources:lbDescriptionXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvDescription" ControlToValidate="tbDescription" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- LOCAL DESCRIPTION question (show if local language is not English)  (MultiLine TEXTBOX) --%>
		<asp:Panel ID="pnlLocalDesc" runat="server" Visible="false">
			<div class="row" >
				<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
					<span class="labelMultiLineText"><asp:Label ID="lbLocalDescSM" runat ="server" Text="<%$ Resources:lbLocalDescSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span><asp:Label ID="lbLocalDescXS" runat ="server" Text="<%$ Resources:lbLocalDescXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbLocalDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator runat="server" ID="rfvLocalDescription" ControlToValidate="tbLocalDescription" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
				</div>
			</div>
		</asp:Panel>



		<%-- TIME OF INCIDENT question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbIncidentTimeSM" runat ="server" Text="<%$ Resources:lbIncidentTimeSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbIncidentTimeXS" runat ="server" Text="<%$ Resources:lbIncidentTimeXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadTimePicker ID="rtpIncidentTime" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadTimePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentTime" ControlToValidate="rtpIncidentTime" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- SHIFT question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbShiftSM" runat ="server" Text="<%$ Resources:lbShiftSMResource1.Text %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbShiftXS" runat ="server" Text="<%$ Resources:lbShiftXSResource1.Text %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlShift" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvShift" ControlToValidate="rddlShift" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- PRODUCTION IMPACT question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="lbProdImpactSM" runat ="server" Text="<%$ Resources:lbProdImpactSMResource1.Text %>"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbProdImpactXS" runat ="server" Text="<%$ Resources:lbProdImpactXSResource1.Text %>"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbProdImpact" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
			</div>
		</div>
	
	</div>

	 	<br /><br />

 </asp:Panel>


<asp:Panel ID="pnlContain" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<asp:Repeater runat="server" ID="rptContain" ClientIDMode="AutoID" OnItemDataBound="rptContain_OnItemDataBound" OnItemCommand="rptContain_ItemCommand">

			<HeaderTemplate>
					
				<div class="row">

					<div class="col-sm-2 text-center">
						<span><b><asp:Label ID="lbConAction" runat ="server" Text="Action"></asp:Label></b></span>
					</div>

					<div class="col-sm-3  text-center">
						<span><b><asp:Label ID="lbConAssignedTo" runat ="server" Text="Assigned To"></asp:Label></b></span>
					</div>

					<div class="col-sm-1 text-left-more">
						<span><b><asp:Label ID="lbConStartDate" runat ="server" Text="Start Date"></asp:Label></b></span>
					</div>

					<div class="col-sm-2 text-center">
						<span style="padding-left:10px;"><b><asp:Label ID="lbConCompletionDate" runat ="server" Text="  Completion Date"></asp:Label></b></span>
					</div>

				</div>
				<br />
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row-fluid">
	
					<div class="col-xs-12  col-sm-3 text-left-more">
						<span><span style="display:inline-block; vertical-align:top;"><asp:Label ID="lbConPrompt" Text="Action " runat="server"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:&nbsp;</span>
						<asp:TextBox ID="tbContainAction" Rows="3" Height="65px" Width="300" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox></span>
						<asp:RequiredFieldValidator runat="server" ID="rfvContainAction" ControlToValidate="tbContainAction" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDropDownList ID="rddlContainPerson" Skin="Metro" CssClass="WarnIfChanged" Width="198" runat="server" OnSelectedIndexChanged="rddlContainPerson_SelectedIndexChanged"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvContainPerson" ControlToValidate="rddlContainPerson" Display="None" InitialValue="[Select One]"  ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more"> 
						<telerik:RadDatePicker ID="rdpStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
						<asp:RequiredFieldValidator runat="server" ID="rvfStartDate" ControlToValidate="rdpStartDate" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDatePicker ID="rdpCompleteDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
					</div>


					<div class="col-xs-12  col-sm-1 text-left-more">
						<asp:CheckBox ID="cbIsComplete" runat="server" Text="Complete" SkinID="Metro" TextAlign="Right"></asp:CheckBox>
					</div>
				</div>
				<br style="float:left; clear:both;"/><br />
			</ItemTemplate>
			<SeparatorTemplate>
					<br /><br />
			</SeparatorTemplate>
			<FooterTemplate>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddContain" CssClass="buttonAdd" runat="server" ToolTip="Add Another Initial Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</div>   

</asp:Panel>

<asp:Panel ID="pnlRoot5Y" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<asp:Repeater runat="server" ID="rptRootCause" ClientIDMode="AutoID" OnItemDataBound="rptRootCause_OnItemDataBound" OnItemCommand="rptRootCause_ItemCommand">
			<HeaderTemplate></HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 text-left">
						<span><asp:Label ID="lbWhyPrompt" Text="Why " runat="server"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:</span>
					</div>
					<div class="col-xs-12 text-left">
						<asp:TextBox ID="tbRootCause" Rows="5" Height="95px" Width="50%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="rfvRootCause" ControlToValidate="tbRootCause" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate><br /><br /></SeparatorTemplate>
			<FooterTemplate>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddRootCause" CssClass="buttonAdd" runat="server" ToolTip="Add Another Root Cause" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</div>

</asp:Panel>



<asp:Panel ID="pnlAction" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<asp:Repeater runat="server" ID="rptAction" ClientIDMode="AutoID" OnItemDataBound="rptAction_OnItemDataBound" OnItemCommand="rptAction_ItemCommand">

			<HeaderTemplate>
					
				<div class="row">

					<div class="col-sm-2 text-center">
						<span><b><asp:Label ID="lbFinAction" runat ="server" Text="Action"></asp:Label></b></span>
					</div>

					<div class="col-sm-3  text-center">
						<span><b><asp:Label ID="lbFinAssignedTo" runat ="server" Text="Assigned To"></asp:Label></b></span>
					</div>

					<div class="col-sm-1 text-left-more">
						<span><b><asp:Label ID="lbFinStartDate" runat ="server" Text="Start Date"></asp:Label></b></span>
					</div>

					<div class="col-sm-2 text-center">
						<span style="padding-left:10px;"><b><asp:Label ID="lbFinCompletionDate" runat ="server" Text="  Completion Date"></asp:Label></b></span>
					</div>

				</div>
				<br />
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row-fluid">

					<div class="col-xs-12  col-sm-3 text-left-more">
						<span><span style="display:inline-block; vertical-align:top;"><asp:Label ID="lbActionPrompt" Text="Action " runat="server"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:&nbsp;</span>
						<asp:TextBox ID="tbFinalAction" Rows="3" Height="65px" Width="300" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox></span>
						<asp:RequiredFieldValidator runat="server" ID="rfvFinalAction" ControlToValidate="tbFinalAction" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDropDownList ID="rddlActionPerson" Skin="Metro" CssClass="WarnIfChanged" Width="198" runat="server" OnSelectedIndexChanged="rddlActionPerson_SelectedIndexChanged"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvActionPerson" ControlToValidate="rddlActionPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more"> 
						<telerik:RadDatePicker ID="rdpFinalStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
						<asp:RequiredFieldValidator runat="server" ID="rvfFinalStartDate" ControlToValidate="rdpFinalStartDate" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDatePicker ID="rdpFinalCompleteDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
					</div>

					<div class="col-xs-12  col-sm-1 text-left-more">
						<asp:CheckBox ID="cbFinalIsComplete" runat="server" Text="Complete" SkinID="Metro" TextAlign="Right"></asp:CheckBox>
					</div>
				</div>
				<br style="float:left; clear:both;"/><br />
			</ItemTemplate>
			<SeparatorTemplate><br /><br /></SeparatorTemplate>
			<FooterTemplate>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<asp:Button ID="btnAddFinal" CssClass="buttonAdd" runat="server" ToolTip="Add Another Final Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true" ></asp:Button>
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</div>   

</asp:Panel>


<asp:Panel ID="pnlApproval" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<asp:Repeater runat="server" ID="rptApprovals" ClientIDMode="AutoID" OnItemDataBound="rptApprovals_OnItemDataBound">
			<HeaderTemplate></HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 col-sm-2 text-left">
						<span><b>Approver&nbsp;<asp:Label ID="lbItemSeq" runat="server"></asp:Label>:</b>&nbsp;&nbsp;
							<asp:Label ID="lbApprover" Width="45%" SkinID="Metro" runat="server"></asp:Label></span>
					</div>
					<div class="col-xs-12 col-sm-3  text-left">
						<asp:Label ID="lbApproveMessage" Rows="5" Height="95px" Width="95%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:Label>
					</div>
					<div class="col-xs-12  col-sm-1 text-left">
						<span><asp:CheckBox ID="cbIsAccepted" runat="server" Font-Bold="false" Text="Accepted" SkinID="Metro" TextAlign="Right"></asp:CheckBox></span>
					</div>
					<div class="col-xs-12 col-sm-2 text-left">
						<span>Date Accepted:&nbsp;
						<telerik:RadDatePicker ID="rdpAcceptDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="120" runat="server"></telerik:RadDatePicker></span>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate><br /><br /></SeparatorTemplate>
			<FooterTemplate>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
				<div class="row">
					<div class="col-xs-12 text-left-more">
						<br />
					</div>
				</div>
			</FooterTemplate>
		</asp:Repeater>

	</div>
</asp:Panel>


<asp:Panel ID="pnlButtons" Visible="true" runat="server">


<div class="container-fluid">


		<div class="row">
			<div class="col-xs-12" style="padding: 5px">
				<asp:Label ID="lblResults" runat="server" ForeColor="ForestGreen" Font-Bold="true" CssClass="textStd"></asp:Label>
			</div>
		</div>

		<br class="visible-xs-block" />

		<div class="row">

			<div class="col-xs-12 text-left ">

				<span><telerik:RadButton ID="btnSave" runat="server" Text="Save" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnSave_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_PowerOutage" />&nbsp;&nbsp;</span>

				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />


				<span><telerik:RadButton ID="btnPrev" runat="server" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnPrev_Click" />&nbsp;&nbsp;</span>
	
				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />

	
				<span><telerik:RadButton ID="btnNext" runat="server" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnNext_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_PowerOutage" /></span>

				
				<span><telerik:RadButton ID="btnClose" runat="server" Text="Close Incident" Visible="false" CssClass="UseSubmitAction" Skin="Metro" 
					 OnClientClicking="StandardConfirm" ValidationGroup="Val_PowerOutage" /></span>


			</div>
		</div>
		
 </div>
	
 </asp:Panel>




