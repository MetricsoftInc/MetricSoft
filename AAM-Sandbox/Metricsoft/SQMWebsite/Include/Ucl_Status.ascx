<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_Status.ascx.cs" Inherits="SQM.Website.Ucl_Status" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Panel ID="pnlStatusComplete" runat="server" Visible = "false">
    <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
        <tr>
            <td id="trStatusInfo" runat="server" class="columnHeader" width="14%">
                <asp:Label ID="lblStatusComplete" runat="server" text="Completed"></asp:Label>
               <%-- <asp:Panel ID="pnlStatusInfo" runat="server"></asp:Panel>--%>
            </td>
            <td CLASS="tableDataAlt" width="86%">
                <asp:CheckBox  ID="cbStatusComplete" runat="server" Text="" CssClass ="prompt WarnIfChanged"/>
            </td>
        </tr>
    </table>
</asp:Panel>