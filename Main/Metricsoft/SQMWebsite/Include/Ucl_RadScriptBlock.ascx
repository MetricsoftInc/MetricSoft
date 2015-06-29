<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_RadScriptBlock.ascx.cs" Inherits="SQM.Website.Ucl_RadScriptBlock" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
    <script type="text/javascript">
        function onPartSearch(sender, args) {
            var arrSplit = args.get_text().split("|");
            sender.get_inputElement().value = arrSplit[1];
        }
        function onQISearch(sender, args) {
            var arrSplit = args.get_text().split("|");
            sender.get_inputElement().value = arrSplit[0] + " - " + arrSplit[1];
        }
    </script>
</telerik:RadScriptBlock>