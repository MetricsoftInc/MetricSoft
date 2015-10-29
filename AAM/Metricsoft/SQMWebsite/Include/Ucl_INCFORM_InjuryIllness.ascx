<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_InjuryIllness.ascx.cs"
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



<%-- including the below is incompatible w/ Telerik release 6/2015.  Don't know why it is needed --%>
<%--<script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>--%>


<asp:HiddenField id="hfIncidentDeletedMsg" runat="server" Value="The Incident has been Deleted"/>
<asp:Label ID="lblRequired" runat="server" Text="Required Fields Must be Completed." ForeColor="#CC0000" Font-Bold="True" Height="25px" Visible="False" meta:resourcekey="lblRequiredResource1"></asp:Label>
<asp:Label ID="lblSubmitted" runat="server" Text="Power Outage submitted." Font-Bold="True" Visible="False" meta:resourcekey="lblSubmittedResource1"></asp:Label>

<div class="container-fluid blueCell" style="padding: 7px; margin-top: 5px;">
	<asp:Panel ID="pnlIncidentHeader" runat="server" meta:resourcekey="pnlIncidentHeaderResource1">
		<div class="row-fluid" >
			<div class="col-xs-12  text-left">
				<span>
				<asp:Label ID="lblAddOrEditIncident" class="prompt" runat="server" meta:resourcekey="lblAddOrEditIncidentResource1"><strong>Add a New Incident:</strong></asp:Label>
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
			<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles" meta:resourcekey="lblFormTitleResource1"></asp:Label>
		</div>
	</div>
</div>

