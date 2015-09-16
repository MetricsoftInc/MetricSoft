<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AdminTabs.ascx.cs" Inherits="SQM.Website.Ucl_AdminTabs" %>

    <asp:Panel ID="pnlCompanyTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" id="Table2">
            <tr align="center" height="24">
                <td id="tdCompanyDetail" runat="server" class="optMenu">
                    <asp:LinkButton ID="lbCompanyDetail_tab" runat="server" class="optNav clickable"  CommandArgument="edit" 
                            onclick="tab_Click">Details</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td id="tdCompanyStd" runat="server" class="optMenu">
                    <asp:LinkButton ID="lbUomStds_tab" runat="server" class="optNav clickable"  CommandArgument="stds" 
                            onclick="tab_Click">Standard Units</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td id="tdCompanyDocs" runat="server" class="optMenu">
                    <asp:LinkButton ID="lbCompanyDocs_tab" runat="server" class="optNav clickable"  CommandArgument="stds" 
                            onclick="tab_Click">Documents</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td id="tdCompanyTargets" runat="server" class="optMenu">
                        <asp:LinkButton ID="lbCompanyTargets_tab" runat="server" class="optNav clickable"  CommandArgument="dept" 
                            onclick="tab_Click">Metric Targets</asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlOrgTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" id="admin_tabs">
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbBusOrgDetail_tab" runat="server" class="optNav clickable"  CommandArgument="edit" 
                            onclick="tab_Click">Details</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                        <asp:LinkButton ID="lbDepartment_tab" runat="server" class="optNav clickable"  CommandArgument="dept" 
                            onclick="tab_Click">Departments</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbBUNotify_tab" runat="server" class="optNav clickable"  CommandArgument="notify" 
                    onclick="tab_Click">Notifications</asp:LinkButton>
                </td>
           </tr>
        </table>
    </asp:Panel>


    <asp:Panel ID="pnlPlantTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" id="Table1">
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                        <asp:LinkButton ID="lbPLantDetail_tab" runat="server" class="optNav clickable" CommandArgument="edit" 
                        onclick="tab_Click">Details</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
					<asp:LinkButton ID="lbPlantUsers_tab" runat="server" class="optNav clickable" CommandArgument="user" 
                        onclick="tab_Click">Users</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                        <asp:LinkButton ID="lbPLantDepartment_tab" runat="server" class="optNav clickable" CommandArgument="dept" 
                        onclick="tab_Click">Departments</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                        <asp:LinkButton ID="lbPlantLine_tab" runat="server" class="optNav clickable" CommandArgument="line" 
                            onclick="tab_Click">Lines / Operations</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td id="Td3" runat="server" class="optMenu">
                    <asp:LinkButton ID="lbPlantNotify_tab" runat="server" class="optNav clickable"  CommandArgument="notify" 
                    onclick="tab_Click">Notifications</asp:LinkButton>
                </td>
           </tr>
        </table>
    </asp:Panel>
 
    <asp:Panel ID="pnlPartTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr align="center" height="24">
                <td class="optMenu">
                        <asp:LinkButton ID="lbPartDetail_tab" runat="server" class="optNav clickable" CommandArgument="edit" 
                            onclick="tab_Click">Details</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td class="optMenu">
                    <asp:LinkButton ID="lbPartUsed_tab" runat="server" class="optNav clickable" CommandArgument="plant" 
                        onclick="tab_Click">Where Used</asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td class="optMenu">
                    <asp:LinkButton ID="lbPartCust_tab" runat="server" class="optNav clickable" CommandArgument="cust" 
                        onclick="tab_Click">Supplied To</asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>
    
    
    <asp:Panel ID="pnlUserTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" id="Table3">
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbUserContact_tab" runat="server" class="optNav clickable" CommandArgument="person" 
                    onclick="tab_Click"  Text="Contact Information"></asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbUserResponsible_tab" runat="server" class="optNav clickable" CommandArgument="responsible" 
                        onclick="tab_Click"  Text="System Access"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlQualityTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbQSDocs_tab" runat="server" class="optNav clickable" 
                        onclick="tab_Click"  Text="Documents"></asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbQSNonConf_tab" runat="server" class="optNav clickable" 
                            onclick="tab_Click" Text="Defect Codes"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlEHSTabs" runat="server" Visible = "false">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" id="Table5">
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbEHSDocs_tab" runat="server" class="optNav clickable" 
                        onclick="tab_Click"  Text="Documents"></asp:LinkButton>
                </td>
            </tr>
            <tr align="center" height="24">
                <td runat="server" class="optMenu">
                    <asp:LinkButton ID="lbEHSMeasure_tab" runat="server" class="optNav clickable" CommandArgument="measehs" 
                    onclick="tab_Click"  Text="Metric Codes"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>
