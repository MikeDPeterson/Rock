<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Core.CampusContextSetter" Codebehind="CampusContextSetter.Device.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Literal ID="lOutput" runat="server" />

        <asp:Literal ID="lDebug" runat="server" Visible="false" />

    </ContentTemplate>
</asp:UpdatePanel>
