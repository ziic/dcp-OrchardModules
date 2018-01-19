using System;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace dcp.Utility.Projection.Providers.SortCriteria
{
    public class QueryParamFieldSortCriterionFormProvider : IFormProvider
    {
        public const string FormName = "QueryParamFieldSortCriterionForm";

        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public QueryParamFieldSortCriterionFormProvider(IShapeFactory shapeFactory)
        {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context)
        {
            Func<IShapeFactory, object> form =
                shape =>
                {

                    var f = Shape.Form(
                        _Options: Shape.Fieldset(
                            _DefaultPartName: Shape.TextBox(
                                Id: "DefaultPartName",
                                Name: "DefaultPartName",
                                Title: T("Default Part name")
                            ),
                            _DefaultSortBy: Shape.TextBox(
                                Id: "DefaultSortBy", 
                                Name: "DefaultSortBy",
                                Title: T("Default sort by")
                                ),
                            _DefaultSortAscTrue: Shape.Radio(
                                Id: "DefaultSortAsc", 
                                Name: "DefaultSortAsc",
                                Checked: true, // default value
                                Title: T("Default sort ascending"), 
                                Value: "true"
                            ),
                            _DefaultSortAscFalse: Shape.Radio(
                                Id: "DefaultSortDesc", Name: "DefaultSortAsc",
                                Title: T("Default sort descending"), Value: "false"
                            ),
                            Description: T("Default sorting")
                        ));

                    return f;
                };

            context.Form(FormName, form);

        }
    }
}