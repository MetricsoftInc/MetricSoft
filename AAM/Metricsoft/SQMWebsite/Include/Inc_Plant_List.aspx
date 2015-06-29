                    <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
                                                 <td>
                                                    <asp:Button ID="lbAssociatePlants1" CSSclass="buttonStd" runat="server" text="Add Plant" 
                                                        onclick="lnkPlantView_Click" CommandArgument=""></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                    </tr>

                     <tr>
                        <td valign="top" align="center" class="admBkgd">
                            <!-- results grid -->
                            <div id="divPlantGVScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvPlantList" Name="gvPlantList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvPlantList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                    <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                                    <asp:TemplateField HeaderText="Business Organization" ItemStyle-Width="30%">
							            <ItemTemplate>
								            <asp:Label ID="lblBusorg_out" runat="server" Text='<%#Eval("BUS_ORG_ID") %>'  ></asp:Label>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Plant Name" ItemStyle-Width="40%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("PLANT_ID") %>'
										    	Text='<%#Eval("PLANT_NAME") %>'  OnClick="lnkPlantView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="DUNS_CODE" HeaderText="Location Code" ItemStyle-Width="20%" />
                                    <asp:BoundField DataField="GRID_CODE" HeaderText="Grid Code" ItemStyle-Width="20%" />
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                             <asp:Label runat="server" ID="lblPlantListEmpty" Text="There are currently no Plants defined." class="GridEmpty" Visible="false"></asp:Label>
                             </div>
                            </td>
                        </tr>
