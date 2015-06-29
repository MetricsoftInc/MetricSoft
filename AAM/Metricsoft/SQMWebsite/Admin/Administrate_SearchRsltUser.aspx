<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_SearchRsltUser.aspx.cs" Inherits="SQM.Website.Administrate_SearchRsltUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <asp:HiddenField ID="hfBase" runat="server" />
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
			            <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblUserSearchTitle" runat="server" Text="User - Search Results"></asp:Label>
                            </td>
					    </tr>
				    </table><BR/> 

<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="2" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="30%">
                                <b>New Search </b>
		                    </td>
                            <td class="tabActiveTableBg" width="70%">
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>&nbsp;
                                <asp:Button ID="lbSearchUser" runat="server" Text="Go" CSSclass="buttonStd" onclick="lbSearchUser_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                    <br>

       	            <table width="98%" cellpadding="3" cellspacing="1" border="0" class="darkBorder">
                        <TR> 
		                    <TD class="tableDataHdr" colspan="5">
			                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
				                    <tr>
					                    <td class="tableDataHdr2">
                                            <asp:Label runat="server" ID="lblUserResultsHdr" Text="Results" Visible="true"></asp:Label>
                                        </td>
	                                    <td align="right">
					                <!-- BUTTONS -->		
					                        <table cellpadding=0 cellspacing=2 border=0>
						                        <tr>
                                                    <TD>
                                                       <asp:Button ID="lbUserAdmin" runat="server" onClick="lbUserAdmin_Click" CSSclass="buttonStd" text="User Administration"></asp:Button>
                                                    </TD>
                                                </tr>
                                            </table>
               			                <!-- END OF BUTTONS -->
					                    </td>
				                    </tr>
			                    </table>
                            </TD>
                        </TR>

                        <table width="98%" border="0" cellspacing="0" cellpadding="0">
                    <!-- ----- begin Local Tab table ----- -->
                            <!--#include file="/Include/Inc_User_List.aspx"-->
                        </table>
                       
			        </table>
                </TD>
            </TR>
        </table>
        <br>
    </div>
</asp:Content>
