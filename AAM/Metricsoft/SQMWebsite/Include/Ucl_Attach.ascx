<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_Attach.ascx.cs" Inherits="SQM.Website.Ucl_Attach" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="RadUpload" TagPrefix="Ucl" %>

<script type="text/javascript">
	function OpenManageAttachmentsWindow() {
		$find("<%=winManageAttachments.ClientID %>").show();
	}
</script>

<asp:HiddenField id="hfTitle" runat="server"/> 
<asp:HiddenField ID="hfDesc" runat="server" />

<asp:Panel runat="server" ID="pnlManageAttachments" Visible="false" >
        <asp:ImageButton ID="imbAttachment" runat="server" tooltip="add or view attachments" ImageUrl="~/images/attach.png"  OnClientClick="PopupCenter('../Shared/Shared_Attach.aspx', 'newPage', 800, 600);  return false;" />
</asp:Panel>

<asp:Panel ID="pnlDisplayAttachments" runat="server" Visible ="false">
    <asp:Repeater ID="rptAttachments" runat="server" OnItemDataBound="rptAttachList_OnItemDataBound">
		<ItemTemplate>
			<div id="divAttachment" runat="server">
                <a href='<%# "/Shared/FileHandler.ashx?DOC=a&DOC_ID="+ Eval("ATTACHMENT_ID").ToString() + "&FILE_NAME=" + Eval("FILE_NAME").ToString() %>'
				    class="distributionImageContainerTall" target="_blank"> <%# DataBinder.Eval(Container.DataItem, "FILE_DESC").ToString() %>
			        <asp:Image ID="imgBindAttachment" CssClass="distributionImage"										
			            ImageUrl='<%# "~/Shared/FileHandler.ashx?DOC=a&DOC_ID=" + Eval("ATTACHMENT_ID").ToString() + "&FILE_NAME=" + Eval("FILE_NAME").ToString() %>' runat="server" />
                    </a>
			</div>
		</ItemTemplate>
	</asp:Repeater>
</asp:Panel>

<asp:Panel ID="pnlDisplayAttachmentsSmall" runat="server" Visible ="false">
    <asp:Repeater ID="rptAttachmentsSmall" runat="server" OnItemDataBound="rptAttachList_OnItemDataBound">
		<ItemTemplate>
			<div id="divAttachment" runat="server">
                <a href='<%# "/Shared/FileHandler.ashx?DOC=a&DOC_ID="+ Eval("ATTACHMENT_ID").ToString() + "&FILE_NAME=" + Eval("FILE_NAME").ToString() %>'
				    class="distributionImageContainerSmall" target="_blank"> <%# DataBinder.Eval(Container.DataItem, "FILE_DESC").ToString() %>
			        <asp:Image ID="imgBindAttachment" CssClass="distributionImageSmall"										
			            ImageUrl='<%# "~/Shared/FileHandler.ashx?DOC=a&DOC_ID=" + Eval("ATTACHMENT_ID").ToString() + "&FILE_NAME=" + Eval("FILE_NAME").ToString() %>' runat="server" />
                    </a>
			</div>
		</ItemTemplate>
	</asp:Repeater>
</asp:Panel>

<asp:Panel ID="pnlListAttachment" runat="server" Visible ="false" class="listingImageContainerTop">
    <asp:Repeater ID="rptListAttachment" runat="server" OnItemDataBound="rptAttachList_OnItemDataBound">
		<ItemTemplate>
			<div id="divAttachment" runat="server" class="listingImageContainer">
                <a href='<%# "/Shared/FileHandler.ashx?DOC=a&DOC_ID="+ Eval("ATTACHMENT_ID").ToString() + "&FILE_NAME=" + Eval("FILE_NAME").ToString() %>'
				    style="text-decoration: underline;" target="_blank"><%-- <%# DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString() %>--%>
			            <asp:Image ID="imgBindAttachment" CssClass="listingImage" ToolTip="click to view full size"									
			                ImageUrl='<%# "~/Shared/FileHandler.ashx?DOC=a&DOC_ID=" + Eval("ATTACHMENT_ID").ToString() + "&FILE_NAME=" + Eval("FILE_NAME").ToString() %>' runat="server" />
                </a>
			</div>
		</ItemTemplate>
	</asp:Repeater>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winManageAttachments" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="350px" Width="500px" Title="Manage Attachments" Behavior="Close, Move">
	<ContentTemplate>
		<div style="margin: 5px;">
			<asp:Label ID="lblManageAttachments" runat="server" CssClass="prompt"></asp:Label>
			<div id="divUpload" runat="server" style="margin-top: 5px;">
				<Ucl:RadUpload ID="uclUpload" runat="server" />
			</div>
		</div>
		<div style="float: left; margin-left: 10px;">
			<span>
				<asp:Button ID="btnSave" CSSclass="buttonStd buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin: 5px;" Onclick="btnSave_Click"></asp:Button>
				<asp:Button ID="btnCancel" CSSclass= "buttonEmphasis buttonPopupClose" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="btnCancel_Click"></asp:Button>
			</span>
		</div>
	</ContentTemplate>
</telerik:RadWindow>

