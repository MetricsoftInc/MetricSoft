<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QualityIssueList.aspx.cs" Inherits="SQM.Website.QualityIssueList" %>
<%@ Register src="~/Include/Ucl_CaseList.ascx" TagName="IssueList" TagPrefix="Ucl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="../css/PSSQM.css" rel="stylesheet" type="text/css" />
<link href="../css/probSolver_default2.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<script src="/scripts/ps_admin.js" type="text/javascript"></script>
<script type="text/javascript">

    function init()
    { document.getElementById('SummaryDiv').style.display = 'none'; }

</script>

<body bgcolor="#FFFFFF" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
     <asp:HiddenField ID="hfBase" runat="server" />
    <div  id="SummaryDiv" class="error_popupdiv">
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
                                                                    <asp:Label ID="lblPartSearchTitle" runat="server" CssClass="popupTitles" Text="Quality Issues"></asp:Label>
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

                                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="darkBorder">
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPartString" runat="server" Text="<%$ Resources:LocalizedText, PartNumber %>"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartString" size="50" maxlength="100" runat="server"/></td>
			                        </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblPlantWhereUsed" runat="server" text="Plant Location"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"><asp:DropDownList ID="ddlPlantWhereUsed" runat="server"></asp:DropDownList></td>
                                    </TR>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblSupplierString" runat="server" text="Supplier"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbSupplierString" runat="server"></asp:TextBox></td>
                                    </TR>
                                </table>

                                <div ID="divIssueList" runat="server" Visible="false">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr style="height: 25px;">
                                            <td class="tableDataHdr" >
                                                <asp:Label runat="server" ID="lblIssueResultsHdr" Text="Results" CSSclass="tableDataHdr2"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <Ucl:IssueList id="uclIssueList" runat="server"/>
                                </div>
                                <!-- END OF PAGE CONTENT MAIN TABLE -->
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

