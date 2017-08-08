<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Cms.PageMenu" Codebehind="PageMenu.ascx.cs" %>
<asp:UpdatePanel ID="upContent" runat="server">
<ContentTemplate>
    <asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
</ContentTemplate>
</asp:UpdatePanel>
