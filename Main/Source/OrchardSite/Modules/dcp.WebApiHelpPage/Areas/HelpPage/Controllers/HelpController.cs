using System;
using System.Web.Http;
using System.Web.Mvc;
using dcp.WebApiHelpPage.Areas.HelpPage.ModelDescriptions;
using dcp.WebApiHelpPage.Areas.HelpPage.Models;
using System.Collections.Generic;
using Orchard.WebApi.Routes;
using System.Linq;
using Orchard.Mvc.Routes;

namespace dcp.WebApiHelpPage.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";
        private readonly IEnumerable<HttpRouteDescriptor> _routes;

        public HelpController(IEnumerable<IHttpRouteProvider> routeProviders)
            : this(HelpPageAreaRegistration.Configuration)
        {
            _routes = routeProviders.SelectMany(x => x.GetRoutes()).Cast<HttpRouteDescriptor>().ToList();
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();

            //var apiDescriptions = Configuration.Services.GetApiExplorer().ApiDescriptions;
            //return View(apiDescriptions);
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}