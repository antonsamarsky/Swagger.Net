using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using System.Web.Http.Routing;
using Swagger.Net.Factories;
using Swagger.Net.Models;

namespace Swagger.Net
{
    /// <summary>
    /// Determines if any request hit the Swagger route. Moves on if not, otherwise responds with JSON Swagger spec doc
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class SwaggerActionFilterAttribute : ActionFilterAttribute
    {
        private ResourceMetadataFactory _factory;
        public ResourceMetadataFactory Factory
        {
            get { return _factory ?? (_factory = new ResourceMetadataFactory()); }
            set { _factory = value; }
        }

        // Intercept all request and handle swagger requests
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!Helper.IsDocRequest(actionContext.ControllerContext.RouteData))
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            // Arrange
            var rootUrl = actionContext.Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var ctlrName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

            // Act
            var docs = this.Factory.GetDocs(rootUrl, ctlrName);

            // Answer
            var formatter = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter;
            var response = Helper.WrapResponse(formatter, docs);

            actionContext.Response = response;

        }

        private static class Helper
        {

            public static bool IsDocRequest(IHttpRouteData routeData)
            {
                var containsSwaggerKey = routeData.Values.ContainsKey(G.SWAGGER);
                return containsSwaggerKey;
            }

            public static HttpResponseMessage WrapResponse(JsonMediaTypeFormatter formatter, ResourceDescription docs)
            {
                var responseContent = new ObjectContent<ResourceDescription>(docs, formatter);
                var response = new HttpResponseMessage { Content = responseContent };
                return response;
            }

        }
    }
}
