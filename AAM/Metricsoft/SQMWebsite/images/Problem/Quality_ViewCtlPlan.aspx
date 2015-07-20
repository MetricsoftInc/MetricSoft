<%@ Page Title="" Language="C#" MasterPageFile="~/Problem.master" AutoEventWireup="true" CodeBehind="Quality_ViewCtlPlan.aspx.cs" Inherits="SQM.Website.Quality_ViewCtlPlan" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminList.ascx" TagName="AdminList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:Panel runat="server" ID="pnlSearchBar">
                            <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                        </asp:Panel>
                        <table width="99%">
			                <tr>
                                <td>
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Define quality control and reaction plans for parts and assemblies throughout their manufacturing process. Search for control plans by part name or part number."></asp:Label>
                                    <asp:Label ID="lblCtlPlanTitle" runat="server" Text="Quality Control Plan" Visible="false"></asp:Label>
                                </td>
                            </tr>
                        </table>

                        <asp:Panel ID="pnlSearchList" runat="server" Visible="false">
                            <Ucl:AdminList id="uclAdminList" runat="server"/>
                        </asp:Panel>

                        <div id="divPageBody" runat="server" style="width: 99%;">
                            <Ucl:ItemHdr id="uclItemHdr" runat="server"/>
                              <table width="100%" border="0" cellspacing="0" cellpadding="2" style="margin-top: 8px;">
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
                                                <asp:TemplateField HeaderText="Operation/<br>Location" ItemStyle-Width="13%" ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPlanStepName_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("STEP_NAME") %>'></asp:Label>
                                                        <br />
                                                         <asp:HiddenField ID="hfPlanStepLocation" runat="server" Value='<%#Eval("LOCATION_ID") %>'/>
                                                         <asp:Image ID="imgPlanStepLocation" Visible="true" runat="server"/>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sampling<br>Plan" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="tbSampleSize" runat="server" CssClass="DataCellSmall" style="width:97%;" text='<%#Eval("SAMPLE_SIZE") %>'></asp:TextBox>
                                                        <!--<asp:Label ID="lblSampleUnit_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("SAMPLE_UNIT") %>'></asp:Label>-->
                                                        <asp:HiddenField ID="hfSampleUnit" runat="server" Value='<%#Eval("SAMPLE_UNIT") %>'/>
                                                        <asp:DropDownList ID="ddlSampleUnit" runat="server" CssClass="DataCellSmall" style="width:98%;"></asp:DropDownList>
                                                        <br />
                                                        Per&nbsp;
                                                        <!--<asp:Label ID="lblSampleRate_out" runat="server" CssClass="DataCellSmall" text='<%#Eval("SAMPLE_RATE") %>'></asp:Label>-->
                                                        <asp:HiddenField ID="hfSampleRate" runat="server" Value='<%#Eval("SAMPLE_RATE") %>'/>
                                                        <asp:DropDownList ID="ddlSampleRate" runat="server" CssClass="DataCellSmall" style="width:98%;"></asp:DropDownList>
                                                        <br />
                                                         <a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC_ID={0}", "15")%>' target="_blank">
                                                         <asp:Image id="imgOperImg" runat="server" ImageUrl="~/images/attach.gif" title="Step instructions"></asp:Image>
                                                         </a>
                                                        <!--<asp:ImageButton ID="btnStepAttachment" runat="server" ImageUrl="~/images/attach.gif" title="Attachments" />-->
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
                                                                <asp:TemplateField HeaderText="Type" ItemStyle-Width="20%">
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
                                                                        <asp:LinkButton ID="lblControlMethod" runat="server" CssClass="DataCellSmall" text='<%#Eval("CONTROL_METHOD") %>'  OnClientClick="PopupCenter('../Problem/ControlMethodList.aspx?', 'newPage', 600, 665); return false;"></asp:LinkButton>
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
                        </div>
                    </form>
                </td>
            </tr>
        </table>

        <br>
    </div>
</asp:Content>
