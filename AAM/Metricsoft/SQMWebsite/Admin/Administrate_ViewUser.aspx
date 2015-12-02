<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Administrate_ViewUser.aspx.cs" Inherits="SQM.Website.Administrate_ViewUser" %>
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

        //if (status == true) {
        //    var ddl = $find('<%=ddlJobCode.ClientID %>');
        //    if (ddl.get_selectedItem().get_value() == "1" || ddl.get_selectedItem().get_value() == "100") {
       //         status = confirm('Please confirm you wish to grant Administrator Role to this user');
        //    }
        //}
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

</script>

	<asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
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
                                <asp:Label ID="lblViewUserTitle" runat="server" Text="Manage Users" Visible="False" ></asp:Label>
                                <asp:Label ID="lblViewUserText" runat="server" Text="Return to Organization List" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <div id="divPageBody" runat="server" style="margin-top: 5px;">
						<div style="float: left; margin-left: 8px;">
                            <asp:Label ID="lblFilterPlant" runat="server" CssClass="prompt" Text="HR Location: "></asp:Label>
                            <telerik:RadComboBox id="ddlPlantList" runat="server" Skin="Metro" ZIndex="9000" Width="280" AutoPostBack="false" EmptyMessage="HR Plant Location"></telerik:RadComboBox>
							<asp:Button id="btnSearchUsers" runat="server" OnClick="FilterUsers" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" style="margin-left: 20px;"/>
                        </div>
                        <asp:Button id="btnAddUser" runat="server" CssClass="buttonAddLarge" style="float: right; margin-right: 25px;" Text="Add User " ToolTip="Add A New User" OnClick="btnUserAdd_Click"/>
                        <br style="clear: both;" />
						<div style="float: left; margin: 4px 0px 4px 8px;">
							<asp:Label ID="lblFilterName" runat="server" Text="User Name: " CssClass="prompt"></asp:Label>
							&nbsp;
							<asp:TextBox ID="tbFilterName" runat="server" MaxLength="100" Width="280"></asp:TextBox>
							&nbsp;&nbsp;
							<span>
							<asp:Label ID="lblFilterStatus" runat="server" CssClass="prompt" Text="Status/Role: "></asp:Label>
								<telerik:RadComboBox id="ddlListStatus" runat="server" Skin="Metro" ZIndex="9000" Width="180" AutoPostBack="false" EmptyMessage="filter by status" >
									<Items>
										<telerik:RadComboBoxItem Text="" Value="" />
										<telerik:RadComboBoxItem Text="Any Status" Value="0" />
										<telerik:RadComboBoxItem Text="Active Only" Value="A" />
										<telerik:RadComboBoxItem Text="Inactive Only" Value="I" />
									</Items>
								</telerik:RadComboBox>
								<telerik:RadComboBox id="ddlRoleList" runat="server" Skin="Metro" ZIndex="9000" Width="180" AutoPostBack="false" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" EmptyMessage="filter by role" >
								</telerik:RadComboBox>
							</span>
                            &nbsp;&nbsp;
                            <asp:Label ID="lblUserCount" runat="server" CssClass="prompt" Text="Count = "></asp:Label>
                            <asp:Label ID="lblUserCount_out" runat="server" CssClass="textStd" ></asp:Label>
						</div>
						<br style="clear: both;" />
						<Ucl:AdminList id="uclUserList" runat="server"/>

                        <telerik:RadWindow runat="server" ID="winUserEdit" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="565" Width="650" Title="User Details" Behaviors="Move">
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
                                                                <asp:Label ID="lblUserMiddleName" runat="server" text="Middle Name"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserMiddleName" size="50" maxlength="100" runat="server"></asp:TextBox>
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
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:TextBox ID="tbUserEmail" size="50" maxlength="100" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblEmpID" runat="server" text="Employee ID / Supervisor ID" ></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
																<span>
																	<asp:TextBox ID="tbEmpID" size="16" maxlength="16" runat="server"></asp:TextBox>&nbsp;&nbsp;
																	<asp:TextBox ID="tbSupvEmpID" size="16" maxlength="16" runat="server"></asp:TextBox>
																</span>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserTitle" runat="server" text="Job Code"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
																<telerik:RadComboBox ID="ddlJobCode" runat="server" ZIndex="9000" Skin="Metro" width=300 Height="350" AutoPostBack="false" Font-Names="Verdana" EmptyMessage="select job code"></telerik:RadComboBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:LocalizedText, PrivilegeGroup %>"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
																<telerik:RadComboBox ID="ddlPrivGroup" runat="server" ZIndex="9000" Skin="Metro" width=300 Height="300" AutoPostBack="false" Font-Names="Verdana" EmptyMessage="select privilege group"></telerik:RadComboBox>
																<br />
																<asp:Label ID="lblPrivScope" runat="server" CssClass="refText"></asp:Label>
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
                                                                <asp:Label ID="lblPlantSelect" runat="server" Text="Accessible Locations"></asp:Label>
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
                                                                <asp:Label ID="lblUserTimezone" runat="server" text="Time Zone"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:DropDownList ID="ddlUserTimezone" runat="server" style="width: 98%;"></asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserLanguage" runat="server" text="Language/Culture"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableDataAlt">
                                                                <asp:DropDownList ID="ddlUserLanguage" runat="server" ToolTip="specify date and number formatting"></asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblUserStatus" runat="server" text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
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
                                                        <tr id="trResetPassword" runat="server" visible="false">
                                                            <td class="columnHeader">
                                                                <asp:Label ID="lblResetPassword" runat="server" text="Reset Password and Send Email"></asp:Label>
                                                            </td>
                                                            <td class="tableDataAlt">&nbsp;</td>
                                                            <td class="tableData"><asp:CheckBox runat="server" ID="cbResetPassword" /></td>
                                                        </tr>
                                                    </table>
                                                </telerik:RadAjaxPanel>
                                            </td>
                                        </tr>
                                    </table>
                                    <span style="float: right; margin: 10px;">
<%--                                        <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" OnClientClick ="return confirmAction('Cancel without saving');" onclick="OnCancel_Click"></asp:Button>--%>
                                        <asp:Button ID="btnCancel" class="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="width: 70px;" onclick="OnCancel_Click"></asp:Button>
                                        <asp:Button ID="btnSave" class="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="width: 70px;" OnClientClick="return ConfirmSaveUser('User');" onclick="OnSave_Click"></asp:Button>
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

