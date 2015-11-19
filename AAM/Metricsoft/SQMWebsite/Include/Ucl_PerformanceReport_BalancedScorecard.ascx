<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PerformanceReport_BalancedScorecard.ascx.cs" Inherits="SQM.Website.Ucl_PerformanceReport_BalancedScorecard" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Repeater ID="rptBalancedScorecard" runat="server" OnItemDataBound="rptBalancedScorecard_ItemDataBound">
	<HeaderTemplate>
		<telerik:RadGrid ID="rgBalancedScorescardHeader" runat="server" Skin="Metro" AutoGenerateColumns="false" OnItemDataBound="rgBalancedScorescardHeader_ItemDataBound">
			<MasterTableView TableLayout="Fixed">
				<NoRecordsTemplate></NoRecordsTemplate>
				<ColumnGroups>
					<telerik:GridColumnGroup HeaderText="Above Target" Name="Target">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="Red" ForeColor="White" />
					</telerik:GridColumnGroup>
					<telerik:GridColumnGroup HeaderText="Year" Name="Year">
						<HeaderStyle Font-Size="X-Large" HorizontalAlign="Center" BackColor="White" />
					</telerik:GridColumnGroup>
				</ColumnGroups>
				<Columns>
					<telerik:GridBoundColumn HeaderText="Target or Better" UniqueName="Target" ColumnGroupName="Target">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="Green" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="JAN" UniqueName="Month1" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="FEB" UniqueName="Month2" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="MAR" UniqueName="Month3" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="APR" UniqueName="Month4" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="MAY" UniqueName="Month5" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="JUN" UniqueName="Month6" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="JUL" UniqueName="Month7" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="AUG" UniqueName="Month8" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="SEP" UniqueName="Month9" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="OCT" UniqueName="Month10" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="NOV" UniqueName="Month11" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="DEC" UniqueName="Month12" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#99FF99" Width="100" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="YTD" UniqueName="YTD" ColumnGroupName="Year">
						<HeaderStyle Font-Bold="true" HorizontalAlign="Center" BackColor="#FFFF99" Width="100" />
					</telerik:GridBoundColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</HeaderTemplate>
	<ItemTemplate>
		<telerik:RadGrid ID="rgBalancedScorecardItem" runat="server" Skin="Metro" AutoGenerateColumns="false" ShowHeader="false"
			OnItemDataBound="rgBalancedScorecardItem_ItemDataBound">
			<ClientSettings EnableAlternatingItems="false" />
			<MasterTableView TableLayout="Fixed">
				<Columns>
					<telerik:GridBoundColumn DataField="ItemType" UniqueName="ItemType" />
					<telerik:GridBoundColumn DataField="Target" DataFormatString="{0:F1}" UniqueName="Target">
						<HeaderStyle Width="50" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Jan" DataFormatString="{0:F1}" UniqueName="Month1">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Feb" DataFormatString="{0:F1}" UniqueName="Month2">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Mar" DataFormatString="{0:F1}" UniqueName="Month3">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Apr" DataFormatString="{0:F1}" UniqueName="Month4">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="May" DataFormatString="{0:F1}" UniqueName="Month5">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Jun" DataFormatString="{0:F1}" UniqueName="Month6">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Jul" DataFormatString="{0:F1}" UniqueName="Month7">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Aug" DataFormatString="{0:F1}" UniqueName="Month8">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Sep" DataFormatString="{0:F1}" UniqueName="Month9">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Oct" DataFormatString="{0:F1}" UniqueName="Month10">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Nov" DataFormatString="{0:F1}" UniqueName="Month11">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Dec" DataFormatString="{0:F1}" UniqueName="Month12">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="YTD" DataFormatString="{0:F1}" UniqueName="YTD">
						<HeaderStyle Width="100" />
						<ItemStyle Font-Bold="true" HorizontalAlign="Center" ForeColor="White" />
					</telerik:GridBoundColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</ItemTemplate>
	<SeparatorTemplate>
		<div style="height: 5px; background-color: #999; border: 1px solid #e5e5e5"></div>
	</SeparatorTemplate>
</asp:Repeater>