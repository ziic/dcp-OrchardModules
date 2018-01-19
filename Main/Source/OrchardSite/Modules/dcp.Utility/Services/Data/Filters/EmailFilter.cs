using Orchard.ContentManagement;
using Orchard.Users.Models;

namespace dcp.Utility.Services.Data.Filters
{
    public class EmailFilter : IHqlQueryFilter
    {
        private readonly string _email;

        public EmailFilter(string email)
        {
            _email = email;
        }

        public IHqlQuery ExecuteFilter(IHqlQuery query)
        {
            query = query.Where(a => a.ContentPartRecord<UserPartRecord>(), p => p.Like("Email", _email, HqlMatchMode.Anywhere));

            return query;
        }
    }
}