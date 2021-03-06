﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_InjuryIllness.ascx.cs"
	Inherits="SQM.Website.Ucl_INCFORM_InjuryIllness" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Root5Y.ascx" TagName="INCFORMRoot5Y" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Contain.ascx" TagName="INCFORMContain" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Action.ascx" TagName="INCFORMAction" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Approval.ascx" TagName="INCFORMApproval" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_LostTime_Hist.ascx" TagName="INCFORMLostTimeHist" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="UploadAttachment" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadScriptBlock.ascx" TagName="RadScript" TagPrefix="Ucl" %>


<script type="text/javascript">

	window.onload = function () {
		InitialAction();
	};

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

	function InitialAction() {
	}

</script>

<script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>



<asp:Label ID="lblRequired" runat="server" Text="Required Fields Must be Completed." ForeColor="#cc0000" Font-Bold="true" Height="25" Visible="false"></asp:Label>
<asp:Label ID="lblSubmitted" runat="server" Text="Power Outage submitted." Font-Bold="true" Visible="false"></asp:Label>


<div class="container">
	<div class="row text_center">
<%--		<div class="col-xs-12 col-sm-4" style="padding: 3px 1px">
			<asp:Label ID="lblFormStepNumber" runat="server" Font-Bold="true" CssClass="textStd"></asp:Label>
		</div>--%>
		<div class="col-xs-12 col-sm-12 text-center">
			<asp:Label ID="lblFormTitle" runat="server" Font-Bold="true" CssClass="pageTitles"></asp:Label>
		</div>
	</div>
</div>


