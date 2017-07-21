﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PrayerRequestEntryKiosk.ascx.cs" Inherits="RockWeb.Plugins.church_ccv.Prayer.PrayerRequestEntryKiosk" %>
<asp:UpdatePanel ID="upAdd" runat="server">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlForm">

                <header>
                    <h1>Prayer Request</h1>
                </header>
                
                <main>
                    <div class="row">
                        <div class="col-md-8">
                            <asp:ValidationSummary ID="valValidation" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                            <Rock:NotificationBox ID="nbWarningMessage" runat="server" NotificationBoxType="Warning" Title="Warning" Visible="false" />

                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <Rock:DataTextBox ID="dtbFirstName" runat="server" Label="First Name" SourceTypeName="church.ccv.Prayer.Model.CampusPrayerRequest, church.ccv.Prayer" PropertyName="FirstName"/>
                                    </div>
                                    <div class="col-md-6">
                                        <Rock:DataTextBox ID="dtbLastName" runat="server" Label="Last Name" SourceTypeName="church.ccv.Prayer.Model.CampusPrayerRequest, church.ccv.Prayer" PropertyName="LastName" />
                                    </div>
                                </div>

                                <Rock:EmailBox ID="ebEmail" runat="server" Label="Email" SourceTypeName="church.ccv.Prayer.Model.CampusPrayerRequest, church.ccv.Prayer" PropertyName="Email" />
                    
                                <div class="row">
                                    <div class="col-md-6">
                                        <Rock:CampusPicker ID="cpCampus" runat="server" Required="true" Label="Campus" />
                                    </div>
                                    <div class="col-md-6">
                                        <Rock:RockDropDownList ID="bddlCategory" runat="server" Required="true" Label="Category" />
                                    </div>
                                </div>
                                <em ID="lblCount" runat="server" class="pull-right badge"></em>
                                <Rock:DataTextBox ID="dtbRequest" runat="server" Label="Request" TextMode="MultiLine" Rows="3" MaxLength="10" ValidateRequestMode="Disabled" SourceTypeName="church.ccv.Prayer.Model.CampusPrayerRequest, church.ccv.Prayer" PropertyName="Text" placeholder="Please pray that..."></Rock:DataTextBox>

                                <% if ( EnableUrgentFlag ) { %>
                                    <Rock:RockCheckBox ID="cbIsUrgent" runat="server" Checked="false" Label="Urgent?" Text="Yes" Help="If 'yes' is checked the request will be flagged as urgent in need of attention quickly." />
                                <% } %>
                                <% if ( EnableCommentsFlag )
                                    { %>
                                    <Rock:RockCheckBox ID="cbAllowComments" runat="server" Checked="false" Label="Allow Encouraging Comments?" Text="Yes" Help="If 'yes' is checked the prayer team can offer encouraging comments on the request." />
                                <% } %>
                                <% if ( EnablePublicDisplayFlag )
                                    { %>
                                    <Rock:RockCheckBox ID="cbAllowPublicDisplay" runat="server" Checked="false" Label="Allow Publication?" Text="Yes" Help="If you check 'yes' you give permission to show the request on the public website." />
                                <% } %>
                            </fieldset>
                        </div>
                        <div class="col-md-4">
                            <asp:LinkButton ID="lbSave" runat="server" AccessKey="s" Text="Save Request" OnClick="btnSave_Click" CssClass="btn btn-primary btn-kiosk btn-kiosk-lg" CausesValidation="True"/>
                        </div>
                    </div>
                </main>

                <footer>
                    <div class="container">
                        <asp:LinkButton ID="lbPrayerCancel" runat="server" CausesValidation="false" OnClick="lbPrayerCancel_Click" CssClass="btn btn-default btn-kiosk">Cancel</asp:LinkButton>
                    </div>
                </footer>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlReceipt" Visible="False">

            <header>
                <h1>Prayer Request Submitted</h1>
            </header>

            <main>
                <div class="row">
                    <div class="col-md-8">

                        <Rock:NotificationBox ID="nbMessage" runat="server" NotificationBoxType="Success" ></Rock:NotificationBox>

                    </div>
                    <div class="col-md-4">
                        <asp:Button ID="btnDone" runat="server" Text="Done" OnClick="btnDone_Click" CssClass="btn btn-primary btn-kiosk btn-kiosk-lg"/>
                    </div>
                </div>
            </main>

            <footer>
            </footer>

        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
