﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Administrate_ViewUser.aspx.cs" Inherits="SQM.Website.Administrate_ViewUser" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminList.ascx" TagName="AdminList" TagPrefix="Ucl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<script type="text/javascript">

    window.onload = function () {
        //filterDependentList('ddlUserBusOrg', 'ddlUserPlant', 'hfUOMCat_out', 'hfMeasureUOM_out');
    }

    function OpenUserEditWindow() {
        $find("<%=winUserEdit.ClientID %>").show();
    }

    function CloseUSerEditWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window 
        oWindow.Close();
    }

    function ConfirmSaveUser(confirmText) {
        var status = confirmChange(confirmText);
  
        if (status == true) {
            var ddl = $find('<%=ddlUserRole.ClientID %>');
            if (ddl.get_selectedItem().get_value() == "1" || ddl.get_selectedItem().get_value() == "100") {
                status = confirm('Please confirm you wish to grant Administrator Role to this user');
            }
        }
        return status;
    }

    function PlantCheckedItems() {
        var str = '';
        var combo = $find("<%= ddlPlantSelect.ClientID %>");
        var items = combo.get_items();
        var count = items.get_count();
        var checkedCount = 0;
        for (var i = 0; i < count ; i++) {
            var item = items.getItem(i);
            if (item.get_checked()) {
                str += item.get_text();
                ++checkedCount;
            }
        }
        if (checkedCount > 3)
            document.getElementById('lblPlantAccess').innerHTML = str;
        else
            document.getElementById('lblPlantAccess').innerHTML = '';
    }

    function ModuleCheckedItems() {
        var str = '';
        var combo = $find("<%= ddlModuleAccess.ClientID %>");
        var items = combo.get_items();
        var count = items.get_count();
        var checkedCount = 0;
        for (var i = 0; i < count ; i++) {
            var item = items.getItem(i);
            if (item.get_checked()) {
                str += item.get_text();
                ++checkedCount;
            }
        }
        if (checkedCount > 3)
            document.getElementById('lblModuleAccess').innerHTML = str;
        else 
            document.getElementById('lblModuleAccess').innerHTML = '';
    }

