<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AdminList.ascx.cs" Inherits="SQM.Website.Ucl_AdminList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel ID="pnlCompanyListRepeater" runat="server" Visible="false">
	<table width="100%">
		<tr>
			<td >
                <asp:Label ID="lblCompanyListInstruction" runat="server" CssClass="instructText" Text="Select a Company from the list below."></asp:Label>
				<asp:Button ID="btnCompanyListClose" runat="server" Text="Close" CssClass="buttonStd"  
					Style="float:right; margin-right: 15px;" OnClick="btnCompanyListClose_Click" />
			</td>
		</tr>
		<tr>
			<td>
				<div id="divCompanyGVScrollRepeater" runat="server" class="">
					<asp:Repeater runat="server" ID="rptCompanyList" ClientIDMode="AutoID" OnItemDataBound="rptCompanyList_OnItemDataBound">
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="2" border="0" width="100%" class="borderSoft">
						</HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCompanyHdr" Text="Company" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:HiddenField runat="server" ID="hfCompanyID" Value='<%#Eval("COMPANY_ID") %>'/>
									<asp:LinkButton ID="lnkCompany_out" runat="server" CommandArgument='<%#Eval("COMPANY_ID") %>'
										Text='<%#Eval("COMPANY_NAME") %>' OnClick="lnkCompanyList_Click" CssClass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblUltDunsHdr" Text="Company Code" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:LinkButton ID="lnkUltDuns_out" runat="server" CommandArgument='<%#Eval("COMPANY_ID") %>'
										Text='<%#Eval("ULT_DUNS_CODE") %>' OnClick="lnkCompanyList_Click" CssClass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCompanyIsSupplier" Text="Supplier" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:CheckBox id="cbCompanyIsSupplier" runat="server" Enabled="false"></asp:CheckBox>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCompanyIsCustomer" Text="Customer" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:CheckBox id="cbCompanyIsCustomer" runat="server" Enabled="false"></asp:CheckBox>
								</td>
                                <td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblStatus" Text="Status" Visible="true"></asp:Label>
									</span>
									<br>
                                    <asp:Label runat="server" ID="lblStatusOut" Value='<%#Eval("STATUS") %>'/>
								</td>
							</tr>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<tr>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCompanyHdr" Text="Company" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:HiddenField runat="server" ID="hfCompanyID" Value='<%#Eval("COMPANY_ID") %>'/>
									<asp:LinkButton ID="lnkCompany_out" runat="server" CommandArgument='<%#Eval("COMPANY_ID") %>'
										Text='<%#Eval("COMPANY_NAME") %>' OnClick="lnkCompanyList_Click" CssClass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblUltDunsHdr" Text="Company Code" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:LinkButton ID="lnkUltDuns_out" runat="server" CommandArgument='<%#Eval("COMPANY_ID") %>'
										Text='<%#Eval("ULT_DUNS_CODE") %>' OnClick="lnkCompanyList_Click" CssClass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCompanyIsSupplier" Text="Supplier" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:CheckBox id="cbCompanyIsSupplier" runat="server" Enabled="false"></asp:CheckBox>
								</td>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblCompanyIsCustomer" Text="Customer" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:CheckBox id="cbCompanyIsCustomer" runat="server" Enabled="false"></asp:CheckBox>
								</td>
                                <td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblStatus" Text="Status" Visible="true"></asp:Label>
									</span>
									<br>
                                    <asp:Label runat="server" ID="lblStatusOut" Value='<%#Eval("STATUS") %>'/>
								</td>
							</tr>
						</AlternatingItemTemplate>
						<FooterTemplate>
							</table></FooterTemplate>
					</asp:Repeater>
				</div>
				<asp:Label runat="server" ID="lblCompanyListEmptyRepeater" Height="40" Text="There are currently no Companies matching your search criteria."
					class="GridEmpty" Visible="false"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlOrgListRepeater" runat="server" Visible="false">
	<table width="100%">
		<tr>
			<td>
				<div id="divGVScrollOrgListRepeater" runat="server">
					<asp:Repeater runat="server" ID="rptBusOrgList" ClientIDMode="AutoID" OnItemDataBound="rptOrgList_OnItemDataBound">
						<HeaderTemplate><table cellspacing="0" cellpadding="2" border="0" width="100%" class="rptDarkBkgd"></HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="listDataAlt" valign="top" style="width: 25%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblOrgNameHdr" Text="Business Organization" Visible="true"></asp:Label>
                                        <asp:Image id="imgBusOrg" runat="server" ImageUrl="~/images/defaulticon/16x16/sitemap.png" style="vertical-align: middle; border: 0px; margin-left: 4px;"/>
									</span>
									<br />
									<asp:LinkButton ID="lnkView" runat="server" CommandArgument='<%#Eval("BUS_ORG_ID") %>'
										Text='<%#Eval("ORG_NAME") %>' OnClick="lnkBusOrgList_Click" CSSclass="linkUnderline"></asp:LinkButton>
								</td>
								<td class="listDataAlt" valign="top" style="width: 15%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblLocCodeHdr" Text="Organization Code" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Label runat="server" ID="lblLocCode_out" Text='<%#Eval("DUNS_CODE") %>' Visible="true"></asp:Label>
								</td>
								<td class="listDataAlt" valign="top" rowspan="2" style="width: 60%;">
									<span class="summaryHeader" style="text-align: center;">
										<asp:Label runat="server" ID="lblBusOrgHdr" Text="Business Locations"
											Visible="true"></asp:Label>
									</span>
									<br>
			                        <div id="divPlantGVScroll" runat="server" class="">
                                        <asp:GridView runat="server" ID="gvPlantList" Name="gvPlantList" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvPlantList_OnRowDataBound">
                                            <HeaderStyle CssClass="HeadingCellTextLeft" />    
                                            <RowStyle CssClass="DataCell" />
                                            <AlternatingRowStyle CssClass="DataCellAlt" /> 
                	                        <Columns>
                    	                        <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                                <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                                                <asp:TemplateField HeaderText="Location Name" ItemStyle-Width="30%">
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblLocation" runat="server" Text="Location Name" CssClass="prompt"></asp:Label>
                                                        &nbsp;
                                                        <asp:Button id="btnAddPlant" runat="server" CssClass="buttonAdd" Text="Add" ToolTip="Add A New Business Location" OnClick="btnPlantAdd_Click" Visible="false"/>
                                                    </HeaderTemplate>
							                        <ItemTemplate>
                                                        <asp:HiddenField id="hfPlantCompanyID" runat="server"  Value='<%#Eval("COMPANY_ID") %>'/>
								                        <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("PLANT_ID") %>'
									                        Text='<%#Eval("PLANT_NAME") %>'  OnClick="lnkPlant_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                                    </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:BoundField DataField="DUNS_CODE" HeaderText="Location Code" ItemStyle-Width="15%" />
                                                <asp:TemplateField HeaderText="Location Type" ItemStyle-Width="15%">
							                        <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hfLocationType" Value='<%#Eval("LOCATION_TYPE") %>'/>
                                                        <asp:Label runat="server" ID="lblLocationType_out"  Text='<%#Eval("LOCATION_TYPE") %>'></asp:Label>
                                                    </ItemTemplate>
						                        </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                                        <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Label runat="server" ID="lblPlantListEmpty" Text="There are currently no Locations defined." class="GridEmpty" Visible="false"></asp:Label>
                                          <asp:Button id="btnAddPlantEmpty" runat="server" CssClass="buttonAdd" Text="Add" ToolTip="Add A New Business Location" OnClick="btnPlantAdd_Click" Visible="false"/>
                                    </div>
								</td>
							</tr>
							<tr>
								<td class="listDataAlt" valign="top" style="width: 25%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblParentBUHdr" Text="Parent Business Org" Visible="false"></asp:Label>
									</span>
									<br>
									<asp:Label runat="server" ID="lblParentBUHdr_out" Text='<%#Eval("PARENT_BUS_ORG_ID") %>' Visible="false"></asp:Label>
								</td>
								<td class="listDataAlt" valign="top" style="width: 15%;">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblOrgStatusHdr" Text="Status" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:HiddenField ID="hfStatus" runat="server" Value='<%#Eval("STATUS") %>' />
									<asp:Label ID="lblStatus" runat="server" DataTextField="STATUS"></asp:Label>
								</td>
							</tr>
                            <tr><td colspan="3"></td></tr>
						</ItemTemplate>
						<FooterTemplate></table></FooterTemplate>
					</asp:Repeater>
				</div>
				<asp:Label runat="server" ID="lblBusOrgListEmptyRepeater" Text="There are currently no Business Organizations defined."
					class="GridEmpty" Visible="false"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>


