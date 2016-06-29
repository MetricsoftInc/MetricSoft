<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Alert.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Alert" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<script type="text/javascript">

	window.onload = function () {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate')) {
			document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate').value = "0";
		}
	}
	window.onbeforeunload = function () {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate')) {
			if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate').value == '1') {
				return 'You have unsaved changes on this page.';
			}
		}
		else {
			return 'cannot resolve changeupdate.';
		}
	}

	function ChangeUpdate(sender, args) {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate')) {
			document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate').value = '1';
		}
		return true;
	}

	function ChangeClear(sender, args) {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate')) {
			document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclAlert_hfChangeUpdate').value = '0';
		}
	}

</script>


<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

<asp:Panel ID="pnlAlert" Visible="false" runat="server">

	<div class="container-fluid" style="margin-top: 8px;">
		<asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Advise other business locations where similar processes are performed or situations exist, of the preventative measures to be taken to mitigate further occurences of this incident. Specify the user groups to receive notifications and the group who will be responsible for verifying and confirming that preventative measures have been implemented in their facility ..."></asp:Label>
		<br /><br />
		<telerik:RadAjaxPanel ID="rapAlert" runat="server" HorizontalAlign="NotSet">

			<table width="100%" border="0"  class="lightTable">
				<tr>
					<td class="columnHeader" width="25%">
						<asp:Label ID="lblLocations" runat="server" text="Locations To Alert"></asp:Label>
					</td>
					<td class="required" width="1%">&nbsp;</td>
					<td class="tableDataAlt" width="74%">
						<telerik:RadComboBox ID="ddlLocations" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex=9000 Skin="Metro" height="300px" Width="98%" OnClientLoad="DisableComboSeparators" OnClientSelectedIndexChanged="ChangeUpdate"></telerik:RadComboBox>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblAlertDesc" runat="server" text="Similar / Affected Processes"></asp:Label>
					</td>
					<td class="required" >&nbsp;</td>
					<td class="tableDataAlt" >
						<asp:TextBox ID="tbAlertDesc" runat="server" Height="65px" Rows="4" SkinID="Metro" TextMode="MultiLine" Width="98%"  onChange="ChangeUpdate()"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblComments" runat="server" text="Implementation / Verficiation Recommendations"></asp:Label>
					</td>
					<td class="tableDataAlt" >&nbsp;</td>
					<td class="tableDataAlt" >
						<asp:TextBox ID="tbComments" runat="server" Height="65px" Rows="4" SkinID="Metro" TextMode="MultiLine" Width="98%"  onChange="ChangeUpdate()"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblNotifyGroupHdr" runat="server" text="Notify User Group(s)"></asp:Label>
					</td>
					<td class="required" >&nbsp;</td>
					<td class="tableDataAlt" >
						<telerik:RadComboBox ID="ddlNotifyGroup" runat="server"  Visible="false" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex=9000 Skin="Metro" height="300px" Width="98%" OnClientSelectedIndexChanged="ChangeUpdate"></telerik:RadComboBox>
						<asp:Label ID="lblNotifyGroup" runat="server" Visible="true" Text=""></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblResponsibleGroup" runat="server" text="Implementation Responsibility"></asp:Label>
					</td>
					<td class="required" >&nbsp;</td>
					<td class="tableDataAlt" >
						<telerik:RadComboBox ID="ddlResponsibleGroup" runat="server" CheckBoxes="False" ZIndex=9000 Skin="Metro" height="300px" Width="350" OnClientSelectedIndexChanged="ChangeUpdate"></telerik:RadComboBox>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblDueDate" runat="server" text="Implementation Due Date"></asp:Label>
					</td>
					<td class="required" >&nbsp;</td>
					<td class="tableDataAlt" >
						<telerik:RadDatePicker ID="rdpDueDate" runat="server" ShowPopupOnFocus="True" Skin="Metro" Width="125">
							<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
							</Calendar>
							<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="" OnClientDateChanged="ChangeUpdate">
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
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblAlertStatusHdr" runat="server" text="Implementation Status"></asp:Label>
					</td>
					<td class="tableDataAlt" >&nbsp;</td>
					<td class="tableDataAlt" >
						<asp:Label ID="lblAlertStatus" runat="server" CssClass="labelEmphasis"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="columnHeader" style="vertical-align: top;">
						<asp:Label ID="lblAlertList" runat="server" text="Parallel Development"></asp:Label>
						<br /><br />
						<span>
							<asp:LinkButton ID="lnkCreateTasks" runat="server"  ToolTip="Assign tasks" CSSClass="buttonRefresh"  text="Assign"  OnClick="lnkCreateTasks_Click" ></asp:LinkButton>
							&nbsp;
							<asp:Label ID="lblCreateTasks" runat="server" CssClass="instructText" Text="responsibility for implementing preventative actions at each business location selected to receive this Incident alert "></asp:Label>
						</span>
					</td>
					<td class="tableDataAlt" >&nbsp;</td>
					<td class="tableDataAlt" >
						<telerik:RadGrid ID="rgAlertTaskList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="False" 
							AutoGenerateColumns="False" OnItemDataBound="rgAlertTaskList_ItemDataBound" Width="100%" GroupPanelPosition="Top">
							<MasterTableView>
								<ExpandCollapseColumn Visible="False">
								</ExpandCollapseColumn>
								<Columns>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" meta:resourcekey="GridTemplateColumnResource1" HeaderText="<%$ Resources:LocalizedText, BusinessLocation %>" UniqueName="AlertID">
										<ItemTemplate>
											<table class="innerTable">
												<tr>
													<td>
														<asp:Label ID="lblLocation" runat="server" CssClass="prompt"></asp:Label>
													</td>
												</tr>
											</table>
											<asp:HiddenField ID="hfLocation" runat="server" Value='<%# Eval("RECORD_SUBID") %>'/>
											<asp:HiddenField id="hfLocationTZ" runat="server"/>
											<asp:HiddenField ID="hfTaskID" runat="server" />
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn2 column" HeaderText="<%$ Resources:LocalizedText, PersonAccountable %>" ShowSortIcon="False" UniqueName="TemplateColumn2" ItemStyle-Width="220">
										<ItemTemplate>
											<telerik:RadComboBox ID="ddlResponsible" runat="server" CheckBoxes="False"  ZIndex=9000 Skin="Metro" height="300px" Width="99%" ></telerik:RadComboBox>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn6 column" HeaderText="<%$ Resources:LocalizedText, Comments %>" UniqueName="TemplateColumn6" ItemStyle-Width="25%">
										<ItemTemplate>
											<asp:Label ID="lblTaskComments" runat="server"></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn5 column" HeaderText="<%$ Resources:LocalizedText, Completed %>" UniqueName="TemplateColumn5">
										<ItemTemplate>
											<asp:Label ID="lblTaskCompleteDT" runat="server"></asp:Label>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
								</Columns>
								<PagerStyle AlwaysVisible="True" />
							</MasterTableView>
							<PagerStyle AlwaysVisible="True"></PagerStyle>
						</telerik:RadGrid>
					</td>
				</tr>
			</table>
			<div class="row" style="margin-top: 10px;">
				<center>
					<span>
						<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Submit %>" CssClass="UseSubmitAction" Skin="Metro" 
							OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Submit %>"/>
					</span>
				</center>
				<asp:Label ID="lblStatusMsg" runat="server" CssClass="labelEmphasis"></asp:Label>
			</div>

		</telerik:RadAjaxPanel>

	</div>

</asp:Panel>

