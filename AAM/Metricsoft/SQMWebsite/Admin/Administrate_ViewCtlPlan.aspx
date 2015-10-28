<%@ Page Title="" Language="C#" MasterPageFile="~/Problem.master" AutoEventWireup="true" CodeBehind="Administrate_ViewCtlPlan.aspx.cs" Inherits="SQM.Website.Administrate_ViewCtlPlan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />

                    <table width="98%" border="0" cellspacing="1" cellpadding="0" class="darkBorder">
			            <tr>
			  	            <td class="tableDataHdr">
					            <table width="100%">
						            <tr>
							            <td class="tableDataHdr2">
                                            <asp:Label ID="lblViewPartTitle" runat="server" Text="Quality Control Plan"></asp:Label>
                                        </td>
							            <td align="right">
								            <table border="0" cellspacing="0" cellpadding="2">
						  			            <tr>
                                                    <td>
                                                        <asp:Button ID="lbSearchParts" CSSclass="buttonStd" runat="server" onClientclick="PopupCenter('../Shared/Shared_PartSearch.aspx?', 'newPage', 800, 600);"  text="Search Control Plans" CommandArgument=""></asp:Button>
									                </td>
 									                <td>
                                                        <asp:Button ID="lbPlanEdit" CSSclass="buttonStd" runat="server" text="Edit Plan" onclick="tab_Click" CommandArgument="edit"></asp:Button>
									                </td>
                                                    <td>
                                                        <asp:Button ID="lbPlanAdd" CSSclass="buttonStd" runat="server" text="Add Plan" onclick="tab_Click" CommandArgument="add"></asp:Button>
									                </td>
                                                    <td>
                                                        <asp:Button ID="lbUploadData" runat="server" class="buttonStd" onclick="lbUploadData_Click" text="Upload Data"></asp:Button>
                                                    </td>
                                                </TR>
                                            </TABLE>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="summaryBkgd" valign="top" align="center">
                                <!--#include file="/Include/Inc_Ctlplan_Detail.aspx"-->
                            </td>
                        </tr>
                    </table>

                     <table width="98%" border="0" cellspacing="0" cellpadding="2">
			            <tr>
			                <td class=admBkgd align=center>
                                <asp:GridView runat="server" ID="gvCtlPlan" Name="gvCtlPlan" CssClass="GridSmall" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvCtlPlan_OnRowDataBound">
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
                                                <br />
                                                <asp:HiddenField ID="hfStepInstructions" runat="server" Value='<%#Eval("INSTRUCTIONS") %>'/>
                                                <asp:Image ID="imgStepType" ImageUrl="~/images/step_01.gif" Visible="true" runat="server"/>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Operation/<br>Location" ItemStyle-Width="15%" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPlanStepName_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("STEP_NAME") %>'></asp:Label>
                                                <br />
                                                 <asp:HiddenField ID="hfPlanStepLocation" runat="server" Value='<%#Eval("LOCATION_ID") %>'/>
                                                 <asp:Image ID="imgPlanStepLocation" Visible="true" runat="server"/>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sampling<br>Plan" ItemStyle-Width="10%" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSampleSize_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("SAMPLE_SIZE") %>'></asp:Label>
                                                &nbsp;
                                                <asp:Label ID="lblSampleUnit_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("SAMPLE_UNIT") %>'></asp:Label>
                                                <br />
                                                Per&nbsp;
                                                <asp:Label ID="lblSampleRate_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("SAMPLE_RATE") %>'></asp:Label>
                                                <br />
                                                <asp:ImageButton ID="btnStepAttachment" runat="server" ImageUrl="~/images/attach.gif" title="<%$ Resources:LocalizedText, Attachments %>" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
           			                    <asp:TemplateField HeaderText="Measures" ItemStyle-Width="35%">
                                            <ItemTemplate>
                                                <asp:GridView runat="server" ID="gvMeasureGrid" Name="gvMeasureGrid" CssClass="GridSmall" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvMeasure_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellText" />
                                                    <RowStyle CssClass="DataCell" Height=26 />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="No." ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblMeasureNum" runat="server" CssClass="DataCellSmall" style="font-weight: bold;" text='<%#Eval("MEASURE_NUM") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Characteristic" ItemStyle-Width="60%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="tbMeasureName" runat="server" CssClass="DataCellSmall" style="width:97%;" text='<%#Eval("MEASURE_NAME") %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Class" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfMeasureClass" runat="server" Value='<%#Eval("MEASURE_CLASS") %>'/>
                                                                <asp:HiddenField ID="hfMeasureInstructions" runat="server" Value='<%#Eval("MEASURE_INSTRUCTIONS") %>'/>
                                                                <asp:Image ID="imgMeasureClass" Visible="true" runat="server"/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Type %>" ItemStyle-Width="20%">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfMeasureType" runat="server" Value='<%#Eval("MEASURE_TYPE") %>'/>
                                                                <asp:DropDownList ID="ddlMeasureType" runat="server" CssClass="DataCellSmall"></asp:DropDownList>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Methods" ItemStyle-Width="35%">
                                            <ItemTemplate>
                                                <asp:GridView runat="server" ID="gvMethodGrid" Name="gvMethodGrid" CssClass="GridSmall" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvMethod_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellText" />
                                                    <RowStyle CssClass="DataCell" Height="26" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Spec Type" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfSpecType" runat="server" Value='<%#Eval("SPEC_TYPE") %>'/>
                                                                <asp:DropDownList ID="ddlSpecType" runat="server" CssClass="DataCellSmall"></asp:DropDownList>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="UOM" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="tbMeasureUOM" runat="server" CssClass="DataCellSmall" text='<%#Eval("UOM") %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Limits" ItemStyle-Width="30%">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfSpecLSL" runat="server" Value='<%#Eval("LSL") %>'/>
                                                                <asp:HiddenField ID="hfSpecUSL" runat="server" Value='<%#Eval("USL") %>'/>
                                                                <asp:TextBox ID="tbSpecValues" runat="server" CssClass="DataCellSmall"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Gauge Sys" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="tbGauge" runat="server" CssClass="DataCellSmall" text='<%#Eval("GAUGE_TYPE") %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Control" ItemStyle-Width="10%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblControlMethod" runat="server" CssClass="DataCellSmall" text='<%#Eval("CONTROL_METHOD") %>'></asp:Label>
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

                    </form>
                </td>
            </tr>
        </table>

        <br>
    </div>
</asp:Content>
