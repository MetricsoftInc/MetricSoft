<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_RadAsyncUpload.ascx.cs"
	Inherits="SQM.Website.Ucl_RadAsyncUpload" EnableViewState="true" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
	function onClientFileUploadedJS(sender, args) {
		var contentType = args.get_fileInfo().ContentType;
		//alert(contentType);
		//alert(document.getElementById('<%=tbAttachDesc.ClientID%>'));
		//alert(document.getElementById('<%=tbAttachDesc.ClientID%>').value);
		//var desc = document.getElementById('<%=tbAttachDesc.ClientID%>').value;
		//alert(desc);
	    var radAsyncUpload = $find('<%=raUpload.ClientID%>');


		//document.getElementById('<%=hfDescriptions.ClientID%>').value += (desc + '|');
		//alert(document.getElementById('<%=hfDescriptions.ClientID%>').value);

		var row = args.get_row();
		if (radAsyncUpload != null) {
			var inputName = radAsyncUpload.getAdditionalFieldID('TextBox');
			var inputType = 'text';
			var inputID = inputName;
			var input = createInput(inputType, inputID, inputName);
			var label = createLabel(inputID);
			var br = document.createElement('br');
			var parentList = row.parentNode;
			document.getElementById('<%=hfListId.ClientID%>').value = parentList.id;
			input.onchange = inputChanged;
			row.appendChild(br);
			row.appendChild(label);
			row.appendChild(input);
		}
	}

	function inputChanged() {
		var parentList = this.parentNode.parentNode;
		if (parentList != null) {
			var rows = parentList.childNodes;
			var descField = document.getElementById('<%=hfDescriptions.ClientID%>');
			descField.value = '';
			for (i = 0; i < rows.length - 1; i++) {
				var textBox = rows[i].childNodes[4];
				descField.value += textBox.value + '|';
			}
		}
	}

	function createInput(inputType, inputID, inputName) {
		 var input = document.createElement( 'input' );
		 input.setAttribute( 'type', inputType);
		 input.setAttribute( 'id', inputID );
		 input.setAttribute( 'class', 'descriptionInput' );
		 input.setAttribute( 'name', inputName );
		 return input;
	}

	function createLabel(forArrt) {
		var label = document.createElement( 'label' );

		 label.setAttribute( 'for', forArrt );
		 label.setAttribute( 'class', 'descriptionLabel' );
		 label.innerHTML = 'Description (optional): ';

		 return label;
	}
	</script>

<telerik:RadGrid ID="rgFiles" runat="server" Skin="Metro" OnDeleteCommand="rgFiles_OnDeleteCommand" OnItemDataBound="rgFiles_ItemDataBound" MasterTableView-CssClass="RadFileExplorer"
 MasterTableView-BorderColor="LightGray" HeaderStyle-Font-Size="11px" MasterTableView-BorderWidth="0" MasterTableView-Font-Size="11px" MasterTableView-ForeColor="#444444">
	<MasterTableView DataKeyNames="AttachmentId" Width="100%" AutoGenerateColumns="False">
		<Columns>
			<telerik:GridTemplateColumn UniqueName="FileNameColumn" HeaderText="File" HeaderStyle-Width="100">
				<ItemTemplate>
					<div class="rfeFileExtension <%# GetFileExtension(DataBinder.Eval(Container.DataItem, "FileName").ToString().ToLower()) %>">
						<a href="/Shared/FileHandler.ashx?DOC=a&DOC_ID=<%# DataBinder.Eval(Container.DataItem, "AttachmentId").ToString() %>&FILE_NAME=<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>"
							style="text-decoration: underline;" target="_blank">
							<asp:Literal id="ltrFileName" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FileName").ToString() %>' />
						</a>
					</div>
				</ItemTemplate>
			</telerik:GridTemplateColumn>
			<telerik:GridBoundColumn UniqueName="DescriptionColumn" DataField="Description" HeaderText="<%$ Resources:LocalizedText, Description %>">
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn UniqueName="SizeColumn" DataField="Size" HeaderText="<%$ Resources:LocalizedText, Size %>" DataFormatString="{0:n0} KB">
			</telerik:GridBoundColumn>
            <telerik:GridTemplateColumn UniqueName="DisplayTypeColumn" DataField="DisplayType" HeaderText="Show in<br/>Reports" ItemStyle-HorizontalAlign="Center" >
                <ItemTemplate>
				    <asp:CheckBox ID="checkBox" runat="server" Checked='<%# GetChecked(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "DisplayType"))) %>' />
				</ItemTemplate>
            </telerik:GridTemplateColumn>
			<telerik:GridButtonColumn UniqueName="DeleteButtonColumn" ButtonType="PushButton" ItemStyle-ForeColor="DarkRed" ItemStyle-BorderStyle="None" ItemStyle-BorderWidth="0" ConfirmTitle="<%$ Resources:LocalizedText, Delete %>"
				ConfirmText="Delete Attachment - Are You Sure?" CommandName="Delete" Text="<%$ Resources:LocalizedText, Delete %>"
				ItemStyle-Font-Underline="true">
			</telerik:GridButtonColumn>
		</Columns>
	</MasterTableView>
    <ClientSettings  EnableAlternatingItems="false"></ClientSettings>
</telerik:RadGrid>
    <br style="clear: both;" /><%--<br />--%>
<table cellpadding="0" cellspacing="0">
	<tr>
		<td id="tdUploadImg" runat="server" style="vertical-align: bottom; padding-bottom: 16px;"><img src="/images/defaulticon/16x16/attachment.png" alt="" style="border: 0; cursor: pointer;" onclick="$telerik.$('.ruFileInput').click();" />
		</td>
		<td>
			<telerik:RadAsyncUpload runat="server" ID="raUpload" MultipleFileSelection="Disabled" MaxFileInputsCount="10" Localization-Select="Browse..."
			skin="Metro" OnClientFileUploaded="onClientFileUploadedJS" />
		</td>
	</tr>
	<tr id="trAttachDesc" runat="server" visible="false">
		<td colspan="2" style="vertical-align: bottom; padding-bottom: 16px;"><img src="/images/defaulticon/16x16/edit-document.png" alt="" style="border: 0; cursor: pointer;">
			<%--</asp:Label>--%><asp:TextBox id="tbAttachDesc" runat="server" maxlength="400" columns="40" CssClass="textStd" ToolTip="<%$ Resources:LocalizedText, AttachmentDescription %>"></asp:TextBox>
		</td>
	</tr>
</table>
<asp:HiddenField ID="hfListId" runat="server" />
<asp:HiddenField ID="hfDescriptions" runat="server" />
<asp:HiddenField ID="hfMode" runat="server" />


