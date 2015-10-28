<%@ Page Title="Audit Questions" Language="C#" MasterPageFile="~/PSMaster.Master"
	AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Audits_Questions.aspx.cs"
	Inherits="SQM.Website.EHS_Audits_Questions" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<div class="admin_tabs">
		<table width="100%" border="0" cellspacing="0" cellpadding="1">
			<tr>
				<td class="tabActiveTableBg" colspan="10" align="center">
					<br />
					<table width="99%">
						<tr>
							<td class="pageTitles">
								<asp:Label ID="lblViewEHSRezTitle" runat="server" Text="Assessment Questions" meta:resourcekey="lblViewEHSRezTitleResource1"></asp:Label>
								<br />
								<asp:Label ID="lblPageInstructions" runat="server" class="instructText" meta:resourcekey="lblPageInstructionsResource1"></asp:Label>
							</td>
						</tr>
					</table>
					<br />
					<telerik:RadFormDecorator ID="RadFormDecorator1" DecorationZoneID="divPageBody" DecoratedControls="All"
						Skin="Metro" runat="server" meta:resourcekey="RadFormDecorator1Resource1" />
					<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 10px;" runat="server">
						<div class="blueCell" style="padding: 10px; font-size: 13px;">
							<asp:Label class="textStd" AssociatedControlID="ddlAuditType" runat="server" Font-Bold="True" Text="<%$ Resources:LocalizedText, AssessmentType %>" />
							&nbsp;
							<asp:DropDownList ID="ddlAuditType" AutoPostBack="True" OnSelectedIndexChanged="ddlAuditType_SelectedIndexChanged"
								runat="server" meta:resourcekey="ddlAuditTypeResource1" />
						</div>
						<br />
						<table cellpadding="0" cellspacing="0" style="width: 100%;">
							<tr>
								<td style="width: 48%; vertical-align: top;">
									<p>
										<strong>Current Questions</strong></p>
									<telerik:RadGrid ID="rgCurrentQuestions" runat="server" CellSpacing="-1" GridLines="Both" GroupPanelPosition="Top" meta:resourcekey="rgCurrentQuestionsResource1">
										<MasterTableView DataKeyNames="QuestionId" Width="100%" TableLayout="Fixed" AutoGenerateColumns="False" AllowSorting="True" >
											<Columns>
												<telerik:GridDragDropColumn HeaderStyle-Width="18px" Visible="true" meta:resourcekey="GridDragDropColumnResource1">
<HeaderStyle Width="18px"></HeaderStyle>
												</telerik:GridDragDropColumn>
												<telerik:GridBoundColumn DataField="QuestionText" HeaderText="Question Text" meta:resourcekey="GridBoundColumnResource1"></telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="QuestionType" HeaderText="Type" meta:resourcekey="GridBoundColumnResource2"></telerik:GridBoundColumn>
											</Columns>
										</MasterTableView>
										<ClientSettings AllowRowsDragDrop="true">
											<Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
										</ClientSettings>
									</telerik:RadGrid>
								</td>
								<td style="width: 4%;">
								</td>
								<td style="width: 48%; vertical-align: top;">
									<p>
										<strong>Available Questions</strong></p>
									<telerik:RadGrid ID="rgAllQuestions" runat="server" CellSpacing="-1" GridLines="Both" GroupPanelPosition="Top" meta:resourcekey="rgAllQuestionsResource1">
										<MasterTableView DataKeyNames="QuestionId" Width="100%" TableLayout="Fixed" AutoGenerateColumns="False" AllowSorting="True" >
											<Columns>
												<telerik:GridDragDropColumn HeaderStyle-Width="18px" Visible="true" meta:resourcekey="GridDragDropColumnResource2">
<HeaderStyle Width="18px"></HeaderStyle>
												</telerik:GridDragDropColumn>
												<telerik:GridBoundColumn DataField="QuestionText" HeaderText="Question Text" meta:resourcekey="GridBoundColumnResource3"></telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="QuestionType" HeaderText="Type" meta:resourcekey="GridBoundColumnResource4"></telerik:GridBoundColumn>
											</Columns>
										</MasterTableView>
										<ClientSettings AllowRowsDragDrop="true">
											<Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
										</ClientSettings>
									</telerik:RadGrid>
								</td>
							</tr>
						</table>
						<h2>
							<asp:Label ID="lblResults" runat="server" meta:resourcekey="lblResultsResource1" /></h2>
						<br />
						<br />
						<br />
					</div>
				</td>
			</tr>
		</table>
	</div>
</asp:Content>
