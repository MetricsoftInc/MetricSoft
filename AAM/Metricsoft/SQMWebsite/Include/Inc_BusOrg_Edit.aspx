                <tr>
                    <td class="editArea">
                     <table width="98%" border="0" cellspacing="0" cellpadding="0">
                        <TR>
                            <TD ALIGN=right>
                                <table border="0" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td>
                                            <asp:Button ID="lbCancelBusOrg1" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>"
                                             OnClientClick="return confirmAction('Cancel without saving');"  onclick="lbCancel_Click"></asp:Button>
                                        </TD>
                                        <td>
                                            <asp:Button ID="lbSaveBusOrg1" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>"
                                             OnClientClick="return confirmChange('Business Organization');"  onclick="lbSave_Click"></asp:Button>
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
                    <table width="98%" align="center" border="0" cellspacing="1" cellpadding="2" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="39%">
                                <asp:Label ID="lblBusorgName" runat="server" text="Organization Name"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td CLASS="tableDataAlt" width="60%">
                                <asp:TextBox ID="tbOrgName" size="50" maxlength="255" runat="server"/><asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator1" ControlToValidate="tbOrgName" runat="server" ErrorMessage="Please enter a Business Organization Name." Display="None"></asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblBusorgLocCode" runat="server" text="Organization Location code"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbOrgLocCode" size="30" maxlength="20" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                               <asp:Label ID="lblBusorgCaseThreshold" runat="server" text="Problem Case Threshold"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbThreshold" size="20" maxlength="20" runat="server"/>
                            <asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator4" ControlToValidate="tbThreshold" runat="server" ErrorMessage="Please enter a Threshold Amount." Display="None"></asp:RequiredFieldValidator><asp:CompareValidator
                                    ID="CompareValidator1" runat="server" ControlToValidate="tbThreshold" Operator="DataTypeCheck" ErrorMessage="Please enter a numerical Threshold Amount."
                                    Type="Double"></asp:CompareValidator></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblBusorgCurrency" runat="server" text="Default Currency"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt">
                                <asp:DropDownList ID="ddlCurrencyCodes" runat="server">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="ddlCurrencyCodes" runat="server"
                                 ErrorMessage="Please enter a Business Organization Preferred Currency"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblBusorgParentOrg" runat="server" text="Parent Business Organization"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlParentBusOrg" runat="server">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="ddlParentBusOrg" runat="server"
                                 ErrorMessage="Please enter a Business Organization Parent Organization."></asp:RequiredFieldValidator>
                         </td>
                        </TR>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblSetBusorgStatus" runat="server" text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList>
                            </td>
                        </TR>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblBusorgUpdatedBy" runat="server" text="Updated By"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblLastUpdate_out" Text="" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblBusorgUpdateDate" runat="server" text="Last Update Date"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblLastUpdateDate_out" Text="" runat="server"/></td>
                        </tr>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CSSclass="promptAlert" ShowMessageBox="True" />
                        </table>
                        </td>
                        </tr>


