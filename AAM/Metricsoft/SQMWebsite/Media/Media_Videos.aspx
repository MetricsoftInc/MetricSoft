<%@ Page Title="Media_Videos" Language="C#" MasterPageFile="~/RspPSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Media_Videos.aspx.cs" Inherits="SQM.Website.Media_Videos" ValidateRequest="false" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_VideoList.ascx" TagName="VideoList" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_VideoForm.ascx" TagName="VideoForm" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_AttachVideo.ascx" TagName="AttachVideo" TagPrefix="Ucl" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		$(window).load(function () {
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfwidth').value = $(window).width();
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfheight').value = $(window).height();
		});

		$(window).resize(function () {
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfwidth').value = $(window).width();
			document.getElementById('ctl00_ContentPlaceHolder_Body_hfheight').value = $(window).height();
		});

		function StandardConfirm(sender, args) {

			// Some pages will have no validators, so skip
			if (typeof Page_ClientValidate === "function") {
				var validated = Page_ClientValidate('Val');

				if (!validated)
					alert("Please fill out all required fields.");
			}
		}
		function DeleteConfirm(button, args) {
			args.set_cancel(!confirm("Delete video - are you sure?  Videos cannot be undeleted."));
		}

	</script>
</asp:Content>
<%--<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>--%>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">
	<asp:HiddenField ID="hfwidth" runat="server" />
	<asp:HiddenField ID="hfheight" runat="server" />
	<div class="pageWrapper">

		<div class="container-fluid tabActiveTableBg">

			<div class="row-fluid">

				<div class="col-xs-12 col-sm-12">

					<span style="float: left; margin-top: 6px;">
						<asp:Label ID="lblViewTitle" runat="server" CssClass="pageTitles" Text="<%$ Resources:LocalizedText, MediaVideos %>"></asp:Label></span>

						<br class="clearfix visible-xs-block" />

						<div class="col-xs-7 col-sm-3">
							<br />
							<span style="clear: both; float: left; margin-top: -14px;">
								<telerik:RadButton ID="rbNew" runat="server" Text="<%$ Resources:LocalizedText, VideoAdd %>" Icon-PrimaryIconUrl="/images/ico-plus.png"
									CssClass="metroIconButton" Skin="Metro" OnClick="rbNew_Click" CausesValidation="False" style="position: relative;" />
							</span>
						</div>

						<br class="clearfix visible-xs-block" />

						<asp:Label ID="lblPageInstructions" runat="server" CssClass="instructTextFloat" Text="<%$ Resources:LocalizedText, MediaVideosInstruct %>"></asp:Label>
				</div>
			</div>

			<br style="clear: both;" />
			<telerik:RadPersistenceManager ID="RadPersistenceManager1" runat="server"></telerik:RadPersistenceManager>

			<div id="divVideoList" runat="server" visible="true">
				<%--	$$$$$$$$$$$$$$ Video Selection START $$$$$$$$$$$$$$$$$$$$$$$ --%>

				<div class="container-fluid summaryDataEnd" style="padding: 3px 4px 7px 0">

					<div class="row-fluid">

						<span style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt"></asp:Label>
						</span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
						<telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="256px" OnClientLoad="DisableComboSeparators" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>

						<div class="visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

					</div>

                    <div class="row-fluid">

                        <span style="float: left; width: 160px;">
                            <asp:Label runat="server" ID="lblVideoType" CssClass="prompt" Text="<%$ Resources:LocalizedText, Type %>"></asp:Label>
                        </span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
                        <telerik:RadComboBox ID="rcbVideoType" runat="server" Style="margin-right: 15px;" ToolTip="<%$ Resources:LocalizedText, VideoSelectType %>" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>

                        <div class="visible-xs"></div>
                        <br class="visible-xs-block" style="margin-top: 7px;" />

                    </div>

					<asp:PlaceHolder ID="phVideo" runat="server">

						<div class="row-fluid">
							<span style="float: left; width: 160px;">
								<asp:Label runat="server" ID="lblVideoOwner" CssClass="prompt" Text="<%$ Resources:LocalizedText, VideoOwner %>"></asp:Label>
							</span>&nbsp;&nbsp;
									<br class="visible-xs-block" />
							<telerik:RadComboBox ID="rcbVideoOwner" runat="server" Style="margin-right: 15px;" ToolTip="<%$ Resources:LocalizedText, VideoSelectOwner %>" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false">
								<Items>
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="all" />
									<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, VideoOwn %>" Value="own" />
								</Items>
							</telerik:RadComboBox>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" style="margin-top: 7px;" />

							<span style="padding-left: 12px;">
								<asp:Label runat="server" ID="lblStatus" CssClass="prompt" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>&nbsp;&nbsp;
                                            <telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="<%$ Resources:LocalizedText, VideoSelectStatus %>" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false">
											</telerik:RadComboBox>
							</span>

							<div class="clearfix visible-xs"></div>
							<br class="visible-xs-block" />

						</div>
					</asp:PlaceHolder>

					<div class="row-fluid" style="margin-top: 7px;">


						<span style="float: left; margin-top: 4px;">
							<span style="padding-right: 20px;">
								<asp:Label runat="server" ID="lblVideoDate" Text="<%$ Resources:LocalizedText, AssessmentDateFrom %>" CssClass="prompt"></asp:Label></span>
							<span style="margin-right: -10px !important;">
								<telerik:RadDatePicker ID="dmFromDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" meta:resourcekey="dmFromDateResource1">
									<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;"></Calendar>

									<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="">
										<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

										<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

										<FocusedStyle Resize="None"></FocusedStyle>

										<DisabledStyle Resize="None"></DisabledStyle>

										<InvalidStyle Resize="None"></InvalidStyle>

										<HoveredStyle Resize="None"></HoveredStyle>

										<EnabledStyle Resize="None"></EnabledStyle>
									</DateInput>

									<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
								</telerik:RadDatePicker>
							</span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" />

						<span>
							<span style="margin-left: 14px; padding-right: 8px;">
								<asp:Label runat="server" ID="lblToDate" CssClass="prompt"></asp:Label>
								<telerik:RadDatePicker ID="dmToDate" runat="server" CssClass="textStd" Width="145px" Skin="Metro" DateInput-Skin="Metro" DateInput-Font-Size="Small" meta:resourcekey="dmToDateResource1">
									<Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" runat="server"></Calendar>

									<DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" LabelWidth="64px" Skin="Metro" Font-Size="Small" Width="" runat="server">
										<EmptyMessageStyle Resize="None"></EmptyMessageStyle>

										<ReadOnlyStyle Resize="None"></ReadOnlyStyle>

										<FocusedStyle Resize="None"></FocusedStyle>

										<DisabledStyle Resize="None"></DisabledStyle>

										<InvalidStyle Resize="None"></InvalidStyle>

										<HoveredStyle Resize="None"></HoveredStyle>

										<EnabledStyle Resize="None"></EnabledStyle>
									</DateInput>

									<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
								</telerik:RadDatePicker>
							</span>
						</span>

						<div class="clearfix visible-xs"></div>
						<br class="visible-xs-block" style="margin-top: 7px;" />

						<span class="noprint">
							<%--<asp:Label ID="lblShowImage" runat="server" Text="Display Initial Image" CssClass="prompt"></asp:Label>
                                        <span style="padding-top: 10px;""><asp:CheckBox id="cbShowImage" runat="server" Checked="false"/></span>--%>
							<asp:Button ID="btnSearch" runat="server" Style="margin-left: 20px;" CssClass="buttonEmphasis" Text="<%$ Resources:LocalizedText, Search %>" ToolTip="<%$ Resources:LocalizedText, VideoList %>" OnClick="btnSearch_Click" CausesValidation="false"/>
						</span>

					</div>
				</div>

				<%--	$$$$$$$$$$$$$$ Video Selection END $$$$$$$$$$$$$$$$$$$$$$$ --%>


				<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel2" HorizontalAlign="NotSet" meta:resourcekey="RadAjaxPanel2Resource1">

					<div class="clearfix visible-xs"></div>
					<br class="visible-xs-block" />

					<div class="row-fluid" style="margin-top: 4px; margin-bottom: 4px;">

						<asp:Panel ID="pnlVideoDetails" runat="server" Width="100%" Visible="False" meta:resourcekey="pnlVideoDetailsResource1">
							<div class="row-fluid">
								<br />
								<asp:Label ID="lblVideoDetails" runat="server" CssClass="prompt" meta:resourcekey="lblVideoDetailsResource1"></asp:Label>
								<asp:LinkButton ID="lnkVideoDetailsClose" runat="server" CssClass="buttonLink" Style="float: right; margin-right: 10px;" OnClick="lnkVideoDetailsClose_Click" ToolTip="<%$ Resources:LocalizedText, Close %>">
                                             <img src="/images/defaulticon/16x16/cancel.png" alt="" style="vertical-align: middle;"/>
								</asp:LinkButton>
								<br />
								<br />
							</div>
						</asp:Panel>
					</div>
				</telerik:RadAjaxPanel>


				<div class="noprint">
					<ucl:videolist id="uclVideoList" runat="server" />
				</div>
			</div>

			<Ucl:VideoForm ID="uclVideoForm" runat="server" />

            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" meta:resourcekey="RadAjaxManager1Resource1">
            </telerik:RadAjaxManager>
		</div>
	</div>

	<Ucl:AttachVideo ID="uclAttachVideo" runat="server" />

</asp:Content>
