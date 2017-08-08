<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Store.PackageDetailLava" Codebehind="PackageDetailLava.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Literal ID="lOutput" runat="server"></asp:Literal>

        <asp:Literal ID="lDebug" Visible="false" runat="server"></asp:Literal>

    </ContentTemplate>
</asp:UpdatePanel>
