<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_DocMgr.ascx.cs" Inherits="SQM.Website.Ucl_DocMgr" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

 <script type="text/javascript">
	 function DisplayDocument(sender, args) {
		 var item = args.get_item();
		 if (item.get_value() != "0") {
			 sender.clearSelection();
			 sender.set_emptyMessage('select document to view');
			 cmd = '../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID=' + item.get_value();
			 window.open(cmd);
		 }
	 }
   </script>

   <telerik:RadAjaxPanel runat="server" ID="pnlSelectDocument" Visible=false>
		<span>
			<asp:Label ID="lblSelectDocs" runat="server" CssClass="promptInverse"  visible=false Text="Documents:"></asp:Label>
			<telerik:RadComboBox ID="ddlSelectDocs" runat="server" ZIndex=9000 Width=200 DropDownCssClass="multipleRowsColumns" DropDownWidth="560px" Skin="Metro"  EmptyMessage="Application References" AutoPostBack="false"  OnClientSelectedIndexChanged="DisplayDocument"></telerik:RadComboBox>
		</span>
   </telerik:RadAjaxPanel>

	<asp:Panel ID="pnlDocMgr" runat="server" Visible = "false">
		<table width="100%" border="0" cellspacing="0" cellpadding="1" class="tabActiveTableBg">
			<tr>
				<td align="center">
					<table width="100%" border="0" noshade cellspacing="1" cellpadding="1" class="lightBorder">
						<tr>
							<td class="tableDataHdr">
								<asp:Label runat="server" ID="lblDefaultDocuments" Text="Stored Documents" CSSclass="tableDataHdr2"></asp:Label>
							</td>
						</tr>
						<tr>
							<td align=center>
								<div id="divDocsGVScroll" runat="server" class="">
								<asp:GridView ID="gvUploadedFiles" runat="server" AutoGenerateColumns="False" Width="100%"
									CssClass="Grid" OnRowDeleting="gvUploadedFiles_RowDeleting" OnRowDataBound="gvUploadedFiles_OnRowDataBound">
										<HeaderStyle CssClass="HeadingCellTextLeft" />    
										<RowStyle CssClass="DataCell" />
									<Columns>
										<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Category" SortExpression="DISPLAY_TYPE" ItemStyle-Width="15%">
											<ItemTemplate>
												<asp:HiddenField  ID="hfDisplayArea" runat="server" Value='<%#Eval("DISPLAY_TYPE") %>' />
												<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:TemplateField HeaderText="" ItemStyle-Width="3%">
											<ItemTemplate>
												<asp:HiddenField  ID="hfFileName" runat="server" Value='<%#Eval("FILE_NAME") %>' />
												<asp:Image  ID="imgFileType" runat="server" HeaderText="File Type" ItemStyle-HorizontalAlign="Center"></asp:Image>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="File Name" SortExpression="FILE_NAME" ItemStyle-Width="30%">
											<ItemTemplate>
										   <a href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID"))%>' class="linkUnderline" target="_blank"><%#Eval("FILE_NAME")%></a>
											</ItemTemplate>
										</asp:TemplateField>

										<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Size" SortExpression="FILE_SIZE"
											ItemStyle-Width="10%">
											<ItemTemplate>
												<%# FormatFilesize(Eval("FILE_SIZE"))%>
											</ItemTemplate>
										</asp:TemplateField>
										<asp:BoundField DataField="FILE_DESC" HeaderText="Description" ItemStyle-Width="35%" />
										<asp:CommandField ShowDeleteButton="True" ItemStyle-HorizontalAlign="Center" />
									</Columns>
								</asp:GridView>
								<asp:Label runat="server" ID="lblDocsListEmpty" Text="The documents list is empty." class="GridEmpty" Visible="false"></asp:Label>
							   </div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td>
					<table border="0" cellspacing="0" cellpadding="5" style="margin-left: 20px">
						<tr>
							<td>
								<input type="button" onclick="PopupCenter('../Shared/Shared_Upload.aspx', 'newPage', 800, 600);"
									value="Upload Documents" class="buttonStd"></input>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
  </asp:Panel>

 <asp:Panel ID="pnlDocList" runat="server" Visible = "false">
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align=center>
				<div class="borderSoft" id="divDocListGVScroll" runat="server">
				<asp:GridView ID="gvDocList" runat="server" AutoGenerateColumns="False" Width="100%" CssClass="Grid"  GridLines="None" OnRowDataBound="gvDocs_OnRowDataBound">
					<HeaderStyle CssClass="HeadingCellTextLeft" />    
					<RowStyle CssClass="DataCell" />
					<Columns>
						<asp:TemplateField HeaderText="" ItemStyle-Width="3%">
							<ItemTemplate>
								<asp:HiddenField  ID="hfFileName" runat="server" Value='<%# Eval("FILE_NAME") %>' />
								<asp:Image  ID="imgFileType" runat="server" HeaderText="" ItemStyle-HorizontalAlign="Center"></asp:Image>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Document Name" ItemStyle-Width="30%" >
							<ItemTemplate>
								<a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_NAME")%></a>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Description" ItemStyle-Width="33%" >
							<ItemTemplate>
								<a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_DESC")%></a>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Category" ItemStyle-Width="15%">
							<ItemTemplate>
								<asp:HiddenField  ID="hfDisplayArea" runat="server" Value='<%# Eval("DISPLAY_TYPE") %>' />
								<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Posted By" ItemStyle-Width="15%">
							<ItemTemplate>
								<asp:HiddenField id="hfPostedDate" runat="server" Value='<%# Eval("UPLOADED_DT") %>'/>
								<asp:HiddenField id="hfPostedBy" runat="server" Value='<%# Eval("UPLOADED_BY") %>'/>
								<asp:Label ID="lblPosted" runat="server"></asp:Label>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
					</asp:GridView>
					<asp:Label runat="server" ID="lblGVDocsListEmpty" Text="Your documents list is empty." class="GridEmpty" Visible="False" ></asp:Label>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlDocListRpt" runat="server" Visible = "false">
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align=center>
				<div id="divDocListRptScroll" runat="server" class="">
					<asp:Repeater runat="server" ID="rptDocList" ClientIDMode="AutoID" OnItemDataBound="rptDocList_OnItemDataBound">
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="2" border="0" width="100%">
						</HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocumentHdr" Text="Document" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Image  ID="imgFileType" runat="server" HeaderText="" ItemStyle-HorizontalAlign="Center" style="dsplay:block; vertical-align: middle;"></asp:Image>
									 <a class="linkUnderline" style="vertical-align: middle;" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_NAME")%></a>
									 <br>
									 <a class="refText" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_DESC")%></a>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocTypeHdr" Text="Scope" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
									<br>
									<asp:Label ID="lblDocReference" runat="server"></asp:Label>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocPostedHdr" Text="Posted" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblPosted" runat="server"></asp:Label>
								</td>
							</tr>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<tr>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocumentHdr" Text="Document" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Image  ID="imgFileType" runat="server" HeaderText="" ItemStyle-HorizontalAlign="Center" style="dsplay:block; vertical-align: middle;"></asp:Image>
									 <a class="linkUnderline"  style="vertical-align: middle;" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_NAME")%></a>
									 <br>
									 <a class="refText" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_DESC")%></a>
								</td>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocTypeHdr" Text="Scope" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
									<br>
									<asp:Label ID="lblDocReference" runat="server"></asp:Label>
								</td>
								<td class="listDataAlt" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocPostedHdr" Text="Posted" Visible="true"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblPosted" runat="server"></asp:Label>
								</td>
							</tr>
						</AlternatingItemTemplate>
						<FooterTemplate>
							</table></FooterTemplate>
					</asp:Repeater>
				</div>
				<asp:Label runat="server" ID="lblDocListRptEmpty" Height="40" Text="The document list is empty."
					class="GridEmpty" Visible="false"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlRadDocsList" runat="server" Visible = "false">                                                 
	<telerik:RadGrid ID="rgDocsList" runat="server" Skin="Metro" MasterTableView-CssClass="RadFileExplorer"
		MasterTableView-BorderColor="LightGray" MasterTableView-BorderWidth="0" MasterTableView-Font-Size="11px" MasterTableView-ForeColor="#444444">
		<MasterTableView DataKeyNames="DOCUMENT_ID" Width="100%" AutoGenerateColumns="False">
			<Columns>
				<telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="Document" HeaderStyle-Width="100">
					<ItemTemplate>
						<div class="rfeFileExtension <%# GetFileExtension(DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString()) %>">
							<a href="/Shared/FileHandler.ashx?DOC=d&DOC_ID=<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID").ToString() %>&FILE_NAME=<%# DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString() %>"
								style="text-decoration: underline;" target="_blank">
								<%# DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString() %>
							</a>
						</div>
					</ItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridBoundColumn DataField="FILE_DESC" HeaderText="Description"></telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="UPLOADED_DT" HeaderText="Posted"></telerik:GridBoundColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
</asp:Panel>