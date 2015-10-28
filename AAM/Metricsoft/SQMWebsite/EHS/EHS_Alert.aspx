<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EHS_Alert.aspx.cs" Inherits="SQM.Website.EHS_Alert" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>EH&amp;S Incident Alert</title>
		<%--<link href="/css/PSSQM_Default.css" rel="stylesheet" type="text/css" />--%>
	</head>
	<body style="background-color: #fff; font-family: Verdana, Arial, sans-serif;">
		<form id="form1" runat="server">
			<div style="width: 100%;">
				<asp:Panel ID="pnlContent" runat="server">

					<%--<asp:Image ID="imgLogo" runat="server" style="position: absolute; left: 10px; top: 10px; width: 150px;" />--%>
                    <asp:Image AlternateText="logo" ID="imgLogo" runat="server" style="margin: 10px; width: 150px;" />
					<h1 style="text-align: center;">
						<asp:Literal ID="ltrCaseName" runat="server">Title</asp:Literal></h1>

                    <table class="alertTable" cellspacing="0" cellpadding="10" style="border: 1px solid #888888;">
                        <tr>
                            <td style="border: 1px solid #888888;"><asp:Label ID="lblPlantName" runat="server" Text="" Font-Bold="true"></asp:Label></td>
                            <td style="border: 1px solid #888888;"><asp:Label ID="lblIncidentId" runat="server" Text="" Font-Bold="true"></asp:Label></td>
                        </tr>
						<tr>
							<td style="border: 1px solid #888888;">
								<strong>Date of Incident:</strong>
								<asp:Literal ID="ltrDate" runat="server">n/a</asp:Literal>
							</td>
							<td style="border: 1px solid #888888;">
								<strong>Time of Incident:</strong>
								<asp:Literal ID="ltrTime" runat="server">n/a</asp:Literal>
							</td>
						</tr>
                        <tr>
							<td style="border: 1px solid #888888;" colspan="2">
								<strong><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, IncidentType%>" />:</strong>
                                <asp:Literal ID="ltrIncidentType" runat="server">n/a</asp:Literal>
							</td>
						</tr>
						<tr>
							<td style="border: 1px solid #888888;" colspan="2">
								<strong>Incident Description:</strong><br />
								<asp:Literal ID="ltrDescription" runat="server">n/a</asp:Literal>
							</td>
						</tr>
                        <tr>
							<td style="border: 1px solid #888888;" colspan="2">
								<strong>Root Cause(s):</strong><br />
								<asp:Literal ID="ltrRootCause" runat="server">n/a</asp:Literal>
							</td>
						</tr>

						<tr>
							<td style="border: 1px solid #888888;" colspan="2">
								<strong>Containment:</strong><br />
								<asp:Literal ID="ltrContainment" runat="server">n/a</asp:Literal>
							</td>
						</tr>

						<tr>
							<td style="border: 1px solid #888888;" colspan="2">
								<strong>Corrective Actions:</strong><br />
								<asp:Literal ID="ltrCorrectiveActions" runat="server">n/a</asp:Literal>
							</td>
						</tr>
						<tr>
							<td style="border: 1px solid #888888;" colspan="2">
								<strong>Photos/Attachments:</strong><br />
								<asp:Repeater ID="rptAttachments" runat="server">
									<ItemTemplate>
										<div class="distributionImageContainer" style="float: left; margin: 10px;">
<%--										<asp:Image ID="imgAttachment" CssClass="distributionImage" Width="200"
										ImageUrl='<%# ImageUrlToBase64String(GetFileHandlerUrl() + Eval("AttachmentId").ToString() + "&FILE_NAME=" + Eval("FileName").ToString()) %>'
										runat="server" /><br />--%>
                                            <asp:Image AlternateText="image" ID="imgAttachment" CssClass="distributionImage" Width="220"
										ImageUrl='<%# AttachmentIdToEncodedImage(Convert.ToDecimal(Eval("AttachmentId"))) %>'
										runat="server" /><br />
                                            <asp:Label ID="lblAttachment"  runat="server" Text='<%#: Eval("Description").ToString() %>' CssClass="textSmall"></asp:Label>
										</div>
									</ItemTemplate>

								</asp:Repeater>
							</td>
						</tr>

					</table>
				</asp:Panel>
				<asp:Panel ID="pnlError" runat="server">
					Invalid Problem Case ID.
				</asp:Panel>
			</div>
		</form>
	</body>
</html>
