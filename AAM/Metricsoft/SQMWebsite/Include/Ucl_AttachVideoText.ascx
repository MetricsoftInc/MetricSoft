<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AttachVideoText.ascx.cs" Inherits="SQM.Website.Ucl_AttachVideoText" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


	<telerik:RadGrid ID="rgFiles" runat="server" Skin="Metro" OnDeleteCommand="rgFiles_OnDeleteCommand" OnItemDataBound="rgFiles_ItemDataBound" MasterTableView-CssClass="RadFileExplorer"
		MasterTableView-BorderColor="LightGray" HeaderStyle-Font-Size="11px" MasterTableView-BorderWidth="0" MasterTableView-Font-Size="11px" MasterTableView-ForeColor="#444444">
		<MasterTableView DataKeyNames="VideoAttachId" Width="100%" AutoGenerateColumns="False">
			<Columns>
				<telerik:GridTemplateColumn HeaderText="">
					<ItemTemplate><asp:LinkButton ID="lbVideoId" runat="server" Text="<%$ Resources:LocalizedText, Edit %>" CausesValidation="false" CommandArgument='<%# Eval("VideoAttachId") %>' Font-Bold="True" OnClick="lbVideoId_OnClick" ToolTip="<%$ Resources:LocalizedText, Edit %>">
										</asp:LinkButton></ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridBoundColumn UniqueName="TitleColumn" DataField="Title" HeaderText="<%$ Resources:LocalizedText, Timestamp %>">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn UniqueName="DescriptionColumn" DataField="Description" HeaderText="<%$ Resources:LocalizedText, Description %>">
				</telerik:GridBoundColumn>
				<telerik:GridButtonColumn UniqueName="DeleteButtonColumn" ButtonType="LinkButton" ConfirmTitle="<%$ Resources:LocalizedText, Delete %>"
					ConfirmText="Delete Text - Are You Sure?" CommandName="Delete" Text="<%$ Resources:LocalizedText, Delete %>"
					ItemStyle-Font-Underline="true">
				</telerik:GridButtonColumn>
			</Columns>
		</MasterTableView>
		<ClientSettings EnableAlternatingItems="false"></ClientSettings>
	</telerik:RadGrid>
	<br />
		<div style="margin: 5px;">
			<asp:Label ID="lblManageText" runat="server" CssClass="prompt"></asp:Label>
			<div id="divUpload" runat="server" style="margin-top: 5px; position: relative;">
				<div style="float: left;"><asp:HiddenField runat="server" ID="hdnVideoAttachId" /><asp:Label runat="server" Text="Timestamp" AssociatedControlID="tbTimestamp"></asp:Label><br /><asp:TextBox runat="server" ID="tbTimestamp" TextMode="MultiLine" Columns="25" Rows="5"></asp:TextBox></div>
				<div style="float: left; padding-left: 10px;"><asp:Label runat="server" Text="Text" AssociatedControlID="tbText"></asp:Label><br /><asp:TextBox runat="server" ID="tbText" TextMode="MultiLine" MaxLength="4000" Columns="50" Rows="10"></asp:TextBox></div>
			</div>
		</div>
<br />
		<div style="margin-left: 10px;">
			<span>
				<asp:Button ID="btnSave" CSSclass="buttonStd buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin: 5px;" Onclick="btnSave_Click" CausesValidation="false"></asp:Button>
				<asp:Button ID="btnCancel" CSSclass= "buttonEmphasis buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnCancel_Click" CausesValidation="false"></asp:Button>
			</span>
		</div>

