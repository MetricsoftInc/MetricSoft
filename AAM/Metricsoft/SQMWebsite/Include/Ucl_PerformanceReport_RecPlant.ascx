<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PerformanceReport_RecPlant.ascx.cs" Inherits="SQM.Website.Ucl_PerformanceReport_RecPlant" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadGrid ID="rgRecPlant" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500" OnItemDataBound="rgRecPlant_ItemDataBound" OnPreRender="rgRecPlant_PreRender">
	<ClientSettings EnableAlternatingItems="false" />
	<MasterTableView Caption="Recordable Comparison by Plant">
		<ColumnGroups>
			<telerik:GridColumnGroup Name="PerformanceResults" HeaderText="Performance Results">
				<HeaderStyle HorizontalAlign="Center" />
			</telerik:GridColumnGroup>
		</ColumnGroups>
		<Columns>
			<telerik:GridBoundColumn DataField="BusinessUnit" HeaderText="BU" UniqueName="BusinessUnit">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="Plant" HeaderText="Plant" UniqueName="Plant">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="RecPreviousYear" UniqueName="RecPreviousYear">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="RecYTD" UniqueName="RecYTD">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="RecAnnualized" HeaderText="Recordabled<br/>Annualized">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn UniqueName="ImprovedOrDeclined" ColumnGroupName="PerformanceResults">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle Font-Bold="true" Font-Size="18px" HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="PercentChange" DataFormatString="{0:P1}" HeaderText="% Change" UniqueName="PercentChange" ColumnGroupName="PerformanceResults">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
		</Columns>
	</MasterTableView>
</telerik:RadGrid>