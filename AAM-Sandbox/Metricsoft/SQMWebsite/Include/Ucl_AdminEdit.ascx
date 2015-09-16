<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_AdminEdit.ascx.cs" Inherits="SQM.Website.Ucl_AdminEdit" %>
<%@ Register src="~/Include/Ucl_BusinessLoc.ascx" TagName="BusLoc" TagPrefix="Ucl" %>
<%@ Register src="~/Include/Ucl_AdminPasswordEdit.ascx" TagName="PassEdit" TagPrefix="Ucl" %>

	<asp:Panel runat="server" ID="pnlUserPrefEdit">
		<table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
			<tr>
				<td class="columnHeader" width="24%">
					<asp:Label ID="lblUserPhone" runat="server" text="Phone"></asp:Label>
				</td>
				<td class="required" width="1%">&nbsp;</td>
				<td class="tableDataAlt" width="75%">
					<asp:TextBox ID="tbUserPhone" size="50" maxlength="20" runat="server"></asp:TextBox>
				</td>
			</tr>
		    <tr id="trHRLoc" runat="server" visible="false">
				<td class="columnHeader">
					<asp:Label ID="lblHRLocation" runat="server" text="HR Business Location"></asp:Label>
				</td>
				<td class="tableDataAlt">&nbsp;</td>
				<td class="tableDataAlt">
					<Ucl:BusLoc id="uclHRLoc" runat="server"/>
				</td>
			</tr>
			<tr id="trWorkingLoc" runat="server" visible="false">
				<td class="columnHeader">
					<asp:Label ID="lblUserCompany" runat="server" text="Working Business Location"></asp:Label>
				</td>
				<td class="tableDataAlt">&nbsp;</td>
				<td class="tableDataAlt">
					<Ucl:BusLoc id="uclWorkingLoc" runat="server"/>
				</td>
			</tr>
            <tr>
				<td class="columnHeader">
                    <asp:LinkButton ID="lnkChangePwd" runat="server" CSSClass="buttonLink" Text="Change Password" OnClick="btnPassEdit_Click" ToolTip="Display password edit fields"></asp:LinkButton>
				</td>
				<td class="tableDataAlt">&nbsp;</td>
				<td class="tableDataAlt">
				    <asp:Panel runat="server" ID="pnlAdminPasswordEdit" Visible="false">
			            <Ucl:PassEdit ID="uclPassEdit" runat="server" strCurrentControl="pnlAdminPasswordEdit" />
		            </asp:Panel>
				</td>
			</tr>
            <tr>
				<td align="left" class="tabActiveTableBg"colspan="3">
					<asp:Button ID="btnPrefCancel" CSSclass="buttonStd" runat="server" text="Cancel" 
						onclick="btnCancel_Click" CommandArgument="prefs"></asp:Button>
					<asp:Button ID="btnPrefSave" CSSclass="buttonEmphasis" runat="server" text="Save" 
					 OnClientClick="return confirmChange('User Preferences');" onclick="btnSave_Click" CommandArgument="prefs"></asp:Button>
				</td>
			</tr>
		</table>
	</asp:Panel>

	<asp:Panel ID="pnlEditDept" runat="server" Visible = "false">
	  <table width="99%" class="editArea">
			<tr>
				<td align="right" class="optionArea">
					<asp:Button ID="btnDeptCancel" CSSclass="buttonStd" runat="server" text="Cancel" 
						onclick="btnCancel_Click" CommandArgument="dept"></asp:Button>
					<asp:Button ID="btnDeptSave" CSSclass="buttonEmphasis" runat="server" text="Save Department" 
						OnClientClick="return confirmChange('Department');" onclick="btnSave_Click" CommandArgument="dept"></asp:Button>
				</td>
			</tr>
			<tr>
				<td class="editArea">
					<table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblDeptCode" runat="server" text="Department Code"></asp:Label>
							</td>
							<td class="required">&nbsp;</td>
							<td CLASS="tableDataAlt"><asp:TextBox ID="tbDeptCode" size="30" maxlength="24" runat="server"/></td>
						</tr>
						<tr>
							<td class="columnHeader" width="39%">
								<asp:Label ID="lblDeptName" runat="server" text="Department Name"></asp:Label>
							</td>
							<td class="required" width="1%">&nbsp;</td>
							<td CLASS="tableDataAlt" width="60%">
								<asp:TextBox ID="tbDeptName" size="50" maxlength="255" runat="server"/>
							</td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblSetDeptStatus" runat="server" text="Status"></asp:Label>
							</td>
							<td class="required">&nbsp;</td>
							<td class="tableDataAlt"><asp:DropDownList ID="ddlDeptStatus" runat="server"></asp:DropDownList>
							</td>
						</tr>
							<tr>
							<td class="columnHeader">
								<asp:Label ID="lblDeptUpdatedBy" Text="Updated By:" runat="server"/>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableData">
								<asp:Label ID="lblDeptLastUpdate" Text="" runat="server"/>
							</td>
						</tr>
						<tr>
							<td class="columnHeader"><asp:Label ID="lblDeptUpdatedDate" Text="Last Update Date" runat="server"/></td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableData"><asp:Label ID="lblDeptLastUpdateDate" Text="" runat="server"/></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</asp:Panel>

		<asp:Panel ID="pnlLaborEdit" runat="server" Visible = "false">
		<table width="99%" class="editArea">
			<tr>
				<td class="optionArea">
					<asp:Button ID="btnLaborCancel" CSSclass="buttonStd" runat="server" text="Cancel" 
						onclick="btnCancel_Click" CommandArgument="labor"></asp:Button>
					<asp:Button ID="btnLaborSave" CSSclass="buttonEmphasis" runat="server" text="Save Labor Code" 
						OnClientClick="return confirmChange('Labor Type');" onclick="btnSave_Click" CommandArgument="labor"></asp:Button>
				</td>
			</tr>
			<tr>
				<td class="editArea">
					<table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblLaborCode" runat="server" text="Labor Code"></asp:Label>
							</td>
							<td class="required">&nbsp;</td>
							<td CLASS="tableDataAlt"><asp:TextBox ID="tbLaborCode" size="30" maxlength="24" runat="server"/></td>
						</tr>
						<tr>
							<td class="columnHeader" width="39%">
								<asp:Label ID="lblLaborName" runat="server" text="Labor Name"></asp:Label>
							</td>
							<td class="required" width="1%">&nbsp;</td>
							<td CLASS="tableDataAlt" width="60%">
								<asp:TextBox ID="tbLaborName" size="50" maxlength="255" runat="server"/>
							</td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblLaborRate" runat="server" text="Labor Rate"></asp:Label>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableDataAlt"><asp:TextBox ID="tbLaborRate" size="20" maxlength="20" runat="server"/></td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblSetLaborStatus" runat="server" text="Status"></asp:Label>
							</td>
							<td class="required">&nbsp;</td>
							<td class="tableDataAlt"><asp:DropDownList ID="ddlLaborStatus" runat="server"></asp:DropDownList>
							</td>
						</tr>
							<tr>
							<td class="columnHeader">
								<asp:Label ID="lblLaborUpdatedBy" Text="Updated By" runat="server"/>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableData">
								<asp:Label ID="lblLaborLastUpdate" Text="" runat="server"/>
							</td>
						</tr>
						<tr>
							<td class="columnHeader"><asp:Label ID="lblLaborUpdatedDate" Text="Last Update Date" runat="server"/></td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableData"><asp:Label ID="lblLaborLastUpdateDate" Text="" runat="server"/></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</asp:Panel>

	<asp:Panel ID="pnlLineEdit" runat="server" Visible = "false">
		<table width="99%" class="editArea">
			<tr>
				<td align="right" class="editArea">
					<asp:Button ID="btnLineCancel" CSSclass="buttonStd" runat="server" text="Cancel" 
						onclick="btnCancel_Click" CommandArgument="line"></asp:Button>
					<asp:Button ID="btnLineSave" CSSclass="buttonEmphasis" runat="server" text="Save" 
						OnClientClick="return confirmChange('Line/Operation');" onclick="btnSave_Click" CommandArgument="line"></asp:Button>
				</td>
			</tr>
			<tr>
				<td class="editArea">
					<table width="100%" align="center" border="0" cellspacing="0" cellpadding="1" class="borderSoft">
						<tr>
							<td class="columnHeader" width="39%">
								<asp:Label ID="lblLineName" runat="server" text="Line/Operation Name"></asp:Label>
							</td>
							<td class="required" width="1%">&nbsp;</td>
							<td CLASS="tableDataAlt" width="60%">
								<asp:TextBox ID="tbLineName" size="50" maxlength="255" runat="server"/>
							</td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblDowntimeRate" runat="server" text="Downtime Rate (per hour)"></asp:Label>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableDataAlt"><asp:TextBox ID="tbLineDownRate" size="20" maxlength="20" runat="server"/></td>
						</tr>
						<tr>
							<td class="columnHeader">
								<asp:Label ID="lblSetLineStatus" runat="server" text="Status"></asp:Label>
							</td>
							<td class="required">&nbsp;</td>
							<td class="tableDataAlt"><asp:DropDownList ID="ddlLineStatus" runat="server"></asp:DropDownList>
							</td>
						</tr>
							<tr>
							<td class="columnHeader">
								<asp:Label ID="lblLineUpdatedBy" Text="Updated By:" runat="server"/>
							</td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableData">
								<asp:Label ID="lblLineLastUpdate" Text="" runat="server"/>
							</td>
						</tr>
						<tr>
							<td class="columnHeader"><asp:Label ID="lblLineUpdatedDate" Text="Last Update Date" runat="server"/></td>
							<td class="tableDataAlt">&nbsp;</td>
							<td CLASS="tableData"><asp:Label ID="lblLineLastUpdateDate" Text="" runat="server"/></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</asp:Panel>





