(function () {

    
    var config = {
        consumerKey: secrets.oldKey,
        consumerSecret: secrets.oldSecret,
        requestTokenUrl: "https://api.pps.io/v1/OAuth/1A/RequestToken",
        accessTokenUrl: "https://api.pps.io/v1/OAuth/1A/AccessToken",
        authorizeTokenUrl: "https://api.pps.io/v1/OAuth/1A/AuthorizeToken", // not implemented.  returns nil
        authorize_callback: "",
        version: "1.0",
        signatureMethod: "HMAC-SHA1",
        rootUrl: "https://api.pps.io/v1/"
    };

    window.APP = window.APP || {};

    window.APP.config = config;
})();