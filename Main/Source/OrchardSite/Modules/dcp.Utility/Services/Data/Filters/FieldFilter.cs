using System;
using Orchard.ContentManagement;
using Orchard.Projections.FieldTypeEditors;

namespace dcp.Utility.Services.Data.Filters
{
    public class FieldFilter<T> : IHqlQueryFilter
    {
        private readonly string _contentTypeName;
        private readonly string _fieldName;
        private readonly T _value;

        public FieldFilter(string contentTypeName, string fieldName, T value)
        {
            _contentTypeName = contentTypeName;
            _fieldName = fieldName;
            _value = value;
        }

        public IHqlQuery ExecuteFilter(IHqlQuery query)
        {
            var t = typeof(T);
            Action<IAliasFactory> alias;
            if (t == typeof(string))
            {
                alias = new StringFieldTypeEditor().GetFilterRelationship(_fieldName);
            }
            else if (t == typeof(int))
            {
                alias = new IntegerFieldTypeEditor().GetFilterRelationship(_fieldName);
            }
            else if (t == typeof(double))
            {
                alias = new FloatFieldTypeEditor().GetFilterRelationship(_fieldName);
            }
            else if (t == typeof(decimal))
            {
                alias = new DecimalFieldTypeEditor().GetFilterRelationship(_fieldName);
            }
            else if (t == typeof(bool))
            {
                alias = new BooleanFieldTypeEditor().GetFilterRelationship(_fieldName);
            }
            else
                throw new NotSupportedException();
            

            query = query.Where(
                alias,
                p => p.And(
                    l => l.Eq("PropertyName", string.Format("{0}.{1}.", _contentTypeName, _fieldName)),
                    r => r.Eq("Value", _value)));

            return query;
        }
    }
}