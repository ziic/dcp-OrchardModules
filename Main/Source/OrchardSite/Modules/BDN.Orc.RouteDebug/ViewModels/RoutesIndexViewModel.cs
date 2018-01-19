using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using BDN.Orc.RouteDebug.Models;

namespace BDN.Orc.RouteDebug.ViewModels
{
    public class RoutesIndexViewModel
    {
        public IList<RouteModel> Routes { get; set; }
        public string Filter { get; set; }
    }
}