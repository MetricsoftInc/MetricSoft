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

 <asp:Panel ID="pnlSelect" Visible="true" runat="server">

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
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentDate" ControlToValidate="rdpIncidentDate" Display="None" ErrorMessage="Required" ValidationGroup="Val"></asp:RequiredFieldValidator>
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
				<telerik:RadDropDownList ID="rddlLocation" Skin="Metro" CssClass="WarnIfChanged" Width="278" runat="server"></telerik:RadDropDownList>
		        <asp:RequiredFieldValidator runat="server" ID="rfvLocation" ControlToValidate="rddlLocation" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val"></asp:RequiredFieldValidator>
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
				<asp:TextBox ID="tbDescription" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" Skin="Metro" runat="server"></asp:TextBox>
		        <asp:RequiredFieldValidator runat="server" ID="rfvDescription" ControlToValidate="tbDescription" Display="None" ErrorMessage="Required"  ValidationGroup="Val"></asp:RequiredFieldValidator>
			</div>
		</div>


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
		        <asp:RequiredFieldValidator runat="server" ID="rfvIncidentTime" ControlToValidate="rtpIncidentTime" Display="None" ErrorMessage="Required"  ValidationGroup="Val"></asp:RequiredFieldValidator>
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
		        <asp:RequiredFieldValidator runat="server" ID="rvfShift" ControlToValidate="rddlShift" Display="None" InitialValue="[Select One]" ErrorMessage="Required"  ValidationGroup="Val"></asp:RequiredFieldValidator>
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
				<asp:TextBox ID="tbProdImpact" Rows="5" Height="95px" Width="75%" TextMode="MultiLine" Skin="Metro" runat="server"></asp:TextBox>
			</div>
		</div>

	
	<br />
<%--	<asp:GridView runat="server" ID="gvPreventLocationsList" Name="gvPreventLocationsList"
		CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" CellPadding="5"
		GridLines="Both" PageSize="20" AllowSorting="false" Width="100%" OnRowDataBound="gvPreventLocationsList_RowDataBound"
		DataKeyNames="PLANT_ID">
		<HeaderStyle CssClass="HeadingCellText" />
		<RowStyle CssClass="DataCell" />
		<Columns>
			<asp:TemplateField HeaderText="Select People to Send Verification Notification<br/>(Drag or Ctrl+click to select multiple)">
				<ItemTemplate>
					<div style="float: left; font-size: 13px; padding: 8px 0 10px 0;">
						<asp:Label ID="lblPlant" runat="server" Text='<%#Eval("PLANT_NAME")%>' ClientIDMode="AutoID"
							Font-Bold="true"></asp:Label>
					</div>
					<div style="float: right;">
						<telerik:RadButton ID="rbSelectAll" runat="server" Text="Select All" Skin="Metro" AutoPostBack="false"
							 OnClientClicked="PVOnClientClicked">
						</telerik:RadButton>
					</div>
					<br style="clear: both;" />
					<telerik:RadGrid runat="server" ID="rgPlantContacts" Name="rgPlantContacts" 
						Skin="Metro" AllowMultiRowSelection="true" AutoGenerateColumns="false" CellPadding="3"
						GridLines="None" AllowSorting="false" ShowHeader="false" Width="100%">
						<ClientSettings EnableRowHoverStyle="true">
							<Selecting AllowRowSelect="True"></Selecting>
							<ClientEvents OnRowSelected="PVRowSelectedChanged" OnRowDeselected="PVRowSelectedChanged" />
						</ClientSettings>
						<MasterTableView DataKeyNames="PERSON_ID" ExpandCollapseColumn-Visible="false">
							<Columns>
								<telerik:GridTemplateColumn ItemStyle-Width="45%">
									<ItemTemplate>
										<asp:Label ID="lblContact" runat="server" Font-Size="8" Text='<%#Capitalize((string)Eval("FIRST_NAME")) + " " + Capitalize((string)Eval("LAST_NAME")) %>'></asp:Label>
										<div style="float: right;">
											<asp:Label ID="lblConfirmed" runat="server" Font-Size="8" ForeColor="Red" BackColor="Wheat" BorderColor="Wheat" BorderWidth="2" Text="Confirmed" Visible="false"></asp:Label>
										</div>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn ItemStyle-Width="45%">
									<ItemTemplate>
										<asp:Label ID="lblEmail" runat="server" Font-Size="8" Text='<%#Eval("JOB_TITLE")%>'></asp:Label>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridClientSelectColumn ItemStyle-Width="10%">
								</telerik:GridClientSelectColumn>
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>

					<asp:Panel ID="pnlComments" runat="server" Visible="false">
						<div style="padding: 2px 7px 7px;">
						<p>Comments:</p>
						<telerik:RadGrid runat="server" ID="rgPlantComments" Name="rgPlantComments" 
							Skin="Metro" AutoGenerateColumns="false" CellPadding="3"
							GridLines="None" AllowSorting="false" ShowHeader="false" Width="100%">
							<MasterTableView ExpandCollapseColumn-Visible="false">
								<Columns>
									<telerik:GridTemplateColumn ItemStyle-Width="45%">
										<ItemTemplate>
											<asp:Label ID="lblContact" runat="server" Font-Size="8" Text='<%#Eval("PersonName")%>'></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn ItemStyle-Width="45%">
										<ItemTemplate>
											<asp:Label ID="lblComment" runat="server" Font-Size="8" Text='<%#Eval("CommentText")%>'></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn ItemStyle-Width="45%">
										<ItemTemplate>
											<asp:Label ID="lblComment" runat="server" Font-Size="8" Text='<%#((DateTime)Eval("CommentDate")).ToShortDateString()%>'></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
								</Columns>
								</MasterTableView>
							</telerik:RadGrid>
							</div>
						</asp:Panel>
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>--%>

	<br />

		<div class="row">

			<div class="col-xs-12 text-left ">

				<span><telerik:RadButton ID="btnSave" runat="server" Text="Save" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					SingleClick="true" SingleClickText="Saving..." OnClick="btnSave_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />&nbsp;&nbsp;</span>

				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />


				<span><telerik:RadButton ID="btnPrev" runat="server" Text="<   Prev" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					SingleClick="true" OnClick="btnPrev_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />&nbsp;&nbsp;</span>
	
				<div class="clearfix visible-xs"></div>
				<br class="visible-xs-block" />

	
				<span><telerik:RadButton ID="btnNext" runat="server" Text="Next   >" Visible="true" CssClass="UseSubmitAction" Skin="Metro" 
					SingleClick="true" OnClick="btnNext_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" /></span>


			</div>
		

	
	<%--<asp:Label ID="lblResults" runat="server" />--%>

	</div>

 </asp:Panel>



