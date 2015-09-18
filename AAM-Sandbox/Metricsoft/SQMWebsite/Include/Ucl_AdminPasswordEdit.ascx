﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AdminPasswordEdit.ascx.cs" Inherits="SQM.Website.Ucl_AdminPasswordEdit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">



	function PasswordValid(txt) {
		var valid = confirmChange(txt);
		if (valid == true) {
			if (isBlank(document.getElementById('tbCurrentPassword').value) == true || isBlank(document.getElementById('tbNewPassword').value) == true || isBlank(document.getElementById('tbConfirmPassword').value) == true) {
				valid = false;
				alert('Please enter all required fields');
			}
			if (document.getElementById('tbNewPassword').value != document.getElementById('tbConfirmPassword').value) {
				valid = false;
				alert('New Password and Confirm Password must match.');
			}
		}
		return valid;
	}

	function OpenPasswordEditWindow() {
	    $find("<%=winPasswordEdit.ClientID %>").show();
	}
    function OpenPasswordForgotWindow() {
        $find("<%=winPasswordForgot.ClientID %>").show();
    }

</script>

<asp:HiddenField ID="wndPercentageValue" runat="server" />
<asp:HiddenField ID="hfPasswordChangedSucces" runat="server" Value="Your password has been changed - click OK to continue logging in." />
<asp:HiddenField runat="server" ID="hfForgotPasswordSent" Value="An email has been sent with your new password.  Please return and use the new password to login."></asp:HiddenField>

<%--<telerik:RadWindow runat="server" ID="winPasswordEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="280" Width="500" Title="Change Password" Behaviors="Move">--%>
<telerik:RadWindow runat="server" ID="winPasswordEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true"  AutoSize="true" Title="Change Password" Behaviors="Move">
    <ContentTemplate>
        <div runat="server" id="divErrorMsg" visible="false">
	        <asp:Label runat="server" ID="lblPassMustUpdate" Text="You must reset your password to continue.<br><br>" CssClass="promptAlert" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblPassFailUpdate"  Text="An error was encountered updating the password. Please try again or contact your system administrator." CssClass="promptAlert" Visible="false"></asp:Label>
	        <asp:Label runat="server" ID="lblPassFailError" CssClass="promptAlert" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblPasswordNoConfirm"  Text="Password not confirmed. Please make sure the confirmation password and new password are the same." CssClass="promptAlert" Visible="false"></asp:Label>
	        <asp:Label runat="server" ID="lblPassFail10"  Text="The Current Password entered does not match your current password." CssClass="promptAlert" Visible="false"></asp:Label>
            <asp:Label runat="server" ID="lblPassPolicyFail"  Text="The password entered does not comply with the policy stated below - please re-enter a new password." CssClass="promptAlert" Visible="false"></asp:Label>
	        <asp:Label runat="server" ID="lblPasswordEmailSubject" Text=" Password Changed" Visible="false" ></asp:Label>
	        <asp:Label runat="server" ID="lblPasswordEmailBody1a" Visible="false" Text="Your password has been updated in the "></asp:Label>
	        <asp:Label runat="server" ID="lblPasswordEmailBody1b" Visible="false" Text=" application."></asp:Label>
	        <asp:Label runat="server" ID="lblPasswordEmailBody2" Text="Please do not reply to this message." Visible="false" ></asp:Label>
        </div>
        <asp:Panel ID="pnlPasswordEdit" runat="server" style="margin-top: 5px;">
            <asp:Label ID="lblPasswordPolicy" runat="server" CssClass="instructText"></asp:Label>
	        <table runat="server" id="tblPassword" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
              <tr style="height: 7px;"><td></td></tr>
		        <tr>
			        <td runat="server" id="tdCurrentPassword" class="columnHeader" style="width: 39%;">
				        <asp:Label ID="lblCurrentPassword" runat="server" Text="Current Password"></asp:Label>
			        </td>
			        <td runat="server" id="req1" class="required" style="width: 1%;">&nbsp;</td>
			        <td runat="server" id="tdCurrentPassTB" class="tableDataAlt" style="width: 60%;">
				        <asp:TextBox ID="tbCurrentPassword" size="25" MaxLength="20" runat="server" TextMode="Password" ></asp:TextBox>
				        <asp:RequiredFieldValidator runat="server" ID="rfvCurrentPassword" ControlToValidate="tbCurrentPassword" ValidationGroup="changepassword" Display="None" ErrorMessage="Current Password is required."></asp:RequiredFieldValidator>
			        </td>
		        </tr>
		        <tr>
			        <td runat="server" id="tdNewPassword" class="columnHeader">
				        <asp:Label ID="lblNewPassword" runat="server" Text="New Password" ></asp:Label>
			        </td>
			        <td runat="server" id="req2" class="required" style="width: 1%;">&nbsp;</td>
			        <td runat="server" id="tdNewPassTB" class="tableDataAlt">
				        <asp:TextBox ID="tbNewPassword" size="25" MaxLength="20" runat="server" TextMode="Password"></asp:TextBox>
			        </td>
		        </tr>
		        <tr>
			        <td runat="server" id="tdConfirmPassword" class="columnHeader">
				        <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password" ></asp:Label>
			        </td>
			        <td runat="server" id="req3" class="required" style="width: 1%;">&nbsp;</td>
			        <td runat="server" id="tdConfirmPassTB" class="tableDataAlt">
				        <asp:TextBox ID="tbConfirmPassword" size="25" MaxLength="20" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <center>
                            <br />
                            <asp:Button ID="btnPassCancel" CssClass="buttonStd" runat="server" Text="Cancel" OnClick="btnPassCancel_Click"></asp:Button>
                            &nbsp;&nbsp;
		                    <asp:Button ID="btnPassSave" CssClass="buttonEmphasis" runat="server" Text="Save Password" ToolTip="Update your new password"
			                    OnClientClick="return PasswordValid('User Password');" OnClick="btnPassSave_Click"></asp:Button>
                        </center>
			        </td>
		        </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</telerik:RadWindow>

