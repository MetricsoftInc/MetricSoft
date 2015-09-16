<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PageTabs.ascx.cs" Inherits="SQM.Website.Ucl_PageTabs" %>
<asp:HiddenField ID="hfControl" runat="server" />
<asp:Panel ID="pnlPageTabs" runat="server" Visible="true">
    <table width="99%" border="0" cellspacing="0" cellpadding="0" id="Table3">
        <tr>
            <td class="navMenuTitles">
                <asp:Label ID="lblPageTabsTitle" runat="server" Text="" class="navMenuTitles"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab0" runat="server" class="optNav clickable" Width="70%" CommandArgument="0"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab0" visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab1" runat="server" class="optNav clickable" Width="70%" CommandArgument="1"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab1" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab2" runat="server" class="optNav clickable" Width="70%" CommandArgument="2"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab2" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab3" runat="server" class="optNav clickable" Width="70%" CommandArgument="3"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab3" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab4" runat="server" class="optNav clickable" Width="70%" CommandArgument="4"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab4" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab5" runat="server" class="optNav clickable" Width="70%" CommandArgument="5"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab5" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab6" runat="server" class="optNav clickable" Width="70%" CommandArgument="6"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab6" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab7" runat="server" class="optNav clickable" Width="70%" CommandArgument="7"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab7" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
        <tr>
            <td class="optMenu">
                <asp:LinkButton ID="lbTab8" runat="server" class="optNav clickable" Width="70%" CommandArgument="8"
                    OnClick="tab_Click" Text=""></asp:LinkButton>
                <asp:Image ID="imgTab8" Visible="false" runat="server" CssClass="optNavImg" />
            </td>
        </tr>
    </table>
</asp:Panel>
