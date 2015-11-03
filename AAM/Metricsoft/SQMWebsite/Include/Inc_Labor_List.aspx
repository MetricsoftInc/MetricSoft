                    <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbLaborAdd1" CSSclass="buttonStd" runat="server" text="Add Labor Type"
                                                        onclick="lnkLaborAdd_Click" CommandArgument="add"></asp:Button>
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
                            <div id="divLaborGVScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvLaborList" Name="gvLaborList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                    <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                                    <asp:BoundField  DataField="LABOR_TYP_ID" Visible="False"/>
                                    <asp:TemplateField HeaderText="Labor Code" ItemStyle-Width="30%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("LABOR_TYP_ID") %>'
										    	Text='<%#Eval("LABOR_CODE") %>' OnClick="lnkLaborView_Click"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="LABOR_NAME" HeaderText="<%$ Resources:LocalizedText, Name %>" ItemStyle-Width="40%" />
                                    <asp:BoundField DataField="LABOR_RATE" HeaderText="<%$ Resources:LocalizedText, Cost %>" ItemStyle-Width="15%" />
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Status %>" ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblLaborListEmpty" Text="There are currently no Labor Types defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                            </td>
                        </tr>

                       <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td class="admBkgd" align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbLaborAdd2" CSSclass="buttonStd" runat="server" text="Add Labor Type"
                                                        onclick="lnkLaborAdd_Click" CommandArgument="add"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                     </tr>