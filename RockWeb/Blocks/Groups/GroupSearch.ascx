<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Groups.GroupSearch" Codebehind="GroupSearch.ascx.cs" %>

<div class="grid">
    <Rock:Grid ID="gGroups" runat="server" EmptyDataText="No Groups Found">
        <Columns>
            <Rock:RockBoundField
                HeaderText="Group"
                DataField="Structure"
                SortExpression="Structure" HtmlEncode="false" />
            <Rock:RockBoundField 
                HeaderText="Type"
                DataField="GroupType" 
                SortExpression="GroupType" />
            <Rock:RockBoundField 
                HeaderText="Member Count"
                ItemStyle-HorizontalAlign="Right"
                DataField="MemberCount" 
                SortExpression="MemberCount"
                DataFormatString="{0:N0}" />
        </Columns>
    </Rock:Grid>
</div>


