<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.SampleProject.ReferralAgencyList" Codebehind="ReferralAgencyList.ascx.cs" %>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <Rock:GridFilter ID="gfSettings" runat="server">
            <Rock:CampusPicker ID="cpCampus" runat="server" Label="Campus" />
            <Rock:RockDropDownList ID="ddlAgencyType" runat="server" Label="Agency Type" />
        </Rock:GridFilter>

        <Rock:ModalAlert ID="mdGridWarning" runat="server" />

        <Rock:Grid ID="gAgencies" runat="server" AllowSorting="true" OnRowSelected="gAgencies_Edit" TooltipField="Description">
            <Columns>
                <global:asp:BoundField DataField="Name" HeaderText="Agency Name" SortExpression="Name" />
                <global:asp:BoundField DataField="Campus.Name" HeaderText="Campus" SortExpression="Campus.Name" />
                <global:asp:BoundField DataField="AgencyTypeValue.Value" HeaderText="Type" SortExpression="AgencyTypeValue.Value" />
                <global:asp:BoundField DataField="ContactName" HeaderText="Contact Name" SortExpression="ContactName" />
                <global:asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" SortExpression="PhoneNumber" />
                <global:asp:BoundField DataField="Website" HeaderText="Website" SortExpression="Website" />
                <Rock:DeleteField OnClick="gAgencies_Delete" />
            </Columns>
        </Rock:Grid>

    </ContentTemplate>
</asp:UpdatePanel>