
		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>	
                              <div id="divPartPlantGVScroll" runat="server" class="">
                              <asp:GridView runat="server" ID="gvPartPlantList" Name="gvPartPlantList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvPartPlant_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Business Org" ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustBusOrg" runat="server" text='<%#Eval("CUST_BUS_ORG_NAME") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="15%" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustPlantID" runat="server" text='<%#Eval("CUSTOMER_PLANT_ID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Plant" ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustPlant" runat="server" text='<%#Eval("CUST_PLANT_NAME") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
            			            <asp:TemplateField HeaderText="Supplier" ItemStyle-Width="40%">
                                        <ItemTemplate>
                                            <asp:GridView runat="server" ID="gvPartSuppGrid" Name="gvPartSuppGrid" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%">
                                                <HeaderStyle CssClass="HeadingCellText" />    
                                                <RowStyle CssClass="DataCell" />
                                                <Columns>
                                                    <asp:BoundField DataField="SUPP_COMPANY_NAME" HeaderText="Company" ItemStyle-Width="50%" />
                                                    <asp:BoundField DataField="SUPP_PLANT_NAME" HeaderText="Supplier Plant" ItemStyle-Width="50%" />
                                                </Columns>
                                            </asp:GridView>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblPartPlantListEmpty" Height="40" Text="There are currently no Part / Plant associations defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>