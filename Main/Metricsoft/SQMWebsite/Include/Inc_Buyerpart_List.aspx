
		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>	
                             <div id="divBuyerPartGVScroll" runat="server" class="">
                              <asp:GridView runat="server" ID="gvBuyerPartList" Name="gvBuyerPartList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvBuyerPart_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:BoundField DataField="PART_NUM" HeaderText="Part Number" ItemStyle-Width="20%" />
					                <asp:BoundField DataField="COMPANY_NAME" HeaderText="Supplier Company" ItemStyle-Width="20%" />
   					                <asp:BoundField DataField="PLANT_NAME" HeaderText="Supplier Plant" ItemStyle-Width="20%" />
   				                    <asp:BoundField DataField="BUYER_CODE" HeaderText="Buyer Code" ItemStyle-Width="10%" />
                                    <asp:TemplateField HeaderText="Active" ItemStyle-Width="8%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:CheckBox ID="cbStatus_out" runat="server" DataTextField="STATUS" Style="margin-left: 33%;" OnCheckedChanged="wasChanged"></asp:CheckBox>
                                        </ItemTemplate>
                                     </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblBuyerPartListEmpty" Height="40" Text="There are currently no Part / Buyer associations defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>