<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Error404.aspx.cs" Inherits="SQM.Website.Error404" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<script src="scripts/ps_admin.js" type="text/javascript"></script>
	<link href="css/PSSQM.css" rel="stylesheet" type="text/css" />
	<asp:HiddenField ID="hfBase" runat="server" />
	<asp:HiddenField ID="hdCurrentActiveTab" runat="server" />
	<asp:HiddenField ID="hdCurrentActiveSecondaryTab" runat="server" />
	<FORM name="dummy">
		<div>
			<table width="100%" border="0" cellspacing="0" cellpadding="1">
				<tr>
					<td class="tabActiveTableBg" colspan="10" align="center">
						<asp:Panel ID="pnlMonitor" runat="server" GroupingText="Error Page" CssClass="sectionTitles" Width="99%" style="margin: 3px;">
							<h1>The system cannot find the page you requested
							</h1>
							<p>
								<asp:Label ID="lblPage" runat="server" Font-Italic="True" ForeColor="#C00000"></asp:Label>
							</p>
							<p>
								<asp:Label ID="lblReferrer" runat="server" class="ErrorMessage"></asp:Label>
							</p>
							<p class="ErrorMessage">
								Please verify your request and try again.
							</p>
						</asp:Panel>
					</td>
				</tr>
			</table>  
		 </div>
	 </FORM>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
</asp:Content>
