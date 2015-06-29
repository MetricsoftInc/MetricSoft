
        <table width="99%" border="0" cellspacing="0" cellpadding="0">
            <TR>
                <TD ALIGN=right class=admBkgd>
                    <table border="0" cellspacing="0" cellpadding="2" style="margin-top: 8px;">
                        <tr>
                            <td>
                                <asp:Button ID="lbCancelPart" CSSclass="buttonStd" runat="server" text="Cancel" 
                                    OnClientClick="return confirmAction('Cancel without saving');"  onclick="lbCancelPart_Click"></asp:Button>
                            </TD>  
                            <td>
                                <asp:Button ID="lbSavePart" CSSclass="buttonEmphasis" runat="server" text="Save" style="margin-right: 20px;" 
                                    OnClientClick="return confirmChange('Business Organization');"  onclick="lbSavePart_Click"></asp:Button>
                            </td>
                        </tr>
                    </table>
                </TD>
            </TR> 

            <tr>
                <td class="editArea">
                    <table width="99%" align="center" border="0" cellspacing="1" cellpadding="2" class="darkBorder">
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblSourceSystem" runat="server" text="Source System"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartSource" size="40" maxlength="50" runat="server"/></td>      
			            </tr>
                        <td class="columnHeader">
                                <asp:Label ID="lblPartSerialNum" runat="server" text="Serial Number"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableDataAlt"><asp:TextBox ID="tbPartSerialNum" size="40" maxlength="100" runat="server"/></td>      
			            </tr>
                            <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblSetPartStatus" runat="server" text="Status"></asp:Label>
                            </td>
                            <td class="required">&nbsp;</td>
                            <td class="tableDataAlt"><asp:DropDownList ID="ddlPartStatus" runat="server"></asp:DropDownList></td>
                        </TR>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPartRevision" runat="server" text="Revision Level or Number"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblPartRevision_out" Text="" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPartDrawing" runat="server" text="Drawing Number or Refernce"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblPartDrawing_out" Text="" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPartUpdatedBy" runat="server" text="Updated By"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblPartLastUpdate" Text="" runat="server"/></td>
                        </tr>
                        <tr>
                            <td class="columnHeader">
                                <asp:Label ID="lblPartUpdatedDate" runat="server" text="Last Update Date"></asp:Label>
                            </td>
                            <td class="tableDataAlt">&nbsp;</td>
                            <td CLASS="tableData"><asp:Label ID="lblPartLastUpdateDate" Text="" runat="server"/></td>
                        </tr>
                    </table>
                 </td>
             </tr>
     </table>
 