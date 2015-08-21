<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PersonList.ascx.cs" Inherits="SQM.Website.Ucl_PersonList" %>

<asp:Panel ID="pnlPartList" runat="server" Visible="false">
	<div id="divPartListScroll" runat="server" class="">
		<asp:GridView runat="server" ID="gvPartList" Name="gvPartList" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvPartList_OnRowDataBound">
			<HeaderStyle CssClass="HeadingCellTextLeft" />    
			<RowStyle CssClass="DataCell" />
			<AlternatingRowStyle CssClass="DataCellAlt" /> 
			<Columns>
				<asp:TemplateField HeaderText="Part Number">
					<ItemTemplate>
						<asp:LinkButton ID="lnkViewPart_out" runat="server" CommandArgument='<%#Eval("Part.PART_ID") %>'
							text='<%#Eval("Part.PART_NUM") %>'  CSSclass="linkUnderline" OnClick="lnkPart_Click"></asp:LinkButton>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Part Description">
					<ItemTemplate>
						<asp:Label ID="lblPartDesc_out" runat="server" text='<%#Eval("Part.PART_NAME") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
		<asp:Label runat="server" ID="lblPartListEmpty" Height="40" Text="Part List Is Empty." class="GridEmpty" Visible="false"></asp:Label>
	</div>
</asp:Panel>


