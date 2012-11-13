using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Swagger.Net.Factories;

namespace Swagger.Net
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocsController : ApiController
    {
        /// <summary>
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get()
        {
            var factory = new ResourceListingFactory();
            var r = factory.CreateResourceListing(ControllerContext);
            return WrapResponse(r);
        }

        public HttpResponseMessage Get(string id)
        {
            var rtn = getDocs(id, this.Request.RequestUri);
            return WrapResponse(rtn);
        }

        private static ResourceListing getDocs(string controllerName, Uri uri)
        {
            var docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();
            ResourceListing r = SwaggerGen.CreateResourceListing(uri, controllerName, true);

            foreach (var api in GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions
                                    .Where(api => api.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName))
            {
                var rApi = CreateResourceApi(api, docProvider);
                r.apis.Add(rApi);
            }

            return r;
        }

        private static ResourceApi CreateResourceApi(ApiDescription api, XmlCommentDocumentationProvider docProvider)
        {
            ResourceApi rApi = SwaggerGen.CreateResourceApi(api);
            ResourceApiOperation rApiOperation = SwaggerGen.CreateResourceApiOperation(api, docProvider);
            rApi.operations.Add(rApiOperation);

            foreach (var param in api.ParameterDescriptions)
            {
                ResourceApiOperationParameter parameter = SwaggerGen.CreateResourceApiOperationParameter(api, param, docProvider);
                rApiOperation.parameters.Add(parameter);
            }
            return rApi;
        }

        private HttpResponseMessage WrapResponse<T>(T resourceListing)
        {
            var formatter = ControllerContext.Configuration.Formatters.JsonFormatter;
            var content = new ObjectContent<T>(resourceListing, formatter);

            var resp = new HttpResponseMessage { Content = content };
            return resp;
        }
    }
}
