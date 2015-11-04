<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_B2BList.ascx.cs" Inherits="SQM.Website.Ucl_B2BList" %>

<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Panel ID="pnlCustPartList" runat="server" Visible="false">
    <asp:Repeater runat="server" ID="rptCustPartHeader" ClientIDMode="AutoID" OnItemDataBound="rptCustPartHeader_OnItemDataBound">
	    <HeaderTemplate>
	    <table cellspacing="0" cellpadding="0" border="0" width="100%" >
	    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td id="tdLocation" runat="server" class="rptInputTableEnd" style="width: 30%; vertical-align: top; padding-top: 5px; padding-left: 3px;">
			            <asp:Label ID="lblLocation" runat="server" CSSClass="prompt"></asp:Label>
                        <asp:HiddenField id="hfLocationID" runat="server"/>
				    </td>
                    <td class="rptInputTable" valign="top" style="width: 70%;">
                        <asp:Repeater runat="server" ID="rptCustPartDetail" ClientIDMode="AutoID">
                            <HeaderTemplate>
				                <table cellspacing="1" cellpadding="1" border="0" width="100%" >
				            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="columnHeader" width="30%">
                                        <asp:Label runat="server" ID="lblLocationCodeHdr" cssclass="prompt" Text="<%$ Resources:LocalizedText, LocationCode %>"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt" width="70%">
                                        <asp:Label ID="lblLocationCode" runat="server" CssClass="textStd"></asp:Label>
			                        </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label runat="server" ID="lblParentCompanyHdr" cssclass="prompt" Text="Parent Company" ></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">
                                            <asp:Label ID="lblParentCompany" runat="server" CssClass="textStd"></asp:Label>
			                        </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:HiddenField  ID="hfQSNotify" runat="server"/>
                                        <asp:Label runat="server" ID="lblQSNotify1" cssclass="prompt" Text="Notify User When Quality Issue Is Created" ></asp:Label>
                                        <asp:LinkButton ID="lnkSaveCust" runat="server" style="float:right; margin-right: 5px;" CSSClass="buttonLink"  OnClick="lnkSaveCust" ToolTip="Save user selections">
                                                <img src="/images/defaulticon/16x16/save.png" alt="" style="vertical-align: middle; border: 0px;" />
                                        </asp:LinkButton>
                                    </td>
                                    <td class="tableDataAlt">
                                        <telerik:RadComboBox ID="ddlQSNotify1" runat="server" Width="220" Skin="Metro" ZIndex=9000 Font-Size=Small EmptyMessage="Select person to notify"></telerik:RadComboBox>
			                            <br />
                                         <telerik:RadComboBox ID="ddlQSNotify2" runat="server" Width="220" Skin="Metro" ZIndex=9000 Font-Size=Small EmptyMessage="Select person to notify"></telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label runat="server" ID="lblPartListHdr" cssclass="prompt" Text="Parts" ></asp:Label>
                                        <asp:LinkButton ID="lnkPartList" runat="server" style="float:right; margin-right: 5px;" CSSClass="buttonLink"  OnClick="lnkDisplayCustParts" ToolTip="Display Parts List">
                                                <img src="/images/defaulticon/16x16/list-ordered.png" alt="" style="vertical-align: middle; border: 0px;" />
                                        </asp:LinkButton>
								        <asp:CheckBox id="cbPartListSelect" runat="server" Visible="false"/>
                                    </td>
                                    <td class="tableDataAlt">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr style="height: 1px;">
                                    <td colspan="2" >
                                        <Ucl:PartList id="uclPartList" runat="server"/>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate></table></FooterTemplate>
                        </asp:Repeater>
                    </td>
			    </tr>
                <tr style="height: 5px;">
                    <td colspan="5"></td>
                </tr>
		    </ItemTemplate>
		    <FooterTemplate>
	    </table></FooterTemplate>
    </asp:Repeater>
    <asp:Label ID="lblCustListEmpty" runat="server" Visible="false" CssClass="textStd" Text="There are no customers defined for the specified criteria."></asp:Label>
</asp:Panel>


