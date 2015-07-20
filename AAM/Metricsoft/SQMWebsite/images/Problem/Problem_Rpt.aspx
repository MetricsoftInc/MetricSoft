<%@ Page Title="" Language="C#" MasterPageFile="~/ReportMaster.Master" AutoEventWireup="true" CodeBehind="Problem_Rpt.aspx.cs" Inherits="SQM.Website.Problem_Rpt" %>
<%@ Register src="~/Include/Ucl_CaseEdit.ascx" TagName="CaseRpt" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="CaseList" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <div>
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:Button ID="lnkProblemListReturn" runat="server" CssClass="buttonReturn" Text="Return To List" OnClick="lnkProblemListReturn_Click"></asp:Button>
                         <div id="divPageBody" runat="server">
                            <br />
                            <div id="divWorkArea" runat="server">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="editArea" width="25%">
                                            <asp:Image ID="imgLogo" runat="server" CssClass="logoImg"/>
                                        </td>
                                        <td class="editArea" width="85%">
                                            <span>
                                                <asp:Label ID="lblMainTitle" runat="server" CssClass="pageTitles" Text="8D Problem Analysis Report"></asp:Label>
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="lblCasePrefix" runat="server" CssClass="pageTitles" Text="Case ID:" Visible="true"></asp:Label>
                                                <asp:Label ID="lblCasePrefixEHS" runat="server" CssClass="pageTitles" Text="Incident:" Visible="false"></asp:Label>
                                                &nbsp;
                                                <asp:Label ID="lblCaseID" runat="server" CssClass="pageTitles"></asp:Label>
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <asp:Panel runat="server" ID="pnlCaseRpt">
                                                <Ucl:CaseRpt id="uclCaseRpt" runat="server"/>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </FORM>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>

