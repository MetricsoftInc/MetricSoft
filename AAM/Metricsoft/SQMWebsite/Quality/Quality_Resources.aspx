<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Quality_Resources.aspx.cs" Inherits="SQM.Website.Quality_Resources"%>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocMgr" TagPrefix="Ucl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<script type="text/javascript">

    function AddNonconf() {
        document.getElementById("pnlNonconfCatEdit").style.display = 'none';
        document.getElementById("pnlNonconfEdit").style.display = 'block';
        document.getElementById("tbNonconfCode").disabled = false;
        document.getElementById("tbNonconfName").disabled = false;
        document.getElementById("tbNonconfDesc").disabled = false;
        document.getElementById("ddlNonconfCategory").disabled = false;
        document.getElementById("ddlNonconfCategory").selectedIndex = 0;
        document.getElementById("ddlNonconfStatus").disabled = false;
        document.getElementById("ddlNonconfStatus").selectedIndex = 0;
        document.getElementById("btnNonconfCancel").disabled = false;
        document.getElementById("btnNonconfSave").disabled = false;

        $("#<%=pnlNonconfEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlNonconfEdit.ClientID%> textarea").val('');

        document.getElementById("hfOper").value = "add";
        document.getElementById("lblAddNonconf").style.display = 'block';

        document.getElementById("tbNonconfCode").focus();

        $('#btnNonconfSave').click(function () {
            return ValidNonconf();
        });
    }

    function AddCategory() {
        document.getElementById("pnlNonconfEdit").style.display = 'none';
        document.getElementById("lblAddNonconfCat").style.display = 'block';
        document.getElementById("pnlNonconfCatEdit").style.display = 'block';
        document.getElementById("pnlNonconfEdit").style.display = 'none';
        document.getElementById("tbNonconfCategoryCode").disabled = false;
        document.getElementById("tbNonconfCategoryName").disabled = false;
        $("#<%=pnlNonconfCatEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlNonconfCatEdit.ClientID%> textarea").val('');
        document.getElementById("btnNonconfCatCancel").disabled = false;
        document.getElementById("btnNonconfCatSave").disabled = false;

        document.getElementById("tbNonconfCategoryCode").focus();
    }

    function ValidCategory() {
        var status = true;
        if (document.getElementById("tbNonconfCategoryCode").value.length < 1 || document.getElementById("tbNonconfCategoryName").value.length < 1) {
            alert('Nonconformance category code and name are required');
            status = false;
        }
        return status;
    }

    function CancelCategory() {
        $("#<%=pnlNonconfCatEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlNonconfCatEdit.ClientID%> textarea").val('');
        document.getElementById("tbNonconfCategoryCode").disabled = true;
        document.getElementById("tbNonconfCategoryName").disabled = true;
        document.getElementById("btnNonconfCatCancel").disabled = true;
        document.getElementById("btnNonconfCatSave").disabled = true;
        document.getElementById("lblAddNonconfCat").style.display = 'none';
        //document.getElementById("pnlNonconfCatEdit").style.display = 'none';
        //document.getElementById("pnlNonconfEdit").style.display = 'block';
    }

    function ValidNonconf() {
        var status = true;
        if (status == true) {
            if ((status = confirmChange('Non-Conformance')) == true)
                document.getElementById("lblAddNonconf").style.display = 'none';
        }

        return status;
    }

    function CancelNonconf() {
        document.getElementById("tbNonconfCode").disabled = true;
        document.getElementById("tbNonconfName").disabled = true;
        document.getElementById("tbNonconfDesc").disabled = true;
        document.getElementById("ddlNonconfCategory").disabled = true;
        document.getElementById("ddlNonconfCategory").selectedIndex = 0;
        document.getElementById("ddlNonconfStatus").disabled = true;
        document.getElementById("ddlNonconfStatus").selectedIndex = 0;
        document.getElementById("btnNonconfCancel").disabled = true;
        document.getElementById("btnNonconfSave").disabled = true;

        $("#<%=pnlNonconfEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlNonconfEdit.ClientID%> textarea").val('');

        document.getElementById("hfOper").value = "";
        document.getElementById("lblAddNonconf").style.display = 'none';
       // $('#btnNonconfCancel').unbind('click');
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
                         <asp:HiddenField ID="hfActiveTab" runat="server" />
                        <asp:HiddenField ID = "hfUOMCategory_out" runat="server"></asp:HiddenField>
                        <asp:HiddenField ID = "hfMeasureUOM_out" runat="server"></asp:HiddenField>
                        <table width="99%">
			                <tr>
                                <td class="pageTitles">
                                    <asp:Label ID="lblViewQualityRezTitle" runat="server"  Text="Quality System Library" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Manage Quality system resources and reference information."></asp:Label>
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
                                            <asp:Panel ID="pnlNonconfList" runat="server" Visible = "false" CssClass="admBkgd">
                                                <table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
                                                    <tr>
                                                        <td class="optionArea">
                                                            <asp:Label ID="lblCategory" runat="server" Text="Problem Area: " CssClass="prompt"></asp:Label>
                                                            <asp:DropDownList ID="ddlProblemArea" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="SelectProblemArea"></asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnNonconfNew" runat="server" Text="Add Nonconformance" ToolTip="" CSSclass="buttonStd" style="margin: 6px;" OnClientClick="AddNonconf(); return false;" ></asp:Button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" align="center" width="65%">
                                                            <div id="divNonconfGVScroll" runat="server" class="">
                                                                <asp:GridView runat="server" ID="gvNonconfCatList" Name="gvNonconfCatList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" ShowHeaderWhenEmpty="True" cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvList_OnNonconfCatRowDataBound">
                                                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                                                    <RowStyle CssClass="DataCell" />
                	                                                <Columns>
                                                                        <asp:TemplateField ItemStyle-Width="17%">
                                                                            <HeaderTemplate>
                                                                                <asp:LinkButton ID="lnkCategoryNew" runat="server" Text="Category" CSSClass="linkUnderline" ToolTip="Add a new category" OnClientClick="AddCategory(); return false;"></asp:LinkButton>
                                                                            </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:HiddenField ID="hfNonconfCat" runat="server" Value='<%#Eval("NONCONF_CD") %>' />
                                                                                <asp:Label ID="lblNonconfcat" runat="server" DataTextField="NONCONF_CD" Style="font-weight:bold;"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField ItemStyle-Width="83%">
                                                                            <HeaderTemplate>
                                                                                <asp:LinkButton ID="lnkNonconfNew" runat="server" Text="Non-Conformance" CSSClass="linkUnderline" ToolTip="Add a new non-conformance"  OnClientClick="AddNonconf(); return false;"></asp:LinkButton>
                                                                            </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:GridView runat="server" ID="gvNonconfList" Name="gvNonconfList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnNonconfRowDataBound">
                                                                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                                                                    <RowStyle CssClass="DataCell" />
                	                                                                <Columns>
                                                                                        <asp:TemplateField HeaderText="Code" ItemStyle-Width="20%">
							                                                                <ItemTemplate>
								                                                                <asp:LinkButton ID="lnkNonconfCD" runat="server" CommandArgument='<%#Eval("NONCONF_ID") %>' CSSClass="linkUnderline"
										                                                            Text='<%#Eval("NONCONF_CD") %>' OnClick="lnkNonconfList_Click"></asp:LinkButton>
                                                                                            </ItemTemplate>
							                                                            </asp:TemplateField>
                                                                                        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%">
                                                                                            <ItemTemplate>
                                                                                                <asp:LinkButton ID="lnkNonconfName" runat="server" CommandArgument='<%#Eval("NONCONF_ID") %>' CSSClass="linkUnderline"
										                                                            Text='<%#Eval("NONCONF_NAME") %>' OnClick="lnkNonconfList_Click"></asp:LinkButton>
                                                                                            </ItemTemplate>
                                                                                        </asp:TemplateField>
                                                                                        <asp:BoundField DataField="NONCONF_DESC" HeaderText="<%$ Resources:LocalizedText, Description %>" ItemStyle-Width="40%" />
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
                                                                <asp:Label runat="server" ID="lblNonconfListEmpty" Text="There are currently no Non-Conformances that meet your selection criteria." class="GridEmpty" Visible="false"></asp:Label>
                                                            </div>
                                                        </td>
                                                        <td valign="top">
                                                            <asp:Panel id="pnlNonconfCatEdit" runat="server" style="display: none;">
                                                                <table width="99%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
                                                                    <tr>
                                                                        <td class="columnHeader" width="24%">
                                                                            <asp:Label ID="lblNonconfCatCode" runat="server" text="Category Code"></asp:Label>
                                                                        </td>
                                                                        <td class="required" width="1%">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt" width="75%">
                                                                            <asp:TextBox ID="tbNonconfCategoryCode"  runat="server" MaxLength="20" Columns="20"/>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="columnHeader">
                                                                            <asp:Label ID="lblNonconfCatName" runat="server" text="Name"></asp:Label>
                                                                        </td>
                                                                        <td class="required">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt">
                                                                            <asp:TextBox ID="tbNonconfCategoryName"  runat="server" TextMode="multiline" rows="2" MaxLength="100" CssClass="textStd" style="width: 97%;"/>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:Button ID="btnNonconfCatCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClientClick="CancelCategory(); return false;" ></asp:Button>
                                                                <asp:Button ID="btnNonconfCatSave" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin: 5px;"
                                                                    OnClientClick="return ValidCategory();" onclick="btnNonconfCatSave_Click" ></asp:Button>
                                                            </asp:Panel>
                                                            <asp:Panel id="pnlNonconfEdit" runat="server">
                                                                <table width="99%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
                                                                    <tr>
                                                                        <td class="columnHeader" width="24%">
                                                                            <asp:Label ID="lblNonconfCode" runat="server" text="Code"></asp:Label>
                                                                        </td>
                                                                        <td class="required" width="1%">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt" width="75%">
                                                                            <asp:TextBox ID="tbNonconfCode"  runat="server" MaxLength="100" Columns="20"/>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="columnHeader">
                                                                            <asp:Label ID="lblNonconfName" runat="server" text="Name"></asp:Label>
                                                                        </td>
                                                                        <td class="required">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt">
                                                                            <asp:TextBox ID="tbNonconfName"  runat="server" TextMode="multiline" rows="2" MaxLength="100" CssClass="textStd" style="width: 97%;"/>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="columnHeader">
                                                                            <asp:Label ID="lblNonconfDesc" runat="server" text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
                                                                        </td>
                                                                        <td class="tableDataAlt">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt">
                                                                            <asp:TextBox ID="tbNonconfDesc"  runat="server" TextMode="multiline" rows="3" MaxLength="200" CssClass="textStd" style="width: 97%;"/>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="columnHeader">
                                                                            <asp:Label ID="lblNonconfCategory" runat="server" text="Category"></asp:Label>
                                                                        </td>
                                                                        <td class="required">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt">
                                                                            <asp:DropDownList ID="ddlNonconfCategory" runat="server"></asp:DropDownList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="columnHeader">
                                                                            <asp:Label ID="lblNonconfStatus" runat="server" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                                                        </td>
                                                                        <td class="tableDataAlt">&nbsp;</td>
                                                                        <td CLASS="tableDataAlt">
                                                                            <asp:DropDownList ID="ddlNonconfStatus" runat="server"></asp:DropDownList>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:Button ID="btnNonconfCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" OnClientClick="CancelNonconf(); return false;" ></asp:Button>
                                                                <asp:Button ID="btnNonconfSave" CSSclass="buttonEmphasis" runat="server" text="Save Non-Conformance" style="margin: 5px;"
                                                                    OnClientClick="return ValidNonconf();" onclick="btnNonconfSave_Click" ></asp:Button>
                                                            </asp:Panel>
                                                            <asp:Label ID="lblAddNonconf" runat="server" CssClass="instructText" Text="Enter non-conformance details" style="display:none;"></asp:Label>
                                                            <asp:Label ID="lblAddNonconfCat" runat="server" Text="Add new non-conformance category" CssClass="instructText" style="display:none;"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
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


