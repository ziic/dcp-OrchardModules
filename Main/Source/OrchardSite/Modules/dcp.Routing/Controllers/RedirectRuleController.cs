using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using dcp.Routing.Models;
using dcp.Routing.Services;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace dcp.Routing.Controllers
{
    [OrchardFeature("dcp.Routing.Redirects")]
    [Admin]
    public class RedirectRuleController : Controller
    {
        private readonly IRoutingAppService _routingAppService;
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        
        private readonly string[] _includeProperties = {"SourceUrl", "DestinationUrl", "IsPermanent"};

        public Localizer T { get; set; }

        public RedirectRuleController(IRoutingAppService routingAppService, IOrchardServices orchardServices, ISiteService siteService, IWebConfigService webConfigService)
        {
            _routingAppService = routingAppService;
            _orchardServices = orchardServices;
            _siteService = siteService;
        }

        [HttpGet]
        public ActionResult List(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var totalCount = _routingAppService.GetRedirectsTotalCount();
            var pagerShape = _orchardServices.New.Pager(pager).TotalItemCount(totalCount);
            if (pager.PageSize == 0)
                pager.PageSize = totalCount;
            var items = _routingAppService.GetRedirects(pager.GetStartIndex(), pager.PageSize);
            return View(new
            {
                Items = items,
                PagerShape = pagerShape
            }.ToExpando());
        }

        [HttpPost]
        public ActionResult List(PagerParameters pagerParameters, string bulkAction, int[] itemIds)
        {
            if (string.IsNullOrEmpty(bulkAction) || bulkAction == "None")
                return List(pagerParameters);

            if (bulkAction == "Move" && itemIds.Any())
            {
                //return Redirect("Move?" + itemIds.Aggregate(string.Empty, (a, x) => a += "itemIds=" + x + "&").TrimEnd('&'));
                TempData["itemIds"] = itemIds;
                return Redirect("Move");
            }

            return List(pagerParameters);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var redirect = _routingAppService.GetRedirect(id);
            if (redirect == null)
                return HttpNotFound();

            return View(redirect);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            var redirect = _routingAppService.GetRedirect(id);
            if (redirect == null)
                return HttpNotFound();

            if (!TryUpdateModel(redirect, _includeProperties))
            {
                _orchardServices.TransactionManager.Cancel();
                return View(redirect);
            }

            if (redirect.SourceUrl == redirect.DestinationUrl)
            {
                ModelState.AddModelError("SourceUrl", "Source url is equal to Destination url");
                _orchardServices.TransactionManager.Cancel();
                return View(redirect);
            }

            var isError = false;
            try
            {
            _routingAppService.Update(redirect);
            }
            catch (ApplicationException e)
            {
                isError = true;
                _orchardServices.Notifier.Add(NotifyType.Error, T(e.Message));
            }

            if (isError)
            {
                return RedirectToAction("Edit", new { id });
            }

            _orchardServices.Notifier.Add(NotifyType.Information, T("Redirect record was saved"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var redirect = _routingAppService.GetRedirect(id);
            if (redirect == null)
                return HttpNotFound();

            _routingAppService.Delete(redirect);

            _orchardServices.Notifier.Add(NotifyType.Information, T("Redirect record was deleted"));

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(FormCollection formCollection)
        {
            var redirect = new RedirectRule();

            if (!TryUpdateModel(redirect, _includeProperties))
            {
                _orchardServices.TransactionManager.Cancel();
                return View(redirect);
            }

            if (redirect.SourceUrl == redirect.DestinationUrl)
            {
                ModelState.AddModelError("SourceUrl", "Source url is equal to Destination url");
                _orchardServices.TransactionManager.Cancel();
                return View(redirect);
            }

            var isError = false;
            try
            {
            _routingAppService.Add(redirect);
            }
            catch (ApplicationException e)
            {
                isError = true;
                _orchardServices.Notifier.Add(NotifyType.Error, T(e.Message));
            }

            if (isError)
            {
                return RedirectToAction("Add");
            }

            _orchardServices.Notifier.Add(NotifyType.Information, T("Redirect record was added"));

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Move(int[] itemIds)
        {
            if (itemIds == null)
            {
                itemIds = GetItemIds();
            }
            if (itemIds == null || !itemIds.Any())
                return RedirectToAction("List");
            var redirectRules = _routingAppService.GetRedirects(itemIds);
            TempData["itemIds"] = itemIds;

            var pager = new Pager(_siteService.GetSiteSettings(), 0, itemIds.Length);
            var pagerShape = _orchardServices.New.Pager(pager).TotalItemCount(itemIds.Length);
            return View(new 
            {
                Items = redirectRules,
                PagerShape = pagerShape
            }.ToExpando());
        }
        
        [HttpPost]
        public ActionResult Move(string bulkAction, int[] itemIds)
        {
            if (itemIds == null)
            {
                itemIds = GetItemIds();
            }

            if (itemIds == null || !itemIds.Any())
                return RedirectToAction("List");
            if (string.IsNullOrEmpty(bulkAction) || bulkAction == "None")
                return Move(itemIds);

            if (bulkAction == "Move" && itemIds.Any())
            {
                var filePath = Server.MapPath("~/Web.config");

                var res = _routingAppService.MoveRedirectRulesToWebConfig(itemIds, filePath);
                if (res)
                    _orchardServices.Notifier.Add(NotifyType.Information, T("Redirect rules were moved to web.config"));
                else
                    _orchardServices.Notifier.Add(NotifyType.Warning, T("Redirect rules were NOT moved to web.config. Maybe you have not enabled IIS Url rewrite module"));
            }

            return RedirectToAction("List");
        }
        private int[] GetItemIds()
        {
            int[] itemIds = null;
            if (TempData["itemIds"] != null)
            {
                itemIds = (int[])TempData["itemIds"];
            }
            else
            {
                var itemIdsStr = ControllerContext.RequestContext.HttpContext.Request.QueryString["itemIds"];
                if (!string.IsNullOrWhiteSpace(itemIdsStr))
                {

                    try
                    {
                        itemIds = itemIdsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    }
                    catch
                    {

                    }
                }
            }
            return itemIds;
        }
    }

    public static class ViewModelHelper
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}