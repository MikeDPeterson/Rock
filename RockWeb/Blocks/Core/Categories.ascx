﻿<%@ Control Language="C#" AutoEventWireup="True" Inherits="RockWeb.Blocks.Core.Categories" Codebehind="Categories.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlList" CssClass="panel panel-block" runat="server">


            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-folder-open"></i> Category List</h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:GridFilter ID="rFilter" runat="server" OnDisplayFilterValue="rFilter_DisplayFilterValue">
                        <Rock:EntityTypePicker ID="entityTypeFilter" runat="server" Required="false" Label="Entity Type" IncludeGlobalOption="true" />
                    </Rock:GridFilter>
                    <Rock:Grid ID="gCategories" runat="server" RowItemText="Category" TooltipField="Description">
                        <Columns>
                            <Rock:ReorderField />
                            <Rock:RockBoundField DataField="Name" HeaderText="Category" />
                            <Rock:RockBoundField DataField="IconCssClass" HeaderText="Icon Class" />
                            <Rock:RockBoundField DataField="ChildCount" HeaderText="Child Categories" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                            <Rock:RockBoundField DataField="EntityType" HeaderText="Entity Type" SortExpression="EntityType" />
                            <Rock:RockBoundField DataField="EntityQualifierField" HeaderText="Entity Qualifier Field" SortExpression="EntityQualifierField" />
                            <Rock:RockBoundField DataField="EntityQualifierValue" HeaderText="Entity Qualifier Value" SortExpression="EntityQualifierValue" />
                            <Rock:EditField OnClick="gCategories_Edit"/>
                            <Rock:SecurityField />
                            <Rock:DeleteField OnClick="gCategories_Delete" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        </asp:Panel>

        <Rock:ModalDialog ID="mdDetails" runat="server" Title="Category" ValidationGroup="EntityTypeName">
            <Content>

                <global::asp:HiddenField ID="hfIdValue" runat="server" />

                <global::asp:Panel ID="pnlEntityInfo" runat="server" CssClass="well">
                    <Rock:EntityTypePicker ID="entityTypePicker" runat="server" Label="Entity Type" Required="true" />

                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbEntityQualifierField" runat="server" Label="Qualifier Field" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockTextBox ID="tbEntityQualifierValue" runat="server" Label="Qualifier Value" />
                        </div>
                    </div>
                </Panel>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:DataTextBox ID="tbName" runat="server" SourceTypeName="Rock.Model.Category, Rock" PropertyName="Name" Required="true" />
                    </div>
                    <div class="col-md-6">
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <Rock:DataTextBox ID="tbDescription" runat="server" SourceTypeName="Rock.Model.Category, Rock" PropertyName="Description" TextMode="MultiLine" Rows="3" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <Rock:CategoryPicker ID="catpParentCategory" runat="server" Label="Parent Category" />
                        <asp:PlaceHolder ID="phAttributes" runat="server" EnableViewState="false"></asp:PlaceHolder>
                    </div>
                    <div class="col-md-6">
                        <Rock:DataTextBox ID="tbIconCssClass" runat="server" SourceTypeName="Rock.Model.Category, Rock" PropertyName="IconCssClass" />
                        <Rock:DataTextBox ID="tbHighlightColor" runat="server" SourceTypeName="Rock.Model.Category, Rock" PropertyName="HighlightColor" />
                    </div>

                </div>

            </Content>
        </Rock:ModalDialog>

        <Rock:NotificationBox ID="nbMessage" runat="server" Title="Error" NotificationBoxType="Danger" Visible="false" />

    </ContentTemplate>
</asp:UpdatePanel>
