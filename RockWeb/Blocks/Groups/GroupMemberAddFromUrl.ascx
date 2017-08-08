<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Groups.GroupMemberAddFromUrl" Codebehind="GroupMemberAddFromUrl.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <div class="alert alert-warning" id="divAlert" runat="server">
            <asp:Literal ID="lAlerts" runat="server" />
        </div>

        <asp:Literal ID="lContent" runat="server" />
        <asp:Literal ID="lDebug" runat="server" />

    </ContentTemplate>
</asp:UpdatePanel>
