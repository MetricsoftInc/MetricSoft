<%@ Page Title="Media_Videos" Language="C#" MasterPageFile="~/PSMaster.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Media_Videos.aspx.cs" Inherits="SQM.Website.Media_Videos" ValidateRequest="false" meta:resourcekey="PageResource1" %>

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
							<div style="float: right; position:relative;">
								<div class="demo-container size-thin" style="float: right; padding-left: 20px;">
									<a id="videoProcess" href="javascript:void(0);">Video Process</a>
									<telerik:RadToolTip RenderMode="Lightweight" ID="RadToolTip2" runat="server" IsClientID="true" HideEvent="ManualClose" Position="MiddleLeft" OffsetX="35"
										RelativeTo="Element" EnableRoundedCorners="true" TargetControlID="videoProcess" EnableShadow="true"
										ShowEvent="OnClick" Width="700px" Height="130px" VisibleOnPageLoad="false" Modal="true">
										II.	American Axle Video Process - Process steps<br />
										<ol>
											<li>Record
												<ol>
												<li>Turn on your iPad and select the “Camera” app icon.></li>
												<li>Hold your iPad in landscape (the long edges are the top and bottom).</li>
												<li>Choose “Video” from the right-side options.
													<ul>
														<li>If you must use zoom, pinch the middle of the screen.</li>
													</ul>
												</li>
												<li>Select the red button to start recording.</li>
												<li>Select the red button to stop recording.</li>
											</ol>
											</li>
											<li>View
												<ol>
													<li>Select the “Photos” app from your iPad Home screen.</li>
													<li>Select the video you wish to view.</li>
												</ol>
											</li>
											<li>Trim
												<ol>
													<li>Tap the top of your screen to bring up the status bar.</li>
													<li>Grab the arrow in a black box on the left and drag to the right to trim the start of a video.</li>
													<li>Grab the arrow at the other end and drag to the left to trim the end. The on-screen preview will help you judge your edit.</li>
													<li>When you are satisfied with the edit, tap the word “Trim” in the upper-right corner and choose the option “Save as a new clip.” Do not click “Trim Original” as this will save over your original. It is best to keep the original in case it is needed later.</li>
												</ol>
											</li>
											<li>Save
												<ol>
													<li>Click the <b>Save</b> button on the iPad.</li>
												</ol>
											</li>
										</ol>
									</telerik:RadToolTip>
								</div>
								<div class="demo-container size-thin" style="float: right;">
									<a id="videoTips" href="javascript:void(0);">Best Practices to take a video</a>
									<telerik:RadToolTip RenderMode="Lightweight" ID="RadToolTip1" runat="server" IsClientID="true" HideEvent="ManualClose" Position="MiddleLeft" OffsetX="35"
										RelativeTo="Element" EnableRoundedCorners="true" TargetControlID="videoTips" EnableShadow="true"
										ShowEvent="OnClick"  Width="700px" Height="130px" VisibleOnPageLoad="false" Modal="true">
										I.	American Axle Video Best Practices<br />
										<ol>
											<li>Lighting
												<ul>
													<li>Do video in a well-lit area.
														<ul>
															<li>Filming in dark areas will cause the iPad camera to compensate and add “video noise” (graininess).</li>
														</ul>
													</li>
													<li>Do ensure the foreground (not the background) is well lit.
														<ul>
															<li>Keep windows behind you when filming inside.</li>
															<li>Keep the sun behind you when filming outside.</li>
														</ul>
													</li>
												</ul>
											</li>
											<li>Steadiness and Zoom
												<ul>
													<li>Do minimize iPad movement.
														<ul>
															<li>In lieu of having a tripod, providing any type of support, such as leaning your iPad against a wall, will minimize movement and enhance your video. </li>
															<li>If you want to follow action by moving your iPad, do it very slowly. </li>
														</ul>
													</li>
													<li>Do not use the digital zoom on your iPad; it will decrease quality.
														<ul>
															<li>Film close to your subject. If you use zoom instead, any movement made with the iPad will be greatly exaggerated. </li>
															<li>If you must use zoom, set it before recording and do not change it while recording.</li>
														</ul>
													</li>
												</ul>
											</li>
											<li>Sound
												<ul>
													<li>Do video in a quiet environment away from machine noises, traffic, air handling, excessive wind, etc.</li>
													<li>Do make sure your subject(s) speak clearly and directly to the camera at all times. 
														<ul>
															<li>The closer the subject is to the iPad, the clearer the sound will be.</li>
															<li><b>Note:</b> At this time, all subjects should speak in English. However, if your video is not a safety video and is only to be shown at your local plant, the speaker may speak in his/her native language.</li>
														</ul>
													</li>
												</ul>
											</li>
											<li>Other Best Practices
												<ul>
													<li>Videos should be no more than 5 minutes in length.</li>
													<li>Do video some “establishing shots.” 
														<ul>
															<li>For example, if filming a video about a specific machine, record the machine for at least 10 seconds with no narration. This footage can be used as background material when text is added over it.</li>
														</ul>
													</li>
													<li>Do video in landscape (horizontal) mode. Do not video holding your iPad vertically. </li>
													<li>What you see is what you get. If your video looks poor on your screen, it will look poor when posted to the plant monitors. </li>
													<li>Do obtain a video release statement from all non-AAM employees. This only applies if you are filming an individual in a non-safety plant video. If you are filming a crowd on plant property, you do not need a release.</li>
													<li>Do not film minors without the expressed written permission of a parent or guardian, who must be an AAM employee. This only applies if you are filming an individual in a non-safety plant video. If you are filming a crowd on plant property, you do not need a release, but you should try not to film the minors in the crowd.</li>
													<li>Do make employees aware when you are filming them. They can opt out of being filmed.</li>
												</ul>
											</li>
										</ol>
									</telerik:RadToolTip>
								</div>
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

					<div class="row-fluid" style="position: relative;">

						<div style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lblPlantSelect" CssClass="prompt"></asp:Label>
						</div>
						<div style="float: left;">
							<telerik:RadComboBox ID="ddlPlantSelect" runat="server" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" ZIndex="9000" Skin="Metro" Height="350px" Width="256px" OnClientLoad="DisableComboSeparators" meta:resourcekey="ddlPlantSelectResource1"></telerik:RadComboBox>
						</div>
					</div>
					<div class="clearfix"></div>

					<div class="row-fluid" style="margin-top: 3px; position: relative;">

						<div style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lblVideoSource" CssClass="prompt" Text="<%$ Resources:LocalizedText, VideoSourceType %>"></asp:Label>
						</div>
						<div style="float: left;">
							<telerik:RadComboBox ID="rcbVideoSource" runat="server" ToolTip="<%$ Resources:LocalizedText, VideoSelectSource %>" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>
						</div>
						<div style="float: left; width: 160px; padding-left: 10px;">
							<asp:Label runat="server" ID="lblVideoType" CssClass="prompt" Text="<%$ Resources:LocalizedText, VideoType %>"></asp:Label>
						</div>
						<div style="float: left;">
							<telerik:RadComboBox ID="rcbVideoType" runat="server" ToolTip="<%$ Resources:LocalizedText, VideoSelectType %>" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false" EnableCheckAllItemsCheckBox="true" CheckBoxes="true">
							</telerik:RadComboBox>
						</div>
					</div>
					<div class="clearfix"></div>

					<div class="row-fluid" style="position: relative;">

						<div style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lblInjuryType" CssClass="prompt" Text="<%$ Resources:LocalizedText, InjuryType %>"></asp:Label>
						</div>
						<div style="float: left;">
							<telerik:RadComboBox ID="rcbInjuryType" runat="server" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false" EnableCheckAllItemsCheckBox="false">
							</telerik:RadComboBox>
						</div>
						<div style="float: left; width: 160px; padding-left: 10px;">
							<asp:Label runat="server" ID="lblBodyPart" CssClass="prompt" Text="<%$ Resources:LocalizedText, BodyPart %>"></asp:Label>
						</div>
						<div style="float: left;">
							<telerik:RadComboBox ID="rcbBodyPart" runat="server" Style="width: 256px;" ZIndex="9000" Skin="Metro" AutoPostBack="false"></telerik:RadComboBox>
						</div>
					</div>
					<div class="clearfix"></div>

					<asp:PlaceHolder ID="phVideo" runat="server">

						<div class="row-fluid" style="position: relative;">
							<div style="float: left; width: 160px;">
								<asp:Label runat="server" ID="lblVideoOwner" CssClass="prompt" Text="<%$ Resources:LocalizedText, VideoOwner %>"></asp:Label>
							</div>
							<div style="float: left; width: 256px;">
								<telerik:RadComboBox ID="rcbVideoOwner" runat="server" ToolTip="<%$ Resources:LocalizedText, VideoSelectOwner %>" Width="135" ZIndex="9000" Skin="Metro" AutoPostBack="false">
									<Items>
										<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, All %>" Value="all" />
										<telerik:RadComboBoxItem Text="<%$ Resources:LocalizedText, VideoOwn %>" Value="own" />
									</Items>
								</telerik:RadComboBox>
							</div>

							<div style="float: left; width: 160px; padding-left: 10px;">
								<asp:Label runat="server" ID="lblStatus" CssClass="prompt" Text="<%$ Resources:LocalizedText, Status %>"></asp:Label>
							</div>
							<div style="float: left;">
								<telerik:RadComboBox ID="rcbStatusSelect" runat="server" ToolTip="<%$ Resources:LocalizedText, VideoSelectStatus %>" Width="256px" ZIndex="9000" Skin="Metro" AutoPostBack="false">
								</telerik:RadComboBox>
							</div>

						</div>
						<div class="clearfix"></div>
					</asp:PlaceHolder>

					<div class="row-fluid" style="margin-top: 7px; position: relative;">

						<div style="float: left; width: 160px;">
							<asp:Label runat="server" ID="lbrKeyWord" CssClass="prompt" Text="<%$ Resources:LocalizedText, KeyWordSearch %>"></asp:Label>&nbsp;<asp:Image runat="server" ID="imgHelp" ImageUrl="/images/ico-question.png" /><telerik:RadToolTip runat="server" TargetControlID="imgHelp" ID="rttKeywordToolTip" IsClientID="false" RelativeTo="Element" OffsetX="35" EnableRoundedCorners="true" EnableShadow="true" Width="200" Height="100" Animation="Fade" Position="MiddleRight" ContentScrolling="Auto" HideEvent="LeaveTargetAndToolTip" RenderMode="Lightweight">
								<asp:Literal runat="server" ID="litKeywordHelp" Text="<%$ Resources:LocalizedText, KeyWordSearchDesc %>"></asp:Literal></telerik:RadToolTip>
						</div>
						<div style="float: left;">
							<asp:TextBox ID="tbKeyWord" runat="server" ToolTip="<%$ Resources:LocalizedText, KeyWordSearchDesc %>" Width="500px" ZIndex="9000" Skin="Metro" AutoPostBack="false"></asp:TextBox>
						</div>
					</div>
					<div class="clearfix"></div>
					<div class="row-fluid" style="margin-top: 7px;">


						<span style="float: left; margin-top: 4px;">
							<span style="padding-right: 20px;">
								<asp:Label runat="server" ID="lblVideoDate" Text="<%$ Resources:LocalizedText, VideoDateFrom %>" CssClass="prompt"></asp:Label></span>
							<%--<span style="margin-right: -10px !important;">--%>
							<span>
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
