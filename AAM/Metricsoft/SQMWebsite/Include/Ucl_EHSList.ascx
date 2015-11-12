<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSList.ascx.cs" Inherits="SQM.Website.Ucl_EHSList" %>

<%@ Register Src="~/Include/Ucl_MetricList.ascx" TagName="MetricList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

  <asp:Panel ID="pnlProfileSelectHdr" runat="server" style="margin-top: 4px;" Visible = "False">
        <table cellspacing=0 cellpadding=2 border=0 width="100%">
			<tr>
                <td id="tdBusLocSelect" runat="server" class=summaryData valign=top width="25%">
				    <SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblBusLocSelectHdr" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                    </SPAN>
                    <BR>
                    <telerik:RadComboBox ID="ddlBusLocSelect" runat="server" Skin="Metro" Width=250px ZIndex=9000 Font-Size=Small  ToolTip="View or modify Metric Profile for the selected location"
                         AutoPostBack="True" OnSelectedIndexChanged="LocationSelect_Click" meta:resourcekey="ddlBusLocSelectResource1"></telerik:RadComboBox>
                    <telerik:RadMenu ID="mnuBusLocSelect" runat="server" Width=250px style="z-index: 2900" EnableAutoScroll="True" OnItemClick="LocationSelect_Click"><defaultgroupsettings repeatdirection="Horizontal" /></telerik:RadMenu>
                    <asp:HiddenField id="hfBusLocProfileUndefined" runat="server" Value=" (undefined profiles are highlighted in red)"/>
		        </td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocCodeHdrPlant" Text="<%$ Resources:LocalizedText, LocationCode %>"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocCodePlant_out"></asp:Label>
				</td>
                <td class=summaryData valign=top>
					<SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblLocationType" Text="<%$ Resources:LocalizedText, LocationType %>"></asp:Label>
                    </SPAN>
                    <BR>
					<asp:Label runat="server" ID="lblLocationType_out"></asp:Label>
				</td>
                <td class=summaryData valign=top>
				    <SPAN CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblProfileUpdateHdr" Text="<%$ Resources:LocalizedText, LastUpdate %>"></asp:Label>
                    </SPAN>
                    <BR>
				    <asp:Label runat="server" ID="lblProfileUpdate_out"></asp:Label>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label runat="server" ID="lblProfileUpdateBy_out"></asp:Label>
			    </td>
            </tr>
        </table>
    </asp:Panel>

