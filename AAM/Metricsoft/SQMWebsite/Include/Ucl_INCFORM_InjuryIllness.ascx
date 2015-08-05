<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_InjuryIllness.ascx.cs"
	Inherits="SQM.Website.Ucl_INCFORM_InjuryIllness" %>
	<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
	<%@ Register Src="~/Include/Ucl_INCFORM_Root5Y.ascx" TagName="INCFORMRoot5Y" TagPrefix="Ucl" %>
	<%@ Register Src="~/Include/Ucl_INCFORM_Contain.ascx" TagName="INCFORMContain" TagPrefix="Ucl" %>
	<%@ Register Src="~/Include/Ucl_INCFORM_Action.ascx" TagName="INCFORMAction" TagPrefix="Ucl" %>
	<%@ Register Src="~/Include/Ucl_INCFORM_Approval.ascx" TagName="INCFORMApproval" TagPrefix="Ucl" %>


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
			var validated = Page_ClientValidate('Val_InjuryIllness');

			if (!validated)
				alert("Please fill out all of the required fields.");
		}
	}

</script>


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
				<span><asp:Label ID="lbIncidentDateSM" runat ="server" Text="Incident Date"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbIncidentDateXS" runat ="server" Text="Incident Date"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpIncidentDate" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDatePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentDate" ControlToValidate="rdpIncidentDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- REPORT DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbReportDateSM" runat ="server" Text="Report Date"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbReportDateXS" runat ="server" Text="Report Date"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpReportDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="278" runat="server"></telerik:RadDatePicker>
			</div>
		</div>


		<%-- LOCATION question --%>	

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbLocationSM" runat ="server"  Text="Location"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbLocationXS" runat ="server" Text="Location"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlLocation" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" OnSelectedIndexChanged="rddlLocation_SelectedIndexChanged"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvLocation" ControlToValidate="rddlLocation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>
		

		
		<%-- DESCRIPTION question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="lbDescriptionSM" runat ="server" Text="Description"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbDescriptionXS" runat ="server" Text="Description"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvDescription" ControlToValidate="tbDescription" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- LOCAL DESCRIPTION question (show if local language is not English)  (MultiLine TEXTBOX) --%>
		<asp:Panel ID="pnlLocalDesc" runat="server" Visible="false">
			<div class="row" >
				<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
					<span class="labelMultiLineText"><asp:Label ID="lbLocalDescSM" runat ="server" Text="Local Description"></asp:Label><span class="requiredStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span><asp:Label ID="lbLocalDescXS" runat ="server" Text="Local Description"></asp:Label><span class="requiredStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbLocalDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator runat="server" ID="rfvLocalDescription" ControlToValidate="tbLocalDescription" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
				</div>
			</div>
		</asp:Panel>



		<%-- TIME OF INCIDENT question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbIncidentTimeSM" runat ="server" Text="Time of Incident"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbIncidentTimeXS" runat ="server" Text="Time of Incident"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadTimePicker ID="rtpIncidentTime" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadTimePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentTime" ControlToValidate="rtpIncidentTime" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- SHIFT question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbShiftSM" runat ="server" Text="Shift"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbShiftXS" runat ="server" Text="Shift"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlShift" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvShift" ControlToValidate="rddlShift" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>



		<%-- DEPARTMENT question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbDepartmentSM" runat ="server" Text="Department"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbDepartmentXS" runat ="server" Text="Department"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbDepartment" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvDepartment" ControlToValidate="tbDepartment" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- OPERATION question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbOperationSM" runat ="server" Text="Operation"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbOperationXS" runat ="server" Text="Operation"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlOperation" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rvfOperation" ControlToValidate="rddlOperation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- Involved Person's Name question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbInvolvedPersonSM" runat ="server" Text="Involved Person's Name"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbInvolvedPersonXS" runat ="server" Text="Involved Person's Name"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInvolvedPerson" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInvolvedPerson" ControlToValidate="tbInvolvedPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- Involved Person's Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="lbInvPersonStatementSM" runat ="server" Text="Involved Person's Statement"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbInvPersonStatementXS" runat ="server" Text="Involved Person's Statement"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInvPersonStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInvPersonStatement" ControlToValidate="tbInvPersonStatement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- SUPERVISOR INFORMED DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbSupvInformedDateSM" runat ="server" Text="Date Supervisor Informed"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbSupvInformedDateXS" runat ="server" Text="Date Supervisor Informed"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpSupvInformedDate" Skin="Metro" CssClass="WarnIfChanged"  Width="278" runat="server"></telerik:RadDatePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvSupvInformedDate" ControlToValidate="rdpSupvInformedDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>

			</div>
		</div>


		
		<%-- SUPERVISOR NAME question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbSupervisorSM" runat ="server" Text="Supervisor's Name"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbSupervisorXS" runat ="server" Text="Supervisor's Name"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlSupervisor" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" OnSelectedIndexChanged="rddlSupervisor_SelectedIndexChanged"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvSupervisor" ControlToValidate="rddlSupervisor" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- SUPERVISOR'S Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="lbSupervisorStatementSM" runat ="server" Text="Supervisor's Statement"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbSupervisorStatementXS" runat ="server" Text="Supervisor's Statement"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbSupervisorStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvSupervisorStatement" ControlToValidate="tbSupervisorStatement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- WITNESSS question --%>
		<asp:Panel ID="pnlWitness" Visible="true" runat="server">
			<br />