<asp:Panel ID="pnlDeptList" runat="server" Visible = "false" CssClass="admBkgd">
    <table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
        <tr>
            <td class="optionArea">
                <asp:Button ID="btnDeptAdd" CSSclass="buttonStd" runat="server" text="Add Department" 
                onclick="btnDeptAdd_Click" CommandArgument="add"></asp:Button>
            </td>
        </tr>
        <tr>
            <td valign="top" align="center">
                <div id="divDeptGVScroll" runat="server" class="">
                    <asp:GridView runat="server" ID="gvDeptList" Name="gvDeptList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvList_OnRowDataBound">
                        <HeaderStyle CssClass="HeadingCellTextLeft" />    
                        <RowStyle CssClass="DataCell" />
                	    <Columns>
                    	    <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                            <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                            <asp:BoundField  DataField="DEPT_ID" Visible="False"/>
                            <asp:TemplateField HeaderText="Department Name" ItemStyle-Width="45%">
							    <ItemTemplate>
								    <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("DEPT_ID") %>' CSSClass="linkUnderline" 
										Text='<%#Eval("DEPT_NAME") %>' OnClick="lnkDeptList_Click"></asp:LinkButton>
                                </ItemTemplate>
							</asp:TemplateField>
                            <asp:BoundField DataField="DEPT_CODE" HeaderText="Department Code" ItemStyle-Width="35%" />
                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                    <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label runat="server" ID="lblDeptListEmpty" Text="There are currently no Departments defined." class="GridEmpty" Visible="false"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>


