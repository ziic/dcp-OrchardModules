using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;

namespace dcp.Utility.Projection.Providers.SortCriteria
{
    public class QueryParamFieldSortCriterionProvider : ISortCriterionProvider
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;
        private readonly IEnumerable<IFieldTypeEditor> _fieldTypeEditors;
        private readonly IWorkContextAccessor _workContextAccessor;

        public QueryParamFieldSortCriterionProvider(
            IContentDefinitionManager contentDefinitionManager,
            IEnumerable<IContentFieldDriver> contentFieldDrivers,
            IEnumerable<IFieldTypeEditor> fieldTypeEditors,
            IWorkContextAccessor workContextAccessor)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentFieldDrivers = contentFieldDrivers;
            _fieldTypeEditors = fieldTypeEditors;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeSortCriterionContext describe)
        {
            var descriptor = describe.For("General", T("General"), T("General sort criteria"));
            descriptor.Element("QueryParamField", T("Query Param Field"),
                               T("Sorts results using query parameters on the HTTP request"),
                               context => ApplyFilter(context),
                               context => DisplaySortCriterion(context),
                               form: QueryParamFieldSortCriterionFormProvider.FormName
                );
        }

        public void ApplyFilter(SortCriterionContext context)
        {
            var request = _workContextAccessor.GetContext().HttpContext.Request;
            var sortField = request.Params["sortby"];

            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = (string)context.State.DefaultSortBy;
                if (string.IsNullOrWhiteSpace(sortField))
                    return;
            }
            
            var ascending = true;
            var defaultSortAscStr = (string)context.State.DefaultSortAsc;
            if (!string.IsNullOrWhiteSpace(defaultSortAscStr))
            {
                bool.TryParse(defaultSortAscStr, out ascending);
            }
            
            var sortasc = request.Params["sortasc"];
            if (!string.IsNullOrWhiteSpace(sortasc))
            {
                bool.TryParse(sortasc, out ascending);
            }
            

            var sortFieldElements = sortField.Split(new[] { '.' }, 2);
            var partName = string.Empty;
            var fieldName = string.Empty;
            if (sortFieldElements.Length != 2)
            {
                if (sortFieldElements.Length == 1)
                {
                    partName = (string) context.State.DefaultPartName;
                    if (string.IsNullOrWhiteSpace(partName))
                        return;

                    fieldName = sortFieldElements[0];
                }
            }
            else
            {
                partName = sortFieldElements[0];
                fieldName = sortFieldElements[1];
            }

            var part = _contentDefinitionManager.ListPartDefinitions().FirstOrDefault(p => p.Name == partName);

            if (part == null)
            {
                return;
            }

            var field = part.Fields.FirstOrDefault(f => f.Name == fieldName);

            if (field == null)
            {
                return;
            }

            var drivers = _contentFieldDrivers.Where(x => x.GetFieldInfo().Any(fi => fi.FieldTypeName == field.FieldDefinition.Name)).ToList();

            var membersContext = new DescribeMembersContext(
                (storageName, storageType, displayName, description) => {
                    // look for a compatible field type editor
                    IFieldTypeEditor fieldTypeEditor = _fieldTypeEditors.FirstOrDefault(x => x.CanHandle(storageType));

                    if (fieldTypeEditor != null)
                    {

                        var propertyName = String.Join(".", part.Name, field.Name, storageName ?? "");

                        // use an alias with the join so that two filters on the same Field Type wont collide
                        var relationship = fieldTypeEditor.GetFilterRelationship(propertyName.ToSafeName());

                        // generate the predicate based on the editor which has been used
                        Action<IHqlExpressionFactory> predicate = y => y.Eq("PropertyName", propertyName);

                        // combines the predicate with a filter on the specific property name of the storage, as implemented in FieldIndexService

                        // apply where clause
                        context.Query = context.Query.Where(relationship, predicate);

                        // apply sort
                        context.Query = ascending
                            ? context.Query.OrderBy(relationship, x => x.Asc("Value"))
                            : context.Query.OrderBy(relationship, x => x.Desc("Value"));
                    }
                });

            foreach (var driver in drivers)
            {
                driver.Describe(membersContext);
            }

        }

        public LocalizedString DisplaySortCriterion(SortCriterionContext context)
        {
            return T("Ordered by fields specified by HTTP query parameters (eg. ?sortby=PartName.FieldName&sortasc=true)");
        }
    }

}