<%@ Page Title="" Language="C#" MasterPageFile="~/Quality.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Administrate_Resources.aspx.cs" Inherits="SQM.Website.Administrate_Resources"%>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<script type="text/javascript">

    function AddMeasure() {
        $("#<%=pnlMeasureEdit.ClientID%> input").attr("disabled", false);
        $("#<%=pnlMeasureEdit.ClientID%> select").attr("disabled", false);
        $("#<%=pnlMeasureEdit.ClientID%> textarea").attr("disabled", false);
        $("#<%=pnlMeasureEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlMeasureEdit.ClientID%> textarea").val('');
        document.getElementById("hfOper").value = "add";
        document.getElementById("lblAddMeasure").style.display = 'block';

        $('#btnMeasureCancel').unbind('click');
        $('#btnMeasureCancel').click(function () {
            CancelMeasure();
        });
        $('#btnMeasureSave').click(function () {
           return ValidMeasure();
        });
    }

    function ValidMeasure() {
        var status = true;
        if (document.getElementById("ddlMeasureUOM").selectedIndex < 1) {
            alert('Standard UOM is required');
            status = false;
        }
        if (status == true) {
            if ((status = confirmChange('Measure')) == true)
                document.getElementById("lblAddMeasure").style.display = 'none';
        }

        return status;
    }

    function CancelMeasure() {
        $("#<%=pnlMeasureEdit.ClientID%> input").attr("disabled", true);
        $("#<%=pnlMeasureEdit.ClientID%> select").attr("disabled", true);
        $("#<%=pnlMeasureEdit.ClientID%> textarea").attr("disabled", true);
        $("#<%=pnlMeasureEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlMeasureEdit.ClientID%> textarea").val('');
        document.getElementById("hfOper").value = "";
        document.getElementById("lblAddMeasure").style.display = 'none';
        $('#btnMeasureCancel').unbind('click');
        return false;
    }