<asp:Panel ID="pnlLaborList" runat="server" Visible = "false" CssClass="admBkgd">
    <table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
        <tr>
            <td class="optionArea">
                <asp:Button ID="btnLaborAdd" CSSclass="buttonStd" runat="server" text="Add Labor Code" 
                onclick="btnLaborAdd_Click" CommandArgument="add"></asp:Button>
            </td>
        </tr>
        <tr>
            <td valign="top" align="center">
                <div id="divLaborGVScroll" runat="server" class="">
                    <asp:GridView runat="server" ID="gvLaborList" Name="gvLaborList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvList_OnRowDataBound">
                        <HeaderStyle CssClass="HeadingCellTextLeft" />    
                        <RowStyle CssClass="DataCell" />
                	    <Columns>
                    	    <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                            <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                            <asp:BoundField  DataField="LABOR_TYP_ID" Visible="False"/>
                            <asp:TemplateField HeaderText="Labor Code" ItemStyle-Width="30%">
							    <ItemTemplate>
								    <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("LABOR_TYP_ID") %>' CSSClass="linkUnderline" 
										Text='<%#Eval("LABOR_CODE") %>' OnClick="lnkLaborList_Click"></asp:LinkButton>
                                </ItemTemplate>
							</asp:TemplateField>
                            <asp:BoundField DataField="LABOR_NAME" HeaderText="Name" ItemStyle-Width="40%" />
                            <asp:BoundField DataField="LABOR_RATE" HeaderText="Cost" ItemStyle-Width="15%" />
                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                    <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label runat="server" ID="lblLaborListEmpty" Text="There are currently no Labor Types defined." class="GridEmpty" Visible="false"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>


