<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_QISearch.ascx.cs" Inherits="SQM.Website.Ucl_QISearch" %>
<%@ Register src="~/Include/Ucl_IncidentList.ascx" TagName="IssueList" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Panel ID="pnlQISearch" runat="server">
    <Ucl:IssueList id="uclIssueSearch" runat="server"/>
    <Ucl:IssueList id="uclIssueList" runat="server"/>
</asp:Panel>
