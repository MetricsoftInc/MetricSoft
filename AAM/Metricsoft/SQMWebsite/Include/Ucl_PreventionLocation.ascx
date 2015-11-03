<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PreventionLocation.ascx.cs"
	Inherits="SQM.Website.Ucl_PreventionLocation" %>
	<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Panel ID="pnlCase" Visible="true" runat="server">
<table width="100%" cellpadding="5" cellspacing="0" style="border-collapse: collapse;">
	<tr>
		<td class="tanCell" style="width: 30%;">
			Case:
		</td>
		<td class="greyCell">
			<telerik:RadComboBox ID="rcbCases" runat="server" Skin="Metro" OnSelectedIndexChanged="rcbCases_SelectedIndexChanged"
				AutoPostBack="true" Width="250" DropDownAutoWidth="Enabled">
			</telerik:RadComboBox>
		</td>
	</tr>
</table>
<br />
</asp:Panel>
<asp:Label ID="lblRequired" runat="server" Text="<%$ Resources:LocalizedText, RequiredFieldsMustBeCompleted %>" ForeColor="#cc0000" Font-Bold="true" Height="25" Visible="false"></asp:Label>
<asp:Label ID="lblSubmitted" runat="server" Text="Prevention Verification submitted." Font-Bold="true" Visible="false"></asp:Label>
<asp:Panel ID="pnlSelect" Visible="false" runat="server">
	<table width="100%" cellpadding="5" cellspacing="0" style="border-collapse: collapse;">
		<tr>
			<td class="tanCell" style="width: 30%;">
				Instructions:
				<span class="requiredStar">*</span>
			</td>
			<td class="greyCell" style="width: 70%;">
				<asp:TextBox ID="tbInstructions" Rows="5" TextMode="MultiLine" Width="100%" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class="tanCell" style="width: 30%;">
				<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, DueDate %>" />:
				<span class="requiredStar">*</span>
			</td>
			<td class="greyCell" style="width: 70%;">
				<telerik:RadDatePicker ID="rdpDueDate" Skin="Metro" runat="server">
				</telerik:RadDatePicker>
			</td>
		</tr>
	</table>
	<br />
	<asp:GridView runat="server" ID="gvPreventLocationsList" Name="gvPreventLocationsList"
		CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" CellPadding="5"
		GridLines="Both" PageSize="20" AllowSorting="false" Width="100%" OnRowDataBound="gvPreventLocationsList_RowDataBound"
		DataKeyNames="PLANT_ID">
		<HeaderStyle CssClass="HeadingCellText" />
		<RowStyle CssClass="DataCell" />
		<Columns>
			<asp:TemplateField HeaderText="Select People to Send Verification Notification<br/>(Drag or Ctrl+click to select multiple)">
				<ItemTemplate>
					<div style="float: left; font-size: 13px; padding: 8px 0 10px 0;">
						<asp:Label ID="lblPlant" runat="server" Text='<%#Eval("PLANT_NAME")%>' ClientIDMode="AutoID"
							Font-Bold="true"></asp:Label>
					</div>
					<div style="float: right;">
						<telerik:RadButton ID="rbSelectAll" runat="server" Text="Select All" Skin="Metro" AutoPostBack="false"
							 OnClientClicked="PVOnClientClicked">
						</telerik:RadButton>
					</div>
					<br style="clear: both;" />
					<telerik:RadGrid runat="server" ID="rgPlantContacts" Name="rgPlantContacts"
						Skin="Metro" AllowMultiRowSelection="true" AutoGenerateColumns="false" CellPadding="3"
						GridLines="None" AllowSorting="false" ShowHeader="false" Width="100%">
						<ClientSettings EnableRowHoverStyle="true">
							<Selecting AllowRowSelect="True"></Selecting>
							<ClientEvents OnRowSelected="PVRowSelectedChanged" OnRowDeselected="PVRowSelectedChanged" />
						</ClientSettings>
						<MasterTableView DataKeyNames="PERSON_ID" ExpandCollapseColumn-Visible="false">
							<Columns>
								<telerik:GridTemplateColumn ItemStyle-Width="45%">
									<ItemTemplate>
										<asp:Label ID="lblContact" runat="server" Font-Size="8" Text='<%#Capitalize((string)Eval("FIRST_NAME")) + " " + Capitalize((string)Eval("LAST_NAME")) %>'></asp:Label>
                                        <div style="float: right;">
                                            <asp:Label ID="lblConfirmed" runat="server" Font-Size="8" ForeColor="Red" BackColor="Wheat" BorderColor="Wheat" BorderWidth="2" Text="Confirmed" Visible="false"></asp:Label>
                                        </div>
                                    </ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn ItemStyle-Width="45%">
									<ItemTemplate>
										<asp:Label ID="lblEmail" runat="server" Font-Size="8" Text='<%#Eval("JOB_TITLE")%>'></asp:Label>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridClientSelectColumn ItemStyle-Width="10%">
								</telerik:GridClientSelectColumn>
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>

                    <asp:Panel ID="pnlComments" runat="server" Visible="false">
                        <div style="padding: 2px 7px 7px;">
                        <p>Comments:</p>
                        <telerik:RadGrid runat="server" ID="rgPlantComments" Name="rgPlantComments"
						    Skin="Metro" AutoGenerateColumns="false" CellPadding="3"
						    GridLines="None" AllowSorting="false" ShowHeader="false" Width="100%">
						    <MasterTableView ExpandCollapseColumn-Visible="false">
							    <Columns>
								    <telerik:GridTemplateColumn ItemStyle-Width="45%">
									    <ItemTemplate>
										    <asp:Label ID="lblContact" runat="server" Font-Size="8" Text='<%#Eval("PersonName")%>'></asp:Label>
                                        </ItemTemplate>
								    </telerik:GridTemplateColumn>
								    <telerik:GridTemplateColumn ItemStyle-Width="45%">
									    <ItemTemplate>
										    <asp:Label ID="lblComment" runat="server" Font-Size="8" Text='<%#Eval("CommentText")%>'></asp:Label>
									    </ItemTemplate>
								    </telerik:GridTemplateColumn>
								    <telerik:GridTemplateColumn ItemStyle-Width="45%">
									    <ItemTemplate>
										    <asp:Label ID="lblComment" runat="server" Font-Size="8" Text='<%#((DateTime)Eval("CommentDate")).ToShortDateString()%>'></asp:Label>
									    </ItemTemplate>
								    </telerik:GridTemplateColumn>
							    </Columns>
						        </MasterTableView>
					        </telerik:RadGrid>
                            </div>
                        </asp:Panel>
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	<br />
	<telerik:RadButton ID="btnSubmit" runat="server" Text="" Width="45%" Skin="Metro" OnClick="btnSubmit_Click" />
    <telerik:RadButton ID="btnClose" runat="server" Text="Close Incident Verification" Width="45%" Skin="Metro" Visible="false" OnClick="btnClose_Click" />
	<asp:Label ID="lblResults" runat="server" />
</asp:Panel>
