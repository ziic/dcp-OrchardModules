using Orchard.ContentManagement;
using Orchard.Taxonomies.Models;

namespace dcp.Utility.Services.Data.Filters
{
    public abstract class TermFilter : IHqlQueryFilter
    {
        private readonly int _termId;
        private readonly string _field;

        protected TermFilter(string field, int termId)
        {
            _termId = termId;
            _field = field;
        }

        public IHqlQuery ExecuteFilter(IHqlQuery query)
        {
            query = query.Where(a => a.ContentPartRecord<TermsPartRecord>()
                    .Property("Terms", "terms"),
                    p => p.Eq("Field", _field))
                    .Where(a => a.ContentPartRecord<TermsPartRecord>()
                        .Property("Terms", "terms")
                        .Property("TermRecord", "termRecord"),
                        p => p.Eq("Id", _termId));

            return query;
        }
    }
}