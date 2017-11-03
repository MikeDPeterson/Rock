﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LoginWrapper.ascx.cs" Inherits="RockWeb.Plugins.church_ccv.Core.LoginWrapper" %>

<div id="divProfilePhoto" runat="server" class="profile-photo"></div>   
<ul class="loginstatus"> 
    
    <li class="dropdown" ID="liDropdown" runat="server">
        
        <a class="masthead-navitem dropdown-toggle navbar-link loginstatus" href="#" data-toggle="dropdown">
        
            <asp:PlaceHolder ID="phHello" runat="server"><asp:Literal ID="lHello" runat="server" /></asp:PlaceHolder>
            <b class="fa fa-caret-down"></b>
        </a>

        <ul class="dropdown-menu">
            <asp:PlaceHolder ID="phMyAccount" runat="server">
                <li>
                    <asp:HyperLink ID="hlMyAccount" runat="server" Text="My Account" />
                </li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phMySettings" runat="server">
                <li>
                    <asp:HyperLink ID="hlMySettings" runat="server" Text="My Settings" />
                </li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phMyProfile" runat="server">
                <li>
                    <asp:HyperLink ID="hlMyProfile" runat="server" Text="My Profile" />
                </li>
            </asp:PlaceHolder>
            <asp:Literal ID="lDropdownItems" runat="server" />

            <li><asp:LinkButton ID="lbLogout" runat="server" OnClick="LoginStatus_lbLogout_Click" CausesValidation="false"></asp:LinkButton></li>
        </ul>

    </li>
    <li ID="liLogin" runat="server" Visible="false"><asp:LinkButton CssClass="masthead-navitem" ID="lbLogin" runat="server" OnClientClick="displayLoginPanel(); return false;" CausesValidation="false" Text="Login">
    <asp:PlaceHolder ID="phNewAccount" runat="server" Visible="false" ><asp:HyperLink ID="hlNewAccount" CssClass="masthead-navitem" runat="server" Text="Create Account" /></asp:PlaceHolder></asp:LinkButton></li>
</ul>

<%-- LOGIN MODAL PANEL--%>
<asp:Panel ID="mdLoginShell" runat="server">
    <div ID="bg-screen" class="bg-screen-hidden">
        <div ID="login-main-panel" class="login-main-panel-hidden">
            <div class="loader-bg loader-bg-hidden">
                <div class="loader loader-hidden"></div>
            </div>

            <div ID="login-inner-panel">
			    <div ID="login-secondary-inner-panel">
                    <div class="row">
                        <div id="divOrgLogin" runat="server" class="col-sm-12">
                            <div>
                                <h1 class="login-form-title text-center">Log In</h1>
                                <p style="margin-top: -.75em; margin-bottom: 3em;" class="small-paragraph-bold">Fill out the form below to securely access your account.</p>
                            </div>
                        
                            <div id="login-form-result-panel" style="visibility: hidden;" class="">
                                <p id="login-form-result-message"></p>
                            </div>

                            <div class="col-md-6 col-md-offset-3 col-sm-8 col-sm-offset-2 col-xs-12">    
                                <div class="form-group rock-text-box login-form-label">
                                    <label class="control-label" for="tbUserName">Username</label>
                                    <input class="form-control" name="tbUserName" type="text" id="tbUserName">
                                </div>

                            
                                <div style="margin: 25px 0 25px 0;"></div>

                                <div class="form-group rock-text-box login-form-label">
                                    <label class="control-label" for="tbPassword">Password</label>
                                    <input class="form-control" name="tbPassword" type="password" id="tbPassword">
                                </div>
                            
                                <div class="checkbox ">
				                    <label><input id="cbRememberMe" type="checkbox" name="cbRememberMe">Remember Me</label>
			                    </div>

                                <div class="row v-center" style="margin: 25px 0 25px 0;">
                                    <div class="col-md-4 col-sm-4 col-xs-6" style="padding: 0;">
                                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="login-form-button btn btn-primary" OnClientClick="return tryLogin();"/>
                                    </div>

                                    <div class="col-md-8 col-sm-4 col-xs-6">
                                        <asp:Button ID="btnHelp" runat="server" Text="Forgot username or password?" CssClass="small-paragraph login-form-forgot" OnClick="LoginModal_btnHelp_Click" CausesValidation="false" />
                                    </div>
                                </div>
                                
                                <asp:Button ID="btnNewAccount" runat="server" Text="Create Account" CssClass="login-form-register btn btn-action" OnClick="LoginModal_btnNewAccount_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

