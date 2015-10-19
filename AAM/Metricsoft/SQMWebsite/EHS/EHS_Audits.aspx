<%@ Page Title="EHS Audits" Language="C#" MasterPageFile="~/RspPSMaster.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Audits.aspx.cs" ClientIDMode="AutoID"
    Inherits="SQM.Website.EHS_Audits" ValidateRequest="false" %>

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
                        <asp:Label ID="lblViewEHSRezTitle" runat="server" CssClass="pageTitles" Text="Manage Environmental Health &amp; Safety Assessments"></asp:Label></span>

                        <br class="clearfix visible-xs-block" />

                        <div class="col-xs-7 col-sm-3">
                            <br />
                            <span style="clear: both; float: left; margin-top: -14px;">
                                <telerik:RadButton ID="rbNew" runat="server" Text="New Assessment" Icon-PrimaryIconUrl="/images/ico-plus.png"
                                    CssClass="metroIconButton" Skin="Metro" OnClick="rbNew_Click" CausesValidation="false" />
                            </span>
                        </div>

                        <br class="clearfix visible-xs-block" />

                        <asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="Add or update EH&amp;S Assessments below."></asp:Label>
                </div>
            </div>

            <br style="clear: both;" />
            <telerik:RadPersistenceManager ID="RadPersistenceManager1" runat="server"></telerik:RadPersistenceManager>

            <div id="divAuditList" runat="server" visible="true">
                <%--	$$$$$$$$$$$$$$ Audit Selection START $$$$$$$$$$$$$$$$$$$$$$$ --%>

                <div class="container-fluid summaryDataEnd" style="padding: 3px 4px 7px 0">

                    <div class="row-fluid">

                        <span style="float: left; width: 160px;">
                            <asp:Label runat="server" ID="lblPlantSelect" Text="Locations:" CssClass="prompt"></asp:Label>
                        </span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
									<br class="visible-xs-block" />
                        <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex="9000" Skin="Metro" Height="350" Width="650" OnClientLoad="DisableComboSeparators"></telerik:RadComboBox>

                        <div class="visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                    </div>

                    <asp:PlaceHolder ID="phAudit" runat="server">

                        <div class="row-fluid">


                            <span style="float: left; width: 160px;">
                                <asp:Label runat="server" ID="lblAuditType" Text="Assessment Type:" CssClass="prompt"></asp:Label>
                            </span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
                            <telerik:RadComboBox ID="rcbAuditType" runat="server" Style="margin-right: 15px;" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ToolTip="Select assessment types to list" Width="325" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>

                            <div class="clearfix visible-xs"></div>
                            <br class="visible-xs-block" style="margin-top: 7px;" />

                            <asp:Label runat="server" ID="lblStatus" Text="Status:" CssClass="prompt"></asp:Label>&nbsp;&nbsp;
                                            <telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="Select assessment status to list" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="Open" Value="A" />
                                                    <telerik:RadComboBoxItem Text="Closed" Value="C" />
                                                    <telerik:RadComboBoxItem Text="All" Value="" />
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
                            <asp:Label runat="server" ID="lblAuditDate" Text="Assessment Date From:" CssClass="prompt"></asp:Label>
                            <telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadDatePicker>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" />

                        <span>
                            <asp:Label runat="server" ID="lblToDate" Text="To:" CssClass="prompt"></asp:Label>
                            <telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="145" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadDatePicker>
                        </span>

                        <div class="clearfix visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                        <span class="noprint">
                            <%--<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt"></asp:Label>
                                        <span style="padding-top: 10px;""><asp:CheckBox id="cbShowImage" runat="server" Checked="false"/></span>--%>
                            <asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="Search" ToolTip="List assessments" OnClick="btnAuditsSearchClick" />
                        </span>

                    </div>
                </div>

                <%--	$$$$$$$$$$$$$$ Audit Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>


                <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2">

                    <div class="clearfix visible-xs"></div>
                    <br class="visible-xs-block" />

                    <div class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">

                        <%--                            <td valign="top" class="noprint">
                                        <asp:Label ID="lblChartType" runat="server" CssClass="prompt" Text="View Statistics: "></asp:Label>
                                        <telerik:RadComboBox ID="ddlChartType" runat="server" ZIndex="9000"  Width="350"  Skin="Metro" EmptyMessage="Select a chart" AutoPostBack="true" OnSelectedIndexChanged="ddlChartTypeChange">
                                        </telerik:RadComboBox>
                                        <p style="float: right; margin-right: 5px; width: 100px;">
                                            <asp:LinkButton ID="lnkPrint" runat="server" CssClass="buttonPrint" Text="Print" style="margin-right: 5px;"  Visible="false" OnClientClick="javascript:window.print()"></asp:LinkButton>
                                            <asp:LinkButton ID="lnkChartClose" runat="server" CssClass="buttonCancel" Visible="false"  OnClick="lnkCloseChart" ToolTip="Close"></asp:LinkButton>
                                        </p>
						</span>

					</div>

                        --%>
                        <%--<asp:Panel ID="pnlChartSection" runat="server" Width="100%">
                                <div class="row-fluid">
                                <div id="divChart" runat="server" class="borderSoft" style="height: 320px; width: 99%; padding: 10px 0;">
                                    <Ucl:RadGauge ID="uclChart" runat="server" />
                                </div>
                                 <div id="divChart" runat="server" class="borderSoft" style="width: 99%; padding: 10px 0;">
                                    <Ucl:RadGauge ID="uclChart" runat="server" />
                                </div>
                                </div>
                            </asp:Panel>--%>
                        <asp:Panel ID="pnlAuditDetails" runat="server" Width="100%" Visible="false">
                            <div class="row-fluid">
                                <br />
                                <asp:HiddenField ID="hfAuditDetails" runat="server" Value="Assessment # Summary" />
                                <asp:Label ID="lblAuditDetails" runat="server" CssClass="prompt"></asp:Label>
                                <asp:LinkButton ID="lnkAuditDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkCloseDetails" ToolTip="Close">
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

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            </telerik:RadAjaxManager>
        </div>
    </div>
</asp:Content>
