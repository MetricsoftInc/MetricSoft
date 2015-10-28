<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Shared_PartSearch.aspx.cs" Inherits="SQM.Website.Shared.Shared_PartSearch" %>
<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="../css/PSSQM_Default.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>

<script type="text/javascript">
    function ShowModalDialog() {
        var x = $find("ModalExtnd1");
        Page_ClientValidate();
        if (!Page_IsValid)
            x.show();
    }

    function init()
    { document.getElementById('SummaryDiv').style.display = 'none'; }
</script>

<body bgcolor="#FFFFFF" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <ajaxToolkit:ModalPopupExtender ID="ModalExtnd1" runat="server" TargetControlID="lblHidden"
    PopupControlID="SummaryDiv" BackgroundCssClass="error_popupbg">
    </ajaxToolkit:ModalPopupExtender>

    <div  id="SummaryDiv" class="error_popupdiv">
<table width="100%" >
    <tr><td class="header">
         Please correct the following:
    </td></tr>
    <tr><td>
        <asp:ValidationSummary id="ValSum1" runat="server" >
        </asp:ValidationSummary>
    </td></tr>
    <tr><td align="center">
        <input type="button" value="<%$ Resources:LocalizedText, OK %>" onclick="$find('ModalExtnd1').hide();"/>
    </td></tr>
</table>
<asp:Label ID="lblHidden" runat="server" Text="hidden" CssClass="error_hidelbl">
</asp:Label>
</div>

    <div>
        <table width="100%" border="0" cellspacing="0" bgcolor="#CCCCCC">
            <tr>
                <!-- PAGE CONTENT CELL -->
                <td width="98%" valign="top" class="tabActiveTableBg">
                    <!-- PAGE CONTENT MAIN TABLE -->
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td valign="top">
                                <!-- PAGE TITLE BORDER TABLE -->
                                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="border">
                                    <tr>
                                        <td valign="top" width="100%">
                                            <!-- PAGE TITLE CONTENT TABLE -->
                                            <table width="100%" border="0" cellspacing="1" cellpadding="0">
                                                <tr style="height: 30px;">
                                                    <!--<td class="borderHeader">-->
                                                    <td width="100%" background="/images/QAIHeaderBg.gif" style="vertical-align: bottom; margin:0px; border:0px;">
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblPartSearchTitle" runat="server" CssClass="popupTitles" Text="Search Parts"></asp:Label>
                                                                </td>
                                                                <td valign="top" nowrap align="right">
                                                                    <asp:Button ID="btnClose" runat="server" class="buttonStd" Text="Close Window" OnClientClick="javascript:window.close();" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!-- END OF PAGE TITLE CONTENT TABLE -->
                                                    </td>
                                                </tr>
                                            </table>

                                            <!------------------------------------->
                                            <!-- END OF PAGE TITLE BORDER TABLE -->
                                        </td>
                                    </tr>
                                </table>

                                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartString" runat="server" text="Part Number or Name"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartString" size="50" maxlength="100" runat="server"/></td>
			                        </tr>
                                    <tr>
                                        <td class="columnHeader" width="29%">
                                            <asp:Label ID="lblPartProgram" runat="server" text="Part Program"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                        <td class="tableDataAlt" width="70%"><asp:DropDownList ID="ddlPartProgram" runat="server"></asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPlantWhereUsed" runat="server" text="Location Where Used"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantWhereUsed" runat="server"></asp:DropDownList></td>
                                    </TR>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartActive" runat="server" text="Active Only"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:CheckBox id="cbPartActive" runat="server" AutoPostBack="false"/></td>
			                        </tr>
                                </table>

                                <asp:Button ID="btnPartSearch" runat="server" Text="<%$ Resources:LocalizedText, Search %>" CSSclass="buttonStd" OnClick="btnSearchParts_Click" style="margin: 5px;"/>
                                <asp:Button ID="btnPartSearchReset" runat="server" text="Reset" CSSclass="buttonStd" OnClick="btnPartSearchReset_Click" style="margin:5px;"/>

                                <asp:Panel ID="pnlSearchList" runat="server" Visible="false">
                                    <Ucl:PartList id="uclSearchList" runat="server"/>
                                </asp:Panel>

                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>