<asp:Panel ID="pnlBaseForm" Visible="true" runat="server">


	<Ucl:RadScript ID="uclRadScript" runat="server" />
	<%--  gotta have this motherfucker for radsearch to work--%>

	<br />

	<div class="container-fluid">

		<%-- INCIDENT DATE question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbIncidentDateSM" runat="server" Text="Incident Date"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbIncidentDateXS" runat="server" Text="Incident Date"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpIncidentDate" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" ShowPopupOnFocus="true"></telerik:RadDatePicker>
				<asp:RequiredFieldValidator runat="server" ID="rfvIncidentDate" ControlToValidate="rdpIncidentDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- REPORT DATE question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbReportDateSM" runat="server" Text="Report Date"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbReportDateXS" runat="server" Text="Report Date"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpReportDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false" Width="278" runat="server"></telerik:RadDatePicker>
			</div>
		</div>


		<%-- LOCATION question --%>

		<%--		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span><asp:Label ID="lbLocationSM" runat ="server"  Text="Location"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span><asp:Label ID="lbLocationXS" runat ="server" Text="Location"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlLocation" Skin="Metro" CssClass="WarnIfChanged" Width="278" DropDownHeight="300" runat="server" OnSelectedIndexChanged="rddlLocation_SelectedIndexChanged"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvLocation" ControlToValidate="rddlLocation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>--%>



		<%-- DESCRIPTION question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">
					<asp:Label ID="lbDescriptionSM" runat="server" Text="Description"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbDescriptionXS" runat="server" Text="Description"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
				<asp:RequiredFieldValidator runat="server" ID="rfvDescription" ControlToValidate="tbDescription" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- LOCAL DESCRIPTION question (show if local language is not English)  (MultiLine TEXTBOX) --%>
		<asp:Panel ID="pnlLocalDesc" runat="server" Visible="false">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
					<span class="labelMultiLineText">
						<asp:Label ID="lbLocalDescSM" runat="server" Text="Local Description"></asp:Label><span class="requiredStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbLocalDescXS" runat="server" Text="Local Description"></asp:Label><span class="requiredStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbLocalDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator runat="server" ID="rfvLocalDescription" ControlToValidate="tbLocalDescription" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
				</div>
			</div>
		</asp:Panel>



		<%-- TIME OF INCIDENT question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbIncidentTimeSM" runat="server" Text="Time of Incident"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbIncidentTimeXS" runat="server" Text="Time of Incident"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadTimePicker ID="rtpIncidentTime" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" ShowPopupOnFocus="true"></telerik:RadTimePicker>
				<asp:RequiredFieldValidator runat="server" ID="rfvIncidentTime" ControlToValidate="rtpIncidentTime" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- SHIFT question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbShiftSM" runat="server" Text="Shift"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbShiftXS" runat="server" Text="Shift"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlShift" Skin="Metro" CssClass="WarnIfChanged" DropDownHeight="300" Width="278" runat="server"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvShift" ControlToValidate="rddlShift" Display="None" InitialValue="[Select One]" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>



		<%-- DEPARTMENT question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbDepartmentSM" runat="server" Text="Department"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbDepartmentXS" runat="server" Text="Department"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlDepartment" Skin="Metro" DropDownHeight="300" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvDepartment" ControlToValidate="rddlDepartment" Display="None" InitialValue="[Select One]" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- OPERATION question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbOperationSM" runat="server" Text="Operation"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbOperationXS" runat="server" Text="Operation"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlOperation" Skin="Metro" CssClass="WarnIfChanged" DropDownHeight="300" Width="278" runat="server"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvOperation" ControlToValidate="rddlOperation" Display="None" InitialValue="[Select One]" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- Involved Person's Name question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbInvolvedPersonSM" runat="server" Text="Involved Person's Name"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInvolvedPersonXS" runat="server" Text="Involved Person's Name"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadAjaxPanel ID="rajx100" runat="server">
						<telerik:RadSearchBox ID="rsbInvolvedPerson" runat="server"  EnableAutoComplete="true" MaxResultCount="20" DataKeyNames="PersonId" Skin="Metro" OnSearch="rsbInvolvedPerson_Search"
							ShowSearchButton="false" EmptyMessage="Begin typing (or spacebar)" Width="276" >
							<DropDownSettings Height="320" Width="510" >
								<HeaderTemplate>
									<table cellpadding="0" cellspacing="1" class="searchBoxResults" width="500" style="margin-left: 5px;">
										<tr>
											<th style="width: 110px; text-align: left;">
												Name
											</th>
											<th style="width: 200px; text-align: left;">
												Email
											</th>
											<th></th>
										</tr>
									</table>
								</HeaderTemplate>
								<ItemTemplate>
									<table cellpadding="0" cellspacing="0" class="searchBoxResults"  width="500" style="margin-left: 5px;">
									<tr>
										<td style="background: #EEEAE0; width: 110px;">
											<b><%# DataBinder.Eval(Container.DataItem, "PersonName") %></b>
										</td>
										<td style="background: #fff; width: 200px;">
											<b><%# DataBinder.Eval(Container.DataItem, "PersonEmail")%></b>
										</td>
										<td id="tdPersonID" runat="server" visible="false">
											<%# DataBinder.Eval(Container.DataItem, "PersonId")%>
										</td>
									</tr>
									</table>
								</ItemTemplate>
							</DropDownSettings>
						</telerik:RadSearchBox>
				</telerik:RadAjaxPanel>
				<%--<telerik:RadDropDownList ID="rddlInvolvedPerson" Skin="Metro" CssClass="WarnIfChanged" Enabled="false" DropDownHeight="300" Width="278" runat="server" OnSelectedIndexChanged="rddlInvolvedPerson_SelectedIndexChanged" Visible="false"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvInvolvedPersonSearch" ControlToValidate="uclInvolvedPersonSearch" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>


		<%-- Involved Person's Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">
					<asp:Label ID="lbInvPersonStatementSM" runat="server" Text="Involved Person's Statement"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInvPersonStatementXS" runat="server" Text="Involved Person's Statement"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInvPersonStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
				<%--<asp:RequiredFieldValidator runat="server" ID="rfvInvPersonStatement" ControlToValidate="tbInvPersonStatement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>


		<%-- SUPERVISOR INFORMED DATE question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbSupvInformedDateSM" runat="server" Text="Date Supervisor Informed"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbSupvInformedDateXS" runat="server" Text="Date Supervisor Informed"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpSupvInformedDate" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" ShowPopupOnFocus="true"></telerik:RadDatePicker>
				<%--<asp:RequiredFieldValidator runat="server" ID="rfvSupvInformedDate" ControlToValidate="rdpSupvInformedDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>



		<%-- SUPERVISOR NAME question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbSupervisorSM" runat="server" Text="Supervisor's Name"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbSupervisorXS" runat="server" Text="Supervisor's Name"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadAjaxPanel ID="rapSupervisor" runat="server">
					<asp:Label ID="lbSupervisor" Width="400" runat="server" Height="23" />
					<%--<asp:RequiredFieldValidator runat="server" ID="rfvSupervisor" ControlToValidate="tbSupervisor" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
				</telerik:RadAjaxPanel>
			</div>
		</div>



		<%-- SUPERVISOR'S Statement question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">
					<asp:Label ID="lbSupervisorStatementSM" runat="server" Text="Supervisor's Statement"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbSupervisorStatementXS" runat="server" Text="Supervisor's Statement"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbSupervisorStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
				<%--<asp:RequiredFieldValidator runat="server" ID="rfvSupervisorStatement" ControlToValidate="tbSupervisorStatement" Display="None" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>


		<%-- WITNESSS question --%>
		<asp:Repeater runat="server" ID="rptWitness" ClientIDMode="AutoID" OnItemDataBound="rptWitness_OnItemDataBound" OnItemCommand="rptWitness_ItemCommand">
			<ItemTemplate>

				<div class="row text-left">

					<div class="col-sm-4 hidden-xs text-left tanLabelColHigh" style="height: 100px;">
						<span class="labelMultiLineText">
							<asp:Label ID="lbWitnessColSM" runat="server" Text="Witness " /><asp:Label ID="lbItemSeq" runat="server"></asp:Label><asp:Label ID="lbRqd1" Text="*" CssClass="requiredCloseStarFloat" runat="server"></asp:Label></span>
					</div>

					<div class="col-xs-12 visible-xs text-left-more">
						<br />
						<br />
						<span>
							<asp:Label ID="lbWitnessColXS" runat="server" Text="Witness " />
							<asp:Label ID="lbItemSeq2" runat="server" /><asp:Label ID="lbRqd2" Text="*" CssClass="requiredCloseStar" runat="server" />
						</span>
					</div>

					<div class="col-xs-12 col-sm-8 text-left greyControlCol" style="height: 100px; padding-bottom: 4px; padding-top: 7px;">

						<div class="row">

							<div class="col-xs-12 col-sm-4 text-left">

								<asp:Label ID="lbWitNamePrompt" runat="server" Text="Name: " />&nbsp;&nbsp;

								<telerik:RadAjaxPanel ID="rajx200" runat="server">
									<telerik:RadSearchBox ID="rsbWitnessName" runat="server" EnableAutoComplete="true" MaxResultCount="20" DataKeyNames="PersonId" Skin="Metro" OnSearch="rsbWitnessName_Search"
										ShowSearchButton="false" EmptyMessage="Begin typing (or spacebar)" Width="100%" CssClass="NoBorders">
										<DropDownSettings Height="320" Width="510">
											<HeaderTemplate>
												<table cellpadding="0" cellspacing="1" class="searchBoxResults" width="500" style="margin-left: 5px;">
													<tr>
														<th style="width: 110px; text-align: left;">Name
														</th>
														<th style="width: 200px; text-align: left;">Email
														</th>
														<th></th>
													</tr>
												</table>
											</HeaderTemplate>
											<ItemTemplate>
												<table cellpadding="0" cellspacing="0" class="searchBoxResults" width="500" style="margin-left: 5px;">
													<tr>
														<td style="background: #EEEAE0; width: 110px;">
															<b><%# DataBinder.Eval(Container.DataItem, "PersonName") %></b>
														</td>
														<td style="background: #fff; width: 200px;">
															<b><%# DataBinder.Eval(Container.DataItem, "PersonEmail")%></b>
														</td>
														<td id="tdPersonID" runat="server" visible="false">
															<%# DataBinder.Eval(Container.DataItem, "PersonId")%>
														</td>
													</tr>
												</table>
											</ItemTemplate>
										</DropDownSettings>
									</telerik:RadSearchBox>
								</telerik:RadAjaxPanel>
							</div>

							<div class="col-xs-12 col-sm-4 text-left">

								<asp:Label ID="lbWitStmntPrompt" runat="server" Text="Statement:"></asp:Label>&nbsp;&nbsp;
								<asp:TextBox ID="tbWitnessStatement" Width="100%" Height="60px" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>

							</div>

							<div class="col-xs-12 col-sm-3 text-left">

								<span style="display: inline-block; padding-top:3px;">
									<telerik:RadButton ID="btnItemDelete" runat="server" Width="100%" Height="10px" CssClass="buttonWrapText" ButtonType="LinkButton" BorderStyle="None" ForeColor="DarkRed" CommandArgument="Delete"
										SingleClick="true" OnClientClicking="DeleteConfirmItem" />
								</span>
							</div>

						</div>

					</div>
				</div>
				<br class="visible-xs" style="padding-top: 5px;" />
			</ItemTemplate>
			<%--<SeparatorTemplate></SeparatorTemplate>--%>
			<FooterTemplate>
				<br class="visible-xs" style="padding-top: 5px;" />
				<div class="row">
					<div class="col-sm-4 text-left tanLabelCol">
						<asp:Button ID="btnAddWitness" CssClass="buttonAdd" runat="server" Font-Size="Smaller" ToolTip="Add Another Witness" Text="Add Another Witness" Style="margin: 7px;" CommandArgument="AddAnother" UseSubmitBehavior="true"></asp:Button>
					</div>
					<div class="col-xs-12 col-sm-8 text-left"></div>
				</div>
			</FooterTemplate>
		</asp:Repeater>


		<%-- INSIDE OUTSIDE question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbInsideOutsideSM" runat="server" Text="Inside or Outside Building"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInsideOutsideXS" runat="server" Text="Inside or Outside Building"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoInside" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Inside&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="Outside"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvInside" ControlToValidate="rdoInside" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- DIRECTLY SUPERVISED by AAM question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbDirectSupvSM" runat="server" Text="Directly Supervised by AAM"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbDirectSupvXS" runat="server" Text="Directly Supervised by AAM"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoDirectSupv" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="No"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvDirectSupv" ControlToValidate="rdoDirectSupv" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- ERGONOMIC CONCERNS question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbErgConcernSM" runat="server" Text="Ergonomic Concerns"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbErgConcernXS" runat="server" Text="Ergonomic Concerns"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoErgConcern" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="No"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvErgConcern" ControlToValidate="rdoErgConcern" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- STD WORK PROCEDURES FOLLOWED question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbStdProcsFollowedSM" runat="server" Text="Standard Work Procedures Followed without Deviation?"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbStdProcsFollowedXS" runat="server" Text="Standard Work Procedures Followed without Deviation?"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoStdProcsFollowed" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Standard&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="Non-Standard"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvStdProcsFollowed" ControlToValidate="rdoStdProcsFollowed" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- TRAINING PROVIDED question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbTrainingProvidedSM" runat="server" Text="Was Training for this Task Provided?"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbTrainingProvidedXS" runat="server" Text="Was Training for this Task Provided?"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoTrainingProvided" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="No"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvTrainingProvided" ControlToValidate="rdoTrainingProvided" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- HOW LONG DOING TASK question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 37px;">
				<span>
					<asp:Label ID="lbTaskYearsSM" runat="server" Text="How long has associate been doing this job/specific task?"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more" style="height: 37px;">
				<br />
				<span>
					<asp:Label ID="lbTaskYearsXS" runat="server" Text="How long has associate been doing this job/specific task?"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol" style="height: 37px;">
				<span style="display: inline-block;"><span>
					<asp:TextBox ID="tbTaskYears" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskYears" Text="Years" runat="server"></asp:Label></span>&nbsp;&nbsp; <span>
						<asp:TextBox ID="tbTaskMonths" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskMonths" Text="Months" runat="server"></asp:Label></span>&nbsp;&nbsp; <span>
							<asp:TextBox ID="tbTaskDays" Width="50" SkinID="Metro" runat="server"></asp:TextBox>&nbsp;<asp:Label ID="lbTaskDays" Text="Days" runat="server"></asp:Label></span></span>&nbsp;&nbsp;
				<asp:RegularExpressionValidator ID="revTaskYears" runat="server" ControlToValidate="tbTaskYears" ValidationExpression="[0-9]+" SetFocusOnError="true" ForeColor="Red" ErrorMessage="Years must be a valid numeric value" ValidationGroup="Val_InjuryIllness"></asp:RegularExpressionValidator><asp:RegularExpressionValidator ID="revTaskMonths" runat="server" ControlToValidate="tbTaskMonths" ValidationExpression="[0-9]+" SetFocusOnError="true" ForeColor="Red" ErrorMessage="Months must be a valid numeric value" ValidationGroup="Val_InjuryIllness"></asp:RegularExpressionValidator><asp:RegularExpressionValidator ID="revTaskDays" runat="server" ControlToValidate="tbTaskDays" ValidationExpression="[0-9]+" SetFocusOnError="true" ForeColor="Red" ErrorMessage="Days must be a valid numeric value" ValidationGroup="Val_InjuryIllness"></asp:RegularExpressionValidator><%--<asp:RequiredFieldValidator runat="server" ID="rfvTaskDays"  ControlToValidate="tbTaskDays" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- FIRST AID question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbFirstAidSM" runat="server" Text="First Aid?"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbFirstAidXS" runat="server" Text="First Aid?"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoFirstAid" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="No"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvFirstAid" ControlToValidate="rdoFirstAid" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- RECORDABLE question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbRecordableSM" runat="server" Text="Recordable?"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbRecordableXS" runat="server" Text="Recordable?"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoRecordable" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="No"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvRecordable" ControlToValidate="rdoRecordable" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- LOST TIME question --%><div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbLostTimeSM" runat="server" Text="Lost Time?"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbLostTimeXS" runat="server" Text="Lost Time?"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoLostTime" OnSelectedIndexChanged="rdoLostTime_SelectedIndexChanged" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" AutoPostBack="true" runat="server">
						<asp:ListItem Value="1" Selected="False" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;"></asp:ListItem>
						<asp:ListItem Value="0" Text="No"></asp:ListItem>
					</asp:RadioButtonList></span><%--<asp:RequiredFieldValidator runat="server" ID="rfvLostTime" ControlToValidate="rdoLostTime" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>
		<%-- EXPECTED RETURN DATE question --%><asp:Panel ID="pnlExpReturnDT" runat="server" Visible="false">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol">
					<span>
						<asp:Label ID="lbExpectReturnDTSM" runat="server" Text="Expected Return Date"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbExpectReturnDTXS" runat="server" Text="Expected Return Date"></asp:Label><span class="requiredCloseStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<telerik:RadDatePicker ID="rdpExpectReturnDT" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" ShowPopupOnFocus="true"></telerik:RadDatePicker>
					<%--<asp:RequiredFieldValidator runat="server" ID="rfvExpectReturnDT" ControlToValidate="rdpExpectReturnDT" Display="None" Enabled="false" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
				</div>
			</div>
		</asp:Panel>



		<%-- TYPE OF INJURY question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbInjuryTypeSM" runat="server" Text="Type of Injury"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInjuryTypeXS" runat="server" Text="Type of Injury"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlInjuryType" Skin="Metro" ExpandDirection="Up" CssClass="WarnIfChanged" Width="278" DropDownHeight="300" runat="server"></telerik:RadDropDownList>
				<%--<asp:RequiredFieldValidator runat="server" ID="rfvInjuryType" ControlToValidate="rddlInjuryType" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>


		<%-- BODY PART question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbBodyPartSM" runat="server" Text="Body Part"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbBodyPartXS" runat="server" Text="Body Part"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlBodyPart" Skin="Metro" ExpandDirection="Up" DropDownHeight="300" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
				<%--<asp:RequiredFieldValidator runat="server" ID="rfvBodyPart" ControlToValidate="rddlBodyPart" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_InjuryIllness"></asp:RequiredFieldValidator>--%>
			</div>
		</div>

		<%-- ATTACHMENTS --%>
		<telerik:RadAjaxPanel EnableAJAX="true" ID="rapAttach" runat="server">
			<div class="row">
				<div id="dvAttachLbl" runat="server" class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 128px;">
					<span>
						<asp:Label ID="lbAttachemntSM" runat="server" Text="Attachments"></asp:Label></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbAttachemntXS" runat="server" Text="Attachments"></asp:Label></span>
				</div>
				<div id="dvAttach" runat="server" class="col-xs-12 col-sm-8 text-left greyControlCol" style="height: 128px;">
					<span style="border: 0 none !important;">
						<Ucl:UploadAttachment ID="uploader" runat="server" />
					</span>
				</div>
			</div>
		</telerik:RadAjaxPanel>
	</div>

	<br />
	<br />

