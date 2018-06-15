using dcp.BootstrapTheme.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace dcp.BootstrapTheme.Handlers {

    public class BootstrapThemeSettingsPartHandler : ContentHandler
    {
        public BootstrapThemeSettingsPartHandler()
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<dcpBootstrapThemeSettingsPart>("Site"));
            Filters.Add(new TemplateFilterForPart<dcpBootstrapThemeSettingsPart>("BootstrapThemeSettings", "Parts/dcpBootstrapThemeSettings", "ThemeBootstrap"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("ThemeBootstrap")));
        }
    }
}