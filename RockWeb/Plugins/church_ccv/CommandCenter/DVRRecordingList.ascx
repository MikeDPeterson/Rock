<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.CommandCenter.DVRRecordingList" Codebehind="DVRRecordingList.ascx.cs" %>

<asp:UpdatePanel ID="upRecordings" runat="server">
    <ContentTemplate>
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-play-circle"></i> DVR Recordings</h1>
            </div>
            <div class="panel-body">
                <div class="grid grid-panel">
                     <Rock:GridFilter ID="rFilter" runat="server">
                        <Rock:DatePicker ID="dtStartDate" runat="server" Label="From Date" />
                        <Rock:DatePicker ID="dtEndDate" runat="server" Label="To Date" />
                        <Rock:CampusPicker ID="cpCampus" runat="server" />
                        <Rock:RockTextBox ID="tbVenue" runat="server" Label="Venue" />
                    </Rock:GridFilter>

                    <Rock:Grid ID="gRecordings" runat="server" EmptyDataText="No DVR Recordings Found" DataKeyNames="WeekendDate,CampusGuid,Venue" AllowSorting="true" OnRowSelected="gRecordings_Edit">
                        <Columns>
                            <Rock:DateField HeaderText="Weekend Date" DataField="WeekendDate" SortExpression="WeekendDate" />
                            <global:asp:BoundField HeaderText="Campus" DataField="Campus" SortExpression="CampusId" />
                            <global:asp:BoundField HeaderText="Venue" DataField="Venue" SortExpression="Venue" />
                            <global:asp:BoundField HeaderText="Recordings" DataField="RecordingCount" SortExpression="RecordingCount" />
                        </Columns>
                    </Rock:Grid>
                </div>
            </div>
        </div>  

    </ContentTemplate>
</asp:UpdatePanel>