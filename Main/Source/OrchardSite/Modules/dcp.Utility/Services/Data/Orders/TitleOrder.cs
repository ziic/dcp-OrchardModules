using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace dcp.Utility.Services.Data.Orders
{
    public class TitleOrder : IHqlQueryOrder
    {
        private readonly bool _desc;

        public TitleOrder(bool desc = false)
        {
            _desc = desc;
        }

        public IHqlQuery ExecuteOrderAsc(IHqlQuery query)
        {
            if (!_desc)
                query = query.OrderBy(a => a.ContentPartRecord<TitlePartRecord>(), o => o.Asc("Title"));
            else
                query = query.OrderBy(a => a.ContentPartRecord<TitlePartRecord>(), o => o.Desc("Title"));

            return query;
        }
    }    
}