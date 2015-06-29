<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_EditBusOrg.aspx.cs" Inherits="SQM.Website.Administrate_EditBusOrg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
 
    <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />

                    <table width="98%" border="0" cellspacing="1" cellpadding="0" class="darkBorder">
			            <tr>
			  	            <td class="tableDataHdr">
					            <table width="100%">
						            <tr>
							            <td class="tableDataHdr2">
                                            <asp:Label ID="lblEditBusOrgTitle" runat="server" Text="Edit Business Organization"></asp:Label>
                                        </td>
							            <td align="right">
								            <table border="0" cellspacing="0" cellpadding="2">
						  			            <tr>
								                    <td>
                                                        <asp:Button ID="lbBusOrgAdmin" runat="server" onClick="lbBusOrgAdmin_Click" CSSclass="buttonStd" text="Business Organization Admin"></asp:Button>
									                </td>
									                <td>
                                                        <asp:Button ID="lbBusOrgSearch" runat="server" onClick="lbBusOrgSearch_Click" CSSclass="buttonStd" text="Search Business Organization"></asp:Button>
									                </td>
                                                </TR>
                                            </TABLE>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <asp:Panel ID="pnlBusOrgDetail" runat="server">
                                <td class="summaryBkgd" valign="top" align="center">
                                    <!--Border table of record data-->
                                    <!--#include file="/Include/Inc_BusOrg_Detail.aspx"-->
                                </td>
                            </asp:Panel>
                        </tr>
                    </table>
                    <br />
                    <table width="98%" border="0" cellspacing="0" cellpadding="0">
                     <!--#include file="/Include/Inc_BusOrg_Edit.aspx"-->
                     </table>

                    <br>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>
</asp:Content>

