<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PersonSearch.ascx.cs" Inherits="SQM.Website.Ucl_PersonSearch" %>
<%--<%@ Register src="~/Include/Ucl_PersonList.ascx" TagName="PersonList" TagPrefix="Ucl" %>--%>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<script type="text/javascript">
	function UpdateProfileButtons() {
		// document.getElementById("btnProfileMeasureNew").disabled = true;
	}

//	function OpenPersonListWindow() {
<%--//		$find("<%=winPersonList.ClientID %>").show();--%>
//	}

//	function ClosePersonListWindow() {
//		var oWindow = GetRadWindow();  //Obtaining a reference to the current window 
//		oWindow.Close();
//	}

</script>

<asp:Button id="btnOpenPersonListWindow" runat="server" CssClass="buttonList" Text=""
	 OnClick="OnOpenPersonListWindow_Click" ToolTip="list persons" Enabled="false" Visible="false"/>

<telerik:RadSearchBox ID="rsbPerson" runat="server"  MaxResultCount="20" EnableAutoComplete="false" DataKeyNames="PersonId" Skin="Metro" OnClientSearch="onPersonSearch" OnSearch="OnSearchServer" 
	ShowSearchButton="false" EmptyMessage="Begin typing (or spacebar)" Width="276">
	<DropDownSettings Height="320" Width="530">
		<HeaderTemplate>
			<table cellpadding="0" cellspacing="1" class="searchBoxResults" width="500" style="margin-left: 5px;">
				<tr>
					<th style="width: 90px; text-align: left;">
						Name
					</th>
					<th style="width: 240px; text-align: left;">
						Email
					</th>
					<th>
					</th>
<%-- 				<th style="width: 140px; text-align: left;" runat="server" id="thPartProgram" visible="false">
						Program
					</th>
					<th style="width: 40px; text-align: center;">
						Active
					</th>--%>
				</tr>
			</table>
		</HeaderTemplate>
		<ItemTemplate>
			<table cellpadding="0" cellspacing="0" class="searchBoxResults" width="500">
			<tr>
				<td style="background: #EEEAE0; width: 90px;">
					<b><%# DataBinder.Eval(Container.DataItem, "PersonName") %></b>
				</td>
				<td style="background: #fff; width: 240px;">
					<b><%# DataBinder.Eval(Container.DataItem, "PersonEmail")%></b>
				</td>
				<td id="tdPersonID" runat="server" visible="false">
					<%# DataBinder.Eval(Container.DataItem, "PersonId")%>
				</td>
<%--			<td style="background: #fff; width: 140px;" id="tdPartProgram" runat="server" visible="false">
					<b><%# ReturnProgramName(DataBinder.Eval(Container.DataItem, "PROGRAM_NAME").ToString())%></b>
				</td>--%>
<%--				<td style="background: #fff; width: 40px; text-align: center;"">
					<%# ReturnActiveStatus(DataBinder.Eval(Container.DataItem, "STATUS").ToString())%>
				</td>--%>
			</tr>
			</table>
		</ItemTemplate>
	</DropDownSettings>
</telerik:RadSearchBox>

<%--<asp:HiddenField id="hfPartNumberPerspective" runat="server"/>--%>
<%--<telerik:RadDropDownList ID="rddlProgram" runat="server" Skin="Metro" Width="160" DropDownWidth="180" Font-Size="9" AutoPostBack="true">
	<Items>
		<telerik:DropDownListItem Text="Any Program" />
	</Items>
</telerik:RadDropDownList>
<telerik:RadDropDownList ID="rddlLocation" runat="server" Skin="Metro" Width="140" DropDownWidth="180" Font-Size="9" AutoPostBack="true">
	<Items>
		<telerik:DropDownListItem Text="Any Location" />
	</Items>
</telerik:RadDropDownList>
<telerik:RadDropDownList ID="rddlStatus" runat="server" Skin="Metro" Width="100" DropDownWidth="100" Font-Size="9" AutoPostBack="true">
	<Items>
		<telerik:DropDownListItem Text="Any Status" Value="any" />
		<telerik:DropDownListItem Text="Active Only" Value="active" />
	</Items>
</telerik:RadDropDownList>--%>


<%--<telerik:RadWindow runat="server" ID="winPersonList" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="700" Behaviors="Move,Close" Title="Select Person">
	<ContentTemplate>
		<div style="margin-top: 5px;">
			<asp:Label ID="lblPersonListInstruct" runat="server" CssClass="instructText" Text=""></asp:Label>
			<br />
			<div style="margin-top: 5px;">
				<Ucl:PersonList id="uclPersonList" runat="server"/>
			</div>
		</div>
	</ContentTemplate>
</telerik:RadWindow>--%>

