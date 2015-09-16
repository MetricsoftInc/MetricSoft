           <tr><td class="admBkgd">&nbsp;</td></tr>
		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>	
                              <asp:GridView runat="server" ID="gvBuyerList" Name="gvBuyerList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="98%" OnRowDataBound="gvBuyerList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />    
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Buyer Code" ItemStyle-Width="20%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkViewBuyer_out" runat="server" CommandArgument='<%#Eval("PERSON_ID") %>'
										    	text='<%#Eval("BUYER_CODE") %>' OnClick="lnkBuyerView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="FIRST_NAME" HeaderText="First name" ItemStyle-Width="22%" />
                                    <asp:BoundField DataField="LAST_NAME" HeaderText="Last Name" ItemStyle-Width="23%" />
                                    <asp:BoundField DataField="NEW_LOCATION_CD" HeaderText="Location" ItemStyle-Width="25%" />
                                    <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblBuyerListEmpty" Height="40" Text="There are currently no Buyers defined." class="GridEmpty" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>