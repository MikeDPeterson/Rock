<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TimeCardQuickEntry.ascx.cs" Inherits="RockWeb.Plugins.church_ccv.Hr.TimeCardQuickEntry" %>
<script runat="server">


</script>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hfTimeCardId" runat="server" />
        <asp:Panel ID="pnlInformationMessage" runat="server" Visible="false">
            <asp:Label ID="lblInformationMessage" runat="server" Text=""></asp:Label><br />
        </asp:Panel>

        <asp:Panel ID="pnlQuickEntry" runat="server">
            <asp:Label ID="lblCurrentTimePeriod" runat="server" Text=""></asp:Label><br />
            <asp:Label ID="lblClockedStatus" runat="server" Text=""></asp:Label>
            <asp:LinkButton ID="lbTimeIn" runat="server" CssClass="btn btn-primary btn-block margin-b-sm" OnClick="lbTimeIn_Click">Clock In</asp:LinkButton>
            <asp:LinkButton ID="lbLunchOut" runat="server" CssClass="btn btn-primary btn-block margin-b-sm" OnClick="lbLunchOut_Click" Visible="false">Lunch Out</asp:LinkButton>
            <asp:LinkButton ID="lbLunchIn" runat="server" CssClass="btn btn-primary btn-block margin-b-sm" OnClick="lbLunchIn_Click" Visible="false">Lunch In</asp:LinkButton>
            <asp:LinkButton ID="lbTimeOut" runat="server" CssClass="btn btn-primary btn-block margin-b-sm" OnClick="lbTimeOut_Click" Visible="false">Clock Out</asp:LinkButton>
            <br />
        </asp:Panel>

        <asp:Panel ID="pnlCreateTimeCard" runat="server" Visible="false">
            <asp:Label ID="lblCreateTimeCard" runat="server" Text=""></asp:Label><br />
            <asp:LinkButton ID="lbCreateTimeCard" runat="server" CssClass="btn btn-primary btn-block margin-b-sm" OnClick="lbCreateTimeCard_Click">Create Time Card</asp:LinkButton>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
