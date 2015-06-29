<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_PartSearch.ascx.cs" Inherits="SQM.Website.Ucl_PartSearch" %>
<%@ Register src="~/Include/Ucl_PartList.ascx" TagName="PartList" TagPrefix="Ucl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<script type="text/javascript">
    function UpdateProfileButtons() {
        // document.getElementById("btnProfileMeasureNew").disabled = true;
    }

    function OpenPartListWindow() {
        $find("<%=winPartList.ClientID %>").show();
    }

    function ClosePartListWindow() {
        var oWindow = GetRadWindow();  //Obtaining a reference to the current window 
        oWindow.Close();
    }
</script>

<asp:Button id="btnOpenPartListWindow" runat="server" CssClass="buttonList" Text="List"
     OnClick="OnOpenPartListWindow_Click" ToolTip="list part numbers"/>

<telerik:RadSearchBox ID="rsbPart" runat="server"  MaxResultCount="20" EnableAutoComplete="true" Skin="Metro" OnClientSearch="onPartSearch" OnSearch="OnSearchServer" 
	ShowSearchButton="false" EmptyMessage="Type a part number" Width="220">
	<DropDownSettings Height="320" Width="530">
		<HeaderTemplate>
			<table cellpadding="0" cellspacing="1" class="searchBoxResults" width="500" style="margin-left: 5px;">
				<tr>
					<th style="width: 80px; text-align: left;">
						Part #
					</th>
					<th style="width: 240px; text-align: left;">
						Part Description
					</th>
					<th style="width: 140px; text-align: left;" runat="server" id="thPartProgram" visible="false">
						Program
					</th>
					<th style="width: 40px; text-align: center;">
						Active
					</th>
				</tr>
			</table>
		</HeaderTemplate>
		<ItemTemplate>
			<table cellpadding="0" cellspacing="0" class="searchBoxResults" width="500">
			<tr>
				<td style="background: #EEEAE0; width: 80px;">
					<b><%# DataBinder.Eval(Container.DataItem, "PART_NUM") %></b>
				</td>
				<td style="background: #fff; width: 240px;">
					<b><%# DataBinder.Eval(Container.DataItem, "PART_NAME")%></b>
				</td>
				<td style="background: #fff; width: 140px;" id="tdPartProgram" runat="server" visible="false">
					<b><%# ReturnProgramName(DataBinder.Eval(Container.DataItem, "PROGRAM_NAME").ToString())%></b>
				</td>
				<td style="background: #fff; width: 40px; text-align: center;"">
					<%# ReturnActiveStatus(DataBinder.Eval(Container.DataItem, "STATUS").ToString())%>
				</td>
			</tr>
			</table>
		</ItemTemplate>
	</DropDownSettings>
</telerik:RadSearchBox>
<asp:HiddenField id="hfPartNumberPerspective" runat="server"/>
<telerik:RadDropDownList ID="rddlProgram" runat="server" Skin="Metro" Width="160" DropDownWidth="180" Font-Size="9" AutoPostBack="true">
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
</telerik:RadDropDownList>


<telerik:RadWindow runat="server" ID="winPartList" RestrictionZoneID="ContentTemplateZone" Skin="Metro" Modal="true" Height="400" Width="700" Behaviors="Move,Close" Title="Select Part Number">
    <ContentTemplate>
        <div style="margin-top: 5px;">
            <asp:Label ID="lblPartListInstruct" runat="server" CssClass="instructText" Text="Part numbers either produced or consumed by the selected Business Location."></asp:Label>
            <br />
            <div style="margin-top: 5px;">
                <Ucl:PartList id="uclPartList" runat="server"/>
            </div>
        </div>
    </ContentTemplate>
</telerik:RadWindow>


