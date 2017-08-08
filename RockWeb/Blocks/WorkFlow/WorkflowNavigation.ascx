<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.WorkFlow.WorkflowNavigation" Codebehind="WorkflowNavigation.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-gears"></i> Workflow Entry</h1>
            </div>
            <div class="panel-body">
                <asp:Panel id="pnlContent" runat="server" CssClass="panel-group" />
            </div>
        </div>
        

    </ContentTemplate>
</asp:UpdatePanel>
