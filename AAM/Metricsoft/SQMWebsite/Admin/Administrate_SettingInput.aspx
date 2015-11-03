<%@ Page Title="Historical Currency Input" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_SettingInput.aspx.cs"
	Inherits="SQM.Website.Administrate_SettingInput" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

	<div class="admin_tabs">
		<table width="100%" border="0" cellspacing="0" cellpadding="12">
			<tr>
				<td class="tabActiveTableBg" colspan="10">
					<asp:Panel runat="server" ID="pnlSearchBar">
						<Ucl:SearchBar ID="uclSearchBar" runat="server" />
					</asp:Panel>
					<table width="99%">
						<tr>
							<td align="left">
								<asp:Label ID="lblDataUploadTitle" runat="server" CssClass="pageTitles" Text="Settings"></asp:Label><br />
								<asp:Label ID="lblPageInstructions" runat="server" CssClass="instructText" Text="Use this page to enter, view, or edit settings used in the application.<br/>"></asp:Label>
							</td>
						</tr>
					</table>

					<div id="divPageBody" runat="server" style="margin-top: 5px;">
						<div id="divNavArea" runat="server" class="listAreaLeft" style="background-color: #FCFCFC;">
							<span style="float: left; margin-left: 5px; margin-bottom: 3px;">
								<asp:Label ID="lblSettingFamily" runat="server" CssClass="prompt" Text="Family: "></asp:Label>
								<asp:DropDownList ID="ddlSettingFamily" runat="server" CssClass="textStd" AutoPostBack="true" OnSelectedIndexChanged="ddlSettingFamily_SelectedIndexChanged">
								</asp:DropDownList>&nbsp;&nbsp;
								<asp:Label ID="lblSettingGroup" runat="server" CssClass="prompt" Text="Group: "></asp:Label>
								<asp:DropDownList ID="ddlSettingGroup" runat="server" CssClass="textStd" AutoPostBack="false">
								</asp:DropDownList>
								&nbsp;&nbsp;
								<asp:Button id="btnSettingSearch" runat="server" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" OnClick="btnSettingSearch_Click"/>
							</span>
							<asp:HiddenField runat="server" ID="hdnEncrypt" />
							<table width="100%" border="0" cellspacing="0" cellpadding="0">
								<tr>
									<td class="admBkgd" align="center">
										<div id="divSettingGVScrollRepeater" runat="server" class="">
											<asp:Repeater runat="server" ID="rptSettingList" ClientIDMode="AutoID" OnItemDataBound="rptSettingList_ItemDataBound">
												<HeaderTemplate>
													<table cellspacing="0" cellpadding="2" border="0" width="100%" class="rptDarkBkgd">
												</HeaderTemplate>
												<ItemTemplate>
													<tr>
														<td class="listData" valign="top" colspan="2">
															<asp:HiddenField ID="hdnSettingCode" runat="server" Value='<%# Eval("SETTING_CD") %>' />
															<asp:Label runat="server" ID="lblShortDesc_out" CssClass="prompt" Text='<%#Eval("XLAT_SHORT") %>'></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="listData" valign="top">
															<asp:Label ID="lblValue" runat="server" Text='<%# Eval("VALUE") %>' CssClass="textStd"></asp:Label>
														</td>
														<td class="listData" valign="top">
															<asp:LinkButton ID="lnkEditCode" runat="server" CommandArgument='<%# Eval("SETTING_CD") %>'
																CssClass="buttonEditRight" OnClick="lnkEditCode_Click" ToolTip="Edit this code"></asp:LinkButton>
														</td>
													</tr>
													<tr style="height: 3px;">
														<td></td>
													</tr>
												</ItemTemplate>
												<AlternatingItemTemplate>
													<tr>
														<td class="listDataAlt" valign="top" colspan="2">
															<asp:HiddenField ID="hdnSettingCode" runat="server" Value='<%# Eval("SETTING_CD") %>' />
															<asp:Label runat="server" ID="lblShortDesc_out" CssClass="prompt" Text='<%#Eval("XLAT_SHORT") %>'></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="listDataAlt" valign="top">
															<asp:Label ID="lblValue" runat="server" Text='<%# Eval("VALUE") %>' CssClass="textStd"></asp:Label>
														</td>
														<td class="listDataAlt" valign="top">
															<asp:LinkButton ID="lnkEditCode" runat="server" CommandArgument='<%# Eval("SETTING_CD") %>'
																CssClass="buttonEditRight" OnClick="lnkEditCode_Click" ToolTip="Edit this code"></asp:LinkButton>
														</td>
													</tr>
													<tr style="height: 3px;">
														<td></td>
													</tr>
												</AlternatingItemTemplate>
												<FooterTemplate>
													</table>
												</FooterTemplate>
											</asp:Repeater>
											<asp:Label runat="server" ID="lblSettingListEmptyRepeater" Height="40px" Text="There are currently no Settings defined."
												class="GridEmpty" Visible="False"></asp:Label>
										</div>
									</td>
								</tr>
							</table>
						</div>
						<div id="divWorkArea" runat="server" class="workAreaRightWide">

							<asp:Panel runat="server" ID="pnlSettingEdit">
								<table width="99%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 24px;">
									<tr>
										<td class="editArea">
											<asp:Label runat="server" ID="lblConfirmMustMatch" Visible="false" CssClass="promptAlert" Text="The Value and Confirm Value fields must match."></asp:Label>
											<asp:Label runat="server" ID="lblErrorUpdating" Visible="false" CssClass="promptAlert" Text="An error was encountered during update. "></asp:Label>
											<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
												<asp:Label ID="lblIdentifyInstruction" runat="server" Text="<b>Settings:</b> manage settingsfor the application." CssClass="instructText"></asp:Label>
												<table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin: 4px 0 4px 0;">
													<tr>
														<td class="columnHeader" width="24%">
															<asp:Label ID="lblCode" runat="server" Text="Code"></asp:Label>
														</td>
														<td class="required" width="1%">&nbsp;</td>
														<td class="tableDataAlt" width="75%">
															<asp:Label ID="lblCodeValue" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="columnHeader">
															<asp:Label ID="lblShortDesc" runat="server" Text="Short Description"></asp:Label>
														</td>
														<td class="required">&nbsp;</td>
														<td class="tableDataAlt">
															<asp:Label ID="lblShortDescription" size="50" MaxLength="40" runat="server"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="columnHeader">
															<asp:Label ID="lblLongDesc" runat="server" Text="Long Description"></asp:Label>
														</td>
														<td class="tableDataAlt">&nbsp;</td>
														<td class="tableDataAlt">
															<asp:Label ID="lblLongDescription" size="50" MaxLength="120" runat="server" TextMode="MultiLine" Rows="2"></asp:Label>
														</td>
													</tr>
													<tr>
														<td class="columnHeader">
															<asp:Label ID="lblValue" runat="server" Text="<%$ Resources:LocalizedText, Value %>"></asp:Label>
														</td>
														<td class="tableDataAlt">&nbsp;</td>
														<td class="tableDataAlt">
															<asp:TextBox ID="tbValue" size="50" TextMode="MultiLine" runat="server"></asp:TextBox>
														</td>
													</tr>
													<tr id="trConfirm" visible="false" runat="server">
														<td class="columnHeader">
															<asp:Label ID="lblValueConfirm" runat="server" Text="Confirm Value"></asp:Label>
														</td>
														<td class="tableDataAlt">&nbsp;</td>
														<td class="tableDataAlt">
															<asp:TextBox ID="tbValueConfirm" size="50" MaxLength="60" runat="server" TextMode="Password"></asp:TextBox>
														</td>
													</tr>
												</table>
											</telerik:RadAjaxPanel>
										</td>
									</tr>
								</table>
							</asp:Panel>

						</div>
					</div>
				</td>
			</tr>
		</table>
	</div>
</asp:Content>
