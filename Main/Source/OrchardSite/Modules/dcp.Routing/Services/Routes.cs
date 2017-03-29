using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Alias.Implementation.Holder;
using Orchard.Alias.Implementation.Map;
using Orchard.Mvc.Routes;

namespace dcp.Routing.Services
{

    public class PrefixedRoute : Route
    {
        private readonly IAliasHolder _aliasHolder;
        private readonly IEnumerable<string> _prefixValues;
        private readonly string _prefixName;

        public PrefixedRoute(string prefixName, IEnumerable<string> prefixValues, RouteValueDictionary defaults, RouteValueDictionary constraints, IAliasHolder aliasHolder, IRouteHandler routeHandler)
            : base("{prefix}/{*path}", defaults, constraints, new RouteValueDictionary()
            {
                {"area", "PrefixedRoutes"}
            }, routeHandler)
        {
            _prefixName = prefixName;
            _prefixValues = prefixValues;
            _aliasHolder = aliasHolder;
            defaults.Add("path", "");
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var data = base.GetRouteData(httpContext);

            if (data == null)
                return null;

            var prefix = (string)data.Values["prefix"];
            if (!_prefixValues.Contains(prefix))
                return null;

            var newPath = (string)data.Values["path"];
            var maps = _aliasHolder.GetMaps();

            foreach (var aliasMap in maps)
            {
                var newRouteValues = new Dictionary<string, string>();
                AliasInfo aliasInfo;

                if (!aliasMap.TryGetAlias(newPath, out aliasInfo))
                {
                    continue;
                }
                foreach (var item in aliasInfo.RouteValues)
                {
                    newRouteValues.Add(item.Key, item.Value);
                }                

                data.Values.Remove("area");

                newRouteValues.Merge(data.Values, true);
                data.Values.ReplaceWith(newRouteValues);
                data.Values[_prefixName] = prefix;
                data.DataTokens["area"] = newRouteValues["area"];
                return data;
            }

            return null;
        }

    }

    public class RewriteToAliasRoute : Route
    {
        private readonly IAliasHolder _aliasHolder;
        private readonly IDictionary<string, IEnumerable<string>> _constraintValues;
        private readonly string _rewriteToUrl;

        public RewriteToAliasRoute(string url, string rewriteToUrl, IDictionary<string, IEnumerable<string>> constraintValues, RouteValueDictionary defaults, RouteValueDictionary constraints, IAliasHolder aliasHolder, IRouteHandler routeHandler)
            : base(url, defaults, constraints, new RouteValueDictionary()
            {
                {"area", "RewriteToAlias"}
            }, routeHandler)
        {
            _rewriteToUrl = rewriteToUrl;
            _constraintValues = constraintValues;
            _aliasHolder = aliasHolder;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var data = base.GetRouteData(httpContext);

            if (data == null)
                return null;

            foreach (var key in data.Values.Keys)
            {
                if (!_constraintValues.Keys.Contains(key)) 
                    continue;

                var constrains = _constraintValues[key];
                var value = (string)data.Values[key];
                if (!constrains.Contains(value, StringComparer.OrdinalIgnoreCase))
                    return null;
            }

            var maps = _aliasHolder.GetMaps();

            foreach (var aliasMap in maps)
            {
                var newRouteValues = new Dictionary<string, string>();
                AliasInfo aliasInfo;

                if (!aliasMap.TryGetAlias(_rewriteToUrl, out aliasInfo))
                {
                    continue;
                }

                foreach (var item in aliasInfo.RouteValues)
                {
                    newRouteValues.Add(item.Key, item.Value);
                }   

                data.Values.Remove("area");
                newRouteValues.Merge(data.Values, true);
                data.Values.ReplaceWith(newRouteValues);
                data.DataTokens["area"] = newRouteValues["area"];
                return data;
            }

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var data = base.GetRouteData(requestContext.HttpContext);
            var keysToRemove = values.Keys.Where(x => !data.Values.ContainsKey(x)).ToArray();
            foreach (var key in keysToRemove)
            {
                values.Remove(key);
            }
            return base.GetVirtualPath(requestContext, values);
        }

    }
    
    public class ExtendExistingAliasRoute : Route
    {
        private readonly IAliasHolder _aliasHolder;
        private string _originalUrl;

        public ExtendExistingAliasRoute(
            string url,
            string originalUrl,
            RouteValueDictionary defaults,
            RouteValueDictionary constraints,
            IAliasHolder aliasHolder,
            IRouteHandler routeHandler)
            : base(url, defaults, constraints, new RouteValueDictionary()
            {
                {"area", "dcp.Routing"}
            }, routeHandler)
        {
            _aliasHolder = aliasHolder;
            _originalUrl = originalUrl;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var data = base.GetRouteData(httpContext);

            if (data == null)
                return null;

            var newPath = string.Join("/", _originalUrl);
            var maps = _aliasHolder.GetMaps();


            foreach (var aliasMap in maps)
            {               
                var newRouteValues = new Dictionary<string, string>();
                AliasInfo aliasInfo;

                if (!aliasMap.TryGetAlias(newPath, out aliasInfo))
                {
                    continue;
                }

                foreach (var item in aliasInfo.RouteValues)
                {
                    newRouteValues.Add(item.Key, item.Value);
                }   


                newRouteValues.Merge(data.Values, true);
                data.Values.ReplaceWith(newRouteValues);
                data.DataTokens["area"] = newRouteValues["area"];
                return data;

            }


            return null;
        }
    }

