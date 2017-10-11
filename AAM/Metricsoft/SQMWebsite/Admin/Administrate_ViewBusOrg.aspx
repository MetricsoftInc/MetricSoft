<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_ViewBusOrg.aspx.cs" Inherits="SQM.Website.Administrate_ViewBusOrg" %>

<%@ Register Src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AdminList.ascx" TagName="AdminList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AdminEdit.ascx" TagName="AdminEdit" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_NotifyList.ascx" TagName="NotifyList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_BusinessLoc.ascx" TagName="BusLoc" TagPrefix="Ucl" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <div class="admin_tabs">
        <asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications" />
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <br />
                    <form name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:Panel runat="server" ID="pnlSearchBar" Style="margin-right: 20px;">
                            <Ucl:SearchBar ID="uclSearchBar" runat="server" />
                        </asp:Panel>

                        <table width="100%" border="0" cellspacing="1" cellpadding="0">
                            <tr>
                                <td align="left" height="20px">
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="&nbsp;Manage the company business structure and resources referenced by the quality and EHS systems. Search for Business Units by name or location code."></asp:Label>
                                    <asp:Label ID="lblViewBusOrgText" runat="server" Text="Return to Organization List" Visible="false"></asp:Label>
                                    <asp:Label ID="lbViewBusStructTitle" runat="server" Text="Business Structure" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Panel ID="pnlSearchList" runat="server" Visible="false">
                                        <Ucl:BusLoc ID="uclBusLoc" runat="server" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>

                        <div id="divPageBody" runat="server">
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td valign="top" align="center">
                                        <Ucl:ItemHdr ID="uclItemHdr" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                            </table>

                            <div id="divNavArea" runat="server" class="navAreaLeft">
                                <Ucl:AdminTabs ID="uclAdminTabs" runat="server" />
                            </div>

                            <div id="divWorkArea" runat="server" class="workAreaRight">
                                <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                    <asp:Panel runat="server" ID="pnlBusOrgEdit" Style="float: left;">
                                        <tr>
                                            <td class="editArea">
                                                <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <table border="0" cellspacing="0" cellpadding="1">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Button ID="lbCancelBusOrg1" CssClass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" Style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;"
                                                                            OnClientClick="return confirmAction('Cancel without saving');" OnClick="lbCancel_Click"></asp:Button>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="lbSaveBusOrg1" CssClass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" Style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;"
                                                                            OnClientClick="return confirmChange('Business Organization');" OnClick="lbSave_Click"></asp:Button>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="editArea">
                                                <table width="99%" align="left" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                                    <tr>
                                                        <td class="columnHeader" width="39%">
                                                            <asp:Label ID="lblBusorgName" runat="server" Text="Organization Name"></asp:Label>
                                                        </td>
                                                        <td class="required" width="1%">&nbsp;</td>
                                                        <td class="tableDataAlt" width="60%">
                                                            <asp:TextBox ID="tbOrgName" size="50" MaxLength="255" runat="server" /><asp:RequiredFieldValidator
                                                                ID="RequiredFieldValidator1" ControlToValidate="tbOrgName" runat="server" ErrorMessage="Please enter a Business Organization Name." Display="None"></asp:RequiredFieldValidator></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgLocCode" runat="server" Text="Organization Code"></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <asp:TextBox ID="tbOrgLocCode" size="30" MaxLength="20" runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgCurrency" runat="server" Text="Default Currency"></asp:Label>
                                                        </td>
                                                        <td class="required">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <asp:DropDownList ID="ddlCurrencyCodes" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgParentOrg" runat="server" Text="Parent Business Organization"></asp:Label>
                                                        </td>
                                                        <td class="required">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <asp:DropDownList ID="ddlParentBusOrg" runat="server"></asp:DropDownList>
                                                        </td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblSetBusorgStatus" runat="server" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblBusorgUpdatedBy" runat="server" Text="Updated By"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableData">
                                                <asp:Label ID="lblLastUpdate_out" Text="" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblBusorgUpdateDate" runat="server" Text="Last Update Date"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableData">
                                                <asp:Label ID="lblLastUpdateDate_out" Text="" runat="server" /></td>
                                        </tr>
                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="promptAlert" ShowMessageBox="True" />
                                </table>
                </td>
            </tr>
        </asp:Panel>
    </div>
    </table>
	                        <asp:Panel runat="server" ID="pnlSubLists">
                                <Ucl:AdminList ID="uclSubLists" runat="server" />
                            </asp:Panel>

    <asp:Panel runat="server" ID="pnlAdminEdit">
        <Ucl:AdminEdit ID="uclAdminEdit" runat="server" />
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlPartProgram">
        <Ucl:PartList ID="uclProgramList" runat="server" />
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlEscalation">
        <Ucl:NotifyList ID="uclNotifyList" runat="server" />
    </asp:Panel>
   
    
    </div>
                         
                            
                         
                    </form>
                </td>
            </tr>
        </table>

        <br>
    </div>

    <asp:HiddenField ID="hfNonconf1" runat="server" />

    <script type="text/javascript">
        var contentID = 'ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder_Body_';

        function DisplayNonConf() {
            var hf1 = document.getElementById('hfNonconf1');
            var dvwk = document.getElementById('divNonconf');
            var inhtml = dvwk.innerHTML;
            inhtml = inhtml + hf1.value;
            dvwk.innerHTML = inhtml;
        }

        function ValidateNotifyList() {
            var grid = document.getElementById(contentID + 'gvNotifyList');
            /*alert(grid.rows.length);*/
            /*alert(grid.rows[1].cells.length);*/
            /*for (var n = 0; n < 6; n++) { */
            /*    var val = grid.rows[1].cells[n].firstChild.value; */
            /*    alert(val); */
            /* } */
        }
    </script>
</asp:Content>

