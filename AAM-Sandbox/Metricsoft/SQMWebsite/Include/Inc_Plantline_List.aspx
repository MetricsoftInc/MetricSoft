                    <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbLineAdd1" CSSclass="buttonStd" runat="server" text="Add Plant Line" 
                                                        onclick="lnkLineAdd_Click" CommandArgument="add"></asp:Button>
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
                            <div id="divLineGVScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvLineList" Name="gvLineList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                                    <asp:BoundField  DataField="PLANT_LINE_ID" Visible="False"/>
                                    <asp:TemplateField HeaderText="Line Name" ItemStyle-Width="60%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("PLANT_LINE_ID") %>'
										    	Text='<%#Eval("PLANT_LINE_NAME") %>' OnClick="lnkLineView_Click"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="DOWNTIME_RATE" HeaderText="Downtime Rate (minutes)" ItemStyle-Width="25%" />
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblLineListEmpty" Text="There are currently no Plant Lines defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                            </td>
                        </tr>

                       <tr>
                        <td class="editArea"colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="0">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbLineAdd2" CSSclass="buttonStd" runat="server" text="Add Plant Line" 
                                                        onclick="lnkLineAdd_Click" CommandArgument="add"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                     </tr>