<script type="text/javascript">
   $(document).keydown(function(e) {
		// ESCAPE key pressed
		if (e.keyCode == 27) {
			hideLoginPanel();
		}
	});
	
	var modal = document.querySelector('#bg-screen');
		modal.addEventListener('click', function(e) {
		hideLoginPanel();
	}, false);

	modal.children[0].addEventListener('click', function(e) {
		e.stopPropagation();
	}, false);
	
	function hideLoginPanel() {

	    // fade OUT the bg screen (the grey overlay)
	    var bgScreen = $("#bg-screen");
	    bgScreen.removeClass("bg-screen-visible");
	    bgScreen.addClass("bg-screen-hidden");

	    // fly OUT the actual login panel
	    var loginPanel = $("#login-main-panel");
	    loginPanel.removeClass("login-main-panel-visible");
	    loginPanel.addClass("login-main-panel-hidden");
	}

    function displayLoginPanel() {

        // fade in the bg screen (the grey overlay)
        var bgScreen = $("#bg-screen");
        bgScreen.removeClass("bg-screen-hidden");
        bgScreen.addClass("bg-screen-visible");

        // fly in the actual login panel
        var loginPanel = $("#login-main-panel");
        loginPanel.removeClass("login-main-panel-hidden");
        loginPanel.addClass("login-main-panel-visible");

        // make sure the loader is hidden
        hideLoader();
    }

    function tryLogin() {

        // hide the response panel
        hideResponsePanel();

        // show the spinner
        displayLoader();

        // get the inputted username / password and whether it's checked or not
        var username = $("#tbUserName").val();
        var password = $("#tbPassword").val();
        var rememberMe = $("#cbRememberMe").is(":checked");

        // attempt to login
        var xmlRequest = new XMLHttpRequest();
        xmlRequest.onreadystatechange = function () { if (this.readyState == 4 && this.status == 200) { return handleLoginResponse(this); } }
        xmlRequest.open("POST", "/api/Web/Login?username=" + username + "&password=" + password + "&persist=" + rememberMe, true);
        xmlRequest.send();

        return false;
    }

    function handleLoginResponse(xmlRequest) {

        hideLoader();

        switch (xmlRequest.responseText) {
            case "Success":
            {
                handleLoginSucceeded();
                break;
            }
            
            case "Invalid":
            {
                showResponsePanel("Invalid username or password");
                break;
            }

            case "LockedOut":
            {
                showResponsePanel("<%=LoginModal_GetLockedOutCaption( ) %>");
                break;
            }

            case "Confirm":
            {
                showResponsePanel("<%=LoginModal_GetConfirmCaption( ) %>"); 
                
                var confirmationUrl = "<%=LoginModal_GetConfirmationUrl( ) %>";
                var confirmAccountTemplate = "<%=LoginModal_GetConfirmAccountTemplate( ) %>";
                var appUrl = "<%=LoginModal_GetAppUrl( ) %>";
                var themeUrl = "<%=LoginModal_GetThemeUrl( ) %>";
                var username = $("#tbUserName").val();

                // invoke the send email api
                var xmlRequest = new XMLHttpRequest();
                xmlRequest.onreadystatechange = function () { if (this.readyState == 4 && this.status == 200) { return handleLoginResponse(this); } }
                xmlRequest.open("GET",  "/api/Web/SendConfirmationEmail" +
                                        "?confirmationUrl=" + encodeURI(confirmationUrl) +
                                        "&confirmAccountTemplate=" + confirmAccountTemplate +
                                        "&appRoot=" + encodeURI(appUrl) +
                                        "&themeRoot=" + encodeURI(themeUrl) +
                                        "&username=" + username
                                        , true);
                xmlRequest.send();

                break;
            }

            default: showResponsePanel("An unknown error has occurred"); break;
        }
    }

    function handleLoginSucceeded() {
        // invoke a postback so the server can redirect us if needed
        __doPostBack("btnLogin", "__LOGIN_SUCCEEDED" + ":");
    }

    function hideResponsePanel() {

        var responsePanel = $("#login-form-result-panel");
        responsePanel.removeClass("alert alert-warning block-message margin-t-md");
        responsePanel.css("visibility", "none");
        responsePanel.css("height", "");
        responsePanel.css("margin", "");
        responsePanel.css("padding", "");

        var responseMessage = $("#login-form-result-message");
        responseMessage.css("height", "");
        responseMessage.css("margin", "");
        responseMessage.css("padding", "");
        responseMessage.css("visibility", "none");
        responseMessage.text("");
    }

    function showResponsePanel(errorMsg) {

        // reveal the panel
        var responsePanel = $("#login-form-result-panel");
        responsePanel.css("visibility", "visible");
        responsePanel.addClass( "alert alert-warning block-message margin-t-md" );

        // display the appropriate message
        var responseMessage = $("#login-form-result-message");
        responseMessage.text(errorMsg);
    }
</script>
<%-- END LOGIN MODAL PANEL--%>