﻿using Orchard.UI.Resources;

namespace PJS.Bootstrap {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineScript("Bootstrap").SetUrl("bootstrap-3.3.7/js/bootstrap.min.js", "bootstrap-3.3.7/js/bootstrap.js").SetVersion("3.3.7").SetDependencies("jQuery");
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
