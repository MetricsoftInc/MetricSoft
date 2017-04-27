<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_INCFORM_Approval.ascx.cs" Inherits="SQM.Website.Ucl_INCFORM_Approval" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<script type="text/javascript">

	window.onload = function () {
	    document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "";

	}
	window.onbeforeunload = function () {
		if (document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value == '1') {
			return 'You have unsaved changes on this page.';
		}
	}
	function ChangeUpdate(sender, args) {
		document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "1";
		return true;
	}
    function ChangeClear(sender, args) {
        document.getElementById(('<%=hfChangeUpdate.ClientID%>')).value = "0";
	}
   
    //only one checkbox checked at a time.
    $(document).ready(function () {
        $('#chkDiv').each(function () {
            $(this).find('.cb').each(function () {
                $(".cb input:checkbox").on('change', function () {
                    $(".cb input:checkbox").not(this).prop('checked', false);
                });
            });
            $(".aspNetDisabled").addClass("custom-label");
        });
        if ($('#ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclapproval_rptApprovals_ctl09_cbIsAccepted').is(':checked')) {
            $(".cb input:checkbox").removeAttr('disabled');
        }
        else {
            $(".cb input:checkbox").attr('disabled', true);
        }
    });
   
    
    $(document.body).on('change', '#ctl00_ContentPlaceHolder_Body_uclIncidentForm_uclapproval_rptApprovals_ctl09_cbIsAccepted', function () {
        if ($(this).is(':checked')) {
            $(".cb input:checkbox").removeAttr('disabled');
        }
        else {
            $(".cb input:checkbox").prop('checked', false);
            $(".cb input:checkbox").attr('disabled', true);
        }
    })
    
</script>

