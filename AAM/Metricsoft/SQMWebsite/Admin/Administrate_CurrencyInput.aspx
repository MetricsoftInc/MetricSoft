<%@ Page Title="Historical Currency Input" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" CodeBehind="Administrate_CurrencyInput.aspx.cs"
    Inherits="SQM.Website.Administrate_CurrencyInput" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <script type="text/javascript">

        function checkInputValue(ccode) {
            var prevData = $("span[clientid='Previous_" + ccode + "']").text();
            var thisInput = $("input[clientid='Input_" + ccode + "']")
            var thisData = thisInput.val();
            var hiddenInput = $("span[clientid='Hidden_" + ccode + "']");
            var toolTip = $("img[clientid='Warning_" + ccode + "']");

            if (prevData !== "n/a") {
                var dataDiff = Math.abs(thisData - prevData);
                if (dataDiff / thisData > 0.15 || dataDiff / prevData > 0.15 || isNaN(prevData / thisData)) {
                    toolTip.removeClass("hidden");
                } else {
                    toolTip.addClass("hidden");
                }
            }
            return true;
        }


    </script>

    <div class="admin_tabs">
        <table width="100%" border="0" cellspacing="0" cellpadding="12">
            <tr>
                <td class="tabActiveTableBg" colspan="10">

                    <asp:Label ID="lblDataUploadTitle" runat="server" CssClass="pageTitles" Text="Historical Currency Exchange Rates"></asp:Label><br />
                    <asp:Label ID="lblPageInstructions" runat="server" CssClass="instructText" Text="Use this page to enter, view, or edit historical currency conversion rates for a given month and year.<br/>
                        Prior month exchange rates are given where available to help with error checking."></asp:Label>

                    <table width="100%" border="0" cellspacing="1" cellpadding="5" class="lightBorder">
                        <tr>
                            <td class="columnHeader" width="200">
                                <b>Currency Rates for Period</b>
                            </td>
                            <td class="tableDataAlt">
                                <telerik:RadMonthYearPicker ID="radPeriodSelect" runat="server"
                                    AutoPostBack="true" OnSelectedDateChanged="radPeriodSelect_SelectedDateChanged" CssClass="textStd" ShowPopupOnFocus="true" Skin="Metro">
                                </telerik:RadMonthYearPicker>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Label ID="lblMessage" runat="server" Text="" Visible="false" ForeColor="#880000" Font-Bold="true"></asp:Label>
                    <table class="currencyTable" cellpadding="0" cellspacing="0">
                        <tr>
                            <th colspan="2">From<img src="/images/defaulticon/16x16/arrow-4-right.png" alt="" style="float: right; opacity: 0.33;" /></th>
                            <th><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, To %>" /></th>
                            <th style="color: #8888aa;">
                                <asp:Literal ID="ltrPrevMonth" runat="server">Previous Month</asp:Literal><br />Exhange Rate</th>
                            <th colspan="2">
                                <asp:Literal ID="ltrCurrentMonth" runat="server">Current Month</asp:Literal><br />Exhange Rate</th>
                        </tr>
                        <asp:Repeater ID="rptCurrency" runat="server">
                            <ItemTemplate>
                                <tr class="<%# Container.ItemIndex % 2 == 0 ? "" : "alternate" %>">
                                    <td style="font-size: 12px; color: #4c4c4c;"><%# Eval("currencyName") %><%--</span>--%>
                                    </td>
                                    <td>
                                        <b><%# Eval("currencyCode") %></b>
                                        <asp:HiddenField ID="hfCurrencyCode" Value='<%# Eval("currencyCode") %>' runat="server" />
                                    </td>
                                    <td><b><%# Eval("baseCurrencyCode") %></b></td>
                                    <td>
                                        <asp:Label runat="server" clientid='<%# "Previous_" + Eval("currencyCode") %>'
                                            Text='<%# DisplayExchangeRate((decimal)Eval("previousValue")) %>' ForeColor="#8888aa"></asp:Label>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox runat="server" Text='<%# Eval("currentValue") %>' ID="tbRate" clientid='<%# "Input_" + Eval("currencyCode") %>'
                                            ClientEvents-OnValueChanged='<%# "function (sender,args){checkInputValue(\"" + Eval("currencyCode") + "\");}"%>'
                                            Width="100" NumberFormat-DecimalDigits="6" EnableViewState="true" IncrementSettings-InterceptMouseWheel="false" MinValue="0">
                                        </telerik:RadNumericTextBox>
                                    </td>
                                    <td>
                                        <asp:Image runat="server" ImageUrl="/images/status/warning.png"
                                            AlternateText="!" ID='imgWarning' ToolTip="Value is significantly different compared to last month's value."
                                            clientid='<%# "Warning_" + Eval("currencyCode") %>' CssClass="hidden" BorderColor="Transparent" BorderWidth="3" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                    <br />
                    <telerik:RadButton ID="rbSave" runat="server" Skin="Metro" Width="480" Text="Save Exchange Rates" OnClick="rbSave_Click"></telerik:RadButton>

                </td>
            </tr>
        </table>
    </div>
</asp:Content>
