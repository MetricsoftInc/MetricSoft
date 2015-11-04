<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_ItemHdr.ascx.cs" Inherits="SQM.Website.Ucl_ItemHdr" %>

    <asp:Panel ID="pnlCompanyHdr" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
				<td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyNameHdr" Text="Company Name" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblCompanyName_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblUltDunsCodeHdr" Text="Company Code" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblUlDunsCode_out" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyStatus" Text="<%$ Resources:LocalizedText, Status %>" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblCompanyStatus_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyUpdatedDate" Text="Last Update Date" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblCompanyUpdatedDate_out" Text="" Visible="true"></asp:Label>
				</td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlCompanyData" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
				<td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyHdr" Text="Company Name" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
			        <asp:LinkButton runat="server" ID="lnkCompany" Text=""  CssClass="linkUnderline" Visible="true" OnClick="lnkCompany_Click"></asp:LinkButton>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyCodeHdr" Text="Company Code" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblCompanyCode" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyStatusHdr" Text="<%$ Resources:LocalizedText, Status %>" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblCompanyStatus2" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
	                <SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblCompanyUsers" Text="Users" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
                     <asp:Label runat="server" ID="lblManageUsers" Text="Manage Users " Visible="false"></asp:Label>
			        <asp:LinkButton runat="server" ID="lnkCompanyUsers" Text="Manage Users"  CssClass="buttonUsers" Visible="true" OnClick="lnkCompanyUsers_Click"></asp:LinkButton>
 		        </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlBusOrgHdr" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
				<td class=summaryData valign=top width="30%">
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblOrgNameHdr" Text="Organization Name" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblOrgName_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top width="20%">
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocCodeHdr" Text="Organization Code" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocCode_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblParentBUHdr" Text="Parent Business Organization" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblParentBU_out" Text="" Visible="true"></asp:Label>
				</td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlPlantHdr" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlantOrgNameHdr" Text="Organization Name" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlantOrgName_out" Text="" Visible="true"></asp:Label>
				</td>
				<td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlantNameHdr" Text="Location Name" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlantName_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocCodeHdrPlant" Text="<%$ Resources:LocalizedText, LocationCode %>" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocCodePlant_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocationType" Text="<%$ Resources:LocalizedText, LocationType %>" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocationType_out" Text="" Visible="true"></asp:Label>
				</td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlPartHdr" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
		    <tr>
			    <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPartNumHdr" Text="Part Number" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPartNum_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPartNumFullHdr" Text="Full Part Number" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPartNumFull_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPartNameHdr" Text="Part Description" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPartName_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPartProgramHdr" Text="Part Program" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPartProgram_out" Text="" Visible="true"></asp:Label>
				</td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlCtlPlanHdr" runat="server" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
				<td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlanNameHdr" Text="Plan Name" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlanName_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlanVersionHdr" Text="Version" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlanVersion_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top colspan="2">
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlanDescHdr" Text="<%$ Resources:LocalizedText, Description %>" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlanDesc_out" Text="" Visible="true"></asp:Label>
				</td>
            </tr>
            <tr>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlanTypeHdr" Text="Plan Type" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlanType_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlanRefHdr" Text="Process Routing" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlanRef_out" Text="" Visible="true"></asp:Label>
				</td>
				<td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblPlanResponsibleHdr" Text="Responsibility" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblPlanResponsible_out" Text="" Visible="true"></asp:Label>
                </td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblEffDateHdr" Text="<%$ Resources:LocalizedText, EffectiveDate %>" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblEffDate_out" Text="" Visible="true"></asp:Label>
				</td>
            </tr>
        </table>
    </asp:Panel>

