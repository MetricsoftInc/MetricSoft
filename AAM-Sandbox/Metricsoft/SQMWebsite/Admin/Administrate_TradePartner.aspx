<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_TradePartner.aspx.cs" Inherits="SQM.Website.Administrate_TradePartner" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <div class="admin_tabs">
      <table width="100%" border="0" cellspacing="0" cellpadding="1">
        <tr>
          <td class="tabActiveTableBg" colspan="10" align="center">
			<BR/>


                  <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
						  <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblTradePartnerAdminTitle" runat="server" Text="Trading Partners"></asp:Label>
                            </td>
						  </tr>
				  </table>
                  <BR/> 

				


					  <!-- DATA ENTRY CONTENT TABLE -->	
				
					  	 
            <TABLE WIDTH="98%" BORDER="0" CELLSPACING="1" CELLPADDING="4" class="darkBorder">
               <tr>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblSearchSupplierCompany" runat="server" Text="Search For Supplier Locations"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbSearchSupplierCompany" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                            </td>
                            <TD>
                             <asp:Button ID="btnSearchSupplierCompany" runat="server" CSSclass="buttonStd" text="Search" CommandArgument="supp" onclick="btnSearchSupplier_Click"></asp:Button>
                           </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
              <tr>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblSearchCustomerCompany" runat="server" Text="Search For Customer Companies"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbSearchCustomerCompany" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                            </td>
                            <TD>
                             <asp:Button ID="btnSearchCustomerCompany" runat="server" CSSclass="buttonStd" text="Search" CommandArgument="cust" onclick="btnSearchCustomer_Click"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <tr>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblSearchBuyer" runat="server" Text="Search For Buyer Codes"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                                <br />
                                 <asp:RadioButtonList runat="server" ID="rblAssignedBuyer" CSSclass="radioList" 
	                                RepeatDirection="Horizontal" RepeatLayout="flow" 
		                            AutoPostBack="false">
							            <asp:ListItem Text="All&nbsp;&nbsp;" Value="" ></asp:ListItem>
								        <asp:ListItem Text="Unassigned" Value="U"></asp:ListItem>
						        </asp:RadioButtonList>
                            </td>
                        <TD>
                             <asp:Button ID="lbSearchBuyer" runat="server" CSSclass="buttonStd" text="Search" onclick="lbSearchBuyer_Click"></asp:Button>
                        </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
           <TR> 
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblUploadData" runat="server" Text="Upload Buyer Codes"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING= CELLPADDING=5>
                        <TR>
                            <TD>
                                <asp:Button ID="lbUploadData" runat="server" CSSclass="buttonStd" 
                                    onclick="lbUploadData_Click" text="Upload Data"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>                   
            </TR>
          </TABLE>
        </td>
      </tr>
    </table>
    <BR>

   </div>
</asp:Content>
