<%@ Page Title="EHS_Audits" Language="C#" MasterPageFile="~/RspPSMaster.Master"
    AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EHS_AssessmentForm.aspx.cs" ClientIDMode="AutoID"
    Inherits="SQM.Website.EHS.EHS_AssessmentForm" ValidateRequest="false" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_Attach.ascx" TagName="AttachWin" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_TaskList.ascx" TagName="TaskList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_TaskStatus.ascx" TagName="Task" TagPrefix="Ucl" %>

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

        function LeaveConfirm(button, args) {
            confirm("If you have made changes and not hit the Save button, changes may be lost. Leave Page?");
        }

        var body = document.body;
        var docElem = document.documentElement;
        var bodyScrollTop = 0;
        var bodyScrollLeft = 0;
        var docElemScrollTop = 0;
        var docElemScrollLeft = 0;
        function OnClientBeforeOpen(sender, args) {
            bodyScrollTop = body.scrollTop;
            bodyScrollLeft = body.scrollLeft;
            docElemScrollTop = docElem.scrollTop;
            docElemScrollLeft = docElem.scrollLeft;
        }
        function OnClientClose() {
            setTimeout(function () {
                body.scrollTop = bodyScrollTop;
                body.scrollLeft = bodyScrollLeft;
                docElem.scrollTop = docElemScrollTop;
                docElem.scrollLeft = docElemScrollLeft;
            }, 30);
        }

        function OpenTaskWindow() {
            $find("<%=winUpdateTask.ClientID %>").show();
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

                    <asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="Add or update EH&amp;S Assessments below." meta:resourcekey="lblPageInstructionsResource1"></asp:Label>
                </div>
            </div>

            <br style="clear: both;" />
            <telerik:RadPersistenceManager ID="RadPersistenceManager1" runat="server"></telerik:RadPersistenceManager>

            <div runat="server" class="row-fluid" id="divAuditDetails">
                <div style="width: 100%; text-align: center; margin-bottom: 10px;">
                    <asp:LinkButton runat="server" ID="lnkReturn" OnClick="lnkReturn_Click"><img src="/images/defaulticon/16x16/arrow-7-up.png" style="vertical-align: middle; border: 0;" border="0" alt="" />
                        Return to List</asp:LinkButton>

                </div>
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
                                                        <asp:HiddenField runat="server" ID="hdnAuditId" />
                                                        <span class="hidden-xs" style="float: right; width: 160px; margin-right: 6px;">
                                                            <span class="requiredStar">&bull;</span> - Required to Create
                                                        </span>

                                                        <div class="clearfix visible-xs-block"></div>
                                                        <br style="clear: both;" />


                                                        <asp:Label ID="lblAuditType" class="textStd" runat="server" Text="<%$ Resources:LocalizedText, AssessmentType %>" />
                                                        <telerik:RadDropDownList ID="rddlAuditType" runat="server" Width="450" Skin="Metro">
                                                        </telerik:RadDropDownList>

                                                        <span class="hidden-xs" style="float: right; width: 160px;">
                                                            <span class="requiredCloseStar">&bull;</span> - Required to Close
                                                        </span>
                                                    </div>
                                                </div>
                                                <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                                    <tr runat="server">
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblLocation" runat="server" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                                                        </td>
                                                        <td class="required" width="1%">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <asp:Label runat="server" ID="lblAuditLocation" Visible="false"></asp:Label>
                                                            <telerik:RadComboBox ID="ddlAuditLocation" runat="server" Skin="Metro" Width="280" ZIndex="10" Font-Size="Small"
                                                                AutoPostBack="true" ToolTip="select an accesible business location" EnableCheckAllItemsCheckBox="false" EmptyMessage="<%$ Resources:LocalizedText, Select %>" OnSelectedIndexChanged="AuditLocation_Select" EnableViewState="true">
                                                            </telerik:RadComboBox>
                                                            <telerik:RadMenu ID="mnuAuditLocation" runat="server" Skin="Default" Width="280" Style="z-index: 9;" EnableAutoScroll="true" DefaultGroupSettings-Flow="Vertical" DefaultGroupSettings-RepeatDirection="Horizontal" OnItemClick="AuditLocation_Select" EnableViewState="true"></telerik:RadMenu>
                                                            <asp:HiddenField runat="server" ID="hdnAuditLocation" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader" width="24%">
                                                            <asp:Label ID="lblDescription" runat="server" Text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
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
                                                            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:LocalizedText, AssessmentDate %>"></asp:Label>
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
                                                    <td class="columnHeader" width="24%">
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
                                 </telerik:RadAjaxPanel>
                                   <br />
                                        <div id="divForm" runat="server">
                                            <div id="divFormRepeater" runat="server" class="" visible="false" style="margin-top: 5px;">
                                                <asp:Repeater runat="server" ID="rptAuditFormTopics" ClientIDMode="AutoID" OnItemDataBound="rptAuditFormTopics_OnItemDataBound">
                                                    <HeaderTemplate><br/><table width="100%" cellpadding="6" cellspacing="0" style="border-collapse: collapse;"></HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr><td colspan="8" class="blueCell" style="width: 100%; font-weight: bold;"><%# Eval("TITLE") %></td></tr>
                                                            <asp:Repeater runat="server" ID="rptAuditFormQuestions" ClientIDMode="AutoID" OnItemDataBound="rptAuditFormQuestions_OnItemDataBound">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td class="tanCell auditquestion" style="width: 30%;"><%# Eval("QuestionText") %><asp:HiddenField runat="server" ID="hdnQuestionId" Value='<%# linkArgs(Eval("AuditId"), Eval("QuestionId")) %>' /></td>
                                                                        <td class="tanCell" style="width: 10px; padding-left: 0 !important;"><asp:Image runat="server" ID="imgHelp" ImageUrl="/images/ico-question.png" /><telerik:RadToolTip runat="server" ID="rttToolTip" IsClientID="false" RelativeTo="Element" Width="400" Height="200" Animation="Fade" Position="MiddleRight" ContentScrolling="Auto" Skin="Metro" HideEvent="LeaveTargetAndToolTip"></telerik:RadToolTip></td>
                                                                        <td class="tanCell" style="width: 10px; padding-left: 0 !important;"><asp:Literal runat="server" ID="litRequiredStar"></asp:Literal></td>
                                                                        <td class="greyCell" runat="server" id="tdCommentLeft"><telerik:RadTextBox runat="server" ID="rtbCommentLeft" TextMode="MultiLine" Rows="2" Resize="Both" Width="400"></telerik:RadTextBox></td>
                                                                        <td class="greyCell"><asp:RadioButtonList runat="server" ID="rblAnswers" CssClass="WarnIfChanged auditanswer" RepeatDirection="Horizontal"></asp:RadioButtonList></td>
                                                                        <td class="greyCell" runat="server" id="tdCommentRight"><telerik:RadTextBox runat="server" ID="rtbCommentRight" TextMode="MultiLine" Rows="2" Resize="Both" Width="400"></telerik:RadTextBox></td>
                                                                        <td class="greyCell" style="width: 10%;">
                                                                            <asp:LinkButton runat="server" ID="lnkAddTask" OnClientClick="OnClientBeforeOpen" OnClick="lnkAddTask_Click" ToolTip="Create a Task necessary to complete this exception" CommandArgument='<%# linkArgs(Eval("AuditId"), Eval("QuestionId")) %>' CausesValidation="false"></asp:LinkButton>
                                                                        </td>
                                                                        <td class="greyCell" style="width: 10%;">
                                                                            <asp:LinkButton ID="LnkAttachment" runat="server" ToolTip="Attachments" CommandArgument='<%# linkArgs(Eval("AuditId"), Eval("QuestionId")) %>' CssClass="refTextSmall" OnClick="lnkAddAttach" CausesValidation="false">
                                                                                <img src="/images/defaulticon/16x16/Attachment.png" alt="" style="vertical-align: middle; border: 0px;" />
                                                                            </asp:LinkButton>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        <tr><td colspan="8" class="greyCell" style="width: 100%; text-align: right; font-weight: bold;"><asp:Label runat="server" ID="lblTopicTotal"></asp:Label></td></tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <tr><td colspan="8" class="greyCell" style="width: 100%; text-align: right; font-weight: bold;">&nbsp;</td></tr>
                                                        <tr>
                                                            <td colspan="8" class="greyCell" style="width: 100%; text-align: right; font-weight: bold;">
                                                                <asp:Label runat="server" ID="lblTotalPossiblePoints"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="8" class="greyCell" style="width: 100%; text-align: right; font-weight: bold;">
                                                                <asp:Label runat="server" ID="lblTotalPointsAchieved"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="8" class="greyCell" style="width: 100%; text-align: right; font-weight: bold;">
                                                                <asp:Label runat="server" ID="lblTotalPointsPercentage"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        </table><br/>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                                <asp:HiddenField runat="server" ID="hdnAttachClick" />
                                            </div>
                                        </div>
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="text-align: right;"><asp:CheckBox runat="server" ID="cbClose" Text="<%$ Resources:LocalizedText, CloseAudit %>" /></td>
                                                    <td>
                                                        <asp:LinkButton ID="LnkAuditAttachment" runat="server" Text="<%$ Resources:LocalizedText, Attachments %>" ToolTip="<%$ Resources:LocalizedText, Attachments %>" CssClass="buttonAttach buttonPopupOpen" OnClick="lnkAddAttach" CausesValidation="false"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr><td colspan="2">&nbsp;</td></tr>
                                                <tr>
                                                    <td style="width: 33%;">
                                                        <telerik:RadButton ID="btnSaveReturn" runat="server" Text="<%$ Resources:LocalizedText, SaveAndReturn %>" Visible="false"
                                                            CssClass="UseSubmitAction" Width="88%" Skin="Metro" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Saving %>"
                                                            OnClick="btnSaveReturn_Click" ValidationGroup="Val" />
                                                    </td>
                                                    <td style="width: 33%; text-align: center;">
                                                        <telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
                                                            Text="Delete Assessment" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
                                                            OnClick="btnDelete_Click" OnClientClicking="DeleteConfirm" CssClass="UseSubmitAction" />
                                                    </td>
                                                </tr>
                                            </table>

                                <br />
                                <br />
                            </div>
                        </td>
                    </tr>
                </table>

            </div>

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            </telerik:RadAjaxManager>

        </div>
    </div>
<telerik:RadWindow runat="server" ID="winUpdateTask" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="True" Height="400px" Width="700px" Title="View/Update Task" Behavior="Close, Move" OnClientClose="OnClientClose">
	<ContentTemplate>
		<Ucl:Task ID="uclTask" runat="server" />
	</ContentTemplate>
</telerik:RadWindow>

    <Ucl:AttachWin ID="uclAttachWin" runat="server" />
</asp:Content>
