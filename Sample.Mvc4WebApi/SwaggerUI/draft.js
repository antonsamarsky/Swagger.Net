jsOAuth = {
    Request: function () { return new global.XMLHttpRequest(); },
    request: function (options) {
        var xhr = Request();
        // copy options and oauth header
        xhr.send(query);
    },
    get: function (url, success, failure) {
        this.request({ 'url': url, 'success': success, 'failure': failure });
    },
    post: function (url, data, success, failure) {
        this.request({ 'method': 'POST', 'url': url, 'data': data, 'success': success, 'failure': failure });
    }

};

swaggerUI = {
    submitOperation: function () {
        var obj = {
            type: this.model.httpMethod,
            url: invocationUrl,
            headers: headerParams,
            data: bodyParam,
            dataType: 'json',
            error: function (xhr, textStatus, error) {
                return _this.showErrorStatus(xhr, textStatus, error);
            },
            success: function (data) {
                return _this.showResponse(data);
            },
            complete: function (data) {
                return _this.showCompleteStatus(data);
            }
        };
        if (obj.type.toLowerCase() === "post" || obj.type.toLowerCase() === "put") {
            obj.contentType = "application/json";
        }
        
        return jQuery.ajax(obj);
    }
}