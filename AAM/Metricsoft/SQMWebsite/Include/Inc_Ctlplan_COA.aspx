                     <table width="98%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>	
                                <asp:GridView runat="server" ID="gvCOAResult" Name="gvCOAResult" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvCOAResult_OnRowDataBound">
                                    <HeaderStyle CssClass="HeadingCellText" />    
                                    <RowStyle CssClass="DataCell" />
                	                <Columns>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPlanStepID" runat="server" text='<%#Eval("CTLPLANSTEP_ID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Step" ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPlanStepNum_out" runat="server" CssClass="DataCellSmall" style="font-weight: bold;" text='<%#Eval("STEP_NUM") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
           			                    <asp:TemplateField HeaderText="Measures Accountability" ItemStyle-Width="95%">
                                            <ItemTemplate>
                                                <asp:GridView runat="server" ID="gvMeasureGrid" Name="gvMeasureGrid" CssClass="GridSmall" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvMeasure_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellText" />    
                                                    <RowStyle CssClass="DataCell" Height=26 />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="No." ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblMeasureNum" runat="server" CssClass="DataCellSmall" style="font-weight: bold;" text='<%#Eval("MEASURE_NUM") %>'></asp:Label>
                                                                <asp:HiddenField id="hfStepID" runat="server" value='<%#Eval("CTLPLANSTEP_ID") %>'></asp:HiddenField>
                                                                <asp:HiddenField id="hfMeasureID" runat="server" value='<%#Eval("CTLMEASURE_ID") %>'></asp:HiddenField>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Characteristic"  ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblMeasureName" runat="server" CssClass="DataCellSmall"  text='<%#Eval("MEASURE_NAME") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Loc"  ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLocator" runat="server" CssClass="DataCellSmall"  text='<%#Eval("LOCATION_REF") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Class" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblMeasureClass" runat="server" text='<%#Eval("MEASURE_CLASS") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Requirements" ItemStyle-Width="12%">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfSpecLSL" runat="server" Value='<%#Eval("LSL") %>'/>
                                                                <asp:HiddenField ID="hfSpecUSL" runat="server" Value='<%#Eval("USL") %>'/>
                                                                <asp:HiddenField ID="hfUOM" runat="server" Value='<%#Eval("UOM") %>'/>
                                                                <asp:Label ID="lblSpecValues" runat="server" CssClass="DataCellSmall"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Results" ItemStyle-Width="18%">
                                                            <ItemTemplate>
                                                                <asp:GridView runat="server" ID="gvMetricGrid" Name="gvMetricGrid" CssClass="GridSmall" ShowHeader="False" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Horizontal" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvMetric_OnRowDataBound">
                                                                    <HeaderStyle CssClass="HeadingCellText" />    
                                                                    <RowStyle CssClass="DataCell" />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Metrics" ItemStyle-Width="50%">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblMetricName" runat="server" CssClass="DataCellSmall" text='<%#Eval("Name") %>' ></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Metrics" ItemStyle-Width="50%">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblMetricValue" runat="server" CssClass="DataCellSmall" text='<%#Eval("Value") %>' ></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Gauge Sys" ItemStyle-Width="12%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblGaugeSys" runat="server"  text='<%#Eval("GAUGE_TYPE") %>' CssClass="DataCellSmall"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Tooling"  ItemStyle-Width="12%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="tbTooling" runat="server"  style="width:97%;" CssClass="DataCellSmall"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="NCM No." ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblNCIssue" runat="server" CssClass="DataCellSmallEmphasis" ></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Process Capability" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Image ID="imgCPK" Visible="true" runat="server"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>