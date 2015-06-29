<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_DefaultDocuments.aspx.cs" Inherits="SQM.Website.Admin.Administrate_DefaultDocuments" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocMgr" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <asp:HiddenField ID="hfBase" runat="server" />
    <div class="admin_tabs">
        <br />
        <table width="99%" border="0" cellspacing="0" cellpadding="0" align="center">
            <tr>
                <td class="pageTitles">
                    <asp:Label ID="lblDefaultDocumentTitle" runat="server" Text="Manage Documents"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" style="margin-top: 10px;" Text="Upload and manage documents used for company/user communications or reference standards."></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <Ucl:DocMgr id="uclDocMgr" runat="server"/>
                </td>
            </tr>
        </table>

    </div>
</asp:Content>

