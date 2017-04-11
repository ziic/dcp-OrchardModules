using dcp.Meta.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Tokens;
using Orchard.UI.Resources;

namespace dcp.Meta.Drivers 
{
    public class MetaDriver : ContentPartDriver<MetaPart> 
    {
        private readonly IWorkContextAccessor _wca;
        private readonly ITokenizer _tokenizer;

        public MetaDriver(IWorkContextAccessor workContextAccessor, ITokenizer tokenizer)
        {
            _wca = workContextAccessor;
             _tokenizer = tokenizer;
        }

        protected override DriverResult Display(MetaPart part, string displayType, dynamic shapeHelper) 
        {
            if (displayType != "Detail") return null;
            var resourceManager = _wca.GetContext().Resolve<IResourceManager>();
            if (!string.IsNullOrWhiteSpace(part.Description)) 
            {
                resourceManager.SetMeta(new MetaEntry 
                {
                    Name = "description",
                    Content = _tokenizer.Replace(part.Description, new {Content = part.ContentItem})
                });
            }
            if (!string.IsNullOrWhiteSpace(part.Keywords)) 
            {
                resourceManager.SetMeta(new MetaEntry 
                {
                    Name = "keywords",
                    Content = _tokenizer.Replace(part.Keywords, new { Content = part.ContentItem })
                });
            }
            return null;
        }

        //GET
        protected override DriverResult Editor(MetaPart part, dynamic shapeHelper) 
        {

            return ContentShape("Parts_Meta_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Meta",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(MetaPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
        protected override void Exporting(MetaPart part, ExportContentContext context)
        {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Keywords", part.Record.Keywords);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Description", part.Record.Description);
        }

        protected override void Importing(MetaPart part, ImportContentContext context)
        {
            part.Record.Keywords = context.Attribute(part.PartDefinition.Name, "Keywords");
            part.Record.Description = context.Attribute(part.PartDefinition.Name, "Description");
        }
    }
}