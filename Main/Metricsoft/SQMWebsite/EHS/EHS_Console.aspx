<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_Console.aspx.cs" Inherits="SQM.Website.EHS_Console" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IncidentList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSIncidentDetails.ascx" TagName="IncidentDetails" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_EHSList.ascx" TagName="EHSList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_MetricList.ascx" TagName="MetricList" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<%@ Register src="~/Include/Ucl_Progress.ascx" TagName="Progress" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSReport.ascx" TagName="GHGReport" TagPrefix="Ucl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">


  <script type="text/javascript">
      function setFocus(sender, eventArgs) {
      }

      function popup(mylink, windowname) {
          if (!window.focus) return true;
          var href;
          if (typeof (mylink) == 'string')
              href = mylink;
          else
              href = mylink.href;
          window.open(href, windowname, 'width=400,height=200,scrollbars=yes');
          return false;
      }
  </script>

 <div class="admin_tabs">
    <table width="100%" border="0" cellspacing="0" cellpadding="1">
        <tr>
            <td class="tabActiveTableBg" colspan="10"> <%-- align="center">--%>
			   <%-- <BR/>--%>
                <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:HiddenField id="hfPeriodSelect" runat="server" Value="--select--"/>
                    <asp:HiddenField ID="hfActiveTab" runat="server" />

                    <table width="99%">
			            <tr>
                            <td class="pageTitles">
                                <asp:Label ID="lblTitle" runat="server" Text="Supervisory Console"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Environmental, Health and Safety data management and reporting activities."></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />

                    <div id="divWorkArea" runat="server" align="center">
                        <table cellspacing=0 cellpadding=2 border=0 width="100%" style="margin-bottom: 3px;">
                            <tr>
                                <td class="summaryDataEnd" style="width: 90px;">
                                    <asp:Label ID="lblView" runat="server" Text="Activity:" CssClass="prompt"></asp:Label>
                                </td>
                                <td class="summaryDataEnd" colspan="2">
                                    
                                    <telerik:RadComboBox ID="ddlReportList" runat="server" Skin="Metro" Width=350  Font-Size=Small EmptyMessage="select an activity..." autopostback="True" onselectedindexchanged="ddlReportList_Select">
				                        <Items>
                                            <telerik:RadComboBoxItem Text="" Value=""/>
                                            <telerik:RadComboBoxItem Text="Export Metric History" Value="1" ToolTip="Select locations and timespan for metric history export (to Excel)"/>
					                        <telerik:RadComboBoxItem Text="Export Incident History" Value="2" ToolTip="Select locations and timespan for incident history export (to Excel)"/>
					                        <telerik:RadComboBoxItem Text="Data Input Status" Value="3" ToolTip="Review reporting period status and finalize metrics for analysis"/>
					                        <telerik:RadComboBoxItem Text="Metrics Effective Date" Value="4" ToolTip="Specify the default date range for calculating and displaying metrics on performance dashboards"/>
					                        <telerik:RadComboBoxItem Text="GHG Emissions Report" Value="11"/>
				                            </Items>
			                        </telerik:RadComboBox>
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="phBasicCriteria" runat="server" visible="false">
                                <tr>
                                    <td class="summaryDataEnd" style="width: 90px;">
                                        <asp:Label runat="server" ID="lblExportPlantSelect"  CssClass="prompt" Text ="Locations:"></asp:Label>
                                    </td>
                                    <td class="summaryDataEnd">
                                        <telerik:RadComboBox ID="ddlExportPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex="9000" Skin="Metro"  Width="350" height="300" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>
		                            </td>
                                    <td class="summaryDataEnd">
							            <asp:Label runat="server" ID="lblExportDateSelect1" CssClass="prompt" Text="From: "></asp:Label>
									    <telerik:RadMonthYearPicker ID="radExportDateSelect1" runat="server" CssClass="textStd" Width="165" Skin="Metro"></telerik:RadMonthYearPicker>
									    &nbsp;
                                        <asp:Label ID="lblExportToDate" runat="server" CssClass="prompt" Text="To: " ></asp:Label>
									    &nbsp;
                                        <telerik:RadMonthYearPicker ID="radExportDateSelect2" runat="server" CssClass="textStd" Width="165" Skin="Metro"></telerik:RadMonthYearPicker>
                                        <asp:Button id="btnSearch" runat="server" style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="search data" OnClick="btnSearch_Click"/>
								    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phIncidentCriteria" runat="server" visible="false">
                                <tr>
                                    <td class="summaryDataEnd" style="width: 90px;">
                                        <asp:Label runat="server" ID="lblExportIncidentType" CssClass="prompt" Text="Incident Types"></asp:Label>
                                    </td>
                                    <td class="summaryDataEnd" colspan="2">
										<telerik:RadComboBox ID="ddlExportIncidentType" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="Select incident types to list" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>
                                        &nbsp;&nbsp;
                                        <asp:Label runat="server" ID="lblExportStatus" CssClass="prompt" Text="Incident Status"></asp:Label>
                                        &nbsp;
                                        <telerik:RadComboBox ID="ddlExportStatusSelect" runat="server"  ToolTip="Select incident status to list" ZIndex="9000" Skin="Metro" AutoPostBack="true">
                                            <Items>
                                                <telerik:RadComboBoxItem Text="Open" Value="A" /> 
                                                <telerik:RadComboBoxItem Text="Overdue" Value="O" /> 
                                                <telerik:RadComboBoxItem Text="Closed" Value="C" /> 
                                                <telerik:RadComboBoxItem Text="All" Value=""/> 
                                            </Items>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </table>
                               
                        <Ucl:Progress id="uclProgress" runat="server"/>
                        <Ucl:Export id="uclExport" runat="server"/>

                        <div id="divMetricsTimespan" runat="server" class="" visible="false">
                            <table cellspacing="0" cellpadding="0" border="0" width="100%" class="borderSoft"> 
                                 <tr>
                                     <td class="columnHeader" width="40%">
                                         <asp:Label ID="lblEffFrom" runat="server" Text="Effective From Month"></asp:Label>
                                     </td>
                                     <td class="tableDataAlt" width="60%">
						                <telerik:RadMonthYearPicker ID="radEffFrom" runat="server" AutoPostBack="false" CssClass="textStd" Width=165 Skin="Metro"></telerik:RadMonthYearPicker>
                                  	</td>
				                </tr>
                                <tr>
					                <td class="columnHeader" width="40%">
                                         <asp:Label ID="lblEffTo" runat="server" Text="Effective To Month"></asp:Label>
                                     </td>
                                     <td class="tableDataAlt" width="60%">
						                <telerik:RadMonthYearPicker ID="radEffTo" runat="server" AutoPostBack="false" CssClass="textStd" Width=165 Skin="Metro"></telerik:RadMonthYearPicker>
                                 	</td>
				                </tr>
				            </table>
                            <div style="float:left; margin: 5px;">
                                <asp:Button ID="btnSaveTimespan" runat="server" CSSClass="buttonEmphasis"  text="Save" onClientClick="return confirmAction('Update effective dates');"  OnClick="btnEffDateSelect" ToolTip="Update effective dates"></asp:Button>
                                <br />
                                <asp:Label ID="lblTimespanDateError" runat="server" Text="The dates selected are invalid. Please check your entries before saving." CssClass="labelEmphasis" visible="false"></asp:Label>
                            </div>
                        </div>

                        <div id="divProfilePeriodScrollRepeater" runat="server" class="" visible="false" style="margin-top: 4px;">
                            <asp:Button id="btnRollupAll" runat="server" Text="Finalize All YTD" CssClass="buttonEmphasis" style="margin: 4px;" Visible="false"  onClientClick="return confirmAction('Finalize all plants');" OnClick="btnRollupAll_Click"/>
					        <asp:Repeater runat="server" ID="rptProfile" ClientIDMode="AutoID" OnItemDataBound="rptProfile_OnItemDataBound">
						        <HeaderTemplate>
							        <table cellspacing="0" cellpadding="0" border="0" width="100%" >
						        </HeaderTemplate>
						        <ItemTemplate>
							        <tr>
								        <td id="tdLocation" runat="server" class="rptInputTableEnd" style="width: 15%; vertical-align: top; padding-top: 2px; padding-left: 3px;">
			                                <asp:Label ID="lblLocation" runat="server" CSSClass="prompt"></asp:Label>
								        </td>
                                        <td class="rptInputTable" valign="top" style="width: 85%;">
                                            <asp:Repeater runat="server" ID="rptProfileStatus" ClientIDMode="AutoID" OnItemDataBound="rptProfileStatus_OnItemDataBound">
                                                <HeaderTemplate>
				                                    <table cellspacing="1" cellpadding="1" border="0" width="100%" >
				                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td class="columnHeader" width="30%">
                                                            <asp:Label runat="server" ID="lblFinalStatusHdr" cssclass="prompt" Text="Local Approval" ></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt" width="70%">
                                                            <asp:Label ID="lblFinalStatus" runat="server" CssClass="textStd"></asp:Label>
			                                            </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label runat="server" ID="lblRateStatusHdr" cssclass="prompt" Text="Exchange Rate" ></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">
                                                             <asp:Label ID="lblRateStatus" runat="server" CssClass="textStd"></asp:Label>
			                                            </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label runat="server" ID="lblPlantInputsHdr" cssclass="prompt" Text="Safety And Accounting Data" ></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">
                                                            <Ucl:EHSList id="uclProdList" runat="server"/>
			                                            </td>
                                                    </tr>
							                        <tr>
                                                        <td class="columnHeader" width="30%">
                                                            <asp:Label runat="server" ID="lblInputsHdr" cssclass="prompt" Text="Metric Inputs" ></asp:Label>
                                                            <asp:LinkButton ID="lnkInputs" runat="server" style="float:right; margin-right: 5px;" CSSClass="buttonLink"  OnClick="lnkDisplayInputs" ToolTip="Display Metric Inputs">
                                                                 <img src="/images/defaulticon/16x16/list-ordered.png" alt="" style="vertical-align: middle; border: 0px;" />
                                                            </asp:LinkButton>
                                                        </td>
                                                        <td class="tableDataAlt" width="70%">
                                                            <asp:Label ID="lblInputs" runat="server" CssClass="textStd"></asp:Label>
                                                            &nbsp;&nbsp;&nbsp;
                                                            <asp:Label ID="lblReqdInputs" runat="server" CssClass="textStd"></asp:Label>
			                                                <asp:CheckBox id="cbInputsSelect" runat="server" Visible="false"/>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label runat="server" ID="lblRollupStatusHdr" cssclass="prompt" Text="Finalize Metrics" ></asp:Label>
                                                            <asp:LinkButton ID="lnkHistory" runat="server" CSSClass="buttonLink"  style="float:right; margin-right: 5px;" Visible="false" OnClick="lnkDisplayInputs" ToolTip="Display Metric Calculations">
                                                                <img src="/images/defaulticon/16x16/list-ordered.png" alt="" style="vertical-align: middle; border: 0px;" />
                                                            </asp:LinkButton>
                                                        </td>
                                                        <td class="tableDataAlt">
                                                            <asp:Button id="btnRollup" runat="server" Text="Finalize" ToolTip="Calculate reporting metrics per company standard UOM's and currency rates." OnClientClick="return confirmAction('finalize Metric calculations for this Business Location');"  onclick="btnRollup_Click"></asp:Button>
                                                            &nbsp;
                                                            <asp:Button id="btnRollupYTD" runat="server" Text="Finalize YTD" ToolTip="Calculate reporting metrics for the year-to-date." OnClientClick="return confirmAction('finalize year-to-date Metric calculations for this Business Location');"  onclick="btnRollup_Click"></asp:Button>
                                                            &nbsp;&nbsp;
                                                            <asp:Label ID="lblRollupStatus" runat="server" CssClass="textStd"></asp:Label>
                                                            <asp:CheckBox id="cbHistorySelect" runat="server" Visible="false"/>
			                                            </td>
                                                    </tr>
                                                    <tr style="height: 1px;">
                                                        <td colspan="2" >
                                                            <Ucl:MetricList id="uclInputsList" runat="server"/>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate></table></FooterTemplate>
                                            </asp:Repeater>
                                        </td>
							        </tr>
                                    <tr style="height: 5px;">
                                        <td colspan="5"></td>
                                    </tr>
						        </ItemTemplate>
						        <FooterTemplate>
							        </table></FooterTemplate>
					        </asp:Repeater>
				            <asp:Label runat="server" ID="lblMetricEmptyRepeater" Height="40" Text="The metric list is empty." class="GridEmpty" Visible="false"></asp:Label>
                        </div>

                        <div id="divMetricHistory" runat="server">
                            <Ucl:MetricList id="uclHistoryList" runat="server"/>
                        </div>

                        <div id="divGHGReport" runat="server" style="margin-top: 4px;">
                            <Ucl:GHGReport id="uclGHGReport" runat="server"/>
                        </div>

                    </div>

                    <br />
                </FORM>
            </td>
        </tr>
    </table>
 </div>
</asp:Content>
