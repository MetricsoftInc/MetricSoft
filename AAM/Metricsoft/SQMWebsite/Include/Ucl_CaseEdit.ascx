<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_CaseEdit.ascx.cs" Inherits="SQM.Website.Ucl_CaseEdit" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_Status.ascx" TagName="Status" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadAsyncUpload.ascx" TagName="RadAsyncUpload" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_EHSIncidentDetails.ascx" TagName="IncidentDetails" TagPrefix="Ucl" %>
<%@ Reference Control="~/Include/Ucl_QualityIssue.ascx" %>
<%--<%@ Register src="~/Include/Ucl_RadScriptBlock.ascx" TagName="RadScript" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_QISearch.ascx" TagName="QISearch" TagPrefix="Ucl" %>--%>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">

    // Prevent form submit on enter
    $(document).keypress(function (e) {
        if (e.which === 13 && e.target.nodeName !== "TEXTAREA") {
            return false;
        }
    });


    function ValidSave1(changeDesc) {
        return confirmChange(changeDesc);
    }

    function CalculateRiskIndex(risk) {
        try {
            var sfx;
            var severity, occur, detect, riskIndex;

            if (risk == "1" || risk == "0") {
                sfx = "1";
                severity = document.getElementById('ddlCase5riskSeverity' + sfx).options[document.getElementById('ddlCase5riskSeverity' + sfx).selectedIndex].value;
                occur = document.getElementById('ddlCase5riskOccur' + sfx).options[document.getElementById('ddlCase5riskOccur' + sfx).selectedIndex].value;
                detect = document.getElementById('ddlCase5riskDetect' + sfx).options[document.getElementById('ddlCase5riskDetect' + sfx).selectedIndex].value;
                riskIndex = (parseInt(severity) * parseInt(occur) * parseInt(detect));
                document.getElementById('lblCase5riskIndex' + sfx).innerHTML = riskIndex.toString();
                document.getElementById('hfCase5riskIndex' + sfx).value = riskIndex.toString();
            }

            if (risk == "2" || risk == "0") {
                sfx = "2";
                severity = document.getElementById('ddlCase5riskSeverity' + sfx).options[document.getElementById('ddlCase5riskSeverity' + sfx).selectedIndex].value;
                occur = document.getElementById('ddlCase5riskOccur' + sfx).options[document.getElementById('ddlCase5riskOccur' + sfx).selectedIndex].value;
                detect = document.getElementById('ddlCase5riskDetect' + sfx).options[document.getElementById('ddlCase5riskDetect' + sfx).selectedIndex].value;
                riskIndex = (parseInt(severity) * parseInt(occur) * parseInt(detect));
                document.getElementById('lblCase5riskIndex' + sfx).innerHTML = riskIndex.toString();
                document.getElementById('hfCase5riskIndex' + sfx).value = riskIndex.toString();
            }
        }
        catch (ex) {
        }
    }

