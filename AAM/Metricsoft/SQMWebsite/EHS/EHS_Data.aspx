<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_Data.aspx.cs" Inherits="SQM.Website.EHS.EHS_Data" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.dataHeader
		{
			padding-bottom: 3px;
			line-height: 1.5em;
		}
	</style>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<div class="container-fluid">
		<span class="pageTitles" style="float: left; margin-top: 6px">Health & Safety Data Input</span>
		<br class="clear" />
		<span class="instructTextFloat">Input Health & Safety values below.</span>
		<br class="clear" />
		<telerik:RadAjaxLoadingPanel ID="radLoading" runat="server" Skin="Metro" />
		<telerik:RadAjaxPanel ID="radAjaxPanel" runat="server" LoadingPanelID="radLoading">
			<div class="container-fluid blueCell">
				<telerik:RadComboBox ID="rcbPlant" runat="server" Skin="Metro" Height="350" Width="400" OnSelectedIndexChanged="rcbPlant_SelectedIndexChanged" AutoPostBack="true"
					CausesValidation="false" />
				<br /><br />
				<span class="prompt">End of Week: </span>
				<telerik:RadDatePicker ID="rdpEndOfWeek" runat="server" Skin="Metro" DateInput-Skin="Metro" ShowPopupOnFocus="true" OnSelectedDateChanged="rdpEndOfWeek_SelectedDateChanged"
					AutoPostBack="true" DateInput-CausesValidation="false">
					<Calendar FirstDayOfWeek="Monday" runat="server" />
				</telerik:RadDatePicker>
			</div>
			<asp:HiddenField ID="hfDaysOfWeek" runat="server" />
			<br />
			<telerik:RadGrid ID="rgData" runat="server" AutoGenerateColumns="false" BorderStyle="None">
				<MasterTableView DataKeyNames="MEASURE_ID">
					<Columns>
						<telerik:GridBoundColumn DataField="MEASURE_NAME">
							<ItemStyle CssClass="tanCell" Width="30%" Font-Bold="true" />
						</telerik:GridBoundColumn>
						<telerik:GridBoundColumn DataField="DATA_TYPE" UniqueName="DataType">
							<HeaderStyle CssClass="displayNone" />
							<ItemStyle CssClass="displayNone" />
						</telerik:GridBoundColumn>
						<telerik:GridTemplateColumn UniqueName="gtcMon">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbMon" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpMon" runat="server" ControlToValidate="rtbMon" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn UniqueName="gtcTue">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbTue" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpTue" runat="server" ControlToValidate="rtbTue" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn UniqueName="gtcWed">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbWed" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpWed" runat="server" ControlToValidate="rtbWed" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn UniqueName="gtcThu">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbThu" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpThu" runat="server" ControlToValidate="rtbThu" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn UniqueName="gtcFri">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbFri" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpFri" runat="server" ControlToValidate="rtbFri" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn UniqueName="gtcSat">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbSat" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpSat" runat="server" ControlToValidate="rtbSat" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
						<telerik:GridTemplateColumn UniqueName="gtcSun">
							<ItemTemplate>
								<telerik:RadTextBox ID="rtbSun" runat="server" Skin="Metro" Width="100" />
								<asp:CompareValidator ID="cmpSun" runat="server" ControlToValidate="rtbSun" Display="Dynamic" Text="*" ForeColor="Red" Operator="DataTypeCheck" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" CssClass="dataHeader" />
							<ItemStyle CssClass="greyCell" Width="10%" HorizontalAlign="Center" />
						</telerik:GridTemplateColumn>
					</Columns>
				</MasterTableView>
			</telerik:RadGrid>
			<br />
			<telerik:RadButton ID="btnSave" runat="server" Text="Save" Skin="Metro" OnClick="btnSave_Click" />
			<asp:Label ID="lblSaved" runat="server" Text="Data saved!" ForeColor="Green" Font-Size="1.5em" Font-Bold="true" style="padding-left: 10px" Visible="false" />
		</telerik:RadAjaxPanel>
	</div>
</asp:Content>