using Orchard.UI.Resources;

namespace dcp.jQueryValidate
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // jQuery Validate scripts
            manifest.DefineScript("jQueryValidate")
                .SetUrl("jquery.validate.min.js", "jquery.validate.js")
                .SetVersion("1.16.0")
                .SetDependencies("jQuery")
                .SetCdn("//ajax.aspnetcdn.com/ajax/jquery.validate/1.16.0/jquery.validate.min.js", "//ajax.aspnetcdn.com/ajax/jquery.validate/1.16.0/jquery.validate.js", true);

            // Additional Methods
            manifest.DefineScript("jQueryValidate_AdditionalMethods")
                .SetUrl("additional-methods.min.js", "additional-methods.js")
                .SetVersion("1.16.0")
                .SetDependencies("jQueryValidate")
                .SetCdn("//ajax.aspnetcdn.com/ajax/jquery.validate/1.16.0/additional-methods.min.js", "//ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/additional-methods.js", true);

            manifest.DefineScript("jquery.validate.unobtrusive")
                .SetUrl("jquery.validate.unobtrusive.js", "jquery.validate.unobtrusive.min.js")
                .SetVersion("5.2.3")
                .SetDependencies("jQuery", "jQueryValidate")
                .SetCdn("//ajax.aspnetcdn.com/ajax/mvc/5.2.3/jquery.validate.unobtrusive.min.js", "//ajax.aspnetcdn.com/ajax/mvc/5.2.3/jquery.validate.unobtrusive.js", true); ;

        }
    }
}