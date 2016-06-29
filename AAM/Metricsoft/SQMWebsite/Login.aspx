<%@ Page Title="" Language="C#" MasterPageFile="~/RspQAILogin.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SQM.Website.Login" %>

<%@ Register Src="~/Include/Ucl_AdminPasswordEdit.ascx" TagName="PassEdit" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
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

			//$("#viewPortWidth").val() = $(window).width();
			
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

		$(document).ready(function () {

			//alert($(window).width());
			$('#<%= viewPortWidth.ClientID %>').val($(window).width());
			//$("#viewPortWidth").val() = $(window).width();
			

		});


	</script>


	<asp:HiddenField ID="viewPortWidth" runat="server" />
	
	<div class="container-fluid">

		<asp:Panel runat="server" ID="pnlLogin">

			<div class="row-fluid">
				<div class="col-xs-12  text-left">
					<p>
						<asp:Label ID="lblMainInfo" runat="server" CssClass="textStd" Visible="False"></asp:Label>
					</p>
				</div>
			</div>

			<div class="row-fluid">

				<div class="col-xs-12 text-left">

					<asp:Label ID="Label1" runat="server" class="prompt" Text="Username:" meta:resourcekey="Label1Resource1"></asp:Label>
					<asp:TextBox ID="tbUsername" runat="server" Style="width: 170px; margin: 5px 4px 0 0;"
						MaxLength="32"></asp:TextBox>


					<div class="clearfix visible-xs"></div>
					<br class="visible-xs-block" />

					<br />
					<asp:Label ID="Label2" runat="server" class="prompt"  Text="Password:" meta:resourcekey="Label2Resource1"></asp:Label>
					<asp:TextBox ID="tbPassword" runat="server" TextMode="Password" 
						MaxLength="32" Style="width: 170px; margin-top: 8px;  margin: 5px 4px 0 2px;" onblur="ClearMessages();"></asp:TextBox>

					<div class="clearfix visible-xs"></div>
					<br class="visible-xs-block" />

					<telerik:RadButton ID="btnLogin" runat="server" meta:resourcekey="btnLoginResource1" CssClass="buttonEmphasis" Skin="Metro" Style="width: 85px;  margin: 5px 10px 0 5px; background-color: #A3461F;"
						OnClick="btnLogin_Click" SingleClick="true"/>

					<%--<asp:Button runat="server" Text="Login" ID="btnLogin" CssClass="buttonEmphasis" Style="width: 80px;  margin: 5px 10px 0 5px;"
						OnClick="btnLogin_Click" meta:resourcekey="btnLoginResource1" />--%>
					<asp:LinkButton runat="server" ID="lnkForgotPassword" class="buttonLinkSmall"
						Text="Forgot Username/Password ?" OnClick="lnkForgotPassword_Click" Visible="False" meta:resourcekey="lnkForgotPasswordResource1"></asp:LinkButton>
				</div>

			</div>
		

			<div class="row-fluid">
				<div class="col-xs-12 text-left">
					<asp:Label runat="server" ID="lblLoginError_out" class="promptAlert" Visible="False" Text="The System could not log you in. Please verify the username and password entered are correct." meta:resourcekey="lblLoginError_outResource1"></asp:Label>
					<asp:Label runat="server" ID="lblLoginAttemptError_out" class="promptAlert" Visible="False" Text="The System could not log you in. Please click the Forgot Username/Password link or contact your system administrator for assistance." meta:resourcekey="lblLoginAttemptError_outResource1"></asp:Label>
					<asp:Label runat="server" ID="lblSessionError_out" class="promptAlert" Visible="False" Text="A different user is already logged into the system from another browser page." meta:resourcekey="lblSessionError_outResource1"></asp:Label>
				</div>
			</div>
		</asp:Panel>

		<div class="row-fluid">
			<div class="col-xs-12 text-left">
				<asp:Panel runat="server" ID="pnlLoginPasswordEdit" Visible="False">
					<Ucl:PassEdit ID="uclPassEdit" runat="server" strCurrentControl="login" />
				</asp:Panel>
			</div>
		</div>

		<div class="row-fluid">
			<div class="col-xs-12 text-left">
				<div id="divLinks" runat="server" style="margin-top: 10px;" visible="false">
				</div>
			</div>
		</div>

		<div class="row-fluid">
			<div class="col-xs-12 text-left">

				<div id="divAnnouncements" runat="server" style="margin-top: 10px;">
					<div id="divLoginMessage" runat="server" class="borderSoft" style="width: 98%; padding: 4px; margin-bottom: 7px;" visible="false">
						<asp:Label ID="lblLoginMessage" runat="server" CssClass="instructText"></asp:Label>
					</div>

					<asp:Panel ID="pnlPosting" runat="server" HorizontalAlign="Center" CssClass="" Width="99%" Visible="False">
						<%--GroupingText="Corporate Objectives & Performance"--%>
						<asp:Label ID="lblPosting" runat="server" CssClass="sectionTitles" Text="Corporate Objectives & Performance"></asp:Label>
						<center>
							<div id="divTicker" runat="server" visible="False">
								<asp:Label ID="lblStats" runat="server" CssClass="prompt" Text="Number Of Days Without A Lost Time Case" meta:resourcekey="lblStatsResource1"></asp:Label>
								<Ucl:RadGauge ID="uclGauge" runat="server" />
							</div>
							<img runat="server" id="imgPostingEHS" style="height: auto; width: auto;" />
							&nbsp;<img runat="server" id="imgPostingQS" style="height: auto; width: auto;" />
						</center>
						<br />
					</asp:Panel>

				</div>
			</div>
		</div>

	</div>

	<%--    </FORM>--%>
</asp:Content>
