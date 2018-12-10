<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialNetworkContent.ascx.cs" Inherits="eStore.UI.Modules.SocialNetworkContent" %>
<asp:Literal ID="lContent" runat="server"></asp:Literal>
<script type="text/javascript" language="javascript">
    window.fbAsyncInit = function () {
        FB.init({
            //appId: 'YOUR_APP_ID', // App ID
            //channelURL: '//WWW.YOUR_DOMAIN.COM/channel.html', // Channel File
            status: true, // check login status
            cookie: true, // enable cookies to allow the server to access the session
            oauth: true, // enable OAuth 2.0
            xfbml: true  // parse XFBML
        });

        // Additional initialization code here
    };

    // Load the SDK Asynchronously
    $(document).ready(function () {
        (function (d) {
            var js, id = 'facebook-jssdk'; if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "http://connect.facebook.net/en_US/all.js";
            d.getElementsByTagName('head')[0].appendChild(js);
        } (document));

        (function (d) {
            var js, id = 'twitter-jssdk'; if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "http://platform.twitter.com/widgets.js";
            d.getElementsByTagName('head')[0].appendChild(js);
        } (document));

        window.___gcfg = { lang: 'en-US' };
        (function () {
            var po = document.createElement('script');
            po.type = 'text/javascript';
            po.async = true;
            po.src = 'https://apis.google.com/js/plusone.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
        })();
    });
</script>

