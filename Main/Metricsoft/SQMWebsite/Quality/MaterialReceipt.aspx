<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="MaterialReceipt.aspx.cs" Inherits="SQM.Website.MaterialReceipt"%>
<%@ Register src="~/Include/Ucl_StreamList.ascx" TagName="StreamList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
   <link type="text/css" href="../css/redmond/jquery-ui-1.8.20.custom.css" rel="Stylesheet" />	
    <script type="text/javascript" src="../scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#tbReceiptDate').datepicker({
                changeMonth: true,
                changeYear: true,
                showOn: "both",
                buttonImage: "/images/calendar.gif",
                buttonImageOnly: true,
                yearRange: "2000:2030",
                buttonText: "Select Material Receipt Date"
            });
            if ($('#tbReceiptDate').is(':disabled') == true) {
                $('#tbReceiptDate').datepicker().datepicker('disable');
            }
        });
    </script>
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
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
                                <td class="pageTitles">
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Record quality inspection and acceptance results."></asp:Label>
                                    <asp:Label ID="lblMaterialReceiptTitle" runat="server" Visible="false" Text="Inspection Recording" ></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <div id="divPageBody" runat="server">
                            <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td>
                                        <asp:Panel ID="pnlStreamSearch" runat="server">
                                            <asp:Label ID="lblStreamPart" runat="server" CssClass="prompt" Text="Part Number: "></asp:Label>
                                            <asp:TextBox ID="tbStreamPart" runat="server" MaxLength="50" Columns="24"></asp:TextBox>
                                            <asp:Label ID="lblStreamSupp" runat="server" CssClass="prompt" style="margin-left: 10px;" Text="Supplier Name: "></asp:Label>
                                            <asp:TextBox ID="tbStreamSupp" runat="server" MaxLength="50" Columns="24"></asp:TextBox>
                                            <asp:Button id="btnStreamSearch" runat="server" Text="Search" CssClass="buttonStd" style="margin-left: 10px;" OnClick="btnStreamSearch_Click"/>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Ucl:StreamList id="uclStreamHdr" runat="server"/>
                                        <Ucl:StreamList id="uclStreamList" runat="server"/>
                                    </td>
                                </tr>
                            </table>

                            <asp:Panel ID="pnlReceiptEdit" runat="server" Visible="false"  Width="99%" class="editArea">
                                <br />
                                <asp:Label ID="lblReceiptTitle" runat="server" Text="Current Receipt:" CssClass="prompt" style="float:left; margin-left: 5px;"></asp:Label>
                                <asp:Label ID="lblReceiptInstruction" runat="server" Text="Enter material inspection results, including number of rejected items and non-conformaces observed." CssClass="instructText" style="float:left; margin-left: 5px;"></asp:Label>
                                <table width="99%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
                                    <tr>
                                        <td class="columnHeader" width="29%">
                                            <asp:Label ID="lblReceiptDate" runat="server" text="Inspection Date"  ></asp:Label>
                                        </td>
                                        <td class="required" width="1%">&nbsp;</td>
                                        <td CLASS="tableDataAlt" width="70%">
                                            <asp:TextBox ID="tbReceiptDate"  runat="server" MaxLength="20" Columns="12" CausesValidation="True"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblLotNumber" runat="server" text="Lot Number"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt">
                                                <asp:TextBox ID="tbLotNumber"  runat="server" MaxLength="100" Columns="20"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblContainerNumber" runat="server" text="Container Number"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt">
                                                <asp:TextBox ID="tbContainerNumber"  runat="server" MaxLength="100" Columns="20"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblReceiptQty" runat="server" text="Qty Received"></asp:Label>
                                        </td>
                                        <td class="required">&nbsp;</td>
                                        <td CLASS="tableDataAlt">
                                                <asp:TextBox ID="tbReceiptQty"  runat="server" MaxLength="8" Columns="10"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblRejectQty" runat="server" text="Qty Rejected"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt">&nbsp;</td>
                                        <td CLASS="tableDataAlt">
                                            <asp:TextBox ID="tbRejectQty"  runat="server" MaxLength="8" Columns="10"/>
                                            &nbsp;&nbsp;
                                            <asp:Button id="btnCreateIssue" runat="server" CssClass = "buttonLink" Text="Create Quality Issue" 
                                                OnClientClick="return confirmAction('Create A Quality Issue For This Receipt');" OnClick="btnCreateIssue_Click"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </div>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>
</asp:Content>
