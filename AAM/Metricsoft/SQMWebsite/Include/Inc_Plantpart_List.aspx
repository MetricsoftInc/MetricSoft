                <tr><td class="admBkgd">&nbsp;</td></tr>
		        <tr>
                   <td>
                     <table width="100%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>
                            <div id="divPlantPartGVScroll" runat="server" class="">
                              <asp:GridView runat="server" ID="gvPlantPartList" Name="gvPlantPartList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="98%" OnRowDataBound="gvPlantPart_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Part Number" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkViewPart_out" runat="server" CommandArgument='<%#Eval("PART_ID") %>'
										    text='<%#Eval("PART_NUM") %>' OnClick="lnkPartView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Company" ItemStyle-Width="18%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustCompanyName_out" runat="server" text='<%#Eval("CUST_COMPANY_NAME") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Location %>" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustCompanyLocation_out" runat="server" text='<%#Eval("CUST_DUNS_CODE") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Plant" ItemStyle-Width="18%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustPlantName_out" runat="server" text='<%#Eval("CUST_PLANT_NAME") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                      		        <asp:TemplateField HeaderText="Supplier Company" ItemStyle-Width="18%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSuppCompanyName_out" runat="server" text='<%#Eval("SUPP_COMPANY_NAME") %>'  ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Plant" ItemStyle-Width="18%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSuppPlantName_out" runat="server" text='<%#Eval("SUPP_PLANT_NAME") %>' ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Location %>" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSuppPlantLocation_out" runat="server" text='<%#Eval("SUPP_PLANT_DUNS_CODE") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Active %>" ItemStyle-Width="8%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:CheckBox ID="cbStatus" runat="server" DataTextField="STATUS" Style="margin-left: 33%;" OnCheckedChanged="wasChanged"></asp:CheckBox>
                                        </ItemTemplate>
                                     </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblPlantPartListEmpty" Height="40" Text="There are currently no Part / Plant associations defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>