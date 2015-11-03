<%@ Page Title="Incident Verification" Language="C#" MasterPageFile="~/PSMaster.Master"
	AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Incident_Verification.aspx.cs"
	Inherits="SQM.Website.EHS_Incident_Verification" %>

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
								<asp:Label ID="lblViewEHSRezTitle" runat="server" Text="Issue Acknowledgement Notification"></asp:Label>
								<br />
								<asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Please acknowledge notification the incident below, and add any comments if necessary."></asp:Label>
							</td>
						</tr>
					</table>
					<br />
					<telerik:RadFormDecorator ID="RadFormDecorator1" DecorationZoneID="divPageBody" DecoratedControls="All"
						Skin="Metro" runat="server" />
					<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 10px;" runat="server">
						<div class="blueCell" style="padding: 10px; font-size: 13px;">
							<asp:Label ID="lblPlantLocation" class="textStd" runat="server" Font-Bold="true">Location</asp:Label>
						</div>
						<br />

						<table style="width: 100%;">
						<tr><td width="80%">
						<strong>Date:</strong><br />
						<asp:Label ID="lblIncidentDate" class="textStd" runat="server" Text="<%$ Resources:LocalizedText, IncidentDate %>" /><br />
						<br />
						<strong>Instructions:</strong><br />
						<asp:Label ID="lblIncidentInstructions" class="textStd" runat="server">Incident Instructions</asp:Label><br />
						<br />
						<strong>Notes:</strong>
						<table style="width: 95%" cellpadding="8" cellspacing="0">
								<asp:Repeater ID="rptComments" runat="server">
									<ItemTemplate>
									<tr>
									<td style="width: 66%;"><%# Eval("Comment") %></td>
									<td style="width: 17%;"><%#Capitalize((string)Eval("FirstName")) + " " + Capitalize((string)Eval("LastName")) %></td>
									<td style="width: 17%;"><%# ((DateTime)Eval("CommentDate")).ToShortDateString() %></td>
									</tr>
									</ItemTemplate>
								</asp:Repeater>
						<tr><td style="background-color: #ddd;">
							<telerik:RadTextBox ID="rtbNewNote" Skin="Metro" runat="server" Width="100%">
							</telerik:RadTextBox>
						</td><td style="background-color: #ddd;">
							<asp:Label ID="lblFullName" runat="server" Text=""></asp:Label></td>
						<td style="background-color: #ddd;">
							<telerik:RadButton ID="rbAddNote" runat="server" Skin="Metro" Text="Add Note"
								onclick="rbAddNote_Click">
							</telerik:RadButton>
						</td></tr>
						</table><br />
						<br />

							<telerik:RadButton ID="btnSubmit" runat="server" Text="I Have Reviewed the Instructions Above" Width="50%" Skin="Metro" OnClick="btnSubmit_Click" /><br />
							<br />

						</td><td style="width: 20%; text-align: center;">
							<asp:Literal ID="ltrDownloadReport" runat="server"></asp:Literal>
						</td>
						</tr></table>
					</div>
				</td>
			</tr>
		</table>
	</div>
</asp:Content>
