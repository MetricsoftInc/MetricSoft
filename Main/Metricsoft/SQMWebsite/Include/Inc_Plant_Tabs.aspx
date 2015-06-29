                           <table width="100%" border="0" cellspacing="0" cellpadding="0" id="admin_tabs">
                                <tr align="center" height="24">
                                   <td class="mainMenu" width="12%" nowrap>
                                         <asp:LinkButton ID="lbPLantDetail_tab" runat="server" class="mainNav clickable" CommandArgument="edit" 
                                            onclick="tab_Click">Details</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="12%" nowrap>
                                         <asp:LinkButton ID="lbPLantDepartment_tab" runat="server" class="mainNav clickable" CommandArgument="dept" 
                                            onclick="tab_Click">Departments</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="15%" nowrap>
                                         <asp:LinkButton ID="lbPlantLine_tab" runat="server" class="mainNav clickable" CommandArgument="line" 
                                             onclick="tab_Click">Plant Lines</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="15%" nowrap>
                                        <asp:LinkButton ID="lbPlantLabor_tab" runat="server" class="mainNav clickable" CommandArgument="labor" 
                                            onclick="tab_Click">Labor</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="15%" nowrap>
                                        <asp:LinkButton ID="lbPlantCustCode_tab" runat="server" class="mainNav clickable" CommandArgument="cust" 
						                    onclick="tab_Click">Customer Codes</asp:LinkButton>
                                    </td>

                                    <td width="15%" nowrap>
                                        <asp:LinkButton ID="lbPlantPart_tab" runat="server" class="mainNav clickable" CommandArgument="part" 
						                    onclick="tab_Click">Parts</asp:LinkButton>
                                    </td>

                                    <td width="90%">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>