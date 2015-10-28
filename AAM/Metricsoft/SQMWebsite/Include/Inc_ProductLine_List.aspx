
                    <head>
                        <style type="text/css">
                            .Grid
                            {
                                margin-bottom: 0px;
                            }
                        </style>
                    </head>
                    <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="1">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbProdLineAdd1" CSSclass="buttonStd" runat="server" text="Add Product Line"
                                                     onclick="lnkProdLineAdd_Click" CommandArgument="add" UseSubmitBehavior="false"></asp:Button>
                                                </td>
                                                 <td>
                                                    <asp:Button ID="lbProdLineSave1" CSSclass="buttonEmphasis" runat="server" text="<%$ Resources:LocalizedText, Save %>" OnClientClick="return confirmChange('Product Lines');" onclick="lbSaveProdLineList_Click"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                    </tr>

                     <tr>
                        <td valign="top" align="center" class="editArea">
                          <div id="divProdLineGVScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvProdlineList" Name="gvProdlineList"
                                CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"
                                CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%"
                                OnRowDataBound="gvProdlineList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellTextLeft" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="COMPANY_ID" Visible="False"/>
                    	            <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                    <asp:TemplateField HeaderText="Product Line Code" ItemStyle-Width="40%">
                                        <ItemTemplate>
                                            <asp:Label ID="lbProdlineCode" runat="server" CSSclass="textSmall" Text='<%#Eval("PRODUCT_LINE_CODE") %>'></asp:Label>
                                            <asp:TextBox ID="tbProdlineCode" runat="server" CSSclass="textStd" Text='<%#Bind("PRODUCT_LINE_CODE") %>' Columns="60" Visible="false" ></asp:TextBox><asp:RequiredFieldValidator runat="server" id="valProdLine" ControlToValidate="tbProdlineCode" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Description %>" ItemStyle-Width="50%">
                                        <ItemTemplate>
                                            <asp:Label ID="lbProdlineDesc" runat="server" CSSclass="textSmall" Text='<%#Eval("PRODUCT_LINE_DESC") %>'></asp:Label>
                                            <asp:TextBox ID="tbProdlineDesc" runat="server" CSSclass="textStd" Text='<%#Bind("PRODUCT_LINE_DESC") %>' Columns="78" Visible="false" ></asp:TextBox><asp:RequiredFieldValidator runat="server" id="valProdDesc" ControlToValidate="tbProdlineDesc" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" ItemStyle-Width="10%">
                                    	<ItemTemplate>
                                      	<asp:HiddenField ID="hfStatus" runat="server" />
                                      	<asp:CheckBox ID="cbStatus" runat="server" Style="margin-left: 33%;" OnClick="DisableEnableValidators(this);"></asp:CheckBox>
                                    	</ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblProdlineListEmpty" Text="There are currently no Product Lines defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                           </td>
                        </tr>

                       <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="1">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbProdLineAdd2" CSSclass="buttonStd" runat="server" text="Add Product Line"
                                                        onclick="lnkProdLineAdd_Click" CommandArgument="add" UseSubmitBehavior="false"></asp:Button>
                                                </td>
                                                <td>
                                                    <asp:Button ID="lbProdLineSave2" CSSclass="buttonEmphasis" runat="server" text="<%$ Resources:LocalizedText, Save %>"  OnClientClick="return Validate_ProductLineList();" onclick="lbSaveProdLineList_Click" CausesValidation="true"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                     </tr>