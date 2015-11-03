
                    <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="2">
			                    <tr>
                                    <td>
                                        <asp:Label ID="lblListType"  runat="server"  class="prompt" style="margin-left: 10px;" Text="List Type: "></asp:Label>
                                        <asp:DropDownList ID="ddlNotifyFor" runat="server" AutoPostBack=true  OnSelectedIndexChanged="SelectNotifyFor_Change"></asp:DropDownList>
                                        <asp:Label ID="lblListDesc_out"  runat="server" class="textSmall" style="margin-left: 5px;" Text=""></asp:Label>
                                    </td>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbMemberAdd1" CSSclass="buttonStd" runat="server" text="Add Member"
                                                    OnClientClick="ValidateNotifyList();" onclick="lnkMemberAdd_Click" CommandArgument="add" UseSubmitBehavior="false"></asp:Button>
                                                </td>
                                                 <td>
                                                    <asp:Button ID="lbNotifySave1" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" OnClientClick="return confirmChange('Notification List');" onclick="lbSaveNotifyList_Click"></asp:Button>
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
                            <!-- results grid -->
                            <div id="divNotifyGVScroll" runat="server" class="">
                            <asp:GridView runat="server" ID="gvNotifyList" Name="gvNotifyList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="98%" OnRowDataBound="gvNotifyList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="COMPANY_ID" Visible="False"/>
                    	            <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                    <asp:BoundField  DataField="NOTIFY_FOR" Visible="False"/>
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, EmailAddress %>" ItemStyle-Width="40%">
                                        <ItemTemplate>
                                            <asp:Label ID="lbEmail" runat="server" CSSclass="textSmall" Text='<%#Eval("EMAIL") %>'></asp:Label>
                                            <asp:TextBox ID="tbEmail" runat="server" CSSclass="textStd" Text='<%#Eval("EMAIL") %>' Columns="56" Visible="false" ></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="First Name" ItemStyle-Width="25%">
                                        <ItemTemplate>
                                            <asp:Label ID="lbFirstName" runat="server" CSSclass="textSmall" Text='<%#Eval("FIRST_NAME") %>'></asp:Label>
                                            <asp:TextBox ID="tbFirstName" runat="server" CSSclass="textStd" Text='<%#Eval("FIRST_NAME") %>' Columns="36" Visible="false" ></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Last Name" ItemStyle-Width="25%">
                                        <ItemTemplate>
                                            <asp:Label ID="lbLastName" runat="server" CSSclass="textSmall" Text='<%#Eval("LAST_NAME") %>'></asp:Label>
                                            <asp:TextBox ID="tbLastName" runat="server" CSSclass="textStd" Text='<%#Eval("LAST_NAME") %>' Columns="36" Visible="false" ></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Delete %>" ItemStyle-Width="10%">
                                    <ItemTemplate>
                                      <asp:HiddenField ID="hfStatus" runat="server" />
                                      <asp:CheckBox ID="cbStatus" runat="server" Style="margin-left: 33%;"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Label runat="server" ID="lblNotifyListEmpty" Text="There are currently no notification members defined." class="GridEmpty" Visible="false"></asp:Label>
                            </div>
                            </td>
                        </tr>

                       <tr>
                        <td class="editArea" colspan="10">
				            <table width="98%" border="0" cellspacing="0" cellpadding="2">
			                    <tr>
			  	                    <td align=right>
					                    <table cellpadding=0 cellspacing=2 border=0>
						                    <tr>
							                    <td>
                                                    <asp:Button ID="lbMemberAdd2" CSSclass="buttonStd" runat="server" text="Add Member"
                                                        onclick="lnkMemberAdd_Click" CommandArgument="add" UseSubmitBehavior="false"></asp:Button>
                                                </td>
                                                <td>
                                                    <asp:Button ID="lbNotifySave2" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" OnClientClick="return confirmChange('Notification List');" onclick="lbSaveNotifyList_Click"></asp:Button>
                                                </td>
						                    </tr>
					                    </table>
				                    </td>
			                    </tr>
			                </table>
                        </td>
                     </tr>