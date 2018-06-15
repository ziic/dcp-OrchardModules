using Orchard.UI.Resources;

namespace dcp.BootstrapTheme
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("Popper")
                .SetUrl("popper/umd/popper.min.js", "popper/umd/popper.js")
                .SetVersion("1.14.3");
            manifest.DefineScript("Bootstrap").SetUrl("bootstrap-4.1.1/js/bootstrap.min.js", "bootstrap-4.1.1/js/bootstrap.js").SetVersion("4.1.1").SetDependencies("jQuery", "Popper");
            manifest.DefineScript("HoverDropdown").SetUrl("hover-dropdown.js").SetDependencies("Bootstrap");
            manifest.DefineScript("Stapel-Modernizr").SetUrl("stapel/modernizr.custom.63321.js");
            manifest.DefineScript("Stapel").SetUrl("stapel/jquery.stapel.js").SetDependencies("jQuery", "Stapel-Modernizr");
            manifest.DefineScript("prettyPhoto").SetUrl("prettyPhoto/jquery.prettyPhoto.js").SetDependencies("jQuery");
            manifest.DefineScript("Custom").SetUrl("custom.js").SetDependencies("jQuery");
            manifest.DefineScript("OrchardTinyMce")
                .SetVersion("1000.1000.1000") //override
                .SetUrl("orchard-tinymce.js").SetDependencies("TinyMce");

            manifest.DefineStyle("Stapel").SetUrl("stapel/stapel.css");
            manifest.DefineStyle("prettyPhoto").SetUrl("prettyPhoto/prettyPhoto.css");

        }
    }
}
