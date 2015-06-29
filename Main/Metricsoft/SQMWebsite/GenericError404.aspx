<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenericError404.aspx.cs" Inherits="SQM.Website.GenericError404" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
	<script src="scripts/ps_admin.js" type="text/javascript"></script>
	<link href="css/PSSQM.css" rel="stylesheet" type="text/css" />
	<FORM name="dummy">
		<div>
			<table width="100%" border="0" cellspacing="0" cellpadding="1">
				<tr>
					<td class="tabActiveTableBg" colspan="10" align="center">
						<asp:Panel ID="pnlMonitor" runat="server" GroupingText="" CssClass="sectionTitles" Width="99%" style="margin: 3px;">
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
</body>
</html>
