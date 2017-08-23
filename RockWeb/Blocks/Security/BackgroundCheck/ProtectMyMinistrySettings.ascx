﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Security.BackgroundCheck.ProtectMyMinistrySettings" Codebehind="ProtectMyMinistrySettings.ascx.cs" %>

<script type="text/javascript">
    function clearActiveDialog() {
        $('#<%=hfActiveDialog.ClientID %>').val('');
    }
</script>

<asp:UpdatePanel ID="upnlRestKeys" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-shield"></i> Protect My Ministry</h1>
                <div class="panel-labels">
                    <Rock:HighlightLabel ID="hlMode" runat="server" LabelType="Type" />
                    <Rock:HighlightLabel ID="hlActive" runat="server" />
                </div>

            </div>

            <div class="panel-body">

                <asp:ValidationSummary ID="valSummary" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                <Rock:NotificationBox ID="nbNotification" runat="server" Title="Please Correct the Following" NotificationBoxType="Danger" Visible="false" />

                <div id="pnlNew" runat="server" class="row">
                    <div class="col-md-6">
                        <asp:Image ID="imgPromotion" runat="server" CssClass="img-responsive" />
                        <div class="actions">
                            <asp:HyperLink ID="hlGetStarted" runat="server" Text="Register For An Account" CssClass="btn btn-primary btn-block margin-b-lg" />
                        </div>
                    </div>
                    <div class="col-md-1">
                    </div>
                    <div class="col-md-5">
                        <Rock:RockTextBox ID="tbUserNameNew" runat="server" Label="Username" Required="true" />
                        <Rock:RockTextBox ID="tbPasswordNew" runat="server" Label="Password" Required="true" TextMode="Password" />
                        <div class="actions">
                            <asp:LinkButton ID="lbSaveNew" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="lbSaveNew_Click" />
                        </div>
                    </div>
                </div>

                <div id="pnlViewDetails" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockLiteral ID="lUserName" runat="server" Label="Username" />
                            <Rock:RockLiteral ID="lPassword" runat="server" Label="Password" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockLiteral ID="lPackages" runat="server" Label="Enabled Background Check Types" />
                        </div>
                    </div>
                    <div class="actions">
                        <asp:LinkButton ID="lbEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="lbEdit_Click" />
                    </div>
                </div>

                <div id="pnlEditDetails" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbUserName" runat="server" Label="Username" Required="true" />
                            <Rock:RockTextBox ID="tbPassword" runat="server" Label="Password" Required="true" TextMode="Password" />
                        </div>
                        <div class="col-md-6">
                            <Rock:UrlLinkBox ID="urlWebHook" runat="server" Label="Result Webhook" Required="true"
                                Help="The URL that Protect My Ministry should use when sending background check results back to your server." />
                            <Rock:RockCheckBox id="cbActive" runat="server" Label="Active" />
                            <Rock:RockCheckBox ID="cbTestMode" runat="server" Label="Test Mode" />
                        </div>
                    </div>
                    <div class="actions">
                        <asp:LinkButton ID="lbSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="lbSave_Click" />
                        <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="lbCancel_Click" />
                    </div>
                </div>

            </div>

        </div>

        <asp:Panel ID="pnlPackages" CssClass="panel panel-block" runat="server" Visible="true">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-archive"></i> Background Check Types</h1>
            </div>
            <div class="panel-body">

                <Rock:ModalAlert ID="mdGridWarningValues" runat="server" />
                <div class="grid grid-panel">
                    <Rock:Grid ID="gDefinedValues" runat="server" AllowPaging="true" DisplayType="Full" RowItemText="Type" OnRowSelected="gDefinedValues_RowSelected" AllowSorting="False" >
                        <Columns>
                            <Rock:ReorderField/>
                            <Rock:RockTemplateField HeaderText="Type" SortExpression="Name">
                                <ItemTemplate>
                                    <%# Eval("Value") %><br /><small><%# Eval("Description") %></small>
                                </ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:RockBoundField DataField="PackageName" HeaderText="Package Name"/>
                            <Rock:RockBoundField DataField="DefaultCounty" HeaderText="County Criminal Default County"/>
                            <Rock:BoolField DataField="SendAddressCounty" HeaderText="Use Home Address for County Criminal" />
                            <Rock:RockBoundField DataField="DefaultState" HeaderText="Statewide Criminal Default State"/>
                            <Rock:BoolField DataField="SendAddressState" HeaderText="Use Home State for Statewide Criminal" />
                            <Rock:DefinedValueField DataField="MVRJurisdication" HeaderText="MVR Jurisdication Code" />
                            <Rock:BoolField DataField="SendAddressStateMVR" HeaderText="Use Home State for MVR Search" />
                            <Rock:DeleteField OnClick="gDefinedValues_Delete" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
                                        
        </asp:Panel>

        <asp:HiddenField ID="hfActiveDialog" runat="server" />

        <Rock:ModalDialog ID="dlgPackage" runat="server" Title="Background Check Type" ValidationGroup="Package" 
            OnSaveClick="dlgPackage_SaveClick" OnCancelScript="clearActiveDialog();">
            <Content>

                <global:asp:HiddenField ID="hfDefinedValueId" runat="server" />
                <global:asp:ValidationSummary ID="valSummaryPackage" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" ValidationGroup="Package" />

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockTextBox ID="tbTitle" runat="server" Label="Background Check Type" Required="true" ValidationGroup="Package" />
                    </div>
                    <div class="col-md-6">
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <Rock:RockTextBox ID="tbDescription" runat="server" Label="Description" ValidationGroup="Package" TextMode="MultiLine" Rows="2" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockTextBox ID="tbPackageName" runat="server" Label="Package Name" ValidationGroup="Package" Help="The exact package name to use when sending the background check request to Protect My Ministry" />
                    </div>
                    <div class="col-md-6">
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockTextBox ID="tbDefaultCounty" runat="server" Label="County Criminal Search Default County" ValidationGroup="Package" Help="Include a valid county name to request a County Criminal Search (a default county or state is required when using the PLUS package)." />
                    </div>
                    <div class="col-md-6">
                        <Rock:RockCheckBox ID="cbSendCounty" runat="server" Label="Use Home Address County" Text="Yes" ValidationGroup="Package" Help="If the person's home address includes a county, use that instead of the default county." />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockTextBox ID="tbDefaultState" runat="server" Label="Statewide Criminal Search Default State" ValidationGroup="Package" Help="Include a valid state to request a Statewide Criminal Search (a default state or county is required when using the PLUS package)." />
                    </div>
                    <div class="col-md-6">
                        <Rock:RockCheckBox ID="cbSendState" runat="server" Label="Use Home Address State" Text="Yes" ValidationGroup="Package" Help="If the person's home address includes a state, use that instead of the default state." />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockDropDownList ID="ddlMVRJurisdication" runat="server" Label="MVR Jurisdication Code" ValidationGroup="Package" Help="Select an MVR Jurisdiction code to request a Motor Vehicle Record search." />
                    </div>
                    <div class="col-md-6">
                        <Rock:RockCheckBox ID="cbSendStateMVR" runat="server" Label="Use Home Address State" Text="Yes" ValidationGroup="Package" Help="If the person's home address includes a state, use that instead of state from jurisdication code." />
                    </div>
                </div>

            </Content>
        </Rock:ModalDialog>

    </ContentTemplate>
</asp:UpdatePanel>