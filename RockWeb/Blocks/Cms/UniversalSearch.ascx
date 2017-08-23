﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Cms.UniversalSearch" Codebehind="UniversalSearch.ascx.cs" %>

<style>
    .model-cannavigate {
        cursor: pointer;
    }
</style>

<asp:UpdatePanel ID="upnlContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block" DefaultButton="btnSearch">
        
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-search"></i> Universal Search</h1>
            </div>
            <div class="panel-body">

                <div class="input-group searchbox">
                    <div class="input-group-addon"><i class="fa fa-search"></i></div>
                    <global:asp:TextBox id="tbSearch" runat="server" CssClass="form-control" Placeholder="Search Rock" />

                    <span id="spanButtonGroup" runat="server" class="input-group-btn">
                        <asp:LinkButton ID="btnSearch" CssClass="btn btn-primary" runat="server" OnClick="btnSearch_Click">Go</asp:LinkButton>
                    </span>
                </div>

                <div class="clearfix margin-t-sm">
                    <asp:LinkButton ID="lbRefineSearch" runat="server" Text="Refine Search" OnClick="lbRefineSearch_Click" CssClass="pull-right" />
                </div>

                <asp:Panel ID="pnlRefineSearch" runat="server" Visible="false">
                    <div class="well margin-t-md">
                        <Rock:RockCheckBoxList ID="cblModelFilter" runat="server" CssClass="js-model-filter" RepeatDirection="Horizontal" Label="Information Types" />

                        <hr />
                        
                        <div class="row">
                            <asp:PlaceHolder ID="phFilters" runat="server" />  
                        </div>
                    </div>
                </asp:Panel>

                <div class="margin-t-lg">
                    <asp:Literal ID="lResults" runat="server" />
                </div>

                <asp:Panel ID="pnlPagination" runat="server" CssClass="text-center">
                     <ul class="pagination">
                        <asp:Literal ID="lPagination" runat="server" />
                    </ul>
                </asp:Panel>
            </div>
                
        </asp:Panel>

        <asp:Panel ID="pnlEditModal" runat="server" Visible="false">
            <Rock:ModalDialog ID="mdEdit" runat="server" OnSaveClick="lbSave_Click" Title="Universal Search Configuration" OnCancelScript="clearDialog();">
                <Content>

                    <asp:UpdatePanel ID="upnlEdit" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockDropDownList ID="ddlSearchType" runat="server" Label="Search Type" />
                                    <Rock:RockTextBox ID="tbResultsPerPage" runat="server" Label="Results Per Page" CssClass="input-width-sm" />
                                    <Rock:RockCheckBox ID="cbShowRefinedSearch" runat="server" Label="Show Refinded Search Options" />
                                    <Rock:RockCheckBox ID="cbShowScores" runat="server" Label="Show Scores" Help="Enables the display of scores for help with debugging." />
                                </div>
                                <div class="col-md-6">
                                    <Rock:RockCheckBoxList ID="cblEnabledModels" runat="server" Label="Enabled Models" />
                                    <Rock:RockCheckBox ID="cbShowFilter" runat="server" Label="Show Model Filter" />
                                    <Rock:RockTextBox ID="tbBaseFieldFilters" runat="server" Label="Base Field Filters" Help="These field filters will always be enabled and will not be changeable by the individual. Uses tha same syntax as the lava command." />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </Content>
            </Rock:ModalDialog>

        </asp:Panel>

        <script>
            Sys.Application.add_load( function () {
                $(".model-cannavigate").click(function () {
                    window.document.location = $(this).data("href");
                });

                $(".js-model-filter input").change(function () {

                    var entityId = $(this).val();
                    var selector = ".js-entity-id-" + entityId + " input";

                    $(selector).prop("checked", $(this).is(':checked'));
                });
            });
        </script>

    </ContentTemplate>
</asp:UpdatePanel>
