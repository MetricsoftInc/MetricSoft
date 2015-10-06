<%@ Page Title="" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" CodeBehind="TaskAction.aspx.cs" Inherits="SQM.Website.TaskAction" %>
<%@ Register src="~/Include/Ucl_TaskStatus.ascx" TagName="Task" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:HiddenField ID="hfBase" runat="server" />
	<asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
	<div>
		<div style="margin: auto; width: 97%; padding: 5px;">
			<div margin: 5px;" class="noprint">
				<asp:Label ID="lblPageTitle" runat="server"  CssClass="pageTitles" Text="Task Action" ></asp:Label>
				<br />
				<asp:Label ID="lblPageInstruct" runat="server" CssClass="instructText" Text="The task below has been assigned to you. You may indicate the task has been completed or re-assign to another person."></asp:Label>
				<br />
			</div>
			<Ucl:Task ID="uclTask" runat="server" />
		</div>
	</div>
</asp:Content>

