<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSRez.ascx.cs" Inherits="SQM.Website.Ucl_EHSRez" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<link type="text/css" href="../css/redmond/jquery-ui-1.8.20.custom.css" rel="Stylesheet" />
<script type="text/javascript" src="../scripts/jquery-1.4.1.min.js"></script>
<script type="text/javascript" src="../scripts/jquery-ui-1.8.20.custom.min.js"></script>
<script type="text/javascript" src="../scripts/datepicker.js"></script>

<script type="text/javascript">
    function UpdateProfileButtons() {
       // document.getElementById("btnProfileMeasureNew").disabled = true;
    }



    function CancelProfileMeasure() {
        $("#<%=pnlMetricEdit.ClientID%> input[type=text]").val('');
        $("#<%=pnlMetricEdit.ClientID%> textarea").val('');
        document.getElementById("lblMetricName").innerHTML = '';
        document.getElementById("lblDisposalDesc").innerHTML = '';
        document.getElementById("tbMetricPrompt").disabled = true;
        document.getElementById("ddlMetricCategory").disabled = true;
        document.getElementById("ddlMetricCategory").selectedIndex = 0;
        document.getElementById("ddlMetricID").disabled = true;
        document.getElementById("ddlMetricID").selectedIndex = 0;
        document.getElementById("ddlMetricDisposalCode").disabled = true;
        document.getElementById("ddlMetricDisposalCode").selectedIndex = 0;
        document.getElementById("ddlMetricCurrency").disabled = true;
        document.getElementById("ddlMetricResponsible").disabled = true;
        document.getElementById("ddlMetricResponsible").selectedIndex = 0;
        document.getElementById("ddlMetricRegStatus").disabled = false;
        document.getElementById("ddlMetricRegStatus").selectedIndex = 0;
        document.getElementById("ddlMetricStatus").disabled = true;
        document.getElementById("ddlMetricStatus").selectedIndex = 0;

        document.getElementById("cbMetricNegValue").disabled = true; document.getElementById("cbMetricNegValue").checked = false;
        document.getElementById("cbMetricRequired").disabled = true; document.getElementById("cbMetricRequired").checked = false;

        document.getElementById("hfOper").value = "";

        document.getElementById("btnProfileMeasureNew").disabled = false;
        document.getElementById("btnMetricCancel").disabled = true;
        document.getElementById("btnMetricSave").disabled = true;

        if (document.getElementById("divMetricUOM") != null) {
            document.getElementById("ddlMetricUOM").disabled = true;
            document.getElementById("ddlMetricUOM").selectedIndex = 0;
        }

        if (document.getElementById("divUserUOM") != null)
            document.getElementById("divUserUOM").style.display = 'none';
    }

    function ValidProfileMeasure() {
        var status = true;

        if ((status=checkForSelect(document.getElementById("ddlMetricID"), "Measure Code is Required")) == true) {
            if ((status = checkForSelectChecked(document.getElementById("ddlMetricUOM"), document.getElementById("cbUserUOM"), "Invoice UOM is Required")) == true) {
                status = checkForSelect(document.getElementById("ddlMetricResponsible"), "Responsible Person is Required");
            }
        }

        return status;
    }

</script>

<asp:HiddenField ID="hfOper" runat="server" />

