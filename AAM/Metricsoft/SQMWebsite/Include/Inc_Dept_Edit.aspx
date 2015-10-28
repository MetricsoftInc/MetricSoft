                    <tr>
                        <td class="editArea">
                         <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD ALIGN=right >
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td>
                                            <asp:Button ID="lbDeptCanncel1" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>"
                                             onclick="lbDeptSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>
                                        <td>
                                            <asp:Button ID="lbDeptSave1" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>"
                                            OnClientClick="return confirmChange('Department');" onclick="lbDeptSave_Click" CommandArgument="edit"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </TD>
                        </TR>
                    </table>
                   </td>
                  </tr>

                  <tr>
                    <td  class="editArea">
                    <table width="98%" align="center" border="0" cellspacing="1" cellpadding="3" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="39%">
                                <asp:Label ID="lblDeptName" runat="server" text="Department Name"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td CLASS="tableDataAlt" width="60%">
                                <asp:TextBox ID="tbDeptName" size="50" maxlength="255" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblDeptCode" runat="server" text="Department Code"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbDeptCode" size="30" maxlength="24" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblSetDeptStatus" runat="server" text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlDeptStatus" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                            <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblDeptUpdatedBy" Text="Updated By:" runat="server"/>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData">
                                <asp:Label ID="lblDeptLastUpdate" Text="" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader"><asp:Label ID="lblDeptUpdatedDate" Text="Last Update Date" runat="server"/></td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblDeptLastUpdateDate" Text="" runat="server"/></td>
                        </tr>
                        </table>
                        </td>
                        </tr>
                       <tr>
                       <td class="editArea">
                      <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD ALIGN=right>
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td>
                                             <asp:Button ID="lbDeptCancel2" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>"
                                             onclick="lbDeptSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>
                                        <td>
                                            <asp:Button ID="lbDeptSave2" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>"
                                             OnClientClick="return confirmChange('Department');" onclick="lbDeptSave_Click" CommandArgument="edit"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </TD>
                        </TR>
                    </table>
                    </td>
                    </tr>