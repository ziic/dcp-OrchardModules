using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.UI.Navigation;
using Orchard.Localization;
using Orchard.Core.Contents;

namespace BDN.Orc.NLog
{
    public class Menus : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("navigation")
                .Add(T("Routes"), "99", item => item.Action("Index", "Admin", new { area = "BDN.Orc.RouteDebug" }));
        }
    }
}