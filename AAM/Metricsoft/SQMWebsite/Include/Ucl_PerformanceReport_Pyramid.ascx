<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PerformanceReport_Pyramid.ascx.cs" Inherits="SQM.Website.Ucl_PerformanceReport_Pyramid" %>
<%@ Register Assembly="SQMWebsite" Namespace="SQM.Website" TagPrefix="SQM" %>
<SQM:Ucl_RadGauge ID="uclChart" runat="server" />
<div style="position: relative; height: 650px">
	<SQM:AAMPyramidChart ID="pyramid" runat="server" Height="600" style="position: absolute; top: 50px; z-index: 1" />
	<table id="pyramidTable" runat="server" style="position: relative; top: 0; z-index: 0">
		<thead>
			<tr>
				<th id="pyramidTable_column1" runat="server"></th>
				<th class="pyramidTable_header" style="border: 1px solid #000">YTD</th>
				<th id="pyramidTable_columnAnnualized" runat="server" class="pyramidTable_header" style="border: 1px solid #000"></th>
				<th id="pyramidTable_columnPreviousYear" runat="server" class="pyramidTable_header" style="border: 1px solid #000"></th>
				<th class="pyramidTable_header" style="border: 1px solid #000">Variance</th>
			</tr>
		</thead>
		<tbody>
			<tr id="pyramidTable_fatalitiesRow" runat="server">
				<td class="pyramidTable_filler"></td>
				<td id="pyramidTable_fatalitiesYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_fatalitiesAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_fatalitiesPreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_fatalitiesVariance" runat="server" style="border: 1px solid #000"></td>
			</tr>
			<tr id="pyramidTable_lostTimeRow" runat="server">
				<td class="pyramidTable_filler"></td>
				<td id="pyramidTable_lostTimeYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_lostTimeAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_lostTimePreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_lostTimeVariance" runat="server" style="border: 1px solid #000"></td>
			</tr>
			<tr id="pyramidTable_recordableRow" runat="server">
				<td class="pyramidTable_filler"></td>
				<td id="pyramidTable_recordableYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_recordableAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_recordablePreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_recordableVariance" runat="server" style="border: 1px solid #000"></td>
			</tr>
			<tr id="pyramidTable_firstAidRow" runat="server">
				<td class="pyramidTable_filler"></td>
				<td id="pyramidTable_firstAidYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_firstAidAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_firstAidPreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_firstAidVariance" runat="server" style="border: 1px solid #000"></td>
			</tr>
			<tr id="pyramidTable_nearMissesRow" runat="server">
				<td class="pyramidTable_filler"></td>
				<td id="pyramidTable_nearMissesYTD" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_nearMissesAnnualized" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_nearMissesPreviousYear" runat="server" class="pyramidTable_cell" style="border: 1px solid #000"></td>
				<td id="pyramidTable_nearMissesVariance" runat="server" style="border: 1px solid #000"></td>
			</tr>
		</tbody>
	</table>
</div>
<div style="page-break-after: always; padding: 1px"></div>
<div id="divJSAsAndAudits_Pyramid" runat="server" class="chartMarginTop"></div>
<div id="divSafetyTrainingHours_Pyramid" runat="server" class="chartMarginTop"></div>