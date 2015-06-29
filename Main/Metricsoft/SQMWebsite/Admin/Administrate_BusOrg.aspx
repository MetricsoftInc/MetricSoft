<%@ Page Title="" Language="C#" MasterPageFile="~/Administrate.master" AutoEventWireup="true" CodeBehind="Administrate_BusOrg.aspx.cs" Inherits="SQM.Website.Administrate_BusOrg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<link href="../css/PSSQM.css" rel="stylesheet" type="text/css" />
    <div class="admin_tabs">
      <table width="100%" border="0" cellspacing="0" cellpadding="2">
        <tr>
          <td class="tabActiveTableBg" colspan="10" align="center">
			<BR/>

<!--Border table of record data-->
                  <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
		            <tr>
                         <td class="pageTitles">
                               <asp:Label ID="lblBusOrgAdminTitle" runat="server" Text="Business Organization Administration"></asp:Label>
                        </td>
				    </tr>
				  </table>
                  <BR/> 
				
<!-- PAGE DATA CONTENT TABLE --> 

					  <!-- DATA ENTRY CONTENT TABLE -->	
				
					  	 
            <TABLE WIDTH="98%" BORDER="0" CELLSPACING="1" CELLPADDING="4" class="darkBorder">
            <TR> 
                <TD CLASS="columnHeader" width="40%">
                     <asp:Label ID="lblNewBusOrg" runat="server" Text="New Business Organization"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt" width="60%">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                        <TD>
                            <asp:Button ID="lbCreateBusOrg" runat="server" CSSclass="buttonStd" text="Create Business Organization" 
                                onclick="lbCreateBusOrg_Click"></asp:Button>
                        </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <TR> 
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblBusOrgSearch" runat="server" Text="Search For Business Organization"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                                <br />
                                    <asp:RadioButtonList runat="server" ID="rblSearchBusOrg" CSSclass="radioList" 
	                                RepeatDirection="Horizontal" RepeatLayout="flow" 
		                            AutoPostBack="false">
							            <asp:ListItem Text="All&nbsp;&nbsp;" Value="%" ></asp:ListItem>
								        <asp:ListItem Text="All Active" Value="%~A"></asp:ListItem>
						        </asp:RadioButtonList>
                           </td>
                            <TD>
                                <asp:Button ID="lbSearchBusOrg" runat="server" CSSclass="buttonStd" text="Search" onclick="lbSearchBusOrg_Click"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
            <TR> 
                <TD CLASS="columnHeader">
                    <asp:Label ID="Label1" runat="server" Text="Search For Plant"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <td>
                                <asp:TextBox ID="tbPlantSearchString" runat="server" CausesValidation="False" MaxLength="50" Style="width: 200px"></asp:TextBox>
                                    <br />
                                    <asp:RadioButtonList runat="server" ID="rblSearchPlant" CSSclass="radioList" 
	                                RepeatDirection="Horizontal" RepeatLayout="flow" 
		                            AutoPostBack="false">
							            <asp:ListItem Text="All&nbsp;&nbsp;" Value="%" ></asp:ListItem>
								        <asp:ListItem Text="All Active" Value="%~A"></asp:ListItem>
						        </asp:RadioButtonList>
                           </td>
                            <TD>
                                <asp:Button ID="lbSearchPlant" runat="server" CSSclass="buttonStd" text="Search" onclick="lbSearchPlant_Click"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>
            </TR>
 
            <TR> 
                <TD CLASS="columnHeader">
                        <asp:Label ID="lblAdminDfltDocs" runat="server" Text="Administrate Default Documents"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                        <TD>
                            <asp:Button ID = "btnDfltDocs" runat="server" CSSclass="buttonStd" 
                                text="Default Documents" onclick="btnDfltDocs_Click" ></asp:Button>
                        </TD>
                        </TR>
                    </TABLE>  
                </TD>                 
            </TR>

            <TR> 
                <TD CLASS="columnHeader">
                    <asp:Label ID="lblUploadData" runat="server" Text="Upload Business Organization Structures"></asp:Label>
                </TD>
                <TD CLASS="tableDataAlt">
                    <TABLE BORDER=0 CELLSPACING=0 CELLPADDING=5>
                        <TR>
                            <TD>
                               <asp:Button ID="lbUploadData" runat="server" CSSclass="buttonStd" onclick="lbUploadData_Click" text="Upload Data"></asp:Button>
                            </TD>
                        </TR>
                    </TABLE>
                </TD>                   
            </TR>
          </TABLE>
         </td>
        </tr>
         </table>
         <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup="file" EnableClientScript="true"/>
	    <BR>
       </div>

</asp:Content>
