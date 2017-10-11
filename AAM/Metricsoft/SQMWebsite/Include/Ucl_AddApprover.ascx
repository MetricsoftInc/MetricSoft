<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AddApprover.ascx.cs" Inherits="SQM.Website.Include.Ucl_AddApprover" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
    function OpenApproverEditWindow() {
        $find("<%=winApproverEdit.ClientID %>").show();
    }

    function CloseUSerEditWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window
        oWindow.Close();
    }


    function validateFunction() {
        var txtDescription = document.getElementById('<%= txtDescription.ClientID %>');
       <%-- var txtDescriptionQuestion = document.getElementById('<%= txtDescriptionQuestion.ClientID %>');--%>
        <%--var ddlApprover = document.getElementById('<%= ddlApprover.ClientID %>');--%>
       <%-- var ddlStep = document.getElementById('<%= ddlStep.ClientID %>');--%>
        <%--var ddlPriv = document.getElementById('<%= ddlPriv.ClientID %>');--%>
        var ddlApproverType = document.getElementById('<%= ddlApproverType.ClientID %>');
        if (ddlApproverType.value == "0") {
            alert("Select Approver Type from list");
            return false;
        }
        if (txtDescription.value == "") {
            alert("Enter Description.");
            return false;
        }
        //if (txtDescriptionQuestion.value == "") {
        //    alert("Enter Description Question.");
        //    return false;
        //}
        if (ddlApprover.value == "0") {
            alert("Select Approver from list");
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
        confirmChange('Approver Information');
    }

</script>

<style>
    .CSSHI
    {
        background-color: #ff8c8c;
    }
    .CSSRHI
    {
        background-color: #ffffff;
    }
</style>
<asp:Panel ID="pnlApproverAction" runat="server" CssClass="admBkgd" Visible="false">
    <asp:HiddenField ID="hfApproverActionContext" runat="server" />
    <asp:HiddenField ID="hfApproverActionBusLoc" runat="server" />
    <asp:Button ID="btnAddApproverAction" runat="server" CssClass="buttonAddLarge" Style="float: left; margin: 5px;" Text="Add Approver" ToolTip="Add an Approver" OnClick="btnApproverItemAdd_Click" />
    <telerik:RadGrid ID="rgApproverAction" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
        AutoGenerateColumns="false" OnItemDataBound="rgApproverAction_ItemDataBound" OnSortCommand="rgApproverAction_SortCommand"
        OnPageIndexChanged="rgApproverAction_PageIndexChanged" OnPageSizeChanged="rgApproverAction_PageSizeChanged" GridLines="None" Width="100%">
        <MasterTableView ExpandCollapseColumn-Visible="false">
            <Columns>
                
                <telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="false" ItemStyle-Width="20%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkApproverItem" OnClick="lnklApproverItem_Click"  CommandArgument='<%#Eval("INCFORM_APPROVER_LIST_ID") %>' Text='<%# Eval("Description") %>' runat="server" CssClass="buttonLink" Font-Bold="true" ForeColor="#000066" ToolTip='<%# Eval("Description") %>'></asp:LinkButton>
                        <%--<asp:Label ID="lblDescription" runat="server" Text=''></asp:Label>--%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Approver Name" ShowSortIcon="true" SortExpression="Approver_SCOPE">
                    <ItemTemplate>
                        <asp:HiddenField ID="hfApproverItemID" runat="server" />
                        <asp:HiddenField ID="hfApproverSSO_ID" runat="server" />
                        <asp:HiddenField ID="hfApproverPerson_Id" runat="server" />
                        <asp:HiddenField ID="hfApproverType" runat="server" />
                         <asp:DropDownList ID="ddlApproverList" runat="server" Skin="Metro" ZIndex="9000" Width="300" CommandArgument='<%#Eval("INCFORM_APPROVER_LIST_ID") %>' AutoPostBack="true" OnSelectedIndexChanged="ddlApproverList_Click"></asp:DropDownList>
                        <%----%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>

    <br />
    <asp:Button ID="btnAddRegionalApproverAction" runat="server" CssClass="buttonAddLarge" Style="float: left; margin: 5px;" Text="Add Regional Approver" ToolTip="Add an Approver" OnClick="btnAddRegionalApproverAction_Click" />
    <telerik:RadGrid ID="rgRegionalApproverAction" runat="server" Skin="Metro" AllowSorting="True" AllowPaging="True" PageSize="20"
        AutoGenerateColumns="false" OnItemDataBound="rgRegionalApproverAction_ItemDataBound" OnSortCommand="rgRegionalApproverAction_SortCommand"
        OnPageIndexChanged="rgRegionalApproverAction_PageIndexChanged" OnPageSizeChanged="rgRegionalApproverAction_PageSizeChanged" GridLines="None" Width="100%">
        <MasterTableView ExpandCollapseColumn-Visible="false">
            <Columns>
                
                <telerik:GridTemplateColumn HeaderText="Description" ShowSortIcon="false" ItemStyle-Width="20%">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkRegionalApproverItem" OnClick="lnklRegionalApproverItem_Click"  CommandArgument='<%#Eval("INCFORM_APPROVER_LIST_ID") %>' Text='<%# Eval("Description") %>' runat="server" CssClass="buttonLink" Font-Bold="true" ForeColor="#000066" ToolTip='<%# Eval("Description") %>'></asp:LinkButton>
                        <%--<asp:Label ID="lblDescription" runat="server" Text='<%# Eval("") %>'></asp:Label>--%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Regional Approver Name" ShowSortIcon="true" SortExpression="Approver_SCOPE">
                    <ItemTemplate>
                        <asp:HiddenField ID="hfApproverItemID" runat="server" />
                        <asp:HiddenField ID="hfApproverSSO_ID" runat="server" />
                        <asp:HiddenField ID="hfApproverPerson_Id" runat="server" />
                        <asp:HiddenField ID="hfApproverType" runat="server" />
                         <asp:DropDownList ID="ddlRegionalApproverList" runat="server" Skin="Metro" ZIndex="9000" Width="300" CommandArgument='<%#Eval("INCFORM_APPROVER_LIST_ID") %>' AutoPostBack="true" OnSelectedIndexChanged="ddlRegionalApproverList_Click"></asp:DropDownList>
                        <%----%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <PagerStyle Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>
</asp:Panel>

<telerik:RadWindow runat="server" ID="winApproverEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="600" Title="Add Approver" Behaviors="Move,Close">
    <ContentTemplate>
        <asp:HiddenField ID="hfApproverActionID" runat="server" />
        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft editArea" style="margin: 8px 0 5px 0;">
                  <tr runat="server" id ="trApprover" visible="false">
                    <td class="columnHeader" width="24%">
                        <asp:Label ID="lblApprover" runat="server" Text="Approver"></asp:Label>
                    </td>
                    <td class="required" width="1%">&nbsp;</td>
                    <td class="tableDataAlt" width="75%">
                        <asp:DropDownList ID="ddlApproverType" runat="server" Skin="Metro" ZIndex="9000" Width="300">
                             <Items>
                                <asp:ListItem Text="select An Option From List" Value="0" runat="server" />
                                <asp:ListItem Text="Approver" Value="A" runat="server" />
                                <asp:ListItem Text="Regional Approver" Value="R" runat="server" /> 
                                 <asp:ListItem Text="Plant Approver Notifications" Value="N" runat="server" /> 
                            </Items>
                        </asp:DropDownList> 
                    </td>
                </tr>
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
               <%-- <tr>
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
              
               <%--   <tr>
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
                                <asp:ListItem Text="Approver Level 1" Value="390" runat="server" />
                                <asp:ListItem Text="Approver Level 2" Value="391" runat="server" />
                                <asp:ListItem Text="Approver Level 3" Value="392" runat="server" />
                                <asp:ListItem Text="Approver Level 4" Value="393" runat="server" />
                                <asp:ListItem Text="Approver Level 5" Value="394" runat="server" />
                                <asp:ListItem Text="Approver Level 6" Value="395" runat="server" />
                            </Items>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="ddlPriv" Display="None"
                            InitialValue="[Select One]" ErrorMessage="Select an option from list" ValidationGroup="ValApp"></asp:RequiredFieldValidator>
                    </td>
                </tr>--%>
            </table>
        </telerik:RadAjaxPanel>
        <span style="float: right; margin: 10px;">
            <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" Style="width: 70px;" OnClick="OnCancelApproverAction_Click"></asp:Button>
            <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" Style="width: 70px;" OnClientClick="return validateFunction();"
                ValidationGroup="ValApp" OnClick="OnSaveApproverAction_Click"></asp:Button>
            &nbsp;
				<asp:Button ID="btnDelete" class="buttonLink" runat="server" Text="<%$ Resources:LocalizedText, Delete %>" Style="width: 70px;" OnClientClick="return confirmAction('Delete This Approver Information');" OnClick="OnDeleteApproverAction_Click"></asp:Button>
        </span>
        <br />
        <center>
                <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
            </center>
    </ContentTemplate>
</telerik:RadWindow>
