<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="EHS_Profile.aspx.cs" Inherits="SQM.Website.EHS_Profile" %>
<%@ Register src="~/Include/Ucl_EHSList.ascx" TagName="EHSList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

<script type="text/javascript">

	window.onload = function () {
		var timeout = document.getElementById('hfTimeout').value;
		var timeoutWarn = ((parseInt(timeout) - 2) * 60000);
		window.setTimeout(function () { alert("Your Session Will Timeout In Approximately 2 Minutes.  Please save your work or your changes will be lost.") }, timeoutWarn);
	}

    function UpdateProfileButtons() {
        // document.getElementById("btnProfileMeasureNew").disabled = true;
    }

        function OpenMetricEditWindow() {
        $find("<%=winMetricEdit.ClientID %>").show();
        }

        function CloseMetricEditWindow() {
            var oWindow = GetRadWindow();  //Obtaining a reference to the current window
            oWindow.Close();
        }

        function ProfileChanged(ddl) {
            document.getElementById('btnMetricSave').disabled = false;
            document.getElementById("btnMetricCancel").disabled = false;
        }

        function ValidProfile() {
        	var status = confirmChange('<%= GetGlobalResourceObject("LocalizedText","ENVProfileValidate1") %>');
            if (status == true) {
            	status = checkForSelect(document.getElementById("ddlFinalApprover"), '<%= GetGlobalResourceObject("LocalizedText","ENVProfileValidate2") %>');
            }
            return status;
        }

        function MetricStatusChange() {
            var ddl = document.getElementById("ctl00_ContentPlaceHolder_Body_winMetricEdit_C_ddlMetricStatus");
            var value = ddl.options[ddl.selectedIndex].value
            if (value != null) {
                if (value == "D") {
                    ddl.style.backgroundColor = "#f08080";
                }
                else {
                    ddl.style.backgroundColor = "#FCFCFC";
                }
            }
        }

        function MetricStatusConfirm() {
            var status = true;
            var ddl = document.getElementById("ctl00_ContentPlaceHolder_Body_winMetricEdit_C_ddlMetricStatus");
            //ddl.style.backgroundColor = "black";
            var value = ddl.options[ddl.selectedIndex].value
            if (value != null) {
                if (value == "D") {
                	status = confirmAction('<%= GetGlobalResourceObject("LocalizedText","ENVProfileDeleteConfirm") %>');
                    if (status == true) {
                    	status = confirm('<%= GetGlobalResourceObject("LocalizedText","ENVProfileDeleteConfirm2") %>');
                    }
                }
            }
            return status;
        }

</script>


