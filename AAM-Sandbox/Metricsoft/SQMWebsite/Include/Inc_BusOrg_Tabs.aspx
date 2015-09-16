                           <table width="100%" border="0" cellspacing="0" cellpadding="0" id="admin_tabs">
                                <tr align="center" height="24">

                                    <td class="mainMenu" width="10%">
                                        <asp:LinkButton ID="lbBusOrgDetail_tab" runat="server" class="mainNav clickable"  CommandArgument="edit" 
                                             onclick="tab_Click">Details</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="12%">
                                            <asp:LinkButton ID="lbProductLine_tab" runat="server" class="mainNav clickable"  CommandArgument="prod" 
                                                onclick="tab_Click">Product Lines</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="12%">
                                         <asp:LinkButton ID="lbDepartment_tab" runat="server" class="mainNav clickable"  CommandArgument="dept" 
                                             onclick="tab_Click">Departments</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="10%">
                                        <asp:LinkButton ID="lbLabor_tab" runat="server" class="mainNav clickable"  CommandArgument="labor" 
                                            onclick="tab_Click">Labor</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="14%">
                                        <asp:LinkButton ID="lbNotify_tab" runat="server" class="mainNav clickable"  CommandArgument="notify" onclick="tab_Click">Notifications</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="14%" >
                                        <asp:LinkButton ID="lbTimer_tab" runat="server" class="mainNav clickable"  CommandArgument="timer" onclick="tab_Click">Workflow Settings</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="14%" >
                                        <asp:LinkButton ID="lbNoncf_tab" runat="server" class="mainNav clickable"  CommandArgument="nonconf" 
                                             onclick="tab_Click">Non-Conformance</asp:LinkButton>
                                    </td>

                                    <td class="mainMenu" width="10%">
                                         <asp:LinkButton ID="lbPlants_tab" runat="server" class="mainNav clickable" CommandArgument="plant" 
                                            onclick="tab_Click">Plants</asp:LinkButton>
                                    </td>
 
                                     <td width="90%">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>