</asp:Panel>

<asp:Panel ID="pnlBaseForm2" Visible="false" runat="server">

	<br />

	<div class="container-fluid">
	</div>

	<br />
	<br />

</asp:Panel>


<Ucl:INCFORMContain ID="uclcontain" runat="server" />

<Ucl:INCFORMRoot5Y ID="uclroot5y" runat="server" />

<Ucl:INCFORMAction ID="uclaction" runat="server" />

<Ucl:INCFORMApproval ID="uclapproval" runat="server" />

<Ucl:INCFORMLostTimeHist ID="ucllosttime" runat="server" />



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
				<span style="margin-top: 5px;">
					<telerik:RadButton ID="btnSubnavSave" runat="server" Text="Save" CssClass="UseSubmitAction" Skin="Metro" Style="margin-right: 10px;"
						OnClick="btnSubnavSave_Click" CommandArgument="0"/>
					<asp:LinkButton ID="btnSubnavIncident" runat="server" Text="Incident" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="0" />
					<asp:LinkButton ID="btnSubnavLostTime" runat="server" Text="Lost time History" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="6"/>
					<asp:LinkButton ID="btnSubnavContainment" runat="server" Text="Initial Action" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="2"/>
					<asp:LinkButton ID="btnSubnavRootCause" runat="server" Text="Root Cause" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="3"/>
					<asp:LinkButton ID="btnSubnavAction" runat="server" Text="Corrective Action" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="4"/>
					<asp:LinkButton ID="btnSubnavApproval" runat="server" Text="Approvals" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="5"/>
					<span style="float:right">
						<telerik:RadButton ID="btnDeleteInc" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
							Text="Delete Incident" SingleClick="true" SingleClickText="Deleting..."
							OnClick="btnDeleteInc_Click" OnClientClicking="DeleteConfirm" CssClass="UseSubmitAction" />
					</span>
				</span>
			</div>
		</div>



		<br />

	<%--	<div class="row">

			<div class="col-xs-12 text-left ">

				<span>
					<telerik:RadButton ID="btnSave" runat="server" Text="Save" Visible="true" CssClass="UseSubmitAction" Skin="Metro"
						OnClick="btnSave_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" />
					&nbsp;&nbsp;</span>
				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />


				<span>
					<telerik:RadButton ID="btnPrev" runat="server" Visible="true" CssClass="UseSubmitAction" Skin="Metro"
						OnClick="btnPrev_Click" />
					&nbsp;&nbsp;</span>
				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />


				<span>
					<telerik:RadButton ID="btnNext" runat="server" Visible="true" CssClass="UseSubmitAction" Skin="Metro"
						OnClick="btnNext_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" />
				</span>


				<span>
					<telerik:RadButton ID="btnClose" runat="server" Text="Close Incident" Visible="false" CssClass="UseSubmitAction" Skin="Metro"
						OnClientClicking="StandardConfirm" ValidationGroup="Val_InjuryIllness" />
				</span>

				<span style="float:right">
					<telerik:RadButton ID="btnDeleteInc" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
						Text="Delete Incident" SingleClick="true" SingleClickText="Deleting..."
						OnClick="btnDeleteInc_Click" OnClientClicking="DeleteConfirm" CssClass="UseSubmitAction" />
				</span>

			</div>
		</div>--%>

	</div>

</asp:Panel>
