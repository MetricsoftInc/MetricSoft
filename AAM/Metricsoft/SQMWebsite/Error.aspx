<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="SQM.Website.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body style="background-color: #FFFFFF">
    <form id="form1" runat="server">
    <div id="bounding_box" style="border: 1px solid #AAA7A0; background-color: #FFFFFF;">
        <div>
            <table class="page" border="0" cellspacing="0" cellpadding="0" style="height: 300px" width="650px">
                <tr valign="top">
                    <td align="center" width="15%" valign="middle">
                    </td>
                    <td align="left">
                        <p style="margin-left: 20px; margin-right: 50px; font-style: italic; font-weight: bold; font-size: 20px; color: #191970;">
                            <asp:Label ID="lblTitle" runat="server" Text="The Application has encountered an error."></asp:Label>
                            <br />
                        </p>
                        <p style="margin-left: 20px; margin-right: 50px; font-weight: bold; font-size: 14px;">
                            <asp:Label ID="lblErrorIndex" runat="server" Text="Error Index: " ></asp:Label>
                            <br />
                            <asp:Label ID="lblError" runat="server" Text="Error: "></asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="lblGenericError" runat="server" Text="To help us resolve this issue, please communicate the Error Information shown above to your system administrator.  Press 'OK' to return to the Login screen." Font-Bold="False"></asp:Label>
                        </p>
                    </td>
                </tr>
                <tr style="height: 50px" valign="top">
                    <td style="margin-left: auto; margin-right: auto" align="center" colspan="2">
                        <input id="Button1" type="button" value="<%$ Resources:LocalizedText, OK %>" onclick="location.href='login.aspx'" style="width: 60px" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