<div class="admin_tabs">
    <table width="100%" border="0" cellspacing="0" cellpadding="1">
        <tr>
            <td class="tabActiveTableBg" colspan="10" align="center">
			    <BR/>
               <%-- <FORM name="dummy">--%>
				<asp:HiddenField id="hfTimeout" runat="server"/>
                <asp:HiddenField ID="hfBase" runat="server" />
                <asp:HiddenField ID="hfOper" runat="server" />
                <asp:HiddenField ID="hfAddMetric" runat ="server" Value="<%$ Resources:LocalizedText, ENVProfileAddMetric %>" />
                <asp:HiddenField ID="hfUpdateMetric" runat ="server" Value="<%$ Resources:LocalizedText, ENVProfileUpdateMetric %>" />
                <asp:HiddenField id="hfErrRequiredInputs" runat="server" Value="<%$ Resources:LocalizedText, ENVProfileRequiredsMsg %>"/>
                <asp:HiddenField id="hfErrMetricHasHistory" runat="server" Value="<%$ Resources:LocalizedText, ENVProfileMetricHasHistoryMsg %>"/>
                <asp:Panel runat="server" ID="pnlSearchBar">
                    <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                </asp:Panel>
                <table width="100%">
			        <tr>
                        <td>
                            <asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text="Monitor energy consumption and waste streams for the selected business location on a monthly basis" meta:resourcekey="lblPageInstructionsResource1"></asp:Label>
                            <asp:Label ID="lblTitle" runat="server" Text="<%$ Resources:LocalizedText, ENVProfileValidate1 %>" Visible="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Ucl:EHSList id="uclInputHdr" runat="server"/>
                        </td>
                    </tr>
                </table>

                <div id="divMessages" runat="server">
                    <asp:Label ID="lblProfileNotExist" runat="server" Text="<%$ Resources:LocalizedText, NoMetricProfile %>" CssClass="labelEmphasis" visible="False"></asp:Label>
                    <asp:Label ID="lblProfileError" runat="server" Text="An error occurred while attempting to open this profile" CssClass="labelEmphasis" visible="False" meta:resourcekey="lblProfileErrorResource1"></asp:Label>
                    <asp:Label ID="lblNoMetrics" runat="server" Text="<%$ Resources:LocalizedText, NoMetricsDefined %>" CssClass="labelEmphasis" visible="False"></asp:Label>
                    <asp:Label ID="lblCopyError" runat="server" Text="An error occurred when attempting to copy the selected profile." CssClass="labelEmphasis" visible="False" meta:resourcekey="lblCopyErrorResource1"></asp:Label>
                </div>

                <div ID="divEHSProfile" runat="server" Visible = "false" >

                    <table width="99.5%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 2px;">
		                <tr>
			                <td class="navSectionBar" style="width: 20px;">
                                <button type="button" id="btnToggleSelects" onclick="ToggleSection('pnlProfileEdit');" class="navSectionBtn">
                                    <img id="pnlProfileEdit_img" src="/images/defaulticon/16x16/arrow-8-right.png"/>
                                </button>
                            </td>
                            <td class="navSectionBar">
                                <asp:Label runat="server" ID="lblProfileEdit" CssClass="prompt" Text="Profile Settings" meta:resourcekey="lblProfileEditResource1" ></asp:Label>
                            </td>
		                </tr>
	                </table>

                    <asp:Panel id="pnlProfileEdit" runat="server">
                        <table width="99.5%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 2px;">
                            <tr>
                                <td class="columnHeader" width="35%">
                                    <asp:Label ID="lblDisplayOption" runat="server" text="Metric Display Order" meta:resourcekey="lblDisplayOptionResource1"></asp:Label>
                                </td>
                                <td class="tableDataAlt" width="1%">&nbsp;</td>
                                <td CLASS="tableDataAlt">
                                    <asp:DropDownList ID="ddlDisplayOrder" runat="server" CssClass="WarnIfChanged" onchange="ProfileChanged(this);">
                                        <asp:ListItem meta:resourcekey="ListItemResource1">By Category</asp:ListItem>
                                        <asp:ListItem meta:resourcekey="ListItemResource2">By Metric Code</asp:ListItem>
                                        <asp:ListItem meta:resourcekey="ListItemResource3">By Metric Name</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Label ID="lblInputsDue" runat="server" text="Day of Month When Inputs Are Due" meta:resourcekey="lblInputsDueResource1"></asp:Label>
                                </td>
                                <td class="required">&nbsp;</td>
                                <td CLASS="tableDataAlt">
                                        <asp:DropDownList ID="ddlDayDue" runat="server" CssClass="WarnIfChanged" onchange="ProfileChanged(this);"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Label ID="lblInputsWarning" runat="server" text="Reminder Prior To Due Date (Days)" meta:resourcekey="lblInputsWarningResource1"></asp:Label>
                                </td>
                                <td class="required">&nbsp;</td>
                                <td CLASS="tableDataAlt">
                                    <asp:DropDownList ID="ddlWarningDays" runat="server" CssClass="WarnIfChanged" onchange="ProfileChanged(this);"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="columnHeader">
                                    <asp:Label ID="lblFinalApprover" runat="server" text="Local Approval By" meta:resourcekey="lblFinalApproverResource1"></asp:Label>
                                </td>
                                <td class="required">&nbsp;</td>
                                <td CLASS="tableDataAlt">
                                    <asp:DropDownList ID="ddlFinalApprover" runat="server" CssClass="WarnIfChanged" Width="250px" onchange="ProfileChanged(this);"></asp:DropDownList>
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="phNormFact" runat="server">
                                <tr>
                                    <td class="columnHeader">
                                        <asp:Label ID="lblNormFact" runat="server" text="Normalize Metrics By" meta:resourcekey="lblNormFactResource1"></asp:Label>
                                    </td>
                                    <td class="tableDataAlt">&nbsp;</td>
                                    <td CLASS="tableDataAlt">
                                        <asp:DropDownList ID="ddlNormFact" runat="server" CssClass="WarnIfChanged"></asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </table>
                        <span style="float: right; margin: 5px;">
                            <asp:Button ID="btnProfileCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" OnClick="btnProfileCancel_Click"></asp:Button>
                            <asp:Button ID="btnProfileSave" CSSclass="buttonEmphasis" runat="server" text="<%$ Resources:LocalizedText, SaveProfile %>"
                                                OnClientClick="return ValidProfile();" OnClick="btnProfileSave_Click"></asp:Button>
                        </span>
                    </asp:Panel>

                    <telerik:RadWindow runat="server" ID="winMetricEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="460px" Width="750px" Title="User Preferences" Behaviors="Move" Behavior="Move" meta:resourcekey="winMetricEditResource1">
                        <ContentTemplate>
                            <asp:Panel id="pnlMetricEdit" style="margin-top: 10px;" runat="server" Visible="False">
                                <asp:UpdatePanel ID="udpMain" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricCategory" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricID" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricDisposalCode" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlMetricUOM" />
                                        <asp:PostBackTrigger ControlID="btnMetricCancel" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <table width="99.5%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft" style="margin-top: 5px;">
                                            <tr>
                                                <td class="columnHeader" width="20%">
                                                    <asp:Label ID="lblMetricCode" runat="server" Text="<%$ Resources:LocalizedText, Metric %>"></asp:Label>
                                                </td>
                                                <td class="required" width="1%">&nbsp;</td>
                                                <td CLASS="tableDataAlt" width="79%">
                                                    <asp:DropDownList ID="ddlMetricCategory" runat="server" CssClass="WarnIfChanged" OnSelectedIndexChanged="UpdateMain" AutoPostback="True"></asp:DropDownList>
                                                    <br/>
							                        <span>
                                                        <asp:DropDownList ID="ddlMetricID" runat="server" CssClass="WarnIfChanged" OnSelectedIndexChanged="UpdateMain" AutoPostback="True" style="width: 400px; margin-top: 3px;"></asp:DropDownList>
                                                        <asp:Label ID="lblMetricName" runat="server" CssClass="refText"></asp:Label>
							                        </span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Label ID="lblMetricStatus" runat="server" text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                                </td>
                                                <td class="tableDataAlt">&nbsp;</td>
                                                <td CLASS="tableDataAlt">
                                                    <asp:DropDownList ID="ddlMetricStatus" runat="server" CssClass="WarnIfChanged" onChange="MetricStatusChange();"></asp:DropDownList>
                                                    &nbsp;&nbsp;
                                                    <asp:Label ID="lblMetricRequired" runat="server" CssClass="prompt" text="Required Monthly:" meta:resourcekey="lblMetricRequiredResource1"></asp:Label>
                                                    <asp:CheckBox id="cbMetricRequired" runat="server" CssClass="WarnIfChanged" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Label ID="lblMetricPrompt" runat="server" Text="<%$ Resources:LocalizedText, DisplayPrompt %>"></asp:Label>
                                                </td>
                                                <td class="tableDataAlt">&nbsp;</td>
                                                <td class="tableDataAlt">
                                                    <asp:TextBox ID="tbMetricPrompt"  runat="server" TextMode="MultiLine" rows="1" MaxLength="200" CssClass="textStd WarnIfChanged" style="width: 97%;" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="columnHeader">
                                                    <asp:Label ID="lblMetricResponsible" runat="server" Text="<%$ Resources:LocalizedText, PersonResponsible %>"></asp:Label>
                                                </td>
                                                <td class="required">&nbsp;</td>
                                                <td class="tableDataAlt">
                                                    <asp:DropDownList ID="ddlMetricResponsible" runat="server" CssClass="WarnIfChanged" Width="250px"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <asp:PlaceHolder ID="phCostWaste" runat="server">
                                                <tr>
                                                    <td class="columnHeader">
                                                        <asp:Label ID="lblMetricNegValue" runat="server" text="Invoice Type" meta:resourcekey="lblMetricNegValueResource1"></asp:Label>
                                                    </td>
                                                    <td id="tdMetricCost" runat="server" class="tableDataAlt">&nbsp;</td>
                                                    <td CLASS="tableDataAlt">
                                                        <asp:DropDownList ID="ddlMetricCost" runat="server" CssClass="WarnIfChanged"></asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="columnHeader">
                                                        <asp:Label ID="lblMetricCurrency" runat="server" Text="<%$ Resources:LocalizedText, BillingCurrency %>"></asp:Label>
                                                    <td id="tdCurrency" runat="server" class="required">&nbsp;</td>
                                                    <td CLASS="tableDataAlt">
                                                        <asp:DropDownList ID="ddlMetricCurrency" runat="server" CssClass="WarnIfChanged"></asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr id="tdRegStatusHdr" runat="server">
                                                    <td  class="columnHeader" runat="server">
                                                        <asp:Label ID="lblMetricRegStatus" runat="server" text="Regulatory Status"></asp:Label>
                                                    </td>
                                                    <td id="tdRegStatus" runat="server" class="tableDataAlt">&nbsp;</td>
                                                    <td CLASS="tableDataAlt" runat="server">
                                                        <asp:DropDownList ID="ddlMetricRegStatus" runat="server" CssClass="WarnIfChanged"></asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr id="tdDisposalHdr" runat="server">
                                                    <td  class="columnHeader" runat="server">
                                                        <asp:Label ID="lblMetricDisposalCode" runat="server" text="UN Disposal Code"></asp:Label>
                                                    </td>
                                                    <td id="tdDisposal" runat="server" class="tableDataAlt">&nbsp;</td>
                                                    <td class="tableDataAlt" runat="server">
                                                        <asp:DropDownList ID="ddlMetricDisposalCode" runat="server" CssClass="WarnIfChanged" OnSelectedIndexChanged="UpdateMain" AutoPostback="True" Width="65px"></asp:DropDownList>
                                                        &nbsp;
                                                        <asp:Label ID="lblDisposalDesc" runat="server" CssClass="refText"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr id="tdWasteCodeHdr" runat="server">
                                                    <td  class="columnHeader" runat="server">
                                                        <asp:Label ID="lblWasteCode" runat="server" text="Country Waste Code"></asp:Label>
                                                    </td>
                                                    <td id="tdWasteCode" runat="server" class="tableDataAlt">&nbsp;</td>
                                                    <td class="tableDataAlt" runat="server">
                                                        <asp:TextBox ID="tbWasteCode" runat="server" CssClass="textStd WarnIfChanged" Columns="40" MaxLength="40"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="columnHeader">
                                                        <asp:Label ID="lblMetricUOM" runat="server" Text="<%$ Resources:LocalizedText, InvoiceUOM %>"></asp:Label>
                                                    </td>
                                                    <td id="tdUOM" runat="server" class="tableDataAlt">&nbsp;</td>
                                                    <td CLASS="tableDataAlt">
                                                        <asp:DropDownList ID="ddlMetricUOM" runat="server" CssClass="WarnIfChanged" OnSelectedIndexChanged="ddlUOMChanged" AutoPostback="True"></asp:DropDownList>
                                                        <span id="spUOMFactor" runat="server" visible="False">
                                                            &nbsp;
                                                            <asp:Label ID="lblUOMFactor" runat="server" text="Conversion Factor:" CssClass="prompt" meta:resourcekey="lblUOMFactorResource1"></asp:Label>
                                                            <asp:TextBox ID="tbUOMFactor" runat="server" CssClass="textStd WarnIfChanged" maxlength="12" Columns="16"></asp:TextBox>
                                                            <asp:Label ID="lblUOMFactorStd" runat="server" Text="x invoice quantity = weight in Kg" CssClass="refText" meta:resourcekey="lblUOMFactorStdResource1"></asp:Label>
                                                        </span>
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="phMetricExt" runat="server">
                                                <tr>
                                                    <td class="columnHeader">
                                                        <asp:Label ID="lblInputDefault" runat="server" text="Default Inputs" meta:resourcekey="lblInputDefaultResource1"></asp:Label>
                                                    </td>
                                                    <td class="tableDataAlt">&nbsp;</td>
                                                    <td CLASS="tableDataAlt" >
                                                        <div style="background-color: #e8e8e8; width: 98%; padding: 3px;">
                                                            <asp:Label ID="lblValueDflt" runat="server" CssClass="prompt" text="Quantity: " meta:resourcekey="lblValueDfltResource1"></asp:Label>
                                                            <asp:TextBox ID="tbValueDflt" runat="server" CssClass="textStd WarnIfChanged" maxlength="20" Columns="13" onblur="<%$ Resources:LocalizedText, NumericInputConfirm %>"></asp:TextBox>
                                                            &nbsp;
                                                            <asp:Label ID="lblCostDflt" runat="server" CssClass="prompt" text="Cost: " meta:resourcekey="lblCostDfltResource1"></asp:Label>
                                                            <asp:TextBox ID="tbCostDflt" runat="server" CssClass="textStd WarnIfChanged" maxlength="20" Columns="13" onblur="<%$ Resources:LocalizedText, NumericInputConfirm %>"></asp:TextBox>
                                                            &nbsp;
                                                            <asp:Label ID="lblEnableOverride" runat="server" CssClass="prompt" text="Allow Override: " meta:resourcekey="lblEnableOverrideResource1"></asp:Label>
                                                            <asp:CheckBox id="cbEnableOverride" runat="server" CssClass="WarnIfChanged" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <br />
                                <span style="float: right; margin: 5px;">
                                    <asp:Button ID="btnMetricCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" enabled=False
                                        OnClick="btnMetricClear_Click"></asp:Button>
                                    <asp:Button ID="btnMetricSave" CSSclass="buttonEmphasis" runat="server" text="<%$ Resources:LocalizedText, SaveMetric %>"
                                        OnClientClick="return MetricStatusConfirm();" OnClick="btnMetricSave_Click"></asp:Button>
                                </span>
                                <br />
                                <center>
                                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
                                </center>
                            </asp:Panel>
                        </ContentTemplate>
                    </telerik:RadWindow>

                    <div id="divMetricListGVScroll" runat="server" class="" style="width: 99.5%; margin-top: 10px;">
                        <asp:LinkButton ID="lnkMeasureAdd" runat="server" CssClass="buttonAdd" style="float:left; margin: 0 0 4px 0;" Text="Add Metric" ToolTip="Add a metric to this profile" OnClick="btnMetricAdd_Click" meta:resourcekey="lnkMeasureAddResource1"></asp:LinkButton>
                        <br style="clear: both;" />
                        <asp:GridView  runat="server" ID="gvMetricList" Name="gvMetricList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True" cellpadding="1" PageSize="20" AllowSorting="True" Width="100%" OnRowDataBound="gvOnProfileRowDataBound">
                            <HeaderStyle CssClass="HeadingCellTextLeft" />
                            <RowStyle CssClass="DataCell" />
                	        <Columns>
                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Metric %>" ItemStyle-Width="22%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkMetricName" runat="server" CommandArgument='<%# Eval("PRMR_ID") %>' CSSClass="linkUnderline"
										        Text='<%# Eval("EHS_MEASURE.MEASURE_NAME") %>' OnClick="lnkMetricList_Click"></asp:LinkButton>
                                        <asp:Label ID="lblMetricPrompt" runat="server" CssClass="refText" visible="False"></asp:Label>
                                        <asp:HiddenField id="hfMetricStatus" runat="server" value='<%# Eval("STATUS") %>'/>
                                        <asp:HiddenField id="hfMetricPrompt" runat="server" value='<%# Eval("MEASURE_PROMPT") %>'/>
                                    </ItemTemplate>

								<ItemStyle Width="22%"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="M Code" meta:resourcekey="TemplateFieldResource2">
							        <ItemTemplate>
                                        <asp:HiddenField id="hfMetricCategory" runat="server" value='<%# Eval("EHS_MEASURE.MEASURE_CATEGORY") %>' />
								        <asp:LinkButton ID="lnkMetricCD" runat="server" CommandArgument='<%# Eval("PRMR_ID") %>' CSSClass="linkUnderline"
										    Text='<%# Eval("EHS_MEASURE.MEASURE_CD") %>' OnClientClick="UpdateProfileButtons();" OnClick="lnkMetricList_Click"></asp:LinkButton>
                                    </ItemTemplate>
							    </asp:TemplateField>
                                <asp:TemplateField HeaderText="" ItemStyle-Width="22px" ItemStyle-HorizontalAlign=Center>
                                    <ItemTemplate>
                                        <asp:HiddenField id="hfMetricRegStatus" runat="server" value='<%# Eval("REG_STATUS") %>' />
                                        <asp:HiddenField id="hfDisposalCode" runat="server" value='<%# Eval("UN_CODE") %>' />
                                            <asp:Image ID="imgHazardType" ToolTip="<%$ Resources:LocalizedText, EnergyInput %>" runat="server" style="vertical-align: middle;" />
                                    </ItemTemplate>

								<ItemStyle HorizontalAlign="Center" Width="22px"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reqd" ItemStyle-HorizontalAlign=Center meta:resourcekey="TemplateFieldResource4">
                                    <ItemTemplate>
                                        <asp:HiddenField id="hfMetricRequired" runat="server" Value='<%# Eval("IS_REQUIRED") %>'/>
                                        <asp:CheckBox id="cbMetricRequired" runat="server" Enabled="False" />
                                        <asp:Image id="imgStatus" runat="server" Visible="False" ToolTip="Metric is inactive" style="vertical-align: middle;" meta:resourcekey="imgStatusResource1"/>
                                    </ItemTemplate>

<ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Responsible" meta:resourcekey="TemplateFieldResource5">
                                    <ItemTemplate>
                                        <asp:Label ID="lblMetricResponsible" runat="server" Text='<%# Eval("PERSON.LAST_NAME",Eval("PERSON.FIRST_NAME","{0}")+" {0}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Invoice Type" meta:resourcekey="TemplateFieldResource6">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInvoiceType" runat="server" Text='<%# Eval("NEG_VALUE_ALLOWED") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Currency" meta:resourcekey="TemplateFieldResource7">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCurrency" runat="server" Text='<%# Eval("DEFAULT_CURRENCY_CODE") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, InvoiceUOM %>">
                                    <ItemTemplate>
                                        <asp:Label ID="lblInvoiceUOM" runat="server" Text='<%# Eval("DEFAULT_UOM") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="UN Code" meta:resourcekey="TemplateFieldResource9">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUNCode" runat="server" Text='<%# Eval("UN_CODE") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Waste Code" meta:resourcekey="TemplateFieldResource10">
                                    <ItemTemplate>
                                        <asp:Label ID="lblWasteCode" runat="server" Text='<%# Eval("WASTE_CODE") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:Label runat="server" ID="lblMetricListEmpty" Text="<%$ Resources:LocalizedText, NoMetricDefinedBusLoc %>" class="GridEmpty" Visible="False"></asp:Label>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlCopyProfile" runat="server" style="float: left; margin-left: 15px;" Visible="False">
                    <telerik:RadComboBox ID="ddlCopyProfile" runat="server" LabelCssClass="prompt" Label="Copy Profile From Location: " ZIndex=9000 Skin="Metro" height="300px" Width="280px"
                        OnClientLoad="DisableComboSeparators" OnSelectedIndexChanged="ddlCopyProfileSelect" AutoPostback="True" ToolTip="Create new profile by copying metrics from an existing profile" meta:resourcekey="ddlCopyProfileResource1" ></telerik:RadComboBox>
                    <br />
                    <telerik:RadComboBox ID="ddlDefaultResponsible" runat="server" LabelCssClass="prompt" Label="Default Responsible: " ZIndex=9000 Skin="Metro" height="300px" Width="280px" ToolTip="Select default responsible person for metric inputs" meta:resourcekey="ddlDefaultResponsibleResource1" ></telerik:RadComboBox>
                    <asp:Button id="btnCopyProfile" runat="server" CssClass="buttonStd" style="margin-left: 10px;" Text="Copy"  OnClientClick="<%$ Resources:LocalizedText, ENVProfileCopyConfirm %>" OnClick="btnCopyProfile_Click" Enabled="False" meta:resourcekey="btnCopyProfileResource1"/>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <br />
</div>

</asp:Content>