</script>
    <asp:Panel ID="pnlCase0" runat="server" Visible = "false">
        <table width="99%" class="editArea">
            <tr>
                <td>
                    <asp:Label ID="lblProbCase0Instruction" runat="server" Text="General description and information about the problem to be analyzed. A Problem Case may reference one or more Quality issue which identify the specific non-conformances, materials, impact, etc... referenced throughout the 8D resolution process." CssClass="instructText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="editArea">
                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                        <tr>
                            <td class="columnHeader"  width="14%">
                                <asp:Label ID="lblCaseType" runat="server" text="Case Type"></asp:Label>
                            </td>
                            <td class="tableDataAlt"  width="1%">&nbsp;</td>
                            <td class="tableDataAlt" width="85%">
                                <telerik:RadComboBox ID="ddlCaseType" runat="server" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" Width="300" Enabled="false"></telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblCaseDesc" runat="server" text="Brief Description"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:TextBox ID="tbCaseDesc" runat="server" CssClass="textStd WarnIfChanged" MaxLength = 180 columns="72"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblCaseSummary" runat="server" text="Problem Description"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td CLASS="tableDataAlt" >
                                 <asp:TextBox ID="tbCaseDescLong" TextMode="multiline" rows="2" maxlength="1000" runat="server" CssClass="commentArea WarnIfChanged"/>
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="phCase0InActive" runat="server">
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblCaseInActive" runat="server" Text="<%$ Resources:LocalizedText, Inactive %>" ></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td class="tableDataAlt">
                                <asp:CheckBox id="cbCaseInActive" runat="server" ToolTip="Inactivate this case (will remove from case list but not delete from the database)"/>
                            </td>
                        </tr>
                        </asp:PlaceHolder>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblCaseIncidents" runat="server" text="Issues Included In This Case" ></asp:Label>
                                <br />
                                <span id="spAddIncident" runat="server" visible="true">
                                    <input type="button" class="buttonAdd" style="margin-top: 4px;" id="btnAddIncident" onclick="PopupCenter('../Quality/QualityIssueList.aspx?', 'newPage', 800, 650);" value="Add" />
                                    <%--<asp:Button ID="btnAddIncident" runat="server" Text="Add" ToolTip="Add a containment action" CSSClass="buttonAdd" style="margin: 7px;" onclick="btnAdd_Click" CommandArgument="inciddent" UseSubmitBehavior="true"></asp:Button>--%>
                                </span>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataSmall">
                                <%--<div ID="divQISearch" runat="server" Visible="false">
                                    <Ucl:RadScript id="uclRadScript" runat="server"/>
                                    <Ucl:QISearch id="uclQISearch" runat="server" />
                                </div>--%>
                                <%--<asp:UpdatePanel ID="udpAddIncident" runat="server" UpdateMode=Conditional>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnAddIncident" />
                                    </Triggers>
                                    <ContentTemplate>--%>
                                        <Ucl:IssueList id="uclIssueList" runat="server"/>
                                    <%--</ContentTemplate>
                                </asp:UpdatePanel>--%>
                            </td>
                        </tr>
                        <tr id="trIncidentDetail1" runat="server" visible="false">
                            <td class="columnHeader">
                                <asp:Label ID="lblIncidentDetail" runat="server" Text="Incident Details"></asp:Label>
                            </td>
                            <td class="tableDataAlt"></td>
                            <td class="tableDataAlt">
                                <Ucl:IncidentDetails ID="uclIncidentDetails0" runat="server" />
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="phCase0Form" runat="server">
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblCasePartsList" runat="server" Text="Material Identification"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt">
                                <asp:GridView runat="server" ID="gvCasePartsList" Name="gvCasePartsList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="99%" >
                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                    <RowStyle CssClass="DataCell" />
                	                    <Columns>
                                            <asp:BoundField DataField="PART_NUM" HeaderText="<%$ Resources:LocalizedText, PartNumber %>" ItemStyle-Width="30%" />
                                            <asp:BoundField DataField="LOT_NUM" HeaderText="Lot Number" ItemStyle-Width="25%" />
                                            <asp:BoundField DataField="CONTAINER_NUM" HeaderText="Container" ItemStyle-Width="25%" />
                                            <asp:BoundField DataField="NC_QTY" HeaderText="NC Qty" ItemStyle-Width="20%" />
                                        </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblCase0Program" runat="server" text="Program(s) Affected"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td class="tableDataAlt"><asp:TextBox ID="tbCase0Program" runat="server" MaxLength = 80 CssClass="fullText WarnIfChanged"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                    <asp:Label ID="lblCase0System" runat="server" text="System(s) Affected"></asp:Label>
                                </td>
                                <td class="tableDataAlt">&nbsp;</td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCase0System" runat="server" MaxLength = 80 CssClass="fullText WarnIfChanged"></asp:TextBox>
                                </td>
                        </tr>
                        </asp:PlaceHolder>
                    </table>
                    <br />
                    <Ucl:Status id="uclStatus0" runat="server"/>
                </td>
            </tr>
            <tr id="trCase0OptionArea" runat="server">
                <td align="right" class="optionArea">
                    <asp:UpdatePanel ID="udpCase0Save" runat="server" ChildrenAsTriggers=false UpdateMode=Conditional>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnCase0Save" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:Button ID="btnCase0Cancel" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                                onclick="btnCancel_Click" CommandArgument="0"></asp:Button>
                            <asp:Button ID="btnCase0Save" CSSclass="buttonEmphasis" runat="server" text="Save Case" Enabled="true" title="save this step of the problem case"
                                OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="0"></asp:Button>
                            <asp:Button id="btnCase0Delete" CssClass="buttonEmphasis" runat="server" style="float: right; margin-right: 10px;" Text="Delete Case" Visible="false" Enabled="false"
                              title="Delete this problem case"  ToolTip="Delete this problem case" OnClientClick="return confirmAction('Delete This Case (NOTE: this action will permanently remove the Problem Case and cannot be un-done)');" onclick="btnDelete_Click" ></asp:Button>
                         </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>

   <asp:Panel ID="pnlCase1" runat="server" Visible = "false">
        <asp:HiddenField id="hfHlpCase1Complete" runat="server" Value="This step may be marked as completed when all problem case steps have been assigned a responsible user and a due date."/>
         <table width="99%" class="editArea" id="Table1" runat="server">
            <tr>
                <td>
                    <asp:Label ID="lblCase1TeamInstruction" runat="server" Text="Select the Team and assign tasks according to each team member's job function or responsibility in resolving the problem case." CssClass="instructText"></asp:Label>
                </td>
            </tr>
        </table>
        <table width="99%" class="editArea">
            <tr>
                <td class="editArea">
                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                        <tr>
                            <td class="columnHeader" width="14%">
                                <asp:Label ID="lblCase1Status" runat="server" text="Task Assignments"></asp:Label>
                            </td>
                            <td class="required" width="1%">&nbsp;</td>
                            <td class="tableDataAlt" width="85%">
                                <asp:GridView runat="server" ID="gvTeamList" Name="gvTaskList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="0" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvTeamList_OnRowDataBound">
                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                    <RowStyle CssClass="DataCell" />
                	                <Columns>
                                        <asp:TemplateField HeaderText="Step" ItemStyle-Width="33">
						                    <ItemTemplate>
                                                <asp:HiddenField ID="hfTaskStep" runat="server" value='<%#Eval("TASK_STEP") %>'></asp:HiddenField>
                                                <asp:Label ID="lblTaskDesc" runat="server" text='<%#Eval("DESCRIPTION") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, DueDate %>" ItemStyle-Width="17%">
						                    <ItemTemplate>
                                                <asp:HiddenField ID="hfDueDate" runat="server" value='<%#Eval("DUE_DT") %>'/>
                                                 <telerik:RadDatePicker ID="radDueDate" runat="server" CssClass="textStd WarnIfChanged" Width=115 Skin="Metro" ToolTip="click to select task due date"></telerik:RadDatePicker>
                                            </ItemTemplate>
				                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Responsible" ItemStyle-Width="25%">
						                    <ItemTemplate>
                                                <asp:HiddenField ID="hfResponsible" runat="server" value='<%#Eval("RESPONSIBLE_ID") %>'/>
                                                <telerik:RadComboBox ID="ddlResponsible" runat="server" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged"></telerik:RadComboBox>
                                            </ItemTemplate>
					                    </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Status %>" ItemStyle-Width="25%">
						                    <ItemTemplate>
                                                <asp:HiddenField ID="hfCompleteDate" runat="server" value='<%#Eval("COMPLETE_DT") %>'/>
                                                <asp:HiddenField ID="hfCompleteBy" runat="server" value='<%#Eval("COMPLETE_ID") %>'/>
                                                <asp:HiddenField ID="hfTaskStatus" runat="server"  value='<%#Eval("STATUS") %>'/>
                                                <asp:Label ID="lblTaskStatus" runat="server" CssClass="refText"></asp:Label>
                                                 &nbsp;&nbsp;
							                    <asp:Label ID="lblCompleteDate" runat="server" ></asp:Label>
                                                <br />
                                                <asp:Label ID="lblCompleteBy" runat="server" ></asp:Label>
                                            </ItemTemplate>
					                    </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr id="trCase1Notify" runat="server">
                            <td class="columnHeader">
                                <asp:Label ID="lblCase1Notify" runat="server" text="Send Email Notification To The Team"></asp:Label>
                                <img src="/images/defaulticon/16x16/mail-sent.png" alt="" style="vertical-align: middle; margin-left: 4px;" />
                            </td>
                             <td class="tableDataAlt">&nbsp;</td>
                            <td class="tableDataAlt">
                                <span>
                                    <asp:CheckBox id="cbCase1Notify" runat="server" Checked="false" ToolTip="Send email/assignment notifications to team members. Note: notifications will only be sent for new task assignments."/>
                                    <asp:Label id="lblCase1NotifyConfirm" runat="server" CssClass="refTextSmall" Visible="false" Text="Email notifications have been sent"></asp:Label>
                                </span>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <Ucl:Status id="uclStatus1" runat="server"/>
                </td>
            </tr>
            <tr id="trCase1OptionArea" runat="server">
                <td align="right" class="optionArea">
                <asp:UpdatePanel ID="udpCase1Save" runat="server" ChildrenAsTriggers=false UpdateMode=Conditional>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnCase1Save" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Button ID="btnCase1Cancel" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                            onclick="btnCancel_Click" CommandArgument="1"></asp:Button>
                        <asp:Button ID="btnCase1Save" CSSclass="buttonEmphasis" runat="server" text="Save Team Assignments" Enabled="true" title="save this step of the problem case"
                            OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="1"></asp:Button>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </td>
            </tr>
        </table>
   </asp:Panel>

    <asp:Panel ID="pnlCase2" runat="server" Visible = "false">
        <asp:PlaceHolder ID="phCase2EHS" runat="server">
           <table width="99%" class="editArea" id="Table2" runat="server">
            <tr>
                <td>
                    <asp:Label id="lblCase2InstructionsEHS" runat="server" CssClass="instructText" Text="Details about the specific problem addressed by this case are provided as reference."></asp:Label>
                </td>
            </tr>
			<tr>
                <td>
                    <Ucl:IncidentDetails ID="uclIncidentDetails" runat="server" />
				</td>
            </tr>
		</table>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phCase2" runat="server">
            <table width="99%" class="editArea" id="tblDefineCase2" runat="server">
                <tr>
                    <td>
                        <asp:Label id="lblProbCase2Instruction" runat="server" CssClass="instructText" Text="Specific questions and information used to define the problem to be analyzed. Details about the specific problem incidents addressed by this case are provided as reference for each question category."></asp:Label>
                        <input type='button' id='btnCase2DisplayIncident' value="Display Incident Details" class="buttonLink" onclick="displayAllCells('IDX','tableDataAlt');"/>
                        &nbsp;
                        <input type='button' id='btnCase2HideIncident' value="Hide Incident Details" class="buttonLink" onclick="hideAllCells('IDX');"/>
                    </td>
                </tr>
                <tr>
                    <td class="editArea">
                        <table width="100%" align="center"  border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                            <tr class="HeadingCellTextLeft">
                                <td width="14%">
                                    <asp:Label ID="lblCaseHdr1" runat="server" Text="<%$ Resources:LocalizedText, Question %>" CssClass="tableDataHdr2"></asp:Label>
                                </td>
                                <td width="28%" class="IDX" ><asp:Label runat="server" ID="lblCase2Hdr1" Text="Incident Details" CssClass="tableDataHdr2"></asp:Label></td>
                                <td width="29%"><asp:Label ID="lblCase2Hdr2" runat="server" Text="The Problem IS" CssClass="tableDataHdr2"></asp:Label></td>
                                <td width="29%"><asp:Label ID="lblCase2Hdr3" runat="server" Text="The Problem IS NOT" CssClass="tableDataHdr2"></asp:Label></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWho1" runat="server" text="Who Reported The Problem ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhoDat1" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhoDat1" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhoDat1','tbCaseDefineWhoIs1'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhoIs1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt" ><asp:TextBox ID="tbCaseDefineWhoIsNot1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWho2" runat="server" text="Who Is Impacted ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhoDat2" runat="server" CssClass="refText" ></asp:Label><asp:ImageButton ID="imbWhoDat2" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhoDat2','tbCaseDefineWhoIs2'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhoIs2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhoIsNot2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhat1" runat="server" text="What Is The Process or Product ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhatDat1" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhatDat1" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhatDat1','tbCaseDefineWhatIs1'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhatIs1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhatIsNot1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhat2" runat="server" text="What Is The Specific Defect or Fault ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhatDat2" runat="server" CssClass="refText" ></asp:Label><asp:ImageButton ID="imbWhatDat2" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhatDat2','tbCaseDefineWhatIs2'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhatIs2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhatIsNot2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhere1" runat="server" text="Where Does The Problem Occur ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhereDat1" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhereDat1" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhereDat1','tbCaseDefineWhereIs1'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhereIs1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhereIsNot1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhere2" runat="server" text="Where Was The Problem Detected ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhereDat2" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhereDat2" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhereDat2','tbCaseDefineWhereIs2'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhereIs2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhereIsNot2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhen1" runat="server" text="When Was The Problem Reported ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhenDat1" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhenDat1" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhenDat1','tbCaseDefineWhenIs1'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhenIs1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhenIsNot1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhy1" runat="server" text="Why Is This A Problem ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhyDat1" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhyDat1" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhyDat1','tbCaseDefineWhyIs1'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhyIs1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhyIsNot1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineWhy2" runat="server" text="Why Is This A Resolution Urgent ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineWhyDat2" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbWhyDat2" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineWhyDat2','tbCaseDefineWhyIs2'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhyIs2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineWhyIsNot2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineHow1" runat="server" text="How Often Is The Problem Detected ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineHowDat1" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbHowDat1" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineHowDat1','tbCaseDefineHowIs1'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineHowIs1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineHowIsNot1" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr class="tableDataAlt">
                                <td class="columnHeader">
                                    <asp:Label ID="lblCaseDefineHow2" runat="server" text="How Is The Problem Measured ?"></asp:Label>
                                </td>
                                <td class="tableDataAlt IDX" valign="top"><asp:Label ID="lblCaseDefineHowDat2" runat="server" CssClass="refText"></asp:Label><asp:ImageButton ID="imbHowDat2" Width="16" Height="16" runat="server" CssClass="imgCentered" title="copy incident details" ImageUrl="/images/arrowRight16.png" OnClientClick="copyValueFromTo('lblCaseDefineHowDat2','tbCaseDefineHowIs2'); return false;"/></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineHowIs2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                <td class="tableDataAlt"><asp:TextBox ID="tbCaseDefineHowIsNot2" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="columnHeader"  width="15%">
                                    <asp:Label ID="lblCaseDefineOther" runat="server" text="Additional Information"></asp:Label>
                                </td>
                                <td class="tableDataAlt" colspan="3"><asp:TextBox ID="tbCaseDefineOther" runat="server" TextMode="multiline" rows="3" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                        </table>
                        <br />
                        <table <table width="100%" align="center"  border="0" cellspacing="0" cellpadding="1">
						    <tr>
                                <td class="editArea">
                                    <asp:Label ID="lblProbCase2Instruction2" runat="server" Text="Summarize the problem based on responses to the questions above." CssClass="instructText"></asp:Label>
                                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                                        <tr>
                                            <td class="columnHeader"  width="14%"><asp:Label ID="lblCaseDefineSummary" runat="server" text="Problem Summary"></asp:Label></td>
                                            <td class="required"  width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="85%"><asp:TextBox ID="tbCaseDefineSummary" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                     <br />
                                    <Ucl:Status id="uclStatus2" runat="server"/>
                                </td>
                            </tr>
					    </table>
				    </td>
			    </tr>
                <tr id="trCase2OptionArea" runat="server">
                    <td align="right" class="optionArea">
                    <asp:UpdatePanel ID="udpCase2Save" runat="server" ChildrenAsTriggers=false UpdateMode=Conditional>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnCase2Save" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:Button ID="btnCase2Cancel" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                                onclick="btnCancel_Click" CommandArgument="2"></asp:Button>
                            <asp:Button ID="btnCase2Save" CSSclass="buttonEmphasis" runat="server" text="Save Definition" Enabled="true" title="save this step of the problem case"
                                OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="2"></asp:Button>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </td>
                </tr>
		    </table>
        </asp:PlaceHolder>
    </asp:Panel>

    <asp:Panel ID="pnlCase3" runat="server" Visible = "false">
        <asp:HiddenField id="hfHlpCase3Complete" runat="server" Value="This step may be marked as complete when at least one containment action has been entered and saved."/>
        <table width="99%" class="editArea">
            <tr>
                <td>
                    <asp:Label ID="lblProbCase3Instruction" runat="server" Text="Describe the overall containment strategy and plan. Define action items and assign implementation dates and responsible team members." CssClass="instructText"></asp:Label>
                </td>
            </tr>
            <asp:PlaceHolder ID="phCase3Initial" runat="server">
                <tr>
                    <td class="editArea">
                        <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                            <tr>
                                <td class="columnHeader" width="14%"><asp:Label ID="lblCase3InitialContain" runat="server" text="<%$ Resources:LocalizedText, InitialAction %>"></asp:Label></td>
                                <td class="tableDataAlt" width="1%"></td>
                                <td class="tableDataAlt" width="85%"><asp:TextBox ID="tbCase3InitialContain" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr style="height: 5px;"><td></td></tr>
            <tr>
                <td class="editArea">
                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                        <tr>
                            <td class="columnHeader" width="14%">
                                <asp:Label ID="lblCasePlanItems" runat="server" text="Containment Actions"></asp:Label>
                                <br />
                                <asp:Button ID="btnAddContainAction" runat="server" Text="Add" ToolTip="Add a containment action" CSSClass="buttonAdd" style="margin: 7px;" onclick="btnAdd_Click" CommandArgument="contain" UseSubmitBehavior="true"></asp:Button>
                            </td>
                            <td class="required" width="1%"></td>
                            <td class="tableDataAlt" width="85%">
                                <asp:GridView runat="server" ID="gvContainList" Name="gvContainList" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false" ShowHeader="True"  CellPadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%"  OnRowDataBound="gvContainList_OnRowDataBound">
                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                    <RowStyle CssClass="DataCell" />
                	                <Columns>
                                        <asp:TemplateField HeaderText="Action Description" ItemStyle-VerticalAlign="Top" >
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfContainItem" runat="server" Value='<%#Eval("ITEM_SEQ") %>' />
                                                <asp:TextBox ID="tbContainAction" runat="server" CssClass="commentArea WarnIfChanged" Text='<%#Eval("ACTION_ITEM") %>' TextMode="multiline" rows="3" maxlength="1000"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-VerticalAlign="Top" ItemStyle-Width="170px">
                                            <HeaderTemplate>
                                                Responsible <asp:Image ID="imgContain" ImageUrl="~/images/ico-question.png" Visible="true" style="vertical-align: middle; border: 0px;" runat="server" ToolTip="Accountability for ensuring the initial containment action will be (or have been) taken by the specified date"/>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfContainResponsible" runat="server" Value='<%#Eval("RESPONSIBLE1_PERSON") %>' />
                                                <asp:HiddenField ID="hfContainStatus" runat="server" Value='<%#Eval("STATUS") %>' />
                                                <telerik:RadComboBox ID="ddlContainResponsible" runat="server" Skin="Metro" style="width: 99%; margin-top:3px;" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" ToolTip="Select the person responsible for implementing or overseeing this action" EmptyMessage="Select Person"></telerik:RadComboBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-VerticalAlign="Top" ItemStyle-Width="120px">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfContainDueDate" runat="server" Value='<%#Eval("DUE_DT") %>' />
                                                <telerik:RadDatePicker ID="radContainDueDate" runat="server" CssClass="textStd WarnIfChanged" Width=115 Skin="Metro" style="margin-top: 3px;" ></telerik:RadDatePicker>
                                                <br />
                                                <telerik:RadComboBox ID="ddlContainStatus" runat="server" Skin="Metro" style="width: 99%; margin-top: 3px;" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" Width="115" EmptyMessage="<%$ Resources:LocalizedText, Status %>" ToolTip="Select the status of the action to be performed"></telerik:RadComboBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                       <tr>
                            <td class="columnHeader" style="vertical-align: top;"><asp:Label ID="lblCase3InitialResult" runat="server" text="Results"></asp:Label></td>
                            <td class="tableDataAlt"></td>
                            <td class="tableDataAlt">
                                <span>
                                    <asp:TextBox ID="tbCase3InitialResult" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged" Width="98%"></asp:TextBox>
                                    <br />
                                    Attachments:<br />
                                    <Ucl:RadAsyncUpload id="uclRadAsyncUpload3" runat="server"/>
                                </span>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="height: 5px;"><td></td></tr>
            <tr>
                <td class="editArea">
                    <Ucl:Status id="uclStatus3" runat="server"/>
                </td>
            </tr>
            <tr style="height: 5px;"><td></td></tr>
            <tr id="trCase3OptionArea" runat="server">
                <td align="right" class="optionArea">
                <asp:UpdatePanel ID="udpCase3Save2" runat="server" ChildrenAsTriggers=false UpdateMode=Conditional>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnCase3Save2" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:Button ID="btnCase3Cancel2" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                                onclick="btnCancel_Click" CommandArgument="3"></asp:Button>
                            <asp:Button ID="btnCase3Save2" CSSclass="buttonEmphasis" runat="server" text="Save Containment" title="save this step of the problem case"
                                OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="3"></asp:Button>
                        </ContentTemplate>
                </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>

   <asp:Panel ID="pnlCase4" runat="server" Visible = "false">
    <asp:HiddenField id="hfHlpCase4Complete" runat="server" Value="This step may be marked as complete when the final root cause has been identified and saved."/>
    <table width="99%" class="editArea">
        <tr>
            <td>
                <asp:Label id="lblProbCase4Instruction" runat="server" CssClass="instructText" Text="Define the problem statement from the results obtained from the Definition step.  Start with asking why the problem occurred and record the answer. Then ask why this answer occurred and continue asking why for each response.  Target '5 Why' question and answers.  Indicate the likely Root Cause(s) to be verified as corrective actions are applied."></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="editArea">
                <table width="100%" align="center"  border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase4Problem" runat="server" text="Problem Statement"></asp:Label></td>
                        <td class="required"  width="1%">&nbsp;</td>
                        <td class="tableDataAlt" width="85%"><asp:TextBox ID="tbCase4Problem" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
					<tr>
                        <td class="columnHeader">
                            <asp:Label ID="lblCase4Steps" runat="server" text="Determination Steps"></asp:Label>
                            <br />
                            <asp:Button ID="btnAddCasuseStep" runat="server" Text="Add" ToolTip="Add a probable cause step" CSSClass="buttonAdd" style="margin: 7px;" onclick="btnAdd_Click" CommandArgument="causestep" UseSubmitBehavior="true"></asp:Button>
                        </td>
                        <td class="required">&nbsp;</td>
                        <td class="tableDataAlt">
                            <asp:GridView runat="server" ID="gvCauseStepList" Name="gvCauseStepList" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false" CellPadding="1" GridLines="None" PageSize="20" AllowSorting="true"
                                Width="99%" OnRowDataBound="gvCauseStepList_OnRowDataBound">
                                <HeaderStyle CssClass="HeadingCellTextLeft" />
                                <RowStyle CssClass="DataCell" />
                	            <Columns>
                    	            <asp:BoundField  DataField="PROBCASE_ID" Visible="False"/>
			                        <asp:BoundField  DataField="ITERATION_NO" Visible="False"/>
                                    <asp:TemplateField ItemStyle-Width="50%" ItemStyle-VerticalAlign="Top">
                                         <HeaderTemplate>
                                            Why (Is The Cause Of <asp:Image ID="imgStepType" ImageUrl="~/images/defaulticon/16x16/arrow-1-up.png" Visible="true" style="vertical-align: middle; border: 0px;" runat="server" ToolTip="Why is this the cause of the preceding answer or problem statement"/>)
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:HiddenField  ID="hfCauseStep" runat="server" Value='<%#Eval("ITERATION_NO") %>'/>
                                            <asp:HiddenField  ID="hfCauseType" runat="server" Value='<%#Eval("CAUSE_TYPE") %>'/>
                                            <asp:HiddenField  ID="hfIsRootCause" runat="server" Value='<%#Eval("IS_ROOTCAUSE") %>'/>
                                            <asp:TextBox ID="tbWhyOccur" runat="server" CssClass="commentArea WarnIfChanged" Text='<%#Eval("WHY_OCCUR") %>' TextMode="multiline" rows="4" maxlength="1000" ></asp:TextBox>
                                            <br />
                                            <asp:Image ID="imgRootCause" ImageUrl="~/images/defaulticon/16x16/asterisk.png" Visible="true" style="float:right;  margin-top: 4px; border: 0px;" runat="server" ToolTip="Is a root cause"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField ItemStyle-Width="50%" ItemStyle-VerticalAlign="Top">
                                        <HeaderTemplate>
                                             Answer/<br />Operational Control Of Root Cause <asp:Image ID="imgCause" ImageUrl="~/images/ico-question.png" Visible="true" style="vertical-align: middle; border: 0px;" runat="server" ToolTip="Indicate 'root' causes by selecting an operation control from the list provided"/>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="tbHowConfirmed" runat="server" CssClass="commentArea WarnIfChanged" Text='<%#Eval("HOW_CONFIRMED") %>' TextMode="multiline" rows="4" maxlength="1000" ></asp:TextBox>
                                            <br />
                                            <telerik:RadComboBox ID="ddlCauseType" runat="server" Skin="Metro"  style="width: 98%;" Font-Size=Small CssClass="WarnIfChanged" EmptyMessage="select if root cause" ToolTip="If this is the ultimate root cause, select the operational control/cause type"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader" width="14%"><asp:Label ID="lblCase4Comments" runat="server" Text="<%$ Resources:LocalizedText, Comments %>"></asp:Label></td>
                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                        <td class="tableDataAlt" width="85%"><asp:TextBox ID="tbCase4Comments" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
                </table>
             </td>
		</tr>
        <tr style="height: 5px;"><td></td></tr>
         <tr>
            <td class="editArea">
                <Ucl:Status id="uclStatus4" runat="server"/>
            </td>
        </tr>
        <tr style="height: 5px;"><td></td></tr>
        <tr id="trCase4OptionArea" runat="server">
            <td align="right" class="optionArea">
                <asp:Button ID="btnCase4Cancel2" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                    onclick="btnCancel_Click" CommandArgument="4"></asp:Button>
                <asp:Button ID="btnCase4Save2" CSSclass="buttonEmphasis" runat="server" text="Save Root Cause" Enabled="true" title="save this step of the problem case"
                    OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="4"></asp:Button>
            </td>
        </tr>
    </table>
