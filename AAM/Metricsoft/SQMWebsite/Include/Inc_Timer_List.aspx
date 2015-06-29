
                     <tr>
                         <td class="editArea">
                            <table width="98%" border="0" cellspacing="0" cellpadding="0">
                                <TR>
                                    <TD ALIGN=right>
                                        <table border="0" cellspacing="0" cellpadding="2">
                                            <tr>
                                                <td>
                                                    <asp:Button ID="lbEditSettings" CSSclass="buttonStd" runat="server" text="Edit Settings" 
                                                    onclick=" lbPlaceholderEvent_Click" CommandArgument="edit"></asp:Button>
                                                </td>
                                            </tr>
                                        </table>
                                    </TD>
                                </TR> 
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="center" class="editArea">
                            <!-- results grid -->
                            <asp:GridView runat="server" ID="gvTimerList" Name="gvTimerList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvTimer_OnRowDataBound" ShowHeader="False" >
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                <asp:TemplateField Visible = "false">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfColName" runat="server" Value='<%#Eval("ColName") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="40%" Visible = "true">
                                    <ItemTemplate>
                                        <asp:Label ID="lbDescription" runat="server" Text='<%#Eval("Description") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField ItemStyle-Width="60%" Visible = "true">
                                    <ItemTemplate>
                                        <asp:Label ID="lbValue" runat="server" Text='<%#Eval("Value") %>'  ></asp:Label>
                                        <asp:TextBox ID="tbValue" runat="server" Text='<%#Eval("Value") %>' Columns="5" Visible="false" ></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </td>
                        </tr>