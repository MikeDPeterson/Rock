<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Finance.ContributionStatementLava" Codebehind="ContributionStatementLava.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Literal ID="lResults" runat="server" />

        <asp:Literal ID="lDebug" runat="server" />

    </ContentTemplate>
</asp:UpdatePanel>
