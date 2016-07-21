<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Media_Upload.aspx.cs" Inherits="SQM.Website.Media_Upload" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="../css/PSSQM_Default.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>

<script type="text/javascript">
	function ShowModalDialog() {
		var x = $find("ModalExtnd1");
		Page_ClientValidate();
		if (!Page_IsValid)
			x.show();
	}

	function init()
	{ document.getElementById('SummaryDiv').style.display = 'none'; }
</script>

<body bgcolor="#FFFFFF" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
	<form id="form1" runat="server">
	 <asp:ScriptManager ID="ScriptManager1" runat="server">
	</asp:ScriptManager>
	<ajaxToolkit:ModalPopupExtender ID="ModalExtnd1" runat="server" TargetControlID="lblHidden"
	PopupControlID="SummaryDiv" BackgroundCssClass="error_popupbg">
	</ajaxToolkit:ModalPopupExtender>
	<div  id="SummaryDiv" class="error_popupdiv">
<table width="100%" >
	<tr><td class="header">
		 Please correct the following:
	</td></tr>
	<tr><td>
		<asp:ValidationSummary id="ValSum1" runat="server" >
		</asp:ValidationSummary>
	</td></tr>
	<tr><td align="center">
		<input type="button" runat="server" value="<%$ Resources:LocalizedText, OK %>" onclick="$find('ModalExtnd1').hide();"/>
	</td></tr>