<asp:Panel ID="pnlApproval" Visible="False" runat="server" meta:resourcekey="pnlApprovalResource1">
	<asp:HiddenField id="hfChangeUpdate" runat="server" Value=""/>

	<div id="divTitle" runat="server" visible="false" class="container" style="margin: 10px 0 10px 0;">
		<div class="row text_center">
			<div class="col-xs-12 col-sm-12 text-center">
				<asp:Label ID="lblFormTitle" runat="server" Font-Bold="True" CssClass="pageTitles"></asp:Label>
			</div>
		</div>
	</div>

	<div class="container-fluid">

		<div id="divStatus" runat="server" visible="false" style="margin: 10px;">
			<asp:Label ID="lblStatusMsg" runat="server" CssClass="labelEmphasis"></asp:Label>
		</div>
		<telerik:RadAjaxPanel ID="rapApprovals" runat="server" HorizontalAlign="NotSet" meta:resourcekey="rapApprovalsResource1">

		<asp:Repeater runat="server" ID="rptApprovals" ClientIDMode="AutoID" OnItemDataBound="rptApprovals_OnItemDataBound" OnItemCommand="rptApprovals_ItemCommand">
			<FooterTemplate>
			</FooterTemplate>
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
				<div class="row">
					<div class="col-xs-12 col-sm-2 text-left">
						<asp:HiddenField ID="hfApprovalID" runat="server" />
						<asp:HiddenField ID="hfItemSeq" runat="server" />
						<asp:HiddenField ID="hfPersonID" runat="server" />
						<asp:HiddenField ID="hfReqdComplete" runat="server" />
						<asp:HiddenField ID="hfRoleDesc" runat="server" />
						<span>
							<asp:PlaceHolder ID="phOnBehalfOf" runat="server" Visible="false">
								<asp:Label runat="server" ID="lblOnBehalfOf" CssClass="refText" Text="<%$ Resources:LocalizedText, OnBehalfOf %>"></asp:Label>
								<br />
							</asp:PlaceHolder>
							<b>
							<asp:Label ID="lbApproverJob" runat="server" meta:resourcekey="lbApproverJobResource1" SkinID="Metro"></asp:Label>
							<asp:Label ID="lbItemSeq" runat="server" meta:resourcekey="lbItemSeqResource1"></asp:Label>
							</b>
							<br />
							<asp:Label ID="lbApprover" runat="server" meta:resourcekey="lbApproverResource1" SkinID="Metro" Width="75%"></asp:Label>
						</span>
					</div>
					<div class="col-xs-12 col-sm-3  text-left">
						<asp:Label ID="lbApproveMessage" runat="server" Height="95px" meta:resourcekey="lbApproveMessageResource1" SkinID="Metro" Width="95%"></asp:Label>
					</div>
					<div class="col-xs-12  col-sm-1 text-left">
						<span>
						<asp:CheckBox ID="cbIsAccepted" CssClass="SGroup"
                             runat="server" Font-Bold="False" 
                            meta:resourcekey="cbIsAcceptedResource1" SkinID="Metro"
                             onChange="return ChangeUpdate();" />
						</span>
					</div>
					<div class="col-xs-12 col-sm-2 text-left">
						<span>Date&nbsp;
						<telerik:RadDatePicker ID="rdpAcceptDate" runat="server" CssClass="WarnIfChanged" Enabled="False" meta:resourcekey="rdpAcceptDateResource1" ShowPopupOnFocus="True" Skin="Metro" Width="120px">
							<Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
							</Calendar>
							<DateInput DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="64px" Width="">
								<EmptyMessageStyle Resize="None" />
								<ReadOnlyStyle Resize="None" />
								<FocusedStyle Resize="None" />
								<DisabledStyle Resize="None" />
								<InvalidStyle Resize="None" />
								<HoveredStyle Resize="None" />
								<EnabledStyle Resize="None" />
							</DateInput>
							<DatePopupButton CssClass="" HoverImageUrl="" ImageUrl="" />
						</telerik:RadDatePicker>
						</span>
					</div>
				</div>
			</ItemTemplate>
			<SeparatorTemplate>
				<br />
			</SeparatorTemplate>
		</asp:Repeater>

		</telerik:RadAjaxPanel>

        <asp:Panel ID="pnlSeverityDescription" Visible="False" runat="server" meta:resourcekey="pnlApprovalSeverityDescription">
        <asp:Label runat="server" ID="lblSeverityLevel" SkinID="Metro" Font-Bold="true" Text="Severity Level" ></asp:Label>
        <asp:Label runat="server" ID="lblSeverityLevelDescription" SkinID="Metro" style="margin-left:120px;" Font-Bold="false"></asp:Label>
        <br /><br /> <br />
        </asp:Panel>
       
        <asp:Panel ID="pnlSeverity" Visible="False" runat="server" meta:resourcekey="pnlApprovalSeverity">
         <div class="row" id="severityDiv" >
                    <div class="col-xs-12 col-sm-12 text-left">
                        <asp:Label ID="lbApproverSeverityLevel" runat="server" meta:resourcekey="lbApproverSeverityLevelResource1" SkinID="Metro" Font-Bold="true" Text="Severity Level"></asp:Label>
                        <br />
                        </div>
                    <div class="col-xs-12 col-sm-12 text-left" id="chkDiv">                     
                        <asp:CheckBox runat="server" ID="chkSeverityLevel00" class="cb custom-label" Text="<%$ Resources:LocalizedText, FirstAid %>" /><br />
                        <asp:CheckBox runat="server" ID="chkSeverityLevel01" class="cb custom-label" Text="<%$ Resources:LocalizedText, L1_Minor %> "/><br />
                        <asp:CheckBox runat="server" ID="chkSeverityLevel02" class="cb custom-label" Text="<%$ Resources:LocalizedText, L2_Significant %>" /><br />
                        <asp:CheckBox runat="server" ID="chkSeverityLevel03" class="cb custom-label" Text="<%$ Resources:LocalizedText, L3_Severe %> " /><br />
                        <asp:CheckBox runat="server" ID="chkSeverityLevel04" class="cb custom-label" Text="<%$ Resources:LocalizedText, L4_Major %>" /><br />
                    </div>
                </div>
        
           <br /><br />
            </asp:Panel>
               <div id="ResponseMsg" class="labelEmphasis">
                   <asp:Label ID="lblResponseMsg" runat="server" Text=""></asp:Label>
               </div>
		<center>
			<telerik:RadButton ID="btnSave" runat="server" Text="<%$ Resources:LocalizedText, Save %>" CssClass="UseSubmitAction" Skin="Metro" 
				OnClientClicked="ChangeClear" OnClick="btnSave_Click" AutoPostBack="true" Visibl="false" SingleClick="true" SingleClickText="<%$ Resources:LocalizedText, Save %>"/>
		</center>
	</div>
</asp:Panel>



