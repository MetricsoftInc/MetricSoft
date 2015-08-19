<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Contain.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Contain" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<script type="text/javascript">


    function OnEditorClientLoad(editor) {
        editor.attachEventHandler("ondblclick", function (e) {
            var sel = editor.getSelection().getParentElement(); //get the currently selected element
            var href = null;
            if (sel.tagName === "A") {
                href = sel.href; //get the href value of the selected link
                window.open(href, null, "height=500,width=500,status=no,toolbar=no,menubar=no,location=no");
                return false;
            }
        }
        );
    }


    function StandardConfirm(sender, args) {

        // Some pages will have no validators, so skip
        if (typeof Page_ClientValidate === "function") {
            var validated = Page_ClientValidate('Val_PowerOutage');

            if (!validated)
                alert("Please fill out all of the required fields.");
        }
    }


    $(function () {
        $('#tblCustomers').footable();
    });

</script>


<asp:Panel ID="pnlContain" Visible="false" runat="server">
    <div class="container-fluid">
        <telerik:RadAjaxPanel ID="rapContain"  runat="server">
            <asp:Repeater runat="server" ID="rptContain" ClientIDMode="AutoID" OnItemDataBound="rptContain_OnItemDataBound" OnItemCommand="rptContain_ItemCommand">
                <HeaderTemplate>
                        <table class="table" border="0">
                            <thead>
                                <tr>
                                    <th class="col-sm-1 text-center"><b>
                                        <asp:Label ID="lbhdItem" runat="server" Text="Item" /></b></th>
                                    <th class="col-sm-3 text-left-more"><b>
                                        <asp:Label ID="lbhdConAction" runat="server" Text="Initial Action" /></b></th>
                                    <th class="col-sm-2 text-left-more"><b>
                                        <asp:Label ID="lbhdConAssignedTo" runat="server" Text="Assigned To" /></b></th>
                                    <th class="col-sm-2 text-left-more"><b>
                                        <asp:Label ID="lbhdConStartDate" runat="server" Text="Date Performed" /></b></th>
                                    <%--<th class="col-sm-2 text-left-more"><b>
                                        <asp:Label ID="lbhdConCmpltDT" runat="server" Text="Completion Date" /></b></th>
                                    <th class="col-sm-1 text-left-more" style="margin-left: -5px;"><b>
                                        <asp:Label ID="lbhdConComplete" runat="server" Text="Cmpltd" /></b></th>--%>
                                </tr>
                            </thead>
                </HeaderTemplate>
                <ItemTemplate>
                        <tbody>
                            <tr>
                                <td class="text-center">
                                    <asp:Label ID="lbItemSeq" runat="server" />
                                </td>
                                <td class="text-left-more">
                                    <asp:TextBox ID="tbContainAction" Rows="3" Height="65px" Width="90%" TextMode="MultiLine" SkinID="Metro" runat="server" />
                                    <asp:RequiredFieldValidator runat="server" ID="rfvContainAction" ControlToValidate="tbContainAction" Display="None" ErrorMessage="Required" />
                                </td>
                                <td class="text-left-more">
                                    <telerik:RadDropDownList ID="rddlContainPerson" Skin="Metro" Width="70%" CssClass="WarnIfChanged" runat="server" OnSelectedIndexChanged="rddlContainPerson_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator runat="server" ID="rfvContainPerson" ControlToValidate="rddlContainPerson" Display="None" InitialValue="[Select One]" ErrorMessage="Required" />
                                </td>
                                <td class="text-left-more">
                                    <telerik:RadDatePicker ID="rdpStartDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true" runat="server"  ShowPopupOnFocus="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="rvfStartDate" ControlToValidate="rdpStartDate" Display="None" ErrorMessage="Required" />
                                </td>
    <%--							<td class="text-left-more">
                                    <telerik:RadDatePicker ID="rdpCompleteDate" Skin="Metro" CssClass="WarnIfChanged" Enabled="true" runat="server" />
                                </td>
                                <td class="text-left">
                                    <asp:CheckBox ID="cbIsComplete" runat="server" SkinID="Metro" />
                                </td>--%>
                                <td class="text-left-more">
                                    <telerik:RadButton ID="btnItemDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" ForeColor="DarkRed"  CommandArgument="Delete" 
                                        Text="Delete Item" SingleClick="true" SingleClickText="Deleting..." OnClientClicking="DeleteConfirmItem" />
                                </td>
                            </tr>
                    </tbody>
                </ItemTemplate>
                <FooterTemplate>
                    </table> 
                    <div class="row" style="padding-top:0;margin-top:-10px;">
                        <div class="col-xs-12 text-left-more">
                            <asp:Button ID="btnAddContain" CssClass="buttonAdd" runat="server" ToolTip="Add Another Initial Corrective Action" Text="Add Another" Style="margin: 7px;" CommandArgument="AddAnother"></asp:Button>
                        </div>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        	&nbsp;
        </telerik:RadAjaxPanel>
    </div>
</asp:Panel>
