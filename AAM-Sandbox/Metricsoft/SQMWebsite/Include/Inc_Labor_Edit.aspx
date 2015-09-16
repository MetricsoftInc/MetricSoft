                  <tr>
                     <td class="editArea">
                         <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD ALIGN=right>
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td>
                                            <asp:Button ID="lbLaborCancel1" CSSclass="buttonStd" runat="server" text="Cancel" 
                                             onclick="lbLaborSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>  
                                        <td>
                                            <asp:Button ID="lbLaborSave1" CSSclass="buttonEmphasis" runat="server" text="Save" 
                                            OnClientClick="return confirmChange('Labor Type');" onclick="lbLaborSave_Click" CommandArgument="edit"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </TD>
                        </TR> 
                    </table>
                  </td>
               </tr>
               <tr>
                <td class="editArea">
                    <table width="98%" align="center" border="0" cellspacing="1" cellpadding="3" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="39%">
                                <asp:Label ID="lblLaborName" runat="server" text="Labor Name"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td CLASS="tableDataAlt" width="60%">
                                <asp:TextBox ID="tbLaborName" size="50" maxlength="255" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblLaborCode" runat="server" text="Labor Code"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbLaborCode" size="30" maxlength="24" runat="server"/></td>
                        </tr>
                       	<tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblLaborRate" runat="server" text="Labor Rate"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbLaborRate" size="20" maxlength="20" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblSetLaborStatus" runat="server" text="Status"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlLaborStatus" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                            <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblLaborUpdatedBy" Text="Updated By" runat="server"/>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData">
                                <asp:Label ID="lblLaborLastUpdate" Text="" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader"><asp:Label ID="lblLaborUpdatedDate" Text="Last Update Date" runat="server"/></td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblLaborLastUpdateDate" Text="" runat="server"/></td>
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
                                             <asp:Button ID="lbLaborCancel2" CSSclass="buttonStd" runat="server" text="Cancel" 
                                             onclick="lbLaborSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>  
                                        <td>
                                            <asp:Button ID="lbLaborSave2" CSSclass="buttonEmphasis" runat="server" text="Save" 
                                            OnClientClick="return confirmChange('Labor Type');" onclick="lbLaborSave_Click" CommandArgument="edit"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </TD>
                        </TR> 
                    </table>
                </td>
              </tr>