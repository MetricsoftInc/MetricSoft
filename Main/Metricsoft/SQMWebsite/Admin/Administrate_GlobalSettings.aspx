<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_GlobalSettings.aspx.cs" Inherits="SQM.Website.Administrate_GlobalSettings" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocMgr" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:Panel runat="server" ID="pnlSearchBar" style="margin-right: 20px;">
                            <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                        </asp:Panel>

                        <table width="99%">
			                <tr>
                                <td>
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" style="margin-left: 10px;" Text="Define company-wide settings used when managing quality reports and problem cases."></asp:Label>
                                    <asp:Label ID="lblViewSettingsTitle" runat="server" Text="Company Profile - " Visible="false"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        
                        <div id="divPageBody" runat="server">
                            <table width="99%">
                                <tr>
                                    <td valign="top" align="center">
                                        <Ucl:ItemHdr id="uclItemHdr" runat="server"/>
                                    </td>
                                </tr>
                               
                                <tr>
                                    <td>
                                        <table width="100%" border="0" cellspacing="0" cellpadding="1">
                                            <tr>
                                                <td class="columnHeader" width="40%">
                                                    <asp:Label ID="lblIsCustomer" runat="server" text="Company may be a customer"></asp:Label>
                                                    &nbsp;
                                                    <asp:Image ID="imgIsCustomer" Visible="true" runat="server" ImageUrl = "~/images/icon_customer2.gif"/>
                                                </td>
                                                <td CLASS="tableDataAlt" width="60%">
                                                    <asp:CheckBox ID="cbIsCustomer" runat="server"/>
                                                </td>      
			                                </tr>
                                            <tr>
                                                <td class="columnHeader" width="40%">
                                                    <asp:Label ID="lblIsSupplier" runat="server" text="Company may be a supplier"></asp:Label>
                                                    &nbsp;
                                                    <asp:Image ID="Image1" Visible="true" runat="server" ImageUrl = "~/images/icon_supplier2.gif"/>
                                                </td>
                                                <td CLASS="tableDataAlt" width="60%">
                                                    <asp:CheckBox ID="cbIsSupplier" runat="server"/>
                                                </td>      
			                                </tr>
                                        </table>
                                        <br />

                                        <asp:Panel ID="pnlBusDocs" runat="server">
                                            <Ucl:DocMgr id="uclDocMgr" runat="server"/>
                                        </asp:Panel>
 
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>
</asp:Content>
