<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SQM.Website.Home" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<%@ Register src="~/Include/Ucl_TaskList.ascx" TagName="TaskList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminEdit.ascx" TagName="PrefsEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DashboardArea.ascx" TagName="Dashboard" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="scripts/ps_admin.js" type="text/javascript"></script>
    <link href="css/PSSQM.css" rel="stylesheet" type="text/css" />
    <asp:HiddenField ID="hfBase" runat="server" />
    <asp:HiddenField ID="hdCurrentActiveTab" runat="server" />
    <asp:HiddenField ID="hdCurrentActiveSecondaryTab" runat="server" />
    <FORM name="dummy">
        <div>
            <table width="100%" border="0" cellspacing="0" cellpadding="1">
                <tr>
                    <td class="tabActiveTableBg" colspan="10" align="center">
			            <BR/>
                        <FORM name="dummy">
                        <br />
                        <div id="divNavArea" runat="server"  class="navAreaLeft">
                            <table width="99%" border="0" cellspacing="0" cellpadding="0" id="Table3">
                                <tr align="center" height="24">
                                    <td class="optMenu">
                                        <asp:LinkButton ID="lbHome2_tab" runat="server" class="optNav clickable" CommandArgument="2" 
                                            Text="Dashboard" onclick="tab_Click"></asp:LinkButton>
                                    </td>
                                </tr>
                                <tr align="center" height="24">
                                    <td  class="optMenu">
                                        <asp:LinkButton ID="lbHome1_tab" runat="server" class="optNav clickable" CommandArgument="1" 
                                        Text="Inbox" onclick="tab_Click"></asp:LinkButton>
                                    </td>
                                </tr>
                                <tr align="center" height="24">
                                    <td class="optMenu">
                                        <asp:LinkButton ID="lbHome4_tab" runat="server" class="optNav clickable" CommandArgument="4" 
                                            Text="Communication" onclick="tab_Click"></asp:LinkButton>
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
                                            <asp:Panel ID="pnlTasks" runat="server" GroupingText="My Inbox" CssClass="sectionTitles" Width="99%" style="margin: 2px;">
                                                <asp:Label ID="lblInboxInstruct" runat="server" CssClass="instructText" style="margin-left: 5px;" Text="Tasks requiring your attention or approval. Click on the links provided to go to the related work screen and complete the specified task(s). Note that some tasks may be delegated to you from another user." ></asp:Label>
                                                <Ucl:TaskList id="uclTaskList" runat="server"/>
                                            </asp:Panel>
                                        </asp:Panel>      

                                        <asp:Panel runat="server" ID="pnlHome4" visible="false">
                                            <asp:Panel ID="pnlCommunications" runat="server" GroupingText="Documents" CssClass="sectionTitles" Width="99%" style="margin: 5px;">
                                                <Ucl:DocList id="uclDocList" runat="server"/>
                                                <br />
                                                <table width="100%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
                                                    <tr>
                                                        <td class="columnHeader" width="29%">
                                                            <asp:Label ID="lblPostDocument" runat="server" text="Post Documents To My Working Location"></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt" width="1%">&nbsp;</td>
                                                        <td class="tableDataAlt" width="70%">
                                                            <asp:ImageButton ID="btnPostDocument" runat="server" ImageUrl="~/images/attach.png" title="attach additional documentation" OnClientClick="PopupCenter('../Shared/Shared_Upload.aspx', 'newPage', 800, 600);  return false;" />
                                                             &nbsp;
                                                            <asp:Label ID="lblPostedDocuments" runat="server" CssClass="refTextSmall"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </asp:Panel>
                         
                                        <asp:Panel runat="server" ID="pnlHome3" visible="false">
                                            <asp:Panel ID="pnlSessionPrefs" runat="server" GroupingText="My Preferences" CssClass="sectionTitles" Width="99%" style="margin: 3px;">
                                                <asp:Label runat="server" ID="lblUserPrefs" CssClass="prompt"  Text="User Account Preferences:"></asp:Label>
                                                <asp:Label ID="lblUserPrefInstruction" runat="server" CssClass="instructText" style="margin: 5px;" Text="Change your contact phone number, default language and task delegation account settings" ></asp:Label>
                                                <asp:Panel ID="pnlUserPrefs" runat="server" style="margin-top: 5px;">
                                                    <Ucl:PrefsEdit id="uclPrefsEdit" runat="server"/>
                                                </asp:Panel>
                                            </asp:Panel>
                                        </asp:Panel>                 

                                        <asp:Panel runat="server" ID="pnlHome2" visible="false">
                                            <asp:Panel ID="pnlMonitor" runat="server" GroupingText="My Dashboard" CssClass="sectionTitles" Width="99%" style="margin: 3px;">
                                                <Ucl:Dashboard  ID="uclDashbd0" runat="server"/>
                                            </asp:Panel>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </form>
                </td>
            </tr>
        </table>
     </div>
 </FORM>
</asp:Content>