<asp:Panel ID="pnlBaseForm" runat="server" meta:resourcekey="pnlBaseFormResource1">

	<div class="container-fluid">
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbIncidentDateSM" runat="server" Text="Incident Date" meta:resourcekey="lbIncidentDateSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbIncidentDateXS" runat="server" Text="Incident Date" meta:resourcekey="lbIncidentDateXSResource1"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpIncidentDate" Skin="Metro" CssClass="WarnIfChanged" Width="278px" runat="server" ShowPopupOnFocus="True" meta:resourcekey="rdpIncidentDateResource1">
					<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
					</Calendar>
					<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</DateInput>
					<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
				</telerik:RadDatePicker>
				<asp:RequiredFieldValidator runat="server" ID="rfvIncidentDate" ControlToValidate="rdpIncidentDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness" meta:resourcekey="rfvIncidentDateResource1"></asp:RequiredFieldValidator>
			</div>
		</div>


		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbReportDateSM" runat="server" Text="Report Date" meta:resourcekey="lbReportDateSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbReportDateXS" runat="server" Text="Report Date" meta:resourcekey="lbReportDateXSResource1"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpReportDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="False" Width="278px" runat="server" meta:resourcekey="rdpReportDateResource1">
					<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
					</Calendar>
					<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</DateInput>
					<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
				</telerik:RadDatePicker>
			</div>
		</div>


		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">
					<asp:Label ID="lbDescriptionSM" runat="server" Text="<%$ Resources:LocalizedText, Description %>"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbDescriptionXS" runat="server" Text="<%$ Resources:LocalizedText, Description %>"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server" meta:resourcekey="tbDescriptionResource1"></asp:TextBox>
				<asp:RequiredFieldValidator runat="server" ID="rfvDescription" ControlToValidate="tbDescription" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness" meta:resourcekey="rfvDescriptionResource1"></asp:RequiredFieldValidator>
			</div>
		</div>


		<asp:Panel ID="pnlLocalDesc" runat="server" Visible="False" meta:resourcekey="pnlLocalDescResource1">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
					<span class="labelMultiLineText">
						<asp:Label ID="lbLocalDescSM" runat="server" Text="Local Description" meta:resourcekey="lbLocalDescSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbLocalDescXS" runat="server" Text="Local Description" meta:resourcekey="lbLocalDescXSResource1"></asp:Label><span class="requiredStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbLocalDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server" meta:resourcekey="tbLocalDescriptionResource1"></asp:TextBox>
					<asp:RequiredFieldValidator runat="server" ID="rfvLocalDescription" ControlToValidate="tbLocalDescription" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness" meta:resourcekey="rfvLocalDescriptionResource1"></asp:RequiredFieldValidator>
				</div>
			</div>
		</asp:Panel>

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbIncidentTimeSM" runat="server" Text="Time of Incident" meta:resourcekey="lbIncidentTimeSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbIncidentTimeXS" runat="server" Text="Time of Incident" meta:resourcekey="lbIncidentTimeXSResource1"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadTimePicker ID="rtpIncidentTime" Skin="Metro" CssClass="WarnIfChanged" Width="278px" runat="server" ShowPopupOnFocus="True" meta:resourcekey="rtpIncidentTimeResource1">
					<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
					</Calendar>
					<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
					<TimeView CellSpacing="-1">
						<HeaderTemplate>
							Time Picker
						</HeaderTemplate>
						<TimeTemplate>
							<a id="A1" runat="server" href="#"></a>
						</TimeTemplate>
					</TimeView>
					<TimePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
					<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</DateInput>
				</telerik:RadTimePicker>
				<asp:RequiredFieldValidator runat="server" ID="rfvIncidentTime" ControlToValidate="rtpIncidentTime" Display="None" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness" meta:resourcekey="rfvIncidentTimeResource1"></asp:RequiredFieldValidator>
			</div>
		</div>

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbShiftSM" runat="server" Text="Shift" meta:resourcekey="lbShiftSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbShiftXS" runat="server" Text="Shift" meta:resourcekey="lbShiftXSResource1"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlShiftID" Skin="Metro" CssClass="WarnIfChanged" ZIndex="9000"  ExpandDirection="Up" DropDownHeight="100px" Width="278px" runat="server" meta:resourcekey="rddlShiftIDResource1"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvShift" ControlToValidate="rddlShiftID" Display="None" InitialValue="[Select One]" ErrorMessage="Required" ValidationGroup="Val_InjuryIllness" meta:resourcekey="rfvShiftResource1"></asp:RequiredFieldValidator>
			</div>
		</div>

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lblDeptTestSM" runat="server" Text="Department" meta:resourcekey="lblDeptTestSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lblDeptTestXS" runat="server" Text="Body Part" meta:resourcekey="lblDeptTestXSResource1"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlDeptTest" Skin="Metro" ZIndex="9000" ExpandDirection="Up" DropDownHeight="300px" DropDownWidth="360px" CssClass="WarnIfChanged" Width="360px" runat="server" meta:resourcekey="rddlDeptTestResource1"></telerik:RadDropDownList>
			</div>
		</div>

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbInvolvedPersonSM" runat="server" Text="Involved Person's Name" meta:resourcekey="lbInvolvedPersonSMResource1"></asp:Label><span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInvolvedPersonXS" runat="server" Text="Involved Person's Name" meta:resourcekey="lbInvolvedPersonXSResource1"></asp:Label><span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadAjaxPanel ID="rajx100" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rajx100Resource1">
						<telerik:RadSearchBox ID="rsbInvolvedPerson" runat="server" MaxResultCount="400" DataKeyNames="PersonId" Skin="Metro" OnSearch="rsbInvolvedPerson_Search"
							ShowSearchButton="False" EmptyMessage="Begin typing (or spacebar)" Width="276px" meta:resourcekey="rsbInvolvedPersonResource1" >
							<DropDownSettings Height="320px" Width="510px">
								<ItemTemplate>
									<table cellpadding="0" cellspacing="0" class="searchBoxResults" style="margin-left: 5px;" width="500">
										<tr>
											<td style="background: #EEEAE0; width: 110px;"><b><%# DataBinder.Eval(Container.DataItem, "PersonName") %></b></td>
											<td style="background: #fff; width: 200px;"><b><%# DataBinder.Eval(Container.DataItem, "PersonEmail") %></b></td>
											<td id="tdPersonID" runat="server" visible="False"><%# DataBinder.Eval(Container.DataItem, "PersonId") %></td>
										</tr>
									</table>
								</ItemTemplate>
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
							</DropDownSettings>
			<%--				<SearchContext DataSourceID="" Enabled="True" TabIndex="0" ShowDefaultItem = "false">
							</SearchContext>--%>
						</telerik:RadSearchBox>
						<span>
							&nbsp;&nbsp;
							<asp:Label ID="lbSupervisorLabel" runat="server" Text="Supervisor: " meta:resourcekey="lbSupervisorLabelResource1"></asp:Label>
							<asp:Label ID="lbSupervisor" runat="server" meta:resourcekey="lbSupervisorResource1" />
						</span>
				</telerik:RadAjaxPanel>
			</div>
		</div>


		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">
					<asp:Label ID="lbInvPersonStatementSM" runat="server" Text="Involved Person's Statement" meta:resourcekey="lbInvPersonStatementSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInvPersonStatementXS" runat="server" Text="Involved Person's Statement" meta:resourcekey="lbInvPersonStatementXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbInvPersonStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server" meta:resourcekey="tbInvPersonStatementResource1"></asp:TextBox>
			</div>
		</div>


		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbSupvInformedDateSM" runat="server" Text="Date Supervisor Informed" meta:resourcekey="lbSupvInformedDateSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbSupvInformedDateXS" runat="server" Text="Date Supervisor Informed" meta:resourcekey="lbSupvInformedDateXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpSupvInformedDate" Skin="Metro" CssClass="WarnIfChanged" Width="278px" runat="server" ShowPopupOnFocus="True" meta:resourcekey="rdpSupvInformedDateResource1">
					<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
					</Calendar>
					<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
						<EmptyMessageStyle Resize="None" />
						<ReadOnlyStyle Resize="None" />
						<FocusedStyle Resize="None" />
						<DisabledStyle Resize="None" />
						<InvalidStyle Resize="None" />
						<HoveredStyle Resize="None" />
						<EnabledStyle Resize="None" />
					</DateInput>
					<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
				</telerik:RadDatePicker>
			</div>
		</div>


		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">
					<asp:Label ID="lbSupervisorStatementSM" runat="server" Text="Supervisor's Statement" meta:resourcekey="lbSupervisorStatementSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbSupervisorStatementXS" runat="server" Text="Supervisor's Statement" meta:resourcekey="lbSupervisorStatementXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="tbSupervisorStatement" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server" meta:resourcekey="tbSupervisorStatementResource1"></asp:TextBox>
			</div>
		</div>


		<asp:Repeater runat="server" ID="rptWitness" ClientIDMode="AutoID" OnItemDataBound="rptWitness_OnItemDataBound" OnItemCommand="rptWitness_ItemCommand">
			<FooterTemplate>
				<br class="visible-xs" style="padding-top: 5px;" />
				<div class="row">
					<div class="col-sm-4 text-left tanLabelCol">
						<asp:Button ID="btnAddWitness" CssClass="buttonAdd" runat="server" Font-Size="Smaller" ToolTip="Add Another Witness" Text="Add Another Witness" Style="margin: 7px;" CommandArgument="AddAnother" meta:resourcekey="btnAddWitnessResource1"></asp:Button>
					</div>
					<div class="col-xs-12 col-sm-8 text-left"></div>
				</div>
			</FooterTemplate>
			<ItemTemplate>
				<div class="row text-left">
					<div class="col-sm-4 hidden-xs text-left tanLabelColHigh" style="height: 100px;">
						<span class="labelMultiLineText">
						<asp:Label ID="lbWitnessColSM" runat="server" meta:resourcekey="lbWitnessColSMResource1" Text="Witness "></asp:Label>
						<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1"></asp:Label>
						<asp:Label ID="lbRqd1" runat="server" CssClass="requiredCloseStarFloat" meta:resourcekey="lbRqd1Resource1" Text="*"></asp:Label>
						</span>
					</div>
					<div class="col-xs-12 visible-xs text-left-more">
						<br />
						<br />
						<span>
						<asp:Label ID="lbWitnessColXS" runat="server" meta:resourcekey="lbWitnessColXSResource1" Text="Witness "></asp:Label>
						<asp:Label ID="lbItemSeq2" runat="server" meta:resourcekey="lbItemSeq2Resource1"></asp:Label>
						<asp:Label ID="lbRqd2" runat="server" CssClass="requiredCloseStar" meta:resourcekey="lbRqd2Resource1" Text="*"></asp:Label>
						</span>
					</div>
					<div class="col-xs-12 col-sm-8 text-left greyControlCol" style="height: 100px; padding-bottom: 4px; padding-top: 7px;">
						<div class="row">
							<div class="col-xs-12 col-sm-4 text-left">
								<asp:Label ID="lbWitNamePrompt" runat="server" meta:resourcekey="lbWitNamePromptResource1" Text="Name: "></asp:Label>
								&nbsp;&nbsp;
								<telerik:RadAjaxPanel ID="rajx200" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rajx200Resource1">
									<telerik:RadSearchBox ID="rsbWitnessName" runat="server" CssClass="NoBorders" DataKeyNames="PersonId" EmptyMessage="Begin typing (or spacebar)" MaxResultCount="400" 
										meta:resourcekey="rsbWitnessNameResource1" OnSearch="rsbWitnessName_Search" ShowSearchButton="False" Skin="Metro" Width="100%">
										<DropDownSettings Height="320px" Width="510px">
											<ItemTemplate>
												<table cellpadding="0" cellspacing="0" class="searchBoxResults" style="margin-left: 5px;" width="500">
													<tr>
														<td style="background: #EEEAE0; width: 110px;"><b><%# DataBinder.Eval(Container.DataItem, "PersonName") %></b></td>
														<td style="background: #fff; width: 200px;"><b><%# DataBinder.Eval(Container.DataItem, "PersonEmail") %></b></td>
														<td id="tdPersonID" runat="server" visible="False"><%# DataBinder.Eval(Container.DataItem, "PersonId") %></td>
													</tr>
												</table>
											</ItemTemplate>
											<HeaderTemplate>
												<table cellpadding="0" cellspacing="1" class="searchBoxResults" style="margin-left: 5px;" width="500">
													<tr>
														<th style="width: 110px; text-align: left;">Name </th>
														<th style="width: 200px; text-align: left;">Email </th>
														<th></th>
													</tr>
												</table>
											</HeaderTemplate>
										</DropDownSettings>
			<%--							<SearchContext DataSourceID="" Enabled="False" TabIndex="0" ShowDefaultItem = "False">
										</SearchContext>--%>
									</telerik:RadSearchBox>
								</telerik:RadAjaxPanel>
							</div>
							<div class="col-xs-12 col-sm-4 text-left">
								<asp:Label ID="lbWitStmntPrompt" runat="server" meta:resourcekey="lbWitStmntPromptResource1" Text="Statement:"></asp:Label>
								&nbsp;&nbsp;
								<asp:TextBox ID="tbWitnessStatement" runat="server" Height="60px" meta:resourcekey="tbWitnessStatementResource1" SkinID="Metro" TextMode="MultiLine" Width="100%"></asp:TextBox>
							</div>
							<div class="col-xs-12 col-sm-3 text-left">
								<span style="display: inline-block; padding-top:3px;">
								<telerik:RadButton ID="btnItemDelete" runat="server" BorderStyle="None" ButtonType="LinkButton" CommandArgument="Delete" CssClass="buttonWrapText" ForeColor="DarkRed" Height="10px" meta:resourcekey="btnItemDeleteResource1" OnClientClicking="DeleteConfirmItem" SingleClick="True" Width="100%">
								</telerik:RadButton>
								</span>
							</div>
						</div>
					</div>
				</div>
				<br class="visible-xs" style="padding-top: 5px;" />
			</ItemTemplate>
		</asp:Repeater>


		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbInsideOutsideSM" runat="server" Text="Inside or Outside Building" meta:resourcekey="lbInsideOutsideSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInsideOutsideXS" runat="server" Text="Inside or Outside Building" meta:resourcekey="lbInsideOutsideXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoInside" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" meta:resourcekey="rdoInsideResource1">
						<asp:ListItem Value="1" Text="Inside&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource1"></asp:ListItem>
						<asp:ListItem Value="0" Text="Outside" meta:resourcekey="ListItemResource2"></asp:ListItem>
					</asp:RadioButtonList></span>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbDirectSupvSM" runat="server" Text="Directly Supervised by AAM" meta:resourcekey="lbDirectSupvSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbDirectSupvXS" runat="server" Text="Directly Supervised by AAM" meta:resourcekey="lbDirectSupvXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoDirectSupv" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" meta:resourcekey="rdoDirectSupvResource1">
						<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource3"></asp:ListItem>
						<asp:ListItem Value="0" Text="<%$ Resources:LocalizedText, No %>"></asp:ListItem>
					</asp:RadioButtonList></span>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbErgConcernSM" runat="server" Text="Ergonomic Concerns" meta:resourcekey="lbErgConcernSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbErgConcernXS" runat="server" Text="Ergonomic Concerns" meta:resourcekey="lbErgConcernXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoErgConcern" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" meta:resourcekey="rdoErgConcernResource1">
						<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource5"></asp:ListItem>
						<asp:ListItem Value="0" Text="<%$ Resources:LocalizedText, No %>"></asp:ListItem>
					</asp:RadioButtonList></span>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 40px;">
				<span>
					<asp:Label ID="lbStdProcsFollowedSM" runat="server" Text="Standard Work Procedures Followed without Deviation?" meta:resourcekey="lbStdProcsFollowedSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more" style="height: 40px;">
				<br />
				<span>
					<asp:Label ID="lbStdProcsFollowedXS" runat="server" Text="Standard Work Procedures Followed without Deviation?" meta:resourcekey="lbStdProcsFollowedXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol" style="height: 40px;">
				<span>
					<asp:RadioButtonList ID="rdoStdProcsFollowed" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" meta:resourcekey="rdoStdProcsFollowedResource1">
						<asp:ListItem Value="1" Text="Standard&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource7"></asp:ListItem>
						<asp:ListItem Value="0" Text="Non-Standard" meta:resourcekey="ListItemResource8"></asp:ListItem>
					</asp:RadioButtonList></span>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbTrainingProvidedSM" runat="server" Text="Was Training for this Task Provided?" meta:resourcekey="lbTrainingProvidedSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbTrainingProvidedXS" runat="server" Text="Was Training for this Task Provided?" meta:resourcekey="lbTrainingProvidedXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 greyControlCol">
				<span>
					<asp:RadioButtonList ID="rdoTrainingProvided" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" meta:resourcekey="rdoTrainingProvidedResource1">
						<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource9"></asp:ListItem>
						<asp:ListItem Value="0" Text="<%$ Resources:LocalizedText, No %>"></asp:ListItem>
					</asp:RadioButtonList></span>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 40px;">
				<span>
					<asp:Label ID="lbTaskYearsSM" runat="server" Text="How long has associate been doing this job/specific task?" meta:resourcekey="lbTaskYearsSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more" style="height: 40px;">
				<br />
				<span>
					<asp:Label ID="lbTaskYearsXS" runat="server" Text="How long has associate been doing this job/specific task?" meta:resourcekey="lbTaskYearsXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol" style="height: 40px;">
				<telerik:RadDropDownList ID="rddlJobTenure" Skin="Metro" ZIndex="9000" ExpandDirection="Up"  Width="278px" DropDownHeight="300px" runat="server" meta:resourcekey="rddlJobTenureResource1"></telerik:RadDropDownList>
			</div>
		</div>

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbInjuryTypeSM" runat="server" Text="Type of Injury" meta:resourcekey="lbInjuryTypeSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbInjuryTypeXS" runat="server" Text="Type of Injury" meta:resourcekey="lbInjuryTypeXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlInjuryType" Skin="Metro" ZIndex="9000" ExpandDirection="Up"  Width="278px" DropDownHeight="300px" runat="server" meta:resourcekey="rddlInjuryTypeResource1"></telerik:RadDropDownList>
			</div>
		</div>

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>
					<asp:Label ID="lbBodyPartSM" runat="server" Text="Body Part" meta:resourcekey="lbBodyPartSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>
					<asp:Label ID="lbBodyPartXS" runat="server" Text="Body Part" meta:resourcekey="lbBodyPartXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlBodyPart" Skin="Metro" ZIndex="9000" ExpandDirection="Up" DropDownHeight="300px"  Width="278px" runat="server" meta:resourcekey="rddlBodyPartResource1"></telerik:RadDropDownList>
			</div>
		</div>

		<telerik:RadAjaxPanel ID="rapSeverity" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapSeverityResource1">
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol">
					<span>
						<asp:Label ID="lbFirstAidSM" runat="server" Text="First Aid?" meta:resourcekey="lbFirstAidSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbFirstAidXS" runat="server" Text="First Aid?" meta:resourcekey="lbFirstAidXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 greyControlCol">
					<span>
						<asp:RadioButtonList ID="rdoFirstAid" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" OnSelectedIndexChanged="Severity_Changed" AutoPostBack="True" meta:resourcekey="rdoFirstAidResource1">
							<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource11"></asp:ListItem>
							<asp:ListItem Value="0" Text="<%$ Resources:LocalizedText, No %>"></asp:ListItem>
						</asp:RadioButtonList></span>
				</div>
			</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol">
					<span>
						<asp:Label ID="lbRecordableSM" runat="server" Text="Recordable?" meta:resourcekey="lbRecordableSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbRecordableXS" runat="server" Text="Recordable?" meta:resourcekey="lbRecordableXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 greyControlCol">
					<span>
						<asp:RadioButtonList ID="rdoRecordable" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" OnSelectedIndexChanged="Severity_Changed" AutoPostBack="True" meta:resourcekey="rdoRecordableResource1">
							<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource13"></asp:ListItem>
							<asp:ListItem Value="0" Text="<%$ Resources:LocalizedText, No %>"></asp:ListItem>
						</asp:RadioButtonList></span>
				</div>
			</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol">
					<span>
						<asp:Label ID="lbFatalitySM" runat="server" Text="Fatality?" meta:resourcekey="lbFatalitySMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbFatalityXS" runat="server" Text="Fatality?" meta:resourcekey="lbFatalityXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 greyControlCol">
					<span>
						<asp:RadioButtonList ID="rdoFatality" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" runat="server" OnSelectedIndexChanged="Severity_Changed" AutoPostBack="True" meta:resourcekey="rdoFatalityResource1">
							<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource15"></asp:ListItem>
							<asp:ListItem Value="0" Text="<%$ Resources:LocalizedText, No %>"></asp:ListItem>
						</asp:RadioButtonList></span>
				</div>
			</div>
			<div class="row">
				<div class="col-sm-4 hidden-xs text-left tanLabelCol">
					<span>
						<asp:Label ID="lbLostTimeSM" runat="server" Text="Lost Or Restricted Time?" meta:resourcekey="lbLostTimeSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbLostTimeXS" runat="server" Text="Lost Or Restricted Time?" meta:resourcekey="lbLostTimeXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 greyControlCol">
					<span>
						<asp:RadioButtonList ID="rdoLostTime" CssClass="radioListHorizontal" RepeatColumns="2" RepeatDirection="Horizontal" AutoPostBack="True" runat="server" meta:resourcekey="rdoLostTimeResource1">
							<asp:ListItem Value="1" Text="Yes&nbsp;&nbsp;&nbsp;&nbsp;" meta:resourcekey="ListItemResource17"></asp:ListItem>
							<asp:ListItem Value="0" Text="No" meta:resourcekey="ListItemResource18"></asp:ListItem>
						</asp:RadioButtonList></span>
				</div>
			</div>
			<asp:Panel ID="pnlExpReturnDT" runat="server" Visible="False" meta:resourcekey="pnlExpReturnDTResource1">
				<div class="row">
					<div class="col-sm-4 hidden-xs text-left tanLabelCol">
						<span>
							<asp:Label ID="lbExpectReturnDTSM" runat="server" Text="Expected Return Date" meta:resourcekey="lbExpectReturnDTSMResource1"></asp:Label><span class="requiredCloseStarFloat">*</span></span>
					</div>
					<div class="col-xs-12 visible-xs text-left-more">
						<br />
						<span>
							<asp:Label ID="lbExpectReturnDTXS" runat="server" Text="Expected Return Date" meta:resourcekey="lbExpectReturnDTXSResource1"></asp:Label><span class="requiredCloseStar">*</span></span>
					</div>
					<div class="col-xs-12 col-sm-8 text-left greyControlCol">
						<telerik:RadDatePicker ID="rdpExpectReturnDT" Skin="Metro" CssClass="WarnIfChanged" Width="278px" runat="server" ShowPopupOnFocus="True" meta:resourcekey="rdpExpectReturnDTResource1">
							<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
							</Calendar>
							<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
								<EmptyMessageStyle Resize="None" />
								<ReadOnlyStyle Resize="None" />
								<FocusedStyle Resize="None" />
								<DisabledStyle Resize="None" />
								<InvalidStyle Resize="None" />
								<HoveredStyle Resize="None" />
								<EnabledStyle Resize="None" />
							</DateInput>
							<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
						</telerik:RadDatePicker>
					</div>
				</div>
			</asp:Panel>
		</telerik:RadAjaxPanel>

		<telerik:RadAjaxPanel ID="rapAttach" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapAttachResource1">
			<div class="row">
				<div id="dvAttachLbl" runat="server" class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 128px;">
					<span>
						<asp:Label ID="lbAttachemntSM" runat="server" Text="<%$ Resources:LocalizedText, Attachments %>"></asp:Label></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>
						<asp:Label ID="lbAttachemntXS" runat="server" Text="<%$ Resources:LocalizedText, Attachments %>"></asp:Label></span>
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

