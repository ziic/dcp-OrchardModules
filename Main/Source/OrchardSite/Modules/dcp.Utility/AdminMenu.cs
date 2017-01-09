using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace dcp.Utility
{
    [OrchardFeature("dcp.Routing")]
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Aliases"), "1.4.1", menu =>
            {
                menu.Add(T("Extended"), "3", item => item.Action("List", "ExtendedAlias", new {area = "dcp.Utility"}).LocalNav().Permission(StandardPermissions.SiteOwner));
            });
        }

        public string MenuName
        {
            get { return "admin"; }
        }
    }
}



