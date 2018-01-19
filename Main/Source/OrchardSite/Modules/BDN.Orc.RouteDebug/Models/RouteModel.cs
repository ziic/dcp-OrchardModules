using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDN.Orc.RouteDebug.Models
{
    public class RouteModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Defaults { get; set; }
        public int Priority { get; set; }
        public string Feature { get; set; }
        public System.Web.Routing.Route  RouteNode { get; set; }
    }
}