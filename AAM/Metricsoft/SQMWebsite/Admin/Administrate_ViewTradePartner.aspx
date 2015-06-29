<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_ViewTradePartner.aspx.cs" Inherits="SQM.Website.Administrate_ViewTradePartner" %>
<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
  <asp:HiddenField ID="hfBase" runat="server" />
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <table width="99%" border="0" cellspacing="0" cellpadding="0" align="center">
			            <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblViewTradePartnerTitle" runat="server" Text="Trading Partners - "></asp:Label>
                            </td>
					    </tr>
				    </table><BR/> 

                    <table width="99%" border="0" cellspacing="0" cellpadding="1" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="30%">
                                <asp:Label ID="lblSearchSupplier" runat="server" Text="Search For Supplier Locations"></asp:Label>
                                <asp:Label ID="lblSearchCustomer" runat="server" Text="Search For Customer Companies"></asp:Label>
		                    </td>
                            <td class="tabActiveTableBg" width="70%">
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>&nbsp;
                                <asp:Button ID="btnSearchCompany" runat="server" Text="Search" CSSclass="buttonStd" onclick="lbSearchCompany_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                    <br>

       	            <table width="99%" cellpadding="3" cellspacing="1" border="0" class="darkBorder">
                        <TR> 
		                    <TD class="tableDataHdr" colspan="5">
			                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
				                    <tr>
					                    <td class="tableDataHdr2">
                                            <asp:Label runat="server" ID="lblSupplierListHdr" Text="Supplier Plants List" Visible="true"></asp:Label>
                                            <asp:Label runat="server" ID="lblCustomerListHdr" Text="Customer Companies List" Visible="true"></asp:Label>
                                        </td>
	                                    <td align="right">
					                <!-- BUTTONS -->		
					                        <table cellpadding=0 cellspacing=2 border=0>
						                        <tr>
                                                    <TD>
                                                        <asp:Button ID="lbTradePartnerAdmin" runat="server" onClick="lbTradePartnerAdmin_Click" CSSclass="buttonStd" text="Return to Trading Partner Admin"></asp:Button>
                                                    </TD>
                                                </tr>
                                            </table>
               			                <!-- END OF BUTTONS -->
					                    </td>
				                    </tr>
			                    </table>
                            </TD>
                        </TR>
                       

                            <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                <asp:Panel ID="pnlB2BSupplier" runat="server" Visible="false">
                                <tr>
                                    <td class="editArea" colspan="10">
				                        <table width="99%" border="0" cellspacing="0" cellpadding="1">
			                                <tr>
                                                <td>
                                                    <asp:Label ID="lblTradePartnerInstruct1" runat="server" class="instructText" Text="Enable or disble problem case reporting to supplier plants for this company by selecting the corresponding checkbox."></asp:Label>
                                                </td>
			  	                                <td align=right>
					                                <table cellpadding=0 cellspacing=2 border=0>
						                                <tr>
							                                <td>
                                                                <asp:Button ID="btnTradePartnerCancel1" CSSclass="buttonStd" runat="server" text="Cancel" 
                                                             onclick="btnTradePartnerCancel_Click" CommandArgument="edit" UseSubmitBehavior="false"></asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnTradePartnerSave1" CSSclass="buttonEmphasis" runat="server" text="Save" OnClientClick="return confirmChange('Trading Partner Assignments');" onclick="btnTradePartnerSave_Click"></asp:Button>
                                                            </td>
						                                </tr>
					                                </table>
				                                </td>
			                                </tr>
                                            <tr>
                                                <td align=left>
                                                    <asp:CheckBox ID="cbTradeParterSelectAll" runat="server" AutoPostBack="true" />
                                                    <asp:Label ID="lblTradePartnerSelectAll" runat="server" CSSClass="prompt" Text="Select All"></asp:Label>
                                                    &nbsp;&nbsp;
                                                    <asp:CheckBox ID="cbTradePartnerDeselectAll" runat="server" AutoPostBack="true" />
                                                    <asp:Label ID="lblTradePartnerDeselectAll" runat="server" CSSClass="prompt" Text="Deselect All"></asp:Label>
                                                </td>
                                            </tr>
			                            </table>
                                    </td>
                                </tr>
                                </asp:Panel>

                                <asp:Panel ID="pnlB2BCustomer" runat="server" Visible="false">
                                <tr>
                                    <td class="editArea" colspan="10">
				                        <table width="99%" border="0" cellspacing="0" cellpadding="1">
			                                <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" class="instructText" Text="Include or exclude customers as impacted on problem reports by selecting the corresponding checkbox."></asp:Label>
                                                </td>
			  	                                <td align=right>
					                                <table cellpadding=0 cellspacing=2 border=0>
						                                <tr>
							                                <td>
                                                                <asp:Button ID="btnTradePartnerCancel2" CSSclass="buttonStd" runat="server" text="Cancel" 
                                                             onclick="btnTradePartnerCancel_Click" CommandArgument="edit" UseSubmitBehavior="false"></asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnTradePartnerSave2" CSSclass="buttonEmphasis" runat="server" text="Save" OnClientClick="return confirmChange('Trading Partner Assignments');" onclick="btnTradePartnerSave_Click"></asp:Button>
                                                            </td>
						                                </tr>
					                                </table>
				                                </td>
			                                </tr>
                                           <tr>
                                                <td align=left>
                                                    <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true" />
                                                    <asp:Label ID="Label2" runat="server" CSSClass="prompt" Text="Select All"></asp:Label>
                                                    &nbsp;&nbsp;
                                                    <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="true" />
                                                    <asp:Label ID="Label3" runat="server" CSSClass="prompt" Text="Deselect All"></asp:Label>
                                                </td>
                                            </tr>
			                            </table>
                                    </td>
                                </tr>
                                </asp:Panel>
                                <Ucl:PartList id="uclPartList" runat="server"/>
                            </table>
                       
			        </table>
                </TD>
            </TR>
        </table>
        <br>
    </div>
</asp:Content>
