<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_ViewBuyer.aspx.cs" Inherits="SQM.Website.Administrate_ViewBuyer" %>
<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />

                    <table width="99%" border="0" cellspacing="1" cellpadding="0">
			            <tr>
			  	            <td>
					            <table width="100%">
						            <tr>
							            <td class="pageTitles">
                                            <asp:Label ID="lblViewBuyerTitle" runat="server" Text="Maintain Buyer Code"></asp:Label>
                                        </td>
							            <td align="right" >
								            <table border="0" cellspacing="0" cellpadding="1" >
						  			            <tr>
								                    <td>
                                                        <asp:Button ID="lbSearchBuyers1" CSSclass="buttonStd" runat="server" text="New Search"
                                                        onclick="lbSearchBuyers_Click" CommandArgument=""></asp:Button>
									                </td>
									                <td>
                                                        <asp:Button ID="lbBuyerEdit1" CSSclass="buttonStd" runat="server" text="Unassigned"
                                                        onclick="tab_Click" CommandArgument="assign"></asp:Button>
									                </td>
									                <td>
                                                        <asp:Button ID="lbBuyerAdd1" CSSclass="buttonStd" runat="server" text="Edit"
                                                        onclick="tab_Click" CommandArgument="edit"></asp:Button>
									                </td>
                                                    <td>
                                                        <asp:Button ID="lbSaveBuyerPerson1" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>"
                                                        onclick="tab_Click" CommandArgument="save"></asp:Button>
									                </td>
                                                </TR>
                                            </TABLE>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>

                    <table width="99%" border="0" cellspacing="1" cellpadding="4" class="darkBorder">
                        <tr style="height: 38px;">
                            <td class="tableDataHdr" colspan="5">
                                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
					                    <td class="tableDataHdr2">
                                            <asp:Label runat="server" ID="lblBuyerResultsHdr" Text="Results" Visible="true"></asp:Label>
                                        </td>
                                        <td align="right" style="padding-right:20px;">
                                            <asp:Label ID="lblUser"  runat="server" class="prompt" Text="Assign User: "></asp:Label>
                                            <asp:DropDownList ID="ddlUsers" runat="server"></asp:DropDownList>
                                        </td>
	                                </tr>
                                </table>
                            </td>
                        </tr>
                        <table width="99%" border="0" cellspacing="0" cellpadding="0">
                            <!-- ----- begin Local Tab table ----- -->
                            <asp:Panel runat="server" ID="pnlBuyerpartList">
                                <Ucl:PartList id="uclPartList" runat="server"/>
                            <tr>
                                <td align="right">
						            <table border="0" cellspacing="0" cellpadding="1">
						                <tr>
							                <td>
                                                <asp:Button ID="lbSearchBuyers2" CSSclass="buttonStd" runat="server" text="New Search"
                                                    onclick="lbSearchBuyers_Click" CommandArgument=""></asp:Button>
								            </td>
								            <td>
                                                <asp:Button ID="lbBuyerEdit2" CSSclass="buttonStd" runat="server" text="Unassigned"
                                                    onclick="tab_Click" CommandArgument="assign"></asp:Button>
							                </td>
						                    <td>
                                                <asp:Button ID="lbBuyerAdd2" CSSclass="buttonStd" runat="server" text="Edit"
                                                 onclick="tab_Click" CommandArgument="edit"></asp:Button>
					                        </td>
                                            <td>
                                                <asp:Button ID="lbSaveBuyerPerson2" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>"
                                                onclick="tab_Click" CommandArgument="save"></asp:Button>
									        </td>
                                        </TR>
                                    </TABLE>
                                </td>
                            </tr>
                            </asp:Panel>
                        </table>

                    </table>

                    </form>
                </td>
            </tr>
        </table>

        <br>
    </div>
</asp:Content>
