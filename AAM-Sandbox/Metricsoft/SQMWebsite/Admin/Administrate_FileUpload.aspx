<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_FileUpload.aspx.cs" Inherits="SQM.Website.Administrate_FileUpload" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <script type="text/javascript">
        function DisplaySelectedFileName(fileUpload) {
            document.getElementById('tbFileSelected').value = fileUpload.value;
            var btn = document.getElementById('btnUpload');
            btn.disabled = false;
            btn = document.getElementById('btnPreview');
            btn.disabled = false;
            document.getElementById('lblSummaryList').style.display = 'none';
        }

        function DisplayPlantSelector(dataType) {
        	var strData = document.getElementById(dataType.id);
        	var e = document.getElementById('dvPlantSelect');
        	if (strData.value == "RECEIPT")
        		e.style.display = 'block';
        	else
				e.style.display = 'none';
        }
    </script>
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:HiddenField ID="hfFileSelected" runat="server" />
			        <BR/>
<!--Border table of record data-->
                    <table width="98%" border="0" cellspacing="0" cellpadding="0" align="center">
			            <tr>
						  	<td class="pageTitles">
                                <asp:Label ID="lblDataUploadTitle" runat="server" Text="Upload Data"></asp:Label>
                            </td>
                       </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText" style="margin-top: 10px;" Text="<br />Use this page to upload or update configuration data.<br /> Supported data objects include monthly Plant Accounting (file name: PART_DATA) and Part Numbers (file name: PART)."></asp:Label>
                            </td>
					    </tr>
				    </table><BR/> 

<!--Border table of record data-->
                    <table width="99%" border="0" cellspacing="1" cellpadding="3" class="lightBorder">
                        <tr>
                            <td class="columnHeader" width="15%">
                                <b>Data File Type</b>
		                    </td>
							<!-- AW20140113 - Add file type and reporting month -->
							<td class="tableDataAlt">
								<asp:DropDownList runat="server" ID="ddlDataType" onchange="DisplayPlantSelector(this);">
									<asp:ListItem Text="select a data type" Value=""></asp:ListItem>
									<%--<asp:ListItem Text="Currency" Value="CURRENCY_DATA"></asp:ListItem>--%>
									<asp:ListItem Text="References" Value="REFERENCE"></asp:ListItem>
									<asp:ListItem Text="Person" Value="PERSON"></asp:ListItem>
									<asp:ListItem Text="Plant" Value="PLANT"></asp:ListItem>
									<asp:ListItem Text="Plant Data" Value="PLANT_DATA"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td class="columnHeader" width="15%">
                                <b>Process File</b>
		                    </td>
							<td class="tableDataAlt">
                                <asp:FileUpload ID="flUpload" runat="server" Width="200" onchange="DisplaySelectedFileName(this);" />
                                <br />
                                <asp:Button ID="btnPreview" runat="server" Text="Preview File" CssClass="buttonStd" onclick="PreviewFile" style="margin-top: 4px;"></asp:Button>
                                &nbsp;&nbsp
                                 <asp:TextBox ID="tbFileSelected" runat="server" Columns="40" ReadOnly="true" style="border:0px;background-color:transparent;" /> 
                                <br />
                                <asp:Button ID="btnUpload" runat="server" Text="Process File"  CssClass="buttonEmphasis" onclick="btnUploadFile_Click" style="margin-top: 4px;"></asp:Button>
                            </td>
                        </tr>
                    </table>
                    <br>

                    <asp:GridView runat="server" ID="gvPreview" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvPreview_OnRowDataBound">
                        <HeaderStyle CssClass="HeadingCellTextLeft" />    
                        <RowStyle CssClass="DataCell" />
                	        <Columns>
                                <asp:TemplateField HeaderText="File Contents" ItemStyle-Width="100%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblLine" runat="server" CssClass="textStd"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                    </asp:GridView>
					<asp:GridView runat="server" ID="gvExcelPreview" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="true"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="false" Width="99%">
                        <HeaderStyle CssClass="HeadingCellTextLeft" />    
                        <RowStyle CssClass="DataCell" />
                    </asp:GridView>

                    <br />
       	            <table width="99%" cellpadding="1" cellspacing="1" border="0" >
                        <TR> 
					        <td class="HeadingCellText" align="center" >
                                <asp:Label runat="server" ID="lblUploadResultsHdr" Text="Results" Visible="true"></asp:Label>
                             </td>
                        </TR>
                        <tr>
                            <td valign="top" align="center" class="admBkgd">
                                <!-- updated items grid -->
                                <asp:GridView runat="server" ID="gvUpdateList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                   <HeaderStyle CssClass="HeadingCellTextLeft" />    
                                  <RowStyle CssClass="DataCell" />
                	              <Columns>
                                     <asp:BoundField DataField="Object" HeaderText="Object" ItemStyle-Width="22%" />
                                     <asp:BoundField DataField="Value" HeaderText="Data" ItemStyle-Width="50%" />
                                     <asp:BoundField DataField="Action" HeaderText="Action" ItemStyle-Width="8%" />
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" align="center" class="admBkgd">
                                <!-- error list grid -->
                                <asp:GridView runat="server" ID="gvErrorList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnRowDataBound">
                                   <HeaderStyle CssClass="HeadingCellTextLeft" />    
                                  <RowStyle CssClass="DataCell" />
                	              <Columns>
                                     <asp:BoundField DataField="LineNo" HeaderText="Line" ItemStyle-Width="5%" />
                                     <asp:BoundField DataField="Node" HeaderText="Tag" ItemStyle-Width="15%" />
                                     <asp:BoundField DataField="Message" HeaderText="Error Message" ItemStyle-Width="55%" />
                                     <asp:BoundField DataField="Value" HeaderText="Value" ItemStyle-Width="25%" />
                                    </Columns>
                                </asp:GridView>
                                <asp:Label runat="server" ID="lblSummaryList" Text="There were no updates." class="GridEmpty" Visible="false"></asp:Label>
                            </td>
                        </tr>
			        </table>
                </TD>
            </TR>
        </table>
        <br>
    </div>
</asp:Content>
