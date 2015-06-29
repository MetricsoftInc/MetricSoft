<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MailTest.aspx.cs" Inherits="SQM.Website.Automated.MailTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Email Test</h1>
        <asp:Button ID="btnEmail" runat="server" Text="Send It!" OnClick="btnEmail_Click" />
    </div>
    </form>
</body>
</html>
