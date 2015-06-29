<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_NC.ascx.cs" Inherits="SQM.Website.Ucl_NC" EnableViewState="True" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


<asp:Panel ID="pnlNC" runat="server">
      <telerik:RadAjaxPanel ID="rax1" runat="server">
                                   
        <table width="99.5%" border="0" cellspacing="0" cellpadding="1" class="" style="margin-bottom: 3px;">
            <tr>
                <td colspan="3">
                    <telerik:RadComboBox ID="ddlProblemArea" runat="server" Skin="Metro" ZIndex=9000  width=400 AutoPostBack="true"  OnSelectedIndexChanged="SelectProblemArea" Font-Size=Small EmptyMessage="Select primary problem area"></telerik:RadComboBox>
                </td>
            </tr>
            <tr style="height: 2px;">
                <td colspan="3"></td>
            </tr>
            <tr class="HeadingCellTextLeft">
                <td width="30%"><asp:Label ID="lblNCCategory" runat="server" Text="Defect Category"></asp:Label></td>
                <td width="62%"><asp:Label ID="lblDefect" runat="server" Text="Non-Conformance"></asp:Label></td>
                <td width="8%"><asp:Label ID="lblCount" runat="server" Text="Count"></asp:Label></td>
            </tr>
            <tr>
                <td>
                     <telerik:RadComboBox ID="ddlNCCategory" runat="server" style="width: 99%;" Skin="Metro" ZIndex=9000  Font-Size=Small OnSelectedIndexChanged="SelectNCCategory" AutoPostBack="true" EmptyMessage="Select defect category"></telerik:RadComboBox>
                </td>
                <td>
                     <telerik:RadComboBox ID="ddlNC" runat="server" style="width: 99%;" Skin="Metro" ZIndex=9000 height="250" Font-Size=Small EmptyMessage="Select specific defect"></telerik:RadComboBox>
                </td>
                <td>
                    <asp:TextBox ID="tbCount" runat="server" size="4" maxlength="10" onblur="ValidNumeric(this, 'please enter numeric values only');"></asp:TextBox>
                </td>
            </tr>
        </table>
     </telerik:RadAjaxPanel>   
</asp:Panel>