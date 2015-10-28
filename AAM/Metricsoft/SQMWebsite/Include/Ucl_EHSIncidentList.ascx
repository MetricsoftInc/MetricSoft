<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Ucl_EHSIncidentList.ascx.cs"
    Inherits="SQM.Website.Ucl_EHSIncidentList" EnableViewState="true" %>

<telerik:RadGrid ID="rgIncidents" runat="server" Skin="Metro" AllowMultiRowSelection="false"
    OnSelectedIndexChanged="rgIncidents_SelectedIndexChanged" ClientSettings-EnablePostBackOnRowClick="true" OnNeedDataSource="rgIncidents_NeedDataSource"
    OnItemDataBound="rgIncidents_ItemDataBound" OnSortCommand="rgIncidents_SortCommand">
    <ClientSettings>
        <Selecting AllowRowSelect="true" />
    </ClientSettings>
    <MasterTableView DataKeyNames="INCIDENT_ID" Width="100%" TableLayout="Fixed" AutoGenerateColumns="False"
        AllowSorting="True" AllowPaging="true" PageSize="10">
        <Columns>
            <telerik:GridTemplateColumn UniqueName="IncidentIdTemplateColumn" HeaderText="Incident ID" HeaderStyle-Font-Size="Smaller"
                SortExpression="INCIDENT_ID" HeaderStyle-Width="75">
                <ItemTemplate>
                    <strong><%# LeadingZeroId(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "INCIDENT_ID"))) %></strong>
                </ItemTemplate>
            </telerik:GridTemplateColumn>

            <telerik:GridBoundColumn DataField="INCIDENT_DT" HeaderText="Date" HeaderStyle-Font-Size="Smaller" DataFormatString="{0:d}"
                HeaderStyle-Width="95">
            </telerik:GridBoundColumn>
            <telerik:GridButtonColumn UniqueName="ClosedButtonColumn" ButtonType="ImageButton" HeaderText="<%$ Resources:LocalizedText, Closed %>" ImageUrl="/images/ico-lock.png" HeaderStyle-Width="52" HeaderStyle-Font-Size="Smaller" CommandName="Select"></telerik:GridButtonColumn>
            <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderStyle-Font-Size="Smaller" HeaderText="<%$ Resources:LocalizedText, IncidentType %>/Description"
                SortExpression="ISSUE_TYPE,DESCRIPTION" HeaderStyle-Width="90%" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <div class="ehsIncidentItem">
                        <div class="ehsIncidentItemInner">
                            <span style="float: right; margin: 1px 6px 0 0;"><%# DisplayProblemIcon((decimal)DataBinder.Eval(Container.DataItem, "INCIDENT_ID")) %></span>
                            <strong><%# DataBinder.Eval(Container.DataItem, "ISSUE_TYPE") %></strong><br />
                            <span class="smallTableText"><%# GetPlantName((decimal)DataBinder.Eval(Container.DataItem, "INCIDENT_ID")) %></span><br />
                            <span class="smallTableText"><%# TruncatedText(DataBinder.Eval(Container.DataItem, "DESCRIPTION").ToString(), 100) %></span>
                        </div>
                    </div>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>