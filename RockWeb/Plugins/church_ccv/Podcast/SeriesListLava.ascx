<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.Podcast.SeriesListLava" Codebehind="SeriesListLava.ascx.cs" %>

<asp:UpdatePanel ID="upnlContent" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
    <ContentTemplate>
        
        <%-- View Panel --%>
        <asp:Panel ID="pnlView" runat="server">
            <asp:Literal ID="lContent" runat="server" Visible="true" />
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
