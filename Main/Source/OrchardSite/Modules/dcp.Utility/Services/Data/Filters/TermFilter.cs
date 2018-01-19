using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Taxonomies.Models;

namespace dcp.Utility.Services.Data.Filters
{
    //public abstract class TermFilter : IHqlQueryFilter
    //{
    //    private readonly int _termId;
    //    private readonly string _field;

    //    protected TermFilter(string field, int termId)
    //    {
    //        _termId = termId;
    //        _field = field;
    //    }

    //    public IHqlQuery ExecuteFilter(IHqlQuery query)
    //    {
    //        query = query.Where(a => a.ContentPartRecord<TermsPartRecord>()
    //                    .Property("Terms", "terms"),
    //                p => p.Eq("Field", _field))
    //            .Where(a => a.ContentPartRecord<TermsPartRecord>()
    //                    .Property("Terms", "terms")
    //                    .Property("TermRecord", "termRecord"),
    //                p => p.Eq("Id", _termId));

    //        return query;
    //    }
    //}

    public class TermFilter : IHqlQueryFilter
    {
        private readonly IEnumerable<int> _termsIds;
        private readonly string _multiplyOperator;
        private int _termsFilterId;
        private readonly string _fieldName;

        public TermFilter(string fieldName, int termId)
            :this(fieldName, new[] { termId}, "AND")
        {
            
        }

        public TermFilter(string fieldName, IEnumerable<int> termsIds)
            :this(fieldName, termsIds, "AND")
        {
            
        }

        public TermFilter(string fieldName, IEnumerable<int> termsIds, string multiplyOperator, int termsFilterId = 0)
        {
            _termsIds = termsIds;
            _multiplyOperator = multiplyOperator;
            _termsFilterId = termsFilterId;
            _fieldName = fieldName;
        }

        public IHqlQuery ExecuteFilter(IHqlQuery query)
        {
            const string termsPrefix = "Terms";

            if (!_termsIds.Any())
            {
                return query;
            }

            query.Where(a => a.ContentPartRecord<TermsPartRecord>()
                .Property("Terms", termsPrefix + _termsFilterId), p => p.Eq("Field", _fieldName));

            Action<IAliasFactory> selector = alias => alias.ContentPartRecord<TermsPartRecord>()
                .Property("Terms", termsPrefix + _termsFilterId)
                .Property("TermRecord", "termRecord" + _termsFilterId);

            var multiplyOperator = _multiplyOperator;
            if (string.IsNullOrEmpty(multiplyOperator))
            {
                multiplyOperator = "OR";
            }

            Action<IHqlExpressionFactory> filter = null;
            foreach (var termId in _termsIds)
            {
                var termFullPath = "/" + termId + "/";

                Action<IHqlExpressionFactory> currentFilter = x => x.Or(lhs => lhs.Eq("Id", termId), rhs => rhs.Like("Path", termFullPath, HqlMatchMode.Start));
                if (filter == null)
                {
                    filter = currentFilter;
                }
                else
                {
                    var oldFilter = (Action<IHqlExpressionFactory>) filter.Clone();
                    if (multiplyOperator == "AND")
                    {
                        filter = x => { x.And(oldFilter, currentFilter); };
                    }
                    else if (multiplyOperator == "OR")
                    {
                        filter = x => { x.Or(oldFilter, currentFilter); };
                    }
                    else
                        throw new NotSupportedException("MultiplyOperator");
                }
            }

            query.Where(selector, filter);

            _termsFilterId++;

            return query;
        }
    }
}