</asp:Panel>

 <asp:Panel ID="pnlCase5" runat="server" Visible = "false">
     <asp:HiddenField id="hfHlpCase5Complete" runat="server" Value="This step may be marked as complete when at least one corrective action has been defined and saved."/>
    <table width="99%" class="editArea">
        <tr>
            <td>
                <asp:Label id="lblProbCase5Instruction" runat="server" CssClass="instructText" Text="Apply Corrective Actions to each of the likely Root Cause(s) identified"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="editArea">
                <asp:Repeater runat="server" ID="rptCauseAction" ClientIDMode="AutoID" OnItemDataBound="rptCauseAction_OnItemDataBound">
					<HeaderTemplate><table cellspacing="0" cellpadding="2" border="0" width="100%" ></HeaderTemplate>
					<ItemTemplate>
                        <tr>
                            <td>
                                <table width="100%" align="center"  border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                                    <tr>
                                        <td class="columnHeader"  width="14%">
                                            <asp:Label ID="lblRelatedCause" runat="server" class="prompt"  Text="<%$ Resources:LocalizedText, RootCause %>"></asp:Label>
                                        </td>
                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                        <td class="tableDataAlt" width="85%">
                                            <asp:HiddenField ID="hfRelatedCause" runat="server"/>
                                            <asp:TextBox ID="tbRelatedCause" runat="server" CssClass="commentArea" ReadOnly="true" TextMode="multiline" rows="2" maxlength="1000"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="columnHeader">
                                            <asp:Label ID="lblCase5ApplyAction" runat="server" Text="<%$ Resources:LocalizedText, CorrectiveActions %>"></asp:Label>
                                            <br />
                                            <asp:Button ID="btnCase5AddAction" CSSclass="buttonAdd" runat="server" ToolTip="Add a corrective action" text="Add" style="margin: 7px;"  onclick="btnAdd_Click" CommandArgument="correctiveaction" UseSubmitBehavior="true"></asp:Button>
                                        </td>
                                        <td class="required">&nbsp;</td>
                                        <td class="tableDataAlt">
                                            <asp:GridView runat="server" ID="gvActionList" Name="gvActionList" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false" ShowHeader="True"  CellPadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%"  OnRowDataBound="gvCauseActionSubList_OnRowDataBound">
                                                <HeaderStyle CssClass="HeadingCellTextLeft" />
                                                <RowStyle CssClass="DataCell" />
                	                            <Columns>
                                                    <asp:TemplateField HeaderText="Action Description" ItemStyle-VerticalAlign="Top">
                                                        <ItemTemplate>
                                                            <asp:HiddenField id="hfRootCauseNo" runat="server" Value='<%#Eval("CAUSE_NO") %>'/>
                                                            <asp:HiddenField ID="hfCorrectiveActionNo" runat="server" Value='<%#Eval("ACTION_NO") %>' />
                                                            <asp:HiddenField ID="hfVerifyObservations" runat="server" Value='<%#Eval("VERIFY_OBSERVATIONS") %>' />
                                                            <asp:HiddenField ID="hfVerifyStatus" runat="server" Value='<%#Eval("VERIFY_STATUS") %>' />
                                                            <asp:TextBox ID="tbCorrectiveAction" runat="server" CssClass="commentArea WarnIfChanged" Text='<%#Eval("ACTION_DESC") %>' TextMode="multiline" rows="3" maxlength="1000"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField  ItemStyle-VerticalAlign="Top" ItemStyle-Width="170px">
                                                        <HeaderTemplate>
                                                            Impacted Category /<br>Responsible <asp:Image ID="imgAction" ImageUrl="~/images/ico-question.png" Visible="true" style="vertical-align: middle; border: 0px;" runat="server" ToolTip="Accountability for ensuring the corrective action will be implemented by the specified date"/>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hfPPPType" runat="server" Value='<%#Eval("PPP_TYPE") %>' />
                                                            <telerik:RadComboBox ID="ddlPPPType" runat="server" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" style="width: 99%; margin-top: 3px;" EmptyMessage="Select action impact category" ToolTip="Identify the impacted resource category (people/process/product) if applicable"></telerik:RadComboBox>
                                                            <asp:HiddenField ID="hfActionResponsible" runat="server" Value='<%#Eval("RESPONSIBLE1_PERSON") %>' />
                                                            <br />
                                                            <telerik:RadComboBox ID="ddlActionResponsible" runat="server" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" style="width: 99%; margin-top: 3px;" ToolTip="Select the person responsible for implementing or overseeing this action" EmptyMessage="Select Person"></telerik:RadComboBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-VerticalAlign="Top" ItemStyle-Width="120px">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hfActionDueDate" runat="server" Value='<%#Eval("EFF_DT") %>' />
                                                            <asp:HiddenField ID="hfActionStatus" runat="server" Value='<%#Eval("STATUS") %>' />
                                                            <telerik:RadDatePicker ID="radActionDueDate" runat="server" CssClass="textStd WarnIfChanged" Width=115 Skin="Metro" style="margin-top: 3px;"></telerik:RadDatePicker>
                                                            <br />
                                                            <telerik:RadComboBox ID="ddlActionStatus" runat="server" Skin="Metro" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" style="width: 99%; margin-top: 3px;" EmptyMessage="<%$ Resources:LocalizedText, Status %>" ToolTip="Select the status of the action to be performed"></telerik:RadComboBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <tr style="height: 5px;"><td></td></tr>
        <tr>
            <td class="editArea">
                <asp:PlaceHolder ID="phCase5Risk" runat="server">
                <asp:Label ID="lblCase5riskTitle" runat="server" Text="Risk Assessment" CssClass="prompt"></asp:Label>
                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                    <tr class="HeadingCellTextLeft">
                        <td width="14%"><asp:Label ID="lblCase5riskHdr0" runat="server"  Text="Risk Factor" CssClass="tableDataHdr2"></asp:Label>
                        <td width="1%"><asp:Label ID="lblCase5risk00" runat="server"  Text="" CssClass="tableDataHdr2"></asp:Label>
                        <td width="40%"><asp:Label ID="lblCase5riskHdr1" runat="server" Text="Incident (as reported)" CssClass="tableDataHdr2"></asp:Label>
                        <td width="40%"><asp:Label ID="lblCase5riskHdr2" runat="server" Text="Revised (corrective action taken)" CssClass="tableDataHdr2"></asp:Label>
                    </tr>
                    <tr>
                        <td class="columnHeader">
                            <asp:ImageButton ID="btnEHSRisk" runat="server" Text="?" ToolTip="click to view risk definitions for Environment, Health and Safety incidents" ImageUrl="/images/ico-question.png" OnClientClick="window.radopen(null, 'winEHSRisk'); return false" />
                            <asp:ImageButton ID="btnQSRisk" runat="server" Text="?" ToolTip="click to view risk definitions for Quality Control incidents" ImageUrl="/images/ico-question.png" OnClientClick="window.radopen(null, 'winQSRisk'); return false" />
                            <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
                                <Windows>
                                    <telerik:RadWindow ID="winEHSRisk" runat="server"  Modal="true" Skin="Metro" AutoSize="true">
                                    <ContentTemplate>
                                        <iframe id="frameEHSRisk" runat="server" width="650px" height="220px" src="../images/help/EHS_RiskTable.jpg">
                                        </iframe>
                                        </ContentTemplate>
                                    </telerik:RadWindow>
                                <telerik:RadWindow ID="winQSRisk" runat="server"  Modal="true" Skin="Metro" AutoSize="true">
                                    <ContentTemplate>
                                        <iframe id="framQSRisk" runat="server" width="680px" height="100px" src="../images/help/Quality_RiskTable.jpg">
                                        </iframe>
                                        </ContentTemplate>
                                    </telerik:RadWindow>
                                </Windows>
                            </telerik:RadWindowManager>
                        </td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:TextBox ID="tbCase5riskState1" runat="server" TextMode="multiline" rows="2" maxlength="1000" CssClass="commentArea WarnIfChanged" ></asp:TextBox></td>
                        <td class="tableDataAlt"><asp:TextBox ID="tbCase5riskState2" runat="server" TextMode="multiline" rows="2" maxlength="1000" CssClass="commentArea WarnIfChanged" ></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase5riskSeverity" runat="server" text="Severity"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:DropDownList ID="ddlCase5riskSeverity1" runat="server" CssClass="WarnIfChanged" onChange="CalculateRiskIndex('1');" AutoPostBack="false"></asp:DropDownList></td>
                        <td class="tableDataAlt"><asp:DropDownList ID="ddlCase5riskSeverity2" runat="server" CssClass="WarnIfChanged" onChange="CalculateRiskIndex('2');" AutoPostBack="false"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase5riskOccur" runat="server" text="Occurence"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:DropDownList ID="ddlCase5riskOccur1" runat="server" CssClass="WarnIfChanged" onChange="CalculateRiskIndex('1');" AutoPostBack="false"></asp:DropDownList></td>
                        <td class="tableDataAlt"><asp:DropDownList ID="ddlCase5riskOccur2" runat="server" CssClass="WarnIfChanged" onChange="CalculateRiskIndex('2');" AutoPostBack="false"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase5riskDetect" runat="server" text="Detection"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:DropDownList ID="ddlCase5riskDetect1" runat="server" CssClass="WarnIfChanged" onChange="CalculateRiskIndex('1');"  AutoPostBack="false"></asp:DropDownList></td>
                        <td class="tableDataAlt"><asp:DropDownList ID="ddlCase5riskDetect2" runat="server" CssClass="WarnIfChanged" onChange="CalculateRiskIndex('2');" AutoPostBack="false"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase5riskIndex" runat="server" text="Risk Index"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:Label ID="lblCase5riskIndex1" runat="server" Columns="5" CssClass="labelEmphasis" ></asp:Label><asp:HiddenField id="hfCase5riskIndex1" runat="server"/></td>
                        <td class="tableDataAlt"><asp:Label ID="lblCase5riskIndex2" runat="server" Columns="5"  CssClass="labelEmphasis"></asp:Label><asp:HiddenField id="hfCase5riskIndex2" runat="server"/></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase3OtherRisk" runat="server" text="Additional Risks"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt" colspan="2"><asp:TextBox ID="tbCase3OtherRisk" runat="server" MaxLength="80" CssClass="fullText WarnIfChanged"></asp:TextBox></td>
                    </tr>
                </table>
            </asp:PlaceHolder>
            </td>
        </tr>
        <tr style="height: 5px;"><td></td></tr>
        <tr>
            <td class="editArea">
                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase5Attachments" runat="server" text="<%$ Resources:LocalizedText, Attachments %>"></asp:Label></td>
                        <td class="tableDataAlt"  width="1%"></td>
                        <td class="tableDataAlt"  width="85%">
                            <Ucl:RadAsyncUpload id="uclRadAsyncUpload5" runat="server"/>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
       <tr style="height: 5px;"><td></td></tr>
        <tr>
            <td>
                <Ucl:Status id="uclStatus5" runat="server"/>
            </td>
        </tr>
        <tr id="trCase5OptionArea" runat="server">
            <td align="right" class="optionArea">
             <asp:UpdatePanel ID="updCase5Save2" runat="server" UpdateMode=Conditional>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCase5Save2" />
                </Triggers>
                <ContentTemplate>
                    <asp:Button ID="btnCase5Cancel2" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                        onclick="btnCancel_Click" CommandArgument="5"></asp:Button>
                    <asp:Button ID="btnCase5Save2" CSSclass="buttonEmphasis" runat="server" text="Save Corrective Action" Enabled="true" title="save this step of the problem case"
                        OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="5"></asp:Button>
                </ContentTemplate>
            </asp:UpdatePanel>
            </td>
        </tr>
    </table>

