<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSReport.ascx.cs" Inherits="SQM.Website.Ucl_EHSReport"  %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel id="pnlCO2Report" runat="server" class="" visible="false">
    <asp:Repeater runat="server" ID="rptCO2Report" ClientIDMode="AutoID" OnItemDataBound="rptCO2Report_OnItemDataBound">
        <HeaderTemplate>
			<table cellspacing="0" cellpadding="1" border="0" width="100%">
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td id="tdLocation" runat="server" class="rptInputTableEnd" style="width: 15%; vertical-align: top; padding-top: 2px; padding-left: 3px;">
			        <asp:Label ID="lblLocation" runat="server" CSSClass="prompt" Text="Location"></asp:Label>
				</td>
                <td class="rptInputTable" valign="top" style="width: 85%;" >
                    <table cellspacing="0" cellpadding="1" border="0" width="100%">
                        <tr>
                            <td>
                                <asp:Label id="lblScope1" runat="server" CssClass="sectionTitlesSmall" Text="Scope 1 Emissions:"></asp:Label>
                                &nbsp;
                                <asp:Label id="lblScope1Instruct" runat="server" CssClass="instructText" Text="Direct emissions from fuels burned on-site or by vehicles owned or operated by this location."></asp:Label>
                                <br /><br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Repeater runat="server" ID="rptScope1Fuel" ClientIDMode="AutoID" OnItemDataBound="rptScope1Fuel_OnItemDataBound">
                                    <HeaderTemplate><table cellspacing="0" cellpadding="1" border="0" width="100%">
                                    </HeaderTemplate>
                                    <ItemTemplate>
						                <tr>
                                            <td class="listDataAlt" valign="top" style="width: 18%;">
                                                <asp:Label runat="server" ID="lblScope1FuelHdr" CssClass="prompt" Text="Fuel"></asp:Label>
                                                <br />
                                                <asp:Label id="lblScope1Fuel" runat="server" CssClass="textStd"></asp:Label>
                                            </td>
                                            <td class="listDataAlt" valign="top" style="width: 18%;">
                                                <asp:Label runat="server" ID="lblScope1FuelQtyHdr" CssClass="prompt" Text="Quantity "></asp:Label>
                                                <br />
                                                <asp:Label id="lblScope1FuelQty" runat="server" CssClass="textStd"></asp:Label>
                                            </td>
                                            <td class="listDataAlt" valign="top" rowspan="1" style="width: 64%;">
                                                <asp:GridView runat="server" ID="gvGasList1" Name="gvGasList1" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="none" 
                                                    PageSize="20" AllowSorting="false" Width="99%" OnRowDataBound="gvGasList1_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellTextLeft" />    
                                                    <RowStyle CssClass="DataCell" />
                                                    <AlternatingRowStyle CssClass="DataCellAlt" /> 
                	                                <Columns>
                                                        <asp:TemplateField HeaderText="Gas" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblGas" runat="server" Text='<%#Eval("GasCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="GWP Factor<sup> 1</sup>" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblGWPFactor" runat="server" Text='<%#Eval("GWPFactor") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="GHG Factor<sup> 2</sup>" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCO2Factor" runat="server" Text='<%#Eval("GHGFactor") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Emissions " ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblEmitValue" runat="server" Text='<%#Eval("GHGValue") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                        <tr>
                            <td vertical-align: top; padding-top: 20px; padding-left: 3px;">
                                <asp:Label id="lblScope2" runat="server" CssClass="sectionTitlesSmall" Text="Scope 2 Emissions:"></asp:Label>
                                &nbsp;
                                <asp:Label id="lblScope2Instruct" runat="server" CssClass="instructText" Text="Indirect emissions resulting from the generation of power produced off-site but purchased or consumed by this location."></asp:Label>
                                <br /><br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Repeater runat="server" ID="rptScope2Fuel" ClientIDMode="AutoID" OnItemDataBound="rptScope2Fuel_OnItemDataBound">
                                    <HeaderTemplate><table cellspacing="0" cellpadding="1" border="0" width="100%">
                                    </HeaderTemplate>
                                    <ItemTemplate>
						                <tr>
                                            <td class="listDataAlt" valign="top" style="width: 18%;">
                                                <asp:Label runat="server" ID="lblScope2FuelHdr" CssClass="prompt" Text="Fuel"></asp:Label>
                                                <br />
                                                <asp:Label id="lblScope2Fuel" runat="server" CssClass="textStd"></asp:Label>
                                            </td>
                                            <td class="listDataAlt" valign="top" style="width: 18%;">
                                                <asp:Label runat="server" ID="lblScope2FuelQtyHdr" CssClass="prompt" Text="Quantity "></asp:Label>
                                                <br />
                                                <asp:Label id="lblScope2FuelQty" runat="server" CssClass="textStd"></asp:Label>
                                            </td>
                                            <td class="listDataAlt" valign="top" rowspan="1" style="width: 64%;">
                                                <asp:GridView runat="server" ID="gvGasList2" Name="gvGasList2" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="none" 
                                                    PageSize="20" AllowSorting="false" Width="99%" OnRowDataBound="gvGasList2_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellTextLeft" />    
                                                    <RowStyle CssClass="DataCell" />
                                                    <AlternatingRowStyle CssClass="DataCellAlt" /> 
                	                                <Columns>
                                                        <asp:TemplateField HeaderText="Gas" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblGas" runat="server" Text='<%#Eval("GasCode") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="GWP Factor<sup> 1</sup>" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblGWPFactor" runat="server" Text='<%#Eval("GWPFactor") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="GHG Factor<sup> 2</sup>" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCO2Factor" runat="server" Text='<%#Eval("GHGFactor") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Emissions " ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblEmitValue" runat="server" Text='<%#Eval("GHGValue") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="height: 5px;">
                <td colspan="5"></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:Repeater>
    <asp:Label ID="lblGWPLegend" runat="server" CssClass="refTextSmall" Text="<sup>1</sup> 100 year Global Warming potential (KG/KWH)"></asp:Label>
    <br />
    <asp:Label ID="lblGHGLegend" runat="server" CssClass="refTextSmall" Text="<sup>2</sup> Regional energy source emissions factor (KG/KWH)"></asp:Label>
	<p class="refTextSmall">  Download <a class="refTextSmall" href="/Documents/GHGReferences.xlsx">GHG References And Factors</a> used to produce this report</p>
</asp:Panel>
