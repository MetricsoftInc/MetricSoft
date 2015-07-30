<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_InjuryIllness.ascx.cs"
	Inherits="SQM.Website.Ucl_INCFORM_InjuryIllness" %>
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
			var validated = Page_ClientValidate('Val_InjuryIllness');

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


		<%-- REMOVE THIS ===>  Here only for testing  PRODUCTION IMPACT question (MultiLine TEXTBOX) --%>
		<asp:Panel ID="pnlRemoveMe" runat="server" Visible="false">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
					<span class="labelMultiLineText"><asp:Label ID="lbProdImpactSM" runat ="server" Text="ProdImpact"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span><asp:Label ID="lbProdImpactXS" runat ="server" Text="ProdImpact"></asp:Label><span class="requiredCloseStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbProdImpact" Rows="5" Height="95px" Visible="false" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
				</div>
			</div>
		</asp:Panel>


		<%-- DEPARTMENT question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label1" runat ="server" Text="Department"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label2" runat ="server" Text="Department"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlDepartment" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvDepartment" ControlToValidate="rddlDepartment" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- OPERATION question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label3" runat ="server" Text="Operation"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label4" runat ="server" Text="Operation"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlOperation" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rvfOperation" ControlToValidate="rddlOperation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- Involved Person's Name question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label5" runat ="server" Text="Involved Person's Name"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label6" runat ="server" Text="Involved Person's Name"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInvolvedPerson" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInvolvedPerson" ControlToValidate="tbInvolvedPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- Involved Person's Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="Label7" runat ="server" Text="Involved Person's Statement"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label8" runat ="server" Text="Involved Person's Statement"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInvPersonStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInvPersonStatement" ControlToValidate="tbInvPersonStatement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- SUPERVISOR INFORMED DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label9" runat ="server" Text="Date Supervisor Informed"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label10" runat ="server" Text="Date Supervisor Informed"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rddlSupvInformedDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="278" runat="server"></telerik:RadDatePicker>
			</div>
		</div>


		
		<%-- SUPERVISOR NAME question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label11" runat ="server" Text="Supervisor's Name"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label12" runat ="server" Text="Supervisor's Name"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbSupervisorName" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvSupervisorName" ControlToValidate="tbSupervisorName" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- SUPERVISOR'S Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="Label13" runat ="server" Text="Supervisor's Statement"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label14" runat ="server" Text="Supervisor's Statement"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbSupervisorStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvSupervisorStatement" ControlToValidate="tbSupervisorStatement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- WITNESS 1 NAME question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label15" runat ="server" Text="Witness 1 Name"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label16" runat ="server" Text="Witness 1's Name"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbWitness1" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rvfWitness1" ControlToValidate="tbWitness1" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		
		<%-- WITNESS 1's  Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText"><asp:Label ID="Label17" runat ="server" Text="Witness 1 Statement"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label18" runat ="server" Text="Witness 1 Statement"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbWiness1Statement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvWiness1Statement" ControlToValidate="tbWiness1Statement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>



		<%-- INSIDE OUTSIDE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label19" runat ="server" Text="Inside or Outside Building"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label20" runat ="server" Text="Inside or Outside Building"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInsideOutside" Width="278" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInsideOutside" ControlToValidate="tbInsideOutside" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- DIRECTLY SUPERVISED by AAM question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label21" runat ="server" Text="Directly Supervised by AAM"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label22" runat ="server" Text="Directly Supervised by AAM"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoDirectSupv" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>




		<%-- ERGONOMIC CONCERNS question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label23" runat ="server" Text="Ergonomic Concerns"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label24" runat ="server" Text="Ergonomic Concerns"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoErgConcern" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>



		<%-- STD WORK PROCEDURES FOLLOWED question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label25" runat ="server" Text="Standard Work Procedures Followed without Deviation?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label26" runat ="server" Text="Standard Work Procedures Followed without Deviation?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoStdProcsFollowed" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Standard&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="Non-Standard"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>



		<%-- TRAINING PROVIDED question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label27" runat ="server" Text="Was Training for this Task Provided?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label28" runat ="server" Text="Was Training for this Task Provided?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoTrainingProvided" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>



		<%-- HOW LONG DOING TASK question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label29" runat ="server" Text="How long has associate been doing this job/specific task?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label30" runat ="server" Text="How long has associate been doing this job/specific task?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol" style="padding-bottom:5px;padding-top:6px;">
				<span><span><asp:TextBox ID="tbTaskYears" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskYears" Text="Years" runat="server"></asp:Label></span>&nbsp;&nbsp;
				<span><asp:TextBox ID="tbTaskMonths" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskMonths" Text="Months" runat="server"></asp:Label></span>&nbsp;&nbsp;
				<span><asp:TextBox ID="tbTaskDays" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskDays" Text="Days" runat="server"></asp:Label></span></span>&nbsp;&nbsp;
			</div>
		</div>




		<%-- FIRST AID question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label31" runat ="server" Text="First Aid?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label32" runat ="server" Text="First Aid?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoFirstAid" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>



		<%-- RECORDABLE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label33" runat ="server" Text="Recordable?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label34" runat ="server" Text="Recordable?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoRecordable" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>


		<%-- LOST TIME question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label35" runat ="server" Text="Lost Time?"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label36" runat ="server" Text="Lost Time?"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
			   <span><asp:RadioButtonList ID="rdoLostTime" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
				   <asp:ListItem Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
				   <asp:ListItem  Text="No"></asp:ListItem>
			   </asp:RadioButtonList></span>				
			</div>
		</div>


		<%-- EXPECTED RETURN DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label39" runat ="server" Text="Expected Return Date"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label40" runat ="server" Text="Expected Return Date"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpExpectReturnDT" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="278" runat="server"></telerik:RadDatePicker>
			</div>
		</div>



		<%-- TYPE OF INJURY question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label37" runat ="server" Text="Type of Injury"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label38" runat ="server" Text="Type of Injury"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlInjuryType" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvInjuryType" ControlToValidate="rddlInjuryType" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- BODY PART question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label41" runat ="server" Text="Body Part"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label42" runat ="server" Text="Body Part"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlBodyPart" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvBodyPart" ControlToValidate="rddlBodyPart" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>

		<%-- ATTACHMENTS --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="Label43" runat ="server" Text="Attachments"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="Label44" runat ="server" Text="Attachments"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<span><telerik:RadButton ID="btnBrowseAttach" runat="server" Text="Browse" Visible="true"  CssClass="UseSubmitAction" 
						OnClick="btnBrowseAttach_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" />&nbsp;&nbsp;
					<telerik:RadButton ID="btnUploadAttach" runat="server" Text="Upload" Visible="true" CssClass="UseSubmitAction"  
						OnClick="btnUploadAttach_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" /></span>
				

			</div>
		</div>



	<%-- ------------------------------- --%>	
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
						<asp:RequiredFieldValidator runat="server" ID="rfvContainAction" ControlToValidate="tbContainAction" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDropDownList ID="rddlContainPerson" Skin="Metro" CssClass="WarnIfChanged" Width="198" runat="server" OnSelectedIndexChanged="rddlContainPerson_SelectedIndexChanged"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvContainPerson" ControlToValidate="rddlContainPerson" Display="None" InitialValue="[Select One]"  ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more"> 
						<telerik:RadDatePicker ID="rdpStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
						<asp:RequiredFieldValidator runat="server" ID="rvfStartDate" ControlToValidate="rdpStartDate" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
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
						<asp:RequiredFieldValidator runat="server" ID="rfvRootCause" ControlToValidate="tbRootCause" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
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
						<asp:RequiredFieldValidator runat="server" ID="rfvFinalAction" ControlToValidate="tbFinalAction" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDropDownList ID="rddlActionPerson" Skin="Metro" CssClass="WarnIfChanged" Width="198" runat="server" OnSelectedIndexChanged="rddlActionPerson_SelectedIndexChanged"></telerik:RadDropDownList>
						<asp:RequiredFieldValidator runat="server" ID="rfvActionPerson" ControlToValidate="rddlActionPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more"> 
						<telerik:RadDatePicker ID="rdpFinalStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true"  Width="175" runat="server"></telerik:RadDatePicker>
						<asp:RequiredFieldValidator runat="server" ID="rvfFinalStartDate" ControlToValidate="rdpFinalStartDate" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
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
						<asp:Label ID="lbApproveMessage" Height="95px" Width="95%" SkinID="Metro" runat="server"></asp:Label>
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




