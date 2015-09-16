		<table width="100%" border="0" cellspacing="0" cellpadding="0" class="darkBorder">
                <tr>
                    <td>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
                            <tr>
                                <td align=left  style="padding-left:30px; margin-top:4px;">
                                    <asp:Label ID = "lblLanguageInfo" runat="server" class="prompt" Text="Language:"></asp:Label>
                                    &nbsp;&nbsp;
                                    <asp:TextBox ID = "tbLanguageInfo" runat="server" Enabled="false" CssClass="displayOnly"></asp:TextBox>
                                </td>
                                <td align=right  style="padding-right:40px; margin-top:4px;">
                                    <asp:Button ID="btnSavePageLabels" runat="server" class="buttonEmphasis" Text="Save Page Text" OnClientClick="return confirmChange('Page Text');" onclick="btnSavePageLabels_Click"></asp:Button>
                                </td>
                            </tr>
                         </table>
                    </td>
                </tr>
                     <tr>
                        <td valign="top" align="center" class="admBkgd">
                            <!-- results grid -->
                            <asp:GridView runat="server" ID="gvPageLabelList" Name="gvPageLabelList" CssClass="GridSmall" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="97%" ShowHeader="True" style="margin-top:4px;" OnRowDataBound="gvPageLabelList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellTextSmall" />    
                                <RowStyle CssClass="DataCellSmall" />
                	            <Columns>
                                <asp:TemplateField HeaderText="Page ID" Visible = "false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPageID_out" runat="server" CssClass="DataCellSmall" Text='<%#Eval("PAGE_ID") %>'></asp:Label>
                                        <asp:HiddenField ID="hfPageID" runat="server" Value='<%#Eval("PAGE_ID") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Control Name" ItemStyle-Width="18%" Visible = "true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLabelName" runat="server" CssClass="DataCellSmall" Text='<%#Eval("LABEL_NAME") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
   				                <asp:BoundField DataField="LABEL_TYPE" HeaderText="Type" ItemStyle-Width="10%" />
                                <asp:TemplateField HeaderText="Default Text" ItemStyle-Width="25%" Visible = "false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBaseLabelText" runat="server" CssClass="DataCellSmall" ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Default Text" ItemStyle-Width="70%" Visible = "true">
                                    <ItemTemplate>
                                        <asp:TextBox ID="tbLabelText" runat="server" CssClass="DataCellSmall" Text='<%#Eval("TEXT") %>' Width="99%"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </td>
                        </tr>
		</table>