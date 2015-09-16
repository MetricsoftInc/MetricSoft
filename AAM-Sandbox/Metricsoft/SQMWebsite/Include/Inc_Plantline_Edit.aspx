                  <tr>
                     <td class="editArea">
                         <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD ALIGN=right class="editArea">
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td>
                                            <asp:Button ID="lbLineCancel1" class="buttonStd" runat="server" text="Cancel" 
                                             onclick="lbLineSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>  
                                        <td>
                                            <asp:Button ID="lbLineSave1" CSSclass="buttonEmphasis" runat="server" text="Save" 
                                             onclick="lbLineSave_Click" OnClientClick="return confirmChange('Plant Line');" CommandArgument="edit"></asp:Button>
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
                                <asp:Label ID="lblLineName" runat="server" text="Line Name"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td CLASS="tableDataAlt" width="60%">
                                <asp:TextBox ID="tbLineName" size="50" maxlength="255" runat="server"/>
                            </td>
                        </tr>
                       	<tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblDowntimeRate" runat="server" text="Downtime Rate (minutes)"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbLineDownRate" size="20" maxlength="20" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblSetLineStatus" runat="server" text="Status"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlLineStatus" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                            <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblLineUpdatedBy" Text="Updated By:" runat="server"/>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData">
                                <asp:Label ID="lblLineLastUpdate" Text="" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader"><asp:Label ID="lblLineUpdatedDate" Text="Last Update Date" runat="server"/></td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblLineLastUpdateDate" Text="" runat="server"/></td>
                        </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="editArea">
                      <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD class="editArea" ALIGN=right>
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td>
                                             <asp:Button ID="lbLineCancel2" CSSclass="buttonStd" runat="server" text="Cancel" 
                                             onclick="lbLineSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>  
                                        <td>
                                            <asp:Button ID="lbLineSave2" CSSclass="buttonEmphasis" runat="server" text="Save"
                                             onclick="lbLineSave_Click" OnClientClick="return confirmChange('Plant Line');" CommandArgument="edit"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </TD>
                        </TR> 
                    </table>
                </td>
              </tr>