<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_Part.aspx.cs" Inherits="SQM.Website.Administrate_Part" %>
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
                                <asp:Label ID="lblPartAdminTitle" runat="server" Text="Part Administration"></asp:Label>
                            </td>
						  </tr>
				  </table>
                  <BR/> 

				
<!-- PAGE DATA CONTENT TABLE --> 

					  <!-- DATA ENTRY CONTENT TABLE -->	
				
					  	 
            <TABLE WIDTH="98%" BORDER="0" CELLSPACING="1" CELLPADDING="4" class="darkBorder">
              <TR> 
                <TD CLASS="columnHeader" width="40%">
                    <asp:Label ID="lblNewPart" runat="server" Text="Add Part"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt" width="60%">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                        <TD>
                            <asp:Button ID="lbCreatePart" runat="server" CSSclass="buttonStd" text="Create Part" onclick="lbCreatePart_Click"></asp:Button>
                        </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <tr>
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblSearchPart" runat="server" Text="Search For Part(s)"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                                <br />
                                <asp:RadioButtonList runat="server" ID="rblSearchPart" CSSclass="radioList" 
	                                RepeatDirection="Horizontal" RepeatLayout="flow" 
		                            AutoPostBack="false">
							            <asp:ListItem Text="All&nbsp;&nbsp;" Value="" ></asp:ListItem>
								        <asp:ListItem Text="All Active" Value="A"></asp:ListItem>
						        </asp:RadioButtonList>
                            </td>
                            <!--
                            <TD>
                                <asp:Button ID="lbSearchPart" runat="server" CSSclass="buttonStd" onclick="lbSearchPart_Click" Text="Search"></asp:Button>
                            </TD>
                            -->
                            <TD>
                                 <input type="button" onclick="PopupCenter('../Shared/Shared_PartSearch.aspx?', 'newPage', 800, 600);"
                                    value="Expanded Search" class="buttonStd"></input>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <TR> 
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblUploadData" runat="server" Text="Upload Part Master Data"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <TD>
                                <asp:Button ID="lbUploadData" runat="server" class="buttonStd" 
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
