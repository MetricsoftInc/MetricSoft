<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_MetricList.ascx.cs" Inherits="SQM.Website.Ucl_MetricList" %>


<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Panel ID="pnlInputsList" runat="server" Visible = "false" style="margin-top: 4px;">
    <asp:HiddenField  runat="server" ID="hfInputsListPlantID"/>
    <asp:HiddenField id="hfInputsListPeriodDate" runat="server"/>
    <span class="summaryHeader navSectionBar" style="text-align: center; width: 100%;">
        <asp:Label runat="server" ID="lblInputsListTitle" Text="Metric Inputs"></asp:Label>
    </span>
    <div id="divInputsListReviewArea" runat="server" visible="false">
        <asp:LinkButton ID="lnkChartClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px; padding-top: 3px;" OnClick="lnkCloseMetric" ToolTip="<%$ Resources:LocalizedText, Close %>">
            <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle;"/>
        </asp:LinkButton>
        <Ucl:RadGauge id="uclGauge" runat="server"/>
    </div>
	<br>
    <div id="divInputsGVScroll" runat="server" class="">
        <asp:GridView runat="server" ID="gvInputsList" Name="gvInputsList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvInputsList_OnRowDataBound">
            <HeaderStyle CssClass="HeadingCellTextLeft" />
            <RowStyle CssClass="DataCell" />
            <Columns>
                <asp:TemplateField HeaderText="Metric" ItemStyle-Width="25%">
					<ItemTemplate>
                        <asp:HiddenField  runat="server" ID="hfPRMRID" Value='<%# Eval("PRMR_ID") %>'/>
                        <asp:HiddenField runat="server" ID="hfStatus" Value='<%# Eval("STATUS") %>'/>
                        <span>
				            <asp:Label runat="server" ID="lblMetricName" CSSClass="textSmall"></asp:Label>
                            <br />
                            <asp:Label runat="server" ID="lblMetricCode" CSSClass="refTextSmall"></asp:Label>
                            <span style="float:right; margin-right: 3px;">
								<asp:LinkButton ID="lnkMetricCD" runat="server" ToolTip="View 12 month input history" CommandArgument='<%#Eval("PRMR_ID") %>' CSSClass="refTextSmall" OnClick="lnkSelectMetric">
                                    <img src="/images/defaulticon/16x16/statistics-chart.png" alt="" style="vertical-align: middle; border: 0px;" />
								</asp:LinkButton>
                            </span>
                        </span>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="" ItemStyle-Width="2%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblRequired" ></asp:Label>
                         <asp:Image ID="imgStatus" ImageUrl="" runat="server" visible="false" ToolTip="This imput marked for delete" style="vertical-align: middle;"/>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="Invoice Period" ItemStyle-Width="23%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblInvoiceDateFrom" CSSClass="textSmall" Text='<%# Eval("EFF_FROM_DT") %>'></asp:Label>
                        &nbsp;&nbsp;To&nbsp;&nbsp;
                        <asp:Label runat="server" ID="lblInvoiceDateTo" CSSClass="textSmall" Text='<%# Eval("EFF_TO_DT") %>'></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="12%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblValue" CSSClass="textSmall" Text='<%# Eval("MEASURE_VALUE") %>'></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="UOM" ItemStyle-Width="5%">
					<ItemTemplate>
				         <asp:Label runat="server" ID="lblValueUOM" CssClass="refTextSmall" Text='<%# Eval("UOM") %>'></asp:Label>
 		            </ItemTemplate>
		        </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Cost %>" ItemStyle-Width="12%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblCost" CSSClass="textSmall" Text='<%# Eval("MEASURE_COST") %>'></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="Credit" ItemStyle-Width="12%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblCredit" CSSClass="textSmall" ></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="" ItemStyle-Width="5%">
					<ItemTemplate>
				         <asp:Label runat="server" ID="lblCostCD" CssClass="refTextSmall" Text='<%# Eval("CURRENCY_CODE") %>'></asp:Label>
 		            </ItemTemplate>
		        </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
 </asp:Panel>

<asp:Panel ID="pnlHSTMetricsList" runat="server" Visible = "false" style="margin-top: 6px;">
     <span class="summaryHeader navSectionBar" style="text-align: center; width: 100%;">
        <asp:Label runat="server" ID="lblHSTMetricsListTitle" Text="Finalized Metrics"></asp:Label>
    </span>
	<br>
    <div id="divHSTMetricsGVScroll" runat="server" class="">
        <asp:GridView runat="server" ID="gvHSTMetricsList" Name="gvHSTMetricsList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvHSTMetricsList_OnRowDataBound">
            <HeaderStyle CssClass="HeadingCellTextLeft" />
            <RowStyle CssClass="DataCell" />
            <Columns>
                <asp:TemplateField HeaderText="Metric" ItemStyle-Width="26%">
					<ItemTemplate>
                        <asp:HiddenField  runat="server" ID="hfHSTMetricID" Value='<%# Eval("MEASURE_ID") %>'/>
                        <span>
				            <asp:Label runat="server" ID="lblHSTMetricName" CSSClass="textSmall"></asp:Label>
                            <br />
                            <asp:Label runat="server" ID="lblHSTMetricCode" CSSClass="refTextSmall"></asp:Label>
                        </span>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="" ItemStyle-Width="2%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblHSTRequired" ></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Value %>" ItemStyle-Width="13%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblHSTValue" CSSClass="textSmall" Text='<%# Eval("MEASURE_VALUE") %>'></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="UOM" ItemStyle-Width="5%">
					<ItemTemplate>
				         <asp:Label runat="server" ID="lblHSTValueUOM" CssClass="refTextSmall" Text='<%# Eval("UOM_ID") %>'></asp:Label>
 		            </ItemTemplate>
		        </asp:TemplateField>
                <asp:TemplateField HeaderText="Input Value" ItemStyle-Width="13%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblHSTInputValue" CSSClass="textSmall" Text='<%# Eval("INPUT_VALUE") %>'></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="UOM" ItemStyle-Width="5%">
					<ItemTemplate>
				         <asp:Label runat="server" ID="lblHSTInputUOM" CssClass="refTextSmall" Text='<%# Eval("INPUT_UOM_ID") %>'></asp:Label>
 		            </ItemTemplate>
		        </asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Cost %>" ItemStyle-Width="13%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblHSTCost" CSSClass="textSmall" Text='<%# Eval("MEASURE_COST") %>'></asp:Label>
                    </ItemTemplate>
		        </asp:TemplateField>
                <asp:TemplateField HeaderText="" ItemStyle-Width="5%">
					<ItemTemplate>
				         <asp:Label runat="server" ID="lblHSTCostCD" CssClass="refTextSmall" Text='<%# Eval("CURRENCY_CODE") %>'></asp:Label>
 		            </ItemTemplate>
		        </asp:TemplateField>
                <asp:TemplateField HeaderText="Input Cost" ItemStyle-Width="13%">
					<ItemTemplate>
				        <asp:Label runat="server" ID="lblHSTInputCost" CSSClass="textSmall" Text='<%# Eval("INPUT_COST") %>'></asp:Label>
                    </ItemTemplate>
				</asp:TemplateField>
                <asp:TemplateField HeaderText="" ItemStyle-Width="5%">
					<ItemTemplate>
				           <asp:Label runat="server" ID="lblHSTInputCostCD" CssClass="refTextSmall" Text='<%# Eval("INPUT_CURRENCY_CODE") %>'></asp:Label>
 		            </ItemTemplate>
		        </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
 </asp:Panel>