<asp:Panel ID="pnlLineList" runat="server" Visible = "false" CssClass="admBkgd">
    <table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
        <tr>
            <td align="left">
                <asp:Button ID="btnLineAdd" CSSclass="buttonStd" runat="server" text="Add Line/Operation" style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;" 
                onclick="btnLineAdd_Click" CommandArgument="add"></asp:Button>
            </td>
        </tr>
        <tr>
            <td valign="top" align="center">
                <div id="divLineGVScroll" runat="server" class="">
                    <asp:GridView runat="server" ID="gvLineList" Name="gvLineList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvList_OnRowDataBound">
                        <HeaderStyle CssClass="HeadingCellTextLeft" />    
                        <RowStyle CssClass="DataCell" />
                	    <Columns>
                            <asp:BoundField  DataField="PLANT_ID" Visible="False"/>
                            <asp:BoundField  DataField="PLANT_LINE_ID" Visible="False"/>
                            <asp:TemplateField HeaderText="Line/Operation Name" ItemStyle-Width="60%">
							    <ItemTemplate>
								    <asp:LinkButton ID="lnkView_out" runat="server" CommandArgument='<%#Eval("PLANT_LINE_ID") %>' CSSClass="linkUnderline" 
										Text='<%#Eval("PLANT_LINE_NAME") %>' OnClick="lnkLineList_Click"></asp:LinkButton>
                                </ItemTemplate>
							</asp:TemplateField>
                            <asp:BoundField DataField="DOWNTIME_RATE" HeaderText="Downtime Rate (per hour)" ItemStyle-Width="25%" />
                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                    <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label runat="server" ID="lblLineListEmpty" Text="There are currently no Lines/Operations defined." class="GridEmpty" Visible="false"></asp:Label>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlUserList" runat="server" Visible="false">
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td class="admBkgd" align="center">
                <div id="divUserGVScroll" runat="server" class="">
                    <telerik:RadGrid ID="rgUserList" name="rgUserList" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="False" AutoGenerateColumns="false" 
                        OnItemDataBound="rgUserList_ItemDataBound" OnSortCommand="rgUserList_SortCommand" GridLines="None" Width="99%">
                        <MasterTableView ExpandCollapseColumn-Visible="false">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderText="User Name" ShowSortIcon="true" SortExpression="LAST_NAME">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkUserName" runat="server" Text='<%# Eval("LAST_NAME") + ", " + Eval("FIRST_NAME") %>' CommandArgument='<%# Eval("PERSON_ID") %>'
								            CssClass="buttonLink" OnClick="lnkUserView_Click" ToolTip="Edit this user"></asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Email Address" ShowSortIcon="true" SortExpression="EMAIL">
                                    <ItemTemplate>
                                        <asp:Label ID="lnkUserEmail" runat="server" Text='<%# Eval("EMAIL") %>' CommandArgument='<%# Eval("PERSON_ID") %>'
								            CssClass="textStd"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Job Code" ShowSortIcon="true" SortExpression="ROLE">
                                    <ItemTemplate>
                                         <asp:Label ID="lblUserRole" runat="server" Text='<%# Eval("JOBCODE_CD") %>'></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="HR Location" ShowSortIcon="true" SortExpression="PLANT_ID">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkHRLocation" runat="server" Text='<%# Eval("PLANT_ID") %>' CommandArgument='<%# Eval("PERSON_ID") %>' CssClass="buttonLink" OnClick="lnkUserView_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Status" ShowSortIcon="true" SortExpression="STATUS">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUserStatus" runat="server" Text='<%# Eval("STATUS") %>'></asp:Label>
                                        <asp:Image id="imgStatus" runat="server" Visible="false" ToolTip="User is inactive" style="vertical-align: middle; border: 0px;"/>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                    <asp:Label runat="server" ID="lblUserListEmpty" Height="40px" Text="There are currently no Users defined." class="GridEmpty" Visible="False"></asp:Label>
                </div>
            </td>
        </tr>
    </table>

</asp:Panel>

