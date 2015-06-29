                     <tr>
                        <td class="editArea" colspan="10">&nbsp;</td>
                     </tr>
                     <tr>
                        <td valign="top" align="center" class="admBkgd">
                            <!-- results grid -->
                            <div class="scrollArea">
                            <asp:GridView runat="server" ID="gvCompanyList" Name="gvCompanyList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="98%" OnRowDataBound="gvList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="COMPANY_ID" Visible="False"/>
                                    <asp:BoundField DataField="COMPANY_NAME" HeaderText="Company Name" ItemStyle-Width="50%" />
                                    <asp:BoundField DataField="ULT_DUNS_CODE" HeaderText="DUNS Code" ItemStyle-Width="30%" />
                                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblCompanyListEmpty" Text="There are currently no Company Codes defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                           </td>
                        </tr>
                        <tr>
                            <td class="editArea" colspan="10">&nbsp;</td>
                        </tr>
 
