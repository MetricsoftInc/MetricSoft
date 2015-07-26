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
				<span>Incident Date:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Incident Date:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpIncidentDate" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDatePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentDate" ControlToValidate="rdpIncidentDate" Display="None" ErrorMessage="Required" ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- REPORT DATE question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>Report Date:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Report Date:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="rdpReportDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="278" runat="server"></telerik:RadDatePicker>
			</div>
		</div>


		<%-- LOCATION question --%>	

		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>Location:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Location:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlLocation" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server" OnSelectedIndexChanged="rddlLocation_SelectedIndexChanged"></telerik:RadDropDownList>
				<asp:RequiredFieldValidator runat="server" ID="rfvLocation" ControlToValidate="rddlLocation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>
		

		
		<%-- DESCRIPTION question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">Description:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Description:&nbsp;<span class="requiredStar">*</span></span>
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
					<span class="labelMultiLineText">Local Description:<span class="requiredStarFloat">*</span></span>
				</div>
				<div class="col-xs-12 visible-xs text-left-more">
					<br />
					<span>Local Description:&nbsp;<span class="requiredStar">*</span></span>
				</div>
				<div class="col-xs-12 col-sm-8 text-left greyControlCol">
					<asp:TextBox ID="tbLocalDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
					<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="tbDescription" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
				</div>
			</div>
		</asp:Panel>



		<%-- TIME OF INCIDENT question --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>Time of Incident:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Time of Incident:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadTimePicker ID="rtpIncidentTime" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadTimePicker>
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentTime" ControlToValidate="rtpIncidentTime" Display="None" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- SHIFT question --%>	
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>Shift:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Shift:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDropDownList ID="rddlShift" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rvfShift" ControlToValidate="rddlShift" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>


		<%-- PRODUCTION IMPACT question (MultiLine TEXTBOX) --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelColHigh">
				<span class="labelMultiLineText">Production Impact:<span class="requiredCloseStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Production Impact:&nbsp;<span class="requiredCloseStar">*</span></span>
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
						<span><b>Action</b></span>
					</div>

					<div class="col-sm-3  text-center">
						<span><b>Assigned To</b></span>
					</div>

					<div class="col-sm-1 text-left-more">
						<span><b>Start Date</b></span>
					</div>

					<div class="col-sm-2 text-center">
						<span style="padding-left:10px;"><b>&nbsp;&nbsp;Completion Date</b></span>
					</div>

				</div>
				<br />
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row-fluid">

					<div class="col-xs-12  col-sm-3 text-left-more">
						<span >Action&nbsp;<asp:Label ID="lblItemSeq" runat="server"></asp:Label>:&nbsp;
						<asp:TextBox ID="tbContainAction" Width="275" SkinID="Metro" runat="server"></asp:TextBox></span>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<asp:TextBox ID="tbContainPerson" Width="200" SkinID="Metro" runat="server"></asp:TextBox>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more"> 
						<telerik:RadDatePicker ID="rdpStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="175" runat="server"></telerik:RadDatePicker>
					</div>

					<div class="col-xs-12 col-sm-2 text-left-more">
						<telerik:RadDatePicker ID="rdpCompleteDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="false"  Width="175" runat="server"></telerik:RadDatePicker>
					</div>


					<div class="col-xs-12  col-sm-1 text-left-more">
						<span style="padding-bottom:0"></span><asp:CheckBox ID="cbIsComplete" runat="server" Text="Complete" SkinID="Metro" TextAlign="Right"></asp:CheckBox></span>
					</div>

						<br  />

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
						<span>Why&nbsp;<asp:Label ID="lblItemSeq" runat="server"></asp:Label>:</span>
					</div>
					<div class="col-xs-12 text-left">
						<asp:TextBox ID="tbRootCause" Rows="5" Height="95px" Width="50%" TextMode="MultiLine" SkinID="Metro" runat="server"></asp:TextBox>
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

		<%-- TESTING --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>Due Date:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Due Date:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<telerik:RadDatePicker ID="RadDatePicker2" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDatePicker>
		        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="RadDatePicker1" Display="None" ErrorMessage="Required" ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>
	</div>

</asp:Panel>


<asp:Panel ID="pnlApproval" Visible="false" runat="server">

	<br />

	<div class="container-fluid">

		<%-- TESTING --%>
		<div class="row">
			<div class="col-sm-4 hidden-xs text-left tanLabelCol">
				<span>Safety Manager:<span class="requiredStarFloat">*</span></span>
			</div>
			<div class="col-xs-12 visible-xs text-left-more">
				<br />
				<span>Safety Manager:&nbsp;<span class="requiredStar">*</span></span>
			</div>
			<div class="col-xs-12 col-sm-8 text-left greyControlCol">
				<asp:TextBox ID="TextBox2" Width="50px" SkinID="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="TextBox2" Display="None" ErrorMessage="Required" ValidationGroup="Val_PowerOutage"></asp:RequiredFieldValidator>
			</div>
		</div>
	</div>

</asp:Panel>


<asp:Panel ID="pnlButtons" Visible="true" runat="server">


<div class="container-fluid">


		<div class="row">
			<div class="col-xs-12" style="padding: 5px">
				<asp:Label ID="lblResults" runat="server" Font-Bold="true" CssClass="textStd"></asp:Label>
			</div>
		</div>

<%--		<div class="clearfix visible-xs"></div>--%>
		<br class="visible-xs-block" />

		<div class="row">

			<div class="col-xs-12 text-left ">

				<span><telerik:RadButton ID="btnSave" runat="server" Text="Save" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnSave_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_PowerOutage" />&nbsp;&nbsp;</span>

				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />


				<span><telerik:RadButton ID="btnPrev" runat="server" Text="<   Prev" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnPrev_Click" />&nbsp;&nbsp;</span>
	
				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />

	
				<span><telerik:RadButton ID="btnNext" runat="server" Text="Next   >" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					OnClick="btnNext_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val_PowerOutage" /></span>

				
				<span><telerik:RadButton ID="btnClose" runat="server" Text="Close Incident" Visible="false" CssClass="UseSubmitAction" Skin="Metro" 
					 OnClientClicking="StandardConfirm" ValidationGroup="Val_PowerOutage" /></span>


			</div>
		</div>
		
 </div>
	
	


 </asp:Panel>




