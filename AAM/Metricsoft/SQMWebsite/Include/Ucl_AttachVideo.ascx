<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AttachVideo.ascx.cs" Inherits="SQM.Website.Ucl_AttachVideo" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="RadUpload" TagPrefix="Ucl" %>

<script type="text/javascript">
	function OpenManageVideosWindow() {
		$find("<%=winManageVideos.ClientID %>").show();
	}
</script>

<asp:HiddenField id="hfTitle" runat="server"/> 
<asp:HiddenField ID="hfDesc" runat="server" />


<asp:Panel runat="server" ID="pnlManageVideos" Visible="false" >
        <asp:ImageButton ID="imbVideo" runat="server" tooltip="add or view videos" ImageUrl="~/images/attach.png"  OnClientClick="PopupCenter('../Shared/Shared_Attach.aspx', 'newPage', 800, 600);  return false;" />
</asp:Panel>

<telerik:RadWindow runat="server" ID="winManageVideos" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="575px" Width="500px" Title="Manage Videos" Behavior="Move">
	<ContentTemplate>
		<div style="margin: 5px;">
			<asp:Label ID="lblManageVideos" runat="server" CssClass="prompt"></asp:Label>
			<div id="divUpload" runat="server" style="margin-top: 5px;">
				<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoUploadInstruction %>"></asp:Literal>
				<br />
				<asp:Panel ID="pnlListVideo" runat="server" class="listingImageContainerTop" Visible="false" Width="100%">
					<telerik:RadGrid ID="rgFiles" runat="server" Skin="Metro" OnDeleteCommand="rgFiles_DeleteCommand" OnItemDataBound="rgFiles_ItemDataBound" MasterTableView-CssClass="RadFileExplorer"
						MasterTableView-BorderColor="LightGray" HeaderStyle-Font-Size="11px" MasterTableView-BorderWidth="0" MasterTableView-Font-Size="11px" MasterTableView-ForeColor="#444444">
						<MasterTableView DataKeyNames="VideoId" Width="100%" AutoGenerateColumns="False">
							<Columns>
								<telerik:GridTemplateColumn UniqueName="FileNameColumn" HeaderText="File" HeaderStyle-Width="100">
									<ItemTemplate>
										<div class="rfeFileExtension <%# GetFileExtension(DataBinder.Eval(Container.DataItem, "FileName").ToString().ToLower()) %>">
											<a href="/Shared/FileHandler.ashx?DOC=v&DOC_ID=<%# DataBinder.Eval(Container.DataItem, "VideoId").ToString() %>&FILE_NAME=<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>"
												style="text-decoration: underline;" target="_blank">
												<asp:Literal ID="ltrFileName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>' />
											</a>
										</div>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridBoundColumn UniqueName="TitleColumn" DataField="Title" HeaderText="<%$ Resources:LocalizedText, Title %>">
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
				<br /><br style="clear: both;" />
				<span class="confirm">
					<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoDate %>"></asp:Literal></span><br />
				<telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" ToolTip="<%$ Resources:LocalizedText, VideoDateDesc %>" ShowPopupOnFocus="true" >
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
				</telerik:RadDatePicker>
				<br /><br />
				<span class="confirm">
					<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, Title %>"></asp:Literal></span><br />
				<%--<asp:TextBox ID="tbTitle" runat="server" MaxLength="100" Width="392px"></asp:TextBox>--%>
				<telerik:RadTextBox ID="rtbTitle" runat="server" MaxLength="100" Width="392px"></telerik:RadTextBox>
				<%--<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="<%$ Resources:LocalizedText, TitleRequired %>" ControlToValidate="tbTitle"></asp:RequiredFieldValidator>--%>
				<br />
				<span class="confirm">
					<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoDescription %>"></asp:Literal></span><br />
				<%--<asp:TextBox ID="tbFileDescription" runat="server" TextMode="MultiLine"
					MaxLength="1000" Width="392px"></asp:TextBox>--%>
				<telerik:RadTextBox runat="server" ID="rtbFileDescription" TextMode="MultiLine"
					MaxLength="1000" Width="392px"></telerik:RadTextBox>
				<br /><br />
				<asp:Panel ID="pnlDisplayArea" runat="server" Visible="true" CssClass="tableDataAlt">
					<span class="confirm">
						<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoType %>"></asp:Literal></span>
					<br />
					<telerik:RadDropDownList ID="rddlVideoType" runat="server"></telerik:RadDropDownList>
					<br /><br />
					<span class="confirm">
						<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, InjuryType %>"></asp:Literal></span>
					<br />
					<telerik:RadDropDownList ID="rddlInjuryType" runat="server"></telerik:RadDropDownList>
					<br /><br />
					<span class="confirm">
						<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, BodyPart %>"></asp:Literal></span>
					<br />
					<telerik:RadDropDownList runat="server" ID="rdlBodyPart"></telerik:RadDropDownList>
				</asp:Panel>
				<br />
				<span class="confirm">Select File </span>
				<br />
				<%--<asp:FileUpload ID="flFileUpload" runat="server" />--%>
				<telerik:RadAsyncUpload runat="server" ID="raUpload" MultipleFileSelection="Disabled" MaxFileInputsCount="1" Localization-Select="Browse..."
	skin="Metro" OnClientFileUploaded="onClientFileUploaded" />
				<%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="<%$ Resources:LocalizedText, UploadError %>" ControlToValidate="flFileUpload"></asp:RequiredFieldValidator>--%>
<asp:HiddenField ID="hfListId" runat="server" />
<asp:HiddenField ID="hfDescriptions" runat="server" />
<asp:HiddenField ID="hfMode" runat="server" />
			</div>
		</div>
		<div style="float: left; margin-left: 10px;">
			<span>
				<asp:Button ID="btnSave" CSSclass="buttonStd buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin: 5px;" Onclick="btnSave_Click"></asp:Button>
				<asp:Button ID="btnCancel" CSSclass= "buttonEmphasis buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnCancel_Click" CausesValidation="false"></asp:Button>
			</span>
		</div>
	</ContentTemplate>
</telerik:RadWindow>

