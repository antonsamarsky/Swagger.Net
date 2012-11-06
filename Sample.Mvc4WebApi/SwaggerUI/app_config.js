(function () {

    var config = {
        consumerKey: "YOUR_CONSUMER_KEY",
        consumerSecret: "YOUR_CONSUMER_SECRET",
        requestTokenUrl: "https://API.DOMAIN.COM/v1/.../RequestToken",
        accessTokenUrl: "https://API.DOMAIN.COM/v1/.../AccessToken",
        authorizeTokenUrl: "https://API.DOMAIN.COM/v1/.../AuthorizeToken", // not implemented.  returns nil
        authorize_callback: "",
        version: "1.0",
        signatureMethod: "HMAC-SHA1",
        rootUrl: "https://API.DOMAIN.COM/v1/"
    };

    window.APP = window.APP || {};
    window.APP.config = config;
})();

