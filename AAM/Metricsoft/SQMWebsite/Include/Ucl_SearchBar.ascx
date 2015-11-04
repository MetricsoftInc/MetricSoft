<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_SearchBar.ascx.cs" Inherits="SQM.Website.Ucl_SearchBar" %>
 <link type="text/css" href="../css/redmond/jquery-ui-1.8.20.custom.css" rel="Stylesheet" />
 <script type="text/javascript">

</script>

    <div id="divSearchBar" runat="server">
		<table width="100%" border="0" cellspacing="0" cellpadding="0" class="tabActiveTableBg">
                <tr>
                    <td class="pageTitles" >
                        <span>
                            <asp:Label ID="lblPageTitle" runat="server" style="margin-left: 5px;"></asp:Label>
                            <asp:Label ID="lblTitleItem_out" runat="server" style="padding-left: 10px;" ></asp:Label>
                        </span>
                    </td>
                    <td align="right">
						<table border="0" cellspacing="0" cellpadding="1">
						  	<tr>
                                <td>
                                    <%-- <asp:TextBox ID="tbSearch" runat="server" style="width:150px;" ></asp:TextBox> --%>
                                     <asp:Button ID="btnReturn" runat="server" onclick="btnReturn_Click" Text="Return" CssClass="buttonReturn" CommandArgument="return" style="margin-right: 30px;" meta:resourcekey="btnReturnResource1"></asp:Button>
                                    <asp:Button ID="btnSearch" class="buttonStd" runat="server" onclick="btnSearch_Click" text="List" style="width: 70px; margin-right: 5px;"></asp:Button>
                                    <asp:Button ID="btnNew"  runat="server" CssClass="buttonAddLarge" onclick="btnNew_Click" text="<%$ Resources:LocalizedText, New %>"  CommandArgument="new"> </asp:Button>
                                    <asp:Button ID="btnEdit" class="buttonStd" runat="server" onclick="btnEdit_Click" Text="<%$ Resources:LocalizedText, Edit %>" style="width: 70px;" CommandArgument="edit" ></asp:Button>
                                    <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" OnClientClick="return confirmChange('Item');" onclick="btnSave_Click" style="width: 70px;" Text="<%$ Resources:LocalizedText, Save %>" CommandArgument="save" ></asp:Button>
                                    <asp:Button ID="btnCancel" class="buttonStd" runat="server" OnClientClick="return confirmAction('Cancel without saving');" onclick="btnCancel_Click" style="width: 70px; margin-right: 5px;" Text="<%$ Resources:LocalizedText, Cancel %>" CommandArgument="cancel"></asp:Button>
                                    <asp:Button ID="btnUpload" class="buttonStd" runat="server" onclick="btnUpload_Click" text="Upload..." style="width: 70px;" CommandArgument="upload"></asp:Button>
                                    &nbsp;&nbsp;&nbsp;&nbsp;
								</td>
                            </TR>
                        </TABLE>
                    </td>
                </tr>
		</table>
   </div>
