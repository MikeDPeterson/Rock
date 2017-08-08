<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Prayer.PrayerRequestListLava" Codebehind="PrayerRequestListLava.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Literal ID="lContent" runat="server"></asp:Literal>

        <asp:Literal ID="lDebug" runat="server" Visible="false"></asp:Literal>

    </ContentTemplate>
</asp:UpdatePanel>
