<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Cms.RSSFeedItem" Codebehind="RSSFeedItem.ascx.cs" %>
<asp:UpdatePanel ID="upContent" runat="server" >
    <ContentTemplate>
        <Rock:NotificationBox ID="nbRssItem" runat="server" NotificationBoxType="Info" />
        <asp:Panel ID="pnlContent" runat="server" Visible="false">
            <asp:PlaceHolder ID="phFeedItem" runat="server" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