<%--<telerik:RadWindow runat="server" ID="winPasswordForgot" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="200" Width="500" Title="Forgot Password" Behaviors="Move">--%>
<telerik:RadWindow runat="server" ID="winPasswordForgot" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" AutoSize="true" Title="Forgot Password" Behaviors="Move">
    <ContentTemplate>
		<asp:Panel ID="pnlForgot" runat="server" style="margin-top: 5px;">
			<table border="0" cellspacing="0" cellpadding="1">
                <tr>
                    <td style="padding-left: 10px; padding-bottom: 7px;">
                        <asp:Label ID="lblForgotPasswordInstruct" runat="server" CssClass="instructText" Text="Please enter the email address associated with your user account. A temporary password will be mailed to you.<br>Contact your system adminstrator if you require additional assistance."></asp:Label>
                    </td>
                </tr>
				<tr>
					<td style="padding-left: 10px;">
						<asp:Label ID="Label3" runat="server" class="prompt" Text="Email Address:"></asp:Label>
                        &nbsp;
						<asp:TextBox ID="tbEmail" runat="server" CausesValidation="false" Style="width: 300px;" MaxLength="50" ></asp:TextBox>
					</td>
				</tr>
                <tr>
                    <td style="padding-left: 10px; padding-top: 7px;">
                        <center>
                            <asp:Button runat="server" Text="Cancel" ID="btnCancelSendEmail" CssClass="buttonStd" style="margin-left: 10px;" ValidationGroup="verify" onclick="btnCancelSend_Click" />
                            <asp:Button runat="server" Text="Submit" ToolTip = "Send temporary account reset notification to this email address" ID="btnSendEmail" CssClass="buttonEmphasis" style="margin-left: 10px;" ValidationGroup="verify" onclick="btnVerify_Click" />
                        </center>
                    </td>
                </tr>
				<tr><td style="padding-left: 10px; padding-top: 7px;">
					<asp:Label runat="server" ID="lblForgotInvalidEmail" Text="The email entered does not exist in the system." class="promptAlert"  Visible="false"></asp:Label>
					<asp:Label runat="server" ID="lblForgotNotSent" Text="There was an error sending the email.  Please contact your system administrator." CssClass="promptAlert" Visible="false"></asp:Label>
					<asp:Label runat="server" ID="lblForgotNotUpdated" Text="There was an error updating the password. Please contact your system administrator." CssClass="promptAlert"  Visible="false"></asp:Label>
				</td></tr>
			</table>
			<asp:Label runat="server" ID="lblForgotPasswordEmailSubject" Text="Password Recovery" Visible="false" ></asp:Label>
			<asp:Label runat="server" ID="lblForgotPasswordEmailBody1a" Visible="false" Text="The following password has been randomly generated for your " ></asp:Label>
			<asp:Label runat="server" ID="lblForgotPasswordEmailBody1b" Visible="false" Text=" account from the Forgot Password option. You must login using this random password, and then you will be prompted to change the password. <br><br>Username: " ></asp:Label>
			<asp:Label runat="server" ID="lblForgotPasswordEmailBody2" Text="<br>New password: " Visible="false" ></asp:Label>
			<asp:Label runat="server" ID="lblForgotPasswordEmailBody3" Text="Please do not reply to this message." Visible="false" ></asp:Label>
		</asp:Panel>
    </ContentTemplate>
</telerik:RadWindow>