<asp:Panel ID="pnlProfileInputHdr" runat="server" style="margin-top: 4px;" Visible = "False">
    <table cellspacing=0 cellpadding=2 border=0 width="100%">
    	<tr>
            <td id="tdLocationSelect" runat="server" class=summaryData valign=top width="265px">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPlantSelectHdr" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                </SPAN>
                <BR>
                 <telerik:RadComboBox ID="ddlPlantSelect" runat="server" Skin="Metro" Width=250px ZIndex=9000 Font-Size=Small ToolTip="View or enter metric data for the selected location"
                     AutoPostBack="True" OnSelectedIndexChanged="LocationSelect_Click" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>
                <telerik:RadMenu ID="mnuPlantSelect" runat="server" Width=250px style="z-index: 2900" EnableAutoScroll="True" OnItemClick="LocationSelect_Click"><defaultgroupsettings repeatdirection="Horizontal" /></telerik:RadMenu>
                <asp:HiddenField id="hfPlantProfileUndefined" runat="server" Value=" (undefined profiles are not enabled for selection)"/>
		    </td>
            <td id="tdLocation" runat="server" class=summaryData valign=top  width="265px;">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPlantNameHdr" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblPlantName_out"></asp:Label>
		    </td>
            <td id="tdPeriodSelect" runat="server" class=summaryData valign=top  width="170px" visible="False">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPeriodSelectHdr" Text="Reporting Month" meta:resourcekey="reportingMonthResource1"></asp:Label>
                </SPAN>
                <BR>
                <telerik:RadMonthYearPicker ID="radPeriodSelect" runat="server" OnSelectedDateChanged="radDateSelect1Click" AutoPostBack="True" CssClass="textStd" Width=165px Skin="Metro"><dateinput autopostback="True" dateformat="M/d/yyyy" displaydateformat="M/d/yyyy" labelwidth="64px" width=""><emptymessagestyle resize="None" /><readonlystyle resize="None" /><focusedstyle resize="None" /><disabledstyle resize="None" /><invalidstyle resize="None" /><hoveredstyle resize="None" /><enabledstyle resize="None" /></dateinput><datepopupbutton cssclass="" hoverimageurl="" imageurl="" /><monthyearnavigationsettings dateisoutofrangemessage="<%$ Resources:LocalizedText, Cancel %>"></monthyearnavigationsettings></telerik:RadMonthYearPicker>
            </td>
            <td id="tdPeriod" runat="server" class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblInputPeriodHdr" Text="Reporting Month" meta:resourcekey="reportingMonthResource1"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblPeriodFrom_out"></asp:Label>
			</td>
            <td class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblDueDateHdr" Text="<%$ Resources:LocalizedText, DueDate %>"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblDueDate_out" style="vertical-align: middle;"></asp:Label>
			</td>
            <td class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblInputStatuseHdr" Text="Input Status" meta:resourcekey="lblInputStatuseHdrResource1"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblInputStatus1_out" Text="0"></asp:Label>
                <asp:Label runat="server" ID="lblInputStatusOf_out" Text = " of " meta:resourcekey="lblInputStatusOf_outResource1"></asp:Label>
                <asp:Label runat="server" ID="lblInputStatus2_out" Text="0"></asp:Label>
                <asp:Label runat="server" ID="lblInputStatus3_out" Text="Required Metrics Entered" meta:resourcekey="lblInputStatus3_outResource1"></asp:Label>
                <asp:PlaceHolder ID="phRateStatus" runat="server" Visible="False">
                   <br />
                    <asp:Label ID="lblCurrency" runat="server" CssClass="textStd"></asp:Label>
                    <asp:Label ID="lblRate" runat="server" CssClass="textStd" Text=" Exchange Rate Recorded On " meta:resourcekey="lblRateResource1"></asp:Label>
                    <asp:Label ID="lblRateStatus" runat="server" CssClass="textStd"></asp:Label>
                </asp:PlaceHolder>
			</td>
            <td class=summaryData valign=top>
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblLastUpdateHdr" Text="<%$ Resources:LocalizedText, LastUpdate %>"></asp:Label>
                </SPAN>
                <BR>
				<asp:Label runat="server" ID="lblLastUpdate_out"></asp:Label>
                &nbsp;
                <asp:Label runat="server" ID="lblLastUpdateBy_out"></asp:Label>
			</td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlPeriodHdr" runat="server" Visible = "False">
    <table cellspacing=0 cellpadding=2 border=0 width="100%">
    	<tr>
            <td id="tdPlantFilter" runat="server" class=summaryData valign=top width="35%">
				<SPAN CLASS=summaryHeader>
                    <asp:Label runat="server" ID="lblPlantFilterHdr" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                </SPAN>
                <BR>
                 <telerik:RadComboBox ID="ddlPlantFilter" runat="server" Skin="Metro" Width=250px ZIndex=9000 Font-Size=Small  CheckBoxes="True" EnableCheckAllItemsCheckBox="True" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>
		    </td>
            <td id="Td1" runat="server" class=summaryData valign=top  width="65%">
                <span id="divFromDate" runat="server">
				    <span CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblDateSelect1" Text="Reporting Month" meta:resourcekey="reportingMonthResource1"></asp:Label>
                    </span>
                    <BR>
				    <telerik:RadMonthYearPicker ID="radDateSelect0" runat="server" CssClass="textStd" Width=165px Skin="Metro"><dateinput dateformat="M/d/yyyy" displaydateformat="M/d/yyyy" labelwidth="64px" width=""><emptymessagestyle resize="None" /><readonlystyle resize="None" /><focusedstyle resize="None" /><disabledstyle resize="None" /><invalidstyle resize="None" /><hoveredstyle resize="None" /><enabledstyle resize="None" /></dateinput><datepopupbutton cssclass="" hoverimageurl="" imageurl="" /><monthyearnavigationsettings dateisoutofrangemessage="<%$ Resources:LocalizedText, Cancel %>"></monthyearnavigationsettings></telerik:RadMonthYearPicker>
                </span>
                <span id="divToDate" runat="server">
                   <span CLASS=summaryHeader>
                        <asp:Label runat="server" ID="lblDateSelect2" Text="Reporting Month" meta:resourcekey="reportingMonthResource1"></asp:Label>
                    </span>
                    <BR>
				    <telerik:RadMonthYearPicker ID="radDateSelect1" runat="server" CssClass="textStd" Width=165px Skin="Metro"><dateinput dateformat="M/d/yyyy" displaydateformat="M/d/yyyy" labelwidth="64px" width=""><emptymessagestyle resize="None" /><readonlystyle resize="None" /><focusedstyle resize="None" /><disabledstyle resize="None" /><invalidstyle resize="None" /><hoveredstyle resize="None" /><enabledstyle resize="None" /></dateinput><datepopupbutton cssclass="" hoverimageurl="" imageurl="" /><monthyearnavigationsettings dateisoutofrangemessage="<%$ Resources:LocalizedText, Cancel %>"></monthyearnavigationsettings></telerik:RadMonthYearPicker>
                    &nbsp;
                    <asp:Label ID="lblToDate" runat="server" Text="<%$ Resources:LocalizedText, To %>" CssClass="prompt"></asp:Label>
                    &nbsp;
                    <telerik:RadMonthYearPicker ID="radDateSelect2" runat="server" CssClass="textStd" Width=165px Skin="Metro"><dateinput dateformat="M/d/yyyy" displaydateformat="M/d/yyyy" labelwidth="64px" width=""><emptymessagestyle resize="None" /><readonlystyle resize="None" /><focusedstyle resize="None" /><disabledstyle resize="None" /><invalidstyle resize="None" /><hoveredstyle resize="None" /><enabledstyle resize="None" /></dateinput><datepopupbutton cssclass="" hoverimageurl="" imageurl="" /><monthyearnavigationsettings dateisoutofrangemessage="<%$ Resources:LocalizedText, Cancel %>"></monthyearnavigationsettings></telerik:RadMonthYearPicker>
                </span>
                &nbsp;&nbsp;
                <asp:Button id="btnSearchMetrics" runat="server" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="Search data" CssClass="buttonEmphasis" onclick="btnSearchMetricsClick" meta:resourcekey="btnSearchMetricsResource1" />
                 <asp:Button ID="btnCalculate" runat="server" Text="Calculate Target Metrics" Visible="False" ToolTip="Calculate YTD corporate metrics per the selected reporting month"  style="margin-left: 30px;" OnClientClick="return confirmAction('Update metric calculations');"  onclick="btnCalculate_Click" meta:resourcekey="btnCalculateResource1"></asp:Button>
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlEHSItemHdr" runat="server" Visible = "False">
    <table id="tblEHSItemHdr" runat="server" cellspacing=0 cellpadding=1 border=0 width="99%" class="">
        <tr runat="server">
			<td class="columnHeader"  width="30%" runat="server">
                    <asp:Label runat="server" ID="lblProfilePlant" cssclass="prompt" Text="<%$ Resources:LocalizedText, BusinessLocation %>" ></asp:Label>
            </td>
            <td class="tableDataAlt" width="70%" runat="server">
                <asp:Label runat="server" ID="lblProfilePlant_out"></asp:Label>
			</td>
        </tr>
        <tr id="trEHSIncident" runat="server">
             <td class="columnHeader" runat="server" >
				<asp:Label runat="server" ID="lblDescription" text="<%$ Resources:LocalizedText, Incident %>"></asp:Label>
			</td>
            <td class="tableDataAlt" runat="server">
                  <asp:Label runat="server" ID="lblIncidentID_out"></asp:Label>
                    &nbsp;-&nbsp;
                <asp:Label runat="server" ID="lblDescription_out" ></asp:Label>
            </td>
        </tr>
        <tr id="trEHSInputPeriod" runat="server">
             <td class="columnHeader" runat="server">
				<asp:Label runat="server" ID="lblInputPeriod" text="Reporting Month"></asp:Label>
			</td>
            <td class="tableDataAlt" runat="server">
                <asp:Label runat="server" ID="lblInputPeriod_out" ></asp:Label>
            </td>
        </tr>
        <tr id="trEHSInput" runat="server">
             <td class="columnHeader" runat="server">
				<asp:Label runat="server" ID="lblInput" text="Inputs"></asp:Label>
			</td>
            <td class="tableDataAlt" runat="server">
                <asp:Label runat="server" ID="lblInput_out" ></asp:Label>
            </td>
        </tr>
        <tr runat="server">
           <td class="columnHeader" runat="server">
                <asp:Label runat="server" ID="lblResponsible" cssclass="prompt" Text="Responsible" ></asp:Label>
            </td>
            <td class="tableDataAlt" runat="server">
                    <asp:Label runat="server" ID="lblResponsible_out" ></asp:Label>
			</td>
        </tr>

    </table>
