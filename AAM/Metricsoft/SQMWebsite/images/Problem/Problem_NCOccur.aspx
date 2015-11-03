<%@ Page Title="" Language="C#" MasterPageFile="~/Problem.master" AutoEventWireup="true" CodeBehind="Problem_NCOccur.aspx.cs" Inherits="SQM.Website.Problem_NCOccur" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_CaseList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<link type="text/css" href="../css/redmond/jquery-ui-1.8.20.custom.css" rel="Stylesheet" />
  <script type="text/javascript">
      function CalculateNonConforming() {
          var totalQty = document.getElementById('tbNCTotalQty');
          var sampleQty = document.getElementById('tbNCSampleQty');
          var nonConformQty = document.getElementById('tbNCNonConformQty');
          var totalEstNCQty = document.getElementById('tbTotalEstNCQty');

          if (checkForNumeric(totalQty.value, "Total quantity must be a positive number")) {
              if (checkForNumeric(sampleQty.value, "Sample quantity must be a positive number")) {
                  if (checkForNumeric(nonConformQty.value, "Non-Conforming quantity must be a positive number") && parseFloat(sampleQty.value) > 0) {
                      totalEstNCQty.value = (parseFloat(nonConformQty.value) / parseFloat(sampleQty.value)) * parseFloat(totalQty.value);
                  }
              }
          }
      }
    </script>
    <script type="text/javascript" src="../scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>
    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="2">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:Panel runat="server" ID="pnlSearchBar">
                            <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                        </asp:Panel>
                        <table width="99%">
			                <tr>
                                <td>
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Document product non-conformances or quality control exceptions. Create new issue or search for exiting issues by issue number, problem type or part number."></asp:Label>
                                     <asp:Label ID="lblNCOccurTitle" runat="server" Text="Quality Issue" Visible="false"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <asp:Panel ID="pnlSearchList" runat="server" Visible="false">
                              <Ucl:IssueList id="uclIssueList" runat="server"/>
                        </asp:Panel>

                    <div id="divPageBody" runat="server">
                        <table width="99%">
                            <tr height="28" class="tableDataHdr">
			                    <td class="tableDataHdr2" >
                                    <asp:Label ID="lblIssueDetail" runat="server" Text="Quality Issue Type:" ></asp:Label>
                                    &nbsp;
                                    <asp:DropDownList ID="ddlQualityIssueType" runat="server"></asp:DropDownList>
                                    <span style="float: right; margin-right: 10px; margin-top: 5px;">
                                        <asp:Label ID="lblIssueDate" runat="server" Text="Reported Date:" ></asp:Label>
                                         &nbsp;
                                        <asp:Label ID="lblIssueDate_out" runat="server" style="font-weight: normal !important;"></asp:Label>
                                         &nbsp;&nbsp;
                                         <asp:Label ID="lblOriginator" runat="server" text="Originator:"></asp:Label>
                                          &nbsp;
                                         <asp:Label ID="lblOriginator_out" runat="server" style="font-weight: normal !important;"/>
                                    </span>
                                </td>
                            </tr>
                            <tr>
                            <tr><td>&nbsp;</td></tr>
                                <td>
                                    <asp:Label ID="lblSourceTitle" runat="server" CssClass="prompt" Text="Detection:"></asp:Label>
                                     &nbsp;
                                    &nbsp;
                                    <asp:Label ID="lblSourceInstruction" runat="server" Text="Specify where the issue occured or was detected. Internal issues (such as manufacturing defects or APQP) require business location and plant line identification. External sources (customer or carriers) must include identification of the impacted customer." CssClass="instructText"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="editArea">
                                    <table width="100%" border="0" cellspacing="1" cellpadding="1" class="darkBorder">
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblDetectedDate" runat="server" text="When Detected"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <script type="text/javascript">
                                                    $(function () {
                                                        $('#tbDateDetected').datepicker({
                                                            changeMonth: true,
                                                            changeYear: true,
                                                            showOn: "both",
                                                            buttonImage: "/images/calendar.gif",
                                                            buttonImageOnly: true,
                                                            yearRange: "2000:2030",
                                                            buttonText: "Select From Date"
                                                        });
                                                        if ($('#tbDateDetected').is(':disabled') == true) {
                                                            $('#tbDateDetected').datepicker().datepicker('disable');
                                                        }
                                                    });
                                                </script>
                                                <asp:TextBox ID="tbDateDetected" runat="server" CausesValidation="True" MaxLength="20" Style="width: 90px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblSource" runat="server" text="Where Detected"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:RadioButtonList runat="server" ID="rblSource" CSSclass=""
	                                                RepeatDirection="Horizontal" RepeatLayout="flow"
		                                            AutoPostBack="false">
							                            <asp:ListItem Text="Internal&nbsp;&nbsp;" Value="IN" ></asp:ListItem>
								                        <asp:ListItem Text="Supplier&nbsp;&nbsp;" Value="SP"></asp:ListItem>
                                                        <asp:ListItem Text="Customer&nbsp;&nbsp;" Value="CS"></asp:ListItem>
                                                        <asp:ListItem Text="Carrier" Value="TR"></asp:ListItem>
						                        </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label runat="server" ID="lblInternalPlant" Text="Business Location and Plant Line" Visible="true"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:DropDownList ID="ddlPlant" runat="server"></asp:DropDownList>
                                                &nbsp;&nbsp;
                                                <asp:DropDownList ID="ddlPlantLine" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label runat="server" ID="lblPartNumber" Text="Part Number" Visible="true"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                  				                <asp:TextBox runat="server" ID="tbPartNumber" Text="" size="40" maxlength="200"></asp:TextBox>
                                                <input type="button" id="btnFindPart" onclick="PopupCenter('../Shared/Shared_PartSearch.aspx?', 'newPage', 800, 600);"
                                                    class="centered" style="background-image: url(/images/search.jpg);  background-color: transparent; border: none; width:27px;height:27px;"> </input>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label runat="server" ID="lblSupplier" Text="Supplier and Location" Visible="true"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox runat="server" ID="tbSupplier" Text="" size="40" maxlength="200"></asp:TextBox>
                                                <input type="button"  id="btnFindSupplier" onclick="PopupCenter('../Shared/Shared_PartSearch.aspx?', 'newPage', 900, 600);"
                                                   class="centered" style="background-image: url(/images/search.jpg);  background-color: transparent; border: none; width:27px;height:27px;"> </input>
                                                &nbsp;&nbsp;
                                               <asp:DropDownList ID="ddlSupplierPlant" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label runat="server" ID="lblCustomer" Text="Impacted/Reporting Customer and Location" Visible="true"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:DropDownList ID="ddlCustomer" runat="server"></asp:DropDownList>
                                                &nbsp;&nbsp;
                                                <asp:DropDownList ID="ddlCustomerPlant" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblIssueDesc" runat="server" text="Issue Description"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbIssueDesc" TextMode="multiline" rows="2" maxlength="1000" runat="server" CssClass="commentArea" style="width:97%;"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <table width="99%" border="0" cellspacing="0" cellpadding="0">
	                        <tr>
		                        <td>
                                   <input type='button' id='lbIdentify_nav' value="Identification" class="buttonLink" onclick="SetFocus('tbNCLotNum');"/>
                                   <input type='button' id='lbEvidence_nav' value="Evidence" class="buttonLink" onclick="SetFocus('ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder_Body_gvQISamples_ctl02_ddlPrimaryNC');"/>
                                   <input type='button' id='lbDisposition_nav' value="Disposition" class="buttonLink" onclick="SetFocus('ddlDisposition');"/>
                                </td>
                            </tr>
                            <tr height="8"><td></td></tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblIdentifyTitle" runat="server" CssClass="prompt" Text="Identification:"></asp:Label>
                                    &nbsp;
                                    &nbsp;
                                    <asp:Label ID="lblIdentifyInstruction" runat="server" Text="Identify any non-conforming material, sampling results and estimated impacted quantities." CssClass="instructText"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="editArea">
                                    <table width="100%" border="0" cellspacing="1" cellpadding="1" class="darkBorder">
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblNCLotNum" runat="server" text="Lot Number"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbNCLotNum" size="50" maxlength="255" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblNCContainer" runat="server" text="Container"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbNCContainer" size="50" maxlength="255" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblNCTotalQty" runat="server" text="Total Quantity"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbNCTotalQty" size="12" maxlength="20" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblNCSampleQty" runat="server" text="Sampled Quantity"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbNCSampleQty" size="12" maxlength="20" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblNCNonConformQty" runat="server" text="Quantity Non-Conforming"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbNCNonConformQty" size="12" maxlength="20" runat="server"/>
                                                &nbsp;
                                                <input type="button" id="btnCalculateNC" onclick="CalculateNonConforming();" value="Calculate Total Suspect Quantity:" class="buttonEmphasis"></input>
                                                <asp:TextBox ID="tbTotalEstNCQty" size="12" maxlength="20" runat="server" Enabled="false"/>
                                            </td>
                                        </tr>
                                    </table>

                                    <br />
                                    <asp:Label ID="lblEvidenceTitle" runat="server" CssClass="prompt" Text="Evidence:"></asp:Label>
                                    &nbsp;
                                    &nbsp;
                                    <asp:Label ID="lblEvidenceInstruction" runat="server" Text="Categorize the non-conformances observed, enter direct measurements if taken and attach any supporting documentation." CssClass="instructText"></asp:Label>
                                    <table width="100%" border="0" cellspacing="0" cellpadding="2">
			                            <tr>
			                                <td class=admBkgd align=center>
                                                <asp:GridView runat="server" ID="gvQISamples" Name="gvQISamples" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvQISamples_OnRowDataBound">
                                                    <HeaderStyle CssClass="HeadingCellText" />
                                                    <RowStyle CssClass="DataCell" />
                	                                <Columns>
                                                        <asp:TemplateField HeaderText="Sample" ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSampleNum" runat="server" text='<%#Eval("SAMPLE_NUM") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Defect Category" ItemStyle-Width="20%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfPrimaryNC" runat="server" Value='<%#Eval("PROBLEM_PRIMARY") %>'/>
                                                                <asp:DropDownList ID="ddlPrimaryNC" runat="server" ></asp:DropDownList>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Defect" ItemStyle-Width="20%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:HiddenField ID="hfSecondaryNC" runat="server" Value='<%#Eval("PROBLEM_SECONDARY") %>'/>
                                                                <asp:DropDownList ID="ddlSecondaryNC" runat="server" ></asp:DropDownList>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Count" ItemStyle-Width="5%" ItemStyle-VerticalAlign="Top">
                                                            <ItemTemplate>
                                                                <asp:TextBox id="tbNCCount" runat="server" style="width:90%;" text='<%#Eval("PROBLEM_COUNT") %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
          			                                    <asp:TemplateField HeaderText="Measures" ItemStyle-Width="35%">
                                                            <ItemTemplate>
                                                                <asp:GridView runat="server" ID="gvMeasureGrid" Name="gvMeasureGrid" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvMeasure_OnRowDataBound">
                                                                    <HeaderStyle CssClass="HeadingCellText" />
                                                                    <RowStyle CssClass="DataCell" Height=26 />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Characteristic" ItemStyle-Width="60%">
                                                                            <ItemTemplate>
                                                                                <!--<asp:TextBox ID="tbMeasureName" runat="server"  style="width:97%;" text='<%#Eval("MEASURE_NAME") %>'></asp:TextBox>-->
                                                                                <asp:DropDownList ID="ddlMeasure" runat="server" style="width:97%;"></asp:DropDownList>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Value %>" ItemStyle-Width="40%">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox ID="tbMeasureValue" runat="server"  style="width:97%;" text='<%#Eval("MEASURE_VALUE") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                             </asp:GridView>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Attach" ItemStyle-Width="5%">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnAttachment" runat="server" ImageUrl="~/images/attach.gif" OnClientClick="PopupCenter('../Shared/Shared_Upload.aspx?upload_type=default_document', 'newPage', 800, 600);  return false;" />
                                                                <asp:HiddenField ID="hfAttachmentID" runat="server" Value='<%#Eval("ATTACHMENT_ID") %>'/>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="100%" border="0" cellspacing="1" cellpadding="1" class="darkBorder">
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblObservations" runat="server" text="Additional Observations"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbObservations" TextMode="multiline" rows="2" maxlength="1000" runat="server" CssClass="commentArea" style="width:97%;"/>
                                            </td>
                                        </tr>
                                   </table>
                                </td>
                            </tr>
                            <tr>
                            <tr><td>&nbsp;</td></tr>
                                <td>
                                    <asp:Label ID="lblDispositionTitle" runat="server" CssClass="prompt" Text="Disposition:"></asp:Label>
                                    &nbsp;
                                    &nbsp;
                                    <asp:Label ID="lblDispositionInstruct" runat="server" Text="Specify the Disposition of the suspect material and if this issue should be escalated to a formal Problem Resolution process." CssClass="instructText"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="editArea">
                                    <table width="100%" border="0" cellspacing="1" cellpadding="1" class="darkBorder">
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblDisposition" runat="server" text="Disposition"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:DropDownList ID="ddlDisposition" runat="server"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblResponsible" runat="server" text="Responsibility"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:RadioButtonList runat="server" ID="rblResponsible" CSSclass=""
	                                                RepeatDirection="Horizontal" RepeatLayout="flow"
		                                            AutoPostBack="false">
							                            <asp:ListItem Text="Internal&nbsp;&nbsp;" Value="IN" ></asp:ListItem>
								                        <asp:ListItem Text="Supplier&nbsp;&nbsp;" Value="SP"></asp:ListItem>
                                                        <asp:ListItem Text="Customer&nbsp;&nbsp;" Value="CS"></asp:ListItem>
                                                        <asp:ListItem Text="Carrier&nbsp;&nbsp;" Value="TR"></asp:ListItem>
                                                        <asp:ListItem Text="Unknown" Value="UN"></asp:ListItem>
						                        </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblNotifyTeam" runat="server" text="Notify Reaction Team"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:CheckBox id="cbNotifyTeam" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblActionRequired" runat="server" text="Is Problem Analysis Required ?"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:CheckBox id="cbActionRequired" runat="server"/>
                                            </td>
                                        </tr>
                                       <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblComments" runat="server" text="<%$ Resources,LocalizedText, Comments %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:TextBox ID="tbComments" TextMode="multiline" rows="2" maxlength="1000" runat="server" CssClass="commentArea" style="width:97%;"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="39%">
                                                <asp:Label ID="lblPrintLabel" runat="server" text="Preview Or Print The Issue Idenfiticaion Label"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="60%">
                                                <asp:Button id="btnPrintLabel" runat="server" onClientClick="Popup('../Problem/QualityIssue_Label.aspx?', 'newPage', 600, 450); return false;"
                                                    text="Print Label" class="buttonStd" />
                                            </td>
                                        </tr>
                                    </table>
                                 </td>
                            </tr>
                        </table>
                        <table width="99%">
			                <tr>
                                <td>
                                    <input type='button' id='lbTop_nav' value="Back To Top" class="buttonLink" onclick="SetFocus('ddlQualityIssueType');"/>
                                </td>
                            </tr>
                        </table>
                   </div>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>
</asp:Content>
