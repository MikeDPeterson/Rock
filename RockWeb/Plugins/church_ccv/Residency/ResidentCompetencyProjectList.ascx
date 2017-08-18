﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Plugins.church_ccv.Residency.ResidentCompetencyProjectList" Codebehind="ResidentCompetencyProjectList.ascx.cs" %>

<asp:UpdatePanel ID="upList" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa  fa-folder-open-o"></i> Projects</h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">

                    <Rock:Grid ID="gProjectList" runat="server" AllowSorting="true" OnRowSelected="gProjectList_RowSelected" DataKeyNames="Id" RowItemText="Project" DisplayType="Light">
                        <Columns>
                            <global:asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <global:asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                            <global:asp:BoundField DataField="MinAssessmentCount" HeaderText="Assessments Required" SortExpression="MinAssessmentCount" HeaderStyle-CssClass="span1" />
                            <global:asp:BoundField DataField="AssessmentCompleted" HeaderText="Assessments Completed" SortExpression="AssessmentCompleted" HeaderStyle-CssClass="span1" />
                            <Rock:BadgeField DataField="AssessmentRemaining" HeaderText="Assessments Remaining" SortExpression="AssessmentRemaining" WarningMin="1" SuccessMax="0" SuccessMin="0" DangerMin="9999" InfoMin="9999" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        </div>
        
        <asp:HiddenField ID="hfCompetencyPersonId" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