<asp:Panel ID="pnlEHSProfile" runat="server" Visible = "false" CssClass="admBkgd">
    <table width="99%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
        <tr>
            <td class="optionArea" colspan="3">
                <asp:Label ID="lblProfileInstruct" runat="server" CssClass="instructText" Text="Monitor energy consumption and waste streams for this business location on a monthly basis. Select the measures of interest ..."></asp:Label>
                <!--
                <br />
                <asp:Label ID="lblMetricsHeader" runat="server" Text="Metrics List" CssClass="prompt" style="vertical-align: middle;"></asp:Label>
                -->
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" width="50%" colspan="1">
                <div id="divMetricListGVScroll" runat="server" class="">
                    <asp:GridView runat="server" ID="gvMetricList" Name="gvMetricList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" ShowHeaderWhenEmpty="True" cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvOnProfileRowDataBound">
                        <HeaderStyle CssClass="HeadingCellText" />
                        <RowStyle CssClass="DataCell" />
                	    <Columns>
                            <asp:TemplateField HeaderText="Measure Name" ItemStyle-Width="35%">
                                <ItemTemplate>
                                        <asp:LinkButton ID="lnkMetricName" runat="server" CommandArgument='<%#Eval("PRMR_ID") %>' CSSClass="linkUnderline"
										    Text='<%#Eval("EHS_MEASURE.MEASURE_NAME") %>' OnClick="lnkMetricList_Click"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="M Code" ItemStyle-Width="15%">
							    <ItemTemplate>
                                    <asp:HiddenField id="hfMetricCategory" runat="server" value='<%#Eval("EHS_MEASURE.MEASURE_CATEGORY") %>' />
								    <asp:LinkButton ID="lnkMetricCD" runat="server" CommandArgument='<%#Eval("PRMR_ID") %>' CSSClass="linkUnderline"
										Text='<%#Eval("EHS_MEASURE.MEASURE_CD") %>' OnClientClick="UpdateProfileButtons();" OnClick="lnkMetricList_Click"></asp:LinkButton>
                                </ItemTemplate>
							</asp:TemplateField>
                            <asp:TemplateField HeaderText="" ItemStyle-Width="5%" ItemStyle-HorizontalAlign=Center>
                                <ItemTemplate>
                                    <asp:HiddenField id="hfMetricRegStatus" runat="server" value='<%#Eval("REG_STATUS") %>' />
                                    <asp:HiddenField id="hfDisposalCode" runat="server" value='<%#Eval("UN_CODE") %>' />
                                     <asp:Image ID="imgHazardType" ImageUrl="" Visible="true" ToolTip="<%$ Resources:LocalizedText, EnergyInput %>" runat="server" style="vertical-align: middle;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Required %>" ItemStyle-Width="15%" ItemStyle-HorizontalAlign=Center>
                                <ItemTemplate>
                                    <asp:CheckBox id="cbMetricRequired" runat="server" enabled="false" Checked='<%#Eval("IS_REQUIRED") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Responsible" ItemStyle-Width="30%">
                                <ItemTemplate>
                                    <asp:Label ID="lblMetricResponsible" runat="server" Text='<%#Eval("PERSON.EMAIL") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label runat="server" ID="lblMetricListEmpty" Text="<%$ Resources:LocalizedText, NoMetricDefinedBusLoc %>" class="GridEmpty" Visible="false"></asp:Label>
                </div>
            </td>
            <td valign="top" width="1%"></td>
            <td valign="top" width="49%">
                <asp:Button ID="btnProfileMeasureNew" runat="server" Text="Add Metric" ToolTip="Add an EHS Measure to this profile" CSSclass="buttonStd" style="margin: 6px;" enabled=false OnClick="btnMetricAdd_Click" ></asp:Button>
                <asp:Button ID="btnMetricCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" enabled=false style="margin: 5px;" OnClientClick="CancelProfileMeasure(); return false;"  ></asp:Button>
                <asp:Button ID="btnMetricSave" CSSclass="buttonStd" runat="server" enabled=false text="Update Metric" style="margin: 5px;"
                        OnClientClick="return ValidProfileMeasure();" OnClick="btnMetricSave_Click" ></asp:Button>
                <asp:Button ID="btnProfileSave" CSSclass="buttonEmphasis" runat="server" enabled=true Text="<%$ Resources:LocalizedText, SaveProfile %>" style="float: right; margin: 5px;"
                        OnClientClick="return confirmChange('Environment Profile');" onclick="btnProfileSave_Click" ></asp:Button>
                <asp:Panel id="pnlMetricEdit" runat="server">
                    <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder1">
                        <tr>
                            <td class="columnHeader" width="34%">
                                <asp:Label ID="lblMetricCode" runat="server" text="Measure"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td CLASS="tableDataAlt" width="65%">
                                <asp:UpdatePanel ID="udpMetricID" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricCategory" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricID" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlMetricCategory" runat="server" OnSelectedIndexChanged="ddlCategoryChanged" AutoPostback="true" ></asp:DropDownList>
                                        <br />
                                        <asp:DropDownList ID="ddlMetricID" runat="server" OnSelectedIndexChanged="ddlUpdateLabel" AutoPostback="true" style="width: 250px;"></asp:DropDownList>
                                        <asp:Label ID="lblMetricName" runat="server" CssClass="refText"></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricPrompt" runat="server" Text="<%$ Resources:LocalizedText, DisplayPrompt %>"></asp:Label>
                            </td>
                            <td class="tableDataAlt" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:TextBox ID="tbMetricPrompt"  runat="server" TextMode="multiline" rows="2" MaxLength="200" CssClass="textStd" style="width: 97%;"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblWasteCode" runat="server" text="Country Waste Code"></asp:Label>
                            </td>
                            <td class="tableDataAlt" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:UpdatePanel ID="udpWaste" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                         <asp:AsyncPostBackTrigger ControlID="ddlMetricCategory" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <asp:TextBox ID="tbWasteCode" runat="server" Columns="20" MaxLength="40"></asp:TextBox>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricDisposalCode" runat="server" text="UN Disposal Code"></asp:Label>
                            </td>
                            <td class="tableDataAlt" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:UpdatePanel ID="udpDisposal" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricDisposalCode" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlMetricDisposalCode" runat="server" OnSelectedIndexChanged="ddlUpdateLabel" AutoPostback="true"></asp:DropDownList>
                                        <asp:Label ID="lblDisposalDesc" runat="server" CssClass="refText"></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricRegStatus" runat="server" text="Regulatory Status"></asp:Label>
                            </td>
                            <td class="tableDataAlt" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:DropDownList ID="ddlMetricRegStatus" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricCurrency" runat="server" Text="<%$ Resources:LocalizedText, BillingCurrency %>"></asp:Label>
                            </td>
                            <td class="required" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:DropDownList ID="ddlMetricCurrency" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricUOM" runat="server" Text="<%$ Resources:LocalizedText, InvoiceUOM %>"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td CLASS="tableDataAlt" >
                                <asp:UpdatePanel ID="udpMetricUOM" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricUOM" />
                                        <asp:AsyncPostBackTrigger ControlID="btnAddUOM" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <div id="divMetricUOM" runat="server" visible=true>
                                            <asp:DropDownList ID="ddlMetricUOM" runat="server" OnSelectedIndexChanged="ddlUOMChanged" AutoPostback="true"></asp:DropDownList>
                                            <asp:Button id="btnAddUOM" runat="server" Text="Add" OnClick="btnAddUOM_Click"/>
                                        </div>
                                        <div id="divUserUOM" runat="server" visible="false">
                                            <asp:Label ID="lblUserUOMInstruction" runat="server" CssClass="prompt" text="Create custom UOM specific to this Profile"></asp:Label>
                                            <table width="98%"  border="0" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td>
                                                       <asp:Label ID="lblUserUOMName" runat="server" Text="<%$ Resources:LocalizedText, Name %>" CssClass="textStd"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="tbUserUOMName" runat="server" maxlength="100" Columns="24"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblUserUOMCode" runat="server" text="Abreviation" CssClass="textStd"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="tbUserUOMCode" runat="server" maxlength="50" Columns="8"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblUserUOMConvertTo" runat="server" text="Converts To" CssClass="textStd"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlUserUOMConvertTo" runat="server"></asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblUserUOMConversionFactor" runat="server" text="Factor (x=)" CssClass="textStd"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="tbUserUOMConversionFactor" runat="server" maxlength="12" Columns="10"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td>
                                                        <asp:CheckBox ID="cbUserUOM" runat="server" Text="Apply" ToolTip="UOM will be saved when the Update Metric button is pressed" CssClass="textStd"/>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricResponsible" runat="server" Text="<%$ Resources:LocalizedText, PersonResponsible %>"></asp:Label>
                            </td>
                            <td class="required" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:DropDownList ID="ddlMetricResponsible" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricNegValue" runat="server" text="Allow Credited Values"></asp:Label>
                            </td>
                            <td class="tableDataAlt" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:CheckBox id="cbMetricNegValue" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricRequired" runat="server" Text="<%$ Resources:LocalizedText, Required %>"></asp:Label>
                            </td>
                            <td class="tableDataAlt" >&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:CheckBox id="cbMetricRequired" runat="server"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblMetricStatus" runat="server" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlMetricStatus" runat="server"></asp:DropDownList></td>
                        </tr>
                    </table>
                    <br />
                    <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder1">
                    <tr>
                        <td class="columnHeader" width="60%">
                            <asp:Label ID="lblInputsDue" runat="server" text="Day of Month when Inputs are Due"></asp:Label>
                        </td>
                        <td class="required" width="1%">&nbsp;</td>
                        <td CLASS="tableDataAlt" width="39%">
                            <asp:DropDownList ID="ddlDayDue" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:Label ID="lblInputsWarning" runat="server" text="Reminder Prior to Due Date (Days)"></asp:Label>
                        </td>
                        <td class="required">&nbsp;</td>
                        <td CLASS="tableDataAlt">
                            <asp:DropDownList ID="ddlWarningDays" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Panel>

