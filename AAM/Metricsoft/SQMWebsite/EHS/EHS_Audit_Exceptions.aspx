<%@ Page Title="EHS Audit_Exceptions" Language="C#" MasterPageFile="~/RspPSMaster.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_Audit_Exceptions.aspx.cs" ClientIDMode="AutoID"
    Inherits="SQM.Website.EHS_Audit_Exceptions" ValidateRequest="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AuditExceptionList.ascx" TagName="AuditExceptionList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_TaskStatus.ascx" TagName="Task" TagPrefix="Ucl" %>

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

        function OpenUpdateTaskWindow() {
            $find("<%=winUpdateTask.ClientID %>").show();
        }

        function OpenUpdateAnswerStatusWindow() {
            $find("<%=winUpdateAnswerStatus.ClientID %>").show();
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
                        <asp:Label ID="lblViewEHSRezTitle" runat="server" CssClass="pageTitles" Text="Environmental Health &amp; Safety Assessment Exceptions"></asp:Label></span>

                        <br class="clearfix visible-xs-block" />

                        <asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="View EH&amp;S Assessment Exceptions below."></asp:Label>
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
                                <asp:Label runat="server" ID="lblAuditType" Text="Audit Type:" CssClass="prompt"></asp:Label>
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


                <div class="noprint">
                    <Ucl:AuditExceptionList ID="uclAuditExceptionList" runat="server" />
                </div>
            </div>

            <telerik:RadWindow runat="server" ID="winUpdateTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="700" Behaviors="Move" Title="Create Task">
                <ContentTemplate>
                    <Ucl:Task ID="uclTask" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadWindow runat="server" ID="winUpdateAnswerStatus" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="300" Width="700" Behaviors="Move" Title="Update Status">
                <ContentTemplate>
                    <div class="container-fluid" style="margin-top: 10px;">
                        <div class="row">
                            <div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 32px;">
                                <asp:Label ID="lblAnswerStatus" runat="server" Text="Status" CssClass="prompt"></asp:Label>
                            </div>
                            <div class="col-xs-12 col-sm-8 text-left greyControlCol">
                                <telerik:RadComboBox ID="ddlAnswerStatus" runat="server" Skin="Metro" ZIndex="9000" Width="90%" Height="330" AutoPostBack="false" EmptyMessage="select status"></telerik:RadComboBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-4 hidden-xs text-left tanLabelCol" style="height: 84px;">
                                <asp:Label ID="lblResolutionComment" runat="server" Text="Comments" CssClass="prompt"></asp:Label>
                            </div>
                            <div class="col-xs-12 col-sm-8 text-left greyControlCol">
                                <asp:TextBox ID="tbResolutionComment" Rows="4" Width="98%" TextMode="MultiLine" runat="server" CssClass="textStd"></asp:TextBox>
                            </div>
                        </div>
                        <br />
                        <div style="float: right; margin: 5px;">
                            <span>
                                <asp:Button ID="btnStatusSave" CssClass="buttonStd" runat="server" Text="Save" Style="margin: 5px;" OnClientClick="return confirmAction('update the assessment exception status');" OnClick="btnStatusSave_Click" ToolTip="update the status of this audit exception"></asp:Button>
                                <asp:Button ID="btnStatusCancel" CssClass="buttonEmphasis" runat="server" Text="Cancel" Style="margin: 5px;" OnClick="btnStatusCancel_Click"></asp:Button>
                            </span>
                        </div>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            </telerik:RadAjaxManager>
        </div>
    </div>
</asp:Content>
