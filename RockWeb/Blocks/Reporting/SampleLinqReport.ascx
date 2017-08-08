<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Reporting.SampleLinqReport" Codebehind="SampleLinqReport.ascx.cs" %>

<asp:UpdatePanel ID="upReport" runat="server">
    <ContentTemplate>



            <div class="panel panel-block">
                <div class="panel-heading">
                    <h1 class="panel-title"><i class="fa fa-exclamation-triangle"></i> Sample Linq Report</h1>
                </div>
                
                <div class="grid">
                    <Rock:Grid ID="gReport" runat="server" AllowSorting="true" EmptyDataText="No Results" />
                </div>
            </div>


    </ContentTemplate>
</asp:UpdatePanel>