</script>
   
     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:HiddenField ID="hfOper" runat="server" />

                    <table width="99%">
			            <tr>
                            <td align="left">
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Manage global resources."></asp:Label>
                                <asp:Label ID="lblPageText" runat="server" class="instructText"  Text="" ></asp:Label>
                                <asp:Label ID="lblViewUserTitle" runat="server" Text="Global Resources" Visible="False" ></asp:Label>
                            </td>
                        </tr>
                    </table>
                    
                    <div id="divPageBody" runat="server">
                        <table width="98%" border="0" cellspacing="0" cellpadding="0">
	                        <tr>
		                        <td>
                                    <Ucl:AdminTabs id="uclAdminTabs" runat="server"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="editArea">
                                    <asp:Panel ID="pnlMeasureList" runat="server" Visible = "false" CssClass="admBkgd">
                                        <table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
                                            <tr>
                                                <td class="optionArea">
                                                    <asp:Label ID="lblMeasureCategory" runat="server" Text="Measure Category:" CssClass="prompt"></asp:Label>
                                                    <asp:DropDownList ID="ddlMeasureCategory" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="SelectMeasureCategory"></asp:DropDownList>
                                                    <asp:Button ID="btnMeasureAdd" CSSclass="buttonStd" runat="server" text="Add Measure" style="float: right; margin-right: 15px;" 
                                                   OnClientClick="AddMeasure(); return false;"  CommandArgument="add"></asp:Button>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblAddMeasure" runat="server" CssClass="instructText" Text="Enter new measure properties" style="display:none;"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top" align="center" width="65%">
                                                    <div id="divMeasureGVScroll" runat="server" class="">
                                                        <asp:GridView runat="server" ID="gvMeasureList" Name="gvMeasureList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Vertical" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvList_OnMeasureRowDataBound">
                                                            <HeaderStyle CssClass="HeadingCellText" />    
                                                            <RowStyle CssClass="DataCell" />
                	                                        <Columns>
                                                                <asp:TemplateField HeaderText="Sub Category" ItemStyle-Width="17%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hfMeasureSubcat" runat="server" Value='<%#Eval("MEASURE_SUBCATEGORY") %>' />
                                                                        <asp:Label ID="lblMeasureSubcat" runat="server" DataTextField="MEASURE_SUBCATEGORY" Style="font-weight:bold;"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Code" ItemStyle-Width="12%">
							                                        <ItemTemplate>
                                                                        <asp:HiddenField id="hfCompanyID" runat="server"  value='<%#Eval("COMPANY_ID") %>'/>
								                                        <asp:LinkButton ID="lnkMeasureCD" runat="server" CommandArgument='<%#Eval("MEASURE_ID") %>' CSSClass="linkUnderline" 
										                                    Text='<%#Eval("MEASURE_CD") %>' OnClick="lnkMeasureList_Click"></asp:LinkButton>
                                                                    </ItemTemplate>
							                                    </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Name" ItemStyle-Width="21%">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkMeasureName" runat="server" CommandArgument='<%#Eval("MEASURE_ID") %>' CSSClass="linkUnderline" 
										                                    Text='<%#Eval("MEASURE_NAME") %>' OnClick="lnkMeasureList_Click"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="MEASURE_DESC" HeaderText="Description" ItemStyle-Width="30%" />
                                                                <asp:TemplateField HeaderText="Std UOM" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hfMeasureUOM" runat="server" Value='<%#Eval("STD_UOM") %>' />
                                                                        <asp:Label ID="lblMeasureUOM_out" runat="server" DataTextField="STD_UOM" ></asp:Label>
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
                                                        <asp:Label runat="server" ID="lblMeasureListEmpty" Text="There are currently no Measures that meet your selection criteria." class="GridEmpty" Visible="false"></asp:Label>
                                                    </div>
                                                </td>
                                                <td valign="top">
                                                    <asp:Panel id="pnlMeasureEdit" runat="server">
                                                        <table width="99%" align="center" border="0" cellspacing="1" cellpadding="1" class="darkBorder">
                                                            <tr>
                                                                <td class="columnHeader" width="24%">
                                                                    <asp:Label ID="lblMeasureCode" runat="server" text="Measure Code"></asp:Label>
                                                                </td>
                                                                <td class="required" width="1%">&nbsp;</td>
                                                                <td CLASS="tableDataAlt" width="75%">
                                                                    <asp:TextBox ID="tbMeasureCode"  runat="server" MaxLength="100" Columns="20"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureName" runat="server" text="Measure Name"></asp:Label>
                                                                </td>
                                                                <td class="required">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:TextBox ID="tbMeasureName"  runat="server" TextMode="multiline" rows="2" MaxLength="100" CssClass="textStd" style="width: 97%;"/>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureDesc" runat="server" text="Description"></asp:Label>
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
                                                                    &nbsp;
                                                                    <asp:LinkButton ID="lnkAddSubcat" runat="server" CSSClass="linkUnderline" Text="Add"></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureUOM" runat="server" text="Std UOM"></asp:Label>
                                                                </td>
                                                                <td class="required">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:Label ID="lblUOMType" runat="server" Text="Type:"></asp:Label>
                                                                    <asp:DropDownList ID="ddlUOMCategory" runat="server" onChange="filterDependentList('ddlUOMCategory','ddlMeasureUOM','hfUOMCat_out','hfMeasureUOM_out');"></asp:DropDownList>
                                                                    &nbsp;&nbsp;
                                                                    <asp:DropDownList ID="ddlMeasureUOM" runat="server" onChange="putSelectedValue('ddlMeasureUOM','hfMeasureUOM_out');"></asp:DropDownList>
                                                                </td>
                                                            </tr
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureCurrency" runat="server" text="Std Currency"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:DropDownList ID="ddlMeasureCurrency" runat="server"></asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label ID="lblMeasureStatus" runat="server" text="Status"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td CLASS="tableDataAlt">
                                                                    <asp:DropDownList ID="ddlMeasureStatus" runat="server"></asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:Button ID="btnMeasureCancel" CSSclass="buttonStd" runat="server" text="Cancel" style="margin: 5px;" ></asp:Button>
                                                        <asp:Button ID="btnMeasureSave" CSSclass="buttonEmphasis" runat="server" text="Update Measure" style="margin: 5px;" 
                                                            OnClientClick="return ValidMeasure();" onclick="btnMeasureSave_Click" ></asp:Button>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </div>
                    </form>
                </td>
            </tr>
        </table>
        <!--#include file="/Include/Inc_Reference_Data.aspx"-->
        <br>
    </div>

</asp:Content>

