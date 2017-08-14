<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Crm.PhotoOptOut" Codebehind="PhotoOptOut.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox runat="server" ID="nbWarning" NotificationBoxType="Warning" Text="No, that's not right. Are you sure you copied that web address correctly?" Visible="false"></Rock:NotificationBox>
        <Rock:NotificationBox runat="server" ID="nbMessage" NotificationBoxType="Success" Text="You've been opted out and should no longer receive photo requests from us." Visible="false"></Rock:NotificationBox>
    </ContentTemplate>
</asp:UpdatePanel>
