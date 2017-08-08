<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Cms.RSSFeed" Codebehind="RSSFeed.ascx.cs" %>
<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox ID="nbRSSFeed" runat="server" NotificationBoxType="Info" Visible="false" />
        <asp:Panel ID="pnlContent" runat="server" Visible="false">
            <asp:PlaceHolder ID="phRSSFeed" runat="server" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
