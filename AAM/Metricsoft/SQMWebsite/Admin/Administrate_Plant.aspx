<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_Plant.aspx.cs" Inherits="SQM.Website.Administrate_Plant" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<%--<link href="css/PSSQM.css" rel="stylesheet" type="text/css" />--%>
    <div class="admin_tabs">
      <table width="100%" border="0" cellspacing="0" cellpadding="2">
        <tr>
          <td class="tabActiveTableBg" colspan="10" align="center">
			<BR/>

<!--Border table of record data-->
                  <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
                    <tr>
				        <td class="pageTitles">
                           <asp:Label ID="lblPlantAdminTitle" runat="server" Text="Plant Administration"></asp:Label>
                        </td>
				    </tr>
				  </table>
                  <BR/>


<!-- PAGE DATA CONTENT TABLE -->

					  <!-- DATA ENTRY CONTENT TABLE -->


            <TABLE WIDTH="98%" BORDER="0" CELLSPACING="1" CELLPADDING="4" class="darkBorder">
            <tr>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblPlantSearch" runat="server" Text="Search For Plant"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                            </td>
                            <TD>
                                 <asp:Button ID="lbSearchPlant" runat="server" CSSclass="buttonStd" Text="<%$ Resources:LocalizedText, Search %>" onclick="lbSearchPlant_Click"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <TR>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblViewAllPlant" runat="server" Text="View All Plants"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                        <TD>
                             <asp:Button ID="lbViewAllPlant" runat="server" CSSclass="buttonStd" text="View All Plants"
                                onclick="lbSearchPlant_Click" CommandArgument="%" ></asp:Button>
                        </TD>
                        <TD>
                               <asp:Button ID="lbViewAllActivePlant" runat="server" CSSclass="buttonStd" text="View All Active"
                                onclick="lbSearchPlant_Click" CommandArgument="%~A" ></asp:Button>
                        </TD>
                        </TR>
                    </TABLE>
            </TR>

            <TR>
                <TD CLASS="columnHeader">Associate Plants
                    <asp:Label ID="lblAssocPlant" runat="server" Text="Associate Plants"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                             <TD>
                               <asp:Button ID="lbAssocPlant" runat="server" CSSclass="buttonStd" text="Associate Plant(s)"
                                onclick="lbSearchPlant_Click" CommandArgument="%~U" ></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <TR>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblUploadData" runat="server" Text="Upload Plant Definitions"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                             <TD>
                                <asp:Button ID="lbUploadData" runat="server" CSSclass="buttonStd"
                                onclick="lbUploadData_Click" text="Upload Data"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
          </TABLE>
        </td>
      </tr>
    </table>
    <BR>

   </div>
</asp:Content>
