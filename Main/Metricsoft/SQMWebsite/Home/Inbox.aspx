<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Inbox.aspx.cs" Inherits="SQM.Website.Inbox" %>
<%@ Register src="~/Include/Ucl_TaskList.ascx" TagName="TaskList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminEdit.ascx" TagName="PrefsEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocList" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        window.onload = function () {
            var timeout = document.getElementById('hfTimeout').value;
            var timeoutWarn = ((parseInt(timeout)-2) * 60000);
            window.setTimeout(function() {alert("Your Session Will Timeout In Approximately 2 Minutes.  Please save your work or cancel the page if you are finished.")}, timeoutWarn);
        }
       </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="scripts/ps_admin.js" type="text/javascript"></script>
    <link href="css/PSSQM.css" rel="stylesheet" type="text/css" />
    <asp:HiddenField ID="hfBase" runat="server" />
    <asp:HiddenField id="hfTimeout" runat="server"/>
    <asp:HiddenField ID="hdCurrentActiveTab" runat="server" />
    <asp:HiddenField ID="hdCurrentActiveSecondaryTab" runat="server" />
    <asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
    <FORM name="dummy">
        <div>
            <table width="100%" border="0" cellspacing="0" cellpadding="1">
                <tr>
                    <td class="tabActiveTableBg" colspan="10" align="center">
			            <BR/>
                        <br />
                        <div id="divNavArea" runat="server"  class="navAreaLeft">
                            <table width="99%" border="0" cellspacing="0" cellpadding="0" id="Table3">
                                <tr align="center" height="24">
                                    <td  class="optMenu">
                                        <asp:LinkButton ID="lbHome1_tab" runat="server" class="optNav clickable" CommandArgument="1" 
                                        Text="Inbox" onclick="tab_Click"></asp:LinkButton>
                                    </td>
                                </tr>
                                <tr align="center" height="24">
                                    <td class="optMenu">
                                        <asp:LinkButton ID="lbHome3_tab" runat="server" class="optNav clickable" CommandArgument="3" 
                                            Text="Preferences" onclick="tab_Click"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>      
                        </div>
                        <div id="divWorkArea" runat="server" class="workAreaRight">
                            <table width="100%" border="0" cellspacing="0" cellpadding="1">
                                <tr>
                                    <td class="tabActiveTableBg" colspan="10" align="center">
                                        <asp:Panel runat="server" ID="pnlHome1" visible="false">
                                            <asp:Panel ID="pnlTasks" runat="server" GroupingText="Inbox" CssClass="sectionTitles" Width="99%" >
                                                <asp:Label ID="lblInboxInstruct" runat="server" CssClass="instructText" style="margin-left: 5px;" Text="Tasks requiring your attention. View or complete a task by clicking the link provided in the task description." ></asp:Label>
                                                <div id="divTaskList2" runat="server" visible ="false">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 8px;">
		                                                <tr>
			                                                <td class="navSectionBar" style="width: 20px;">
                                                                <button type="button" id="btnToggleTaskList2" onclick="ToggleSection('pnlTaskList2');" class="navSectionBtn" >
                                                                    <img id="pnlTaskList2_img" src="/images/defaulticon/16x16/arrow-8-up.png"/>
                                                                </button>
                                                            </td>
                                                            <td class="navSectionBar">
                                                               <asp:Label ID="lblTaskList2" runat="server" CssClass="prompt" Text="Overdue Tasks Escalated To Your Attention"></asp:Label>
                                                                &nbsp;&nbsp;(<asp:Label ID="lblTaskCount2" runat="server" CssClass="prompt"></asp:Label>)
                                                            </td>
		                                                </tr>
	                                                </table>
                                                    <asp:Panel id="pnlTaskList2" runat="server" Visible="false">
                                                        <Ucl:TaskList id="uclTaskList2" runat="server"/>
                                                    </asp:Panel>
                                                </div>
                                                
                                                <div id="divTaskList0" runat="server">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 8px;">
		                                                <tr>
			                                                <td class="navSectionBar" style="width: 20px;">
                                                                <button type="button" id="btnToggleTaskList0" onclick="ToggleSection('pnlTaskList0');" class="navSectionBtn" >
                                                                    <img id="pnlTaskList0_img" src="/images/defaulticon/16x16/arrow-8-up.png"/>
                                                                </button>
                                                            </td>
                                                            <td class="navSectionBar">
                                                                <asp:Label ID="lblTaskList0" runat="server" CssClass="prompt" Text="Tasks Assigned To You"></asp:Label>
                                                                &nbsp;&nbsp;(<asp:Label ID="lblTaskCount0" runat="server" CssClass="prompt"></asp:Label>)
                                                            </td>
		                                                </tr>
	                                                </table>
                                                    <asp:Panel id="pnlTaskList0" runat="server">
                                                        <Ucl:TaskList id="uclTaskList0" runat="server"/>
                                                    </asp:Panel>
                                                </div>

                                                <div id="divTaskList1" runat="server" visible ="false">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 8px;">
		                                                <tr>
			                                                <td class="navSectionBar" style="width: 20px;">
                                                                <button type="button" id="btnToggleTaskList1" onclick="ToggleSection('pnlTaskList1');" class="navSectionBtn" >
                                                                    <img id="pnlTaskList1_img" src="/images/defaulticon/16x16/arrow-8-up.png"/>
                                                                </button>
                                                            </td>
                                                            <td class="navSectionBar">
                                                                <asp:Label ID="lblTaskList1" runat="server" CssClass="prompt" Text="Tasks Delegated To You On Behalf Of Another User"></asp:Label>
                                                                &nbsp;&nbsp;(<asp:Label ID="lblTaskCount1" runat="server" CssClass="prompt"></asp:Label>)
                                                            </td>
		                                                </tr>
	                                                </table>
                                                    <asp:Panel id="pnlTaskList1" runat="server" Visible="false">
                                                        <Ucl:TaskList id="uclTaskList1" runat="server"/>
                                                    </asp:Panel>
                                                </div>
                                            </asp:Panel>
                                        </asp:Panel>      
                                             
                                        <asp:Panel runat="server" ID="pnlHome3" visible="false">
                                            <asp:Panel ID="pnlSessionPrefs" runat="server" GroupingText="My Preferences" CssClass="sectionTitles" Width="99%" >
                                                 <asp:Label ID="lblUserPrefInstruction" runat="server" CssClass="instructText" style="margin: 5px;" Text="Change your contact phone number or password" ></asp:Label>
                                                <asp:Panel ID="pnlUserPrefs" runat="server" style="margin-top: 5px;">
                                                    <Ucl:PrefsEdit id="uclPrefsEdit" runat="server"/>
                                                </asp:Panel>
                                            </asp:Panel>
                                        </asp:Panel>                 

                                    </td>
                                </tr>
                            </table>
                        </div>
                </td>
            </tr>
        </table>
     </div>
 </FORM>
</asp:Content>
