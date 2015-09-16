<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_QualityIssue.ascx.cs" Inherits="SQM.Website.Ucl_QualityIssue" EnableViewState="True"  %>

<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadScriptBlock.ascx" TagName="RadScript" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_PartSearch.ascx" TagName="PartSearch" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_NC.ascx" TagName="NC" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="RadAsyncUpload" TagPrefix="Ucl" %>
<%--<%@ Register src="~/Include/Ucl_QISearch.ascx" TagName="QISearch" TagPrefix="Ucl" %>--%>
<%@ Register src="~/Include/Ucl_Response.ascx" TagName="Response" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

  <script type="text/javascript">

      window.onload = function () {
          InitialAction();
      };

      function CalculateNonConforming() {
          var totalQty = document.getElementById('tbNCTotalQty');
          var sampleQty = document.getElementById('tbNCSampleQty');
          var nonConformQty = document.getElementById('tbNCNonConformQty');
          var totalEstNCQty = document.getElementById('tbTotalEstNCQty');

          if (checkForNumeric(totalQty.value, "Total quantity must be a positive number")) {
              if (checkForNumeric(sampleQty.value, "Sample quantity must be a positive number")) {
                  if (checkForNumeric(nonConformQty.value, "Non-Conforming quantity must be a positive number") && parseFloat(sampleQty.value) > 0) {
                      var qty = (parseFloat(nonConformQty.value) / parseFloat(sampleQty.value)) * parseFloat(totalQty.value);
                      totalEstNCQty.value = (qty.toFixed(1)).toString();
                  }
              }
          }
      }

      function InitialAction() {
      }
    </script>
    <%--<script type="text/javascript" src="../scripts/jquery-1.4.1.min.js"></script>--%>
    <script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>

