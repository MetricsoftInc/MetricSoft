<%@ Page Title="" Language="C#" MasterPageFile="~/Problem.master" AutoEventWireup="true" CodeBehind="Quality_ViewSCORModel.aspx.cs" Inherits="SQM.Website.Quality_ViewSCORModel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
  <script type="text/javascript">
 
    </script>

    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <table width="98%">
			                <tr>
                                <td class="pageTitles">
                                    <span>
                                        <asp:Label ID="lblSCORTitle" runat="server" Text="Fulfillment Performance Metrics"></asp:Label>
                                    </span>
                                </td>
                                <td align="right">
						            <table border="0" cellspacing="0" cellpadding="2">
						  	            <tr>
                                            <td>
                                                <asp:Button ID="btnCancel1" runat="server" OnClientClick="return confirmAction('Cancel without saving');" onClick="btnCancel_Click" CSSclass="buttonStd" text="Cancel"></asp:Button>
									        </td>
								            <td>
                                                <asp:Button ID="btnSave1" class="buttonEmphasis" runat="server"  OnClientClick="return confirmChange('Metric definitions');" onclick="btnSave_Click" text="Save" CommandArgument=""></asp:Button>
									        </td>
                                        </TR>
                                    </TABLE>
                                </td>
                            </tr>
                        </table>

                        <table width="98%">
                            <tr height="28" class="tableDataHdr">
			                    <td class="tableDataHdr2" >
                                    <asp:Label ID="lblModelDetail" runat="server" Text="Model Definition" ></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="summaryBkgd" valign="top" align="center">
                                    <table cellspacing=0 cellpadding=2 border=0 width="100%">
					                    <tr>
                                            <td class=summaryData valign=top>
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblModelHdr" Text="Description" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <asp:Label runat="server" ID="lblModel_out" Text="" Visible="true"></asp:Label>
						                    </td>
			                                <td class=summaryData valign=top>
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblUpdatByHdr" Text="Updated By" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
                                               <asp:Label runat="server" ID="lblUpdateBy_out" Text="" Visible="true"></asp:Label>
						                    </td>
                                            <td class=summaryData valign=top>
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblUpdateDateHdr" Text="Line" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
                                               <asp:Label runat="server" ID="lblUpdateDate_out" Text="" Visible="true"></asp:Label>
						                    </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <table width="98%" border="0" cellspacing="0" cellpadding="2">
			                <tr>
			                    <td class=admBkgd align=center>	
                                    <asp:GridView runat="server" ID="gvMetrics" Name="gvMetrics" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvMetrics_OnRowDataBound">
                                    <HeaderStyle CssClass="HeadingCellText" />    
                                    <RowStyle CssClass="DataCell" />
                	                    <Columns>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMetricID" runat="server" text='<%#Eval("SCOR_METRIC_ID") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="METRIC_NAME" HeaderText="Metric" ItemStyle-Width="15%" />
                                            <asp:BoundField DataField="METRIC_DESC" HeaderText="Description" ItemStyle-Width="25%" />
           			                        <asp:TemplateField HeaderText="Factors" ItemStyle-Width="60%">
                                                <ItemTemplate>
                                                    <asp:GridView runat="server" ID="gvFactorGrid" Name="gvFactorGrid" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvFactor_OnRowDataBound">
                                                        <HeaderStyle CssClass="HeadingCellText" />    
                                                        <RowStyle CssClass="DataCell" Height=26 />
                                                        <Columns>
                                                            <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblMetricID" runat="server" text='<%#Eval("SCOR_METRIC_ID") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                           <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblMetricFactorID" runat="server" text='<%#Eval("METRIC_FACTOR_ID") %>'></asp:Label>
                                                                </ItemTemplate>
                                                          </asp:TemplateField>
                                                          <asp:TemplateField HeaderText="Factor" ItemStyle-Width="40%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbFactorName_out" runat="server"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Description" ItemStyle-Width="50%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbFactorDesc_out" runat="server"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                       <asp:TemplateField HeaderText="Weight" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="tbFactorWeight" runat="server" style="width:90%;"></asp:TextBox>
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
                    </FORM>
                </td>
            </tr>
        </table>
        <br />
    </div>
</asp:Content>
