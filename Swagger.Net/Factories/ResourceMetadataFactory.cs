﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public class ResourceMetadataFactory
    {

        #region --- fields & ctors ---

        private string _appVirtualPath;
        private XmlCommentDocumentationProvider _docProvider;
        private readonly ParameterMetadataFactory _parameterFactory;

        public ResourceMetadataFactory()
        {
            var path = HttpRuntime.AppDomainAppVirtualPath;
            var docProvider = (IDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
            _parameterFactory = new ParameterMetadataFactory();
            initialize(path, docProvider);
        }

        public ResourceMetadataFactory(string virtualPath, XmlCommentDocumentationProvider docProvider, ParameterMetadataFactory parameterFactory)
        {
            _parameterFactory = parameterFactory;
            initialize(virtualPath, docProvider);
        }

        public void initialize(string appVirtualPath, IDocumentationProvider docProvider)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
            _docProvider = (XmlCommentDocumentationProvider)docProvider;
        }

        #endregion --- fields & ctors ---

        public ResourceDescription CreateResourceMetadata(Uri uri, string controllerName)
        {

            var rtnResource = new ResourceDescription()
            {
                apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(),
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath,
                resourcePath = controllerName
            };

            return rtnResource;
        }

        public IList<Api> CreateApiElements(string controllerName, IEnumerable<ApiDescription> apiDescs)
        {
            var filteredDescs = apiDescs
                .Where(d => d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName)           // current controller
                .Where(d => !(d.Route.Defaults.ContainsKey(G.SWAGGER)));                         // and not swagger doc meta route '/api/docs/...'

            var apis = filteredDescs.Select(apiDesc => GetApiMetadata(apiDesc));

            return apis.ToList();
        }

        public void PopulateApiAndModelElements(string controllerName, IEnumerable<ApiDescription> apiDescriptions, ResourceDescription docs)
        {
            var filteredDescs = apiDescriptions
                .Where(d => d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName)           // current controller
                .Where(d => !(d.Route.Defaults.ContainsKey(G.SWAGGER)));                         // and not swagger doc meta route '/api/docs/...'

            filteredDescs.ToList().ForEach(apiDesc => PopulateDocs(apiDesc, docs));
        }

        private void PopulateDocs(ApiDescription apiDesc, ResourceDescription docs)
        {
            var api = CreateApiRoot(apiDesc);
            var myModels = new Dictionary<Type, Model>();

            var operations = CreateOperationRoot(apiDesc);
            foreach (var op in operations)
            {
                var parameters = _parameterFactory.CreateParameters(apiDesc.ParameterDescriptions, apiDesc.RelativePath);
                op.parameters.AddRange(parameters);
                foreach (var apiParameterDescription in apiDesc.ParameterDescriptions)
                {
                    var type = apiParameterDescription.ParameterDescriptor.ParameterType;
                    var m = GetResourceModel(type);
                    if (docs.models.All(x => x.Name != m.Name))
                    {
                        docs.models.Add(m);
                    }
                }
            }

            api.operations.AddRange(operations);
            docs.apis.Add(api);

        }

        /// <summary>
        /// Create ApiRoot
        ///     Add Operations
        ///         Add Parameters
        /// </summary>
        private Api GetApiMetadata(ApiDescription apiDesc)
        {
            var api = CreateApiRoot(apiDesc);

            var operations = CreateOperationRoot(apiDesc);
            foreach (var op in operations)
            {
                var parameters = _parameterFactory.CreateParameters(apiDesc.ParameterDescriptions, apiDesc.RelativePath);
                op.parameters.AddRange(parameters);
            }
            api.operations.AddRange(operations);

            return api;
        }

        public Api CreateApiRoot(ApiDescription desc)
        {
            var api = new Api()
            {
                path = "/" + desc.RelativePath,
                description = desc.Documentation
            };

            return api;
        }

        public IList<Operation> CreateOperationRoot(ApiDescription apiDesc)
        {

            var responseClass = CalculateResponseClass(apiDesc.ActionDescriptor.ReturnType);
            var remarks = _docProvider.GetRemarks(apiDesc.ActionDescriptor);

            var rApiOperation = new Operation()
            {
                httpMethod = apiDesc.HttpMethod.ToString(),
                nickname = apiDesc.ActionDescriptor.ActionName,
                responseClass = responseClass,
                summary = apiDesc.Documentation,
                notes = remarks,
            };

            return new List<Operation>() { rApiOperation };
        }

        private static string CalculateResponseClass(Type returnType)
        {
            return returnType == null ? "void" : returnType.Name;
        }

        public Model GetResourceModel(Type type)
        {
            return _docProvider.GetApiModel(type);
        }

        public IEnumerable<Model> GetResourceModels(IEnumerable<Type> paramTypes)
        {
            return paramTypes.Select(type => GetResourceModel(type));
        }


    }
}

