using Orchard.Environment;
using System.Web.Http;
using System.Web.Mvc;
using System;
using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Routing;
using System.Linq;
using Orchard.WebApi.Routes;
using System.Web.Http.Routing;
using Orchard;

namespace dcp.WebApiHelpPage.Areas.HelpPage
{

    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var moduleName = "dcp.WebApiHelpPage";
            return new[]
            {
                new RouteDescriptor
                {
                    Name = "HelpPage_Default",
                    Route = new Route(
                        "Help/{action}/{apiId}",
                        new RouteValueDictionary {
                            {"area", moduleName}, // this is the name of your module
                            {"controller", "Help"},
                            {"action", "Index"},
                            { "apiId", UrlParameter.Optional }
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", moduleName} // this is the name of your module
                        },
                        new MvcRouteHandler()
                        )
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }

    public class HelpPageAreaRegistration : IOrchardShellEvents
    {
        private readonly List<HttpRouteDescriptor> _routes;
        private readonly IWorkContextAccessor _workContextAccessor;

        public static HttpConfiguration Configuration { get; private set; }

        public HelpPageAreaRegistration(IEnumerable<IHttpRouteProvider> routeProviders, IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            _routes = routeProviders.SelectMany(x => x.GetRoutes()).Cast<HttpRouteDescriptor>().ToList();
            Configuration = new HttpConfiguration();

            var i = 0;
            _routes.ForEach(x =>
            {
                Configuration.Routes.MapHttpRoute(
                    name: "DefaultApi" + i,
                    routeTemplate: x.RouteTemplate,
                    defaults: x.Defaults
                );
                i++;
            });

        }

        public void Activated()
        {
            HelpPageConfig.Register(Configuration, _workContextAccessor.GetContext().HttpContext);
            var apiDescriptions = Configuration.Services.GetApiExplorer().ApiDescriptions;

            //fix relative path to set as Orchard
            //foreach (var d in apiDescriptions)
            //{
            //    var moduleName = d.ActionDescriptor.ControllerDescriptor.ControllerType.Assembly.FullName.Split(',')[0];

            //    var route = _routes.Where(x =>
            //    {
            //        var defaults = HtmlHelper.AnonymousObjectToHtmlAttributes(x.Defaults);

            //        return defaults["area"] != null && (string)defaults["area"] == moduleName;
            //    }).FirstOrDefault();

            //    if (route == null)
            //        continue;

            //    var i = route.RouteTemplate.IndexOf("{controller}");
            //    if (i < 0)
            //        i = route.RouteTemplate.IndexOf("{id}");
            //    if (i >= 0)
            //        d.RelativePath = route.RouteTemplate.Substring(0, i) + d.RelativePath;
            //}

            //remove unwanted
            var incorrectApiDescription = apiDescriptions.Where(x =>
            {
                var moduleName = x.ActionDescriptor.ControllerDescriptor.ControllerType.Assembly.FullName.Split(',')[0];

                return (string)x.Route.Defaults["area"] != moduleName;
            }).ToList();
            incorrectApiDescription.ForEach(x => apiDescriptions.Remove(x));
        }

        public void Terminating()
        {
            
        }
    }
}