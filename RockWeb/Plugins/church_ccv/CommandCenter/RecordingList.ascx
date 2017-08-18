<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.CommandCenter.RecordingList" Codebehind="RecordingList.ascx.cs" %>

<asp:UpdatePanel ID="upRecordings" runat="server">
    <ContentTemplate>

        <Rock:ModalAlert ID="mdGridWarning" runat="server" />

        <Rock:GridFilter ID="rFilter" runat="server">
            <Rock:CampusPicker ID="cpCampus" runat="server" />
            <Rock:DatePicker ID="dtStartDate" runat="server" Label="From Date" />
            <Rock:DatePicker ID="dtEndDate" runat="server" Label="To Date" />
            <Rock:RockTextBox ID="tbStream" runat="server" Label="Stream"></Rock:RockTextBox>
            <Rock:RockTextBox ID="tbVenue" runat="server" Label="Venue"></Rock:RockTextBox>
            <Rock:RockTextBox ID="tbLabel" runat="server" Label="Label"></Rock:RockTextBox>
            <Rock:RockTextBox ID="tbRecording" runat="server" Label="Recording"></Rock:RockTextBox>
        </Rock:GridFilter>

        <Rock:Grid ID="gRecordings" runat="server" EmptyDataText="No Recordings Found" AllowSorting="true" OnRowSelected="gRecordings_Edit">
            <Columns>
                <global::asp:BoundField HeaderText="Campus" DataField="Campus" SortExpression="Campus" />
                <global::asp:BoundField HeaderText="Date" DataField="Date" SortExpression="Date" DataFormatString="{0:MM/dd/yy}" />
                <global::asp:BoundField HeaderText="Stream" DataField="StreamName" SortExpression="StreamName" />
                <global::asp:BoundField HeaderText="Venue" DataField="Venue" SortExpression="Venue" />
                <global::asp:BoundField HeaderText="Label" DataField="Label" SortExpression="Label" />
                <global::asp:BoundField HeaderText="Recording" DataField="RecordingName" SortExpression="RecordingName" />
                <global::asp:BoundField HeaderText="Started" DataField="StartTime" SortExpression="StartTime" DataFormatString="{0:MM/dd/yy hh:mm:ss tt}" />
                <global::asp:BoundField HeaderText="Stopped" DataField="StopTime" SortExpression="StopTime" DataFormatString="{0:MM/dd/yy hh:mm:ss tt}" />
                <global::asp:BoundField HeaderText="Length" DataField="Length" />
                <%--<Rock:LinkButtonField CssClass="btn btn-default btn-sm" OnClick="gRecordings_Start" Text='<i class="fa fa-circle"></i>' />--%> 
                <%--<Rock:LinkButtonField CssClass="btn btn-default btn-sm" OnClick="gRecordings_Stop" Text='<i class="fa fa-stop"></i>' />--%> 
                <Rock:DeleteField OnClick="gRecordings_Delete" />
            </Columns>
        </Rock:Grid>

    </ContentTemplate>
</asp:UpdatePanel>

