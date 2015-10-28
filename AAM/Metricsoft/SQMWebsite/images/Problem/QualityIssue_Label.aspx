<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QualityIssueLabel.aspx.cs" Inherits="SQM.Website.QualityIssueLabel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<link href="../css/PSSQM.css" rel="stylesheet" type="text/css" />
<link href="../css/probSolver_default2.css" rel="stylesheet" type="text/css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<script src="/scripts/ps_admin.js" type="text/javascript"></script>
<script type="text/javascript">

</script>

<body bgcolor="#FFFFFF" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <div  id="SummaryDiv" class="error_popupdiv">
</div>

        <div id="divLabel" runat="server">
        </div>
        <br />
       <input type="button" id="btnPrintLabel" value="<%$ Resources:LocalizedText, Print %>" class="buttonStd" style="width:80px;margin-left:40px;" onclick="javascript:window.close();"></input>
    </form>
</body>
</html>

