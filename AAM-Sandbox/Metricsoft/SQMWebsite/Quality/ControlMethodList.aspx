<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlMethodList.aspx.cs" Inherits="SQM.Website.ControlMethodList" %>

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
                                                                    <asp:Label ID="lblQualityMethodTitle" runat="server" CssClass="popupTitles" Text="Control Methods (Standard)"></asp:Label>
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
                               
                                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="0" class="lightBorder">
                                    <tr>
                                        <td class="columnHeader" style="BACKGROUND-COLOR: #d4d0c8; FONT-WEIGHT: bold; COLOR:#333333;" width="49%">
                                            <asp:Label ID="lblControlChartHdr" runat="server" text="Process Control Charts"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt"  style="BACKGROUND-COLOR: #d4d0c8;">&nbsp;</td>
                                        <td CLASS="tableDataAlt" style="BACKGROUND-COLOR: #d4d0c8;" FONT-WEIGHT: bold; COLOR:#333333;" width="50%">
                                            <asp:Label ID="Label1" runat="server" text="Select the default control charts used for Statistical Process Control"></asp:Label>
                                        </td>      
			                        </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblVarChart" runat="server" text="Variables Data Control Chart"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"><asp:DropDownList ID="ddlVarChart" runat="server"></asp:DropDownList></td>
                                    </TR>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblAttChart" runat="server" text="Attributes Data Control Chart"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td class="tableDataAlt"><asp:DropDownList ID="ddlAttChart" runat="server"></asp:DropDownList></td>
                                    </TR>
                                </table>
                                <br />
                                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="0" class="lightBorder">
                                    <tr>
                                        <td class="columnHeader" style="BACKGROUND-COLOR: #d4d0c8; FONT-WEIGHT: bold; COLOR:#333333;" width="49%">
                                            <asp:Label ID="lblTestHdr" runat="server" text="Process Control Rules"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt"  style="BACKGROUND-COLOR: #d4d0c8;">&nbsp;</td>
                                        <td CLASS="tableDataAlt" style="BACKGROUND-COLOR: #d4d0c8;" width="47%">
                                            <asp:Label ID="lblTestInputHdr" runat="server" text="Number of points (as graphed in a series) required to evaluate and trigger a violation of the rule"></asp:Label>
                                        </td>      
			                        </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label2" runat="server" text="Specification Violation"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt"><asp:CheckBox id="cbTest01" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest01" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label3" runat="server" text="Control Limit Violation (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest11" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest11" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label5" runat="server" text="Trend (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest13" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest13" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                     <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label6" runat="server" text="Shift (zone A) (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest14" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbTest14" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                                Of
                                                <asp:TextBox ID="tbTest14P" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label7" runat="server" text="Shift (zone B) (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest15" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbTest15" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                                Of
                                                <asp:TextBox ID="tbTest15P" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label8" runat="server" text="Shift (zone C) (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest16" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbTest16" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                                Of
                                                <asp:TextBox ID="tbTest16P" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label9" runat="server" text="Mixture (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest17" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest17" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label10" runat="server" text="Systematic Variable (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest18" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest18" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="Label11" runat="server" text="Stratification (mean)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest19" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest19" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr><td style="BACKGROUND-COLOR: #d4d0c8;">&nbsp</td><td style="BACKGROUND-COLOR: #d4d0c8;">&nbsp</td><td style="BACKGROUND-COLOR: #d4d0c8;">&nbsp</td></tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest22" runat="server" text="Control Limit Violation (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest21" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest21" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest23" runat="server" text="Trend (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest23" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest23" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest24" runat="server" text="Shift (zone A) (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest24" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbTest24" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                                Of
                                                <asp:TextBox ID="tbTest24P" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest25" runat="server" text="Shift (zone B) (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest25" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbTest25" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                                Of
                                                <asp:TextBox ID="tbTest25P" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest26" runat="server" text="Shift (zone C) (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest26" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt">
                                            <span>
                                                <asp:TextBox ID="tbTest26" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                                Of
                                                <asp:TextBox ID="tbTest26P" runat="server" MaxLength="2" size="2"></asp:TextBox>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest27" runat="server" text="Mixture (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest27" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest27" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest28" runat="server" text="Systematic Variable (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest28" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest28" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblTest29" runat="server" text="Stratification (variance)"></asp:Label>
                                        </td>
                                         <td class="tableDataAlt"><asp:CheckBox id="cbTest29" runat="server" AutoPostBack="false"/></td>
                                        <td class="tableDataAlt"><asp:TextBox ID="tbTest29" runat="server" MaxLength="2" size="2"></asp:TextBox></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div class="scrollArea" style="width: 430px; margin-left: 80px;">
        <asp:Image ID="Image1" ImageUrl="~/images/ControlTests.png" runat="server"/>
    </div>
    </form>
</body>
</html>