<asp:Panel ID="pnlSuppPartList" runat="server" Visible="false">
    <asp:Repeater runat="server" ID="rptSuppPartHeader" ClientIDMode="AutoID" OnItemDataBound="rptSuppPartHeader_OnItemDataBound">
	    <HeaderTemplate>
	    <table cellspacing="0" cellpadding="0" border="0" width="100%" >
	    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td id="tdLocation" runat="server" class="rptInputTableEnd" style="width: 30%; vertical-align: top; padding-top: 5px; padding-left: 3px;">
			            <asp:Label ID="lblLocation" runat="server" CSSClass="prompt"></asp:Label>
                        <asp:HiddenField id="hfLocationID" runat="server"/>
				    </td>
                    <td class="rptInputTable" valign="top" style="width: 70%;">
                        <asp:Repeater runat="server" ID="rptSuppPartDetail" ClientIDMode="AutoID">
                            <HeaderTemplate>
				                <table cellspacing="1" cellpadding="1" border="0" width="100%" >
				            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="columnHeader" width="30%">
                                        <asp:Label runat="server" ID="lblLocationCodeHdr" cssclass="prompt" Text="<%$ Resources:LocalizedText, LocationCode %>"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt" width="70%">
                                        <asp:Label ID="lblLocationCode" runat="server" CssClass="textStd"></asp:Label>
			                        </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label runat="server" ID="lblParentCompanyHdr" cssclass="prompt" Text="Parent Company" ></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">
                                            <asp:Label ID="lblParentCompany" runat="server" CssClass="textStd"></asp:Label>
			                        </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:HiddenField  ID="hfQSNotify" runat="server"/>
                                        <asp:Label runat="server" ID="lblQSNotify1" cssclass="prompt" Text="Notify User When Quality Issue Is Created" ></asp:Label>
                                        <asp:LinkButton ID="lnkSaveSupp" runat="server" style="float:right; margin-right: 5px;" CSSClass="buttonLink"  OnClick="lnkSaveSupp" ToolTip="Save user selections">
                                                <img src="/images/defaulticon/16x16/save.png" alt="" style="vertical-align: middle; border: 0px;" />
                                        </asp:LinkButton>
                                    </td>
                                    <td class="tableDataAlt">
                                        <telerik:RadComboBox ID="ddlQSNotify1" runat="server" Width="220" Skin="Metro" ZIndex=9000 Font-Size=Small EmptyMessage="Select person to notify"></telerik:RadComboBox>
			                            <br />
                                         <telerik:RadComboBox ID="ddlQSNotify2" runat="server" Width="220" Skin="Metro" ZIndex=9000 Font-Size=Small EmptyMessage="Select person to notify"></telerik:RadComboBox>
                                    </td>
                                </tr>
                                </tr>
                                    <td class="columnHeader">
                                        <asp:Label runat="server" ID="lblPartListHdr" cssclass="prompt" Text="Parts" ></asp:Label>
                                        <asp:LinkButton ID="lnkPartList" runat="server" style="float:right; margin-right: 5px;" CSSClass="buttonLink"  OnClick="lnkDisplaySuppParts" ToolTip="Display Parts List">
                                                <img src="/images/defaulticon/16x16/list-ordered.png" alt="" style="vertical-align: middle; border: 0px;" />
                                        </asp:LinkButton>
								        <asp:CheckBox id="cbPartListSelect" runat="server" Visible="false"/>
                                    </td>
                                    <td class="tableDataAlt">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr style="height: 1px;">
                                    <td colspan="2" >
                                        <Ucl:PartList id="uclPartList" runat="server"/>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate></table></FooterTemplate>
                        </asp:Repeater>
                    </td>
			    </tr>
                <tr style="height: 5px;">
                    <td colspan="5"></td>
                </tr>
		    </ItemTemplate>
		    <FooterTemplate>
	    </table></FooterTemplate>
    </asp:Repeater>
    <asp:Label ID="lblSuppListEmpty" runat="server" Visible="false" CssClass="textStd" Text="There are no suppliers defined for the specified criteria."></asp:Label>
</asp:Panel>

<asp:Panel ID="pnlReceiptList" runat="server" Visible="false" Width="99%">
    <telerik:RadGrid ID="rgReceiptList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
        AutoGenerateColumns="false" OnItemDataBound="rgReceiptList_ItemDataBound" OnSortCommand="rgReceiptList_SortCommand"
        OnPageIndexChanged="rgReceiptList_PageIndexChanged" OnPageSizeChanged="rgReceiptList_PageSizeChanged" GridLines="None" Width="100%">
        <MasterTableView ExpandCollapseColumn-Visible="false">
            <Columns>
		        <telerik:GridTemplateColumn ShowSortIcon="true" SortExpression="Receipt.RECEIPT_NUMBER">
                    <ItemTemplate>
                        <asp:Label ID="lblReceiptNum" runat="server" Text='<%#Eval("Receipt.RECEIPT_NUMBER") %>'></asp:Label>
                        <br />
                        <span style="white-space: nowrap;">
                           <asp:LinkButton ID="lbIssueId" CssClass="buttonEditRightBold" runat="server" ToolTip="Edit issue" OnClick="lnkReceipt_Click">
                            </asp:LinkButton>
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Receipt Date" ShowSortIcon="true" SortExpression="Receipt.RECEIPT_DT">
                    <ItemTemplate>
                        <asp:Label ID="lblIncidentDT" Text='<%# ((DateTime)Eval("Receipt.RECEIPT_DT")).ToShortDateString() %>' runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Received Location/<br>Supplier Location" ShowSortIcon="true" SortExpression="CustomerPlant.PLANT_NAME">
                    <ItemTemplate>
                        <asp:Label ID="lblLocation" runat="server" Text='<%#Eval("CustomerPlant.PLANT_NAME") %>'></asp:Label>
                        <br />
                        <span style="white-space: nowrap;">
                            <asp:Image ID="imgRespLocation" Visible="true" runat="server" style="vertical-align:middle;" ImageUrl = "~/images/icon_supplier2.gif" ToolTip="supplier location"/>
                            <asp:Label ID="lblRespLocation" runat="server" Text='<%#Eval("SupplierPlant.PLANT_NAME") %>'></asp:Label>
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
		        <telerik:GridTemplateColumn UniqueName="PartColumn" HeaderText="<%$ Resources:LocalizedText, PartNumber %>" ShowSortIcon="true" SortExpression="Part.PART_NUM">
                    <ItemTemplate>
                        <asp:Label ID="lblPartNum" runat="server" Text='<%#Eval("Part.PART_NUM") %>'></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Qty Received/<br>Value" ShowSortIcon="false">
                    <ItemTemplate>
                        <asp:Label ID="lblReceiptQty" runat="server" Text='<%#Eval("Receipt.RECEIPT_QTY") %>'></asp:Label>
                            <br />
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblReceiptCost" runat="server" Text='<%#Eval("Receipt.RECEIPT_COST") %>'></asp:Label>
                            &nbsp;
                            (<asp:Label ID="lblReceiptCurrency" runat="server" Text='<%#Eval("Receipt.CURRENCY_CODE") %>'></asp:Label>)
                            </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>
</asp:Panel>