<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_ViewPart.aspx.cs" Inherits="SQM.Website.Administrate_ViewPart" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

<script type="text/javascript">

    function comboLoad(sender) {
        var item = sender.get_items().getItem(0),
            checkBoxElement = item.get_checkBoxElement(),
            itemParent = checkBoxElement.parentNode;

        itemParent.removeChild(checkBoxElement);

        item = sender.get_items().getItem(4),
            checkBoxElement = item.get_checkBoxElement(),
            itemParent = checkBoxElement.parentNode;

        itemParent.removeChild(checkBoxElement);
    }

    function OpenPartDetailWindow() {
        $find("<%=winPartDetail.ClientID %>").show();
    }

    function ClosePartDetailWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window
        oWindow.Close();
    }

</script>

 <div class="admin_tabs">
    <table width="100%" border="0" cellspacing="0" cellpadding="1">
        <tr>
            <td class="tabActiveTableBg" colspan="10" align="center">
			    <BR/>
                <asp:HiddenField ID="hfBase" runat="server" />
                <asp:HiddenField id="hfRequiredInputs" runat="server" Value="Please enter all required (*) fields before saving."/>
                <asp:Panel runat="server" ID="pnlSearchBar" style="margin-right: 20px;">
                    <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                </asp:Panel>
                <table width="99%">
			        <tr>
                        <td  align="left">
                            <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Identify parts and view where they are used within the organization. Search for parts by supplier source and/or end customer."></asp:Label>
                            <asp:Label ID="lblViewPartTitle" runat="server" Text="Maintain Parts" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </table>

                <asp:Panel ID="pnlSearchList" runat="server" Style="margin-top: 4px;">
                    <table cellspacing=0 cellpadding=2 border=0 width="100%">
                        <tr>
                                <td class=summaryData valign=top>
				                <SPAN CLASS=summaryHeader>
                                    <asp:Label ID="lblFilterBySupplier" runat="server" Text="Part Source" ></asp:Label>
                                </SPAN>
                                <BR>
                                    <telerik:RadComboBox ID="ddlSourceSelect" runat="server" Width="200" Skin="Metro" ZIndex=9000 Font-Size=Small EmptyMessage="<%$ Resources:LocalizedText, Select %>" AutoPostBack="true">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="" Value=""/>
                                            <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="-1"/>
                                            <telerik:RadComboBoxItem Text="Internal" Value="0"/>
                                            <telerik:RadComboBoxItem Text="Any Supplier" Value="-2"/>
                                            <telerik:RadComboBoxItem Text="Specific Supplier" Value="-22" IsSeparator="true"/>
                                        </Items>
                                    </telerik:RadComboBox>
                            </td>
                            <td class=summaryData valign=top>
				                <SPAN CLASS=summaryHeader>
                                    <asp:Label ID="lblFilterByUsed" runat="server" Text="Where Used"></asp:Label>
                                </SPAN>
                                <BR>
                                <telerik:RadComboBox ID="ddlUsedSelect" runat="server" Width="400" Skin="Metro" ZIndex=9000 Font-Size=Small EmptyMessage="Select where part is used" AutoPostBack="false"
                                    CheckBoxes="true" EnableCheckAllItemsCheckBox="false" OnClientLoad="comboLoad">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="" Value=""/>
                                            <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="-1"/>
                                            <telerik:RadComboBoxItem Text="Internal" Value="0"/>
                                            <telerik:RadComboBoxItem Text="Any Customer" Value="-2" />
                                            <telerik:RadComboBoxItem Text="Specific Customer" Value="-22" IsSeparator="true"/>
                                        </Items>
                                </telerik:RadComboBox>
                                &nbsp;&nbsp;
                                <asp:Button id="btnPartSearch" runat="server" CssClass="buttonEmphasis" text="<%$ Resources:LocalizedText, Search %>" OnClick="OnPartSearch"/>
                                &nbsp;&nbsp;
                                <asp:Button ID="btnPartNew" CSSclass="buttonAddLarge" runat="server" text="<%$ Resources:LocalizedText, New %>" onclick="OnPartAdd" ToolTip="Add a part associated with the trading partners selected"></asp:Button>
                            </td>
                            <td class=summaryData valign=top>
				                <SPAN CLASS=summaryHeader>
                                    <asp:Label ID="lblPartCount_hdr" runat="server" Text="Part Count"></asp:Label>
                                </SPAN>
                                <BR>
                                <asp:Label ID="lblPartCount" runat="server" CssClass="textStd"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <Ucl:PartList id="uclSearchList" runat="server"/>
                </asp:Panel>

                <telerik:RadWindow runat="server" ID="winPartDetail"  Skin="Metro" Modal="true" Height="325" Width="550" Title="Part Details" Behaviors="Move">
                    <ContentTemplate>
                        <asp:Panel id="pnlPartDetail" style="margin-top: 10px;" runat="server">
                            <table width="99%" align="center" border="0" cellspacing="1" cellpadding="2" class="borderSoft">
                                <tr>
                                    <td class="columnHeader" width="30%">
                                        <asp:Label ID="lblPartNum" runat="server" text="Part Number"></asp:Label>
                                    </td>
                                    <td class="required" width="1%">&nbsp;</td>
                                    <td CLASS="tableDataAlt" width="69%"><asp:TextBox ID="tbPartNumber" size="50" maxlength="50" runat="server" CssClass="textStd" ReadOnly="true"/></td>
			                    </tr>
                                <tr>
                                    <td class="columnHeader" >
                                        <asp:Label ID="lblPartName" runat="server" text="Part Description"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartName"  TextMode="multiline" rows="2" maxlength="300" runat="server" CssClass="textStd" style="width: 97%;"/></td>
			                    </tr>
                                <asp:PlaceHolder ID="plPartName" runat="server" Visible="false">
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartPrefix" runat="server" text="Part Prefix"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartPrefix" size="10" maxlength="20" runat="server" CssClass="textStd"/></td>
			                        </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartSuffix" runat="server" text="Part Suffix"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartSuffix" size="10" maxlength="20" runat="server" CssClass="textStd"/></td>
			                        </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartSep" runat="server" text="Separator"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartSep" size="3" maxlength="3" runat="server" CssClass="textStd"/></td>
			                        </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartSerialNum" runat="server" text="Serial Number"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartSerialNum" size="50" maxlength="100" runat="server" CssClass="textStd"/></td>
			                        </tr>
                                </asp:PlaceHolder>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblPartProgram" runat="server" text="Part Program"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td CLASS="tableDataAlt">
                                        <asp:DropDownList ID="ddlPartProgram" runat="server"></asp:DropDownList>
                                    </td>
			                    </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblPartRevision" runat="server" text="Revision Level or Number"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td CLASS="tableData">
                                        <asp:TextBox ID="tbPartRevision" runat="server" CssClass="textStd"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblSetPartStatus" runat="server" text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                    </td>
                                    <td class="required">&nbsp;</td>
                                    <td class="tableDataAlt"><asp:DropDownList ID="ddlPartStatus" runat="server"></asp:DropDownList></td>
                                </TR>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblPartUpdatedBy" runat="server" text="Updated By"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td CLASS="tableData"><asp:Label ID="lblPartLastUpdate" Text="" runat="server" CssClass="textStd"/></td>
                                </tr>
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblPartUpdatedDate" runat="server" text="Last Update Date"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td CLASS="tableData"><asp:Label ID="lblPartLastUpdateDate" Text="" runat="server" CssClass="textStd"/></td>
                                </tr>
                            </table>
                            <span style="float: right; margin: 5px;">
                                <asp:Button ID="lbCancelPart" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>"
                                    onclick="lbCancelPart_Click"></asp:Button>
                                <asp:Button ID="lbSavePart" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin-right: 20px;"
                                    OnClientClick="return confirmChange('this part');"  onclick="lbSavePart_Click"></asp:Button>
                            </span>
                            <br />
                            <center>
                                <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
                            </center>
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadWindow>
            </td>
        </tr>
    </table>
    <br>
</div>
</asp:Content>
