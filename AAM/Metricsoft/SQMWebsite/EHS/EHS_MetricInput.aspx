<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_MetricInput.aspx.cs" Inherits="SQM.Website.EHS_MetricInput" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_EHSList.ascx" TagName="EHSList" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

  <script type="text/javascript">

      window.onload = function () {
          var timeout = document.getElementById('hfTimeout').value;
          var timeoutWarn = ((parseInt(timeout)-2) * 60000);
          window.setTimeout(function() {alert("Your Session Will Timeout In Approximately 2 Minutes.  Please save your work or your changes will be lost.")}, timeoutWarn);
      }

      function setFocus(sender, eventArgs) {
          var selectedDate1 = null;
          var selectedDate2 = null;
          var thisID = sender.get_id();
          var focusID;
          if (thisID.indexOf('radDateFrom') > -1) {
              var dtp1 = $find(thisID);
              selectedDate1 = dtp1.get_selectedDate();
              if (selectedDate1 != null) {
                     InputChanged(dtp1);
                     focusID = thisID.replace('radDateFrom', 'radDateTo_dateInput');
                     document.getElementById(focusID).focus();
                }
          }
          else {
              var dtp2 = $find(thisID);
              selectedDate2 = dtp2.get_selectedDate();
              if (selectedDate2 != null) {
                  var dt1ID = focusID = thisID.replace('radDateTo', 'radDateFrom');
                  var dtp1 = $find(dt1ID);
                    selectedDate1 = dtp1.get_selectedDate();
                    if (selectedDate2 < selectedDate1) {
                        alert('Invoice From date should be less than invoice To date');
                        dtp2.set_selectedDate(null);
                    }
                    else {
                        InputChanged(dtp1);
                        focusID = thisID.replace('radDateTo', 'tbMetricValue');
                        document.getElementById(focusID).focus();
                    }
              }
          }
      }

      function CheckInputDelete(cbId) {
         // alert(document.getElementById(cbId).checked);
          var numDelete = parseInt(document.getElementById('hfNumDelete').value);
          if (document.getElementById(cbId).checked == true) {
              numDelete += 1;
              document.getElementById(cbId).parentElement.parentElement.style.backgroundColor = 'Yellow';
          }
          else {
              numDelete -= 1;
              document.getElementById(cbId).parentElement.parentElement.style.backgroundColor = 'White';
          }
          document.getElementById('hfNumDelete').value = Math.max(0,numDelete).toString();
      }

      function ValidateInputs() {
          var status = true;
          var numDelete = parseInt(document.getElementById('hfNumDelete').value);
          var numChanged = parseInt(document.getElementById('hfNumChanged').value);
          if (numDelete > 0) {
              status = confirm('Some Metric Inputs have been marked for deletion. Do you wish to continue saving ?');
          }
          else if (numChanged > 0) {
              status = confirm('This reporting month was previously approved, however some Metric Inputs have now been changed. Do you wish to continue saving ?');
          }
          else {
              status = confirmChange('Metric Inputs');
          }

          return status;
      }

      function InputChanged(tb) {
          if (document.getElementById('hfWasApproved').value.length > 0)
          {
              var numChanged = (parseInt(document.getElementById('hfNumChanged').value) + 1);
              document.getElementById('hfNumChanged').value = Math.max(0, numChanged).toString();
          }
      }

  </script>

 <div class="admin_tabs">
     <telerik:RadCalendar ID="sharedCalendar" Visible="false" runat="server"></telerik:RadCalendar>
    <table width="100%" border="0" cellspacing="0" cellpadding="1">
        <tr>
            <td class="tabActiveTableBg" colspan="10" align="center">
			    <BR/>
                <FORM name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:HiddenField id="hfTimeout" runat="server"/>
                    <asp:HiddenField ID="hfNumDelete" runat="server" value="0"/>
                    <asp:HiddenField ID="hfNumChanged" runat="server" value="0"/>
                    <asp:HiddenField ID="hfWasApproved" runat="server" value=""/>
                    <asp:HiddenField id="hfPeriodSelect" runat="server" Value="--select--"/>
                    <asp:HiddenField id="hfDefaultValue" runat="server" Value="Default value - "/>
                    <table width="99.5%">
			            <tr>
                            <td  class="noprint">
                                <asp:Label ID="lblPageTitle" runat="server" CssClass="pageTitles" Text="Data Input" style="margin-left: 0px;"></asp:Label>
                                <br />
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Enter quantities and costs for energy usage and waste disposal for this business location."></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <Ucl:EHSList id="uclInputHdr" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                 &nbsp;
                                <asp:PlaceHolder ID="phApproval" runat="server">
                                    <asp:Label ID="lblFinalApproval" runat="server" Text="Local Approval:" CssClass="prompt"></asp:Label>
                                    <asp:CheckBox id="cbFinalApproval" runat="server" Enabled="false"/>
                                    <asp:Label ID="lblFinalApprovalBy" runat="server" CssClass="textStd"></asp:Label>
                                </asp:PlaceHolder>
                                &nbsp;
                                &nbsp;
                                <asp:PlaceHolder ID="phRateStatus" runat="server" Visible="false">
                                    <asp:Label ID="lblRate" runat="server" CssClass="prompt" Text="Exchange Rate Recorded: "></asp:Label>
                                    <asp:Label ID="lblCurrency" runat="server" CssClass="textStd"></asp:Label>
                                    &nbsp;
                                    <asp:Label ID="lblRateStatus" runat="server" CssClass="textStd"></asp:Label>
                                </asp:PlaceHolder>
                                 <span style="float: right; margin-right: 5px;"  class="noprint">
                                    <asp:Button ID="btnSave1" class="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="width: 70px; margin-top: 4px; margin-left: 20px;" OnClientClick="return ValidateInputs();" onclick="OnSave_Click"></asp:Button>
                                    <asp:Button ID="btnCancel1" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" OnClientClick ="return confirmAction('Cancel without saving');" onclick="OnCancel_Click"></asp:Button>
                                </span>
                            </td>
                        </tr>
                    </table>

                    <div id="divMessages" runat="server">
                        <asp:Label ID="lblProfileNotExist" runat="server" Text="A metric profile doest not exist for this location" CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblNoMetrics" runat="server" Text="There are no Metrics defined for this profile" CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblNoInputs" runat="server" Text="There are no metric inputs for this period" CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblInputError" runat="server" Text="One or more inputs are incomplete. Please verify your invoice dates and quantities entered. " CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblPeriodLimit" runat="server" Text="Note: The selected date is the oldest period that you may enter or modify" CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblRangeWarning" runat="server" text="Some inputs may be out of range based on historical data values. Please double check your entries and press Save again to confirm." CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:Label ID="lblIncompleteInputs" runat="server" text="Some inputs are incomplete. Please ensure all desired inputs include both invoice date and value entries." CssClass="labelEmphasis" visible="false"></asp:Label>
                        <asp:HiddenField id="hfDeleteText" runat="server" Value="This input will be deleted when the reporting period is finalized"/>
                    </div>

                    <div id="divProfilePeriodScrollRepeater" runat="server" class="" visible="false" style="margin-top: 5px;">
					    <asp:Repeater runat="server" ID="rptProfilePeriod" ClientIDMode="AutoID" OnItemDataBound="rptProfilePeriod_OnItemDataBound">
						    <HeaderTemplate>
							    <table cellspacing="0" cellpadding="1" border="0" width="99%" >
                                    <tr>
                                        <td style="vertical-align: bottom;">
                                            <span class="noprint">
                                                <asp:Label ID="lblFormInstructions" runat="server" class="instructText"  Text="<b>Metrics</b> assigned to you. Please complete the required (*) inputs by the due date."></asp:Label>
                                            </span>
                                            <span class="noscreen">
                                                <asp:Label ID="Label2" runat="server" class="instructText"  Text="<b>Metrics</b>"></asp:Label>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblMetricTypeHdr" runat="server" Text="" cssclass="prompt"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblMetricReqdHdr" runat="server" Text="" cssclass="prompt"></asp:Label>
                                        </td>
                                         <td>
                                            <asp:Label ID="Label1" runat="server" Text="" cssclass="prompt"></asp:Label>
                                        </td>
                                        <td style="vertical-align: bottom;">
                                            <asp:Label ID="lblInvoiceHdr" runat="server" Text=" Invoice Period (from / to)" cssclass="prompt" ></asp:Label>
                                            <asp:Label ID="lblValueHdr" runat="server" Text="Quantity" cssclass="prompt" style="margin-left: 74px;"></asp:Label>
                                            <asp:Label ID="lblCostHdr" runat="server" Text="<%$ Resources:LocalizedText, Cost %>" cssclass="prompt" style="margin-left: 95px;"></asp:Label>
                                            <asp:Label ID="lblCreditHdr" runat="server" Text=" or  Credit Amount" cssclass="prompt" style="margin-left: 78px;"></asp:Label>
                                            <asp:Label ID="lblDelete" runat="server" Text="Del" cssclass="prompt" style="margin-left: 54px;" ToolTip="Delete this input"></asp:Label>
                                           <%-- <asp:Image ID="imgDelete" runat="server" ImageUrl="/images/defaulticon/16x16/delete.png" AlternateText="<%$ Resources:LocalizedText, Delete %>" ToolTip="<%$ Resources:LocalizedText, Delete %>" style="vertical-align: middle; border: 0px; margin-left: 54px;" />--%>
                                        </td>
                                    </tr>
                                    <tr style="height: 3px;"><td></td></tr>
						    </HeaderTemplate>
						    <ItemTemplate>
							    <tr>
								    <td id="tdMetricName" runat="server" class="rptInputTable" >
			                            <asp:Label ID="lblMetricName" runat="server" CSSClass="prompt" Text='<%#Eval("MEASURE_PROMPT") %>'></asp:Label>
								        <br />
                                        <asp:Label ID="lblMetricCD" runat="server" CssClass="refTextSmall" Text='<%#Eval("EHS_MEASURE.MEASURE_CD") %>'></asp:Label>
                                        <span style="float:right; margin-right: 3px;">
								            <asp:LinkButton ID="lnkMetricCD" runat="server" ToolTip="View 12 month input history" CommandArgument='<%#Eval("PRMR_ID") %>' CSSClass="refTextSmall" OnClick="lnkSelectMetric">
                                                <img src="/images/defaulticon/16x16/statistics-chart.png" alt="" style="vertical-align: middle; border: 0px;" />
								            </asp:LinkButton>
                                             <asp:LinkButton ID="lnkReviewAreaClose" runat="server" visible="false" CSSClass="buttonLink" CommandArgument='<%#Eval("PRMR_ID") %>' OnClick="lnkCloseResults" ToolTip="Close input history">
                                                 <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle; border: 0px;"/>
                                            </asp:LinkButton>
                                        </span>
                                        <asp:CheckBox id="cbMetricSelect" runat="server" Visible="false"/>
								    </td>
                                    <td class="rptInputTable" valign="middle" width="8px">
                                       <asp:Image ID="imgHazardType" ImageUrl="" Visible="true" runat="server" ToolTip="Energy input" style="vertical-align: middle;" />
                                    </td>
                                    <td id="tdMetricReqd" runat="server" class="rptInputTable" width="8px" >&nbsp;</td>
                                    <td  class="rptInputTableEnd" valign="middle" width="17px">
                                        <asp:ImageButton ID="ibAddInput" runat="server" ToolTip="add new input" ImageUrl="/images/plus.png"  OnClick="lnkMetricInput_Click" CommandArgument='<%#Eval("PRMR_ID") %>' />
                                    </td>
                                    <td class="rptInputTable" valign="top" style="width: 700px;">
                                        <asp:Repeater runat="server" ID="rptProfileInput" ClientIDMode="AutoID" OnItemDataBound="rptProfileInput_OnItemDataBound">
                                            <HeaderTemplate><table cellspacing="0" cellpadding="2" border="0" width="100%"></HeaderTemplate>
                                            <ItemTemplate>
							                    <tr>
                                                    <td class="rptInputTableInner" valign="middle" width="230px">
                                                        <asp:HiddenField id="hfInputDate" runat="server" Value='<%#Eval("INPUT_DT") %>'/>
                                                        <asp:HiddenField id="hfStatus" runat="server" Value='<%#Eval("STATUS") %>'/>
                                                        <span>
                                                            <telerik:RadDatePicker ID="radDateFrom" runat="server" CssClass="textStd" Width=112 Skin="Metro" ZIndex="9000">
                                                                <ClientEvents OnDateSelected="setFocus" />
                                                            </telerik:RadDatePicker>
                                                            <telerik:RadDatePicker ID="radDateTo" name="radDateTo" runat="server" CssClass="textStd" Width=112 Skin="Metro" ZIndex="9000">
                                                                <ClientEvents OnDateSelected="setFocus" />
                                                            </telerik:RadDatePicker>
                                                        </span>
								                    </td>
								                    <td class="rptInputTableInner" valign="middle" width="150px">
                                                        <span>
 									                        <asp:TextBox ID="tbMetricValue"  runat="server" maxlength="15" columns="12" CssClass="textStd WarnIfChanged" onChange="InputChanged(this);" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                            <asp:Label ID="lblMetricUOM" runat="server" CssClass="refTextSmall" ></asp:Label>
                                                        </span>
								                    </td>
                                                    <td class="rptInputTableInner" valign="middle"  width="275px">
                                                        <span>
 									                        <asp:TextBox ID="tbMetricCost"  runat="server" maxlength="15" columns="12" CssClass="textStd WarnIfChanged" onChange="InputChanged(this);"  onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                            &nbsp;
                                                            <asp:TextBox ID="tbMetricCredit"  runat="server" maxlength="15" columns="12" CssClass="textStd WarnIfChanged" enabled = "false" onChange="InputChanged(this);" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                            <asp:Label ID="lblMetricCurrency" runat="server" CssClass="refTextSmall" ></asp:Label>
                                                        </span>
								                    </td>
                                                    <td class="rptInputTableOpen" valign="middle">
                                                        <asp:CheckBox ID="cbDelete" runat="server" enabled="false" ToolTip="Delete this input" />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate></table></FooterTemplate>
                                        </asp:Repeater>
                                    </td>
							    </tr>
                                <tr style="height: 4px;">
                                    <td colspan="5">
                                        <div id="divReviewArea" name="divReviewArea" runat="server" visible="false">
                                        </div>
                                    </td>
                                </tr>
						    </ItemTemplate>
						    <FooterTemplate>
							    </table></FooterTemplate>
					    </asp:Repeater>
				        <asp:Label runat="server" ID="lblMetricEmptyRepeater" Height="40" Text="The metric list is empty." class="GridEmpty" Visible="false"></asp:Label>
                    </div>
                    <br />
                    <span style="float: left; margin-left: 5px;" class="noprint">
                        <asp:HiddenField id="hfExportText" runat="server" Value="Export (4 month) input history to Excel"/>
                        <asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="<%$ Resources:LocalizedText, Print %>" OnClientClick="javascript:window.print()"></asp:LinkButton>
                        <asp:LinkButton  ID="lnkExport" runat="server" Text="<%$ Resources:LocalizedText, Export %>" ToolTip="<%$ Resources:LocalizedText, ExportDataToExcelFormat %>" CssClass="buttonDownload" style="margin-left: 5px;" OnClick="lnkExport_Click"></asp:LinkButton>
                        <Ucl:Export id="uclExport" runat="server"/>
                     </span>
                    <span style="float: right; margin-right: 5px;" class="noprint">
                        <asp:Button ID="btnSave2" class="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="width: 70px; margin-right: 5px;" OnClientClick="return ValidateInputs();" onclick="OnSave_Click"></asp:Button>
                        <asp:Button ID="btnCancel2" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" OnClientClick ="return confirmAction('Cancel without saving');" onclick="OnCancel_Click"></asp:Button>
                    </span>
                    <br />
                    <Ucl:RadGauge id="uclGauge" runat="server"/>
                </FORM>
            </td>
        </tr>
    </table>
 </div>
</asp:Content>
