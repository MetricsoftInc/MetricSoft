                    <tr>
                        <td class="editArea">
                           <table width="100%" border="0" cellspacing="0" cellpadding="2">
			                    <tr>
			  	                    <td align=center>
                                        <table width="98%" border="0" cellspacing="1" cellpadding="2">
			                                <tr>
			  	                                <td align=right>
					                                <table cellpadding=0 cellspacing=2 border=0>
						                                <tr>
                                                            <td>
                                                                <asp:Button ID="lbSave1" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" OnClientClick="return confirmChange('List');" onclick="lbSaveNonconf_Click"></asp:Button>
                                                            </td>
						                                </tr>
					                                </table>
				                                </td>
			                                </tr>
			                            </table>

                              <asp:GridView runat="server" ID="gvNonconfList" Name="gvNonconfList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Vertical" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvNonconf_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                                    <asp:TemplateField HeaderText="Problem Type" ItemStyle-Width="30%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfProblemType" runat="server" Value='<%#Eval("ProblemType") %>' />
                                            <asp:Label ID="lblProblemType" runat="server" DataTextField="ProblemType" Style="font-weight:bold;"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Primary Non-Conformance" ItemStyle-Width="30%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfPrimaryNC" runat="server" Value='<%#Eval("PrimaryNC") %>' />
                                            <asp:Label ID="lblPrimaryNC" runat="server" DataTextField="PrimaryNC" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="SecondaryNC" HeaderText="Secondary Non-Conformance" ItemStyle-Width="30%" />
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Active %>" ItemStyle-Width="10%">
                                    <ItemTemplate>
                                      <asp:HiddenField ID="hfStatus" runat="server" Value='<%#Eval("SecondaryNCStatus") %>' />
                                      <asp:CheckBox ID="cbStatus" runat="server" DataTextField="SecondaryNCStatus" Style="margin-left: 33%;" OnCheckedChanged="wasChanged"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <table width="100%" border="0" cellspacing="1" cellpadding="2">
			                    <tr>
			  	                    <td class="editArea" align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
                                                <td>
                                                    <asp:Button ID="lbSave2" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" OnClientClick="return confirmChange('List');" onclick="lbSaveNonconf_Click"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                    </tr>
                </table>
            </td>
          </tr>