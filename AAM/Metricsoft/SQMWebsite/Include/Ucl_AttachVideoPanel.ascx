<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AttachVideoPanel.ascx.cs" Inherits="SQM.Website.Ucl_AttachVideoPanel" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="RadUpload" TagPrefix="Ucl" %>
<%--<%@ Register src="~/Include/Ucl_Progress.ascx" TagName="Progress" TagPrefix="Ucl" %>--%>

<script type="text/javascript">

	window.onload = function () {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclVideoPanel_hfChangeUpdate').value = "";
	}
	window.onbeforeunload = function () {
		if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclVideoPanel_hfChangeUpdate').value == '1') {
			return 'You have unsaved changes on this page.';
		}
	}
	function ChangeUpdate(sender, args) {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclVideoPanel_hfChangeUpdate').value = '1';
		return true;
	}
	function ChangeClear(sender, args) {
		document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclVideoPanel_hfChangeUpdate').value = '0';
	}

	function ValidateOnSave(sender, args) {
		var valid = false;
		try {
			if (isBlank(document.getElementById('tbTitle').value) == false) {
				if (isBlank(document.getElementById('tbFileDescription').value) == false) {
					valid = true;
				}
			}

			if (valid == false) {
				alert('<%= GetGlobalResourceObject("LocalizedText","RequiredFieldsMustBeCompleted") %>');
			}

			args.set_cancel(!valid);
		}
		catch (ex) {
		}
	}

</script>

<asp:HiddenField id="hfTitle" runat="server"/> 
<asp:HiddenField ID="hfDesc" runat="server" />
<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>


<asp:Panel ID="pnlManageAttachVideos" runat="server"  style="margin: 5px;" Visible="false">
<telerik:RadAjaxPanel ID="rapManageVideos"  runat="server">
	<asp:Label ID="lblManageVideos" runat="server" CssClass="prompt"></asp:Label>
	<div id="divUpload" runat="server" style="margin-top: 5px;">
		<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:LocalizedText, VideoUploadInstruction %>"></asp:Literal>
		<br />
		<asp:Panel ID="pnlListVideo" runat="server"  Visible="false" Width="99%" style="margin-bottom: 7px;">
			<telerik:RadGrid ID="rgFiles" runat="server" Skin="Metro" OnDeleteCommand="rgFiles_DeleteCommand" OnItemDataBound="rgFiles_ItemDataBound" >
				<MasterTableView DataKeyNames="VideoId" Width="100%" AutoGenerateColumns="False">
					<Columns>
						<telerik:GridTemplateColumn UniqueName="FileNameColumn" HeaderText="File" HeaderStyle-Width="30%">
							<ItemTemplate>
								<div class="rfeFileExtension <%# GetFileExtension(DataBinder.Eval(Container.DataItem, "FileName").ToString().ToLower()) %>">
									<a href="/Shared/FileHandler.ashx?DOC=v&DOC_ID=<%# DataBinder.Eval(Container.DataItem, "VideoId").ToString() %>&FILE_NAME=<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>"
										style="text-decoration: underline;" target="_blank">
										<asp:Literal ID="ltrFileName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>' />
									</a>
								</div>
							</ItemTemplate>
						</telerik:GridTemplateColumn>
						<telerik:GridBoundColumn UniqueName="TitleColumn" DataField="Title" HeaderStyle-Width="45%" HeaderText="<%$ Resources:LocalizedText, Title %>">
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn UniqueName="SizeColumn" DataField="Size" HeaderText="<%$ Resources:LocalizedText, Size %>" DataFormatString="{0:n0} KB">
						</telerik:GridBoundColumn>
						<telerik:GridButtonColumn UniqueName="DeleteButtonColumn" ButtonType="LinkButton" ConfirmTitle="<%$ Resources:LocalizedText, Delete %>"
							ConfirmText="Delete Video - Are You Sure?" CommandName="Delete" Text="<%$ Resources:LocalizedText, Delete %>"
							ItemStyle-Font-Underline="true">
						</telerik:GridButtonColumn>
					</Columns>
				</MasterTableView>
				<ClientSettings EnableAlternatingItems="false"></ClientSettings>
			</telerik:RadGrid>
		</asp:Panel>
		<br style="clear: both;" />
