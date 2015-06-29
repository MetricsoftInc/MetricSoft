<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_ViewPlant.aspx.cs" Inherits="SQM.Website.Administrate_ViewPlant" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminList.ascx" TagName="AdminList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminEdit.ascx" TagName="AdminEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_NotifyList.ascx" TagName="NotifyList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocMgr" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_B2BList.ascx" TagName="B2BList" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

     <div class="admin_tabs">
        <asp:HiddenField id="hfErrRequiredInputs" runat="server" Value="Please enter all required (*) fields before saving."/>
        <asp:HiddenField id="hfErrSaveError" runat="server" Value="An error occured while attempting to save this record. Please check your entries and try again."/>
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:Panel runat="server" ID="pnlSearchBar" style="margin-right: 20px">
                        <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                    </asp:Panel>

                    <table width="99%" border="0" cellspacing="1" cellpadding="0">
                        <tr>
                            <td align="left" height="22px;">
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Manage business locations. Define site-specific settings and preferences referenced by the quality and EHS applications when collecting and reporting plant information."></asp:Label>
                                <asp:Label ID="lblViewPlantTitle" runat="server" Text="Business Location" Visible="false"></asp:Label>
                                <asp:Label ID="lblViewBusOrgText" runat="server" Text="Return to Organization List" Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" align="center">
                                 <Ucl:ItemHdr id="uclItemHdr" runat="server"/>
                            </td>
                        </tr>
                    </table>
                    
                    <br />
                    <div id="divNavArea" runat="server"  class="navAreaLeft">
                        <Ucl:AdminTabs id="uclAdminTabs" runat="server"/>
                    </div>
                    <div id="divWorkArea" runat="server" class="workAreaRight">
                        <table width="99%" border="0" cellspacing="0" cellpadding="0">
                            <asp:Panel runat="server" ID="pnlPlantEdit">
                                 <tr>
                                    <td class="editArea">
                                         <table width="98%" border="0" cellspacing="0" cellpadding="0">
                                        <TR>
                                            <TD>
                                                <table border="0" cellspacing="0" cellpadding="1">
                                                    <tr>
                                                         <td>
                                                            <asp:Button ID="lbPlantCancel1" CSSclass="buttonStd" runat="server" text="Cancel"  style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;" 
                                                             onclick="lbPlantSave_Click" CommandArgument="cancel"></asp:Button>
                                                        </TD>  
                                                        <td>
                                                            <asp:Button ID="lbSavePlant1" CSSclass="buttonEmphasis" runat="server" text="Save" style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;" 
                                                             OnClientClick="return confirmChange('Plant');" onclick="lbPlantSave_Click" CommandArgument="edit"></asp:Button>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </TD>
                                        </TR> 
                                    </table>
                                 </td>
                              </tr>
                            <tr>
                                 <td class="editArea">
                                    <table width="98%" align="left" border="0" cellspacing="0" cellpadding="1"  class="borderSoft" >
                                        <tr>
                                            <td class="columnHeader" width="34%">
                                                <asp:Label ID="lblPlantBusOrg" runat="server" text="Business Organization"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="65%"><asp:DropDownList ID="ddlParentBusOrg" runat="server"></asp:DropDownList></td>
                                        </TR>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantNameEdit" runat="server" text="Location Name"></asp:Label>
                                            </td>
                                            <td class="required" >&nbsp;</td>
                                            <td CLASS="tableDataAlt" ><asp:TextBox ID="tbPlantName" size="60" maxlength="100" runat="server"/></td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantDescEdit" runat="server" text="Location Description"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbPlantDesc" TextMode="multiline" rows="2" maxlength="100" CssClass="textStd" runat="server" style="width: 97%;"/></td>
                                        </tr>
                                         <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantLocCode" runat="server" text="Location Code"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbPlantLocCode" size="30" maxlength="20" runat="server"/></td>
			                            </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantStatus" runat="server" text="Status"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantStatus" runat="server"></asp:DropDownList></td>
                                        </TR>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblAddress1" runat="server" text="Street Address 1"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbAddress1" size="60" maxlength="150" runat="server"/></td>
			                            </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblAddress2" runat="server" text="Street Address 2"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbAddress2" size="60" maxlength="150" runat="server"/></td>
			                            </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblCity" runat="server" text="City"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableDataAlt">
                                                <asp:TextBox ID="tbCity" size="60" maxlength="80" runat="server"/>
                                            </td>
			                            </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblState" runat="server" text="State Or Province / Postal Code"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableDataAlt">
                                                <span>
                                                    <asp:TextBox ID="tbState" size="42" maxlength="80" runat="server"/>
                                                    &nbsp;
                                                    <asp:TextBox ID="tbPostal" size="12" maxlength="50" runat="server"/>
                                                </span>
                                            </td>
			                            </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantCOCode" runat="server" text="Country Location"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlCountryCode" runat="server"></asp:DropDownList></td>
                                        </tr>
                                       <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantTimezone" runat="server" text="Local Time Zone"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantTimezone" runat="server"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPowerSourcedRegion" runat="server" text="Power Sourced Region"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPowerSourcedRegion" runat="server"></asp:DropDownList></td>
                                        </tr>
                                        <%--<tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantLanguage" runat="server" text="Preferred Language"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:DropDownList ID="ddlPlantLanguage" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantCurrencyCode" runat="server" text="Default Currency"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantCurrencyCodes" runat="server"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                    <asp:Label ID="lblLocationType" runat="server" text="Location Type"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlLocationType" runat="server"></asp:DropDownList></td>
                                        </TR>
                                        <tr>
                                            <td class="columnHeader">
                                                    <asp:Label ID="Label1" runat="server" text="Record Energy and Waste Data"></asp:Label>
                                                </td>
                                                <td class="tableDataAlt">&nbsp;</td>
                                                <td class="tableDataAlt">
                                                    <asp:CheckBox id="cbTrackEWData" runat="server" Checked="true"/>
                                                </td>
                                        </TR>
                                        <tr>
                                            <td class="columnHeader">
                                                    <asp:Label ID="lblTrackFinData" runat="server" text="Record Financial Data"></asp:Label>
                                                </td>
                                                <td class="tableDataAlt">&nbsp;</td>
                                                <td class="tableDataAlt">
                                                    <asp:CheckBox id="cbTrackFinData" runat="server" Checked="true"/>
                                                </td>
                                        </TR>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblPlantUpdatedBy" runat="server" text="Updated By"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td CLASS="tableData">
                                                <asp:Label ID="lblPlantLastUpdate" Text="" runat="server"/>
                                                &nbsp;&nbsp;
                                                <asp:Label ID="lblPlantLastUpdateDate" Text="" runat="server"/>
                                            </td>
                                        </tr>
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

                            <asp:Panel runat="server" ID="pnlEscalation">
                                 <Ucl:NotifyList id="uclNotifyList" runat="server"/>
                            </asp:Panel>

                            <asp:Panel runat="server" ID="pnlPlantDocs">
                                <Ucl:DocMgr id="uclDocMgr" runat="server"/>
                            </asp:Panel>

                            <asp:Panel ID="pnlB2B" runat="server" Visible="false">
                                <Ucl:B2BList id="uclCustList" runat="server"/>
                            </asp:Panel>

                            </table>
                        </div>
                    </form>
                </td>
            </tr>
        </table>

        <br>
    </div>

</asp:Content>
