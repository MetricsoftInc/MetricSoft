<%@ Page Title="" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Administrate_Company.aspx.cs" Inherits="SQM.Website.Administrate_Company"%>
<%@ Register src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_DocMgr.ascx" TagName="DocMgr" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_ItemHdr.ascx" TagName="ItemHdr" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_SearchBar.ascx" TagName="SearchBar" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_NotifyList.ascx" TagName="NotifyList" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_PrivGroupList.ascx" TagName="PrivGroupList" TagPrefix="Ucl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
<script type="text/javascript">

    function ValidTarget() {
        var status = confirmChange('Target Value');
        return status;
    }

</script>

	<asp:HiddenField ID="hfDocviewMessage" runat="server" Value="System Communications"/>
     <div class="admin_tabs">

        <table width="100%" border="0" cellspacing="0" cellpadding="1">
            <tr>
                <td class="tabActiveTableBg" colspan="10" align="center">
			        <BR/>
                    <FORM name="dummy">
                        <asp:HiddenField ID="hfBase" runat="server" />
                        <asp:HiddenField ID="hfOper" runat="server" />
                         <asp:HiddenField ID="hfActiveTab" runat="server" />

                        <table width="100%">
			                <tr>
                                <td class="pageTitles">
                                    <asp:Label ID="lblViewCompanyRezTitle" runat="server"  Text="Company Information" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblPageInstructions" runat="server" class="instructText"  Text="Manage company-wide settngs."></asp:Label>
                                    <asp:Label ID="lblViewBusOrgText" runat="server" Text="Return to Organization List" Visible="false"></asp:Label>
                                </td>
                                <td valign="top" align="center">
                                    <Ucl:SearchBar id="uclSearchBar" runat="server"/>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" align="center" colspan="2">
                                    <Ucl:ItemHdr id="uclItemHdr" runat="server"/>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <div id="divPageBody" runat="server">
                            <div id="divNavArea" runat="server"  class="navAreaLeft">
                                 <Ucl:AdminTabs id="uclAdminTabs" runat="server"/>
                            </div>
                            <div id="divWorkArea" runat="server" class="workAreaRight">
                                <table width="99%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td class="editArea">
                                            <Ucl:DocMgr id="uclDocMgr" runat="server"/>

                                            <asp:Panel ID="pnlDetails" runat="server" Visible = "false" CssClass="admBkgd">
                                                <asp:PlaceHolder ID="phUpdateCompany" runat="server">
                                                    <asp:Button ID="btnCancelCompany" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;"
                                                        OnClientClick="return confirmAction('Cancel without saving');"  onclick="CancelCompany"></asp:Button>
                                                    <asp:Button ID="btnSaveCompany" CSSclass="buttonEmphasis" runat="server" Text="<%$ Resources:LocalizedText, Save %>" style="margin-top: 8px; margin-bottom: 8px; margin-left: 5px;"
                                                        OnClientClick="return confirmChange('Company Settings');"  onclick="UpdateCompany"></asp:Button>
                                                </asp:PlaceHolder>
                                                <table width="100%" border="0" cellspacing="0" cellpadding="1">
                                                    <tr>
                                                        <td class="columnHeader">
                                                            <asp:Label ID="lblCompanyStatus" runat="server" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                                        </td>
                                                        <td class="tableDataAlt">
                                                            <asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList>
                                                        </td>
                                                    </TR>
                                                </table>
                                            </asp:Panel>

											<asp:Panel runat="server" ID="pnlEscalation" Visible="false">
												<Ucl:NotifyList id="uclNotifyList" runat="server"/>
											</asp:Panel>

                                            <asp:Panel ID="pnlUomStd" runat="server" Visible = "false">
		                                        <table width="99%" class="editArea">
		                                            <tr>
			                                            <td class="editArea">
				                                            <table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
					                                            <tr>
						                                            <td class="columnHeader" width="39%">
							                                            <asp:Label ID="lblStdEnergy" runat="server" text="Energy Unit of Measure "></asp:Label>
						                                            </td>
						                                            <td class="tableDataAlt" width="60%"><asp:DropDownList ID="ddlStdEnergy" runat="server" Enabled="false"></asp:DropDownList>
						                                            </td>
					                                            </tr>
					                                            <tr>
						                                            <td class="columnHeader">
							                                            <asp:Label ID="lblStdWieght" runat="server" text="Weight Unit of Measure"></asp:Label>
						                                            </td>
						                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlStdWeight" runat="server" Enabled="false"></asp:DropDownList>
						                                            </td>
					                                            </tr>
					                                            <tr>
						                                            <td class="columnHeader">
							                                            <asp:Label ID="lblStdVolume" runat="server" text="Volume Unit of Measure"></asp:Label>
						                                            </td>
						                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlStdVolume" runat="server" Enabled="false"></asp:DropDownList>
						                                            </td>
					                                            </tr>
					                                            <tr>
						                                            <td class="columnHeader">
							                                            <asp:Label ID="lblStdLqdVolume" runat="server" text="Liquid Volume Unit of Measure"></asp:Label>
						                                            </td>
						                                            <td class="tableDataAlt"><asp:DropDownList ID="ddlStdLqdVolume" runat="server" Enabled="false"></asp:DropDownList>
						                                            </td>
					                                            </tr>
                                                                 <tr>
						                                            <td class="columnHeader">
							                                            <asp:Label ID="lblStdCurrency" runat="server" text="Currency"></asp:Label>
						                                            </td>
						                                            <td class="tableDataAlt"><asp:Label ID="lblStdCurrency_out" runat="server" CssClass="textStd"></asp:Label>
						                                            </td>
					                                            </tr>
				                                            </table>
			                                            </td>
		                                            </tr>
	                                            </table>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlTargetList" runat="server" Visible = "false" CssClass="admBkgd" style="margin-left: 5px;">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="admBkgd">
                                                    <tr>
                                                        <td valign="top" align="center" width="58%">
                                                             <asp:UpdatePanel ID="udpList" runat="server" UpdateMode=Conditional>
                                                                <Triggers>
                                                                    <asp:AsyncPostBackTrigger ControlID="btnTargetSave" />
                                                                </Triggers>
                                                                <ContentTemplate>
                                                                    <div id="divNonconfGVScroll" runat="server" class="">
                                                                        <asp:GridView runat="server" ID="gvTargetList" Name="gvTargetList" CssClass="Grid" ClientIDMode="AutoID" AutoGenerateColumns="false" cellpadding="1" GridLines="Both" PageSize="20" AllowSorting="true" Width="100%" OnRowDataBound="gvList_OnTargetRowDataBound">
                                                                            <HeaderStyle CssClass="HeadingCellTextLeft" />
                                                                            <RowStyle CssClass="DataCell" />
                	                                                        <Columns>
                                                                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, Target %>" ItemStyle-Width="50%">
							                                                        <ItemTemplate>
								                                                        <asp:LinkButton ID="lnkTargetCD" runat="server" CommandArgument='<%#Eval("TARGET_ID") %>' CSSClass="linkUnderline"
										                                                    Text='<%#Eval("DESCR_SHORT") %>' OnClick="lnkTargetList_Click"></asp:LinkButton>
                                                                                        <asp:HiddenField id="hfCalcsScope" runat="server" Value='<%#Eval("CALCS_SCOPE") %>'/>
                                                                                    </ItemTemplate>
							                                                    </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="FY Year" ItemStyle-Width="25%">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblTargetYear" runat="server" Text='<%#Eval("EFF_YEAR") %>' ></asp:Label>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="<%$ Resources:LocalizedText, TargetValue %>" ItemStyle-Width="25%">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblTargetValue" runat="server" ></asp:Label>
                                                                                        <asp:HiddenField id="hfTargetValue" runat="server" Value='<%#Eval("TARGET_VALUE") %>'/>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                </Columns>
                                                                        </asp:GridView>
                                                                        <asp:Label runat="server" ID="lblTargetListEmpty" Text="There are currently no Targets defined." class="GridEmpty" Visible="false"></asp:Label>
                                                                    </div>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </td>
                                                        <td valign="top" width="42%">
                                                            <asp:Button ID="btnTargetNew" runat="server" Text="Add Target" ToolTip="" CSSclass="buttonStd" style="margin: 6px;" onclick="btnTargetAdd_Click"></asp:Button>
                                                            <asp:Panel id="pnlTargetEdit" runat="server" enabled="false" style="margin-left: 3px;">
                                                                <asp:UpdatePanel ID="udpTarget" runat="server" UpdateMode=Conditional>
                                                                    <Triggers>
                                                                        <asp:AsyncPostBackTrigger ControlID="btnTargetNew" />
                                                                        <asp:AsyncPostBackTrigger ControlID="btnTargetSave" />
                                                                        <asp:AsyncPostBackTrigger ControlID="btnTargetCancel" />
                                                                    </Triggers>
                                                                    <ContentTemplate>
                                                                        <table width="99%" align="center" border="0" cellspacing="1" cellpadding="1" class="lightBorder">
                                                                            <tr>
                                                                                <td class="columnHeader" width="34%">
                                                                                    <asp:Label ID="lblTargetName" runat="server" Text="<%$ Resources:LocalizedText, Target %>"></asp:Label>
                                                                                </td>
                                                                                <td class="required" width="1%">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt" width="65%">
                                                                                    <telerik:RadComboBox ID="ddlTarget" runat="server" Skin="Metro" Zindex="9000" width="200" Font-Size=Small AutoPostBack="false"></telerik:RadComboBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblTargetDescLong" runat="server" text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
                                                                                </td>
                                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <asp:TextBox ID="tbTargetDescLong"  runat="server" TextMode="multiline" rows="3" MaxLength="200" CssClass="textStd" style="width: 97%;"/>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblStatType" runat="server" text="Calculation"></asp:Label>
                                                                                </td>
                                                                                <td class="required">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <asp:DropDownList ID="ddlStatType" runat="server">
                                                                                        <asp:ListItem Value="sum">Total</asp:ListItem>
                                                                                        <asp:ListItem Value="pctChange" Text="<%$ Resources:LocalizedText, PercentChange %>" />
                                                                                        <asp:ListItem Value="deltaDy">Last Occurence</asp:ListItem>
										                                            </asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblEffYear" runat="server" text="For Year"></asp:Label>
                                                                                </td>
                                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <asp:DropDownList ID="ddlEffYear" runat="server"></asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblDateSpan" runat="server" text="Time Span"></asp:Label>
                                                                                </td>
                                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <telerik:RadAjaxPanel ID="RadAjaxPanel2" runat="server">
                                                                                        <telerik:RadButton ID="btnYTDMetric" runat="server" CssClass="prompt" Text="<%$ Resources:LocalizedText, YearToDate %>" OnClick="onTimeSpanClick" AutoPostBack=true ButtonType=ToggleButton ToggleType=Radio ToolTip="Metric calculated as year-to-date" CommandArgument="1"></telerik:RadButton>
                                                                                        <telerik:RadButton ID="btnYOYMetric" runat="server" CssClass="prompt" Text="Year Over Year" OnClick="onTimeSpanClick" AutoPostBack=true ButtonType=ToggleButton ToggleType=Radio ToolTip="Metric calculated as difference from previous year" CommandArgument="2"></telerik:RadButton>
                                                                                        <telerik:RadButton ID="btnABSMetric" runat="server" CssClass="prompt" Text="Last Occur" OnClick="onTimeSpanClick" AutoPostBack=true ButtonType=ToggleButton ToggleType=Radio ToolTip="Metric calculated from last occurence" CommandArgument="0"></telerik:RadButton>
                                                                                    </telerik:RadAjaxPanel>
                                                                                </td>
                                                                            </tr>
                                                      		                <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblTargetValue" runat="server" Text="<%$ Resources:LocalizedText, TargetValue %>"></asp:Label>
                                                                                </td>
                                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <asp:TextBox ID="tbTargetValue"  runat="server" MaxLength="15" Columns="15" onblur="ValidNumeric(this, 'please enter numeric values only');"/>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblTargetMinMax" runat="server" text="Target Is Min or Max"></asp:Label>
                                                                                </td>
                                                                                <td class="tableDataAlt">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
                                                                                    <telerik:RadButton ID="btnTargetMin" runat="server" CssClass="prompt" Text="Minimum" OnClick="onMinMaxClick" AutoPostBack=true ButtonType=ToggleButton ToggleType=Radio ToolTip="Target represents the minimum desired value for this metric" CommandArgument="0"></telerik:RadButton>
                                                                                    <telerik:RadButton ID="btnTargetMax" runat="server" CssClass="prompt" Text="Maximum" OnClick="onMinMaxClick" AutoPostBack=true ButtonType=ToggleButton ToggleType=Radio ToolTip="Target represents the maximim desired value for this metric" CommandArgument="1"></telerik:RadButton>
                                                                                    </telerik:RadAjaxPanel>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="columnHeader">
                                                                                    <asp:Label ID="lblTargetStatus" runat="server" text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
                                                                                </td>
                                                                                <td class="required">&nbsp;</td>
                                                                                <td CLASS="tableDataAlt">
                                                                                    <asp:DropDownList ID="ddlTargetStatus" runat="server"></asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>

                                                                        <asp:Button ID="btnTargetCancel" CSSclass="buttonStd" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>" style="margin: 5px;" onclick="btnTargetSave_Click" enabled="false" CommandArgument="cancel" ></asp:Button>
                                                                        <asp:Button ID="btnTargetSave" CSSclass="buttonEmphasis" runat="server" text="Save Target" style="margin: 5px;" Enabled="false" CommandArgument="save"
                                                                         OnClientClick="return ValidTarget();" onclick="btnTargetSave_Click" ></asp:Button>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </asp:Panel>
                                                            <asp:Label ID="lblAddTarget" runat="server" CssClass="instructText" Text="Enter Target details" style="display:none;"></asp:Label>

                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlPrivGroups" runat="server" Visible = "false">
                                                <Ucl:PrivGroupList ID="uclPrivGroupList" runat="server" />
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </form>
                </td>
            </tr>
        </table>
        <br>
    </div>

</asp:Content>



