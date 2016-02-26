<%@ Page Title="EHS_Audits" Language="C#" MasterPageFile="~/RspPSMaster.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Assessments.aspx.cs" ClientIDMode="AutoID"
    Inherits="SQM.Website.EHS.EHS_Assessments" ValidateRequest="false" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AuditList.ascx" TagName="AuditList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSAssessmentForm.ascx" TagName="AssessmentForm" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_TaskList.ascx" TagName="TaskList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_Attach.ascx" TagName="AttachWin" TagPrefix="Ucl" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        $(window).load(function () {
            document.getElementById('ctl00_ContentPlaceHolder_Body_hfwidth').value = $(window).width();
            document.getElementById('ctl00_ContentPlaceHolder_Body_hfheight').value = $(window).height();
        });

        $(window).resize(function () {
            document.getElementById('ctl00_ContentPlaceHolder_Body_hfwidth').value = $(window).width();
            document.getElementById('ctl00_ContentPlaceHolder_Body_hfheight').value = $(window).height();
        });

        function StandardConfirm(sender, args) {

            // Some pages will have no validators, so skip
            if (typeof Page_ClientValidate === "function") {
                var validated = Page_ClientValidate('Val');

                if (!validated)
                    alert("Please fill out all required fields.");
            }
        }
        function DeleteConfirm(button, args) {
            args.set_cancel(!confirm("Delete assessment - are you sure?  Assessments cannot be undeleted."));
        }

        <%--function OpenUpdateTaskWindow() {
            $find("<%=winUpdateTask.ClientID %>").show();
        }--%>

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
    <asp:HiddenField ID="hfwidth" runat="server" />
    <asp:HiddenField ID="hfheight" runat="server" />
    <div class="pageWrapper">

        <div class="container-fluid tabActiveTableBg">

            <div class="row-fluid">

                <div class="col-xs-12 col-sm-12">

                    <span style="float: left; margin-top: 6px;">
                        <asp:Label ID="lblViewEHSRezTitle" runat="server" CssClass="pageTitles" Text="Manage Environmental Health &amp; Safety Assessments" meta:resourcekey="lblViewEHSRezTitleResource1"></asp:Label></span>

                        <br class="clearfix visible-xs-block" />

                        <div class="col-xs-7 col-sm-3">
                            <br />
                            <span style="clear: both; float: left; margin-top: -14px;">
                                <telerik:RadButton ID="rbNew" runat="server" Text="New Assessment" Icon-PrimaryIconUrl="/images/ico-plus.png"
                                    CssClass="metroIconButton" Skin="Metro" OnClick="rbNew_Click" CausesValidation="False" meta:resourcekey="rbNewResource1" style="position: relative;" />
                            </span>
                        </div>

                        <br class="clearfix visible-xs-block" />

                        <asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="Add or update EH&amp;S Assessments below." meta:resourcekey="lblPageInstructionsResource1"></asp:Label>
                </div>
            </div>

            <br style="clear: both;" />
            <telerik:RadPersistenceManager ID="RadPersistenceManager1" runat="server"></telerik:RadPersistenceManager>

            <div id="divAuditList" runat="server" visible="true">
                <%--	$$$$$$$$$$$$$$ Audit Selection START $$$$$$$$$$$$$$$$$$$$$$$ --%>

                <div class="container-fluid summaryDataEnd" style="padding: 3px 4px 7px 0">

                    <div class="row-fluid">

                        <span style="float: left; width: 160px;">
                            <asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt"></asp:Label>
                        </span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
                        <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="256px" OnClientLoad="DisableComboSeparators" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>

                        <div class="visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                    </div>

                    <asp:PlaceHolder ID="phAudit" runat="server">

                        <div class="row-fluid">


                            <span style="float: left; width: 160px;">
                                <asp:Label runat="server" ID="lblAuditType" CssClass="prompt" meta:resourcekey="lblAuditTypeResource1"></asp:Label>
                            </span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
                            <telerik:RadComboBox ID="rcbAuditType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="<%$ Resources:LocalizedText, SelectAssessmentTypes %>" Width="256" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" style="margin-top: 7px;" />

                            <span style="padding-left:12px;">
                             <asp:Label runat="server" ID="lblStatus" CssClass="prompt"></asp:Label>&nbsp;&nbsp;
                                            <telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="<%$ Resources:LocalizedText, SelectAssessmentStatus %>" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Open %>" Value="A" />
                                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Closed %>" Value="C" />
                                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="" />
                                                    <%--<telerik:RadComboBoxItem Text="Data Incomplete" Value="N" />--%>
                                                    <%--<telerik:RadComboBoxItem Text="Actions Pending" Value="T" />--%>
                                                </Items>
                                            </telerik:RadComboBox></span>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" />

                        </div>
                    </asp:PlaceHolder>

                    <div class="row-fluid" style="margin-top: 7px;">


                        <span style="float: left; margin-top: 4px;">
                            <span style="padding-right:20px;"><asp:Label runat="server" ID="lblAuditDate" Text="<%$ Resources:LocalizedText, AssessmentDateFrom %>" CssClass="prompt"></asp:Label></span>
                            <span style="margin-right: -10px !important;"><telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" meta:resourcekey="dmFromDateResource1">