<asp:Panel ID="pnlQualityIssue" runat="server" Visible="true">
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:HiddenField ID="hfAlertSaveIssue" runat="server" Value="Please complete and save the issue before proceeding" />

                        <div id="divPageBody" runat="server">

                            <div id="divWorkArea" runat="server">
                              <%--  <telerik:RadAjaxPanel ID="ajax1" runat="server">--%>
                                <Ucl:RadScript id="uclRadScript" runat="server"/>  <%--  gotta have this motherfucker for radsearch to work--%>
                                <div id="divRecord" runat="server">
                                    <table width="99%" border="0" style="margin-top: 4px;">
                                        <tr>
                                            <td>    
                                                <asp:Label ID="lblIssueInstruction" runat="server" Text="Issue summary" CssClass="prompt"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="99%" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 3px;">
                                        <tr id="trQIActivity" runat="server">
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblActivityType" runat="server" Text="Event"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <telerik:RadComboBox ID="ddlIncidentType" runat="server" Width="220" Skin="Metro" ZIndex=9000 AutoPostBack="true" OnSelectedIndexChanged="SelectActivityType" Font-Size=Small EmptyMessage="Select activity reported">
                                                    <Items>
                                                        <telerik:RadComboBoxItem  Value="" Text=""/>
                                                        <telerik:RadComboBoxItem  Value="RCV" Text="Receiving Inspection"/>
                                                        <telerik:RadComboBoxItem  Value="PRQ" Text="Production Quality"/>
                                                        <telerik:RadComboBoxItem  Value="CST" Text="Customer Reject Avoidance"/>
                                                    </Items>
                                                </telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label runat="server" ID="lblDetectedLocation" Text="Detected Location"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <span>
                                                    <telerik:RadAjaxPanel ID="ajx1" runat="server">
                                                        <telerik:RadComboBox ID="ddlReportedLocation" runat="server" style="width: 98%" Skin="Metro" ZIndex=9000 AutoPostBack="true" OnSelectedIndexChanged="OnLocationChanged" Font-Size=Small EmptyMessage="Select business location where issue was reported"></telerik:RadComboBox>
                                                    </telerik:RadAjaxPanel>
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" >
                                                <asp:Label runat="server" ID="lblIncidentDate" Text="Event Date / Reported By"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                 <telerik:RadDatePicker ID="radIssueDate" runat="server" CssClass="textStd" Width=115 Skin="Metro"></telerik:RadDatePicker>
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="lblOriginator_out" runat="server" style="font-weight: normal !important;"/>
                                                    &nbsp;&nbsp;
                                                <asp:Label ID="lblIssueDate_out" runat="server" style="font-weight: normal !important;"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label runat="server" ID="lblIncidentSeverity" Text="Event Category" Visible="true"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                 <telerik:RadComboBox ID="ddlIncidentSeverity2" runat="server" Skin="Metro" Width="350" ZIndex=9000 AutoPostBack="false" Font-Size=Small EmptyMessage="Select">
                                                    </telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblIssueDesc" runat="server" text="Description"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:TextBox ID="tbIssueDesc" TextMode="multiline" rows="3" maxlength="1000" runat="server" CssClass="commentArea"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblAttach1" runat="server" text="Attachments"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <Ucl:RadAsyncUpload id="uclRadAttach" runat="server"/>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div id="divDetection" runat="server">
                                    <table width="99%" border="0" style="margin-top: 2px;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblIdentifyInstruction" runat="server" Text="Issue Details" CssClass="prompt"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="99%" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                        <tr id="trPartType" runat="server">
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblPartType" runat="server" text="Part Type Or Family"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <telerik:RadComboBox ID="ddlPartType" runat="server"  Skin="Metro" ZIndex=9000 width="350" AutoPostBack="false" Font-Size=Small ToolTip="Select part type or family from the list provided" EmptyMessage="Select part type or family"></telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label runat="server" ID="lblPartNumber" Text="Part Number"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                    <telerik:RadAjaxPanel ID="ajx2" runat="server">
                                                    <span>
                                                        <Ucl:PartSearch id="uclPartSearch1" runat="server" />
                                                        &nbsp;
                                                        <asp:Label ID="lblPartDesc" runat="server" CssClass="refText"></asp:Label>
                                                    </span>
                                                </telerik:RadAjaxPanel>
                                            </td>
                                        </tr>
                                        <tr id="trReceipt" runat="server">
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblReceipt" runat="server" text="Receipt Number"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:TextBox ID="tbReceipt" runat="server" size="40" maxlength="40"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblRelatedParts" runat="server" text="Related or Suspected Parts"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:TextBox ID="tbRelatedParts" TextMode="multiline" rows="1" maxlength="200" runat="server" CssClass="commentArea"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblNCLotNum" runat="server" text="Lot Number"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:TextBox ID="tbNCLotNum" size="40" maxlength="255" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblNCTotalQty" runat="server" text="Total Quantity"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:TextBox ID="tbNCTotalQty" size="15" maxlength="20" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblNCSampleQty" runat="server" text="Sampled Quantity"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:TextBox ID="tbNCSampleQty" size="15" maxlength="20" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblNCNonConformQty" runat="server" text="Quantity Non-Conforming"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:TextBox ID="tbNCNonConformQty" size="15" maxlength="20" runat="server"  onblur="CalculateNonConforming();" />
                                                &nbsp;
                                                <asp:Label ID="lblCalculateNC" runat="server" Text="Total Quantity Non-Conforming" CssClass="prompt" Visible="false"></asp:Label>
                                                <asp:Button ID="lnkCalculateNC" runat="server" CssClass="buttonCalculate" OnClientClick="CalculateNonConforming();" text="Total Quantity Non-Conforming" ToolTip="calculate total suspect quantity non-conforming" Visible="true"></asp:Button>
                                                =&nbsp;
                                                <asp:TextBox ID="tbTotalEstNCQty" size="15" maxlength="20" runat="server" Enabled="false"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label ID="lblNonConformances" runat="server" text="Non-Conformance Details"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <div id="divEvidence" runat="server" style="margin-top: 3px;">
                                                    <asp:Label ID="lblEvidenceInstruction" runat="server" Text="Categorize any non-conformances observed. Enter direct measurements if taken or provide observations describing the problem." CssClass="instructText"></asp:Label>
                                                    <table width="99%" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 2px;" >
                                                        <tr>
                                                            <td class="columnHeaderTop" width="24% >
                                                                <asp:Label runat="server" ID="lblProblemArea" Text="Primary Problem Area<br><br><br>Specific Nonconformance"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                                            <td class="tableDataAlt" width="75%">
                                                                <Ucl:NC id="uclNC" runat="server" />
                                                            </td>
                                                            </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblMeasurements" runat="server" text="Physical Measurements"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:GridView runat="server" ID="gvMeasureGrid" Name="gvMeasureGrid" CssClass="" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="none" BorderStyle="none" BorderWidth="0px" PageSize="20" AllowSorting="true" Width="99.5%" OnRowDataBound="gvMeasure_OnRowDataBound">
                                                                    <HeaderStyle CssClass="HeadingCellTextLeft" BorderWidth="0px" />    
                                                                    <RowStyle CssClass="DataCell" Height=26 />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Print/Spec" ItemStyle-Width="50%">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox ID="tbMeasureName" runat="server"  CssClass="textStd" style="width:97%;" text='<%#Eval("MEASURE_NAME") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Actual Value" ItemStyle-Width="50%">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox ID="tbMeasureValue" runat="server" CssClass="textStd"  style="width:97%;" text='<%#Eval("MEASURE_VALUE") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblObservations" runat="server" text="Additional Observations"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbObservations" TextMode="multiline" rows="3" maxlength="1000" runat="server" CssClass="commentArea"/>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader">
                                                 <asp:Label ID="lblIssueCost" runat="server" text="Estimated Costs"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <table width="99.5%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 3px;">
                                                    <tr>
                                                        <td colspan="4">
                                                            <asp:Label ID="lblCostsInstruct" runat="server" Text="Summarize costs related to this issue. Either actual costs incurred or potential costs avoided due to early detection, containement, etc." CssClass="instructText"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr class="HeadingCellTextLeft">
                                                        <td class="columnHeader" width="24%">
                                                            <asp:UpdatePanel ID="updCurrency" runat="server" UpdateMode=Conditional>
                                                                <Triggers>
                                                                    <asp:AsyncPostBackTrigger ControlID="ddlReportedLocation" />
                                                                </Triggers>
                                                                <ContentTemplate>
                                                                    <asp:Label ID="lblCurrency" runat="server" Text="Currency:" CssClass="prompt"></asp:Label>
                                                                    <telerik:RadComboBox ID="ddlCurrency" runat="server" Skin="Metro" ZIndex=9000  AutoPostBack="false" Font-Size=Small  width=75 Height="200" EmptyMessage="currency"></telerik:RadComboBox>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </td>
                                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                                        <td width="17%"><asp:Label ID="lblCostEst" runat="server" Text="Cost"></asp:Label></td>
                                                        <td width="58%"><asp:Label ID="lblCostDetails" runat="server" Text="Details"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader" width="24%"><asp:Label ID="lblActCost" runat="server" Text="Actual" CssClass="prompt"></asp:Label></td>
                                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                                        <td><asp:TextBox ID="tbActCost" size="15" maxlength="20" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/></td>
                                                        <td><asp:TextBox ID="tbActCostNote" runat="server" TextMode="multiline" rows="1"  MaxLength="400" CssClass="commentArea"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader" width="24%"><asp:Label ID="lblPotCost" runat="server" Text="Avoided" CssClass="prompt"></asp:Label></td>
                                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                                        <td><asp:TextBox ID="tbPotCost" size="15" maxlength="20" runat="server" onblur="ValidNumeric(this, 'please enter numeric values only');"/></td>
                                                        <td><asp:TextBox ID="tbPotCostNote" runat="server" TextMode="multiline" rows="1"  MaxLength="400" CssClass="commentArea"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div id="divDisposition" runat="server">
                                    <table width="99%" border="0" style="margin-top: 2px;">
                                        <tr>
                                            <td>    
                                                <asp:Label ID="lblDispositionInstruct" runat="server" Text="Issue disposition and additional steps" CssClass="prompt"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="99%" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 3px;" >
                                        <tr>
                                            <td colspan="3">
                                                <asp:UpdatePanel ID="updResponsible" runat="server" UpdateMode=Conditional>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="uclPartSearch1" />
                                                        <asp:AsyncPostBackTrigger ControlID="ddlResponsibleLocation" />
                                                    </Triggers>
                                                    <ContentTemplate>
                                                        <table width="100%"  border="0" cellspacing="0" cellpadding="1">
                                                            <tr>
                                                                <td class="columnHeader"  width="24%">
                                                                    <asp:Label runat="server" ID="lblResponsibleLocation" Text="Responsible Location"></asp:Label>
                                                                </td>
                                                                <td class="required" width="1%">&nbsp;</td>
                                                                <td class="tableDataAlt" width="75%">
                                                                    <telerik:RadComboBox ID="ddlResponsibleLocation" runat="server" style="width: 98%" Skin="Metro" ZIndex=9000 AutoPostBack="true" OnSelectedIndexChanged="OnResponsibleLocationChanged" Font-Size=Small EmptyMessage="Select location responsible for resolving the issue"></telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label runat="server" ID="lblResponsible" Text="Responsible Person(s)"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td class="tableDataAlt">
                                                                    <telerik:RadComboBox ID="ddlResponsible" runat="server" Skin="Metro" ZIndex=9000 Font-Size=Small Width="350" CheckBoxes="true" EnableCheckAllItemsCheckBox="false"></telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="columnHeader">
                                                                    <asp:Label runat="server" ID="lblResponseTime" Text="Response Required Within"></asp:Label>
                                                                </td>
                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                <td class="tableDataAlt">
                                                                    <telerik:RadComboBox ID="ddlResponseTime" runat="server" Width=105 Skin="Metro" Height="300" ZIndex=9000 Font-Size=Small EmptyMessage="Days" ToolTip="Response required within number of days from issue creation"></telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                     </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <span>
                                                    <asp:Label runat="server" ID="lblDupIssue" Text="Repeat Of Issue"></asp:Label>
                                                     <input ID="btnDupIncident" runat="server" type="button" style="margin-top: 4px;" class="buttonSearch" ToolTip="find repeat issues based on the part type and non-conformances observed for the current issue" onclick="PopupCenter('../Quality/QualityIssueList.aspx?', 'newPage', 800, 650);" value="Search" />
                                                    <%--<asp:Button ID="btnDupIncident" runat="server" style="float: right; margin-right: 5px;" ToolTip="find repeat issues based on the part type and non-conformances observed for the current issue" CSSClass="buttonSearch" text="Search" OnClick="btnDupIncident_Click"></asp:Button>--%>
                                                </span>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:TextBox ID="tbDupIssue" runat="server" CssClass="textStd" MaxLength="40"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" >
                                                <asp:Label ID="lblDisposition" runat="server" text="Disposition"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" >&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <telerik:RadComboBox ID="ddlDisposition" runat="server" Width="350" Skin="Metro" ZIndex=9000 AutoPostBack="false" Font-Size=Small EmptyMessage="Material disposition"></telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" >
                                                <asp:Label ID="lblProblemResolution" runat="server" text="Problem Resolution"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <span>
                                                    <asp:CheckBox id="cb8DRequired" runat="server" Text="8D Required" ToolTip="Problem Analysis Required " CssClass="textStd" TextAlign="Left"/>
                                                    <asp:PlaceHolder ID="ph8DRef" runat="server">
                                                        &nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="lbl8DRef" runat="server" CssClass="prompt" Text="Associated VPRS Number: "></asp:Label>
                                                        <asp:TextBox ID="tb8DRef" runat="server" CssClass="textStd" size="24" maxlength="40"></asp:TextBox>
                                                        <%-- maps to REF_OPERATION--%>
                                                    </asp:PlaceHolder>
                                                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" >
                                                <asp:Label ID="lblStatus" runat="server" text="Status"></asp:Label>
                                            </td>
                                            <td class="required" >&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <span>
                                                    <telerik:RadComboBox ID="ddlStatus" runat="server" Width="350" Skin="Metro" ZIndex=9000 AutoPostBack="false" Font-Size=Small EmptyMessage="Issue status"></telerik:RadComboBox>
                                                    &nbsp;&nbsp;
                                                    <asp:PlaceHolder ID="phNotify" runat="server" Visible="false">
                                                        <img src="/images/defaulticon/16x16/mail-sent.png" alt="" style="vertical-align: middle; margin-left: 4px;" />
                                                        <asp:CheckBox id="cbNotify" runat="server" CssClass="textStd" TextAlign="Left" text="Re-send Issue Notifications " ToolTip="For existing incidents, check to resend email notifications to assigned users. Note: notifications will be send automatically when creating a new issue."/>
                                                    </asp:PlaceHolder>
                                                </span>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Button id="btnPrintLabel" runat="server" onClientClick="Popup('../Quality/QualityIssue_Label.aspx?', 'newPage', 600, 450); return false;"
                                                    text="Print Label" class="buttonLink" Visible="false" />
                                </div>

                                <table width="99%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 7px;">
						            <tr>
							            <td style="width: 20px;">
                                             <button type="button" id="btnToggleIssueResponse" onclick="ToggleSectionVisible('pnlIssueResponse','0');">
                                                    <img id="pnlIssueResponse_img" src="/images/defaulticon/16x16/arrow-8-right.png"/>
                                            </button>
                                        </td>
                                        <td>
                                            &nbsp;
                                            <asp:Label ID="lblIssueResponseInstruction" runat="server" class="prompt" Text="Acknowledge and responses"></asp:Label>
                                            &nbsp(<asp:Label ID="lblIssueResponseCount" runat="server"></asp:Label>)
                                        </td>
                                    </tr>
                                </table>

                                <asp:Panel ID="pnlIssueResponse" runat="server" style="height:1px; visibility: hidden; margin-top: 0px;" >
                                    <div id="divResponse" runat="server">
                                        <table width="99%" border="0">
                                            <tr>
                                                <td>    
                                                    <asp:Label ID="lblResponseInstruct" runat="server" Text="Responses and comments between the parties responsible for resolving this issue." CssClass="instructText"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                        <table width="99%" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 3px;">
                                            <tr>
                                                <td class="columnHeader" width="24%">
                                                    <asp:Label ID="lblResponseComment" runat="server" Text="Response Log"></asp:Label>
                                                </td>
                                                <td class="tableDataAlt" width="1%">&nbsp;</td>
                                                <td class="tableDataAlt" width="75%">
                                                    <Ucl:Response id="uclResponse" runat="server" />
                                                    <asp:PlaceHolder ID="phResponseAlert" runat="server">
                                                        <img src="/images/defaulticon/16x16/sticky-note.png" alt="" style="vertical-align: middle; margin-left: 4px;" />
                                                        <asp:CheckBox id="cbResponseAlert" runat="server" Text="Request A Follow-Up Response " CssClass="textStd" TextAlign="Left" ToolTip="Request a follow-up response from any responsible persons via a new inbox task and email alert."/>
                                                    </asp:PlaceHolder>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </asp:Panel>

                            </div>
                        </div>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>
</asp:Panel>