    public class ExtendedAliasRoute : RouteBase, IRouteWithArea
    {
        private readonly string _url;
        private IDictionary<string, string> _routeValues;
        private readonly AliasMap _aliasMap;

        private readonly Route _route;
        private readonly List<string> _keys;
        public ExtendedAliasRoute(string url, IRouteHandler routeHandler, IAliasHolder aliasHolder)
        {
            _route = new Route(url, new RouteValueDictionary
                        {
                            {"httproute", true},
                            {"area", "dcp.Routing"}
                        },
            new RouteValueDictionary(),
            new RouteValueDictionary
            {
                {"area", "dcp.Routing"}
                }, routeHandler);
            _url = url;
            _aliasMap = aliasHolder.GetMap("Contents");
            var matches = Regex.Matches(_url, @"\{(.+?)\}");
            _keys = new List<string>();
            foreach (Match match in matches)
            {
                _keys.Add(match.Groups[1].Value);
            }
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var dataDest = _route.GetRouteData(httpContext);

            if (dataDest == null)
                return null;
            
            AliasInfo aliasInfo;
            if (!_aliasMap.TryGetAlias(_url, out aliasInfo))
                return null;


            _routeValues = aliasInfo.RouteValues;
            

            dataDest.Values.Remove("area");

            var dataSource = new Dictionary<string, string>(_routeValues);
            dataSource.Merge(dataDest.Values, true);
            dataDest.Values.ReplaceWith(dataSource);
            dataDest.DataTokens["area"] = dataSource["area"];
            return dataDest;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {           
            if (values.ContainsKey("area") && !requestContext.RouteData.Values.ContainsValue(values["area"]))
                return null;

            if (values.ContainsKey("controller") && !requestContext.RouteData.Values.ContainsValue(values["controller"]))
                return null;

            if (values.ContainsKey("action") && !requestContext.RouteData.Values.ContainsValue(values["action"]))
                return null;

            if (values.ContainsKey("Id") && !requestContext.RouteData.Values.ContainsValue(values["Id"]))
                return null;

            var excludeRouteKeys = new[] { "area", "action", "controller", "Id", "httproute" };
           
            var filteredRouteValues = values.Where(x => !excludeRouteKeys.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);
            var allRouteValues = new RouteValueDictionary(filteredRouteValues);
            requestContext.RouteData.Values.Where(x => !excludeRouteKeys.Contains(x.Key) && _keys.Contains(x.Key)).ToList().ForEach(x =>
            {
                if (!allRouteValues.ContainsKey(x.Key))
                    allRouteValues.Add(x.Key, x.Value);
            });
            
            var res = _route.GetVirtualPath(requestContext, allRouteValues);
            return res;
            }
            
        public string Area { get { return "Contents"; } }
    }

    
    public class ExtendedAliasRoutes : IRouteProvider
    {
        private readonly IAliasHolder _aliasHolder;
        private readonly IExtendedAliasService _extendedAliasService;

        public ExtendedAliasRoutes(IExtendedAliasService extendedAliasService, IAliasHolder aliasHolder)
        {
            _aliasHolder = aliasHolder;
            _extendedAliasService = extendedAliasService;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var extendedAliasRecords = _extendedAliasService.GetAll();
            var i = 0;
            return extendedAliasRecords.Select(x =>
            {
                i++;
                
                return new RouteDescriptor
                {
                    Name = !string.IsNullOrEmpty(x.RouteName) ? x.RouteName : "ExtendedAlias" + i,
                    Priority = 82,
                    Route = new ExtendedAliasRoute(x.AliasRecord.Path, new MvcRouteHandler(), _aliasHolder)
                };
            }).ToList();
        }
    }

    public static class RouteValuesHelper
    {

        public static RouteValueDictionary Merge(this RouteValueDictionary dest, RouteValueDictionary source, bool forceReplace = false)
        {
            foreach (var routeValue in source)
            {
                if (forceReplace)
                {
                    if (dest.Keys.Any(x => x == routeValue.Key))
                    {
                        dest.Remove(routeValue.Key);
                    }
                }
                if (dest.Keys.All(x => x != routeValue.Key))
                    dest.Add(routeValue.Key, routeValue.Value);
            }

            return dest;
        }

        public static RouteValueDictionary ReplaceWith(this RouteValueDictionary dest, RouteValueDictionary source)
        {
            dest.Clear();
            foreach (var routeValue in source)
            {
                dest.Add(routeValue.Key, routeValue.Value);
            }
            return dest;
        }

        public static IDictionary<string, string> Merge(this IDictionary<string, string> dest, RouteValueDictionary source, bool forceReplace = false)
        {
            foreach (var routeValue in source)
            {
                if (forceReplace)
                {
                    if (dest.Keys.Any(x => x == routeValue.Key))
                    {
                        dest.Remove(routeValue.Key);
                    }
                }
                if (dest.Keys.All(x => x != routeValue.Key))
                    dest.Add(routeValue.Key, routeValue.Value.ToString());
            }

            return dest;
        }

        public static RouteValueDictionary ReplaceWith(this RouteValueDictionary dest, IDictionary<string, string> source)
        {
            dest.Clear();
            foreach (var routeValue in source)
            {
                dest.Add(routeValue.Key, routeValue.Value);
            }
            return dest;
        }

    }
}