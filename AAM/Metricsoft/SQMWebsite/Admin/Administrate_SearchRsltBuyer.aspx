<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_SearchRsltBuyer.aspx.cs" Inherits="SQM.Website.Administrate_SearchRsltBuyer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
  <asp:HiddenField ID="hfBase" runat="server" />
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
			            <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblBuyerSearchTitle" runat="server" Text="Buyer Search Results"></asp:Label>
                            </td>
					    </tr>
				    </table><BR/>

<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="1" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="30%">
                                <b>New Search </b>
		                    </td>
                            <td class="tabActiveTableBg" width="70%">
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>&nbsp;
                                <asp:Button ID="lbSearchBuyer" runat="server" Text="Go" CSSclass="buttonStd" onclick="lbSearchBuyer_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                    <br>

       	            <table width="98%" cellpadding="3" cellspacing="1" border="0" class="darkBorder">
                        <TR>
		                    <TD class="tableDataHdr" colspan="5">
			                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
				                    <tr>
					                    <td class="tableDataHdr2">
                                            <asp:Label runat="server" ID="lblBuyerResultsHdr" Text="Results" Visible="true"></asp:Label>
                                        </td>
	                                    <td align="right">
					                <!-- BUTTONS -->
					                        <table cellpadding=0 cellspacing=2 border=0>
						                        <tr>
                                                    <TD>
                                                        <asp:Button ID="lbBuyerAdmin" runat="server" onClick="lbBuyerAdmin_Click" CSSclass="buttonStd" text="Return to Trading Partner Admin"></asp:Button>
                                                    </TD>
                                                </tr>
                                            </table>
               			                <!-- END OF BUTTONS -->
					                    </td>
				                    </tr>
			                    </table>
                            </TD>
                        </TR>

                        <table width="98%" border="0" cellspacing="0" cellpadding="0">
                               <tr><td class="admBkgd">&nbsp;</td></tr>
		                            <tr>
                                       <td>
                                         <table width="100%" border="0" cellspacing="0" cellpadding="1">
			                                <tr>
			                                    <td class=admBkgd align=center>
                                                  <asp:GridView runat="server" ID="gvBuyerList" Name="gvBuyerList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="98%" OnRowDataBound="gvBuyerList_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellText" />
                                                    <RowStyle CssClass="DataCell" />
                	                                <Columns>
                                                        <asp:TemplateField HeaderText="Buyer Code" ItemStyle-Width="20%">
							                                <ItemTemplate>
								                                <asp:LinkButton ID="lnkViewBuyer_out" runat="server" CommandArgument='<%#Eval("PERSON_ID") %>'
										    	                    text='<%#Eval("BUYER_CODE") %>' OnClick="lnkBuyerView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                                            </ItemTemplate>
							                            </asp:TemplateField>
                                                        <asp:BoundField DataField="FIRST_NAME" HeaderText="First name" ItemStyle-Width="22%" />
                                                        <asp:BoundField DataField="LAST_NAME" HeaderText="Last Name" ItemStyle-Width="23%" />
                                                        <asp:BoundField DataField="NEW_LOCATION_CD" HeaderText="<%$ Resources:LocalizedText, Location %>" ItemStyle-Width="25%" />
                                                        <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Status %>" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                                                <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Label runat="server" ID="lblBuyerListEmpty" Height="40" Text="There are currently no Buyers defined." class="GridEmpty" Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                              </tr>
                        </table>

			        </table>
                </TD>
            </TR>
        </table>
        <br>
    </div>
</asp:Content>
