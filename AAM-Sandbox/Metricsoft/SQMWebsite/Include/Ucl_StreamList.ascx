<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_StreamList.ascx.cs" Inherits="SQM.Website.Ucl_StreamList" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
    <link type="text/css" href="../css/redmond/jquery-ui-1.8.20.custom.css" rel="Stylesheet" />	
    <script type="text/javascript" src="../scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#tbReceiptDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "both",
                buttonImage: "/images/calendar.gif",
                buttonImageOnly: true,
                yearRange: "2000:2030",
                buttonText: "Select Material Receipt Date"
            });
            if ($('#tbReceiptDate').is(':disabled') == true) {
                $('#tbReceiptDate').datepicker().datepicker('disable');
            }
        });
    </script>

	<asp:Panel ID="pnlSuppStreamList" runat="server" Visible="false">
		<table width="100%">
			<tr>
				<td>
					<div id="divSuppStreamGVScroll" runat="server" class="">
						<asp:Repeater runat="server" ID="rptSuppStreamList" ClientIDMode="AutoID" OnItemDataBound="rptSuppStreamList_OnItemDataBound">
							<HeaderTemplate>
								<table cellspacing="0" cellpadding="2" border="0" width="100%" class="borderSoft">
							</HeaderTemplate>
							<ItemTemplate>
								<tr>
									<td class="listData" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblSuppName" text="Supplier" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkSuppName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listData" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblSuppPlantName" text="Supplier Location" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkSuppPlantName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listData" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblPartNum" Text="Part Number" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkPartNum" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
										    OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
								</tr>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<tr>
									<td class="listDataAlt" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblSuppName" text="Supplier" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkSuppName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listDataAlt" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblSuppPlantName" text="Supplier Location" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkSuppPlantName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listDataAlt" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblPartNum" Text="Part Number" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkPartNum" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
								</tr>
							</AlternatingItemTemplate>
							<FooterTemplate>
								</table></FooterTemplate>
						</asp:Repeater>
					</div>
					<asp:Label runat="server" ID="lblSuppStreamListEmpty" Height="40" Text="There are no items that match your search criteria."
						class="GridEmpty" Visible="false"></asp:Label>
				</td>
			</tr>
		</table>
	</asp:Panel>

    <asp:Panel ID="pnlCustStreamList" runat="server" Visible="false">
		<table width="100%">
			<tr>
				<td>
					<div id="divCustStreamGVScroll" runat="server" class="">
						<asp:Repeater runat="server" ID="rptCustStreamList" ClientIDMode="AutoID" OnItemDataBound="rptCustStreamList_OnItemDataBound">
							<HeaderTemplate>
								<table cellspacing="0" cellpadding="2" border="0" width="100%" class="borderSoft">
							</HeaderTemplate>
							<ItemTemplate>
								<tr>
									<td class="listData" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblCustName" text="Customer" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkCustName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listData" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblCustPlantName" text="Customer Location" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkCustPlantName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listData" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblPartNum" Text="Part Number" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkPartNum" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
										    OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
								</tr>
							</ItemTemplate>
							<AlternatingItemTemplate>
								<tr>
									<td class="listDataAlt" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblCustName" text="Customer" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkCustName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listDataAlt" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblCustPlantName" text="Customer Location" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkCustPlantName" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
                                    <td class="listDataAlt" valign="top">
										<span class="summaryHeader">
											<asp:Label runat="server" ID="lblPartNum" Text="Part Number" Visible="true"></asp:Label>
										</span>
										<br>
										<asp:LinkButton ID="lnkPartNum" runat="server" CommandArgument='<%#Eval("STREAM_ID") %>'
											OnClick="lnkStreamList_Click" CssClass="linkUnderline"></asp:LinkButton>
									</td>
								</tr>
							</AlternatingItemTemplate>
							<SeparatorTemplate>
								<tr>
									<td colspan="4">
										<hr />
									</td>
								</tr>
							</SeparatorTemplate>
							<FooterTemplate>
								</table></FooterTemplate>
						</asp:Repeater>
					</div>
					<asp:Label runat="server" ID="lblCustStreamListEmpty" Height="40" Text="There are no items that match your search criteria."
						class="GridEmpty" Visible="false"></asp:Label>
				</td>
			</tr>
		</table>
	</asp:Panel>

    <asp:Panel ID="pnlStreamHdr" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
				<td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblSuppName" Text="Supplier Name"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblSuppName_out"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblSuppPlantName" Text="Supplier Location"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblSuppPlantName_out"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPartNum" Text="Part Number"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPartNum_out"></asp:Label>
				</td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlStreamRecList" runat="server" Visible="false" class="editArea">
        <asp:Label ID="lblStreamRecList" runat="server" Text="Inspection History" CssClass="prompt" style="margin-left: 5px;"></asp:Label>
		<table width="100%">
			<tr>
				 <td valign="top" align=center>
					<div id="divStreamRecGVScroll" runat="server" class="">
                        <asp:GridView runat="server" ID="gvStreamRecList" Name="gvStreamRecList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%">
                            <HeaderStyle CssClass="HeadingCellText" />    
                            <RowStyle CssClass="DataCell" />
                	        <Columns>
                                <asp:TemplateField HeaderText="Receipt Date" ItemStyle-Width="15%">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hfDate" runat="server" Value='<%#Eval("EFF_DT") %>' />
                                        <asp:Label id="lblDate" runat="server" text='<%#Eval("EFF_DT") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="LOT_NUM" HeaderText="Lot Number" ItemStyle-Width="20%" />
                                <asp:BoundField DataField="ITEM_QTY" HeaderText="Qty Received" ItemStyle-Width="15%" />
                                <asp:BoundField DataField="ITEM_NC_QTY" HeaderText="Qty Reject" ItemStyle-Width="15%" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <asp:Label runat="server" ID="lblStreamRecListEmpty" Height="40" Text="There are no historical inspection records."
						class="GridEmpty" Visible="false"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlCRList" runat="server" Visible="false">
	<table width="100%">
		<tr>
			<td>
				<div id="divGVCRListRepeater" runat="server">
					<asp:Repeater runat="server" ID="rptCRList" ClientIDMode="AutoID" OnItemDataBound="rptCRList_OnItemDataBound">
						<HeaderTemplate><table cellspacing="0" cellpadding="2" border="0" width="100%" class="rptDarkBkgd"></HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="listDataAlt" valign="top" style="width: 25%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lbCRNameHdr" Text="Report Name"></asp:Label>
									</span>
									<br />
									<asp:LinkButton ID="lnkViewCRName" runat="server" CommandArgument='<%#Eval("CostReport.COST_REPORT_ID") %>'
										Text='<%#Eval("CostReport.COST_REPORT_NAME") %>' OnClick="lnkCRList_Click" CSSclass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listDataAlt" valign="top" style="width: 15%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCRIDHdr" Text="Report ID" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:LinkButton ID="lnkViewCRID" runat="server" CommandArgument='<%#Eval("CostReport.COST_REPORT_ID") %>'
										Text='<%#Eval("CostReport.COST_REPORT_ID") %>' OnClick="lnkCRList_Click" CSSclass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listDataAlt" valign="top" rowspan="2" style="width: 60%;">
									<span class="summaryHeader" style="text-align: center;">
										<asp:Label runat="server" ID="lblBusOrgHdr" Text="Issues Included"
											Visible="true"></asp:Label>
									</span>
									<br>
 									<Ucl:IssueList id="uclIncidents" runat="server"/>
								</td>
							</tr>
							<tr>
								<td class="listDataAlt" valign="top" style="width: 25%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCRCreated" Text="Created By"></asp:Label>
									</span>
									<br>
									<asp:Label runat="server" ID="lblCRCreateBy" Text='<%#Eval("CostReport.CREATE_BY") %>'></asp:Label>
									&nbsp;
									<asp:Label runat="server" ID="lblCRCreateDT" Text='<%#Eval("CostReport.CREATE_DT") %>'></asp:Label>
								</td>
								<td class="listDataAlt" valign="top" style="width: 15%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCRTypeHdr" Text="Costs Type"></asp:Label>
									</span>
									<br>
									<asp:Label runat="server" ID="lblCRType" Text='<%#Eval("CostReport.COST_REPORT_TYPE") %>'></asp:Label>
								</td>
							</tr>
                            <tr><td colspan="3"></td></tr>
						</ItemTemplate>
						<FooterTemplate></table></FooterTemplate>
					</asp:Repeater>
				</div>
				<asp:Label runat="server" ID="lbCRListEmptyRepeater" Text="There are currently no Cost Reports defined."
					class="GridEmpty" Visible="false"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>
