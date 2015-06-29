<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSList.ascx.cs" Inherits="SQM.Website.Ucl_EHSList" %>

<%@ Register Src="~/Include/Ucl_MetricList.ascx" TagName="MetricList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

  <asp:Panel ID="pnlProfileSelectHdr" runat="server" style="margin-top: 4px;" Visible = "false">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
                <td id="tdBusLocSelect" runat="server" class=summaryData valign=top width="25%">
				    <SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblBusLocSelectHdr" Text="Business Location"></asp:Label>
                    </SPAN>
                    <BR>
                    <telerik:RadComboBox ID="ddlBusLocSelect" runat="server" Skin="Metro" Width=250 ZIndex=9000 Font-Size=Small  ToolTip="View or modify Metric Profile for the selected location"
                         AutoPostBack="true" OnSelectedIndexChanged="LocationSelect_Click"></telerik:RadComboBox>
                    <telerik:RadMenu ID="mnuBusLocSelect" runat="server" Skin="Default" Width=250 style="z-index: 2900" EnableAutoScroll="true"  DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="LocationSelect_Click"></telerik:RadMenu>
                    <asp:HiddenField id="hfBusLocProfileUndefined" runat="server" Value=" (undefined profiles are highlighted in red)"/>
		        </td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocCodeHdrPlant" Text="Location Code" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocCodePlant_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocationType" Text="Location Type" Visible="true"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocationType_out" Text="" Visible="true"></asp:Label>
				</td>
                <td class=summaryData valign=top>
				    <SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblProfileUpdateHdr" Text="Last Update"></asp:Label>
                    </SPAN>
                    <BR>
				    <asp:Label runat="server" ID="lblProfileUpdate_out"></asp:Label>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server" ID="lblProfileUpdateBy_out"></asp:Label>
			    </td>
            </tr>
        </table>
    </asp:Panel>

<asp:Panel ID="pnlProfileInputHdr" runat="server" style="margin-top: 4px;" Visible = "false">
    <table cellspacing=0 cellpadding=2 border=0 width="100%">
    	<tr>
            <td id="tdLocationSelect" runat="server" class=summaryData valign=top width="265px">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPlantSelectHdr" Text="Business Location"></asp:Label>
                </SPAN>
                <BR>
                 <telerik:RadComboBox ID="ddlPlantSelect" runat="server" Skin="Metro" Width=250 ZIndex=9000 Font-Size=Small ToolTip="View or enter metric data for the selected location"
                     AutoPostBack="true" OnSelectedIndexChanged="LocationSelect_Click"></telerik:RadComboBox>
                <telerik:RadMenu ID="mnuPlantSelect" runat="server" Skin="Default" Width=250 style="z-index: 2900" EnableAutoScroll="true" DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="LocationSelect_Click"></telerik:RadMenu>
                <asp:HiddenField id="hfPlantProfileUndefined" runat="server" Value=" (undefined profiles are not enabled for selection)"/>
		    </td>
            <td id="tdLocation" runat="server" class=summaryData valign=top  width="265px;">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPlantNameHdr" Text="Business Location"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblPlantName_out" Text=""></asp:Label>
		    </td>
            <td id="tdPeriodSelect" runat="server" class=summaryData valign=top  width="170px" visible="false">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPeriodSelectHdr" Text="Reporting Month"></asp:Label>
                </SPAN>
                <BR>
                <telerik:RadMonthYearPicker ID="radPeriodSelect" runat="server" OnSelectedDateChanged="radDateSelect1Click" AutoPostBack="true" CssClass="textStd" Width=165 Skin="Metro"></telerik:RadMonthYearPicker>
            </td>
            <td id="tdPeriod" runat="server" class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblInputPeriodHdr" Text="Reporting Month"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblPeriodFrom_out"></asp:Label>
			</td>
            <td class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblDueDateHdr" Text="Due Date"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblDueDate_out" style="vertical-align: middle;"></asp:Label>
			</td>
            <td class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblInputStatuseHdr" Text="Input Status"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblInputStatus1_out" Text="0"></asp:Label>
                <asp:Label runat="server" ID="lblInputStatusOf_out" Text = " of "></asp:Label>
                <asp:Label runat="server" ID="lblInputStatus2_out" Text="0"></asp:Label>
                <asp:Label runat="server" ID="lblInputStatus3_out" Text=" Required Metrics Entered"></asp:Label>
                <asp:PlaceHolder ID="phRateStatus" runat="server" Visible="false">
                   <br />
                    <asp:Label ID="lblCurrency" runat="server" CssClass="textStd"></asp:Label>
                    <asp:Label ID="lblRate" runat="server" CssClass="textStd" Text=" Exchange Rate Recorded On "></asp:Label>
                    <asp:Label ID="lblRateStatus" runat="server" CssClass="textStd"></asp:Label>
                </asp:PlaceHolder>
			</td>
            <td class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblLastUpdateHdr" Text="Last Update"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblLastUpdate_out"></asp:Label>
                &nbsp;
                <asp:Label runat="server" ID="lblLastUpdateBy_out"></asp:Label>
			</td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlPeriodHdr" runat="server" Visible = "false">
    <table cellspacing=0 cellpadding=2 border=0 width="100%">
    	<tr>
            <td id="tdPlantFilter" runat="server" class=summaryData valign=top width="35%">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPlantFilterHdr" Text="Business Location"></asp:Label>
                </SPAN>
                <BR>
                 <telerik:RadComboBox ID="ddlPlantFilter" runat="server" Skin="Metro" Width=250 ZIndex=9000 Font-Size=Small  CheckBoxes="true" EnableCheckAllItemsCheckBox="true" AutoPostBack ="false" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>
		    </td>
            <td id="Td1" runat="server" class=summaryData valign=top  width="65%">
                <span id="divFromDate" runat="server">
				    <span CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblDateSelect1" Text="Reporting Month"></asp:Label>
                    </span>
                    <BR>
				    <telerik:RadMonthYearPicker ID="radDateSelect0" runat="server" CssClass="textStd" Width=165 Skin="Metro"></telerik:RadMonthYearPicker>
                </span>
                <span id="divToDate" runat="server">
                   <span CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblDateSelect2" Text="Reporting Month"></asp:Label>
                    </span>
                    <BR>
				    <telerik:RadMonthYearPicker ID="radDateSelect1" runat="server" CssClass="textStd" Width=165 Skin="Metro"></telerik:RadMonthYearPicker>
                    &nbsp;
                    <asp:Label ID="lblToDate" runat="server" Text="To" CssClass="prompt"></asp:Label>
                    &nbsp;
                    <telerik:RadMonthYearPicker ID="radDateSelect2" runat="server" CssClass="textStd" Width=165 Skin="Metro" ></telerik:RadMonthYearPicker>
                </span>
                &nbsp;&nbsp;
                <asp:Button id="btnSearchMetrics" runat="server" text="Search" ToolTip="search data" CssClass="buttonEmphasis" onclick="btnSearchMetricsClick"/>
                 <asp:Button ID="btnCalculate" runat="server" Text="Calculate Target Metrics" Visible="false" ToolTip="Calculate YTD corporate metrics per the selected reporting month"  style="margin-left: 30px;" OnClientClick="return confirmAction('Update metric calculations');"  onclick="btnCalculate_Click"></asp:Button>
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlEHSItemHdr" runat="server" Visible = "false">
    <table id="tblEHSItemHdr" runat="server" cellspacing=0 cellpadding=1 border=0 width="99%" class="">
        <tr>
			<td class="columnHeader"  width="30%">
                    <asp:Label runat="server" ID="lblProfilePlant" cssclass="prompt" Text="Business Location" ></asp:Label>
            </td>
            <td class="tableDataAlt" width="70%">
                <asp:Label runat="server" ID="lblProfilePlant_out"></asp:Label>
			</td>
        </tr>
        <tr id="trEHSIncident" runat="server">
             <td class="columnHeader" >
				<asp:Label runat="server" ID="lblDescription" text="Incident"></asp:Label>
			</td>
            <td class="tableDataAlt">
                  <asp:Label runat="server" ID="lblIncidentID_out"></asp:Label>
                    &nbsp;-&nbsp;
                <asp:Label runat="server" ID="lblDescription_out" ></asp:Label>
            </td>
        </tr>
        <tr id="trEHSInputPeriod" runat="server">
             <td class="columnHeader">
				<asp:Label runat="server" ID="lblInputPeriod" text="Reporting Month"></asp:Label>
			</td>
            <td class="tableDataAlt">
                <asp:Label runat="server" ID="lblInputPeriod_out" ></asp:Label>
            </td>
        </tr>
        <tr id="trEHSInput" runat="server">
             <td class="columnHeader">
				<asp:Label runat="server" ID="lblInput" text="Inputs"></asp:Label>
			</td>
            <td class="tableDataAlt">
                <asp:Label runat="server" ID="lblInput_out" ></asp:Label>
            </td>
        </tr>
        <tr>
           <td class="columnHeader">
                <asp:Label runat="server" ID="lblResponsible" cssclass="prompt" Text="Responsible" ></asp:Label>
            </td>
            <td class="tableDataAlt">
                    <asp:Label runat="server" ID="lblResponsible_out" ></asp:Label>
			</td>
        </tr>
 
    </table>
