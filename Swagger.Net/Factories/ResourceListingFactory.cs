using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Swagger.Net.Factories
{
    public class ResourceListingFactory
    {
        public ResourceListing CreateResourceListing(HttpControllerContext controllerContext)
        {
            ResourceListing r = SwaggerGen.CreateResourceListing(controllerContext);
            List<string> uniqueControllers = new List<string>();

            foreach (var api in GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions)
            {
                string controllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (uniqueControllers.Contains(controllerName)) continue;

                uniqueControllers.Add(controllerName);

                ResourceApi rApi = SwaggerGen.CreateResourceApi(api);
                r.apis.Add(rApi);
            }
            return r;
        }
    }
}
