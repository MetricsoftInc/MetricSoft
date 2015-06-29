
<%@ Page Title="" Language="C#" MasterPageFile="~/QAILogin.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SQM.Website.Login"  %>
<%@ Register src="~/Include/Ucl_AdminPasswordEdit.ascx" TagName="PassEdit" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="scripts/ps_admin.js" type="text/javascript"></script>
    <script type="text/javascript">
    var pageLoaded = false;
    window.onload = function () {
        pageLoaded = true;
        // mt - let's turn off the autocomplete attribute for all text input fields 
        // mt - added to remediate potential 'session fixation' security threat
        var inputElements = document.getElementsByTagName("input");
        for (i = 0; inputElements[i]; i++) {
            if (inputElements[i].id.indexOf('_tb') > -1) {
                inputElements[i].setAttribute("autocomplete", "off");
            }
        }
    }

    function AlertPostback(message, cmd) {
        if (message != '')
            alert(message);
        __doPostBack(cmd);
    }

    function ClearMessages() {
        var lbl = document.getElementById('lblLoginError_out');
        if (lbl) {
            lbl.style.display = 'none';
        }
    }

    </script>

	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td valign="top">
                    <asp:Panel runat="server" ID="pnlLogin">
						<table border="0" cellspacing="0" cellpadding="1">
                            <tr>
                                <td colspan="2" style="padding-left: 10px; padding-bottom: 5px;" >
                                    <asp:Label ID="lblMainInfo" runat="server" CssClass="textStd" visible="false"></asp:Label>
                                </td>
                            </tr>
							<tr>
								<td style="padding-left: 10px;">
									<asp:Label ID="Label1" runat="server" class="prompt" Text="Username:"></asp:Label>
									<asp:TextBox ID="tbUsername" runat="server" CausesValidation="false" Style="width: 170px" Text="" 
										MaxLength="32" ></asp:TextBox>
								</td>
								<td>
                                    &nbsp;&nbsp;&nbsp;
									<asp:Label ID="Label2" runat="server" class="prompt" Text="Password:"></asp:Label>
									<asp:TextBox ID="tbPassword" runat="server" CausesValidation="false" TextMode="Password"
										MaxLength="32" Style="width: 170px" onblur="ClearMessages();" ></asp:TextBox>&nbsp;
									<asp:Button runat="server" Text="Login" ID="btnLogin" Cssclass="buttonEmphasis" style="width: 80px;"
										OnClick="btnLogin_Click"/>
                                    &nbsp;&nbsp&nbsp;
									<asp:LinkButton runat="server" ID="lnkForgotPassword" class="buttonLinkSmall" 
										Text="Forgot Username/Password ?" onClick="lnkForgotPassword_Click" ></asp:LinkButton>
								</td>
							</tr>
                            <tr style="height: 5px;"><td colspan="2"></td></tr>
                            <tr>
                                <td style="padding-left: 10px;" colspan="2">
                                    <asp:Label runat="server" ID="lblLoginError_out" class="promptAlert" Visible="false" Text="The System could not log you in. Please verify the username and password entered are correct."></asp:Label>
                                    <asp:Label runat="server" ID="lblLoginAttemptError_out" class="promptAlert" Visible="false" Text="The System could not log you in. Please click the Forgot Username/Password link or contact your system administrator for assistance."></asp:Label>
                                    <asp:Label runat="server" ID="lblSessionError_out" class="promptAlert" Visible="false" Text="A different user is already logged into the system from another browser page."></asp:Label>
                                </td>
                            </tr>
						</table>
                    </asp:Panel>

					<asp:Panel runat="server" ID="pnlLoginPasswordEdit" Visible="false">
						<Ucl:PassEdit ID="uclPassEdit" runat="server" strCurrentControl="login" />
					</asp:Panel>
			</td>
		</tr>
	</table>

     <div id="divLinks" runat="server" style="margin-top: 10px;" visible="false">
     </div>

    <div id="divAnnouncements" runat="server" style="margin-top: 10px;">
        <div id="divLoginMessage" runat="server" class="borderSoft" style="width: 98%; padding: 4px; margin-bottom: 7px;" visible="false">
            <asp:Label ID="lblLoginMessage" runat="server" CssClass="instructText"></asp:Label>
        </div>
        <asp:Panel ID="pnlPosting" runat="server"  HorizontalAlign="Center"  GroupingText="Corporate Objectives & Performance" CssClass="sectionTitles" Width="99%" Visible="false"> 
            <center>
               <div id="divTicker" runat="server" visible="false">
                    <asp:Label id="lblStats" runat="server" CssClass="prompt" text="Number Of Days Without A Lost Time Case"></asp:Label>
                    <Ucl:RadGauge id="uclGauge" runat="server"/>
                </div>
                <img runat="server" id="imgPostingEHS" style="height: auto; width: auto;" />
                <img runat="server" id="imgPostingQS" style="height: auto; width: auto;" />
            </center>
        </asp:Panel>
	</div>

<%--    </FORM>--%>
</asp:Content>
