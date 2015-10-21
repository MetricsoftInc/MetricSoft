﻿<%@ Page Title="EHS Audits" Language="C#" MasterPageFile="~/RspPSMaster.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Audits.aspx.cs" ClientIDMode="AutoID"
    Inherits="SQM.Website.EHS_Audits" ValidateRequest="false" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AuditList.ascx" TagName="AuditList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSAuditForm.ascx" TagName="AuditForm" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>

<%@ Reference Control="~/Include/Ucl_RadAsyncUpload.ascx" %>

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
                            <asp:Label runat="server" ID="lblPlantSelect" Text="Locations:" CssClass="prompt" meta:resourcekey="lblPlantSelectResource1"></asp:Label>
                        </span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<br class="visible-xs-block" />
                        <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="650px" OnClientLoad="DisableComboSeparators" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>

                        <div class="visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                    </div>

                    <asp:PlaceHolder ID="phAudit" runat="server">

                        <div class="row-fluid">


                            <span style="float: left; width: 160px;">
                                <asp:Label runat="server" ID="lblAuditType" Text="Assessment Type:" CssClass="prompt" meta:resourcekey="lblAuditTypeResource1"></asp:Label>
                            </span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
                            <telerik:RadComboBox ID="rcbAuditType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="Select assessment types to list" Width="325" ZIndex="9000" Skin="Metro" AutoPostBack="false" meta:resourcekey="rcbAuditTypeResource1"></telerik:RadComboBox>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" style="margin-top: 7px;" />

                            <asp:Label runat="server" ID="lblStatus" Text="Status:" CssClass="prompt" meta:resourcekey="lblStatusResource1"></asp:Label>&nbsp;&nbsp;
                                            <telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="Select assessment status to list" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false" meta:resourcekey="rcbStatusSelectResource1">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="Open" Value="A" meta:resourcekey="RadComboBoxItemResource1" />
                                                    <telerik:RadComboBoxItem Text="Closed" Value="C" meta:resourcekey="RadComboBoxItemResource2" />
                                                    <telerik:RadComboBoxItem Text="All" Value="" meta:resourcekey="RadComboBoxItemResource3" />
                                                    <%--<telerik:RadComboBoxItem Text="Data Incomplete" Value="N" />--%>
                                                    <%--<telerik:RadComboBoxItem Text="Actions Pending" Value="T" />--%>
                                                </Items>
                                            </telerik:RadComboBox>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" />

                        </div>
                    </asp:PlaceHolder>

                    <div class="row-fluid" style="margin-top: 7px;">


                        <span style="float: left; margin-top: 4px;">
                            <asp:Label runat="server" ID="lblAuditDate" Text="Assessment Date From:" CssClass="prompt" meta:resourcekey="lblAuditDateResource1"></asp:Label>
                            <telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" Culture="en-US" meta:resourcekey="dmFromDateResource1">
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
						</telerik:RadDatePicker>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" />

                        <span>
                            <asp:Label runat="server" ID="lblToDate" Text="To:" CssClass="prompt" meta:resourcekey="lblToDateResource1"></asp:Label>
                            <telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" Culture="en-US" meta:resourcekey="dmToDateResource1">
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
						</telerik:RadDatePicker>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                        <span class="noprint">
                            <%--<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt"></asp:Label>
                                        <span style="padding-top: 10px;""><asp:CheckBox id="cbShowImage" runat="server" Checked="false"/></span>--%>
                            <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List assessments" OnClick="btnAuditsSearchClick" meta:resourcekey="btnSearchResource1" />
                        </span>

                    </div>
                </div>

                <%--	$$$$$$$$$$$$$$ Audit Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>


                <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2" HorizontalAlign="NotSet" meta:resourcekey="RadAjaxPanel2Resource1">

                    <div class="clearfix visible-xs"></div>
                    <br class="visible-xs-block" />

                    <div class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">

                        <asp:Panel ID="pnlAuditDetails" runat="server" Width="100%" Visible="False" meta:resourcekey="pnlAuditDetailsResource1">
                            <div class="row-fluid">
                                <br />
                                <asp:HiddenField ID="hfAuditDetails" runat="server" Value="Assessment # Summary" />
                                <asp:Label ID="lblAuditDetails" runat="server" CssClass="prompt" meta:resourcekey="lblAuditDetailsResource1"></asp:Label>
                                <asp:LinkButton ID="lnkAuditDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkCloseDetails" ToolTip="Close" meta:resourcekey="lnkAuditDetailsCloseResource1">
                                             <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle;"/>
                                </asp:LinkButton>
                                <br />
                                <br />
                            </div>
                        </asp:Panel>
                    </div>
                </telerik:RadAjaxPanel>


                <div class="noprint">
                    <Ucl:AuditList ID="uclAuditList" runat="server" />
                </div>
            </div>

            <Ucl:AuditForm ID="uclAuditForm" runat="server" />

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" meta:resourcekey="RadAjaxManager1Resource1">
            </telerik:RadAjaxManager>
        </div>
    </div>
</asp:Content>