</asp:Panel>

<asp:Panel ID="pnlCase6" runat="server" Visible = "false">
    <asp:HiddenField id="hfHlpCase6Complete" runat="server" Value="This step may be marked as completed when observations and effectivness ratings for all corrective actions have been entered and saved."/>
    <table width="99%" class="editArea">
        <tr>
            <td>
                <asp:Label ID="lblProbCase6Instruction" runat="server" Text="Document how corrective actions were tested and effectiveness of the results. Examples of corrective action verification may include the following: statistical evidence (data sampling), hypothesis testing, toggling the issue state on/off while observing the results, etc. Attach any supporting worksheets or computations created during the verification process." CssClass="instructText"></asp:Label>
                <asp:Label ID="lblProbCase6InstructionEhs" runat="server" Text="Document how Corrective Actions were tested and effectiveness of the results." CssClass="instructText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="editArea">
                <asp:PlaceHolder ID="phCase6Trial" runat="server">
                    <div style="border: 1px; border-color:gray; border-style:solid; overflow: auto;" >
                        <table width="99%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 4px;">
						    <tr>
							    <td style="width: 20px;">
                                    <button type="button" id="btnCase6TrialList" onclick="ToggleSectionVisible('pnlCase6TrialList',ElementHeight('ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder_Body_uclCaseEdit_uclTrialList_gvVerifyTrialList',12));">
                                        <img id="pnlCase6TrialList_img" src="/images/plus.png"/>
                                    </button>
                                </td>
							    <td>
                                    <span>
                                        <asp:Label runat="server" ID="lblCase6TrialList" CssClass="prompt" Text="Verification Trials"></asp:Label>
                                        <asp:Label ID="lblCase6TrialListInstruction" runat="server" Text="Expand this section to view previous verification trials and results." CssClass="instructText" style="margin-left: 30px;"></asp:Label>
                                    </span>
                                </td>
						    </tr>
					    </table>
                    </div>
                </asp:PlaceHolder>
                <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase6TargetDate" runat="server" text="Target Verification Date"></asp:Label></td>
                        <td class="required"  width="1%"></td>
                        <td class="tableDataAlt"  width="85%">
                            <telerik:RadDatePicker ID="radCase6TargetDate" runat="server" CssClass="textStd WarnIfChanged" Width=115 Skin="Metro"></telerik:RadDatePicker>
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase6VerifyMethod" runat="server" text="Verification Methods Used"></asp:Label></td>
                        <td class="tableDataAlt"></td>
                        <td class="tableDataAlt" ><asp:TextBox ID="tbCase6VerifyMethod" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase6ActionEffective" runat="server" text="Corrective Action Effectiveness"></asp:Label></td>
                        <td class="required" width="1%">&nbsp;</td>
                        <td class="tableDataAlt" width="85%">
                            <div id="div1" runat="server" class="">
                                <asp:GridView runat="server" ID="gvActionEfectiveList" Name="gvActionEffectiveList" CssClass="GridAlt" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="None" PageSize="20" AllowSorting="true" Width="99%" OnRowDataBound="gvActionEffectiveList_OnRowDataBound">
                                    <HeaderStyle CssClass="HeadingCellTextLeft" />
                                    <RowStyle CssClass="DataCell" />
                	                <Columns>
                                        <asp:TemplateField HeaderText="Corrective Action Tested" ItemStyle-Width="40%" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:TextBox ID="tbVerifyAction" runat="server" CssClass="commentArea" ReadOnly="true" TextMode="multiline" rows="3" Text='<%#Eval("Action") %>'></asp:TextBox>
                                                <asp:HiddenField ID="hfRootCauseNo" runat="server" Value='<%#Eval("RelatedCauseNo") %>' />
						                        <asp:HiddenField ID="hfActionNo" runat="server" Value='<%#Eval("ActionNo") %>' />
						                        <asp:HiddenField ID="hfActionCode" runat="server" Value='<%#Eval("ActionCode") %>' />
						                        <asp:HiddenField ID="hfActionEffDate" runat="server" Value='<%#Eval("EffDate") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Observations" ItemStyle-Width="40%">
                                            <ItemTemplate>
                                                <asp:TextBox ID="tbVerifyObservations" runat="server" CssClass="commentArea WarnIfChanged" Text='<%#Eval("VerifyObservations") %>' TextMode="multiline" rows="3" maxlength="1000"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Effectiveness" ItemStyle-Width="20%" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfVerifyStatus" runat="server" Value='<%#Eval("VerifyStatus") %>' />
                                                <telerik:RadComboBox ID="ddlVerify" runat="server" style="width: 99%; margin-top: 3px;"  Skin="Metro" class="WarnIfChanged" ></telerik:RadComboBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                           </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase6VerifyResult" runat="server" text="Results"></asp:Label></td>
                        <td class="tableDataAlt"></td>
                        <td class="tableDataAlt">
                            <asp:TextBox ID="tbCase6VerifyResult" runat="server" TextMode="multiline" rows="4" maxlength="1000" CssClass="commentArea WarnIfChanged" Width="98%"></asp:TextBox>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phCase6Identification" runat="server">
                        <tr>
                            <td class="columnHeader"><asp:Label ID="lblCase6Identification" runat="server" text="Post-Action Equipment, Material or Process Identification Requirements"></asp:Label></td>
                            <td class="tableDataAlt"></td>
                            <td class="tableDataAlt"><asp:TextBox ID="tbCase6Identification" runat="server" TextMode="multiline" rows="2" maxlength="400" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                        </tr>
                    </asp:PlaceHolder>
                </table>
            </td>
        </tr>
        <tr style="height: 5px;"><td></td></tr>
        <tr>
            <asp:PlaceHolder ID="phCase6Attachments" runat="server">
            <td class="editArea">
                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase6Attachments" runat="server" text="<%$ Resources:LocalizedText, Attachments %>"></asp:Label></td>
                        <td class="tableDataAlt" width="1%"></td>
                        <td class="tableDataAlt" width="85%">
                            <Ucl:RadAsyncUpload id="uclRadAsyncUpload6" runat="server"/>
                        </td>
                    </tr>
                </table>
            </td>
            </asp:PlaceHolder>
        </tr>
        <tr style="height: 5px;"><td></td></tr>
        <tr>
            <td>
                <Ucl:Status id="uclStatus6" runat="server"/>
            </td>
        </tr>
        <tr id="trCase6OptionArea" runat="server">
            <td align="right" class="optionArea">
            <asp:UpdatePanel ID="udpCase6Save" runat="server" UpdateMode=Conditional>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCase6Save2" />
                </Triggers>
                <ContentTemplate>
                    <asp:Button ID="btnCase6Cancel2" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                        onclick="btnCancel_Click" CommandArgument="6"></asp:Button>
                    <asp:Button ID="btnCase6Save2" CSSclass="buttonEmphasis" runat="server" text="Save Verification" Enabled="true" title="save this step of the problem case"
                        OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="6"></asp:Button>
                    <asp:Button ID="btnCase6AddTrial2" CSSclass="buttonStd" runat="server" text="Save As Trial" Enabled="true" title="save the current verification results as a trial"
                        OnClientClick="return confirmChange('Verification Trial');" onclick="btnAdd_Click" CommandArgument="trial"></asp:Button>
                </ContentTemplate>
            </asp:UpdatePanel>
            </td>
        </tr>
    </table>
 </asp:Panel>

 <asp:Panel ID="pnlCase7" runat="server" Visible = "false">
     <asp:HiddenField id="hfHlpCase7Complete" runat="server" Value="This step may be marked as complete when all subsequent steps have been completed and all fields marked as required (*) have been entered and saved."/>
    <table width="99%" class="editArea">
        <tr>
            <td>
                <asp:Label ID="lblProbCase7Instruction" runat="server" Text="Specify how this issue will be avoided in the future and any other areas or processes that may be at risk." CssClass="instructText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="editArea">
                <table width="100%" align="center"  border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase7Method" runat="server" text="Prevention Method(s)"></asp:Label></td>
                        <td class="required"  width="1%">&nbsp;</td>
                        <td class="tableDataAlt" width="85%"><asp:TextBox ID="tbCase7Method" runat="server" TextMode="multiline" rows="4" maxlength="2000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
                    <tr id="trImpactedAreas" runat="server">
                        <td class="columnHeader"><asp:Label ID="lblCase7Areas" runat="server" text="Other Areas Possibly Impacted"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt">
                            <asp:TextBox ID="tbCase7ImpactedAreas" runat="server" TextMode="multiline" rows="2" maxlength="200" CssClass="commentArea WarnIfChanged" ></asp:TextBox>
                        </td>
                    </tr>
                     <tr id="trImpactedLocs" runat="server">
                        <td class="columnHeader"><asp:Label ID="lblCase7Locs" runat="server" text="Locations Possibly Impacted"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt">
                            <telerik:RadAjaxPanel runat="server" ID="rjxPlantSelect">
                                <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" ZIndex=9000 Skin="Metro"  Width="600"></telerik:RadComboBox>
                                <br />
                                <asp:TextBox ID="tbCase7ImpactedLocs" runat="server" TextMode="multiline" rows="2" maxlength="200" CssClass="commentArea WarnIfChanged" ></asp:TextBox>
                            </telerik:RadAjaxPanel>
                        </td>
                    </tr>
                    <tr id="trImpactedDocs" runat="server">
                        <td class="columnHeader"><asp:Label ID="lblCase7Documentation" runat="server" text="Update Documentation"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt">
                            <asp:UpdatePanel ID="udpDocumentationList" runat="server" ChildrenAsTriggers=true UpdateMode=Conditional>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnDocumentationAdd" />
                                     <asp:PostBackTrigger ControlID="gvDocumentationList" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:GridView runat="server" ID="gvDocumentationList" Name="gvDocumentationList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false"  CellPadding="1" GridLines="Both" PageSize="20" AllowSorting="false" Width="100%" OnRowDataBound="gvCase7Documentation_OnRowDataBound" >
                                        <HeaderStyle CssClass="HeadingCellTextLeft" />
                                        <RowStyle CssClass="DataCell" />
                	                    <Columns>
                                            <asp:TemplateField HeaderText="Affected Document" ItemStyle-Width="30%">
                                                <ItemTemplate>
                                                    <asp:HiddenField id="hfItemType" runat="server" value='<%#Eval("PREVENT_ITEM_TYPE") %>'/>
                                                    <asp:TextBox ID="tbDocument" runat="server" CssClass="textStd WarnIfChanged" MaxLength = 400 columns="30" Text='<%#Eval("PREVENT_ITEM_NAME") %>' ></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Responsible" ItemStyle-Width="30%">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hfResponsible" runat="server" Value='<%#Eval("RESPONSIBLE_PERSON") %>'/>
                                                    <telerik:RadComboBox ID="ddlResponsible" runat="server" Skin="Metro" style="width: 98%;" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" ToolTip="Select the person responsible for implementing or overseeing this change" EmptyMessage="Select Person"></telerik:RadComboBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, DueDate %>" ItemStyle-Width="16%">
						                        <ItemTemplate>
                                                    <asp:HiddenField ID="hfTargetDate" runat="server" value='<%#Eval("TARGET_DT") %>'/>
                                                     <telerik:RadDatePicker ID="radTargetDate" runat="server" CssClass="textStd WarnIfChanged" Width=115 Skin="Metro" ToolTip="click to select due date"></telerik:RadDatePicker>
                                                </ItemTemplate>
				                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Status %>" ItemStyle-Width="14%">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hfStatus" runat="server" value='<%#Eval("CONFIRM_STATUS") %>'/>
                                                    <telerik:RadComboBox ID="ddlStatus" runat="server" Skin="Metro" style="width: 98%;" ZIndex=9000 Font-Size=Small CssClass="WarnIfChanged" ToolTip="Enter the change status" EmptyMessage="<%$ Resources:LocalizedText, Status %>"></telerik:RadComboBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:Button ID="btnDocumentationAdd" CSSclass="buttonAdd" runat="server" text="Add Document" style="margin: 5px;"
                                         onclick="btnDocumentationAdd_Click" CommandArgument="team" ></asp:Button>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                <br />
                <Ucl:Status id="uclStatus7" runat="server"/>
            </td>
        </tr>
        <tr id="trCase7OptionArea" runat="server">
            <td align="right" class="optionArea">
            <asp:UpdatePanel ID="udpCase7Save" runat="server" UpdateMode=Conditional>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnCase7Save" />
                </Triggers>
                <ContentTemplate>
                    <asp:Button ID="btnCase7Cancel" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                        onclick="btnCancel_Click" CommandArgument="7"></asp:Button>
                    <asp:Button ID="btnCase7Save" CSSclass="buttonEmphasis" runat="server" text="Save Prevention" Enabled="true" title="save this step of the problem case"
                        OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="7"></asp:Button>
                </ContentTemplate>
            </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Panel>


 <asp:Panel ID="pnlCase8" runat="server" Visible = "false">
    <asp:HiddenField id="hfHlpCase8Complete" runat="server" Value="This step may be marked as complete when all subsequent steps have been completed and all fields marked as required (*) have been entered and saved. Note: this is the final step in the problem solving process."/>
    <table width="99%" class="editArea">
        <tr>
            <td>
                <asp:Label id="lblProbCase8Instruction" runat="server" CssClass="instructText" Text="Close the problem case and notify the team."></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="editArea">
                <table width="100%" align="center"  border="0" cellspacing="0" cellpadding="1" class="borderSoft">
                    <tr>
                        <td class="columnHeader"  width="14%"><asp:Label ID="lblCase8Conclusion" runat="server" text="Conclusions"></asp:Label></td>
                        <td class="required"  width="1%">&nbsp;</td>
                        <td class="tableDataAlt" width="85%"><asp:TextBox ID="tbCase8Conclusion" runat="server" TextMode="multiline" rows="8" maxlength="4000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
                    <tr id="trCase8Notify" runat="server">
                        <td class="columnHeader">
                            <asp:Label ID="lblCase8Notify" runat="server" text="Send Email Notification To The Team"></asp:Label>
                            <img src="/images/defaulticon/16x16/mail-sent.png" alt="" style="vertical-align: middle; margin-left: 4px;" />
                        </td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt">
                             <span>
                                <asp:CheckBox id="cbCase8Notify" runat="server" Checked="false"/>
                                <asp:Label id="lblCase8NotifyConfirm" runat="server" CssClass="refTextSmall" Visible="false" Text="Email notifications have been sent"></asp:Label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase8MessageTitle" runat="server" text="Notification Title"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:TextBox ID="tbCase8MessageTitle" runat="server" TextMode="multiline" rows="2" maxlength="200" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="columnHeader"><asp:Label ID="lblCase8Message" runat="server" text="Team Message"></asp:Label></td>
                        <td class="tableDataAlt">&nbsp;</td>
                        <td class="tableDataAlt"><asp:TextBox ID="tbCase8Message" runat="server" TextMode="multiline" rows="6" maxlength="2000" CssClass="commentArea WarnIfChanged"></asp:TextBox></td>
                    </tr>
                </table>
                <br />
                    <Ucl:Status id="uclStatus8" runat="server"/>
                </td>
            </tr>
            <tr id="trCase8OptionArea" runat="server">
                <td align="right" class="optionArea">
                <asp:UpdatePanel ID="udpCase8Save" runat="server" ChildrenAsTriggers=false UpdateMode=Conditional>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnCase8Save" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Button ID="btnCase8Cancel" CSSclass="buttonStd" runat="server" text="<%$ Resources:LocalizedText, Cancel %>"
                            onclick="btnCancel_Click" CommandArgument="8"></asp:Button>
                        <asp:Button ID="btnCase8Save" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" Enabled="true"  style="width: 70px;" title="save this step of the problem case"
                            OnClientClick="return confirmChange('Problem Case');" onclick="btnSave_Click" CommandArgument="8"></asp:Button>
                   </ContentTemplate>
                </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>
