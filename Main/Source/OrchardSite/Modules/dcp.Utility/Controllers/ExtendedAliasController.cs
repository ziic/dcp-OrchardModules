using Orchard;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Admin;
using System.Web;
using System.Web.Mvc;
using dcp.Utility.Routing.Services;
using Orchard.Environment.Extensions;

namespace dcp.Utility.Controllers
{
    [OrchardFeature("dcp.Routing")]
    [Admin]
    public class ExtendedAliasController : Controller
    {
        private readonly IExtendedAliasService _extendedAliasService;
        private readonly IOrchardServices _orchardServices;

        public Localizer T { get; set; }

        public ExtendedAliasController(IExtendedAliasService extendedAliasService, IOrchardServices orchardServices)
        {
            _extendedAliasService = extendedAliasService;
            _orchardServices = orchardServices;
        }
        
        [HttpPost]
        public ActionResult Restart()
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage extended aliases")))
                return new HttpUnauthorizedResult();
            
            HttpRuntime.UnloadAppDomain();
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List(string restart = null)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage extended aliases")))
                return new HttpUnauthorizedResult();

            if (!string.IsNullOrEmpty(restart))
            {
                HttpRuntime.UnloadAppDomain();
                return RedirectToAction("List");
            }

            var records = _extendedAliasService.GetAll();
            return View(records);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage extended aliases")))
                return new HttpUnauthorizedResult();

            var record = _extendedAliasService.Get(id);

            return View(record);
        }

        [HttpPost]
        public ActionResult Edit(int id, string routeName)
        {
            if (!_orchardServices.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage extended aliases")))
                return new HttpUnauthorizedResult();

            _extendedAliasService.Update(id, routeName);

            return RedirectToAction("List");
        }
    }
}