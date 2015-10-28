<%@ Page Title="Currency Test" Language="C#" MasterPageFile="~/PSMaster.Master"
	AutoEventWireup="true" EnableEventValidation="false" CodeBehind="CurrencyTest.aspx.cs"
	Inherits="SQM.Website.CurrencyTest" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<div class="admin_tabs">
		<table width="100%" border="0" cellspacing="0" cellpadding="1">
			<tr>
				<td class="tabActiveTableBg" colspan="10" align="center">
					<br />
					<table width="99%">
						<tr>
							<td class="pageTitles">
								<asp:Label ID="lblViewEHSRezTitle" runat="server" Text="Currency Test"></asp:Label>
								<br />
								<asp:Label ID="lblPageInstructions" runat="server" class="instructText" Text=""></asp:Label>
							</td>
						</tr>
					</table>
					<br />
					<telerik:RadFormDecorator ID="RadFormDecorator1" DecorationZoneID="divPageBody" DecoratedControls="All"
						Skin="Metro" runat="server" />
						<div id="divPageBody" class="textStd" style="text-align: left; margin: 0 10px;" runat="server">
						<div class="blueCell" style="padding: 10px; font-size: 13px;">
							Convert
							&nbsp;
							<asp:TextBox ID="tbQuantity" runat="server">1</asp:TextBox>
							<asp:DropDownList ID="ddlFromCurrency" AutoPostBack="false" runat="server">
								<asp:ListItem Selected="True">USD</asp:ListItem>
								<asp:ListItem>CAD</asp:ListItem>
								<asp:ListItem>CNY</asp:ListItem>
								<asp:ListItem>CZK</asp:ListItem>
								<asp:ListItem>EUR</asp:ListItem>
								<asp:ListItem>HKD</asp:ListItem>
								<asp:ListItem>INR</asp:ListItem>
								<asp:ListItem>JPY</asp:ListItem>
								<asp:ListItem>MXN</asp:ListItem>
								<asp:ListItem>RUB</asp:ListItem>
							</asp:DropDownList>
							to
								<asp:DropDownList ID="ddlToCurrency" AutoPostBack="false" runat="server">
								<asp:ListItem>USD</asp:ListItem>
								<asp:ListItem>CAD</asp:ListItem>
								<asp:ListItem>CNY</asp:ListItem>
								<asp:ListItem>CZK</asp:ListItem>
								<asp:ListItem Selected="True">EUR</asp:ListItem>
								<asp:ListItem>HKD</asp:ListItem>
								<asp:ListItem>INR</asp:ListItem>
								<asp:ListItem>JPY</asp:ListItem>
								<asp:ListItem>MXN</asp:ListItem>
								<asp:ListItem>RUB</asp:ListItem>
							</asp:DropDownList>
							for Year/Month:
							<asp:DropDownList ID="ddlYear" AutoPostBack="false" runat="server">
								<asp:ListItem>2014</asp:ListItem>
                                <asp:ListItem>2013</asp:ListItem>
								<asp:ListItem>2012</asp:ListItem>
								<asp:ListItem>2011</asp:ListItem>
								<asp:ListItem>2010</asp:ListItem>
								<asp:ListItem>2009</asp:ListItem>
								<asp:ListItem>2008</asp:ListItem>
								<asp:ListItem>2007</asp:ListItem>
								<asp:ListItem>2006</asp:ListItem>
								<asp:ListItem>2005</asp:ListItem>
								<asp:ListItem>2004</asp:ListItem>
								<asp:ListItem>2003</asp:ListItem>
								<asp:ListItem>2002</asp:ListItem>
								<asp:ListItem>2001</asp:ListItem>
								<asp:ListItem>2000</asp:ListItem>
							</asp:DropDownList>
							<asp:DropDownList ID="ddlMonth" AutoPostBack="false" runat="server">
								<asp:ListItem>01</asp:ListItem>
								<asp:ListItem>02</asp:ListItem>
								<asp:ListItem>03</asp:ListItem>
								<asp:ListItem>04</asp:ListItem>
								<asp:ListItem>05</asp:ListItem>
								<asp:ListItem>06</asp:ListItem>
								<asp:ListItem>07</asp:ListItem>
								<asp:ListItem>08</asp:ListItem>
								<asp:ListItem>09</asp:ListItem>
								<asp:ListItem>10</asp:ListItem>
								<asp:ListItem>11</asp:ListItem>
								<asp:ListItem>12</asp:ListItem>
							</asp:DropDownList>
                            <br />
                            Base currency:
							<asp:DropDownList ID="ddlBaseCurrency" AutoPostBack="false" runat="server">
								<asp:ListItem>USD</asp:ListItem>
								<asp:ListItem>CAD</asp:ListItem>
								<asp:ListItem>CNY</asp:ListItem>
								<asp:ListItem>CZK</asp:ListItem>
								<asp:ListItem Selected="True">EUR</asp:ListItem>
								<asp:ListItem>HKD</asp:ListItem>
								<asp:ListItem>INR</asp:ListItem>
								<asp:ListItem>JPY</asp:ListItem>
								<asp:ListItem>MXN</asp:ListItem>
								<asp:ListItem>RUB</asp:ListItem>
							</asp:DropDownList>
							<telerik:RadButton ID="btnSubmit" runat="server" Text="<%$ Resources:LocalizedText, Submit %>" Width="100" Skin="Metro" OnClick="btnSubmit_Click" />
						</div>
						<br />
						<h2><asp:Label ID="lblResults" runat="server" /></h2>

						<br />
						<br />
						<br />
					</div>
				</td>
			</tr>
		</table>
	</div>
</asp:Content>
