<%@ Page Title="EHS - Reports" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="EHS_Reports.aspx.cs" Inherits="SQM.Website.EHS_Reports" %>

<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <br />
                    <table width="99%">
                        <tr>
                            <td class="pageTitles">
                                <asp:Label ID="lblViewEHSRezTitle" runat="server" Text="Reports"></asp:Label>
                                <br />
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="">Reporting charts</asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <div style="padding: 20px; width: 95%;">
                        <telerik:RadHtmlChart ID="RadHtmlChart1" runat="server">
                        </telerik:RadHtmlChart>
                        <br />
                        <br />
                        <br />
                        <telerik:RadHtmlChart ID="RadHtmlChart2" runat="server">
                        </telerik:RadHtmlChart>
                        <br />
                        <br />
                        <br />
                        <telerik:RadHtmlChart ID="RadHtmlChart3" runat="server">
                        </telerik:RadHtmlChart>
                        <br />
                        <br />
                        <br />
                        <telerik:RadHtmlChart ID="RadHtmlChart4" runat="server">
                        </telerik:RadHtmlChart>
                        <br />
                        <br />
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
