<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_ViewBusOrg.aspx.cs" Inherits="SQM.Website.Administrate_ViewBusOrg" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminList.ascx" TagName="AdminList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminEdit.ascx" TagName="AdminEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_NotifyList.ascx" TagName="NotifyList" TagPrefix="Ucl" %>

<%@ Register src="~/Include/Ucl_BusinessLoc.ascx" TagName="BusLoc" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
 
     <div class="admin_tabs">
        <asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:Panel runat="server" ID="pnlSearchBar" style="margin-right: 20px;">
                        <Ucl:SearchBar id="uclSearchBar" runat="server"/>
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
                                <%-- <br />--%>
                                <asp:Panel ID="pnlSearchList" runat="server" Visible="false">
                                    <Ucl:BusLoc id="uclBusLoc" runat="server"/>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                   <%-- <br />--%>
                    
                    <div id="divPageBody" runat="server">
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td valign="top" align="center">
                                    <Ucl:ItemHdr id="uclItemHdr" runat="server"/>
                                </td>
                            </tr>
                            <tr><td>&nbsp;</td></tr>
                            </table>

                            <div id="divNavArea" runat="server"  class="navAreaLeft">
                                <Ucl:AdminTabs id="uclAdminTabs" runat="server"/>
                            </div>

                            <div id="divWorkArea" runat="server" class="workAreaRight">
                                <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                    <asp:Panel runat="server" ID="pnlBusOrgEdit" style="float:left;">
                                         <tr>
                                            <td class="editArea">
                                                 <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                                    <TR>
                                                        <TD>
                                                            <table border="0" cellspacing="0" cellpadding="1">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Button ID="lbCancelBusOrg1" CSSclass="buttonStd" runat="server" text="Cancel" style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;"
                                                                         OnClientClick="return confirmAction('Cancel without saving');"  onclick="lbCancel_Click"></asp:Button>
                                                                    </TD>  
                                                                    <td>
                                                                        <asp:Button ID="lbSaveBusOrg1" CSSclass="buttonEmphasis" runat="server" text="Save" style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;"
                                                                         OnClientClick="return confirmChange('Business Organization');"  onclick="lbSave_Click"></asp:Button>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </TD>
                                                    </TR> 
                                                </table>
                                               </td>
                                              </tr>

                                              <tr>
                                                <td  class="editArea">
                                                <table width="99%" align="left" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                                    <tr>
                                                        <td class="columnHeader" width="39%">
                                                            <asp:Label ID="lblBusorgName" runat="server" text="Organization Name"></asp:Label>
                                                        </td>
                                                        <td class="required" width="1%">&nbsp;</td>
                                                        <td CLASS="tableDataAlt" width="60%">
                                                            <asp:TextBox ID="tbOrgName" size="50" maxlength="255" runat="server"/><asp:RequiredFieldValidator
                                                                ID="RequiredFieldValidator1" ControlToValidate="tbOrgName" runat="server" ErrorMessage="Please enter a Business Organization Name." Display="None"></asp:RequiredFieldValidator></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgLocCode" runat="server" text="Organization Code"></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">&nbsp;</td>
                                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbOrgLocCode" size="30" maxlength="20" runat="server"/></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgCurrency" runat="server" text="Default Currency"></asp:Label>
                                                        </td>
                                                        <td class="required">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <asp:DropDownList ID="ddlCurrencyCodes" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgParentOrg" runat="server" text="Parent Business Organization"></asp:Label>
                                                        </td>
                                                        <td class="required">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <asp:DropDownList ID="ddlParentBusOrg" runat="server"></asp:DropDownList>
                                                        </td>
                                                     </td>
                                                    </TR>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblSetBusorgStatus" runat="server" text="Status"></asp:Label>
                                                        </td>
                                                        <td class="required">&nbsp;</td>
                                                        <td class="tableDataAlt"><asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </TR>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgUpdatedBy" runat="server" text="Updated By"></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">&nbsp;</td>
                                                        <td CLASS="tableData"><asp:Label ID="lblLastUpdate_out" Text="" runat="server"/></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblBusorgUpdateDate" runat="server" text="Last Update Date"></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">&nbsp;</td>
                                                        <td CLASS="tableData"><asp:Label ID="lblLastUpdateDate_out" Text="" runat="server"/></td>
                                                    </tr>
                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CSSclass="promptAlert" ShowMessageBox="True" />
                                                </table>
                                            </td>
                                        </tr>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlSubLists">
                                        <Ucl:AdminList id="uclSubLists" runat="server"/>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlAdminEdit"> 
                                        <Ucl:AdminEdit id="uclAdminEdit" runat="server"/>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlPartProgram">
                                        <Ucl:PartList ID="uclProgramList" runat="server" />
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlEscalation">
                                        <Ucl:NotifyList id="uclNotifyList" runat="server"/>
                                    </asp:Panel>
                                </div>
                            </table>
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

