                           <table width="100%" border="0" cellspacing="0" cellpadding="0" id="admin_tabs">
                                <tr align="center">
                                    <div ID="divUserContactTab" runat="server">
                                        <td class="tab_left">
                                            <img src="/images/tabActiveFirstLeft.gif" alt="" width="8" height="24" border="0">
                                        </td>
                                        <td background="/images/tabActiveBkgd.gif" class="tabInactiveLink tab_middle" width="10%" nowrap>
                                         <asp:LinkButton ID="lbUserContact" runat="server" class="tab clickable" CommandArgument="identify" 
                                            onclick="tab_Click">Identification</asp:LinkButton>
                                        </td>
                                        <td width="8" class="tab_right">
                                        <img src="/images/tabActiveRight.gif" width="8" height="24">
                                        </td>
                                    </div>

                                    <div ID="divUserPrefsTab" runat="server">
                                        <td width="10" class="tab_left">
                                            <img src="/images/tabInactiveLeft.gif" alt="" width="8" height="24" border="0">
                                        </td>
                                        <td background="/images/tabInactiveBkgd.gif" class="tabInactiveLink tab_middle" width="15%" nowrap>
                                            <asp:LinkButton ID="lbUserPreference" runat="server" class="tab clickable" CommandArgument="evidence" 
                                             onclick="tab_Click">Evidence</asp:LinkButton>
                                        </td>
                                        <td width="8" class="tab_right">
                                            <img src="/images/tabInactiveRight.gif" width="8" height="24">
                                        </td>
                                    </div>

                                    <div ID="divUserDelegateTab" runat="server">
                                        <td width="10" class="tab_left">
                                            <img src="/images/tabInactiveLeft.gif" alt="" width="8" height="24" border="0">
                                        </td>
                                        <td background="/images/tabInactiveBkgd.gif" class="tabInactiveLink tab_middle" width="15%" nowrap>
                                            <asp:LinkButton ID="lbUserDelegate" runat="server" class="tab clickable" CommandArgument="label" 
                                             onclick="tab_Click">Label</asp:LinkButton>
                                        </td>
                                        <td width="8" class="tab_right">
                                            <img src="/images/tabInactiveRight.gif" width="8" height="24">
                                        </td>
                                    </div>
                                    <td width="90%">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>