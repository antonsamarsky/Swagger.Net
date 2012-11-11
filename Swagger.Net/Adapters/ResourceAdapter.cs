﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    /// <summary>
    /// | .net              | swagger           |
    /// -----------------------------------------
    /// | EndpointMetadata  | Resource Listing  |
    /// </summary>
    public class ResourceAdapter
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly IEnumerable<ApiDescription> _apiDescriptions;

        public ResourceAdapter()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public ResourceAdapter(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/'); 
            _apiDescriptions = apiDescs;
        }

        #endregion --- fields & ctors ---

        public ResourceListing CreateResourceListing(Uri uri)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
            var apis = CreateResourceElements(_apiDescriptions).ToArray();
            var basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath;

            var rtnListing = new ResourceListing()
            {
                apiVersion = apiVersion,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = basePath,
                apis = apis
            };

            return rtnListing;
        }

        public IList<ResourceListing.Api> CreateResourceElements(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = new Dictionary<String, ResourceListing.Api>();

            foreach (var desc in apiDescs)
            {
                var ctlrName = desc.ActionDescriptor.ControllerDescriptor.ControllerName;

                if (!rtnApis.ContainsKey(ctlrName))
                {
                    var res = new ResourceListing.Api
                    {
                        // todo: this is returning url with query string parameters only if first method has param(s)
                        path = GetPath(desc),
                        description = desc.Documentation
                    };
                    rtnApis.Add(ctlrName, res);
                }
            }

            return rtnApis.Values.ToList();
        }

        private static string GetPath(ApiDescription desc)
        {
            // todo: this
            return "/api/docs/" + desc.ActionDescriptor.ControllerDescriptor.ControllerName;
            //string path;
            //var questionIndex = desc.RelativePath.IndexOf("?");
            //if (questionIndex < 1)
            //{
            //    path = desc.RelativePath;
            //}
            //else
            //{
            //    path = desc.RelativePath.Substring(0, questionIndex);
            //}
            //return path;
        }

    }
}
