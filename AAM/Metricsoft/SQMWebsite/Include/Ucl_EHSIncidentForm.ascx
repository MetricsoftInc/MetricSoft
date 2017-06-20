<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Ucl_EHSIncidentForm.ascx.cs" Inherits="SQM.Website.Ucl_EHSIncidentForm" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_EHSIncidentDetails.ascx" TagName="IncidentDetails" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Contain.ascx" TagName="Containment" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Root5Y.ascx" TagName="RootCause" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Causation.ascx" TagName="Causation" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Action.ascx" TagName="Action" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Alert.ascx" TagName="Alert" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_INCFORM_Approval.ascx" TagName="Approval" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AttachVideoPanel.ascx" TagName="AttachVideoPanel" TagPrefix="Ucl" %>


<link rel="stylesheet" href="http://kendo.cdn.telerik.com/2017.2.504/styles/kendo.common.min.css" />
<link rel="stylesheet" href="http://kendo.cdn.telerik.com/2017.2.504/styles/kendo.rtl.min.css" />
<link rel="stylesheet" href="http://kendo.cdn.telerik.com/2017.2.504/styles/kendo.silver.min.css" />

<script src="http://kendo.cdn.telerik.com/2017.2.504/js/kendo.all.min.js"></script>

<style>
    #tbl_timeline tr td {
        padding-bottom: 5px;
    }

    input[name="DatePiker"] {
        width: 80%;
    }

    .linkButton {
        cursor: pointer;
        font-size: Smaller;
        margin: 7px;
        background: url(/images/plus.png) no-repeat 5px 0px;
        padding-left: 25px;
        padding-top: 0px;
        padding-bottom: 4px;
        border: 0;
        FONT-FAMILY: Verdana, Arial, Helvetica, sans-serif;
        font-size: 11px;
        font-weight: bold;
        color: #191970;
        text-decoration: underline;
    }


    #tbl_timeline {
        width: 100% !important;
    }

    .k-picker-wrap .k-icon {
        cursor: pointer;
        margin-left: 2px !important;
        margin-top: 0px !important;
    }

    .k-state-default > .k-select {
        border-color: transparent !important;
    }

    html .km-pane-wrapper .k-header {
        background-color: transparent !important;
    }

    .k-picker-wrap.k-state-default {
        border-color: #aaa !important;
        border-radius: 0px !important;
        width: 132px !important;
        border: none;
        margin-bottom: 5px !important;
        height: 22px;
    }

        .k-picker-wrap.k-state-default input {
            border: 1px solid #aaa !important;
            height: 16px;
            border-radius: 0px !important;
        }

    .k-autocomplete.k-state-hover, .k-dropdown-wrap.k-state-hover, .k-numeric-wrap.k-state-hover, .k-picker-wrap.k-state-hover, .k-autocomplete.k-state-focused, .k-dropdown-wrap.k-state-focused, .k-multiselect.k-header.k-state-focused, .k-numeric-wrap.k-state-focused, .k-picker-wrap.k-state-focused {
        background-color: #fff !important;
        background-image: none !important;
        background-position: 50% 50%;
        border-color: #fff;
        box-shadow: none !important;
    }

    .k-state-selected {
        border-radius: 0px !important;
    }

    .k-popup .k-list .k-item {
        width: 65px !important;
        float: left !important;
        text-align: center !important;
        margin-top: -1px !important;
    }

    input[name="desc_TimePicker"] {
        width: 370px;
        height: 65px;
        border: 1px solid #aaa;
    }

    .k-list-container.k-list-scroller.k-popup.k-group.k-reset {
        width: 230px !important;
        background-color: #fff !important;
        height: auto !important;
    }

    ul li.k-item {
        border-right: 1px solid #999 !important;
    }

        ul li.k-item:nth-child(3n) {
            border-right: 0px solid #999 !important;
        }

    .k-icon.k-i-clock {
        background-image: url('/images/watch.png');
        background-repeat: no-repeat;
        background-position: center;
        text-indent: 100%;
        white-space: nowrap;
        overflow: hidden;
        width: 22px;
        height: 22px;
        display: block;
    }

        .k-icon.k-i-clock:hover {
            background-image: url('/images/hover-watch.png');
        }

    .k-i-clock::before {
        content: "\e107";
        display: none;
    }

    .k-icon.k-i-calendar {
        background-image: url('/images/calender.jpg');
        background-repeat: no-repeat;
        background-position: center;
        text-indent: 100%;
        white-space: nowrap;
        overflow: hidden;
        width: 22px;
        height: 22px;
        display: block;
    }

        .k-icon.k-i-calendar:hover {
            background-image: url('/images/hover-calender.jpg');
        }

    .k-i-calendar::before {
        content: "\e107";
        display: none;
    }
