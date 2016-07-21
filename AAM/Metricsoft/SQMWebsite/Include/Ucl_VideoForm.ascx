<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_VideoForm.ascx.cs" Inherits="SQM.Website.Ucl_VideoForm" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Include/Ucl_RadAsyncUploadVideo.ascx" TagName="UploadAttachment" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadScriptBlock.ascx" TagName="RadScript" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AttachVideoText.ascx" TagName="UploadText" TagPrefix="Ucl" %>

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

    function toggle_visibility(id) {
    	var e = document.getElementById(id);
    	if (e.style.display == 'block')
    		e.style.display = 'none';
    	else
    		e.style.display = 'block';
    }
</script>

<div id="divVideoForm" runat="server" visible="false">
    <div style="width: 100%; text-align: center; margin-bottom: 10px;"><a href="~/Media/Media_Videos.aspx" id="ahReturn" runat="server">
        <img src="/images/defaulticon/16x16/arrow-7-up.png" style="vertical-align: middle; border: 0;" border="0" alt="" /><asp:Literal runat="server" Text="<%$ Resources:LocalizedText, ReturnToList %>"></asp:Literal></a></div>
    <table style="width: 100%" class="textStd">
        <tr>
            <td>
                <div id="divPageBody" class="textStd" style="text-align: left; margin: 0 0px;" runat="server">
                    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
                        <asp:Label ID="lblResults" runat="server" />
						<div class="container-fluid blueCell" style="padding: 7px;"">

                            <asp:Panel ID="pnlVideoHeader" runat="server">
                                <div class="blueCell" style="padding: 7px;">

                                    <div class="row-fluid">

                                        <div class="col-xs-12  text-left">

                                            <asp:Label ID="lblAddOrEditVideo" class="textStd" runat="server" Text="<%$ Resources:LocalizedText, VideoEdit %>:"><strong></strong></asp:Label>&nbsp;&nbsp;&nbsp;<asp:Literal runat="server" ID="litVideoLink"></asp:Literal>

                                        </div>
                                    </div>
                                    <table width="100%" align="center" border="0" cellspacing="0" cellpadding="5" class="borderSoft">
                                        <tr runat="server">
                                            <td class="columnHeader">
                                                <asp:Label ID="lblSourceType" runat="server" Text="<%$ Resources:LocalizedText, VideoSourceType %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:Label runat="server" ID="lblVideoSourceType"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr runat="server">
                                            <td class="columnHeader">
                                                <asp:Label ID="lblType" runat="server" Text="<%$ Resources:LocalizedText, VideoType %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:Label runat="server" ID="lblVideoType"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr runat="server">
                                            <td class="columnHeader">
                                                <asp:Label ID="lblLocation" runat="server" Text="<%$ Resources:LocalizedText, BusinessLocation %>"></asp:Label>
                                            </td>
                                            <td class="required" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt">
                                                <asp:Label runat="server" ID="lblVideoLocation"></asp:Label>
                                                <asp:HiddenField runat="server" ID="hdnVideoLocation" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblVideoPerson" runat="server" Text="<%$ Resources:LocalizedText, VideoOwner %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblVideoPersonName"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblDate" runat="server" Text="<%$ Resources:LocalizedText, VideoDate %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblVideoDate"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblIncidentDate" runat="server" Text="<%$ Resources:LocalizedText, IncidentDate %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblVideoIncidentDate"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblInjuryType" runat="server" Text="<%$ Resources:LocalizedText, InjuryType %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblVideoInjuryType"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="columnHeader" width="24%">
                                                <asp:Label ID="lblBodyPart" runat="server" Text="<%$ Resources:LocalizedText, BodyPart %>"></asp:Label>
                                            </td>
                                            <td class="tableDataAlt" width="1%">&nbsp;</td>
                                            <td class="tableDataAlt" width="75%">
                                                <asp:Label runat="server" ID="lblVideoBodyPart"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:Panel>
                        </div>
                        <br />
                        <asp:Panel ID="pnlAddEdit" runat="server">

                            <div id="divForm" runat="server">
                                <table style="width: 100%; vertical-align: top;">
									<tr>
										<td class="columnHeader" width="24%">
											<asp:Label runat="server" ID="lblStatus" CssClass="prompt" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
										</td>
										<td class="tableDataAlt">&nbsp;</td>
										<td class="tableDataAlt" width="75%">
                                            <telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="<%$ Resources:LocalizedText, VideoSelectStatus %>" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false">
											</telerik:RadComboBox>
										</td>
									</tr>
									<tr>
										<td class="columnHeader" width="24%">
											<asp:Label ID="lblTitle" runat="server" Text="<%$ Resources:LocalizedText, Title %>"></asp:Label>
										</td>
										<td class="tableDataAlt">&nbsp;</td>
										<td class="tableDataAlt" width="75%">
											<asp:TextBox ID="tbTitle" MaxLength="100" runat="server"></asp:TextBox>
											<asp:RequiredFieldValidator runat="server" ID="rfvTitle" ControlToValidate="tbTitle" Display="Dynamic" ErrorMessage="<%$ Resources:LocalizedText, TitleRequired %>" ></asp:RequiredFieldValidator>
										</td>
									</tr>
									<tr>
										<td class="columnHeader" width="24%" style="vertical-align: top;">
											<asp:Label ID="lblDescription" runat="server" Text="<%$ Resources:LocalizedText, Description %>"></asp:Label>
										</td>
										<td class="tableDataAlt">&nbsp;</td>
										<td class="tableDataAlt" width="75%">
											<asp:TextBox ID="tbDescription" TextMode="MultiLine" Rows="2" Columns="50" MaxLength="250" runat="server"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td class="columnHeader" width="24%">
											<asp:Label ID="lblAvailablity" runat="server" Text="<%$ Resources:LocalizedText, Availability %>"></asp:Label>
										</td>
										<td class="tableDataAlt">&nbsp;</td>
										<td class="tableDataAlt" width="75%">
											<asp:DropDownList runat="server" ID="ddlAvailability">
												<asp:ListItem Text="Global" Value="g"></asp:ListItem>
												<asp:ListItem Text="Local Only" Value="l"></asp:ListItem>
											</asp:DropDownList>
										</td>
									</tr>
									<tr>
										<td class="columnHeader" width="24%" style="vertical-align: top;">
											<asp:Label ID="lblReleaseForms" runat="server" Text="<%$ Resources:LocalizedText, VideoReleaseFormsRequired %>"></asp:Label>
										</td>
										<td class="tableDataAlt">&nbsp;</td>
										<td class="tableDataAlt" width="75%">
											<asp:CheckBox Text="<%$ Resources:LocalizedText, VideoReleaseFormsDesc %>" TextAlign="right" runat="server" ID="cbReleaseForms" OnClick="javascript:toggle_visibility('dvAttach');" />
											<br />
											<div id="dvAttach" runat="server" class="col-xs-12 col-sm-8 text-left greyControlCol">
												<span style="border: 0 none !important;">
													<Ucl:UploadAttachment runat="server" ID="uploadReleases" />
												</span>
											</div>
										</td>
									</tr>
									<tr>
										<td class="columnHeader" width="24%" style="vertical-align: top;">
											<asp:Label ID="lblTextAttachments" runat="server" Text="<%$ Resources:LocalizedText, VideoTextAdded %>"></asp:Label>
										</td>
										<td class="tableDataAlt">&nbsp;</td>
										<td class="tableDataAlt" width="75%">
											<asp:CheckBox Text="" TextAlign="right" runat="server" ID="cbVideoText" OnClick="javascript:toggle_visibility('dvText');" />
											<br />
											<div id="dvText" runat="server" class="col-xs-12 col-sm-8 text-left greyControlCol">
												<span style="border: 0 none !important;">
													<asp:Literal runat="server" Text="<%$ Resources:LocalizedText, VideoTextDisclaimer %>"></asp:Literal>
													<br /><br />
													<Ucl:UploadText runat="server" ID="uploadText"></Ucl:UploadText>
												</span>
											</div>
										</td>
									</tr>
									<tr style="padding-top: 10px;">
                                        <td style="width: 33%;">
                                            <telerik:RadButton ID="btnSaveReturn" runat="server" Text="<%$ Resources:LocalizedText, SaveAndReturn %>" Visible="false"
                                                CssClass="UseSubmitAction" Width="88%" Skin="Metro" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Saving %>"
                                                OnClick="btnSaveReturn_Click" OnClientClicking="StandardConfirm" ValidationGroup="Val" />
                                        </td>
										<td>&nbsp;</td>
                                        <td style="width: 33%; text-align: center;">
                                            <telerik:RadButton ID="btnDelete" runat="server" ButtonType="LinkButton" BorderStyle="None" Visible="false" ForeColor="DarkRed"
                                                Text="<%$ Resources:LocalizedText, Delete %>" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Deleting %>"
                                                OnClick="btnDelete_Click" OnClientClicking="DeleteConfirm" CssClass="UseSubmitAction" ToolTip="<%$ Resources:LocalizedText, VideoDelete %>" />
                                        </td>
                                    </tr>
                                </table>

                            </div>
                        </asp:Panel>

                    </telerik:RadAjaxPanel>
                    <br />
                    <br />
                </div>
            </td>
        </tr>
    </table>
</div>

<div id="divAuditReportForm" runat="server" visible="false">

</div>
