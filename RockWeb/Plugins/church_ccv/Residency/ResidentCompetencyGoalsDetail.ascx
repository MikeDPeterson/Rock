<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.Residency.ResidentCompetencyGoalsDetail" Codebehind="ResidentCompetencyGoalsDetail.ascx.cs" %>

<asp:UpdatePanel ID="upDetail" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlDetails" runat="server" Visible="false">
            <p>
                <asp:Literal ID="lblGoals" runat="server" />
            </p>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
