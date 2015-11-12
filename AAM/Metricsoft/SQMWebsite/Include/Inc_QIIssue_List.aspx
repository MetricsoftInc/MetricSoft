	                                    <tr>
                                            <td align="right">
                                                <asp:Button id="btnIssueListCancel" runat="server" Text="<%$ Resources:LocalizedText, Close %>" CSSClass="buttonStd" OnClick="btnIssueListCancel_Click"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align=center>
                                                <div id="divGVScroll" runat="server" class="">
                                                        <asp:GridView runat="server" ID="gvIssueList" Name="gvIssueList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                                            <HeaderStyle CssClass="HeadingCellText" />
                                                            <RowStyle CssClass="DataCell" />
                	                                        <Columns>
                                                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Task %>" ItemStyle-Width="15%">
							                                        <ItemTemplate>
                                                                        <asp:Label ID="lblIssueTask_out" runat="server"></asp:Label>
                                                                    </ItemTemplate>
							                                    </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Issue %>" ItemStyle-Width="10%">
							                                        <ItemTemplate>
                                                                        <asp:HiddenField id="hfIssueID" runat="server" Value='<%#Eval("QIO_ID") %>'></asp:HiddenField>
                                                                        <asp:LinkButton ID="lnkViewIssue_out" runat="server" CommandArgument='<%#Eval("QIO_ID") %>'
										    	                            OnClick="lnkIssue_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                                                    </ItemTemplate>
							                                    </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Issue Date" ItemStyle-Width="10%">
							                                        <ItemTemplate>
								                                        <asp:LinkButton ID="lnkIssueDate_out" runat="server" text='<%#Eval("LAST_UPD_DT") %>' CommandArgument='<%#Eval("QIO_ID") %>'
                                                                         OnClick="lnkIssue_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                                                    </ItemTemplate>
							                                    </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, PartNumber %>" ItemStyle-Width="15%">
							                                        <ItemTemplate>
								                                        <asp:Label ID="lblPartNum_out" runat="server" ></asp:Label>
                                                                    </ItemTemplate>
							                                    </asp:TemplateField>
                                                                <asp:BoundField DataField="PROBLEM_TYPE" HeaderText="Problem Type" ItemStyle-Width="15%" />
                                                                <asp:TemplateField  HeaderText="<%$ Resources:LocalizedText, Disposition %>" ItemStyle-Width="15%">
                                                                    <ItemTemplate>
								                                        <asp:Label ID="lblDisposition_out" runat="server" text='<%#Eval("DISPOSITION") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                               </asp:TemplateField>
                                                                <asp:TemplateField  HeaderText="Responsibility" ItemStyle-Width="15%">
                                                                    <ItemTemplate>
								                                        <asp:Label ID="lblResponsible_out" runat="server" text='<%#Eval("RESPONSIBLE") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                               </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <asp:Label runat="server" ID="lblIssueListEmpty" Height="40" Text="No Quality Issues matching your search criteria." class="GridEmpty" Visible="false"></asp:Label>
                                                </div>
                                            </td>
                                        </tr>