<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;"></Calendar>

<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="">
<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

<FocusedStyle Resize="None"></FocusedStyle>

<DisabledStyle Resize="None"></DisabledStyle>

<InvalidStyle Resize="None"></InvalidStyle>

<HoveredStyle Resize="None"></HoveredStyle>

<EnabledStyle Resize="None"></EnabledStyle>
</DateInput>

<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
						</telerik:RadDatePicker></span>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" />

                        <span>
                            <span style="margin-left: 14px; padding-right:8px;"><asp:Label runat="server" ID="lblToDate" CssClass="prompt"></asp:Label>
                            <telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" meta:resourcekey="dmToDateResource1">
<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" runat="server"></Calendar>

<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="" runat="server">
<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

<FocusedStyle Resize="None"></FocusedStyle>

<DisabledStyle Resize="None"></DisabledStyle>

<InvalidStyle Resize="None"></InvalidStyle>

<HoveredStyle Resize="None"></HoveredStyle>

<EnabledStyle Resize="None"></EnabledStyle>
</DateInput>

<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
						</telerik:RadDatePicker></span>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                        <span class="noprint">
                            <%--<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt"></asp:Label>
                                        <span style="padding-top: 10px;""><asp:CheckBox id="cbShowImage" runat="server" Checked="false"/></span>--%>
                            <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListAssessments %>" OnClick="btnAuditsSearchClick" />
                        </span>

                    </div>
                </div>

                <%--	$$$$$$$$$$$$$$ Audit Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>


<%--                <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2" HorizontalAlign="NotSet" meta:resourcekey="RadAjaxPanel2Resource1">--%>

                    <div class="clearfix visible-xs"></div>
                    <br class="visible-xs-block" />

                    <div id="divChartSelect" runat="server" class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">
						<span class="noprint">
							<asp:Label ID="lblChartType" runat="server" CssClass="prompt" Text="<%$ Resources:LocalizedText, ViewStatistics %>"></asp:Label>
							<telerik:RadComboBox ID="ddlChartType" runat="server" ZIndex="9000" Width="256px" Skin="Metro" EmptyMessage="<%$ Resources:LocalizedText, SelectAChart %>" AutoPostBack="True" OnSelectedIndexChanged="ddlChartTypeChange">
							</telerik:RadComboBox>
							<p style="float: right; margin-right: 5px; width: 100px;">
								<asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="<%$ Resources:LocalizedText, Print %>" Style="margin-right: 5px;" Visible="False" OnClientClick="javascript:window.print()"></asp:LinkButton>
								<asp:LinkButton ID="lnkChartClose" runat="server" CssClass="buttonCancel" Visible="False" OnClick="lnkCloseChart" ToolTip="<%$ Resources:LocalizedText, Close %>"></asp:LinkButton>
							</p>
						</span>
					</div>

                   <asp:Panel ID="pnlChartSection" runat="server" Width="100%">
						<div class="row-fluid">
							<div id="divChart" runat="server" class="borderSoft" style="width: 99%; padding: 10px 0;">
								<Ucl:RadGauge ID="uclChart" runat="server" />
                                <%--<div style="position: relative;">
                                    <div style="width: 30%; float: left;">
                                        <Ucl:RadGauge ID="uclChartPrev1" runat="server" />
                                    </div>
                                    <div style="width: 30%; float: left;">
                                        <Ucl:RadGauge ID="uclChartPrev2" runat="server" />
                                    </div>
                                    <div style="width: 30%; float: left;">
                                        <Ucl:RadGauge ID="uclChartPrev3" runat="server" />
                                    </div>
                                </div>--%>
                            </div>
                            <div  id="divDataResults" runat="server" class="borderSoft" style="width: 99%; padding: 10px 0;">
                                data report
                            </div>
 						</div>
					</asp:Panel>



                       <asp:Panel ID="pnlAuditDetails" runat="server" Width="100%" Visible="False" meta:resourcekey="pnlAuditDetailsResource1">
                            <div class="row-fluid">
                                <br />
                                <asp:Label ID="lblAuditDetails" runat="server" CssClass="prompt" meta:resourcekey="lblAuditDetailsResource1"></asp:Label>
                                <asp:LinkButton ID="lnkAuditDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkCloseDetails" ToolTip="<%$ Resources:LocalizedText, Close %>">
                                             <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle;"/>
                                </asp:LinkButton>
                                <br />
                                <br />
                            </div>
                        </asp:Panel>
                <%--</telerik:RadAjaxPanel>--%>


                <div class="noprint">
                    <Ucl:AuditList ID="uclAuditList" runat="server" />
                </div>
            </div>

            <div>
                <Ucl:AssessmentForm ID="uclAssessmentForm" runat="server" />
            </div>
            

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" meta:resourcekey="RadAjaxManager1Resource1">
            </telerik:RadAjaxManager>
        </div>
    </div>
    <Ucl:TaskList ID="uclTaskList" runat="server" />

    <Ucl:AttachWin ID="uclAttachWin" runat="server" />

</asp:Content>
