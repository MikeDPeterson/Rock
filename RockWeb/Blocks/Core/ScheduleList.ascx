<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Administration.ScheduleList" Codebehind="ScheduleList.ascx.cs" %>

<asp:UpdatePanel ID="upScheduleList" runat="server">
    <ContentTemplate>
        <Rock:ModalAlert ID="mdGridWarning" runat="server" />
        
        <div class="grid">
            <Rock:Grid ID="gSchedules" runat="server" AllowSorting="true" OnRowSelected="gSchedules_Edit">
                <Columns>
                    <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                    <Rock:RockBoundField DataField="CategoryName" HeaderText="Category" SortExpression="CategoryName" />
                    <Rock:DeleteField OnClick="gSchedules_Delete" />
                </Columns>
            </Rock:Grid>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
