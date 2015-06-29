
		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>	
                            <div id="divPartCustGVScroll" runat="server" class="">
                              <asp:GridView runat="server" ID="gvPartCustList" Name="gvPartCustList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvPartCust_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Business Org" ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustBusOrg" runat="server" text='<%#Eval("SUPP_BUS_ORG_NAME") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="15%" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSuppPlantID" runat="server" text='<%#Eval("SUPPLIER_PLANT_ID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Plant" ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSuppPlant" runat="server" text='<%#Eval("SUPP_PLANT_NAME") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
            			            <asp:TemplateField HeaderText="Customer" ItemStyle-Width="40%">
                                        <ItemTemplate>
                                            <asp:GridView runat="server" ID="gvPartCustGrid" Name="gvPartCustGrid" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%">
                                                <HeaderStyle CssClass="HeadingCellText" />    
                                                <RowStyle CssClass="DataCell" />
                                                <Columns>
                                                    <asp:BoundField DataField="CUST_COMPANY_NAME" HeaderText="Company" ItemStyle-Width="50%" />
                                                    <asp:BoundField DataField="CUST_PLANT_NAME" HeaderText="Customer Plant" ItemStyle-Width="50%" />
                                                </Columns>
                                            </asp:GridView>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblPartCustListEmpty" Height="40" Text="There are currently no Part / Customer associations defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>