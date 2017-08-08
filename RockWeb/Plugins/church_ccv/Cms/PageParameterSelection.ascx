<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.Cms.PageParameterSelection" Codebehind="PageParameterSelection.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:RockDropDownList ID="ddlSelection" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSelection_SelectedIndexChanged" />
    </ContentTemplate>
</asp:UpdatePanel>