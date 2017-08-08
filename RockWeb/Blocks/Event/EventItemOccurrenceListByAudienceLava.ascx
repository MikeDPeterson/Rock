<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Event.EventItemOccurrenceListByAudienceLava" Codebehind="EventItemOccurrenceListByAudienceLava.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Literal ID="lMessages" runat="server" />

        <asp:Literal ID="lContent" runat="server" />

        <asp:Literal ID="lDebug" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
