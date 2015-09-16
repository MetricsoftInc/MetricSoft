<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_SearchRsltPlant.aspx.cs" Inherits="SQM.Website.Administrate_SearchRsltPlant" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
 

    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <asp:HiddenField ID="hfBase" runat="server" />

			        <BR/>
<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
			            <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblPlantSearchTitle" runat="server" Text="Plant - Search Results"></asp:Label>
                            </td>
					    </tr>
				    </table>
                    <BR/> 

<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="2" class="darkBorder">
                        <tr>
                            <td class="columnHeader" width="30%">
                                <b>New Search </b>
		                    </td>
                            <td class="tabActiveTableBg" width="70%">
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>&nbsp;
                                <asp:Button ID="lbSearchPlant" runat="server" Text="Go" CSSclass="buttonStd" onclick="lbSearchPlant_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                    <br>

                    <table width="98%" border="0" cellspacing="0" cellpadding="3">
       	            <!--<table width="98%" cellpadding="3" cellspacing="1" border="0" class="darkBorder">-->
                        <TR> 
		                    <TD class="tableDataHdr" colspan="5">
			                    <table width="100%" cellpadding="0" cellspacing="0" border="0">
				                    <tr>
					                    <td class="tableDataHdr2" >
                                            <asp:Label runat="server" ID="lblPlantResultsHdr" Text="Results" Visible="true"></asp:Label>
                                        </td>
	                                    <td align="right">
					                <!-- BUTTONS -->		
					                        <table cellpadding=0 cellspacing=2 border=0>
						                        <tr>
                                                    <TD>
                                                       <asp:Button ID="lbPlantAdmin" runat="server" onClick="lbPlantAdmin_Click" CSSclass="buttonStd" text="Business Organization Admin"></asp:Button>
                                                    </TD>
                                                </tr>
                                            </table>
               			                <!-- END OF BUTTONS -->
					                    </td>
				                    </tr>
			                    </table>
                            </TD>
                        </TR>

                        <asp:Panel runat="server" ID="pnlPlant">
                        <!--#include file="/Include/Inc_Plant_List.aspx"-->
                        </asp:Panel>

			        
                    </table>
                </TD>
            </TR>
        </table>
        <br>
    </div>
</asp:Content>
