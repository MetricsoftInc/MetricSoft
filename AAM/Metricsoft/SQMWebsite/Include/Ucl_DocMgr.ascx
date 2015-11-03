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

   <telerik:RadAjaxPanel runat="server" ID="pnlSelectDocument" Visible=False HorizontalAlign="NotSet">
		<span>
			<asp:Label ID="lblSelectDocs" runat="server" CssClass="promptInverse"  visible=False></asp:Label>
			<telerik:RadComboBox ID="ddlSelectDocs" runat="server" ZIndex=9000 Width=200px DropDownCssClass="multipleRowsColumns" DropDownWidth="560px" Skin="Metro"  EmptyMessage="Application References"  OnClientSelectedIndexChanged="DisplayDocument" meta:resourcekey="ddlSelectDocsResource1"></telerik:RadComboBox>
		</span>
   </telerik:RadAjaxPanel>

	<asp:Panel ID="pnlDocMgr" runat="server" Visible = "False">
		<table width="100%" border="0" cellspacing="0" cellpadding="1" class="tabActiveTableBg">
			<tr>
				<td align="center">
					<table width="100%" border="0" noshade cellspacing="1" cellpadding="1" class="lightBorder">
						<tr>
							<td class="tableDataHdr">
								<asp:Label runat="server" ID="lblDefaultDocuments" Text="Stored Documents" CSSclass="tableDataHdr2" meta:resourcekey="lblDefaultDocumentsResource1"></asp:Label>
							</td>
						</tr>
						<tr>
							<td align=center>
								<div id="divDocsGVScroll" runat="server" class="">
								<asp:GridView ID="gvUploadedFiles" runat="server" AutoGenerateColumns="False" Width="100%"
									CssClass="Grid" OnRowDeleting="gvUploadedFiles_RowDeleting" OnRowDataBound="gvUploadedFiles_OnRowDataBound">
										<Columns>
											<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="<%$ Resources:LocalizedText, Category %>" SortExpression="DISPLAY_TYPE">
												<ItemTemplate>
													<asp:HiddenField ID="hfDisplayArea" runat="server" Value='<%# Eval("DISPLAY_TYPE") %>' />
													<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
												</ItemTemplate>
												<ItemStyle Width="15%" />
											</asp:TemplateField>
											<asp:TemplateField>
												<ItemTemplate>
													<asp:HiddenField ID="hfFileName" runat="server" Value='<%# Eval("FILE_NAME") %>' />
													<asp:Image ID="imgFileType" runat="server" HeaderText="File Type" ItemStyle-HorizontalAlign="Center" />
												</ItemTemplate>
												<ItemStyle Width="3%" />
											</asp:TemplateField>
											<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="<%$ Resources:LocalizedText, FileName %>" SortExpression="FILE_NAME">
												<ItemTemplate>
													<a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%# Eval("FILE_NAME") %></a>
												</ItemTemplate>
												<ItemStyle Width="30%" />
											</asp:TemplateField>
											<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="<%$ Resources:LocalizedText, Size %>" SortExpression="FILE_SIZE">
												<ItemTemplate>
													<%# FormatFilesize(Eval("FILE_SIZE")) %>
												</ItemTemplate>
												<ItemStyle Width="10%" />
											</asp:TemplateField>
											<asp:BoundField DataField="FILE_DESC" HeaderText="<%$ Resources:LocalizedText, Description %>">
											<ItemStyle Width="35%" />
											</asp:BoundField>
											<asp:CommandField ShowDeleteButton="True">
											<ItemStyle HorizontalAlign="Center" />
											</asp:CommandField>
										</Columns>
										<HeaderStyle CssClass="HeadingCellTextLeft" />
										<RowStyle CssClass="DataCell" />
								</asp:GridView>
								<asp:Label runat="server" ID="lblDocsListEmpty" Text="The documents list is empty." class="GridEmpty" Visible="False" meta:resourcekey="lblDocsListEmptyResource1"></asp:Label>
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

 <asp:Panel ID="pnlDocList" runat="server" Visible = "False">
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align=center>
				<div class="borderSoft" id="divDocListGVScroll" runat="server">
				<asp:GridView ID="gvDocList" runat="server" AutoGenerateColumns="False" Width="100%" CssClass="Grid"  GridLines="None" OnRowDataBound="gvDocs_OnRowDataBound">
					<Columns>
						<asp:TemplateField>
							<ItemTemplate>
								<asp:HiddenField ID="hfFileName" runat="server" Value='<%# Eval("FILE_NAME") %>' />
								<asp:Image ID="imgFileType" runat="server" HeaderText="" ItemStyle-HorizontalAlign="Center" />
							</ItemTemplate>
							<ItemStyle Width="3%" />
						</asp:TemplateField>
						<asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Document Name" meta:resourcekey="TemplateFieldResource6">
							<ItemTemplate>
								<a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%# Eval("FILE_NAME") %></a>
							</ItemTemplate>
							<ItemStyle Width="30%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Description %>">
							<ItemTemplate>
								<a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%# Eval("FILE_DESC") %></a>
							</ItemTemplate>
							<ItemStyle Width="33%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Category %>">
							<ItemTemplate>
								<asp:HiddenField ID="hfDisplayArea" runat="server" Value='<%# Eval("DISPLAY_TYPE") %>' />
								<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
							</ItemTemplate>
							<ItemStyle Width="15%" />
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Posted By" meta:resourcekey="TemplateFieldResource9">
							<ItemTemplate>
								<asp:HiddenField ID="hfPostedDate" runat="server" Value='<%# Eval("UPLOADED_DT") %>' />
								<asp:HiddenField ID="hfPostedBy" runat="server" Value='<%# Eval("UPLOADED_BY") %>' />
								<asp:Label ID="lblPosted" runat="server"></asp:Label>
							</ItemTemplate>
							<ItemStyle Width="15%" />
						</asp:TemplateField>
					</Columns>
					<HeaderStyle CssClass="HeadingCellTextLeft" />
					<RowStyle CssClass="DataCell" />
					</asp:GridView>
					<asp:Label runat="server" ID="lblGVDocsListEmpty" Text="Your documents list is empty." class="GridEmpty" Visible="False" meta:resourcekey="lblGVDocsListEmptyResource1" ></asp:Label>
				</div>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlDocListRpt" runat="server" Visible = "False">
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td align=center>
				<div id="divDocListRptScroll" runat="server" class="">
					<asp:Repeater runat="server" ID="rptDocList" ClientIDMode="AutoID" OnItemDataBound="rptDocList_OnItemDataBound">
						<AlternatingItemTemplate>
							<tr>
								<td class="listDataAlt" valign="top"><span class="summaryHeader">
									<asp:Label ID="lblDocumentHdr" runat="server" meta:resourcekey="lblDocumentHdrResource1" Text="Document"></asp:Label>
									</span>
									<br>
									<asp:Image ID="imgFileType" runat="server" HeaderText="" ItemStyle-HorizontalAlign="Center" style="dsplay:block; vertical-align: middle;" />
									<a class="linkUnderline" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' style="vertical-align: middle;" target="_blank"><%# Eval("FILE_NAME") %></a>
									<br><a class="refText" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%# Eval("FILE_DESC") %></a></br>
									</br>
								</td>
								<td class="listDataAlt" valign="top"><span class="summaryHeader">
									<asp:Label ID="lblDocTypeHdr" runat="server" meta:resourcekey="lblDocTypeHdrResource1" Text="Scope"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
									<br>
									<asp:Label ID="lblDocReference" runat="server"></asp:Label>
									</br>
									</br>
								</td>
								<td class="listDataAlt" valign="top"><span class="summaryHeader">
									<asp:Label ID="lblDocPostedHdr" runat="server" meta:resourcekey="lblDocPostedHdrResource1" Text="Posted"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblPosted" runat="server"></asp:Label>
									</br>
								</td>
							</tr>
						</AlternatingItemTemplate>
						<FooterTemplate>
							</table>
						</FooterTemplate>
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="2" border="0" width="100%">
						</HeaderTemplate>
						<ItemTemplate>
							<tr>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocumentHdr" Text="Document" meta:resourcekey="lblDocumentHdrResource1"></asp:Label>
									</span>
									<br>
									<asp:Image  ID="imgFileType" runat="server" HeaderText="" ItemStyle-HorizontalAlign="Center" style="dsplay:block; vertical-align: middle;"></asp:Image>
									 <a class="linkUnderline" style="vertical-align: middle;" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_NAME")%></a>
									 <br>
									 <a class="refText" href='<%# String.Format("../Shared/SQMImageHandler.ashx?DOC=d&DOC_ID={0}", Eval("DOCUMENT_ID")) %>' target="_blank"><%#Eval("FILE_DESC")%></a>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocTypeHdr" Text="Scope" meta:resourcekey="lblDocTypeHdrResource1"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblDisplayArea" runat="server"></asp:Label>
									<br>
									<asp:Label ID="lblDocReference" runat="server"></asp:Label>
								</td>
								<td class="listData" valign="top">
									<span class="summaryHeader">
										<asp:Label runat="server" ID="lblDocPostedHdr" Text="Posted" meta:resourcekey="lblDocPostedHdrResource1"></asp:Label>
									</span>
									<br>
									<asp:Label ID="lblPosted" runat="server"></asp:Label>
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
				</div>
				<asp:Label runat="server" ID="lblDocListRptEmpty" Height="40px" Text="The document list is empty."
					class="GridEmpty" Visible="False" meta:resourcekey="lblDocListRptEmptyResource1"></asp:Label>
			</td>
		</tr>
	</table>
