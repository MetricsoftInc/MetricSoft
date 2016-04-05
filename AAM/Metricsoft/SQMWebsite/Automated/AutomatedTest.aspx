<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutomatedTest.aspx.cs" Inherits="SQM.Website.Automated.AutomatedTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Automated Test</title>
		<script type="text/javascript">
			function CloseWindow() {
				window.close();
			}
		</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="ltrStatus" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