</style>

<script type="text/javascript">

   

    

    function afterSelectTabError(msg) {
        alert(msg);
    }

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

    window.onload = function () {
       
        document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = "";
    }
    window.onbeforeunload = function () {
        if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value == '1') {
            return 'You have unsaved changes on this page.';
        }
    }
    //function ChangeUpdate(sender, args) {
    function ChangeUpdate()
    {
        document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = '1';
       // alert(document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_119').value);
        // For selection change of Number of Fire Extinguishers Used value update on Type of Fire.
        if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_119') != null)
        {
            if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_119').value <  3)
            {
                document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_120').value = "Small Fire ";
            }
            else if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_119').value > 2)
            {
                document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_120').value = "Fire ";
            }
            else
            {
                document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_120').value = " ";
            }
        }
        return true;
    }
    function ChangeClear(sender, args)
    {
        document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = '0';
    }
    function CheckChange() {

        var ret = true;
        if (document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value == '1') {
            ret = confirm('You have unsaved changes on this page. \n\n Are you sure you want to leave this page ?');
            if (ret == true) {
                document.getElementById('ctl00_ContentPlaceHolder_Body_uclIncidentForm_hfChangeUpdate').value = '0';
            }
        }
        return ret;
    }
    //To create timeline grid for saved data.
    function onloadPage(values) {
        try {
            var TimeLine_Date1, TimeLine_Time1, TimeLine_Text1 = [];
            var value1 = values.split('|');
            TimeLine_Date1 = value1[2].split(',');
            TimeLine_Time1 = value1[0].split(',');
            TimeLine_Text1 = value1[1].split(',');
            for (var i = 0; i < TimeLine_Date1.length - 1; i++) {
                var row = "<tr>" +
                    "<td><input name = 'DatePiker' value='" + TimeLine_Date1[i] + "' class='dp_" + i + "' id='dp_" + i + "' ></input></td>" +
                    "<td><input name = 'TimePiker' value='" + TimeLine_Time1[i] + "' class='tp_" + i + "' id='tp_" + i + "' ></input></td>" +
                    "<td><input name = 'desc_TimePicker' value='" + TimeLine_Text1[i] + "' type='textarea' row='2' col='20' CssClass = 'WarnIfChanged' Width = 280 id='txt_" + i + "'></input></td>" +
                    "<td><input type='button' class='BtnMinus' value='(-)' id='btn_" + i + "'/></td></tr>";
                $(row).appendTo($("#tbl_timeline"))

                $(".tp_" + i).kendoTimePicker({
                    interval: 60
                });
                $(".dp_" + i).kendoDatePicker({
                    animation: false
                });
            }
        } catch (ex) {
            console.log(ex.message);
        }

    }

    function onloadPage1() {
        try {
            for (var i = 0; i < TimeLine_Time.length; i++) {

                console.log(TimeLine_Date[i], TimeLine_Time[i], TimeLine_Text[i]);

                var row = "<tr>" +
                    "<td><input name = 'DatePiker' value='" + TimeLine_Date[i] + "' class='dp_" + i + "' id='dp_" + i + "' ></input></td>" +
                    "<td><input name = 'TimePiker' value='" + TimeLine_Time[i] + "' class='tp_" + i + "' id='tp_" + i + "' ></input></td>" +
                    "<td><input name = 'desc_TimePicker' value='" + TimeLine_Text[i] + "' type='textarea' row='2' col='20' CssClass = 'WarnIfChanged' Width = 280 id='txt_" + i + "'></input></td>" +
                    "<td><input type='button' class='BtnMinus' value='(-)' id='btn_" + i + "'/></td></tr>";
                $(row).appendTo($("#tbl_timeline"))

                $(".tp_" + i).kendoTimePicker({
                    interval: 60
                });
                $(".dp_" + i).kendoDatePicker({
                    animation: false
                });
            }

        } catch (ex) {
            console.log(ex.message);
        }

    }


    $(document).ready(function () {
        var rowCount = 0;
        onloadPage1();

        $("#ctl00_ContentPlaceHolder_Body_uclIncidentForm_addrows").click(function () {
            var rowCount = $('#tbl_timeline tr').length;
            console.log(rowCount);
            var newId = ++rowCount;
            var row =
                "<tr>" +
                "<td><input name = 'DatePiker' value=''  id='dp_" + newId + "' class='dp_" + newId + "'></input></td>" +
                "<td><input name = 'TimePiker' value=''  id='tp_" + newId + "' class='tp_" + newId + "'></input></td>" +
                "<td><input name = 'desc_TimePicker' value=''   id='txt_" + newId + "' class='desc_html' type='textarea' row='6' col='20'></input></td>" +
                "<td><input type='button' class='BtnMinus' value='(-)' id='btn_" + newId + "'/></td>" +
                "</tr>";
            console.log(newId);
            $(row).appendTo($("#tbl_timeline"));

            $(".tp_" + newId).kendoTimePicker({
                interval: 60
            });
            $(".dp_" + newId).kendoDatePicker({
                animation: false
            });
        });

        $("#tbl_timeline").on("click", ".BtnMinus", deleteRow);

    });

    function deleteRow() {
        var par = $(this).parent().parent();
        par.remove();
    };

