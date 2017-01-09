using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;

namespace dcp.Utility
{
    public class SourcePartDefinition
    {
        public string ContentItemName { get; set; }
        public Type PartType { get; set; }
    }

    public class SourceFieldDefinition
    {
        public SourcePartDefinition SourcePart { get; set; }
        public Type FieldType { get; set; }
        public string FieldName { get; set; }
    }


    public interface IViewModelContentItemBuilder : IDependency
    {
        ContentItem Build(string viewModelContentItemName, IEnumerable<SourcePartDefinition> sourceParts, IEnumerable<SourceFieldDefinition> sourceFields);
        ContentItem Build(string viewModelContentItemName, IEnumerable<SourceFieldDefinition> fields);
        ContentItem Build(string viewModelContentItemName, IEnumerable<SourcePartDefinition> parts);
    }

    public class ViewModelContentItemBuilder : IViewModelContentItemBuilder
    {
        private readonly IOrchardServices _orchardServices;

        public ViewModelContentItemBuilder(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        public ContentItem Build(string viewModelContentItemName, IEnumerable<SourcePartDefinition> parts)
        {
            return Build(viewModelContentItemName, parts, null);
        }

        public ContentItem Build(string viewModelContentItemName, IEnumerable<SourceFieldDefinition> fields)
        {
            return Build(viewModelContentItemName, null, fields);
        }

        public ContentItem Build(string viewModelContentItemName, IEnumerable<SourcePartDefinition> parts, IEnumerable<SourceFieldDefinition> fields)
        {
            var viewModelContentItem = _orchardServices.ContentManager.New(viewModelContentItemName);

            if (parts != null)
            {
                foreach (var partContext in parts)
                {
                    var contentItem = _orchardServices.ContentManager.New(partContext.ContentItemName);

                    var part = contentItem.Get(partContext.PartType) as ContentPart;

                    if (part != null)
                        viewModelContentItem.Weld(part);
                }
            }

            if (fields != null)
            {
                foreach (var fieldContext in fields)
                {
                    var contentItem = _orchardServices.ContentManager.New(fieldContext.SourcePart.ContentItemName);

                    var partSource = contentItem.Get(fieldContext.SourcePart.PartType) as ContentPart;
                    if (partSource == null)
                    {
                        continue;
                    }

                    var field = partSource.Fields.SingleOrDefault(x => x.GetType() == fieldContext.FieldType && x.Name == fieldContext.FieldName);

                    if (field == null)
                    {
                        continue;
                    }

                    var part = new ContentPart();
                    var fieldsDefinitions = new List<ContentPartFieldDefinition>
                    {
                        field.PartFieldDefinition
                    };
                    var contentPartDefinition = new ContentPartDefinition(partSource.PartDefinition.Name, fieldsDefinitions, new SettingsDictionary());
                    var contentTypePartDefinition = new ContentTypePartDefinition(contentPartDefinition, new SettingsDictionary());
                    part.TypePartDefinition = contentTypePartDefinition;
                    part.Weld(field);

                    viewModelContentItem.Weld(part);
                }
            }

            return viewModelContentItem;
        }
    }
}