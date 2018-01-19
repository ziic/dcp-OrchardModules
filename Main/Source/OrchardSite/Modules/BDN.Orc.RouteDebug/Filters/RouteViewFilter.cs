using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Filters;
using System.Web.Mvc;
using Orchard;
using Orchard.Security;
using Orchard.DisplayManagement;
using Orchard.UI.Admin;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using Orchard.Environment.Extensions;
using BDN.Orc.RouteDebug.Core;

namespace BDN.Orc.NLog.Filters
{
    [OrchardFeature("BDN Route Debug Widget")]
    public class RouteViewFilter : FilterProvider, IResultFilter, IActionFilter
    {
        private readonly IAuthorizer _authorizer;
        private readonly dynamic _shapeFactory;
        private readonly WorkContext _workContext;
        private readonly IEnumerable<IRouteProvider> _routeProviders;

        public RouteViewFilter(WorkContext workContext, IAuthorizer authorizer, IShapeFactory shapeFactory, IEnumerable<IRouteProvider> routeProviders)
        {
            _workContext = workContext;
            _shapeFactory = shapeFactory;
            _authorizer = authorizer;
            _routeProviders = routeProviders;
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        private bool IsActivable()
        {
            // activate on front-end only
            if (AdminFilter.IsApplied(new RequestContext(_workContext.HttpContext, new RouteData())))
            {
                return false;
            }

            // if not logged as a site owner, still activate if it's a local request (development machine)
            if (!_authorizer.Authorize(StandardPermissions.SiteOwner))
            {
                return _workContext.HttpContext.Request.IsLocal;
            }

            return true;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
            {
                return;
            }

            if (!this.IsActivable())
            {
                return;
            }

            var placement = _workContext.Layout.Head;
            var shape = _shapeFactory.RouteView();
            shape.Routes = RouteHelper.GetRoutes(_routeProviders);
            placement.Add(shape);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }
}