<%--			<div class="container-fluid">--%>

				<asp:Repeater runat="server" ID="rptWitness" ClientIDMode="AutoID" OnItemDataBound="rptWitness_OnItemDataBound" OnItemCommand="rptWitness_ItemCommand">
					<HeaderTemplate>

						<div class="row">
							<div class="col-sm-2 text-center">
								<span><b>
									<asp:Label ID="lbWitnessName" runat="server" Text="Witness Name"></asp:Label></b></span>
							</div>
							<div class="col-sm-3  text-center">
								<span><b>
									<asp:Label ID="lbWitnessStatement" runat="server" Text="Statement"></asp:Label></b></span>
							</div>
						</div>
						<br />
					</HeaderTemplate>
					<ItemTemplate>
						<div class="row-fluid">

							<div class="col-xs-12 col-sm-2 text-left-more">
								<asp:TextBox ID="tbWitnessName" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator runat="server" ID="rfvWitnessName" ControlToValidate="tbWitnessName" Display="None" InitialValue="[Select One]" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
							</div>

							<div class="col-xs-12  col-sm-3 text-left-more">
								<span><span style="display: inline-block; vertical-align: top;">
									<asp:Label ID="lbWitnessPrompt" Text="Witness " runat="server"></asp:Label><asp:Label ID="lbItemSeq" runat="server"></asp:Label>:&nbsp;</span>
									<asp:TextBox ID="tbWitnessStatement" Rows="3" Height="65px" Width="300" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox></span>
								<asp:RequiredFieldValidator runat="server" ID="rfvWitnessStatement" ControlToValidate="tbWitnessStatement" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
							</div>

						</div>
						<br style="float: left; clear: both;" />
						<br />
					</ItemTemplate>
					<SeparatorTemplate>
						<br />
						<br />
					</SeparatorTemplate>
					<FooterTemplate>
						<div class="row">
							<div class="col-xs-12 text-left-more">
								<br />
							</div>
						</div>
						<div class="row">
							<div class="col-xs-12 text-left-more">
								<asp:Button ID="btnAddFinal" CssClass="buttonAdd" runat="server" ToolTip="Add Another Final Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true"></asp:Button>
							</div>
						</div>
						<div class="row">
							<div class="col-xs-12 text-left-more">
								<br />
							</div>
						</div>
					</FooterTemplate>
				</asp:Repeater>
	<%--		</div>--%>
		</asp:Panel>


		<%-- INSIDE OUTSIDE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbInsideOutsideSM" runat ="server" Text="Inside or Outside Building"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbInsideOutsideXS" runat ="server" Text="Inside or Outside Building"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInsideOutside" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInsideOutside" ControlToValidate="tbInsideOutside" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- DIRECTLY SUPERVISED by AAM question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbDirectSupvSM" runat ="server" Text="Directly Supervised by AAM"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbDirectSupvXS" runat ="server" Text="Directly Supervised by AAM"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoDirectSupv" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>	
		        <asp:RequiredFieldValidator runat="server" ID="rfvDirectSupv" ControlToValidate="rdoDirectSupv" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
						
			</div>
		</div>




		<%-- ERGONOMIC CONCERNS question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbErgConcernSM" runat ="server" Text="Ergonomic Concerns"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbErgConcernXS" runat ="server" Text="Ergonomic Concerns"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoErgConcern" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
		        <asp:RequiredFieldValidator runat="server" ID="rfvErgConcern" ControlToValidate="rdoErgConcern" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>

			</div>
		</div>



		<%-- STD WORK PROCEDURES FOLLOWED question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbStdProcsFollowedSM" runat ="server" Text="Standard Work Procedures Followed without Deviation?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbStdProcsFollowedXS" runat ="server" Text="Standard Work Procedures Followed without Deviation?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoStdProcsFollowed" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Standard&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="Non-Standard"></asp:ListItem>
			   </asp:RadioButtonList></span>	
		        <asp:RequiredFieldValidator runat="server" ID="rfvStdProcsFollowed" ControlToValidate="rdoStdProcsFollowed" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
							
			</div>
		</div>



		<%-- TRAINING PROVIDED question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbTrainingProvidedSM" runat ="server" Text="Was Training for this Task Provided?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbTrainingProvidedXS" runat ="server" Text="Was Training for this Task Provided?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoTrainingProvided" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>	
		        <asp:RequiredFieldValidator runat="server" ID="rfvTrainingProvided" ControlToValidate="rdoTrainingProvided" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>



		<%-- HOW LONG DOING TASK question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbTaskYearsSM" runat ="server" Text="How long has associate been doing this job/specific task?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbTaskYearsXS" runat ="server" Text="How long has associate been doing this job/specific task?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol" style="padding-bottom:5px;padding-top:6px;">
				<span><span><asp:TextBox ID="tbTaskYears" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskYears" Text="Years" runat="server"></asp:Label></span>&nbsp;&nbsp;
				<span><asp:TextBox ID="tbTaskMonths" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskMonths" Text="Months" runat="server"></asp:Label></span>&nbsp;&nbsp;
				<span><asp:TextBox ID="tbTaskDays" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskDays" Text="Days" runat="server"></asp:Label></span></span>&nbsp;&nbsp;
		        <asp:RequiredFieldValidator runat="server" ID="rfvTaskDays" ControlToValidate="tbTaskDays" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>




		<%-- FIRST AID question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbFirstAidSM" runat ="server" Text="First Aid?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbFirstAidXS" runat ="server" Text="First Aid?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoFirstAid" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>		
		        <asp:RequiredFieldValidator runat="server" ID="rfvFirstAid" ControlToValidate="rdoFirstAid" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>



		<%-- RECORDABLE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbRecordableSM" runat ="server" Text="Recordable?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbRecordableXS" runat ="server" Text="Recordable?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoRecordable" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>		
		        <asp:RequiredFieldValidator runat="server" ID="rfvRecordable" ControlToValidate="rdoRecordable" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- LOST TIME question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbLostTimeSM" runat ="server" Text="Lost Time?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbLostTimeXS" runat ="server" Text="Lost Time?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoLostTime" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>		
		        <asp:RequiredFieldValidator runat="server" ID="rfvLostTime" ControlToValidate="rdoLostTime" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- EXPECTED RETURN DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbExpectReturnDTSM" runat ="server" Text="Expected Return Date"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbExpectReturnDTXS" runat ="server" Text="Expected Return Date"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpExpectReturnDT" Skin="Metro" CssClass="WarnIfChanged"  Width="278" runat="server"></telerik:RadDatePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvExpectReturnDT" ControlToValidate="rdpExpectReturnDT" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>



		<%-- TYPE OF INJURY question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbInjuryTypeSM" runat ="server" Text="Type of Injury"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbInjuryTypeXS" runat ="server" Text="Type of Injury"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlInjuryType" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInjuryType" ControlToValidate="rddlInjuryType" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- BODY PART question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbBodyPartSM" runat ="server" Text="Body Part"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbBodyPartXS" runat ="server" Text="Body Part"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlBodyPart" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvBodyPart" ControlToValidate="rddlBodyPart" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>

		<%-- ATTACHMENTS --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbAttachemntSM" runat ="server" Text="Attachments"></asp:Label></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbAttachemntXS" runat ="server" Text="Attachments"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<span><telerik:RadButton ID="btnBrowseAttach" runat="server" Text="Browse" Visible="true"  CssClass="UseSubmitAction" 
						OnClick="btnBrowseAttach_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" />&nbsp;&nbsp;
					<telerik:RadButton ID="btnUploadAttach" runat="server" Text="Upload" Visible="true" CssClass="UseSubmitAction"  
						OnClick="btnUploadAttach_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" /></span>
				

			</div>
		</div>
	</div>

	 	<br /><br />

 </asp:Panel>

 <asp:Panel ID="pnlBaseForm2" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

	</div>

	 	<br /><br />

 </asp:Panel>


 <ucl:INCFORMContain id="uclcontain" runat="server" />

 <ucl:INCFORMRoot5Y id="uclroot5y" runat="server" />

 <ucl:INCFORMAction id="uclaction" runat="server" />

 <ucl:INCFORMApproval id="uclapproval" runat="server" />


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
					OnClick="btnSave_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" />&nbsp;&nbsp;</span>

				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />


				<span><telerik:RadButton ID="btnPrev" runat="server" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnPrev_Click" />&nbsp;&nbsp;</span>
	
				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />

	
				<span><telerik:RadButton ID="btnNext" runat="server" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnNext_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" /></span>

				
				<span><telerik:RadButton ID="btnClose" runat="server" Text="Close Incident" Visible="false" CssClass="UseSubmitAction" Skin="Metro" 
					 OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" /></span>


			</div>
		</div>
		
 </div>
	
 </asp:Panel>




