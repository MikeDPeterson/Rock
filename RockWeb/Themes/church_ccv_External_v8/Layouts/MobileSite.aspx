﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="Rock.Web.UI.RockPage" %>

<!DOCTYPE html>

<html class="no-js">
<head runat="server">

    <script type="text/javascript">var _sf_startpt=(new Date()).getTime()</script>

    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta charset="utf-8">
    <title></title>

    <script src="<%# ResolveRockUrl("~/Scripts/modernizr.js", true) %>" ></script>
    <script src="<%# ResolveRockUrl("~/Scripts/jquery-1.12.4.min.js", true) %>"></script>

    <!-- Set the viewport width to device width for mobile -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <!-- Included CSS Files -->
    <!-- <link rel="stylesheet" href="<%# ResolveRockUrl("~~/Styles/bootstrap.css", true) %>"/> -->
    <!-- <link rel="stylesheet" href="<%# ResolveRockUrl("~~/Styles/mobilesite-theme.css", true) %>"/> -->

    <link rel="stylesheet" href="/m/styles/main.css">

    <!-- Icons -->
    <link rel="shortcut icon" href="<%# ResolveRockUrl("~~/Assets/Icons/favicon.ico", true) %>">
    <link rel="apple-touch-icon-precomposed" sizes="144x144" href="<%# ResolveRockUrl("~~/Assets/Icons/touch-icon-ipad-retina.png", true) %>">
    <link rel="apple-touch-icon-precomposed" sizes="114x114" href="<%# ResolveRockUrl("~~/Assets/Icons/touch-icon-iphone-retina.png", true) %>">
    <link rel="apple-touch-icon-precomposed" sizes="72x72" href="<%# ResolveRockUrl("~~/Assets/Icons/touch-icon-ipad.png", true) %>">
    <link rel="apple-touch-icon-precomposed" href="<%# ResolveRockUrl("~~/Assets/Icons/touch-icon-iphone.png", true) %>">

</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="sManager" runat="server" />

        <asp:UpdateProgress ID="updateProgress" runat="server">
            <ProgressTemplate>
                <div class="updateprogress-status">
                    <div class="spinner">
                        <div class="rect1"></div>
                        <div class="rect2"></div>
                        <div class="rect3"></div>
                        <div class="rect4"></div>
                        <div class="rect5"></div>
                    </div>
                </div>
                <div class="updateprogress-bg modal-backdrop">
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

        <header class="Masthead">
            <a class="BackButton" onclick="window.history.back()"></a>
            <a class="CCVLogo" href="/m"></a>
        </header>

        <article class="Wrapper">
            <!-- Start Content Area -->
            <Rock:Zone Name="Main" runat="server" />
        </article>

        <div class="Footer">
            <p class="Copyright">Copyright 2017 Christ’s Church of the Valley. All Rights Reserved.</p>
            <p class="ViewFullLink">
            <a href="http://my.ccv.church/landing" class="ccvonline-link">ccv.church full web site</a></p>
        </div>

    </form>

    <script type="text/javascript">
        var _sf_async_config = { uid: 23413, domain: 'my.ccv.church', useCanonical: true };
        (function() {
            function loadChartbeat() {
                window._sf_endpt = (new Date()).getTime();
                var e = document.createElement('script');
                e.setAttribute('language', 'javascript');
                e.setAttribute('type', 'text/javascript');
                e.setAttribute('src','//static.chartbeat.com/js/chartbeat.js');
                document.body.appendChild(e);
            };
            var oldonload = window.onload;
            window.onload = (typeof window.onload != 'function') ?
                loadChartbeat : function() { oldonload(); loadChartbeat(); };
        })();
    </script>

</body>
</html>