</script>
   
     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <br />
                    <form name="dummy">
                    <asp:HiddenField ID="hfBase" runat="server" />
                    <asp:HiddenField ID="hfSelectedRole" runat="server" />
                    <asp:HiddenField ID = "hfUserBusOrg_out" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID = "hfUserPlant_out" runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hfAddUser" runat ="server" Value="Add USer" />
                    <asp:HiddenField ID="hfUpdateUser" runat ="server" Value="Update User" />
                    <asp:Panel runat="server" ID="pnlSearchBar">
                        <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                    </asp:Panel>
                    
                    <table width="99%">
			            <tr>
                            <td align="left">
                                <asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Manage user contact information, application preferences and access privleges. Search for users by first/last name or user ID."></asp:Label>
                                <asp:Label ID="lblViewUserTitle" runat="server" Text="Manage Users For:" Visible="False" ></asp:Label>
                                <asp:Label ID="lblViewUserText" runat="server" Text="Return to Organization List" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <div id="divPageBody" runat="server" style="margin-top: 5px;">
                        <span style="float: left; margin: 8px;">
                            <asp:Label ID="lblUserList1" runat="server" CssClass="prompt" Text="Filter User List: "></asp:Label>
                            <telerik:RadComboBox id="ddlListStatus" runat="server" Skin="Metro" ZIndex="9000" AutoPostBack="true" EmptyMessage="by status or role" OnSelectedIndexChanged="UserStatusSelect">
                                <Items>
                                    <telerik:RadComboBoxItem Text="" Value="" />
                                    <telerik:RadComboBoxItem Text="Any Status" Value="0" />
                                    <telerik:RadComboBoxItem Text="Active Only" Value="A" />
                                    <telerik:RadComboBoxItem Text="Inactive Only" Value="I" />
                                    <telerik:RadComboBoxItem Text="Plant Admin Role" Value="PA" />
                                    <telerik:RadComboBoxItem Text="Company Admin Role" Value="CA" />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;
                            <telerik:RadComboBox id="ddlListModule" runat="server" Skin="Metro" ZIndex="9000" AutoPostBack="true" EmptyMessage="by module access" OnSelectedIndexChanged="UserStatusSelect">
                                <Items>
                                    <telerik:RadComboBoxItem Text="" Value="" />
                                    <telerik:RadComboBoxItem Text="Any Modules" Value="0" />
                                    <telerik:RadComboBoxItem Text="EH & S Modules" Value="EHS" />
                                    <telerik:RadComboBoxItem Text="Quality Modules" Value="SQM" />
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Label ID="lblUserCount" runat="server" CssClass="prompt" Text="Count = "></asp:Label>
                            <asp:Label ID="lblUserCount_out" runat="server" CssClass="textStd" ></asp:Label>
                        </span>
                        <asp:Button id="btnAddUser" runat="server" CssClass="buttonAddLarge" style="float: right; margin-right: 25px;" Text="Add User " ToolTip="Add A New User" OnClick="btnUserAdd_Click"/>
                        <br style="clear: both;" />
                        <Ucl:AdminList id="uclUserList" runat="server"/>
                        
                        <telerik:RadWindow runat="server" ID="winUserEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="625" Width="650" Title="User Details" Behaviors="Move">
                            <ContentTemplate>
                                <asp:Panel runat="server" ID="pnlUserEdit" >
                                     <table width="99%" border="0" cellspacing="0" cellpadding="0" style="margin-top: 4px;">
                                        <tr>
                                             <td class="editArea">
                                                <asp:Label id="lblErrRequiredInputs" runat="server" Visible="false" Text="Please enter all required (*) fields before saving."/>
											    <asp:Label runat="server" ID="lblDuplicateSSOId" Visible="false" CssClass="promptAlert" Text="The User ID already exists.  Please enter a unique ID."></asp:Label>
											    <asp:Label runat="server" ID="lblDuplicateEmail" Visible="false" CssClass="promptAlert" Text="The Email already exists.  Please enter a unique email." ></asp:Label>
											    <asp:Label runat="server" ID="lblPasswordEmailSubject" Text="New User" Visible="false"></asp:Label>
											    <asp:Label runat="server" ID="lblPasswordEmailBody1a" Visible="false" Text="A new user account has been created for you in the Metricsoft application. You must login using this random password, and then you will be prompted to change the password."></asp:Label>
											    <asp:Label runat="server" ID="lblPasswordEmailBody1b" Visible="false" Text="<br><br>Your Username: "></asp:Label>
											    <asp:Label runat="server" ID="lblPasswordEmailBody2" Text="<br>Your temporary password: " Visible="false" ></asp:Label>
											    <asp:Label runat="server" ID="lblPasswordEmailBody2b" Text="<br><br>Access the website by clicking on the link: " Visible="false"></asp:Label>
											    <asp:Label runat="server" ID="lblPasswordEmailBody3" Text="Please do not reply to this email.<br><br>Thank you." Visible="false" ></asp:Label>
											    <asp:Label runat="server" ID="lblResetEmailSubject" Text="Password Reset" Visible="false"></asp:Label>
                                                <asp:Label runat="server" ID="lblUserRoleEmailSubjecta" Visible="false" Text="Your "></asp:Label>
											    <asp:Label runat="server" ID="lblUserRoleEmailSubjectb" Visible="false" Text=" User Role Has Changed"></asp:Label>
                                                <asp:Label runat="server" ID="lblUserRoleEmailBodya" Visible="false" Text="Your access or privilege to the "></asp:Label>
                                                <asp:Label runat="server" ID="lblUserRoleEmailBodyb" Visible="false" Text=" system has changed."></asp:Label>
											    <asp:Label runat="server" ID="lblUserRoleEmailBodyc" Visible="false" Text="Please do not reply to this email."></asp:Label>

                                                <asp:HiddenField id="hfCQM" runat="server" Value="Core"/>
                                                <asp:HiddenField id="hfSQM" runat="server" Value="Quality"/>
                                                <asp:HiddenField id="hfEHS" runat="server" Value="EHS"/>

                                                <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1">
                                                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="2" class="borderSoft" style="margin: 4px 0 4px 0; ">
                                                        <tr>
                                                            <td class="columnHeader" width="24%">
                                                                <asp:Label ID="lblUserSSOID" runat="server" text="User ID"></asp:Label>
                                                            </td>
                                                            <td class="required" width="1%">&nbsp;</td>
                                                            <td class="tableDataAlt" width="75%">
                                                                <asp:TextBox ID="tbUserSSOID" size="50" maxlength="100" runat="server" ></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserFirstName" runat="server" text="First Name"></asp:Label>
                                                            </td>
                                                            <td class="required">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserFirstName" size="50" maxlength="100" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserLastName" runat="server" text="Last Name"></asp:Label>
                                                            </td>
                                                            <td class="required">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserLastName" size="50" maxlength="100" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserTitle" runat="server" text="Job Title"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserTitle" size="50" maxlength="60" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserPhone" runat="server" text="Phone"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserPhone" size="50" maxlength="40" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserEmail" runat="server" text="Email" ></asp:Label>
                                                            </td>
                                                            <td class="required">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserEmail" size="50" maxlength="100" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserHRLocation" runat="server" text="HR Business Location"></asp:Label>
                                                            </td>
                                                             <td class="required">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <telerik:RadComboBox ID="ddlHRLocation" runat="server" Height="300" Style="width: 98%;" ZIndex="9000" Skin="Metro" AutoPostBack="true" OnSelectedIndexChanged="ddlLocationChange" Font-Names="Verdana"></telerik:RadComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblPlantSelect" runat="server" Text="Accessible Internal Locations"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                             <td class="tableDataAlt">
                                                                <telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" OnClientLoad="DisableComboSeparators" OnClientDropDownClosed="PlantCheckedItems" Height="300" Style="width: 98%;" ZIndex="9000" Skin="Metro" Font-Names="Verdana"
                                                                   ToolTip="Specify additional internal locations that the user may enter data or view information" ></telerik:RadComboBox>
                                                                <br />
                                                                <asp:Label ID="lblPlantAccess" runat="server" CssClass="refText"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblCustLocation" runat="server" Text="Alternate Partner Working Location"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                             <td class="tableDataAlt">
                                                                <telerik:RadComboBox ID="ddlCustPlantSelect" runat="server" CheckBoxes="true" EnableCheckAllItemsCheckBox="false" OnClientLoad="DisableComboSeparators" Height="300" Style="width: 98%;" ZIndex="9000" Skin="Metro" Font-Names="Verdana" 
                                                                   ToolTip="For Quality System users, you may specifiy a customer location where this user may work and report quality reports">
                                                                </telerik:RadComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserTimezone" runat="server" text="Time Zone"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:DropDownList ID="ddlUserTimezone" runat="server" style="width: 98%;"></asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserLanguage" runat="server" text="Date/Number Culture"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:DropDownList ID="ddlUserLanguage" runat="server" ToolTip="specify date and number formatting"></asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserStatus" runat="server" text="Status"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlUserStatus" runat="server"></asp:DropDownList></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserLoginDate" runat="server" text="Last Login"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableData"><asp:Label ID="lblUserLoginDate_out" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserUpdateDate" runat="server" text="Last Update By"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableData"><asp:Label ID="lblUserLastUpdate" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblResetPassword" runat="server" text="Reset Password and Send Email"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableData"><asp:CheckBox runat="server" ID="cbResetPassword" /></td>
                                                        </tr>
                                                    </table>
                                                </telerik:RadAjaxPanel>

                                                <asp:Label ID="lblAccessInstruction" runat="server" Text="<b>User Privileges:</b> specify the user's system role and modules that may be accessed." CssClass="instructText"></asp:Label>
                                                <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft" style="margin-top: 4px;">
                                                    <tr>
                                                        <td class="columnHeader" width="24%">
                                                            <asp:Label ID="lblUserRole" runat="server" text="User Role"></asp:Label>
                                                        </td>
                                                       <td class="required" width="1%">&nbsp;</td>
                                                        <td class="tableDataAlt" width="75%">
                                                            <telerik:RadComboBox ID="ddlUserRole" runat="server" ZIndex="9000" Skin="Metro" width=200 AutoPostBack="false" Font-Names="Verdana"></telerik:RadComboBox>
                                                            &nbsp;
                                                            <asp:CheckBox id="cbUserRcvEscalation" runat="server" Visible="false" CssClass="textStd" Text=" May Receive Escalation Notices"/>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblUserAccess" runat="server" text="Module Access"></asp:Label>
                                                        </td>
                                                       <td class="tableDataAlt">&nbsp;</td>
                                                        <td class="tableDataAlt">
                                                            <telerik:RadComboBox ID="ddlModuleAccess" runat="server" ZIndex="9000" height="120" Style="width: 98%;" Skin="Metro" DropDownCssClass="multipleRowsColumnsNarrow" DropDownWidth="450px" CheckBoxes="true" EnableCheckAllItemsCheckBox="false" AutoPostBack="false" Font-Names="Verdana" OnClientLoad="DisableComboSeparators" OnClientDropDownClosed="ModuleCheckedItems"></telerik:RadComboBox>
                                                            <br />
                                                            <asp:Label ID="lblModuleAccess" runat="server" CssClass="refText"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                    <span style="float: right; margin: 10px;">
<%--                                        <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="Cancel" style="width: 70px;" OnClientClick ="return confirmAction('Cancel without saving');" onclick="OnCancel_Click"></asp:Button>--%>
                                        <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="Cancel" style="width: 70px;" onclick="OnCancel_Click"></asp:Button>
                                        <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text ="Save" style="width: 70px;" OnClientClick="return ConfirmSaveUser('User');" onclick="OnSave_Click"></asp:Button>
                                    </span>
                                    <br />
                                    <center>
                                        <asp:Label ID="lblErrorMessage" runat="server" CssClass="labelEmphasis"></asp:Label>
                                    </center>
                                </asp:Panel>
                            </ContentTemplate>
                        </telerik:RadWindow>

                    </div>
                    </form>
                </td>
            </tr>
        </table>

        <br />
    </div>

</asp:Content>

