		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>
                            <div id="divPartGVScroll" runat="server" class="">
                              <asp:GridView runat="server" ID="gvPartList" Name="gvPartList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Part Number" ItemStyle-Width="20%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkViewPart_out" runat="server" CommandArgument='<%#Eval("PART_ID") %>'
										    	text='<%#Eval("PART_NUM") %>' OnClick="lnkPartView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Part Name" ItemStyle-Width="30%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkViewPartName_out" runat="server" CommandArgument='<%#Eval("PART_ID") %>'
										    	text='<%#Eval("PART_NAME") %>' OnClick="lnkPartView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="PART_SUFFIX" HeaderText="Suffix" ItemStyle-Width="10%" />
                                    <asp:BoundField DataField="SOURCE_SYSTEM" HeaderText="Source System" ItemStyle-Width="30%" />
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                     </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </div>
                            <asp:Label runat="server" ID="lblPartListEmpty" Height="40" Text="There are currently no Parts defined." class="GridEmpty" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>