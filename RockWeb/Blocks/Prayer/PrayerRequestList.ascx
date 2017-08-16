﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Prayer.PrayerRequestList" Codebehind="PrayerRequestList.ascx.cs" %>
<asp:UpdatePanel ID="upPrayerRequests" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlLists" runat="server" Visible="true">
            

            <div class="panel panel-block">
                <div class="panel-heading">
                    <h1 class="panel-title"><i class="fa fa-cloud-upload"></i> Prayer Requests</h1>
                </div>
                <div class="panel-body">
                    <div class="grid grid-panel">
                        <Rock:GridFilter ID="gfFilter" runat="server" OnApplyFilterClick="gfFilter_ApplyFilterClick" OnDisplayFilterValue="gfFilter_DisplayFilterValue">
                            <Rock:DateRangePicker ID="drpDateRange" runat="server" Label="Date Range" />

                            <Rock:RockDropDownList ID="ddlApprovedFilter" runat="server" Label="Approval Status">
                                <global::asp:ListItem Text="[All]" Value="all" />
                                <global::asp:ListItem Text="Approved" Value="approved" />
                                <glboal::asp:ListItem Text="Unapproved" Value="unapproved" />
                            </Rock:RockDropDownList>

                            <Rock:RockDropDownList ID="ddlUrgentFilter" runat="server" Label="Urgent Status">
                                <global::asp:ListItem Text="[All]" Value="all" />
                                <global::asp:ListItem Text="Urgent" Value="urgent" />
                                <global::asp:ListItem Text="Non-Urgent" Value="non-urgent" />
                            </Rock:RockDropDownList>

                            <Rock:RockDropDownList ID="ddlPublicFilter" runat="server" Label="Private/Public">
                                <global::asp:ListItem Text="[All]" Value="all" />
                                <global::asp:ListItem Text="Public" Value="public" />
                                <global::asp:ListItem Text="Non-Public" Value="non-public" />
                            </Rock:RockDropDownList>

                            <Rock:RockDropDownList ID="ddlActiveFilter" runat="server" Label="Active Status">
                                <global::asp:ListItem Text="[All]" Value="all" />
                                <global::asp:ListItem Text="Active" Value="active" />
                                <global::asp:ListItem Text="Inactive" Value="inactive" />
                            </Rock:RockDropDownList>

                            <Rock:RockDropDownList ID="ddlAllowCommentsFilter" runat="server" Label="Commenting">
                                <global::asp:ListItem Text="[All]" Value="all" />
                                <global::asp:ListItem Text="Allowed" Value="allow" />
                                <global::asp:ListItem Text="Not Allowed" Value="unallow" />
                            </Rock:RockDropDownList>

                            <Rock:CategoryPicker ID="catpPrayerCategoryFilter" runat="server" Label="Category" EntityTypeName="Rock.Model.PrayerRequest"/>

                            <Rock:RockCheckBox ID="cbShowExpired" runat="server" Label="Show Expired Requests?" />

                        </Rock:GridFilter>

                        <Rock:ModalAlert ID="maGridWarning" runat="server" />

                        <Rock:Grid ID="gPrayerRequests" runat="server" AllowSorting="true" RowItemText="request" OnRowSelected="gPrayerRequests_Edit" OnRowDataBound="gPrayerRequests_RowDataBound" >
                            <Columns>
                                <Rock:RockBoundField DataField="FullName" HeaderText="Name" SortExpression="FullName" />
                                <Rock:RockBoundField DataField="CategoryName" HeaderText="Category" SortExpression="CategoryName" />
                                <Rock:DateField DataField="EnteredDate" HeaderText="Entered" SortExpression="EnteredDate"/>
                                <Rock:RockBoundField DataField="Text" HeaderText="Request" SortExpression="Text" />
                                <Rock:BadgeField DataField="PrayerCount" HeaderText="Prayer Count" SortExpression="PrayerCount" DangerMin="0" DangerMax="0" SuccessMin="3" />
                                <Rock:BadgeField DataField="FlagCount" HeaderText="Flag Count" SortExpression="FlagCount" DangerMin="4" WarningMin="2" InfoMin="1" InfoMax="2" />
                                <Rock:ToggleField DataField="IsApproved" HeaderText="Approved?" ButtonSizeCssClass="btn-xs" Enabled="True" OnCssClass="btn-success" OnText="Yes" OffText="No" SortExpression="IsApproved" OnCheckedChanged="gPrayerRequests_CheckChanged" />
                                <Rock:DeleteField OnClick="gPrayerRequests_Delete"  />
                            </Columns>
                        </Rock:Grid>
                    </div>
                </div>
            </div>

            

        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