</table>
<asp:Label ID="lblHidden" runat="server" Text="hidden" CssClass="error_hidelbl">
</asp:Label>
</div>

	<div>
		<table width="100%" border="0" cellspacing="0" bgcolor="#CCCCCC">
			<tr>
				<!-- PAGE CONTENT CELL -->
				<td width="98%" valign="top">
					<!-- PAGE CONTENT MAIN TABLE -->
					<table width="100%" border="0" cellspacing="0" cellpadding="0">
						<tr>
							<td valign="top">
								<!-- PAGE TITLE BORDER TABLE -->
								<table width="100%" border="0" cellspacing="0" cellpadding="0" class="border">
									<tr>
										<td valign="top" width="100%">
											<!-- PAGE TITLE CONTENT TABLE -->
											<table width="100%" border="0" cellspacing="1" cellpadding="0">
												<tr style="height: 30px;">
													<!--<td class="borderHeader">-->
													<td width="100%" background="/images/QAIHeaderBg.gif" style="vertical-align: bottom; margin:0px; border:0px;">
														<table width="100%">
															<tr>
																<td>
																	<asp:Label ID="lblDocumentUploadTitle" runat="server" CssClass="popupTitles" Text="<%$ Resources:LocalizedText, VideoUpload %>"></asp:Label>
																</td>
																<td valign="top" nowrap align="right" >
																	<asp:Button ID="btnClose" runat="server" class="buttonStd" Text="Close Window" OnClientClick="javascript:window.opener.document.forms[0].submit(); window.close();" />
																</td>
															</tr>
														</table>
														<!-- END OF PAGE TITLE CONTENT TABLE -->
													</td>
												</tr>
											</table>

											<!------------------------------------->
											<!-- END OF PAGE TITLE BORDER TABLE -->
										</td>
									</tr>
								</table>

								<!-- PAGE DATA BORDER TABLE -->
								<table width="100%" border="0" cellspacing="0" cellpadding="0" class="border">
									<tr>
										<td valign="top" width="100%">
											<!-- PAGE DATA CONTENT TABLE -->
											<table width="100%" border="0" cellspacing="1" cellpadding="5">
												<tr>
													<td bgcolor="#999999" valign="top" align="center" colspan="2">
														<!-- DATA ENTRY WHITE BORDER TABLE -->
														<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#FFFFFF">
															<tr>
																<td>

																	<!-- DATA ENTRY CONTENT TABLE -->

																	<table width="100%" border="0" cellspacing="1" cellpadding="5">
																		<tr>
																			<td class="columnHeader" width="15%">
																				Step 1:
																			</td>
																			<td class="tableDataAlt">
																				<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoUploadInstruction %>"></asp:Literal>
																				<br />
																				<span class="confirm"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoDate %>"></asp:Literal></span>
																				<telerik:raddatepicker id="dmFromDate" runat="server" cssclass="textStd" width="145px" skin="Metro" dateinput-skin="Metro" dateinput-font-size="Small" ToolTip="<%$ Resources:LocalizedText, VideoDateDesc %>">
																					<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;"></Calendar>

																					<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="">
																					<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

																					<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

																					<FocusedStyle Resize="None"></FocusedStyle>

																					<DisabledStyle Resize="None"></DisabledStyle>

																					<InvalidStyle Resize="None"></InvalidStyle>

																					<HoveredStyle Resize="None"></HoveredStyle>

																					<EnabledStyle Resize="None"></EnabledStyle>
																					</DateInput>

																					<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
																				</telerik:raddatepicker>

																				<span class="confirm"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, Title %>"></asp:Literal></span>
																				<asp:TextBox ID="tbTitle" runat="server" MaxLength="100" Width="392px"></asp:TextBox>
																				<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="<%$ Resources:LocalizedText, TitleRequired %>" ControlToValidate="tbTitle"></asp:RequiredFieldValidator>

																				<span class="confirm"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoDescription %>"></asp:Literal></span>
																				<asp:TextBox ID="tbFileDescription" runat="server" TextMode="MultiLine"
																					MaxLength="1000" Width="392px"></asp:TextBox>

																				<asp:Panel ID="pnlDisplayArea" runat="server" Visible="true" CssClass="tableDataAlt">
																					<span class="confirm"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoType %>"></asp:Literal></span>
																					<br />
																					<asp:DropDownList ID="ddlInjuryType" runat="server"></asp:DropDownList>

																					<span class="confirm"><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, BodyPart %>"></asp:Literal></span>
																					<br />
																					<telerik:RadDropDownList runat="server" ID="rdlBodyPart"></telerik:RadDropDownList>
																				 </asp:Panel>
																			</td>
																		</tr>
																		<tr>
																			<td class="columnHeader">
																				Step 2:
																			</td>
																			<td class="tableDataAlt" width="85%">
																				<span class="confirm">Select File </span>
																				<br />
																				<asp:FileUpload ID="flFileUpload" runat="server" />
																				<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="<%$ Resources:LocalizedText, UploadError %>" ControlToValidate="flFileUpload"></asp:RequiredFieldValidator>
																			</td>
																		</tr>
																		<tr>
																			<td class="columnHeader">
																				Step 3:
																			</td>
																			<td class="tableDataAlt">
																				<span class="confirm">Upload </span>
																				<br>
																				<asp:Label ID="lblDocUpload" runat="server" class="tableDataAlt" Text="Click the 'upload' button. The file will display in the list below when complete."></asp:Label>
																				<br>
																				<br>

																				<table cellpadding="1" cellspacing="1">
																					<tr>
																						<td>
																							<asp:Button ID="lbUpload" runat="server" CssClass="buttonEmphasis"
																								onclick="lbUpload_Click" CausesValidation="true"
																								onclientclick="document.body.style.cursor='wait';" text="Upload Document"/>
																						</td>
																						<td></td>
																					</tr>
																					<%--<tr><td>
																					   <div id="divUploadGVScroll" runat="server" class="scrollAreaSmall">
																						<asp:GridView ID="gvUploadedFiles" runat="server" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%"
																							onrowdeleting="gvUploadedFiles_RowDeleting" OnRowDataBound="gvUploadedFiles_OnRowDataBound">
																							<HeaderStyle CssClass="HeadingCellTextLeft" />
																							<RowStyle CssClass="DataCell" />
																							<Columns>
																								<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Type %>" ItemStyle-Width="10%">
																									<ItemTemplate>
																										<asp:HiddenField  ID="hfFileName" runat="server" Value='<%#Eval("FILE_NAME") %>' />
																										<asp:Image  ID="imgFileType" runat="server" HeaderText="File Type" ItemStyle-HorizontalAlign="Center"></asp:Image>
																									</ItemTemplate>
																								</asp:TemplateField>
																								<asp:BoundField DataField="FILE_NAME" HeaderText="<%$ Resources:LocalizedText, FileName %>"  ItemStyle-Width="30%" />
																								<asp:BoundField DataField="FILE_DESC" HeaderText="File Description"  ItemStyle-Width="30%" />
																								<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="<%$ Resources:LocalizedText, Size %>" ItemStyle-Width="15%"
																									SortExpression="FILE_SIZE">
																									<ItemTemplate>
																									   <%# FormatFilesize(Eval("FILE_SIZE"))%>
																									</ItemTemplate>
																								</asp:TemplateField>
																								<asp:CommandField ShowDeleteButton="True" />
																							</Columns>
																						</asp:GridView>
																						</div>
																					</td></tr>--%>
																				</table>

																			</td>
																		</tr>
																		<tr>
																			<td class="columnHeader">
																				Step 5:
																			</td>
																			<td class="tableDataAlt">
																				Done<br>
																				Click 'done' to close this window.<br>
																				<br>
																				<table cellpadding="1" cellspacing="1">
																					<tr>
																						<td>
																							<asp:Button ID="Button1" runat="server" class="buttonStd" Text="<%$ Resources:LocalizedText, Done %>" OnClientClick="javascript:window.opener.document.forms[0].submit(); window.close();" />
																						</td>
																					</tr>
																				</table>
																			</td>
																		</tr>
																	</table>
																</td>
															</tr>
														</table>
														<!-- END OF PAGE DATA CONTENT TABLE -->
													</td>
												</tr>
											</table>
											<!------------------------------------>
											<!-- END OF PAGE DATA BORDER TABLE -->
										</td>
									</tr>
								</table>




								<!-- END OF PAGE CONTENT MAIN TABLE -->
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
