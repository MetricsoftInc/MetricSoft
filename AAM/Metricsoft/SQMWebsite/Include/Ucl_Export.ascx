<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_Export.ascx.cs" Inherits="SQM.Website.Ucl_Export"  %>

    <asp:HiddenField id="hfShowProgress" runat="server" Value="true"/>
    <div id="divExport" runat="server">
         <asp:LinkButton  ID="lnkExport" runat="server" Text="<%$ Resources:LocalizedText, Export %>" ToolTip="Export to Excel Format" CssClass="buttonDownload" style="margin-left: 5px;" OnClick="lnkExportClick"></asp:LinkButton>
    </div>
