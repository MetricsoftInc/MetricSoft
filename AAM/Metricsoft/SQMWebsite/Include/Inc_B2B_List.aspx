
		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>
                              <div class="scrollArea">
                              <asp:GridView runat="server" ID="gvB2BList" Name="gvB2BList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="98%" OnRowDataBound="gvList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    	<asp:TemplateField HeaderText="Company" ItemStyle-Width="24%">
						                    <ItemTemplate>
							                    <asp:Label ID="lblCompany_out" runat="server" text='<%#Eval("CompanyName") %>' ></asp:Label>
                                        	</ItemTemplate>
					                    </asp:TemplateField>
                                    	<asp:TemplateField HeaderText="Business Organization" ItemStyle-Width="24%">
						                    <ItemTemplate>
							                    <asp:Label ID="lblBusorg_out" runat="server" text='<%#Eval("BusorgName") %>' ></asp:Label>
                                        	</ItemTemplate>
					                    </asp:TemplateField>
                                    	<asp:TemplateField HeaderText="Plant" ItemStyle-Width="24%">
						                    <ItemTemplate>
							                    <asp:Label ID="lblPlant_out" runat="server" text='<%#Eval("PlantName") %>' ></asp:Label>
                                        	</ItemTemplate>
					                    </asp:TemplateField>
                                    	<asp:TemplateField HeaderText="Location" ItemStyle-Width="20%">
						                    <ItemTemplate>
							                    <asp:Label ID="lblPlantlocation_out" runat="server" text='<%#Eval("PlantLocation") %>' ></asp:Label>
                                        	</ItemTemplate>
					                    </asp:TemplateField>
                                    	<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Active %>" ItemStyle-Width="8%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfSelect_out" runat="server" Value='<%#Eval("IsSelected") %>' />
                                            <asp:CheckBox ID="cbSelect_out" runat="server" DataTextField="IsSelected" Style="margin-left: 33%;"></asp:CheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblB2BListEmpty" Height="40" Text="No Plants matching your search criteria." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>