<asp:Panel ID="pnlPartPlantList" runat="server" Visible = "false">
	<table width="99%" border="0" cellspacing="0" cellpadding="1">
		<tr>
			<td class=admBkgd align=center>
				<div id="divPartPlantGVScroll" runat="server" class="">
					<asp:GridView runat="server" ID="gvPartPlantList" Name="gvPartPlantList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" style="margin-top: 8px;" OnRowDataBound="gvPartPlant_OnRowDataBound">
						<HeaderStyle CssClass="HeadingCellTextLeft" />    
						<RowStyle CssClass="DataCell" />
							<Columns>
								<asp:TemplateField HeaderText="Business Org" ItemStyle-Width="15%">
									<ItemTemplate>
										<asp:Label ID="lblCustBusOrg" runat="server" text='<%#Eval("CUST_BUS_ORG_NAME") %>'></asp:Label>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="15%" Visible="false">
									<ItemTemplate>
										<asp:Label ID="lblCustPlantID" runat="server" text='<%#Eval("CUSTOMER_PLANT_ID") %>'></asp:Label>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Location" ItemStyle-Width="15%">
									<ItemTemplate>
										<asp:Label ID="lblCustPlant" runat="server" text='<%#Eval("CUST_PLANT_NAME") %>'></asp:Label>
									</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Supplier" ItemStyle-Width="40%">
									<ItemTemplate>
										<asp:GridView runat="server" ID="gvPartSuppGrid" Name="gvPartSuppGrid" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%">
											<HeaderStyle CssClass="HeadingCellTextLeft" />    
											<RowStyle CssClass="DataCell" />
											<Columns>
												<asp:BoundField DataField="SUPP_COMPANY_NAME" HeaderText="Company" ItemStyle-Width="50%" />
												<asp:BoundField DataField="SUPP_PLANT_NAME" HeaderText="Supplier Location" ItemStyle-Width="50%" />
											</Columns>
										</asp:GridView>
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
						</asp:GridView>
					<asp:Label runat="server" ID="lblPartPlantListEmpty" Height="40" Text="There are currently no Part / Location associations defined." class="GridEmpty" Visible="false"></asp:Label>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlProgramList" runat="server" Visible = "false"  CssClass="admBkgd">
	<table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
		<tr><td>&nbsp;</td></tr>
		<tr>
			 <td valign="top" align="center" class="editArea">
				<div id="divProgramGVScroll" runat="server" class="">
					<asp:GridView runat="server" ID="gvProgramList" Name="gvProgramList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvProgramList_OnRowDataBound">
						<HeaderStyle CssClass="HeadingCellTextLeft" />    
						<RowStyle CssClass="DataCell" />
						<Columns>
							<asp:TemplateField HeaderText="Customer Company" ItemStyle-Width="30%">
								<ItemTemplate>
									<asp:HiddenField id="hfCustomerID" runat="server" value='<%#Eval("CUSTOMER_ID") %>'/>
									<asp:Label ID="lblCustomer" runat="server" text='<%#Eval("CUSTOMER_ID") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Part Programs" ItemStyle-Width="70%">
								<ItemTemplate>
									<asp:GridView runat="server" ID="gvProgram" Name="gvProgram" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" >
										<HeaderStyle CssClass="HeadingCellTextLeft" />    
										<RowStyle CssClass="DataCell" Height=26 />
										<Columns>
											<asp:BoundField  DataField="PROGRAM_ID" Visible="False"/>
											<asp:BoundField DataField="PROGRAM_NAME" HeaderText="Program Name" ItemStyle-Width="35%" />
											<asp:BoundField DataField="PROGRAM_CODE" HeaderText="Program Code" ItemStyle-Width="20%" />
											<asp:BoundField DataField="PROGRAM_DESC" HeaderText="Program Description" ItemStyle-Width="45%" />
										</Columns>
									</asp:GridView>
								</ItemTemplate>
							</asp:TemplateField>
						</Columns>
					</asp:GridView>
					<asp:Label runat="server" ID="lblProgramListEmpty" Text="There are currently no Part Programs defined." class="GridEmpty" Visible="false"></asp:Label>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlProgramPartList" runat="server" Visible="false">
	<table width="100%">
		<tr>
			<td>
				<asp:Repeater runat="server" ID="rptProgramPart" ClientIDMode="AutoID" OnItemDataBound="rptProgramPart_OnItemDataBound">
					<HeaderTemplate><table cellspacing="0" cellpadding="2" border="0" width="100%" class="rptDarkBkgd"></HeaderTemplate>
					<ItemTemplate>
						<tr>
							<td class="listDataAlt" valign="top" style="width: 20%;">
								<span class="summaryHeader">
									<asp:Label runat="server" ID="lblPartSourceHdr" Text="Source"></asp:Label>
								</span>
								<br />
								<asp:Label runat="server" ID="lblPartSource"></asp:Label>
								<br />
								<asp:Label runat="server" ID="lblPartSourceCode" CssClass="refText"></asp:Label>
							</td>
							<td class="listDataAlt" valign="top" style="width: 20%;">
								<span class="summaryHeader">
									<asp:Label runat="server" ID="lblPartUsedHdr" Text="Where Used"></asp:Label>
								</span>
								<br>
								<asp:Label runat="server" ID="lblPartUsed" ></asp:Label>
								<br />
								<asp:Label runat="server" ID="lblPartUsedCode" CssClass="refText"></asp:Label>
							</td>
							<td class="listDataAlt" valign="top" rowspan="1" style="width: 60%;">
								<span class="summaryHeader" style="text-align: center;">
									<asp:Label runat="server" ID="lblPartsHdr" Text="Parts List"></asp:Label>
								</span>
								<br>
								<div id="divPlantGVScroll" runat="server" class="">
									<asp:GridView runat="server" ID="gvProgramPartList" Name="gvProgramPartList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvProgramPartList_OnRowDataBound">
										<HeaderStyle CssClass="HeadingCellTextLeft" />    
										<RowStyle CssClass="DataCell" />
										<AlternatingRowStyle CssClass="DataCellAlt" /> 
										<Columns>
											<asp:TemplateField HeaderText="Part Number" ItemStyle-Width="40%">
												<ItemTemplate>
													<asp:LinkButton ID="lnkPartNumber" runat="server" CommandArgument='<%#Eval("Part.PART_ID") %>'
														Text='<%#Eval("Part.PART_NUM") %>'  OnClick="lnkPart_Click" CSSclass="linkUnderline" ToolTip="View part details"></asp:LinkButton>
												</ItemTemplate>
											</asp:TemplateField>
											<asp:TemplateField HeaderText="Part Description" ItemStyle-Width="60%">
												<ItemTemplate>
													<asp:Label ID="lblPartName" runat="server" CssClass="textStd" Text='<%#Eval("Part.PART_NAME") %>'></asp:Label>
												</ItemTemplate>
											</asp:TemplateField>
										</Columns>
									</asp:GridView>
									<asp:Label runat="server" ID="lblProgramPartListEmpty" Text="There are currently no parts defined." class="GridEmpty" Visible="false"></asp:Label>
								</div>
							</td>
						</tr>
						<tr><td colspan="3"></td></tr>
					</ItemTemplate>
					<FooterTemplate></table></FooterTemplate>
				</asp:Repeater>
				<asp:Label runat="server" ID="lblProgramPartEmptyRepeater" Text="There are currently no parts defined."
					class="GridEmpty" Visible="false"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>
 