</asp:Panel>

<asp:Panel ID="pnlProdList" runat="server" Visible="false" style="margin-top: 4px;">
    <asp:GridView runat="server" ID="gvProdList" Name="gvProdList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  cellpadding="1" GridLines="Both" PageSize="20" Width="70%" >
        <HeaderStyle CssClass="HeadingCellTextLeftSmall" />    
        <RowStyle CssClass="textSmall" />
        <Columns>
            <asp:TemplateField HeaderText="Data Name" ItemStyle-Width="60%">
			    <ItemTemplate>
                    <asp:Label runat="server" ID="lblProdField" Text='<%# Eval("Key") %>'></asp:Label>
                </ItemTemplate>
		    </asp:TemplateField>
                <asp:TemplateField HeaderText="Value" ItemStyle-Width="40%">
			    <ItemTemplate>
                    <asp:Label runat="server" ID="lblProdValue" Text='<%# Eval("Value") %>'></asp:Label>
                </ItemTemplate>
		    </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>


<asp:Panel ID="pnlProfilePeriodList" runat="server" Visible="false" Width="100%" style="margin-top: 4px;">
    <telerik:RadGrid ID="rgProfilePeriodList" runat="server" Skin="Metro" AllowSorting="false" AllowPaging="false" PageSize="20"
        AutoGenerateColumns="false" OnItemDataBound="rgProfilePeriodList_ItemDataBound" GridLines="None" Width="100%">
        <MasterTableView ExpandCollapseColumn-Visible="false">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Reporting Period" ItemStyle-Width="150px" ShowSortIcon="false">
                    <ItemTemplate>
                         <asp:LinkButton ID="lnkPeriod" OnClick="lnkPeriod_Click" CssClass="buttonLink" runat="server" ToolTip="select period">
                         </asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Inputs" HeaderText="Inputs" ShowSortIcon="false">
                    <ItemTemplate>
                        <Ucl:MetricList id="uclInputsList" runat="server"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Panel>

<asp:Panel ID="pnlMessage" runat="server" Visible = "false" style="margin-top: 6px;">
	<asp:Label runat="server" ID="lblMessage"></asp:Label>
</asp:Panel>
