<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSAuditScheduleDetail.ascx.cs" Inherits="SQM.Website.Ucl_EHSAuditScheduleDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function OnEditorClientLoad(editor) {
        editor.attachEventHandler("ondblclick", function (e) {
                var sel = editor.getSelection().getParentElement(); //get the currently selected element
                var href = null;
                if (sel.tagName === "A") {
                    href = sel.href; //get the href value of the selected link
                    window.open(href, null, "height=500,width=500,status=no,toolbar=no,menubar=no,location=no");
                    return false;
                }
            }
        );
    }
</script>

<div id="divAuditForm" runat="server" visible="false">
    <div style="width: 100%; text-align: center; margin-bottom: 10px;"><a href="~/EHS/EHS_Audit_Scheduler.aspx" id="ahReturn" runat="server">
        <img src="/images/defaulticon/16x16/arrow-7-up.png" style="vertical-align: middle; border: 0;" border="0" alt="" />
        Return to List</a></div>
    <table style="width: 100%" class="textStd">
        <tr>
            <td>
                <div id="divPageBody" class="textStd" style="text-align: left; margin: 0 0px;" runat="server">
                    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
                        <asp:Label ID="lblResults" runat="server" />
						<div class="container-fluid blueCell" style="padding: 7px;"">

                            <asp:Panel ID="pnlAuditHeader" runat="server">
                                <div class="blueCell" style="padding: 7px;">

                                    <div class="row-fluid">

                                        <div class="col-xs-12  text-left">


                                            <asp:Label ID="lblAddOrEditAudit" class="textStd" runat="server"><strong>Add a New Assessment:</strong></asp:Label>

                                            <span class="hidden-xs" style="float: right; width: 160px; margin-right: 6px;">
                                                <span class="requiredStar">&bull;</span> - Required to Create
                                            </span>

                                            <div class="clearfix visible-xs-block"></div>
                                            <br style="clear: both;" />



                                            <span class="hidden-xs" style="float: right; width: 160px;">
                                                <span class="requiredCloseStar">&bull;</span> - Required to Close
                                            </span>
                                        </div>
                                    </div>
                                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                        <tr>
                                            <td class="columnHeader">
                                                <asp:Label runat="server" ID="lblAuditType" Text="<%$ Resources:LocalizedText, AssessmentType %>"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:Label ID="lblScheduleAuditType" runat="server" Visible="false"></asp:Label>
                                                <telerik:RadDropDownList ID="rddlAuditType" runat="server" Width="280" CausesValidation="false" Skin="Metro">
                                                </telerik:RadDropDownList>
                                            </td>
                                        </tr>
                                        <tr runat="server">
                                            <td class="columnHeader">
                                                <asp:Label ID="lblLocation" runat="server" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:Label runat="server" ID="lblAuditLocation" Visible="false"></asp:Label>
                                                <telerik:RadComboBox ID="ddlAuditLocation" runat="server" Skin="Metro" Width="280" ZIndex="10" Font-Size="Small" EmptyMessage="<%$ Resources:LocalizedText, Select %>"
                                                    AutoPostBack="true" ToolTip="select an accesible business location" EnableCheckAllItemsCheckBox="false" OnSelectedIndexChanged="AuditLocation_Select" EnableViewState="true">
                                                </telerik:RadComboBox>
                                                <telerik:RadMenu ID="mnuAuditLocation" runat="server" Skin="Default" Width="280" Style="z-index: 9;" EnableAutoScroll="true" DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="AuditLocation_Select" EnableViewState="true"></telerik:RadMenu>
                                                <asp:HiddenField runat="server" ID="hdnAuditLocation" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblJobcode" runat="server" Text="Assessment Group"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblAuditJobcode" Visible="false"></asp:Label>
                                                <telerik:RadDropDownList ID="rddlAuditJobcodes" runat="server" CausesValidation="false" Skin="Metro" Width="400px"></telerik:RadDropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblDayOfWeek" runat="server" Text="<%$ Resources:LocalizedText, DayOfWeek %>"></asp:Label>
                                            </td>
                                            <td class="required">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <telerik:RadDropDownList ID="rddlDayOfWeek" runat="server" CausesValidation="false" Skin="Metro" Width="400px"></telerik:RadDropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblInactive" runat="server" Text="<%$ Resources:LocalizedText, Inactive %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:CheckBox runat="server" ID="cbInactive" />
                                            </td>
                                        </tr>
                                    </table>

                                </div>
                            </asp:Panel>
                        </div>
                        <asp:Panel ID="pnlAddEdit" runat="server">

                            <div id="divForm" runat="server">
                                <asp:Panel ID="pnlForm" runat="server">
                                </asp:Panel>

                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 33%;">
                                            <telerik:RadButton ID="btnSaveReturn" runat="server" Text="<%$ Resources:LocalizedText, SaveAndReturn %>" Visible="true"
                                                CssClass="UseSubmitAction" Width="88%" Skin="Metro" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Saving %>"
                                                OnClick="btnSaveReturn_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />
                                        </td>
                                        <td style="width: 33%;">
                                        </td>
                                        <td style="width: 33%; text-align: center;">
                                            <telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
                                                Text="Delete Assessment" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
                                                OnClick="btnDelete_Click" OnClientClicking="DeleteConfirm" CssClass="UseSubmitAction" />
                                        </td>
                                    </tr>
                                </table>

                            </div>
                        </asp:Panel>

                    </telerik:RadAjaxPanel>
                    <br />
                    <br />
                </div>
            </td>
        </tr>
    </table>
</div>

<div id="divAuditReportForm" runat="server" visible="false">

</div>
