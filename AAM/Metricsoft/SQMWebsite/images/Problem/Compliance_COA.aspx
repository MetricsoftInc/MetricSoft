<%@ Page Title="" Language="C#" MasterPageFile="~/Problem.master" AutoEventWireup="true" CodeBehind="Compliance_COA.aspx.cs" Inherits="SQM.Website.Compliance_COA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
  <script type="text/javascript">
 
   </script>
   <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <table width="99%">
			                <tr>
                                <td class="pageTitles">
                                    <span>
                                        <asp:Label ID="lblCOATitle" runat="server" Text="Compliance Report"></asp:Label>
                                    </span>
                                </td>
                                <td align="right">
						            <table border="0" cellspacing="0" cellpadding="2">
						  	            <tr>
                                            <td>
                                                <asp:Button ID="btnCancel1" runat="server" OnClientClick="return confirmAction('Cancel without saving');" onClick="btnCancel_Click" CSSclass="buttonStd" text="Cancel"></asp:Button>
									        </td>
								            <td>
                                                <asp:Button ID="btnSave1" class="buttonEmphasis" runat="server"  OnClientClick="return confirmChange('Compliance form');" onclick="btnSave_Click" text="Save and Submit" CommandArgument=""></asp:Button>
									        </td>
                                        </TR>
                                    </TABLE>
                                </td>
                            </tr>
                        </table>

                        <table width="99%">
                            <tr height="28" class="tableDataHdr">
			                    <td class="tableDataHdr2" >
                                    <asp:Label ID="lblFormType" runat="server" Text="Report Type: " ></asp:Label>
                                    &nbsp;
                                    <asp:DropDownList ID="ddlFormType" runat="server"></asp:DropDownList>
                                    <span style="float: right;">
                                        Date From:
							            <asp:TextBox runat="server" ID="TextBox1" Text="" size="10" maxlength="20"></asp:TextBox>
                                        &nbsp;To:&nbsp;
                                        <asp:TextBox runat="server" ID="TextBox2" Text="" size="10" maxlength="20"></asp:TextBox>
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td class="summaryBkgd" valign="top" align="center">
                                    <table cellspacing=0 cellpadding=2 border=0 width="100%">
					                    <tr>
                                            <td class=summaryData valign=top>
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblParentBUHdr" Text="Business Organization" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <asp:Label runat="server" ID="lblParentBU_out" Text="" Visible="true"></asp:Label>
						                    </td>
			                                <td class=summaryData valign=top>
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblPlantHdr" Text="Plant/Location" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
                                                <asp:DropDownList ID="ddlPlant" runat="server"></asp:DropDownList>
						                    </td>
                                            <td class=summaryData valign=top colspan="2">
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblReceiverCompany" Text="Recipient Company" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <asp:TextBox runat="server" ID="tbReceiverCompany" Text="" size="30" maxlength="100"></asp:TextBox>
                                                <input type="button" onclick="PopupCenter('../Shared/Shared_PartSearch.aspx?', 'newPage', 900, 600);"
                                                    value="Search" class="buttonStd"></input>
						                    </td>
                                        </tr>
                                        <tr>
                                            <td class=summaryData valign=top >
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblPartNumber" Text="Part Number" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <asp:TextBox runat="server" ID="tbPartNumber" Text="" size="30" maxlength="100"></asp:TextBox>
                                                <input type="button" onclick="PopupCenter('../Shared/Shared_PartSearch.aspx?', 'newPage', 800, 600);"
                                                    value="Search" class="buttonStd"></input>
						                    </td>
                                            <td class=summaryData valign=top >
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblDateRange" Text="Manufacturing Process" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <asp:TextBox runat="server" ID="tbProcess" Text="" size="30" maxlength="100"></asp:TextBox>
						                    </td>
                                            <td class=summaryData valign=top >
                                                <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lbPONumber" Text="Order Number" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <asp:TextBox runat="server" ID="tbPONumber" Text="" size="30" maxlength="100"></asp:TextBox>
						                    </td>
                                            <td class=summaryData valign=top>
							                    <SPAN CLASS=summaryHeader>
                                                    <asp:Label runat="server" ID="lblCOAHistory" Text="" Visible="true"></asp:Label>
                                                </SPAN>
                                                <BR>
							                    <input type='button' id='lbCOAHistory' value="History..." title="Search for existing reports" class="buttonLink" onclick="PopupCenter('../Problem/QualityIssueList.aspx?', 'newPage', 800, 600);"></asp:LinkButton>
						                    </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <asp:Panel ID="pnlPartDetail" runat="server">
                                <tr>
                                    <td class="summaryBkgd" valign="top" align="center">
                                    <!--#include file="/Include/Inc_Part_Detail.aspx"-->
                                    </td>
                                </tr>
                            </asp:Panel>
                        </table>
                        <br />
                        <asp:Panel ID="pnlPartBOM" runat="server">
                            <table width="99%" border="0" cellspacing="0" cellpadding="2">
			                    <tr>
			                        <td class=admBkgd align=center>
                                        <table  width="100%" border="0" cellspacing="0" cellpadding="2">
                                            <tr>
                                                <td class="admBkgd">
                                                    <asp:Label ID="lblPartBOMMsg" runat="server" CssClass="prompt" Text="Component Part and Sub-assembly Index"></asp:Label>
                                                </td>
                                            </tr>
                                            <!--#include file="/Include/Inc_Partbom_List.aspx"-->
                                            <tr>
                                                <td class="admBkgd">
                                                    <asp:CheckBox  ID="cbApproval1" runat="server" Text="Approved" CssClass ="prompt"/>
                                                </td>
                                            </tr>
                                         </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <br />
                        <asp:Panel ID="pnlCtlResult" runat="server">
                            <table width="99%" border="0" cellspacing="0" cellpadding="2">
			                    <tr>
			                        <td class=admBkgd align=center>
                                        <table  width="100%" border="0" cellspacing="0" cellpadding="2">
                                            <tr>
                                                <td class="admBkgd">
                                                    <asp:Label ID="lblCtlResultMsg" runat="server" CssClass="prompt" Text="Characteristic Accountability, Verification and Compatibility Evaluation"></asp:Label>
                                                </td>
                                            </tr>
                                            <!--#include file="/Include/Inc_Ctlplan_COA.aspx"-->
                                            <tr>
                                                <td class="admBkgd">
                                                    <asp:CheckBox  ID="cbApproval2" runat="server" Text="Approved" CssClass ="prompt"/>
                                                </td>
                                            </tr>
                                         </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                    </FORM>
                </td>
            </tr>
        </table>
        <br />
    </div>
</asp:Content>
