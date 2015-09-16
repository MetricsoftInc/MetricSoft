                    <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                   <asp:Button ID="lbDeptAdd1" CSSclass="buttonStd" runat="server" text="Add a Department" 
                                                        onclick="lnkDeptAdd_Click" CommandArgument="add"></asp:Button>
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
                            <div id="divDeptGVScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvDeptList" Name="gvDeptList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                    <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                                  <asp:BoundField  DataField="DEPT_ID" Visible="False"/>
                                    <asp:TemplateField HeaderText="Department Name" ItemStyle-Width="45%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("DEPT_ID") %>'
										    	Text='<%#Eval("DEPT_NAME") %>' OnClick="lnkDeptView_Click"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="DEPT_CODE" HeaderText="Department Code" ItemStyle-Width="35%" />
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblDeptListEmpty" Text="There are currently no Departments defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                          </td>
                       </tr>

                       <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbDeptAdd2" CSSclass="buttonStd" runat="server" text="Add a Department" 
                                                        onclick="lnkDeptAdd_Click" CommandArgument="add"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                     </tr>