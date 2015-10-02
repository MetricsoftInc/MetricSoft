<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PersonList.ascx.cs" Inherits="SQM.Website.Ucl_PersonList" %>

<asp:Panel ID="pnlPersonList" runat="server" Visible="false">
	<div id="divPartListScroll" runat="server" class="">
		<asp:GridView runat="server" ID="gvPersonList"  CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvPersonList_OnRowDataBound">
			<HeaderStyle CssClass="HeadingCellTextLeft" />    
			<RowStyle CssClass="DataCell" />
			<AlternatingRowStyle CssClass="DataCellAlt" /> 
			<Columns>
				<asp:TemplateField HeaderText="Person">
					<ItemTemplate>
						<asp:LinkButton ID="lnkViewPerson_out" runat="server" CommandArgument='<%#Eval("PersonId") %>'
							text='<%#Eval("PersonName") %>'  CSSclass="linkUnderline" OnClick="lnkPerson_Click"></asp:LinkButton>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Email">
					<ItemTemplate>
						<asp:Label ID="lblPartDesc_out" runat="server" text='<%#Eval("PersonEmail") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
		<asp:Label runat="server" ID="lblPartListEmpty" Height="40" Text="Person List Is Empty." class="GridEmpty" Visible="false"></asp:Label>
	</div>
</asp:Panel>

 