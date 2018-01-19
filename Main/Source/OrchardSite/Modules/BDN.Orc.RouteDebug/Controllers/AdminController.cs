using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.Mvc.Routes;
using BDN.Orc.RouteDebug.ViewModels;
using BDN.Orc.RouteDebug.Core;
using System.Linq;

namespace BDN.Orc.RouteDebug.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEnumerable<IRouteProvider> _routeProviders;
        public AdminController(IEnumerable<IRouteProvider> routeProviders)
        {
            _routeProviders = routeProviders;
        }

        public ActionResult Index(string filter)
        {
            var routes = RouteHelper.GetRoutes(_routeProviders);

            if (!string.IsNullOrEmpty(filter))
            {
                routes = routes.Where(x => x.Url.ToLower().Contains(filter.ToLower()))
                    .Union(routes.Where(x=>x.Feature.ToLower().Contains(filter.ToLower())))
                    .Union(routes.Where(x => x.Defaults.ToLower().Contains(filter.ToLower())))
                    .Distinct()
                    .ToList();
            }

            return View(new RoutesIndexViewModel() { Routes = routes, Filter = filter });
        }
    }
}
