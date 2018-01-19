using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Reflection;
using Orchard.Mvc.Routes;
using BDN.Orc.RouteDebug.Models;

namespace BDN.Orc.RouteDebug.Core
{
    public static class RouteHelper
    {
        public static Route CastRoute(RouteBase routeBase)
        {
            var route = routeBase as Route;
            if (route == null)
            {
                // cheat!
                // TODO: Create an interface for self reporting routes.
                var type = routeBase.GetType();
                var property = type.GetProperty("__DebugRoute", BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                {
                    route = property.GetValue(routeBase, null) as Route;
                }
            }
            return route;
        }

        public static List<RouteModel> GetRoutes(IEnumerable<IRouteProvider> routeProviders)
        {
            var routes = new List<RouteModel>();
            foreach (var item in routeProviders)
            {
                foreach (var r in item.GetRoutes())
                {
                    var route = RouteHelper.CastRoute(r.Route);
                    if (route != null)
                    {
                        var feature = route.Defaults["area"] as string;

                        if (string.IsNullOrEmpty(feature))
                            feature = item.GetType().Name;

                        routes.Add(new RouteModel()
                        {
                            Feature = feature,
                            Name = r.Name,
                            Url = route.Url,
                            Defaults = string.Join(", ", route.Defaults.Select(x => string.Format("{0} - {1}", x.Key, x.Value))),
                            Priority = r.Priority,
                            RouteNode = route
                        });
                    }
                }
            }
            return routes.OrderByDescending(x => x.Priority).ToList();
        }
    }
}