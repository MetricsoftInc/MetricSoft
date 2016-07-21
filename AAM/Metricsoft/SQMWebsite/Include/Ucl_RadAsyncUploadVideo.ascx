<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_RadAsyncUploadVideo.ascx.cs"
	Inherits="SQM.Website.Ucl_RadAsyncUploadVideo" EnableViewState="true" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadGrid ID="rgFiles" runat="server" Skin="Metro" OnDeleteCommand="rgFiles_OnDeleteCommand" OnItemDataBound="rgFiles_ItemDataBound" MasterTableView-CssClass="RadFileExplorer"
 MasterTableView-BorderColor="LightGray" HeaderStyle-Font-Size="11px" MasterTableView-BorderWidth="0" MasterTableView-Font-Size="11px" MasterTableView-ForeColor="#444444">
	<MasterTableView DataKeyNames="VideoId" Width="100%" AutoGenerateColumns="False">
		<Columns>
			<telerik:GridTemplateColumn UniqueName="FileNameColumn" HeaderText="File" HeaderStyle-Width="100">
				<ItemTemplate>
					<div class="rfeFileExtension <%# GetVideoFileExtension(DataBinder.Eval(Container.DataItem, "FileName").ToString().ToLower()) %>">
						<a href="/Shared/FileHandler.ashx?DOC=va&DOC_ID=<%# DataBinder.Eval(Container.DataItem, "VideoAttachId").ToString() %>&FILE_NAME=<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>"
							style="text-decoration: underline;" target="_blank">
							<asp:Literal id="ltrFileName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>' />
						</a>
					</div>
				</ItemTemplate>
			</telerik:GridTemplateColumn>
			<telerik:GridBoundColumn UniqueName="DescriptionColumn" DataField="Description" HeaderText="<%$ Resources:LocalizedText, Description %>">
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn UniqueName="SizeColumn" DataField="Size" HeaderText="<%$ Resources:LocalizedText, Size %>" DataFormatString="{0:n0} KB">
			</telerik:GridBoundColumn>
            <telerik:GridTemplateColumn UniqueName="DisplayTypeColumn" DataField="DisplayType" HeaderText="Show in<br/>Reports" ItemStyle-HorizontalAlign="Center" >
                <ItemTemplate>
				    <asp:CheckBox ID="checkBox" runat="server" Checked='<%# GetVideoChecked(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "DisplayType"))) %>' />
				</ItemTemplate>
            </telerik:GridTemplateColumn>
			<telerik:GridButtonColumn UniqueName="DeleteButtonColumn" ButtonType="LinkButton" ConfirmTitle="<%$ Resources:LocalizedText, Delete %>"
				ConfirmText="Delete Attachment - Are You Sure?" CommandName="Delete" Text="<%$ Resources:LocalizedText, Delete %>"
				ItemStyle-Font-Underline="true">
			</telerik:GridButtonColumn>
		</Columns>
	</MasterTableView>
    <ClientSettings  EnableAlternatingItems="false"></ClientSettings>
</telerik:RadGrid>
    <br style="clear: both;" /><%--<br />--%>
<table cellpadding="0" cellspacing="0"><tr>
    <td id="tdUploadImg" runat="server" style="vertical-align: bottom; padding-bottom: 16px;"><img src="/images/defaulticon/16x16/attachment.png" alt="" style="border: 0; cursor: pointer;" onclick="$telerik.$('.ruFileInput').click();" /></td>
    <td><telerik:RadAsyncUpload runat="server" ID="raUpload" MultipleFileSelection="Disabled" MaxFileInputsCount="10" Localization-Select="Browse..."
	skin="Metro" OnClientFileUploaded="onClientFileUploadedVideo" /></td></table>
<asp:HiddenField ID="hfListId" runat="server" />
<asp:HiddenField ID="hfDescriptions" runat="server" />
<asp:HiddenField ID="hfMode" runat="server" />


