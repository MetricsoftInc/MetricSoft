                 <tr>
                    <td class="editArea">
                         <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD ALIGN=right>
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                         <td>
                                            <asp:Button ID="lbPlantCancel1" CSSclass="buttonStd" runat="server" text="Cancel" 
                                             onclick="lbPlantSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>  
                                        <td>
                                            <asp:Button ID="lbSavePlant1" CSSclass="buttonEmphasis" runat="server" text="Save" 
                                             OnClientClick="return confirmChange('Plant');" onclick="lbPlantSave_Click" CommandArgument="edit"></asp:Button>
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
                    <table width="98%" align="center" border="0" cellspacing="1" cellpadding="2" class="darkBorder">
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantBusOrg" runat="server" text="Business Organization"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlParentBusOrg" runat="server"></asp:DropDownList></td>
                        </TR>
                        <tr>
                            <td class="columnHeader" width="39%">
                                <asp:Label ID="lblPlantNameEdit" runat="server" text="Plant Name"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td CLASS="tableDataAlt" width="60%"><asp:TextBox ID="tbPlantName" size="50" maxlength="255" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantStatus" runat="server" text="Status"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantStatus" runat="server"></asp:DropDownList></td>
                        </TR>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantOrgLocCode" runat="server" text="Organization Location Code"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbOrgLocCode" size="30" enabled="false" maxlength="20" runat="server"/></td>
                        </tr>
			            <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantLocCode" runat="server" text="Location Code"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbPlantLocCode" size="30" maxlength="20" runat="server"/></td>
			            </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantCurrencyCode" runat="server" text="Default Currency"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantCurrencyCodes" runat="server"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantCRTreshold" runat="server" text="Cost Recovery Threshold"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbPlantCRThreshold" size="20" maxlength="20" runat="server"/></td>      
			            </tr>
                       <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantTimezone" runat="server" text="Local Time Zone"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantTimezone" runat="server"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantUpdatedBy" runat="server" text="Updated By"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblPlantLastUpdate" Text="" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPlantUpdateDate" runat="server" text="Last Update Date"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblPlantLastUpdateDate" Text="" runat="server"/></td>
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
                                            <asp:Button ID="lbPlantCancel2" CSSclass="buttonStd" runat="server" text="Cancel" 
                                             onclick="lbPlantSave_Click" CommandArgument="cancel"></asp:Button>
                                        </TD>  
                                        <td>
                                            <asp:Button ID="lbSavePlant2" CSSclass="buttonEmphasis" runat="server" text="Save" 
                                             OnClientClick="return confirmChange('Plant');" onclick="lbPlantSave_Click" CommandArgument="edit"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </TD>
                        </TR> 
                    </table>
                </td>
               </tr>