</script>

<div id="divIncidentForm" runat="server">
    <%--<asp:ScriptManager ID="ScriptManager1" AsyncPostBackTimeOut="36000" runat="server" />--%>
    <asp:HiddenField ID="hfChangeUpdate" runat="server" Value="" />
    <asp:HiddenField ID="HiddenField1" runat="server" Value="" />

    <table style="width: 100%" class="textStd">
        <tr>
            <td>
                <div id="divPageBody" class="textStd" style="text-align: left; margin: 0 0;" runat="server">
                    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" HorizontalAlign="NotSet" meta:resourcekey="RadAjaxPanel1Resource1">
                        <asp:Label ID="lblResults" runat="server" meta:resourcekey="lblResultsResource1" />
                        <asp:Panel ID="pnlAddEdit" runat="server" meta:resourcekey="pnlAddEditResource1">
                            <div class="container-fluid blueCell" style="padding: 7px; margin-top: 5px;">
                                <asp:Panel ID="pnlIncidentHeader" runat="server" meta:resourcekey="pnlIncidentHeaderResource1">
                                    <div class="row-fluid">
                                        <div class="col-xs-12  text-left">
                                            <span>
                                                <asp:Label ID="lblAddOrEditIncident" class="prompt" runat="server" Font-Bold="true" Text="<%$ Resources:LocalizedText, AddANewIncident %>" />
                                                <a href="/EHS/EHS_Incidents.aspx" id="ahReturn" runat="server" style="font-size: medium; margin-left: 40px;">
                                                    <img src="/images/defaulticon/16x16/arrow-7-up.png" style="vertical-align: middle; border: 0;" border="0" alt="" />
                                                    Return to List</a>
                                            </span>
                                            <span class="hidden-xs" style="float: right; width: 160px; margin-right: 6px;">
                                                <span class="requiredStar">&bull;</span> - Required to Create</span>
                                            <div style="clear: both;"></div>
                                            <span class="hidden-xs" style="float: right; width: 160px; margin-right: 6px;">
                                                <span class="requiredCloseStar">&bull;</span> - Required to Close</span>
                                        </div>
                                    </div>
                                    <br class="clearfix" style="clear: both;" />
                                    <div class="row-fluid" style="margin-top: -80px;">
                                        <div class="col-xs-12 text-left">
                                            <asp:Label runat="server" ID="lblIncidentLocation" class="textStd" meta:resourcekey="lblIncidentLocationResource1"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblIncidentType" class="textStd" runat="server" meta:resourcekey="lblIncidentTypeResource1">Type:  </asp:Label>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                            <div class="container" style="margin-top: 5px;">
                                <div class="row text_center">
                                    <div class="col-xs-12 col-sm-12 text-center">
                                        <asp:Label ID="lblPageTitle" runat="server" Font-Bold="True" CssClass="pageTitles" meta:resourcekey="lblPageTitleResource1"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div id="divForm" runat="server" visible="False">

                                <telerik:RadAjaxPanel ID="ajaxPanel" runat="server" meta:resourcekey="pnlFormResource1">
                                    <%--  <asp:Panel ID="pnlForm" runat="server" meta:resourcekey="pnlFormResource1">
                                    </asp:Panel>--%>
                                </telerik:RadAjaxPanel>


                                <div id="divSubnav" runat="server">
                                    <div id="divSubnavPage" runat="server" visible="False">
                                        <Ucl:Containment ID="uclcontain" runat="server" Visible="False" />
                                        <Ucl:RootCause ID="uclroot5y" runat="server" Visible="False" />
                                        <Ucl:Causation ID="uclCausation" runat="server" Visible="False" />
                                        <Ucl:Action ID="uclaction" runat="server" Visible="False" />
                                        <Ucl:Approval ID="uclapproval" runat="server" Visible="False" />
                                        <Ucl:Alert ID="uclAlert" runat="server" Visible="false" />
                                        <Ucl:AttachVideoPanel ID="uclVideoPanel" runat="server" Visible="false" />
                                    </div>

                                    <div>
                                        <center>
											<telerik:RadButton ID="btnSubnavSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro"
												OnClientClicked="ChangeClear" OnClick="btnSubnavSave_Click" CommandArgument="0" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
											<telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="False" ForeColor="DarkRed" Style="margin-left: 30px; margin-top: 5px;"
												Text="<%$ Resources:LocalizedText, DeleteIncident %>" SingleClick="True" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
												OnClientClicking="function(sender,args){RadConfirmAction(sender, args, 'Delete this Incident');}" OnClick="btnDelete_Click" CssClass="UseSubmitAction" />
										</center>
                                    </div>
                                    <div style="margin-top: 10px;">
                                        <center>
											<asp:LinkButton ID="btnSubnavIncident" runat="server" Text="<%$ Resources:LocalizedText, Incident %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="1" />

									<asp:LinkButton ID="btnSubnavVideo" runat="server"  Text="<%$ Resources:LocalizedText, VideoUpload %>"  CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClientClick="return CheckChange();" OnClick="btnSubnav_Click" CommandArgument="1.1" visible="false"/>

									<asp:LinkButton ID="btnSubnavContainment" runat="server" Text="<%$ Resources:LocalizedText, InitialAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="2"/>

									<asp:LinkButton ID="btnSubnavInitialActionApproval" runat="server" Text="<%$ Resources:LocalizedText, InitialActionApproval %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="2.5"/>

									<asp:LinkButton ID="btnSubnavRootCause" runat="server" Text="<%$ Resources:LocalizedText, RootCause %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="3"/>

									<asp:LinkButton ID="btnSubnavCausation" runat="server" Text="<%$ Resources:LocalizedText, Causation %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="4"/>

									<asp:LinkButton ID="btnSubnavAction" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveAction %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="5"/>

									<asp:LinkButton ID="btnSubnavCorrectiveActionApproval" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveActionApproval %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="5.5"/>

									<asp:LinkButton ID="btnSubnavApproval" runat="server" Text="<%$ Resources:LocalizedText, Approvals %>" CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
												OnClick="btnSubnav_Click" CommandArgument="10"/>

                                  <%--  To enable Preventative Measures at browser back button visible false is deleted.--%>
									<%--<asp:LinkButton ID="btnSubnavAlert" runat="server" Text="<%$ Resources:LocalizedText, PreventativeMeasure %>"  CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="11" visible="false"/>--%>
                                            <asp:LinkButton ID="btnSubnavAlert" runat="server" Text="<%$ Resources:LocalizedText, PreventativeMeasure %>"  CssClass="buttonLink" style="font-weight:bold; margin-right: 8px;"
										OnClick="btnSubnav_Click" CommandArgument="11"/>
										</center>
                                    </div>
                                </div>
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

