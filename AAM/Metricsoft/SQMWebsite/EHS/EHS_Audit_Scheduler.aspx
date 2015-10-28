﻿<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Audit_Scheduler.aspx.cs" ClientIDMode="AutoID"
    Inherits="SQM.Website.EHS_Audit_Scheduler" ValidateRequest="false" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AuditScheduleList.ascx" TagName="AuditScheduleList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSAuditScheduleDetail.ascx" TagName="AuditScheduleDetail" TagPrefix="Ucl" %>

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
            args.set_cancel(!confirm("Delete assessment schedule - are you sure?  Assessment schedules cannot be undeleted."));
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
                        <asp:Label ID="lblViewEHSRezTitle" runat="server" CssClass="pageTitles" Text="Manage Environmental Health &amp; Safety Assessment Schedules" meta:resourcekey="lblViewEHSRezTitleResource1"></asp:Label></span>

                        <br class="clearfix visible-xs-block" />

                        <div class="col-xs-7 col-sm-3">
                            <br />
                            <span style="clear: both; float: left; margin-top: -14px;">
                                <telerik:RadButton ID="rbNew" runat="server" Text="New Assessment Schedule" Icon-PrimaryIconUrl="/images/ico-plus.png"
                                    CssClass="metroIconButton" Skin="Metro" OnClick="rbNew_Click" CausesValidation="False" meta:resourcekey="rbNewResource1" style="position: relative;" />
                            </span>
                        </div>

                        <br class="clearfix visible-xs-block" />

                        <asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="Add or update EH&amp;S Assessment Schedules below." meta:resourcekey="lblPageInstructionsResource1"></asp:Label>
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
                        </span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<br class="visible-xs-block" />
                        <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="650px" OnClientLoad="DisableComboSeparators" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>

                        <div class="visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                    </div>

                    <asp:PlaceHolder ID="phAudit" runat="server">

                        <div class="row-fluid">


                            <span style="float: left; width: 160px;">
                                <asp:Label runat="server" ID="lblAuditType" CssClass="prompt"></asp:Label>
                            </span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
                            <telerik:RadComboBox ID="rcbAuditType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="<%$ Resources:LocalizedText, SelectAssessmentTypes %>" Width="325" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" style="margin-top: 7px;" />

                            <asp:Label runat="server" ID="lblStatus" CssClass="prompt"></asp:Label>&nbsp;&nbsp;
                                            <telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="Select assessment schedules to list" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false" meta:resourcekey="rcbStatusSelectResource1">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Active %>" Value="A" />
                                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, Inactive %>" Value="I" />
                                                    <telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="" />
                                                </Items>
                                            </telerik:RadComboBox>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" />

                        </div>
                    </asp:PlaceHolder>

                    <div class="row-fluid" style="margin-top: 7px;">


                        <span style="float: left; margin-top: 4px;">
                            <asp:Label runat="server" ID="lblScheduleDay" Text="Scheduled Day:" CssClass="prompt" meta:resourcekey="lblScheduleDayResource1"></asp:Label>
                            <telerik:RadComboBox ID="rcbScheduleDay" runat="server" Style="margin-right: 15px;" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ToolTip="Select scheduled days to list" Width="325px" ZIndex="9000" Skin="Metro" meta:resourcekey="rcbScheduleDayResource1"></telerik:RadComboBox>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                        <span class="noprint">
                            <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, ListAssessmentSchedules %>" OnClick="btnAuditsSearchClick" />
                        </span>

                    </div>
                </div>

                <%--	$$$$$$$$$$$$$$ Audit Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>


                <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2" HorizontalAlign="NotSet">

                    <div class="clearfix visible-xs"></div>
                    <br class="visible-xs-block" />

                    <div class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">

                        <asp:Panel ID="pnlAuditDetails" runat="server" Width="100%" Visible="False">
                            <div class="row-fluid">
                                <br />
                                <asp:HiddenField ID="hfAuditDetails" runat="server" Value="Assessment # Summary" />
                                <asp:Label ID="lblAuditDetails" runat="server" CssClass="prompt"></asp:Label>
                                <asp:LinkButton ID="lnkAuditDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkCloseDetails" ToolTip="<%$ Resources:LocalizedText, Close %>">
                                             <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle;"/>
                                </asp:LinkButton>
                                <br />
                                <br />
                            </div>
                        </asp:Panel>
                </div>
                </telerik:RadAjaxPanel>

                <div class="noprint">
                    <Ucl:AuditScheduleList ID="uclAuditScheduleList" runat="server" />
                </div>
            </div>

            <Ucl:AuditScheduleDetail ID="uclAuditScheduleDetail" runat="server" />

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            </telerik:RadAjaxManager>
        </div>
    </div>
</asp:Content>