<asp:Panel ID="pnlBaseForm2" Visible="False" runat="server" meta:resourcekey="pnlBaseForm2Resource1">

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



<asp:Panel ID="pnlButtons" runat="server" meta:resourcekey="pnlButtonsResource1">


	<div class="container-fluid">


		<div class="row">
			<div class="col-xs-12" style="padding: 5px">
				<asp:Label ID="lblResults" runat="server" ForeColor="ForestGreen" Font-Bold="True" CssClass="textStd" meta:resourcekey="lblResultsResource1"></asp:Label>
			</div>
		</div>
		<br class="visible-xs-block" />


		<div class="row">
			<div class="col-xs-12 text-left ">
				<span style="margin-top: 5px;">
					<telerik:RadButton ID="btnSubnavSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" Style="margin-right: 10px;"
						OnClick="btnSubnavSave_Click" CommandArgument="0"/>
					<asp:LinkButton ID="btnSubnavIncident" runat="server" Text="<%$ Resources:LocalizedText, Incident %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="0" />
					<asp:LinkButton ID="btnSubnavLostTime" runat="server" Text="Lost time History" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="6" meta:resourcekey="btnSubnavLostTimeResource1"/>
					<asp:LinkButton ID="btnSubnavContainment" runat="server" Text="<%$ Resources:LocalizedText, InitialAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="2"/>
					<asp:LinkButton ID="btnSubnavRootCause" runat="server" Text="<%$ Resources:LocalizedText, RootCause %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="3"/>
					<asp:LinkButton ID="btnSubnavAction" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="4"/>
					<asp:LinkButton ID="btnSubnavApproval" runat="server" Text="<%$ Resources:LocalizedText, Approvals %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
						OnClick="btnSubnav_Click" CommandArgument="5"/>
					<span style="float:right">
						<telerik:RadButton ID="btnDeleteInc" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="False" ForeColor="DarkRed"
							Text="<%$ Resources:LocalizedText, DeleteIncident %>" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
							OnClientClicking="function(sender,args){RadConfirmAction(sender, args, 'Delete this Incident');}" OnClick="btnDeleteInc_Click" CssClass="UseSubmitAction" />
					</span>
				</span>
			</div>
		</div>

		<br />

	</div>

</asp:Panel>
