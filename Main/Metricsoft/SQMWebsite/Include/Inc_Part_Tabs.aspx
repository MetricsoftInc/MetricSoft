                           <table width="100%" border="0" cellspacing="0" cellpadding="0" id="admin_tabs">
                                <tr align="center" height="24">
                                    <td class="mainMenu" width="15%" nowrap>
                                         <asp:LinkButton ID="lbPartDetail_tab" runat="server" class="mainNav clickable" CommandArgument="edit" 
                                             onclick="tab_Click">Details</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="15%" nowrap>
                                         <asp:LinkButton ID="lbPartBuyer_tab" runat="server" class="mainNav clickable" CommandArgument="buyer" 
                                             onclick="tab_Click">Buyer-Supplier</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu"  width="15%" nowrap>
                                        <asp:LinkButton ID="lbPartUsed_tab" runat="server" class="mainNav clickable" CommandArgument="plant" 
                                            onclick="tab_Click">Where Used</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="15%" nowrap>
                                        <asp:LinkButton ID="lbPartCust_tab" runat="server" class="mainNav clickable" CommandArgument="cust" 
                                            onclick="tab_Click">Supplied To</asp:LinkButton>
                                    </td>

                                    <td width="15%" nowrap>
                                        <asp:LinkButton ID="lbPartBOM_tab" runat="server" class="mainNav clickable" CommandArgument="bom" 
                                            onclick="tab_Click">Assembly</asp:LinkButton>
                                    </td>

                                    <td width="90%">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>