<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.Cms.ZoneHider" ViewStateMode="Disabled" Codebehind="ZoneHider.ascx.cs" %>

<asp:UpdatePanel ID="upDetail" runat="server">
    <ContentTemplate>
        <style runat="server" id="sBreadCrumbStyleHidden" visible="false">
            .breadcrumb {
                display: none;
            }
        </style>
    </ContentTemplate>
</asp:UpdatePanel>

