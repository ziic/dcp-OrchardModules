using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace dcp.Utility.Services.Data.Filters
{
    public class TitleFilter : IHqlQueryFilter
    {
        private readonly string _title;

        public TitleFilter(string title)
        {
            _title = title;
        }

        public IHqlQuery ExecuteFilter(IHqlQuery query)
        {
            query = query.Where(a => a.ContentPartRecord<TitlePartRecord>(), p => p.Like("Title", _title, HqlMatchMode.Anywhere));

            return query;
        }
    }
}