<script type="text/javascript">

    //This script is to maintain control state after post back of the page so that links controls will work as it is after postback as well.
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(function (s, e) {
        var rowCount = 0;
        
        $("#ctl00_ContentPlaceHolder_Body_uclIncidentForm_addrows").click(function () {
            var rowCount = $('#tbl_timeline tr').length;
            var newId = ++rowCount;
            //  var row = "<tr><td><input type='time' id='txt_'" + newId + " value='name' text = 'name' class='timepicker_html'></input></td><td><input type='textarea' row='6' col='20' id='txt_'" + newId + " value='name' class='desc_html'></input></td><td><input type='button' class='BtnMinus' value='(-)' id='btn_'" + newId + "/></td></tr>";
            var row =
                "<tr>" +
                "<td><input name = 'DatePiker' value=''  id='dp_" + newId + "' class='dp_" + newId + "'></input></td>" +
                "<td><input name = 'TimePiker' value=''  id='tp_" + newId + "' class='tp_" + newId + "'></input></td>" +
                "<td><input name = 'desc_TimePicker' value=''   id='txt_" + newId + "' class='desc_html' type='textarea' row='6' col='20'></input></td>" +
                "<td><input type='button' class='BtnMinus' value='(-)' id='btn_" + newId + "'/></td>" +
                "</tr>";
            console.log(newId);
            $(row).appendTo($("#tbl_timeline"));

            $(".tp_" + newId).kendoTimePicker({
                interval: 60
            });
            $(".dp_" + newId).kendoDatePicker({
                animation: false
            });
        });

        $("#tbl_timeline").on("click", ".BtnMinus", deleteRow);
    });



</script>
