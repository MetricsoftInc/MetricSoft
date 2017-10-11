<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AddRegionalApprover.ascx.cs" Inherits="SQM.Website.Include.Ucl_AddRegionalApprover" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
    function OpenRegionalApproverEditWindow() {
        $find("<%=winRegionalApproverEdit.ClientID %>").show();
    }

    function CloseUSerEditWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window
        oWindow.Close();
    }


    function validateFunction() {
        var txtDescription = document.getElementById('<%= txtDescription.ClientID %>');
     <%--   var txtDescriptionQuestion = document.getElementById('<%= txtDescriptionQuestion.ClientID %>');--%>
        var ddlRegionalApprover = document.getElementById('<%= ddlRegionalApprover.ClientID %>');
        <%--var ddlStep = document.getElementById('<%= ddlStep.ClientID %>');--%>
        <%--var ddlPriv = document.getElementById('<%= ddlPriv.ClientID %>');--%>
        if (txtDescription.value == "") {
            alert("Enter Description.");
            return false;
        }
        //if (txtDescriptionQuestion.value == "") {
        //    alert("Enter Description Question.");
        //    return false;
        //}
        if (ddlRegionalApprover.value == "0") {
            alert("Select Regional Approver from list");
            return false;
        }
        //if (ddlStep.value == "0") {
        //    alert("Select Step from list");
        //    return false;
        //}
        //if (ddlPriv.value == "0") {
        //    alert("Select Privilege from list");
        //    return false;
        //}
        confirmChange('Regional Approver Information');
    }

</script>


<asp:Panel ID="pnlRegionalApproverAction" runat="server" CssClass="admBkgd" Visible="false">
    <asp:HiddenField ID="hfRegionalApproverActionContext" runat="server" />
    <asp:HiddenField ID="hfRegionalApproverActionBusLoc" runat="server" />
    <asp:Button ID="btnAddRegionalApproverAction" runat="server" CssClass="buttonAddLarge" Style="float: left; margin: 5px;" Text="Add Regional Approver" ToolTip="Add an Regional Approver" OnClick="btnRegionalApproverItemAdd_Click" />
    <telerik:RadGrid ID="rgRegionalApproverAction" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
        AutoGenerateColumns="false" OnItemDataBound="rgRegionalApproverAction_ItemDataBound" OnSortCommand="rgRegionalApproverAction_SortCommand"
        OnPageIndexChanged="rgRegionalApproverAction_PageIndexChanged" OnPageSizeChanged="rgRegionalApproverAction_PageSizeChanged" GridLines="None" Width="100%">
        <MasterTableView ExpandCollapseColumn-Visible="false">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Regional Approver Name" ShowSortIcon="true" SortExpression="RegionalApprover_SCOPE">
                    <ItemTemplate>
                        <asp:HiddenField ID="hfRegionalApproverItemID" runat="server" />
                        <asp:HiddenField ID="hfRegionalApproverSSO_ID" runat="server" />
                        <asp:HiddenField ID="hfRegionalApproverPerson_Id" runat="server" />
                        <asp:LinkButton ID="lnkRegionalApproverItem" OnClick="lnklRegionalApproverItem_Click" CommandArgument='<%#Eval("INCFORM_REGIONAL_APPROVER_LIST_ID") %>' Text='<%# Eval("Name") %>' runat="server" CssClass="buttonLink" Font-Bold="true" ForeColor="#000066" ToolTip="RegionalApprover Name"></asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="false">
                    <ItemTemplate>
                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
        <PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winRegionalApproverEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="600" Title="Add Regional Approver" Behaviors="Move,Close">
    <ContentTemplate>
        <asp:HiddenField ID="hfRegionalApproverActionID" runat="server" />
        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft editArea" style="margin: 8px 0 5px 0;">
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblDescription" runat="server" Text="Description"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:TextBox ID="txtDescription" runat="server" Skin="Metro" ZIndex="9000" MaxLength="200" Width="300"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="rfvLocalDescription" ControlToValidate="txtDescription" Display="None"
                            ErrorMessage="Enter Description" ValidationGroup="ValApp"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <%--<tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblDescriptionQuestion" runat="server" Text="Question"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:TextBox ID="txtDescriptionQuestion" runat="server" Skin="Metro" ZIndex="9000" MaxLength="200" Width="300"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtDescriptionQuestion" Display="None"
                            ErrorMessage="Enter Description Question" ValidationGroup="ValApp"></asp:RequiredFieldValidator>
                    </td>
                </tr>--%>
                <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblRegionalApprover" runat="server" Text="Regional Approver"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:DropDownList ID="ddlRegionalApprover" runat="server" Skin="Metro" ZIndex="9000" Width="300"></asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="ddlRegionalApprover" Display="None"
                            InitialValue="[Select One]" ErrorMessage="Select an option from list" ValidationGroup="ValApp"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <%--<tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblStep" runat="server" Text="Step"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:DropDownList ID="ddlStep" runat="server" Skin="Metro" ZIndex="9000" Width="300">
                            <Items>
                                <asp:ListItem Text="select An Option From List" Value="0" runat="server" />
                                <asp:ListItem Text="Flash Report Signoff" Value="2.50" runat="server" />
                                <asp:ListItem Text="Investigation Report Signoff" Value="5.50" runat="server" />
                                <asp:ListItem Text="Incident Closure" Value="10.0" runat="server" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="ddlStep" Display="None"
                            InitialValue="[Select One]" ErrorMessage="Select an option from list" ValidationGroup="ValApp"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                     <tr>
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblPrivGroup" runat="server" Text="Priv Group"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:DropDownList ID="ddlPriv" runat="server" Skin="Metro" ZIndex="9000" Width="300" EmptyMessage="Please select step from list.">
                            <Items>
                                <asp:ListItem Text="select An Option From List" Value="0" runat="server" />
                                <asp:ListItem Text="RegionalApprover Level 1" Value="390" runat="server" />
                                <asp:ListItem Text="RegionalApprover Level 2" Value="391" runat="server" />
                                <asp:ListItem Text="RegionalApprover Level 3" Value="392" runat="server" />
                                <asp:ListItem Text="RegionalApprover Level 4" Value="393" runat="server" />
                                <asp:ListItem Text="RegionalApprover Level 5" Value="394" runat="server" />
                                <asp:ListItem Text="RegionalApprover Level 6" Value="395" runat="server" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="ddlPriv" Display="None"
                            InitialValue="[Select One]" ErrorMessage="Select an option from list" ValidationGroup="ValApp"></asp:RequiredFieldValidator>
                    </td>
                </tr>--%>
            </table>
        </telerik:RadAjaxPanel>
        <span style="float: right; margin: 10px;">
            <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" Style="width: 70px;" OnClick="OnCancelRegionalApproverAction_Click"></asp:Button>
            <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" Style="width: 70px;" OnClientClick="return validateFunction();"
                ValidationGroup="ValApp" OnClick="OnSaveRegionalApproverAction_Click"></asp:Button>
            &nbsp;
				<asp:Button ID="btnDelete" class="buttonLink" runat="server" Text="<%$ Resources:LocalizedText, Delete %>" Style="width: 70px;" OnClientClick="return confirmAction('Delete This RegionalApprover Information');" OnClick="OnDeleteRegionalApproverAction_Click"></asp:Button>
        </span>
        <br />
        <center>
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
            </center>
    </ContentTemplate>
</telerik:RadWindow>
