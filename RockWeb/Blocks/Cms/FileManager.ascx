<%@ Control Language="C#" AutoEventWireup="true" Inherits="RockWeb.Blocks.Cms.FileManager" Codebehind="FileManager.ascx.cs" %>

<asp:UpdatePanel runat="server" ID="upnlContent">
    <ContentTemplate>
        <iframe id="iframeFileBrowser" class="js-file-browser file-browser" runat="server"  style="width: 100%; height: 420px; display:none;" scrolling="no" frameBorder="0" />
        <script>
            
            $(document).ready(function () {
                Sys.Application.add_load(function () {
                    $('.js-file-browser').fadeIn(50);
                });
            });
        </script>
    </ContentTemplate>
</asp:UpdatePanel>

