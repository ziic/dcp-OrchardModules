using Orchard.Localization;
using Orchard.UI.Navigation;

namespace dcp.Routing
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder

                // Image set
                .AddImageSet("dcp.Routing")

                // "Webshop"
                .Add(item => item

                    .Caption(T("Routing"))
                    .Position("7")
                    .LinkToFirstChild(false)

                    .Add(subItem => subItem
                        .Caption(T("Redirects"))
                        .Action("List", "Admin", new { area = "dcp.Routing" })
                        .Add(x => x
                            .Caption(T("List"))
                            .Action("List", "Admin", new { area = "dcp.Routing" })
                            .LocalNav())
                    )
                );
        }
    }
}