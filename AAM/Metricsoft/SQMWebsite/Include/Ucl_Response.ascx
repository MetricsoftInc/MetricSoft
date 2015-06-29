<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_Response.ascx.cs" Inherits="SQM.Website.Ucl_Response" EnableViewState="True" %>

<%@ Register src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="RadAsyncUpload" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Panel ID="pnlResponseList" runat="server" Visible="false">
	<table width="100%">
		<tr>
			<td>
				<div id="divGVScrollResponseList" runat="server">
					<asp:Repeater runat="server" ID="rptResponseList" ClientIDMode="AutoID" OnItemDataBound="rptResponseList_OnItemDataBound">
						<HeaderTemplate><table cellspacing="0" cellpadding="2" border="0" width="100%" class="rptDarkBkgd"></HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="listDataAlt" style="width: 20%; vertical-align: top;">
									<asp:Label runat="server" ID="lblPersonName_out" CssClass="refText"></asp:Label>
                                    <br />
									<asp:Label runat="server" ID="lblResponseDate_out" CssClass="refText"></asp:Label>
								</td>
								<td class="listDataAlt" valign="top" style="width: 80%;">
                                     <telerik:RadTextBox ID="rtResponseText_out" runat="server" Skin="Metro" MaxLength="4000" BorderStyle="Dotted" 
                                         TextMode="MultiLine" Rows="4" Width="99%" Wrap="true" EnableSingleInputRendering="false" ReadOnly="true" ForeColor="#B22222" ></telerik:RadTextBox>
								</td>
							</tr>
						</ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr>
								<td class="listData" style="width: 20%; vertical-align: top;">
									<asp:Label runat="server" ID="lblPersonName_out" CssClass="refText"></asp:Label>
                                    <br />
									<asp:Label runat="server" ID="lblResponseDate_out" CssClass="refText"></asp:Label>
								</td>
								<td class="listData" valign="top" style="width: 80%;">
                                     <telerik:RadTextBox ID="rtResponseText_out" runat="server" Skin="Metro" MaxLength="4000" BorderStyle="Dotted" 
                                         TextMode="MultiLine" Rows="4" Width="99%" Wrap="true" EnableSingleInputRendering="false" ReadOnly="true" ForeColor="#B22222" ></telerik:RadTextBox>
								</td>
							</tr>
                        </AlternatingItemTemplate>
						<FooterTemplate></table></FooterTemplate>
					</asp:Repeater>
				</div>
				<asp:Label runat="server" ID="lblResponseListEmpty" Text="There are no responses." class="GridEmpty" Visible="false"></asp:Label>
			</td>
		</tr>
        <tr>
            <td>
                <span>
                    <telerik:RadTextBox ID="rtResponseInput" runat="server" Skin="Metro" MaxLength="4000" TextMode="MultiLine" Rows="4" Width="97%" Wrap="true" EnableSingleInputRendering="false" 
                         EmptyMessage="Enter a response. Your response will be added when the page is saved."></telerik:RadTextBox>
                    <br />
                    <Ucl:RadAsyncUpload id="uclResponseAttach" runat="server"/>
                    <%--<asp:Button id="btnResponseInput" runat="server" CssClass="buttonStd" Text="Enter"/>--%>
                </span>
            </td>
        </tr>
	</table>
</asp:Panel>

