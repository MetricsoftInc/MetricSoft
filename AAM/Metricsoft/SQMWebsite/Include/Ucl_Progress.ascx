<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_Progress.ascx.cs" Inherits="SQM.Website.Ucl_Progress"  %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
    <div id="divProgress" runat="server">
        <center>
            <telerik:RadProgressManager id="radProgressMgr" runat="server" />
            <telerik:RadProgressArea ID="radProgressArea" runat="server" Skin="Metro" ProgressIndicators="FilesCountBar,CurrentFileName">
                <Localization CurrentFileName="Working: " />
            </telerik:RadProgressArea>
        </center>
    </div>
