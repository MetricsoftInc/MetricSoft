<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Resources.aspx.cs" Inherits="SQM.Website.EHS_Resources"%>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocMgr" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<script type="text/javascript">

    function OpenMeasureEditWindow() {
        $find("<%=winMeasureEdit.ClientID %>").show();
    }

</script>

     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <br />
                    <form name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:HiddenField ID="hfOper" runat="server" />
                        <asp:HiddenField ID="hfActiveTab" runat="server" />
                        <asp:HiddenField ID="hfAddMeasure" runat ="server" Value="Add Metric" />
                        <asp:HiddenField ID="hfUpdateMeasure" runat ="server" Value="Update Metric" />
                        <asp:HiddenField id="hfErrRequiredInputs" runat="server" Value="Please enter all required (*) fields before saving."/>
                        <asp:HiddenField id="hfDuplicateMeasure" runat="server" Value="Duplicate Metric Code."/>
                         <table width="99%">
			                <tr>
                                <td class="pageTitles">
                                    <asp:Label ID="lblViewEHSRezTitle" runat="server"  Text="Environmental, Health & Safety System Library" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Manage Environmental, Health & Safety system resources and reference information."></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <div id="divPageBody" runat="server">
                            <div id="divNavArea" runat="server"  class="navAreaLeft">
                                <Ucl:AdminTabs id="uclAdminTabs" runat="server"/>
                            </div>

                            <div id="divWorkArea" runat="server" class="workAreaRight">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="editArea">
									        <Ucl:DocMgr id="uclDocMgr" runat="server"/>

                                            <telerik:RadWindow runat="server" ID="winMeasureEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="300" Width="700" Behaviors="Move" Title="Metric Code">
                                                <ContentTemplate>
                                                    <asp:Panel id="pnlMeasureEdit" runat="server" Visible="false">
                                                        <table width="99%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                                            <tr>
                                                                <td class="columnHeader" width="24%">
                                                                    <asp:Label ID="lblMeasureCode" runat="server" text="Metric Code"></asp:Label>
                                                                </td>
                                                                <td class="required" width="1%">&nbsp;</td>
                                                                <td CLASS="tableDataAlt" width="75%">
                                                                    <asp:TextBox ID="tbMeasureCode"  runat="server" MaxLength="100" Columns="20"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureName" runat="server" text="Metric Name"></asp:Label>
                                                                </td>
                                                                <td class="required">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:TextBox ID="tbMeasureName"  runat="server" TextMode="multiline" rows="2" MaxLength="100" CssClass="textStd" style="width: 97%;"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureDesc" runat="server" text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:TextBox ID="tbMeasureDesc"  runat="server" TextMode="multiline" rows="3" MaxLength="200" CssClass="textStd" style="width: 97%;"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureSubcategory" runat="server" text="Sub Category"></asp:Label>
                                                                </td>
                                                                <td class="required">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:DropDownList ID="ddlMeasureSubcategory" runat="server"></asp:DropDownList>
                                                                    &nbsp;&nbsp;
                                                                    <asp:PlaceHolder ID="phProdTableField" runat="server">
                                                                        <asp:Label id="lblProdTableField" runat="server" Text="Production Data Type: " CssClass="prompt"></asp:Label>
                                                                        <asp:DropDownList ID="ddlProdTableField" runat="server">
                                                                            <items>
                                                                                <asp:ListItem Value="OPER_COST">Material Cost</asp:ListItem>
                                                                                <asp:ListItem Value="REVENUE">Revenue</asp:ListItem>
                                                                                <asp:ListItem Value="TIME_WORKED">Time Worked</asp:ListItem>
                                                                                <asp:ListItem Value="PRODUCTION">Production</asp:ListItem>
                                                                                <asp:ListItem Value="THROUGHPUT">Material Throughput</asp:ListItem>
                                                                            </items>
                                                                        </asp:DropDownList>
                                                                    </asp:PlaceHolder>
                                                                     <asp:PlaceHolder ID="phSafeTableField" runat="server">
                                                                        <asp:Label id="lblSafeTableField" runat="server" Text="Safety Data Type: " CssClass="prompt"></asp:Label>
                                                                        <asp:DropDownList ID="ddlSafeTableField" runat="server">
                                                                            <items>
                                                                                <asp:ListItem Value="RECORDED_CASES">Recorded Cases</asp:ListItem>
                                                                                <asp:ListItem Value="TIME_LOST_CASES">Lost Tiime Cases</asp:ListItem>
                                                                                <asp:ListItem Value="TIME_LOST">Lost Tiime</asp:ListItem>
                                                                            </items>
                                                                        </asp:DropDownList>
                                                                    </asp:PlaceHolder>
                                                                    <asp:PlaceHolder ID="phOutputUOM" runat="server">
                                                                        <asp:Label id="lblOutputUOM" runat="server" Text="Unit Of Measure: " CssClass="prompt"></asp:Label>
                                                                        <asp:DropDownList ID="ddlOutputUOM" runat="server">
                                                                        </asp:DropDownList>
                                                                    </asp:PlaceHolder>
                                                                </td>
                                                            </tr>
                                                            <tr id="trMeasureEFMType" runat="server">
                                                            <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureEFMType" runat="server" text="Energy/Fuel Type" Enabled="false"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:DropDownList ID="ddlMeasureEFMType" runat="server" ></asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureStatus" runat="server" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:DropDownList ID="ddlMeasureStatus" runat="server"></asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <span style="float: right; margin: 5px;">
                                                            <asp:Button ID="btnMeasureCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClick="ClearMeasure"  ></asp:Button>
                                                            <asp:Button ID="btnMeasureSave" CSSclass="buttonEmphasis" runat="server" text="Save Metric" style="margin: 5px;"
                                                                onclick="btnMeasureSave_Click" ></asp:Button>
                                                        </span>
                                                        <br />
                                                        <center>
                                                            <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
                                                        </center>
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </telerik:RadWindow>

                                            <asp:Panel ID="pnlMeasureList" runat="server" Visible = "false" CssClass="admBkgd">
                                                <table width="99.5%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
                                                    <tr>
                                                        <td class="optionArea" colspan="2">
                                                            <asp:Label ID="lblMeasureCategory" runat="server" Text="List By Metric Category: " CssClass="prompt"></asp:Label>
                                                            <asp:DropDownList ID="ddlMeasureCategory" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="ddlMeasureCategoryChanged" ToolTip="Select a specific category to enable adding metrics"></asp:DropDownList>
                                                            &nbsp;&nbsp;
                                                            <asp:LinkButton ID="lnkMeasureNew" runat="server" Text="Add Metric Of This Category" CSSClass="buttonAdd" ToolTip="Add a new metric" OnClick="AddMeasure"></asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" align="center">
                                                            <div id="divMeasureGVScroll" runat="server" class="">
                                                                <asp:GridView runat="server" ID="gvMeasureSubcatList" Name="gvMeasureSubcatList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" ShowHeaderWhenEmpty="True" cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvList_OnMeasureSubcatRowDataBound">
                                                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                                                    <RowStyle CssClass="DataCell" />
                	                                                <Columns>
                                                                        <asp:TemplateField ItemStyle-Width="17%">
                                                                            <HeaderTemplate>
                                                                                <asp:LinkButton ID="lnkSubCatNew" runat="server" Text="Sub Category" ToolTip="Add a new metric sub category" CSSClass="prompt" Enabled="false" OnClientClick="AddSubCategory(); return false;"></asp:LinkButton>
                                                                            </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:HiddenField ID="hfMeasureSubcat" runat="server" Value='<%#Eval("MEASURE_CD") %>' />
                                                                                <asp:Label ID="lblMeasureSubcat" runat="server" DataTextField="MEASURE_CD" Style="font-weight:bold;"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField ItemStyle-Width="83%">
                                                                            <HeaderTemplate>
                                                                               <asp:Label ID="lblMeasureHdr" runat="server" text="Metric"></asp:Label>
                                                                            </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:GridView runat="server" ID="gvMeasureList" Name="gvMeasureList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnMeasureRowDataBound">
                                                                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                                                                    <RowStyle CssClass="DataCell" />
                	                                                                <Columns>
                                                                                        <asp:TemplateField HeaderText="Code" ItemStyle-Width="12%">
							                                                                <ItemTemplate>
								                                                                <asp:LinkButton ID="lnkMeasureCD" runat="server" CommandArgument='<%#Eval("MEASURE_ID") %>' CSSClass="linkUnderline"
										                                                            Text='<%#Eval("MEASURE_CD") %>' OnClick="lnkMeasureList_Click"></asp:LinkButton>
                                                                                            </ItemTemplate>
							                                                            </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="Name" ItemStyle-Width="25%">
                                                                                            <ItemTemplate>
                                                                                                <asp:LinkButton ID="lnkMeasureName" runat="server" CommandArgument='<%#Eval("MEASURE_ID") %>' CSSClass="linkUnderline"
										                                                            Text='<%#Eval("MEASURE_NAME") %>' OnClick="lnkMeasureList_Click"></asp:LinkButton>
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:BoundField DataField="MEASURE_DESC" HeaderText="<%$ Resources:LocalizedText, Description %>" ItemStyle-Width="35%" />
                                                                                        <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Status %>" ItemStyle-Width="10%">
                                                                                            <ItemTemplate>
                                                                                                <asp:HiddenField ID="hfStatus_out" runat="server" Value='<%#Eval("STATUS") %>' />
                                                                                                <asp:Label ID="lblStatus_out" runat="server" DataTextField="STATUS" ></asp:Label>
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                     </Columns>
                                                                                </asp:GridView>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                                <asp:Label runat="server" ID="lblMeasureListEmpty" Text="There are currently no Metrics that meet your selection criteria." class="GridEmpty" Visible="false"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:panel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>

</asp:Content>


