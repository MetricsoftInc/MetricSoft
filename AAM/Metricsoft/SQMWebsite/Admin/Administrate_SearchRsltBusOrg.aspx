<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_SearchRsltBusOrg.aspx.cs" Inherits="SQM.Website.Administrate_SearchRsltBusOrg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <asp:HiddenField ID="hfBase" runat="server" />
			        <BR/>
<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
			            <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblBusOrgSearchTitle" runat="server" Text="Business Organization - Search Results"></asp:Label>
                            </td>
					    </tr>
				    </table><BR/>

<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="2" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="30%">
                                <b>New Search </b>
		                    </td>
                            <td class="tabActiveTableBg" width="70%">
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>&nbsp;
                                <asp:Button ID="lbSearchBusOrg" runat="server" Text="Go" CSSclass="buttonStd" onclick="lbSearchBusOrg_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                    <br>

       	            <table width="98%" cellpadding="3" cellspacing="1" border="0" class="darkBorder">
                        <TR>
		                    <TD class="tableDataHdr" colspan="5">
			                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
				                    <tr>
					                    <td class="tableDataHdr2">
                                            <asp:Label runat="server" ID="lblBusOrgResultsHdr" Text="Results" Visible="true"></asp:Label>
                                        </td>
					                    <td align="right">
						                <!-- BUTTONS -->
					                        <table cellpadding=0 cellspacing=2 border=0>
						                        <tr>
                                                   <td>
                                                        <asp:Button ID="lbBusOrgAdmin" runat="server" onClick="lbBusOrgAdmin_Click" CSSclass="buttonStd" text="Business Organization Admin"></asp:Button>
									                </td>
                                                </tr>
                                            </table>
               			                <!-- END OF BUTTONS -->
					                    </td>
				                    </tr>
			                    </table>
                            </TD>
                        </TR>

                        <tr>
                            <td>
                            <!-- results grid -->
                             <div ID="divGVScroll" runat="server" >
                            <asp:GridView runat="server" ID="gvBusOrgList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="2" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvBusOrgList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellText" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="BUS_ORG_ID" Visible="False"/>
                                    <asp:TemplateField HeaderText="Organization Name" ItemStyle-Width="30%">
							            <ItemTemplate>
								            <asp:LinkButton ID="lnkView" runat="server" CommandArgument='<%#Eval("BUS_ORG_ID") %>'
										    	Text='<%#Eval("ORG_NAME") %>' OnClick="lnkView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                        </ItemTemplate>
							        </asp:TemplateField>
                                    <asp:BoundField DataField="DUNS_CODE" HeaderText="DUNS Code" ItemStyle-Width="10%" />
                                    <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Status %>" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfStatus" runat="server" Value='<%#Eval("STATUS") %>' />
                                            <asp:Label ID="lblStatus" runat="server" DataTextField="STATUS" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Associated Plants" ItemStyle-Width="50%">
                                        <ItemTemplate>
                                            <asp:GridView runat="server" ID="gvPlantGrid" Name="gvPlantGrid" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%">
                                                <HeaderStyle CssClass="HeadingCellText" />
                                                <RowStyle CssClass="DataCell" />
                                                <Columns>
                                                    <asp:BoundField DataField="PLANT_ID" HeaderText="Plant ID" Visible="false"/>
                                                    <asp:TemplateField HeaderText="Plant Name" ItemStyle-Width="40%">
							                            <ItemTemplate>
								                            <asp:LinkButton ID="lnkViewPlant" runat="server" CommandArgument='<%#Eval("PLANT_ID") %>'
										    	            Text='<%#Eval("PLANT_NAME") %>' OnClick="lnkPlantView_Click" CSSclass="linkUnderline"></asp:LinkButton>
                                                        </ItemTemplate>
							                        </asp:TemplateField>
                                                    <asp:BoundField DataField="DUNS_CODE" HeaderText="<%$ Resources:LocalizedText, LocationCode %>" ItemStyle-Width="40%" />
                                                </Columns>
                                            </asp:GridView>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </div>
                            <asp:Label runat="server" ID="lblBusOrgListEmpty" Text="There are currently no Business Organizations defined." class="GridEmpty" Visible="false"></asp:Label>
                            </td>
                        </tr>

                        <TR>
		                    <TD class="tableDataHdr" colspan="5">
			                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
				                    <tr>
					                    <td align="right">
						                <!-- BUTTONS -->
					                        <table cellpadding=0 cellspacing=2 border=0>
						                        <tr>
                                                   <td>
                                                        <asp:Button ID="lbBusOrgAdmin2" runat="server" onClick="lbBusOrgAdmin_Click" CSSclass="buttonStd" text="Business Organization Admin"></asp:Button>
									                </td>
                                                </tr>
                                            </table>
               			                <!-- END OF BUTTONS -->
					                    </td>
				                    </tr>
			                    </table>
                            </TD>
                        </TR>

			        </table>
                </TD>
            </TR>
        </table>
        <br>
    </div>
</asp:Content>
