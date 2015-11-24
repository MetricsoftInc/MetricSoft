<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PerformanceReport_Metrics.ascx.cs" Inherits="SQM.Website.Ucl_PerformanceReport_Metrics" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="SQMWebsite" Namespace="SQM.Website" TagPrefix="SQM" %>
<Ucl:RadGauge ID="uclChart" runat="server" />
<div id="divTitle" runat="server" style="font-size: large; font-weight: bold; text-align: center" class="divTitleMetrics"></div>
<br />
<telerik:RadGrid ID="rgReport" runat="server" Skin="Metro" AutoGenerateColumns="false" Width="1500" OnItemDataBound="rgReport_ItemDataBound" OnPreRender="rgReport_PreRender">
	<AlternatingItemStyle BackColor="Transparent" />
	<MasterTableView>
		<ColumnGroups>
			<telerik:GridColumnGroup Name="None1" />
			<telerik:GridColumnGroup Name="None2" />
			<telerik:GridColumnGroup Name="Incidents" HeaderText="Incidents" />
			<telerik:GridColumnGroup Name="Frequency" HeaderText="Frequency" />
			<telerik:GridColumnGroup Name="Restricted" HeaderText="Restricted" />
			<telerik:GridColumnGroup Name="Severity" HeaderText="Severity" />
		</ColumnGroups>
		<Columns>
			<telerik:GridBoundColumn DataField="Month" HeaderText="Year" />
			<telerik:GridBoundColumn DataField="TRIR" DataFormatString="{0:F1}" HeaderText="TRIR" ColumnGroupName="None1" />
			<telerik:GridBoundColumn DataField="FrequencyRate" DataFormatString="{0:F1}" HeaderText="Frequency<br>Rate" ColumnGroupName="None1" />
			<telerik:GridBoundColumn DataField="SeverityRate" DataFormatString="{0:F1}" HeaderText="Severity<br>Rate" ColumnGroupName="None1" />
			<telerik:GridBoundColumn DataField="ManHours" DataFormatString="{0:N0}" HeaderText="Man-hours" UniqueName="ManHours" ColumnGroupName="None1" />
			<telerik:GridBoundColumn DataField="Incidents" HeaderText="Total<br>Recordable<br>Cases" UniqueName="Incidents" ColumnGroupName="Incidents" />
			<telerik:GridBoundColumn DataField="Frequency" HeaderText="Total Lost<br>Time Cases" UniqueName="Frequency" ColumnGroupName="Frequency" />
			<telerik:GridBoundColumn DataField="Restricted" HeaderText="Total<br>Days<br>Restricted" UniqueName="Restricted" ColumnGroupName="Restricted" />
			<telerik:GridBoundColumn DataField="Severity" HeaderText="Total Lost<br>Time Days" UniqueName="Severity" ColumnGroupName="Severity" />
			<telerik:GridBoundColumn DataField="FirstAid" HeaderText="First Aid<br>Cases" UniqueName="FirstAid" ColumnGroupName="None2" />
			<telerik:GridBoundColumn DataField="Leadership" HeaderText="Leadership<br>Safety<br>Walks" UniqueName="Leadership" ColumnGroupName="None2" />
			<telerik:GridBoundColumn DataField="JSAs" HeaderText="JSAs<br>Completed" UniqueName="JSAs" ColumnGroupName="None2" />
			<telerik:GridBoundColumn DataField="SafetyTraining" HeaderText="Total Safety<br>Training<br>Hours" UniqueName="SafetyTraining" ColumnGroupName="None2" />
		</Columns>
	</MasterTableView>
</telerik:RadGrid>
<div id="divTRIR" runat="server" class="chartMarginTop"></div>
<div style="page-break-after: always; padding: 1px"></div>
<div id="divFrequencyRate" runat="server" class="chartMarginTop"></div>
<div id="divSeverityRate" runat="server" class="chartMarginTop"></div>
<div style="page-break-after: always; padding: 1px"></div>
<div id="divPie1" runat="server" style="overflow: hidden" class="chartMarginTop">
	<SQM:PieChart ID="pieRecordableType" runat="server" Title="Recordable Injuries by Type" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
	<SQM:PieChart ID="pieRecordableBodyPart" runat="server" Title="Recordable Injuries by Body Part" Width="740" Height="500" StartAngle="45" Style="float: right"
		CssClass="pieChart" />
</div>
<div id="divPie2" runat="server" style="overflow: hidden" class="chartMarginTop">
	<SQM:PieChart ID="pieRecordableRootCause" runat="server" Title="Injury Root Causes" Width="740" Height="500" StartAngle="45" Style="float: left" CssClass="pieChart" />
	<SQM:PieChart ID="pieRecordableTenure" runat="server" Title="Tenure of Injured Associate" Width="740" Height="500" StartAngle="45" Style="float: right" CssClass="pieChart" />
</div>
<div id="divBreakPie" runat="server" style="page-break-after: always; padding: 1px"></div>
<div id="divPie3" runat="server" style="overflow: hidden" class="chartMarginTop">
	<SQM:PieChart ID="pieRecordableDaysToClose" runat="server" Title="Days to Close Investigations" Width="740" Height="500" StartAngle="45" CssClass="pieChart" />
</div>
<div style="overflow: hidden" class="chartMarginTop">
	<div id="divJSAsAndAudits_Metrics" runat="server" style="float: left"></div>
	<div id="divSafetyTrainingHours_Metrics" runat="server" style="float: right"></div>
</div>