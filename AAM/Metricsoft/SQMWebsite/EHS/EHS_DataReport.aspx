<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_DataReport.aspx.cs" Inherits="SQM.Website.EHS.EHS_DataReport" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<div class="container-fluid">
		<span class="pageTitles" style="float: left; margin-top: 6px">Health & Safety Report</span>
		<br class="clear" />
		<telerik:RadAjaxLoadingPanel ID="radLoading" runat="server" Skin="Metro" />
		<telerik:RadAjaxPanel ID="radAjaxPanel" runat="server" LoadingPanelID="radLoading">
			<br />
			<div class="container-fluid blueCell">
				<span class="prompt">End of Week: </span>
				<telerik:RadDatePicker ID="rdpEndOfWeek" runat="server" Skin="Metro" DateInput-Skin="Metro" ShowPopupOnFocus="true" OnSelectedDateChanged="rdpEndOfWeek_SelectedDateChanged"
					AutoPostBack="true">
					<Calendar FirstDayOfWeek="Monday" runat="server" />
				</telerik:RadDatePicker>
			</div>
			<br />
			<telerik:RadGrid ID="rgData" runat="server" Skin="Metro" AutoGenerateColumns="false" BorderStyle="None" Width="2500">
				<MasterTableView DataKeyNames="PlantID">
					<Columns>
						<telerik:GridBoundColumn DataField="PlantName" HeaderText="<%$ Resources:LocalizedText, Location %>" UniqueName="PlantName" />
						<telerik:GridBoundColumn DataField="PersonLastName" HeaderText="EHS Rep" />
					</Columns>
				</MasterTableView>
			</telerik:RadGrid>
		</telerik:RadAjaxPanel>
	</div>
</asp:Content>