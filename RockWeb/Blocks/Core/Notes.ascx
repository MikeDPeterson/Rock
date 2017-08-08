<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Core.Notes" Codebehind="Notes.ascx.cs" %>

<asp:UpdatePanel ID="upNotes" runat="server">
    <ContentTemplate>
        <Rock:NoteContainer ID="notesTimeline" runat="server"/>
    </ContentTemplate>
</asp:UpdatePanel>
