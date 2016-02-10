<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PerformanceReport_TRIRPlant.ascx.cs" Inherits="SQM.Website.Ucl_PerformanceReport_TRIRPlant" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadGrid ID="rgTRIRPlant" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500" OnItemDataBound="rgTRIRPlant_ItemDataBound" OnPreRender="rgTRIRPlant_PreRender">
	<ClientSettings EnableAlternatingItems="false" />
	<MasterTableView Caption="TRIR Comparison by Plant">
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
			<telerik:GridBoundColumn DataField="TRIRGoal" DataFormatString="{0:F1}" HeaderText="TRIR Goal">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="TRIR2YearsAgo" DataFormatString="{0:F1}" UniqueName="TRIR2YearsAgo">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="TRIRPreviousYear" DataFormatString="{0:F1}" UniqueName="TRIRPreviousYear">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="TRIRYTD" DataFormatString="{0:F1}" UniqueName="TRIRYTD">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn UniqueName="ImprovedOrDeclined" ColumnGroupName="PerformanceResults">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle Font-Bold="true" Font-Size="18px" HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="PercentChange" DataFormatString="{0:P1}" UniqueName="PercentChange" ColumnGroupName="PerformanceResults">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
			<telerik:GridBoundColumn DataField="ProgressToGoal" DataFormatString="{0:P1}" HeaderText="Progress<br/>to Goal" UniqueName="ProgressToGoal"
				ColumnGroupName="PerformanceResults">
				<HeaderStyle HorizontalAlign="Center" />
				<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
			</telerik:GridBoundColumn>
		</Columns>
	</MasterTableView>
</telerik:RadGrid>