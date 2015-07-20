<%@ Page Title="Incident Questions" Language="C#" MasterPageFile="~/PSMaster.Master"
	AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Incidents_Questions.aspx.cs"
	Inherits="SQM.Website.EHS_Incidents_Questions" %>

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
								<asp:Label ID="lblViewEHSRezTitle" runat="server" Text="Incident Questions"></asp:Label>
								<br />
								<asp:Label ID="lblPageInstructions" runat="server"  CssClass="instructText" Text=""></asp:Label>
							</td>
						</tr>
					</table>
					<br />
					<telerik:RadFormDecorator ID="RadFormDecorator1" DecorationZoneID="divPageBody" DecoratedControls="All"
						Skin="Metro" runat="server" />
					<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 10px;" runat="server">
						<div class="blueCell" style="padding: 10px; font-size: 13px;">
							<asp:Label CssClass="textStd" AssociatedControlID="ddlIncidentType" runat="server" Font-Bold="true">Incident Type</asp:Label>
							&nbsp;
							<asp:DropDownList ID="ddlIncidentType" AutoPostBack="true" OnSelectedIndexChanged="ddlIncidentType_SelectedIndexChanged"
								runat="server" />
						</div>
						<br />
						<table cellpadding="0" cellspacing="0" style="width: 100%;">
							<tr>
								<td style="width: 48%; vertical-align: top;">
									<p>
										<strong>Current Questions</strong></p>
									<telerik:RadGrid ID="rgCurrentQuestions" runat="server">
										<MasterTableView DataKeyNames="QuestionId" Width="100%" TableLayout="Fixed" AutoGenerateColumns="False" AllowSorting="True" >
											<Columns>
												<telerik:GridDragDropColumn HeaderStyle-Width="18px" Visible="true">
												</telerik:GridDragDropColumn>
												<telerik:GridBoundColumn DataField="QuestionText" HeaderText="Question Text"></telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="QuestionType" HeaderText="Type"></telerik:GridBoundColumn>
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
									<telerik:RadGrid ID="rgAllQuestions" runat="server">
										<MasterTableView DataKeyNames="QuestionId" Width="100%" TableLayout="Fixed" AutoGenerateColumns="False" AllowSorting="True" >
											<Columns>
												<telerik:GridDragDropColumn HeaderStyle-Width="18px" Visible="true">
												</telerik:GridDragDropColumn>
												<telerik:GridBoundColumn DataField="QuestionText" HeaderText="Question Text"></telerik:GridBoundColumn>
												<telerik:GridBoundColumn DataField="QuestionType" HeaderText="Type"></telerik:GridBoundColumn>
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
							<asp:Label ID="lblResults" runat="server" /></h2>
						<br />
						<br />
						<br />
					</div>
				</td>
			</tr>
		</table>
	</div>
</asp:Content>
