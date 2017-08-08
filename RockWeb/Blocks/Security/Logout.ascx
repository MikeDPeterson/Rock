<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Security.Logout" Codebehind="Logout.ascx.cs" %>



<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Literal ID="lOutput" runat="server" />

        <asp:LinkButton ID="lbAdminLogout" runat="server" CssClass="btn btn-warning" Text="Logout" Visible="false" OnClick="lbAdminLogout_Click" />

    </ContentTemplate>
</asp:UpdatePanel>