</asp:Panel>

<asp:Panel ID="pnlRadDocsList" runat="server" Visible = "False">
	<telerik:RadGrid ID="rgDocsList" runat="server" Skin="Metro" GroupPanelPosition="Top">
		<MasterTableView DataKeyNames="DOCUMENT_ID" Width="100%" AutoGenerateColumns="False" BorderColor="LightGray" BorderWidth="0px" CssClass="RadFileExplorer" Font-Size="11px" ForeColor="#444444">
			<Columns>
				<telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="Document" FilterControlAltText="Filter TemplateColumn column" meta:resourcekey="GridTemplateColumnResource1">
					<ItemTemplate>
						<div class="rfeFileExtension <%# GetFileExtension(DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString()) %>">
							<a href="/Shared/FileHandler.ashx?DOC=d&DOC_ID=<%# DataBinder.Eval(Container.DataItem, "DOCUMENT_ID").ToString() %>&FILE_NAME=<%# DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString() %>"
								style="text-decoration: underline;" target="_blank">
								<%# DataBinder.Eval(Container.DataItem, "FILE_NAME").ToString() %>
							</a>
						</div>
					</ItemTemplate>
					<HeaderStyle Width="100px" />
				</telerik:GridTemplateColumn>
				<telerik:GridBoundColumn DataField="FILE_DESC" HeaderText="<%$ Resources:LocalizedText, Description %>" FilterControlAltText="Filter FILE_DESC column" UniqueName="FILE_DESC"></telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="UPLOADED_DT" HeaderText="Posted" FilterControlAltText="Filter UPLOADED_DT column" meta:resourcekey="GridBoundColumnResource2" UniqueName="UPLOADED_DT"></telerik:GridBoundColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
</asp:Panel>