<%--		<Ucl:Progress id="uclProgress" runat="server"/>--%>
		<asp:Panel ID="pnlAttachVideoBody" runat="server">
			<table width="99%" border="0"  class="lightTable">
				<tr>
					<td class="columnHeader" width="20%">
						<asp:Label ID="lblVideoDate" runat="server" Text="<%$ Resources:LocalizedText, VideoDate %>"></asp:Label>
					</td>
					<td class="tableDataAlt" width="1%">&nbsp;</td>
					<td class="tableDataAlt">
						<telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" ToolTip="<%$ Resources:LocalizedText, VideoDateDesc %>">
							<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" ShowPopupOnFocus="true" FastNavigationNextText="&amp;lt;&amp;lt;"></Calendar>
							<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="" OnClientDateChanged="ChangeUpdate">
								<EmptyMessageStyle Resize="None"></EmptyMessageStyle>
								<ReadOnlyStyle Resize="None"></ReadOnlyStyle>
								<FocusedStyle Resize="None"></FocusedStyle>
								<DisabledStyle Resize="None"></DisabledStyle>
								<InvalidStyle Resize="None"></InvalidStyle>
								<HoveredStyle Resize="None"></HoveredStyle>
								<EnabledStyle Resize="None"></EnabledStyle>
							</DateInput>
							<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
						</telerik:RadDatePicker>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblVideoTitle" runat="server" Text="<%$ Resources:LocalizedText, Title %>"></asp:Label>
					</td>
					<td class="required">&nbsp;</td>
					<td class="tableDataAlt">
						<asp:TextBox ID="tbTitle" runat="server" CssClas="textStd" MaxLength="100" Width="450px" onChange="ChangeUpdate()"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblVideoDesc" runat="server" Text="<%$ Resources:LocalizedText, VideoDescription %>"></asp:Label>
					</td>
					<td class="required">&nbsp;</td>
					<td class="tableDataAlt">
						<asp:TextBox ID="tbFileDescription" runat="server" CssClass="textStd" TextMode="MultiLine" Rows="3" MaxLength="1000" Width="450px" onChange="ChangeUpdate()"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblVideoType" runat="server" Text="<%$ Resources:LocalizedText, VideoType %>"></asp:Label>
					</td>
					<td class="required">&nbsp;</td>
					<td class="tableDataAlt">
						<telerik:RadDropDownList ID="ddlVideoType" runat="server" Skin="Metro" Width="450" OnClientSelectedIndexChanged="ChangeUpdate" ></telerik:RadDropDownList>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblInjuryType" runat="server" Text="<%$ Resources:LocalizedText, InjuryType %>"></asp:Label>
					</td>
					<td class="tableDataAlt">&nbsp;</td>
					<td class="tableDataAlt">
						<telerik:RadDropDownList ID="ddlInjuryType" runat="server" Skin="Metro" Width="450" DropDownHeight="300px" ExpandDirection="Up" OnClientSelectedIndexChanged="ChangeUpdate"></telerik:RadDropDownList>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblBodyPart" runat="server" Text="<%$ Resources:LocalizedText, BodyPart %>"></asp:Label>
					</td>
					<td class="tableDataAlt">&nbsp;</td>
					<td class="tableDataAlt">
						<telerik:RadDropDownList runat="server" ID="rdlBodyPart" Skin="Metro" Width="450" DropDownHeight="300px" ExpandDirection="Up" OnClientSelectedIndexChanged="ChangeUpdate" ></telerik:RadDropDownList>
					</td>
				</tr>
				<tr>
					<td class="columnHeader">
						<asp:Label ID="lblVideoAttach" runat="server" Text="<%$ Resources:LocalizedText, Video %>"></asp:Label>
					</td>
					<td class="tableDataAlt">&nbsp;</td>
					<td class="tableDataAlt">
						<telerik:RadAsyncUpload runat="server" ID="raUpload" MultipleFileSelection="Disabled" MaxFileInputsCount="1" Localization-Select="Browse..."
							skin="Metro" OnClientFileUploaded="onClientFileUploaded" />
						<asp:HiddenField ID="hfListId" runat="server" />
						<asp:HiddenField ID="hfDescriptions" runat="server" />
						<asp:HiddenField ID="hfMode" runat="server" />
					</td>
				</tr>
			</table>
			<asp:Panel id="pnlAttachMsg" runat="server" Visible ="false">
				<br />
				<asp:Label ID="lblAttachMsg" runat="server" CssClass="labelEmphasis"></asp:Label>
			</asp:Panel>
		</asp:Panel>
		<div style="margin: 10px;">
			<center>
			<span>
				<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
						 OnClick="btnSave_Click"  SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
				<%--OnClientClicked="ChangeClear" OnClick="btnSave_Click"  AutoPostBack="true" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>--%>
				<%--<asp:Button ID="btnSave" CSSclass="buttonStd buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin: 5px;" Onclick="btnSave_Click"></asp:Button>--%>
				<asp:Button ID="btnCancel" CSSclass= "buttonEmphasis buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnCancel_Click" CausesValidation="false"></asp:Button>
			</span>
			</center>
		</div>
	</div>
</telerik:RadAjaxPanel>
</asp:Panel>

