<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Cms.Redirect" Codebehind="Redirect.ascx.cs" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox ID="nbAlert" runat="server" NotificationBoxType="Danger" />
    </ContentTemplate>
</asp:UpdatePanel>