</asp:Panel>

<asp:Panel ID="pnlProdList" runat="server" Visible="False" style="margin-top: 4px;">
    <asp:GridView runat="server" ID="gvProdList" Name="gvProdList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="False"  cellpadding="1" PageSize="20" Width="70%">
        <Columns>
			<asp:TemplateField HeaderText="Data Name" meta:resourcekey="TemplateFieldResource1">
				<ItemTemplate>
					<asp:Label ID="lblProdField" runat="server" Text='<%# Eval("Key") %>'></asp:Label>
				</ItemTemplate>
				<ItemStyle Width="60%" />
			</asp:TemplateField>
			<asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Value %>">
				<ItemTemplate>
					<asp:Label ID="lblProdValue" runat="server" Text='<%# Eval("Value") %>'></asp:Label>
				</ItemTemplate>
				<ItemStyle Width="40%" />
			</asp:TemplateField>
		</Columns>
        <HeaderStyle CssClass="HeadingCellTextLeftSmall" />
        <RowStyle CssClass="textSmall" />
    </asp:GridView>
</asp:Panel>


<asp:Panel ID="pnlProfilePeriodList" runat="server" Visible="False" Width="100%" style="margin-top: 4px;">
    <telerik:RadGrid ID="rgProfilePeriodList" runat="server" Skin="Metro" PageSize="20"
        AutoGenerateColumns="False" OnItemDataBound="rgProfilePeriodList_ItemDataBound" Width="100%" GroupPanelPosition="Top">
        <MasterTableView>
            <ExpandCollapseColumn Visible="False">
			</ExpandCollapseColumn>
			<Columns>
				<telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Reporting Period" meta:resourcekey="GridTemplateColumnResource1" ShowSortIcon="False" UniqueName="TemplateColumn">
					<ItemTemplate>
						<asp:LinkButton ID="lnkPeriod" runat="server" CssClass="buttonLink" meta:resourcekey="lnkPeriodResource1" OnClick="lnkPeriod_Click" ToolTip="Select period"></asp:LinkButton>
					</ItemTemplate>
					<ItemStyle Width="150px" />
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn FilterControlAltText="Filter Inputs column" HeaderText="Inputs" meta:resourcekey="GridTemplateColumnResource2" ShowSortIcon="False" UniqueName="Inputs">
					<ItemTemplate>
						<Ucl:MetricList ID="uclInputsList" runat="server" />
					</ItemTemplate>
				</telerik:GridTemplateColumn>
			</Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Panel>

<asp:Panel ID="pnlMessage" runat="server" Visible = "False" style="margin-top: 6px;">
	<asp:Label runat="server" ID="lblMessage"></asp:Label>
</asp:Panel>
