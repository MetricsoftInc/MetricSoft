<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSAuditForm.ascx.cs" Inherits="SQM.Website.Ucl_EHSAuditForm" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_EHSAuditDetails.ascx" TagName="AuditDetails" TagPrefix="Ucl" %>

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
    <div style="width: 100%; text-align: center; margin-bottom: 10px;"><a href="~/EHS/EHS_Audits.aspx" id="ahReturn" runat="server">
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


                                            <asp:Label ID="lblAuditType" class="textStd" runat="server">Assessment Type:</asp:Label>
                                            <telerik:RadDropDownList ID="rddlAuditType" runat="server" Width="450" AutoPostBack="true" CausesValidation="false"
                                                OnSelectedIndexChanged="rddlAuditType_SelectedIndexChanged" Skin="Metro">
                                            </telerik:RadDropDownList>

                                            <span class="hidden-xs" style="float: right; width: 160px;">
                                                <span class="requiredCloseStar">&bull;</span> - Required to Close
                                            </span>
                                        </div>
                                    </div>
                                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                        <tr runat="server">
                                            <td class="columnHeader">
                                                <asp:Label ID="lblLocation" runat="server" Text="Business Location"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:Label runat="server" ID="lblAuditLocation" Visible="false"></asp:Label>
                                                <telerik:RadComboBox ID="ddlAuditLocation" runat="server" Skin="Metro" Width="280" ZIndex="10" Font-Size="Small"
                                                    AutoPostBack="true" ToolTip="select an accesible business location" EnableCheckAllItemsCheckBox="false" OnSelectedIndexChanged="AuditLocation_Select" EnableViewState="true">
                                                </telerik:RadComboBox>
                                                <telerik:RadMenu ID="mnuAuditLocation" runat="server" Skin="Default" Width="280" Style="z-index: 9;" EnableAutoScroll="true" DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="AuditLocation_Select" EnableViewState="true"></telerik:RadMenu>
                                                <asp:HiddenField runat="server" ID="hdnAuditLocation" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblDescription" runat="server" Text="Description"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblAuditDescription" Visible="false"></asp:Label>
                                                <asp:TextBox ID="tbDescription" TextMode="MultiLine" Rows="2" Columns="50" MaxLength="4050" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblAuditPerson" runat="server" Text="Assessment Person"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblAuditPersonName" Visible="false"></asp:Label>
                                                <telerik:RadDropDownList ID="rddlAuditUsers" runat="server" CausesValidation="false" Skin="Metro" Width="400px"></telerik:RadDropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblAuditDate" runat="server" Text="Assessment Date"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblAuditDueDate" Visible="false"></asp:Label>
                                                <telerik:RadDatePicker ID="dmAuditDate" runat="server" CssClass="textStd" Width="145" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small"></telerik:RadDatePicker>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:Panel>
                        </div>
                        <br />
                            <asp:Panel ID="pnlDepartment" runat="server">
                                <div class="blueCell" style="padding: 7px;">
                                            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                                <tr>
                                                    <td  class="columnHeader" width="24%">
                                                        <asp:Label ID="lblDept" runat="server">Department:</asp:Label>
                                                    </td>
                                                    <td class="required" width="1%">&nbsp;</td>
                                                    <td class="tableDataAlt" width="75%">
                                                        <asp:Label runat="server" ID="lblDepartment" Visible="false"></asp:Label>
                                                        <telerik:RadDropDownList ID="rddlDepartment" runat="server" Width="450" AutoPostBack="true" CausesValidation="false" Skin="Metro">
                                                        </telerik:RadDropDownList>
                                                        <asp:RequiredFieldValidator ID="rfv1" runat="server" ControlToValidate="rddlDepartment" InitialValue="-1" ValidationGroup="Val" ErrorMessage="Please select a department" />
                                                    </td>
                                                </tr>
                                            </table>
                                </div>
                            </asp:Panel>
                        <br />
                        <ucl:AuditDetails id="uclAuditDetails" runat="server" />
                        <asp:Panel ID="pnlAddEdit" runat="server">

                            <div id="divForm" runat="server">
                                <asp:Panel ID="pnlForm" runat="server">
                                </asp:Panel>
                        
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 33%;">
                                            <telerik:RadButton ID="btnSaveReturn" runat="server" Text="Save &amp; Return" Visible="false"
                                                CssClass="UseSubmitAction" Width="88%" Skin="Metro" SingleClick="true" SingleClickText="Saving..."
                                                OnClick="btnSaveReturn_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />
                                        </td>
                                        <td style="width: 33%;">
                                            <telerik:RadButton ID="btnSaveContinue" runat="server" Text="Save &amp; Create Report" Visible="false"
                                                 Icon-SecondaryIconUrl="/images/ico-arr-rt-wht.png" SingleClick="true" SingleClickText="Saving..."
                                                CssClass="UseSubmitAction metroIconButtonSecondary" Width="88%" Skin="Metro"
                                                OnClick="btnSaveContinue_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />
                                        </td>
                                        <td style="width: 33%; text-align: center;">
                                            <telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
                                                Text="Delete Assessment" SingleClick="true" SingleClickText="Deleting..."
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
