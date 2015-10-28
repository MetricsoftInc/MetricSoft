<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_BusinessLoc.ascx.cs" Inherits="SQM.Website.Ucl_BusinessLoc" %>
<%@ Register src="~/Include/Ucl_AdminList.ascx" TagName="AdminList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>

<asp:Panel ID="pnlBusinessLocEdit" runat="server" Visible="false">
    <asp:DropDownList ID="ddlSelectLocation" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSelectLocation_Change" Visible="false">
        <asp:ListItem Value="1">prime</asp:ListItem>
    </asp:DropDownList>
    <asp:Button id="btnCancel" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" Title="cancel selection" CssClass="buttonStd"  OnClick="btnListCompanies_Click" CommandArgument="done" Enabled="false" Visible="false"/>
    <asp:Button id="btnReset" runat="server" Text="Reset" Title="reset to default (HR location)" CssClass="buttonStd"  OnClick="btnListCompanies_Click" CommandArgument="reset" Visible="false"/>
    <br />
    <asp:Panel ID="pnlSelectCompany" runat="server" Visible="false" Width="100%">
        <table  width="100%">
            <tr>
                <td align="center">
                    <Ucl:ItemHdr id="uclItemHdr" runat="server"/>
                </td>
            </tr>
        </table>
        <Ucl:AdminList id="uclCompanyList" runat="server"/>
    </asp:Panel>
    <asp:Panel ID="pnlLocationText" runat="server" Visible="false">
        <asp:Label ID="lblSelBusOrg" CssClass="textStd" runat="server"></asp:Label>
        ,&nbsp;
        <asp:Label ID="lblSelPlant" CssClass="textStd" runat="server"></asp:Label>
    </asp:Panel>
</asp:Panel>

<asp:Panel ID="pnlBusinessLoc" runat="server" Visible="false" CssClass="tableDataAlt">
    <asp:Label ID="lblUserCompany" runat="server"></asp:Label>
    <br />
    <asp:Label ID="lblUserBusOrg" runat="server"></asp:Label>
    ,&nbsp;
    <asp:Label ID="lblUserPlant" runat="server"></asp:Label>
</asp:Panel>